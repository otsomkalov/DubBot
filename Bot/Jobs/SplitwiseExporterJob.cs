using Bot.Extensions;
using Microsoft.Extensions.Options;
using Quartz;
using Splitwise.Clients.Interfaces;
using Splitwise.Requests.Expense;

namespace Bot.Jobs;

[DisallowConcurrentExecution]
public class SplitwiseExporterJob : IJob
{
    private readonly ILogger<SplitwiseExporterJob> _logger;
    private readonly ISplitwiseClient _splitwiseClient;
    private readonly TakeoutService _takeoutService;
    private readonly TelegramSettings _telegramSettings;
    private readonly UserService _userService;

    public SplitwiseExporterJob(ISplitwiseClient splitwiseClient, UserService userService, TakeoutService takeoutService,
        ILogger<SplitwiseExporterJob> logger, IOptions<TelegramSettings> telegramSettings)
    {
        _splitwiseClient = splitwiseClient;
        _userService = userService;
        _takeoutService = takeoutService;
        _logger = logger;
        _telegramSettings = telegramSettings.Value;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var sundayDate = DateTime.Now.GetLastSunday();

        var users = await _userService.ListAsync();

        var payments = new List<PaymentRequest>();

        var cost = .0M;

        foreach (var user in users)
        {
            var takeoutsSinceSunday = await _takeoutService.ListSinceAsync(user.Id, sundayDate);

            var userOwe = .0M;

            foreach (var takeout in takeoutsSinceSunday)
            {
                var pricePerGram = takeout.Order.Price / takeout.Order.Amount;

                userOwe += pricePerGram * takeout.Amount;
            }

            if (userOwe == decimal.Zero)
            {
                continue;
            }

            payments.Add(new()
            {
                UserId = user.SplitwiseId,
                OwedShare = userOwe
            });

            cost += userOwe;
        }

        if (cost == decimal.Zero)
        {
            return;
        }

        var adminUser = users.Single(u => u.Id == _telegramSettings.AdminId);

        payments.Add(new()
        {
            UserId = adminUser.SplitwiseId,
            PaidShare = cost
        });

        var createExpenseRequest = new UpsertExpenseRequest
        {
            Description = $"Дуб {sundayDate.Date:d}-{DateTime.Now.Date:d}",
            CategoryId = 23,
            CurrencyCode = "UAH",
            Date = DateTime.Now,
            Payments = payments,
            Cost = cost
        };

        try
        {
            var result = await _splitwiseClient.Expense.CreateAsync(createExpenseRequest);

            if (result.IsFailed)
            {
                _logger.LogError("There are errors during creating expense: {Errors}",
                    string.Join('\n', result.Errors.Select(e => e.Message)));
            }
            else
            {
                if (result.Value.Errors?.Base?.Any() == true)
                {
                    _logger.LogError("There are errors during creating expense: {Errors}", string.Join('\n', result.Value.Errors.Base));
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during creating expense:");
        }
    }
}
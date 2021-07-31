using System;
using System.Linq;
using System.Threading.Tasks;
using Bot.Extensions;
using Bot.Services;
using Microsoft.Extensions.Logging;
using Quartz;
using Splitwise.Clients.Interfaces;
using Splitwise.Requests.Expense;

namespace Bot.Jobs
{
    [DisallowConcurrentExecution]
    public class SplitwiseExporterJob : IJob
    {
        private readonly ILogger<SplitwiseExporterJob> _logger;
        private readonly ISplitwiseClient _splitwiseClient;
        private readonly TakeoutService _takeoutService;
        private readonly UserService _userService;

        public SplitwiseExporterJob(ISplitwiseClient splitwiseClient, UserService userService, TakeoutService takeoutService,
            ILogger<SplitwiseExporterJob> logger)
        {
            _splitwiseClient = splitwiseClient;
            _userService = userService;
            _takeoutService = takeoutService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var sundayDate = DateTime.Now.GetLastSunday();

            var users = await _userService.ListAsync();

            var createExpenseRequest = new UpsertExpenseRequest
            {
                Description = $"Дуб {sundayDate.Date}-{DateTime.Now.Date}",
                CategoryId = 23,
                CurrencyCode = "UAH",
                Date = DateTime.Now
            };

            var cost = .0M;

            foreach (var user in users)
            {
                var takeoutsSinceSunday = await _takeoutService.ListSinceAsync(user.Id, sundayDate);

                var userOwe = .0M;

                foreach (var takeout in takeoutsSinceSunday)
                {
                    var pricePerGram = takeout.Order.Amount / takeout.Order.Price;

                    userOwe += pricePerGram * takeout.Amount;
                }

                createExpenseRequest.Payments.Add(new()
                {
                    UserId = user.SplitwiseId,
                    OwedShare = userOwe
                });

                cost += userOwe;
            }

            createExpenseRequest.Cost = cost;

            try
            {
                var result = await _splitwiseClient.Expense.CreateAsync(createExpenseRequest);

                if (result.IsFailed)
                {
                    _logger.LogError(string.Join('\n', result.Errors.Select(e => e.Message)));
                }
                else
                {
                    if (result.Value.Errors?.Base?.Any() == true)
                    {
                        _logger.LogError(string.Join('\n', result.Value.Errors.Base));
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during creating expense:");
            }
        }
    }
}
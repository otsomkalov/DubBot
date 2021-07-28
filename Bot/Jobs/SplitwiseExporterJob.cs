using System;
using System.Threading.Tasks;
using Bot.Extensions;
using Bot.Services;
using Quartz;
using Splitwise.Clients;
using Splitwise.Clients.Interfaces;
using Splitwise.Requests.Expense;

namespace Bot.Jobs
{
    [DisallowConcurrentExecution]
    public class SplitwiseExporterJob : IJob
    {
        private readonly SplitwiseClient _splitwiseClient;
        private readonly UserService _userService;
        private readonly TakeoutService _takeoutService;

        public SplitwiseExporterJob(SplitwiseClient splitwiseClient, UserService userService, TakeoutService takeoutService)
        {
            _splitwiseClient = splitwiseClient;
            _userService = userService;
            _takeoutService = takeoutService;
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

            var result = await _splitwiseClient.Expense.CreateAsync(createExpenseRequest);
        }
    }
}
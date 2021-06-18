using Bot.Data;
using Bot.Jobs;
using Bot.Services;
using Bot.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Quartz;
using Telegram.Bot;

[assembly: ApiController]
namespace Bot
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(_configuration.GetConnectionString(DatabaseSettings.ConnectionStringName));
            });

            services.Configure<TelegramSettings>(_configuration.GetSection(TelegramSettings.SectionName));

            services.AddSingleton<ITelegramBotClient>(provider =>
            {
                var settings = provider.GetService<IOptions<TelegramSettings>>()!.Value;

                return new TelegramBotClient(settings.Token);
            });

            services.AddScoped<MessageService>()
                .AddScoped<CallbackQueryService, CallbackQueryService>()
                .AddScoped<OrderService>()
                .AddScoped<UserService>()
                .AddScoped<TakeoutService>();

            services.AddLocalization();

            services.AddQuartz(quartzConfigurator =>
            {
                quartzConfigurator.UseMicrosoftDependencyInjectionScopedJobFactory();

                var jobKey = new JobKey(nameof(NotifierJob));

                quartzConfigurator.AddJob<NotifierJob>(jobKey);

                quartzConfigurator.AddTrigger(triggerConfigurator => triggerConfigurator.ForJob(jobKey)
                    .WithIdentity(nameof(NotifierJob))
                    .WithCronSchedule(_configuration[$"{QuartzSettings.SectionName}:{nameof(NotifierJob)}"]));
            });

            services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

            services.AddControllers()
                .AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}

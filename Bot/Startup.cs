using Bot.Data;
using Bot.Services;
using Bot.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Telegram.Bot;

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

            services.AddSingleton<ITelegramBotClient>(
                provider =>
                {
                    var settings = provider.GetService<IOptions<TelegramSettings>>().Value;

                    return new TelegramBotClient(settings.Token);
                });
            
            services.AddScoped<IMessageService, MessageService>()
                .AddScoped<ICallbackQueryService, CallbackQueryService>()
                .AddScoped<OrderService>()
                .AddScoped<UserService>()
                .AddScoped<OrderPartService>();
            
            services.AddControllers()
                .AddNewtonsoftJson();
            
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "Bot", Version = "v1"}); });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bot v1"));
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}

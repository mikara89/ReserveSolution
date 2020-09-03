using SendMailService.Messaging.Receive.Options;
using SendMailService.Service;
using SendMailService.Service.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SendMailService.Messaging.Receive.Receive;
using System.Text;

namespace SendMailService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            services.AddTransient<ISendMailService, Service.SendMailService>();
            services.AddTransient<SendAppMailService>();

            services.Configure<AppEmailSettings>(Configuration.GetSection(nameof(AppEmailSettings)));
            services.Configure<RabbitMqMailConfiguration>(Configuration.GetSection("RabbitMqMail"));

            services.AddHostedService<SendMailMessageReceiver>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

using Mango.Services.Email.Messaging;

namespace Mango.Services.Email.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IAzureServiceBusConsumer ServiceBusConsumer { get; set; }
        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            ServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();

            IHostApplicationLifetime applicationLifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            applicationLifetime.ApplicationStarted.Register(()=> ServiceBusConsumer.Start());
            applicationLifetime.ApplicationStopped.Register(()=>ServiceBusConsumer.Stop());
            return app;
        }
    }
}

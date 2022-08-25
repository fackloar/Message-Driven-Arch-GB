using MassTransit;
using Restaurant.Notification.Consumers;

namespace Restaraunt.Notification
{
    public class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumer<NotifierTableBookedConsumer>();
                x.AddConsumer<KitchenReadyConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.UseMessageRetry(r =>
                    {
                        r.Exponential(5,
                            TimeSpan.FromSeconds(1),
                            TimeSpan.FromSeconds(100),
                            TimeSpan.FromSeconds(5));
                        r.Ignore<StackOverflowException>();
                        r.Ignore<ArgumentNullException>(x => x.Message.Contains("Consumer"));
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            builder.Services.AddSingleton<Notifier>();

            var app = builder.Build();
            app.Run();
        }
    }
}

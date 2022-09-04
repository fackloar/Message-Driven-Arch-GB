using MassTransit;
using Restaraunt.Notification;
using Restaraunt.Notification.Consumers;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<NotifyConsumer>()
        .Endpoint(e =>
        {
            e.Temporary = true;
        });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("sparrow.rmq.cloudamqp.com", 5671, "rvapidqy", h =>
        {
            h.Username("rvapidqy");
            h.Password("x1XJkf1mQU1iqkfCEfs1J7DeXhblPQkz");

            h.UseSsl(s =>
            {
                s.Protocol = System.Security.Authentication.SslProtocols.Tls12;
            });
        });

        cfg.UseMessageRetry(r =>
        {
            r.Exponential(5,
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(100),
                TimeSpan.FromSeconds(5));
            r.Ignore<StackOverflowException>();
            r.Ignore<ArgumentNullException>(x => x.Message.Contains("Consumer"));
        });

        cfg.UseInMemoryOutbox();
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddSingleton<Notifier>();

var app = builder.Build();
app.Run();

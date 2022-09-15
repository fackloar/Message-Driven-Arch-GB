using MassTransit;
using MassTransit.Audit;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Prometheus;
using Restaraunt.Booking.Classes;
using Restaraunt.Kitchen;
using Restaraunt.Kitchen.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var serviceProvider = builder.Services.BuildServiceProvider();
var auditStore = serviceProvider.GetService<IMessageAuditStore>();


builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<KitchenBookingRequestedConsumer>(cfg =>
    {
        cfg.UseMessageRetry(r => r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)));
        cfg.UseScheduledRedelivery(r => r.Incremental(3, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10)));
    })
        .Endpoint(e =>
        {
            e.Temporary = true;
        });

    x.AddConsumer<KitchenBookingRequestFaultConsumer>(cfg =>
    {
        cfg.UseMessageRetry(r => r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)));
        cfg.UseScheduledRedelivery(r => r.Incremental(3, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10)));
    })
        .Endpoint(e =>
        {
            e.Temporary = true;
        });

    x.AddConsumer<KitchenBookingCancellationConsumer>(cfg =>
    {
        cfg.UseMessageRetry(r => r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)));
        cfg.UseScheduledRedelivery(r => r.Incremental(3, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10)));
    })
        .Endpoint(e =>
        {
            e.Temporary = true;
        });

    x.AddDelayedMessageScheduler();

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
        cfg.UseDelayedMessageScheduler();
        cfg.UseInMemoryOutbox();
        cfg.ConfigureEndpoints(context);
        cfg.ConnectSendAuditObservers(auditStore);
        cfg.ConnectConsumeAuditObserver(auditStore);
        cfg.UsePrometheusMetrics(serviceName: "restaurant_kitchen");
    });
});

builder.Services
    .AddSingleton<Manager>()
    .AddSingleton<IMessageAuditStore, LoggingAuditStore>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapMetrics();
app.Run();
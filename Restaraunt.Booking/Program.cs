using MassTransit;
using Restaraunt.Booking.Classes;
using Restaraunt.Booking.Consumers;
using Restaraunt.Messages.Interfaces;
using Restaraunt.Messages;
using System.Diagnostics;
using MassTransit.Audit;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var serviceProvider = builder.Services.BuildServiceProvider();
var auditStore = serviceProvider.GetService<IMessageAuditStore>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BookingRequestConsumer>(cfg =>
    {
        cfg.UseMessageRetry(r => r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)));
        cfg.UseScheduledRedelivery(r => r.Incremental(3, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10)));
    })
        .Endpoint(e =>
        {
            e.Temporary = true;
        });

    x.AddConsumer<BookingRequestFaultConsumer>(cfg =>
    {
        cfg.UseMessageRetry(r => r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)));
        cfg.UseScheduledRedelivery(r => r.Incremental(3, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10)));
    })
        .Endpoint(e =>
        {
            e.Temporary = true;
        });

    x.AddConsumer<BookingCancellationConsumer>(cfg =>
    {
        cfg.UseMessageRetry(r => r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)));
        cfg.UseScheduledRedelivery(r => r.Incremental(3, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10)));
    })
        .Endpoint(e =>
        {
            e.Temporary = true;
        });

    x.AddSagaStateMachine<RestarauntBookingSaga, RestarauntBooking>()
        .Endpoint(e =>
        {
            e.Temporary = true;
        })
        .InMemoryRepository();

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
        cfg.Durable = false;
        cfg.ConnectSendAuditObservers(auditStore);
        cfg.ConnectConsumeAuditObserver(auditStore);
        cfg.UsePrometheusMetrics(serviceName: "restaraunt_booking");
    });
});

builder.Services
    .AddTransient<RestarauntBooking>()
    .AddTransient<RestarauntBookingSaga>()
    .AddTransient<RestarauntClass>()
    .AddSingleton(typeof(IRepository<>), typeof(InMemoryRepository<>))
    .AddSingleton<IMessageAuditStore, LoggingAuditStore>();

builder.Services.AddHostedService<BookingWorker>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.MapMetrics();
app.Run();

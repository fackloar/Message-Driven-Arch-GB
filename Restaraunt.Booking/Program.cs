using MassTransit;
using Restaraunt.Booking.Classes;
using Restaraunt.Booking.Consumers;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BookingRequestConsumer>()
        .Endpoint(e =>
        {
            e.Temporary = true;
        });

    x.AddConsumer<BookingRequestFaultConsumer>()
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
    });
});

builder.Services.AddTransient<RestarauntBooking>();
builder.Services.AddTransient<RestarauntBookingSaga>();
builder.Services.AddTransient<RestarauntClass>();

builder.Services.AddHostedService<BookingWorker>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.Run();
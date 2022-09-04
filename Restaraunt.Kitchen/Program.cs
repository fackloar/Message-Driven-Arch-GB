using MassTransit;
using Restaraunt.Kitchen;
using Restaraunt.Kitchen.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<KitchenBookingRequestedConsumer>(
                            configurator =>
                            {
                            })
                            .Endpoint(e =>
                            {
                                e.Temporary = true;
                            }); ;
    x.AddConsumer<KitchenBookingRequestFaultConsumer>()
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
        cfg.UseInMemoryOutbox();
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddSingleton<Manager>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.Run();
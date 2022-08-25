using MassTransit;
using Restaraunt.Kitchen;
using Restaraunt.Kitchen.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<KitchenTableBookedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddSingleton<Manager>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.Run();
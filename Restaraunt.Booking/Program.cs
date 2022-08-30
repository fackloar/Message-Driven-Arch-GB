using Restaraunt.Booking.Classes;
using Restaraunt.Notification;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHostedService<Worker>();
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

Console.OutputEncoding = System.Text.Encoding.UTF8;
var rest = new RestarauntClass();
var speakerBot = new SpeakerBot(rest);

app.Start();
speakerBot.InitialHello();


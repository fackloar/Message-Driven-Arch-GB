using Restaraunt.Booking.Classes;
using Restaraunt.Messages.Interfaces;

namespace Restaraunt.Booking.Tests;

public class BookingRequestConsumerTests : IAsyncLifetime
{
    private readonly ServiceProvider _provider;
    private readonly ITestHarness _harness;
    private readonly TestWriter _writer;
    private readonly RestarauntClass _restaurant;

    public BookingRequestConsumerTests(ITestOutputHelper output)
    {
        _writer = new TestWriter(output);
        _provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<BookingRequestConsumer>(); })
            .AddLogging()
            .AddTransient<RestarauntClass>()
            .AddSingleton<IRepository<BookingRequest>, InMemoryRepository<BookingRequest>>()
            .BuildServiceProvider(true);

        _harness = _provider.GetTestHarness();
    }

    public async Task InitializeAsync()
    {
        await _harness.Start();
    }

    public async Task DisposeAsync()
    {
        await _harness.OutputTimeline(_writer, options => options.Now().IncludeAddress());
        await _provider.DisposeAsync();
    }

    [Fact]
    public async Task ConsumeRequest()
    {
        var orderId = Guid.NewGuid();

        await _harness.Bus.Publish(
            (IBookingRequest)new BookingRequest(
                orderId,
                Guid.NewGuid(),
                Dish.Pasta,
                DateTime.Now,
                3));

        Assert.True(await _harness.Consumed.Any<IBookingRequest>());
    }

    [Fact]
    public async Task PublishTableBookedOnConsume()
    {
        var consumer = _harness.GetConsumerHarness<BookingRequestConsumer>();

        var orderId = NewId.NextGuid();
        var bus = _harness.Bus;

        await bus.Publish((IBookingRequest)
            new BookingRequest(
                orderId,
                Guid.NewGuid(),
                Dish.Pasta,
                DateTime.Now,
                3));

        Assert.Contains(consumer.Consumed.Select<IBookingRequest>(), x => x.Context.Message.OrderId == orderId);

        Assert.Contains(_harness.Published.Select<ITableBooked>(), x => x.Context.Message.OrderId == orderId);
    }
}
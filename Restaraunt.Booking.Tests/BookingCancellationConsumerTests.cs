using Microsoft.Extensions.DependencyInjection;
using Restaraunt.Booking.Classes;
using Restaraunt.Messages.Interfaces;

namespace Restaraunt.Booking.Tests;

public class BookingCancellationConsumerTests : IAsyncLifetime
{
    private readonly ServiceProvider _provider;
    private readonly ITestHarness _harness;
    private readonly TestWriter _writer;
    private readonly RestarauntClass _restaurant;

    public BookingCancellationConsumerTests(ITestOutputHelper output)
    {
        _writer = new TestWriter(output);
        _provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<BookingCancellationConsumer>(); })
            .AddLogging()
            .AddSingleton<RestarauntClass>()
            .AddSingleton<IRepository<BookingCancellation>, InMemoryRepository<BookingCancellation>>()
            .BuildServiceProvider(true);

        _harness = _provider.GetTestHarness();
        _restaurant = _provider.GetService<RestarauntClass>()!;
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

        await _harness.Bus.Publish((IBookingCancellation)new BookingCancellation(orderId, 1));

        Assert.True(await _harness.Consumed.Any<IBookingCancellation>());
    }
}
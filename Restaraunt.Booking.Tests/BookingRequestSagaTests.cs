using Restaraunt.Booking.Classes;
using Restaraunt.Messages.Interfaces;
using Restaraunt.Kitchen;
using Restaraunt.Kitchen.Consumers;

namespace Restaraunt.Booking.Tests;

public class BookingRequestSagaTests : IAsyncLifetime
{
    private readonly ServiceProvider _provider;
    private readonly ITestHarness _harness;
    private readonly TestWriter _writer;
    private readonly RestarauntClass _restaurant;

    public BookingRequestSagaTests(ITestOutputHelper output)
    {
        _writer = new TestWriter(output);
        _provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<BookingRequestConsumer>();
                
                x.AddSagaStateMachine<RestarauntBookingSaga, RestarauntBooking>()
                    .InMemoryRepository();
                x.AddPublishMessageScheduler();
                x.UsingInMemory((context, cfg) =>
                {
                    cfg.UseInMemoryScheduler();
                    cfg.ConfigureEndpoints(context);
                });
            })
            .AddLogging()
            .AddTransient<RestarauntClass>()
            .AddSingleton(typeof(IRepository<>), typeof(InMemoryRepository<>))
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
    public async Task SagaSucceed()
    {
        var orderId = NewId.NextGuid();
        var clientId = NewId.NextGuid();
        
        await _harness.Bus.Publish(new BookingRequest(
            orderId,
            clientId,
            Dish.Pasta,
            DateTime.Now,
            3));
        
        Assert.True(await _harness.Published.Any<IBookingRequest>());
        Assert.True(await _harness.Consumed.Any<IBookingRequest>());

        var sagaHarness = _harness.GetSagaStateMachineHarness<RestarauntBookingSaga, RestarauntBooking>();

        Assert.True(await sagaHarness.Consumed.Any<IBookingRequest>());
        Assert.True(await sagaHarness.Created.Any(x => x.CorrelationId == orderId));

        var saga = sagaHarness.Created.Contains(orderId);

        Assert.NotNull(saga);
        Assert.Equal(saga.ClientId, clientId);
        Assert.True(await _harness.Published.Any<ITableBooked>());
        Assert.True(await _harness.Published.Any<INotify>());
        Assert.Equal(3, saga.CurrentState);
    }
}
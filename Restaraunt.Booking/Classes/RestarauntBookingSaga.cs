using MassTransit;
using Restaraunt.Booking.Consumers;
using Restaraunt.Messages;
using Restaraunt.Messages.Interfaces;
using Automatonymous;

namespace Restaraunt.Booking.Classes
{
    public class RestarauntBookingSaga : MassTransitStateMachine<RestarauntBooking>
    {
        public RestarauntBookingSaga()
        {
            InstanceState(x => x.CurrentState);

            Event(() => BookingRequested,
                x =>
                    x.CorrelateById(context => context.Message.OrderId)
                        .SelectId(context => context.Message.OrderId));

            Event(() => TableBooked,
                x =>
                    x.CorrelateById(context => context.Message.OrderId));

            Event(() => KitchenReady,
                x =>
                    x.CorrelateById(context => context.Message.OrderId));

            CompositeEvent(() => BookingApproved,
                x => x.ReadyEventStatus, KitchenReady, TableBooked);

            Event(() => BookingRequestFault,
                x =>
                    x.CorrelateById(m => m.Message.Message.OrderId));

            Schedule(() => BookingExpired,
                x => x.ExpirationId, x =>
                {
                    x.Delay = TimeSpan.FromSeconds(5);
                    x.Received = e => e.CorrelateById(context => context.Message.OrderId);
                });

            Schedule(() => GuestArrival,
                x => x.ClientId, x =>
                {
                    x.Received = e => e.CorrelateById(context => context.Message.ClientId);
                });

            Initially(
                When(BookingRequested)
                    .Then(context =>
                    {
                        context.Saga.CorrelationId = context.Message.OrderId;
                        context.Saga.OrderId = context.Message.OrderId;
                        context.Saga.ClientId = context.Message.ClientId;
                        context.Saga.TimeOfArrival = context.Message.EstimatedTimeOfArrival;

                        Console.WriteLine("Saga: " + context.Message.CreationDate);
                    })
                    .Schedule(BookingExpired,
                        context => new BookingExpire(context.Saga),
                        context => TimeSpan.FromSeconds(1))
                    .TransitionTo(AwaitingBookingApproved)
            );

            During(AwaitingBookingApproved,
                When(TableBooked)
                    .Then(context => context.Saga.TableId = context.Message.TableId),
                When(BookingApproved)
                    .Unschedule(BookingExpired)
                    .Publish(context =>
                        (INotify)new Notify(context.Saga.OrderId,
                            context.Saga.ClientId,
                            $"Стол успешно забронирован"))
                    .Schedule(GuestArrival,
                        context => new GuestArrival(context.Saga),
                        context => TimeSpan.FromSeconds(context.Saga.TimeOfArrival))
                    .Publish(context => (INotify)new Notify(context.Saga.OrderId, context.Saga.ClientId, "Ожидаем гостя"))
                    .TransitionTo(AwaitingGuestArrival),

                When(BookingRequestFault)
                    .Then(context => Console.WriteLine($"Ошибочка вышла!"))
                    .Publish(context => (INotify)new Notify(context.Saga.OrderId,
                        context.Saga.ClientId,
                        $"Приносим извинения, стол забронировать не получилось."))
                    .Publish(context => (IBookingCancellation)
                        new BookingCancellation(context.Message.Message.OrderId, context.Saga.TableId))
                    .Finalize(),

                When(BookingExpired.Received)
                    .Then(context => Console.WriteLine($"Отмена заказа {context.Saga.OrderId}"))
                    .Finalize()
            );

            During(AwaitingGuestArrival,
                When(GuestArrival.Received)
                    .Publish(context => (INotify)new Notify(context.Saga.OrderId, context.Saga.ClientId, "Гость прибыл"))
                    .Finalize());

            SetCompletedWhenFinalized();
        }
        public MassTransit.State AwaitingBookingApproved { get; private set; }
        public MassTransit.State AwaitingGuestArrival { get; private set; }
        public Event<IBookingRequest> BookingRequested { get; private set; }
        public Event<ITableBooked> TableBooked { get; private set; }
        public Event<IKitchenReady> KitchenReady { get; private set; }

        public Event<Fault<IBookingRequest>> BookingRequestFault { get; private set; }

        public Schedule<RestarauntBooking, IBookingExpire> BookingExpired { get; private set; }
        public Schedule<RestarauntBooking, IGuestArrival> GuestArrival { get; private set; }
        public Event BookingApproved { get; private set; }
    }
}

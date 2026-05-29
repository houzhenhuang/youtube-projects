using CleanArchitecture.Domain.DomainEvents;

namespace CleanArchitecture.Application.DomainEventsHandlers.Products;

internal sealed class OrderCreatedDomainEventHandler : INotificationHandler<ProductCreatedDomainEvent>
{
    // private readonly IBus _bus;
    //
    // public OrderCreatedDomainEventHandler(IBus bus)
    // {
    //     _bus = bus;
    // }

    public async Task Handle(ProductCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        //   //发布集成事件
        // await _bus.Send(new OrderCreatedIntegrationEvent(notification.OrderId.Value));
    }
}
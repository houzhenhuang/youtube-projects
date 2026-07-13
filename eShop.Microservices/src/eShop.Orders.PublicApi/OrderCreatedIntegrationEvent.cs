using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Orders.PublicApi;

public class OrderCreatedIntegrationEvent
{
    public Guid OrderId { get; set; }
}

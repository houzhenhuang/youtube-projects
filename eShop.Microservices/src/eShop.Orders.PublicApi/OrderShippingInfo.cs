using System;
using System.Collections.Generic;
using System.Text;

namespace eShop.Orders.PublicApi;

public sealed record OrderShippingInfo(Guid OrderId, string ShippingAddress);

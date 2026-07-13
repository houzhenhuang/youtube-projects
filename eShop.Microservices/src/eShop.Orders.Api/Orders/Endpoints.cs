using eShop.Orders.Api.Models;
using eShop.Orders.PublicApi;
using eShop.Products.PublicApi;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace eShop.Orders.Api.Orders;

public static class Endpoints
{
    internal sealed record CreateOrderDto(
        string CustomerName,
        string ShippingAddress,
        List<CreateOrderItemDto> Items);

    internal sealed record CreateOrderItemDto(int ProductId, int Quantity);

    public static void MapOrdersEndpoints(this WebApplication app)
    {
        app.MapPost("api/orders", async (
            CreateOrderDto createOrderDto,
            IProductService productService,
            OrdersDbContext dbContext,
            IPublishEndpoint publishEndpoint) =>
        {
            if (!createOrderDto.Items.Any())
            {
                return Results.Ok("订单必须至少包含一个商品。");
            }

            if (createOrderDto.Items.Any(i => i.Quantity <= 0))
            {
                return Results.Ok("订单商品数量必须大于0。");
            }

            var validatedItems = new List<(CreateOrderItemDto ItemRequest, ProductDto ProductDetails)>();
            var productIds = createOrderDto.Items.Select(i => i.ProductId).ToArray();
            var productDtos = await productService.GetProductsByIdsAsync(productIds);

            foreach (var itemRequest in createOrderDto.Items)
            {
                ProductDto? productDto;
                try
                {
                    productDto = productDtos.Find(p => p.Id == itemRequest.ProductId);
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        $"获取产品信息失败。{ex.Message}",
                        statusCode: StatusCodes.Status503ServiceUnavailable);
                }

                if (productDto is null)
                {
                    return Results.BadRequest($"未找到 ID 为 {itemRequest.ProductId} 的产品。");
                }

                if (productDto.AvailableQuantity < itemRequest.Quantity)
                {
                    return Results.BadRequest($"产品 {productDto.Name} 数量不足。可用：{productDto.AvailableQuantity}，请求：{itemRequest.Quantity}。");
                }

                validatedItems.Add((itemRequest, productDto));
            }

            var orderItems = new List<OrderItem>();
            decimal calclatedTotalPrice = 0;

            foreach (var (itemRequest, productDetails) in validatedItems)
            {
                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = Guid.Empty,
                    ProductId = productDetails.Id,
                    ProductName = productDetails.Name,
                    UnitPrice = productDetails.Price,
                    Quantity = itemRequest.Quantity
                };

                orderItems.Add(orderItem);
                calclatedTotalPrice += orderItem.UnitPrice * orderItem.Quantity;
            }

            Order order = new()
            {
                Id = Guid.NewGuid(),
                CustomerName = createOrderDto.CustomerName,
                ShippingAddress = createOrderDto.ShippingAddress,
                TotalPrice = calclatedTotalPrice,
                OrderDate = DateTime.UtcNow,
                Items = orderItems
            };

            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();

            var orderCreatedEvent = new OrderCreatedIntegrationEvent
            {
                OrderId = order.Id
            };
            await publishEndpoint.Publish(orderCreatedEvent);

            return Results.Created($"api/orders/{order.Id}", order);
        })
        .Produces<Order>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status503ServiceUnavailable)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .WithName("CreateOrder");

        app.MapGet("api/orders/{id:guid}", async (Guid id, OrdersDbContext dbContext) =>
        {
            var order = await dbContext.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(order);
        });

        app.MapGet("api/orders/{id:guid}/shipping-info", async (Guid id, IOrderService orderService) =>
        {
            var order = await orderService.GetOrderForShippingAsync(id);
            if (order == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(order);
        })
        .Produces<OrderShippingInfo>()
        .Produces(StatusCodes.Status404NotFound)
        .WithName("GetOrderShippingInfo");

        // 预定库存
        app.MapPut("orders/{id:guid}/reserve-stock", async (Guid id, IOrderService orderService) =>
        {
            var orderInfo = await orderService.ReserveOrderStockAsync(id);

            return Results.Ok(orderInfo);
        })
        .Produces<ReserveOrderStockResult>()
        .Produces(StatusCodes.Status404NotFound)
        .WithName("ReserveOrderStock");
    }
}

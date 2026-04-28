using ResilientOrders.Api.Models;

namespace ResilientOrders.Api.Services;

public interface IOrderService
{
    decimal CalculateTotal(decimal subtotal, decimal discountRate);
    Order PlaceOrder(CreateOrderRequest request);
    Order? GetOrder(int id);
    IEnumerable<Order> ListOrders();
}

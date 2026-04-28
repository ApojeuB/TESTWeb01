using ResilientOrders.Api.Models;

namespace ResilientOrders.Api.Services;

public interface IOrderRepository
{
    Order? GetById(int id);
    IEnumerable<Order> GetAll();
    Order Save(Order order);
    bool Delete(int id);
}

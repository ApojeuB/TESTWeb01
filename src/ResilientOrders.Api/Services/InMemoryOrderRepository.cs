using ResilientOrders.Api.Models;

namespace ResilientOrders.Api.Services;

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly Dictionary<int, Order> _store = new();
    private int _nextId = 1;
    private readonly object _lock = new();

    public Order? GetById(int id)
    {
        lock (_lock)
        {
            return _store.TryGetValue(id, out var order) ? order : null;
        }
    }

    public IEnumerable<Order> GetAll()
    {
        lock (_lock)
        {
            return _store.Values.ToList();
        }
    }

    public Order Save(Order order)
    {
        lock (_lock)
        {
            if (order.Id == 0)
            {
                order.Id = _nextId++;
            }
            _store[order.Id] = order;
            return order;
        }
    }

    public bool Delete(int id)
    {
        lock (_lock)
        {
            return _store.Remove(id);
        }
    }
}

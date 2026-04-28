using ResilientOrders.Api.Models;

namespace ResilientOrders.Api.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;

    public OrderService(IOrderRepository repository)
    {
        _repository = repository;
    }

    public decimal CalculateTotal(decimal subtotal, decimal discountRate)
    {
        if (subtotal < 0)
        {
            throw new ArgumentException("Subtotal não pode ser negativo.", nameof(subtotal));
        }

        if (discountRate < 0 || discountRate > 1)
        {
            throw new ArgumentException("Desconto deve estar entre 0 e 1.", nameof(discountRate));
        }

        return Math.Round(subtotal * (1 - discountRate), 2);
    }

    public Order PlaceOrder(CreateOrderRequest request)
    {
        if (request.Items is null || request.Items.Count == 0)
        {
            throw new InvalidOperationException("Pedido não pode estar vazio.");
        }

        var subtotal = request.Items.Sum(item => item.Subtotal);
        var total = CalculateTotal(subtotal, request.DiscountRate);

        var order = new Order
        {
            CustomerName = request.CustomerName,
            Items = request.Items,
            DiscountRate = request.DiscountRate,
            Total = total
        };

        return _repository.Save(order);
    }

    public Order? GetOrder(int id) => _repository.GetById(id);

    public IEnumerable<Order> ListOrders() => _repository.GetAll();
}

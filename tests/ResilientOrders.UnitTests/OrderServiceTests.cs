using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using ResilientOrders.Api.Models;
using ResilientOrders.Api.Services;
using Xunit;

namespace ResilientOrders.UnitTests;

public class OrderServiceTests
{
    [Fact(DisplayName = "CalculateTotal: aplica desconto corretamente sobre o subtotal")]
    public void CalculateTotal_WithValidDiscount_ReturnsExpectedValue()
    {
        // Arrange
        var mockRepo = new Mock<IOrderRepository>();
        var service = new OrderService(mockRepo.Object);

        // Act
        var result = service.CalculateTotal(100m, 0.10m);

        // Assert
        result.Should().Be(90m);
    }

    [Fact(DisplayName = "CalculateTotal: desconto negativo lança ArgumentException")]
    public void CalculateTotal_WithNegativeDiscount_ThrowsArgumentException()
    {
        // Arrange
        var mockRepo = new Mock<IOrderRepository>();
        var service = new OrderService(mockRepo.Object);

        // Act
        Action act = () => service.CalculateTotal(100m, -0.5m);

        // Assert
        act.Should()
           .Throw<ArgumentException>()
           .WithMessage("*entre 0 e 1*");
    }

    [Fact(DisplayName = "PlaceOrder: pedido sem itens lança InvalidOperationException")]
    public void PlaceOrder_WithEmptyItems_ThrowsInvalidOperationException()
    {
        // Arrange
        var mockRepo = new Mock<IOrderRepository>();

        mockRepo
            .Setup(r => r.Save(It.IsAny<Order>()))
            .Returns<Order>(o => o);

        var service = new OrderService(mockRepo.Object);

        var request = new OrderRequest
        {
            CustomerName = "Teste",
            Items = new List<OrderItem>(),
            DiscountRate = 0.1m
        };

        // Act
        Action act = () => service.PlaceOrder(request);

        // Assert
        act.Should().Throw<InvalidOperationException>();

        mockRepo.Verify(r => r.Save(It.IsAny<Order>()), Times.Never);
    }

    [Fact(DisplayName = "PlaceOrder: pedido válido salva uma vez e retorna total correto")]
    public void PlaceOrder_WithValidItems_SavesOnce_AndReturnsOrderWithTotal()
    {
        // Arrange
        var mockRepo = new Mock<IOrderRepository>();

        mockRepo
            .Setup(r => r.Save(It.IsAny<Order>()))
            .Returns<Order>(o => o);

        var service = new OrderService(mockRepo.Object);

        var request = new OrderRequest
        {
            CustomerName = "Maria",
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = 1,
                    ProductName = "Mouse",
                    UnitPrice = 50,
                    Quantity = 2
                }
            },
            DiscountRate = 0.1m
        };

        // Act
        var result = service.PlaceOrder(request);

        // Assert
        result.Should().NotBeNull();
        result.Total.Should().Be(90m);

        mockRepo.Verify(r => r.Save(It.IsAny<Order>()), Times.Once);
    }
}
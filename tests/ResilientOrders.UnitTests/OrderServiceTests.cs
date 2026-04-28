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
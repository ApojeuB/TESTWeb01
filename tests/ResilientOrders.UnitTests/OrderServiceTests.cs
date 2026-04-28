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
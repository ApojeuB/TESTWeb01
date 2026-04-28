using System.Net;
using FluentAssertions;
using RestSharp;
using Xunit;

namespace ResilientOrders.IntegrationTests;

// =====================================================================
//  ETAPA 2 — Testes de Endpoint (RestSharp + FluentAssertions)
//
//  A fixture ApiFactoryFixture sobe a API em memória. Use:
//      var client = new RestClient(_fixture.HttpClient);
//
//  Cobertura mínima esperada:
//    1) GET /api/orders     -> 200 OK e corpo é uma lista
//    2) POST /api/orders    -> 201 Created com Location e corpo válido
//    3) POST /api/orders    -> 400 BadRequest quando o pedido é inválido
//                             (ex.: lista de itens vazia)
//
//  Dicas:
//    - request.AddJsonBody(new { ... }) já define Content-Type: application/json
//    - response.StatusCode.Should().Be(HttpStatusCode.Created);
//    - response.Content.Should().Contain("orderId");  // ou desserializar
// =====================================================================

[Collection("api")]
public class OrdersEndpointTests
{
    private readonly ApiFactoryFixture _fixture;
    private readonly RestClient _client;

    public OrdersEndpointTests(ApiFactoryFixture fixture)
    {
        _fixture = fixture;
        _client = new RestClient(_fixture.HttpClient);
    }

    [Fact(DisplayName = "GET /api/orders retorna 200 OK")]
    public async Task GetOrders_ReturnsOk()
    {
        // Arrange
        // TODO: criar RestRequest("/api/orders", Method.Get)

        // Act
        // TODO: response = await _client.ExecuteAsync(request)

        // Assert
        // TODO: response.StatusCode.Should().Be(HttpStatusCode.OK)
        //       response.Content.Should().NotBeNull()
        Assert.Fail("Implementar teste — remover quando concluir.");
    }

    [Fact(DisplayName = "POST /api/orders com payload válido retorna 201 Created")]
    public async Task PostOrder_WithValidPayload_Returns201()
    {
        // Arrange
        // TODO: montar payload anônimo:
        //   new { customerName = "Maria", items = new[] {
        //       new { productId = 1, productName = "Mouse", unitPrice = 50, quantity = 2 }
        //   }, discountRate = 0.1 }

        // Act
        // TODO: enviar POST com AddJsonBody(payload)

        // Assert
        // TODO: response.StatusCode.Should().Be(HttpStatusCode.Created);
        //       response.Content.Should().Contain("total");
        Assert.Fail("Implementar teste — remover quando concluir.");
    }

    [Fact(DisplayName = "POST /api/orders sem itens retorna 400 BadRequest")]
    public async Task PostOrder_WithEmptyItems_Returns400()
    {
        // Arrange
        // TODO: payload com items = new object[] { }

        // Act
        // TODO: enviar POST com AddJsonBody(payload)

        // Assert
        // TODO: response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        //       response.Content.Should().Contain("vazio");
        Assert.Fail("Implementar teste — remover quando concluir.");
    }

    // -------------------------------------------------------------------
    //  DESAFIO BÔNUS — Resiliência com Polly
    //  Implemente uma Retry Policy de 3 tentativas com backoff exponencial
    //  ao redor de uma chamada que aponte para uma URL inexistente
    //  ("http://localhost:9/api/orders") e prove que a policy executou
    //  exatamente 3 retries (use uma variável de contagem capturada na
    //  callback onRetry).
    //
    //  Dica:
    //    var attempts = 0;
    //    var policy = Policy
    //        .Handle<Exception>()
    //        .WaitAndRetryAsync(3,
    //            attempt => TimeSpan.FromMilliseconds(50 * attempt),
    //            (_, _, _, _) => attempts++);
    //
    //    Func<Task> act = async () => await policy.ExecuteAsync(...);
    //    await act.Should().ThrowAsync<Exception>();
    //    attempts.Should().Be(3);
    // -------------------------------------------------------------------
}

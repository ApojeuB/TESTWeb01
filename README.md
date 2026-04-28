# Resilient Orders — Template de Atividade Prática

Template para a aula **Testes de Resiliência de Serviços & Automação de Testes**
da disciplina *Arquitetura de Aplicações Web* (Newton/Wyden).

> Stack: **.NET 8 · C# · xUnit · Moq · FluentAssertions · RestSharp · Polly**

---

## 🎯 Missão

Você recebe uma API funcional de pedidos (`ResilientOrders.Api`) e dois projetos
de testes vazios. Sua missão é **implementar os testes** seguindo o handout da
aula:

| Etapa     | Onde                                                       | O que fazer                                                                  |
| --------- | ---------------------------------------------------------- | ---------------------------------------------------------------------------- |
| Setup     | raiz                                                       | `dotnet restore` e `dotnet build`                                            |
| Etapa 1   | `tests/ResilientOrders.UnitTests/OrderServiceTests.cs`     | 3 testes unitários do `OrderService` com xUnit + Moq + FluentAssertions      |
| Etapa 2   | `tests/ResilientOrders.IntegrationTests/OrdersEndpointTests.cs` | Testes de endpoint (GET, POST 201, POST 400) com RestSharp + FluentAssertions |
| Bônus     | mesmo arquivo da Etapa 2                                   | Retry Policy com Polly comprovando 3 retries                                 |

⏱️ 75 min em duplas · commit ao final de cada etapa

---

## 🗂️ Estrutura

```
project/
├── ResilientOrders.sln
├── src/
│   └── ResilientOrders.Api/              ← Aplicação WebAPI (não mexer)
│       ├── Controllers/OrdersController.cs
│       ├── Models/Order.cs
│       ├── Services/
│       │   ├── IOrderRepository.cs
│       │   ├── InMemoryOrderRepository.cs
│       │   ├── IOrderService.cs
│       │   └── OrderService.cs           ← classe sob teste
│       └── Program.cs
└── tests/
    ├── ResilientOrders.UnitTests/        ← ETAPA 1 (você implementa)
    │   └── OrderServiceTests.cs
    └── ResilientOrders.IntegrationTests/ ← ETAPA 2 + Bônus (você implementa)
        ├── ApiFactoryFixture.cs
        └── OrdersEndpointTests.cs
```

---

## ▶️ Como rodar

```bash
# Restore + build
dotnet restore
dotnet build

# Subir a API (porta 5000) — opcional, os testes de integração sobem em memória
dotnet run --project src/ResilientOrders.Api

# Rodar todos os testes
dotnet test

# Rodar só os unitários
dotnet test tests/ResilientOrders.UnitTests

# Rodar só os de endpoint
dotnet test tests/ResilientOrders.IntegrationTests
```

Swagger fica em `http://localhost:5000/swagger`.

---

## 🧪 Etapa 1 — Testes Unitários (xUnit + Moq + FluentAssertions)

Abra `OrderServiceTests.cs` e implemente os **3 testes** marcados com `TODO`,
seguindo o **padrão AAA**:

1. **Cálculo correto:** `CalculateTotal(100, 0.10)` deve retornar `90`.
2. **Desconto inválido:** `CalculateTotal(100, -0.5)` deve lançar `ArgumentException`.
3. **Pedido vazio:** `PlaceOrder(request com Items vazios)` deve lançar
   `InvalidOperationException` e **não** chamar `repository.Save`.

### Esqueleto AAA

```csharp
// Arrange
var mockRepo = new Mock<IOrderRepository>();
var service  = new OrderService(mockRepo.Object);

// Act
var result = service.CalculateTotal(100m, 0.10m);

// Assert
result.Should().Be(90m);
```

### Verificando exceções com FluentAssertions

```csharp
Action act = () => service.CalculateTotal(100m, -0.5m);
act.Should().Throw<ArgumentException>()
   .WithMessage("*entre 0 e 1*");
```

### Verificando comportamento com Moq

```csharp
mockRepo.Verify(r => r.Save(It.IsAny<Order>()), Times.Never);
```

---

## 🌐 Etapa 2 — Testes de Endpoint (RestSharp + FluentAssertions)

Abra `OrdersEndpointTests.cs`. A `ApiFactoryFixture` já sobe a API em memória —
basta usar o `RestClient` injetado no construtor.

Implemente os **3 testes** marcados com `TODO`:

1. `GET /api/orders` → 200 OK
2. `POST /api/orders` com payload válido → 201 Created
3. `POST /api/orders` com `items` vazio → 400 BadRequest

### Exemplo de POST com RestSharp

```csharp
var request = new RestRequest("/api/orders", Method.Post);
request.AddJsonBody(new {
    customerName = "Maria",
    items = new[] {
        new { productId = 1, productName = "Mouse", unitPrice = 50, quantity = 2 }
    },
    discountRate = 0.1
});

var response = await _client.ExecuteAsync(request);

response.StatusCode.Should().Be(HttpStatusCode.Created);
response.Content.Should().Contain("total");
```

---

## 🔄 Desafio Bônus — Polly (Retry Policy)

No mesmo arquivo de endpoint, implemente o teste descrito ao final do arquivo.
A ideia é montar uma `WaitAndRetryAsync(3, ...)` ao redor de uma chamada que
sempre falha (ex.: URL inexistente) e provar que a policy fez exatamente
**3 retries** antes de propagar a exceção.

```csharp
var attempts = 0;
var policy = Policy
    .Handle<Exception>()
    .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(50 * attempt),
        onRetry: (_, _, _, _) => attempts++);
```

---

## ✅ Critérios de aceite (exit ticket)

- [ ] 3 testes unitários verdes em `ResilientOrders.UnitTests`
- [ ] 3 testes de endpoint verdes em `ResilientOrders.IntegrationTests`
- [ ] Padrão AAA evidente nos comentários `// Arrange / // Act / // Assert`
- [ ] Pelo menos 1 uso de `Mock.Verify(...)` para validar comportamento
- [ ] Pelo menos 1 uso de `FluentAssertions` para checar corpo de resposta
- [ ] (Bônus) Retry Policy com Polly comprovando 3 retries

---

## 📌 Referências

- xUnit: <https://xunit.net>
- Moq: <https://github.com/devlooped/moq>
- FluentAssertions: <https://fluentassertions.com>
- RestSharp: <https://restsharp.dev>
- Polly: <https://github.com/App-vNext/Polly>
- Pirâmide de testes (Fowler): <https://martinfowler.com/bliki/TestPyramid.html>

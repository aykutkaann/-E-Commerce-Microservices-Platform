using OrderService.Models;

namespace OrderService.Services;

public class OrderService : IOrderService
{
    private readonly HttpClient _httpClient;
    private readonly OrderRepository _repository; 

    public OrderService(HttpClient httpClient, OrderRepository repository)
    {
        _httpClient = httpClient;
        _repository = repository; 
    }

    public Task<List<Order>> GetAllAsync()
    {
        return Task.FromResult(_repository.Orders.ToList());
    }

    public Task<Order?> GetByIdAsync(int id)
    {
        var order = _repository.Orders.FirstOrDefault(o => o.Id == id);
        return Task.FromResult(order);
    }

    public Task<List<Order>> GetByUserIdAsync(int userId)
    {
        var orders = _repository.Orders.Where(o => o.UserId == userId).ToList();
        return Task.FromResult(orders);
    }

    public async Task<(Order? Order, string? Error)> CreateAsync(CreateOrderRequest request)
    {
        ProductResponse? product = null;

        try
        {
            product = await _httpClient.GetFromJsonAsync<ProductResponse>(
                $"/api/products/{request.ProductId}");
        }
        catch
        {
            return (null, "ProductService'e ulaşılamadı.");
        }

        if (product is null)
            return (null, $"Ürün bulunamadı. ProductId: {request.ProductId}");

        if (product.Stock < request.Quantity)
            return (null, $"Yetersiz stok. Mevcut: {product.Stock}, İstenen: {request.Quantity}");

        var order = new Order
        {
            Id = _repository.NextId++, 
            UserId = request.UserId,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            TotalPrice = product.Price * request.Quantity,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _repository.Orders.Add(order); 
        return (order, null);
    }

    public Task<Order?> UpdateStatusAsync(int id, OrderStatus status)
    {
        var order = _repository.Orders.FirstOrDefault(o => o.Id == id);
        if (order is null) return Task.FromResult<Order?>(null);

        order.Status = status;
        return Task.FromResult<Order?>(order);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var order = _repository.Orders.FirstOrDefault(o => o.Id == id);
        if (order is null) return Task.FromResult(false);

        _repository.Orders.Remove(order);
        return Task.FromResult(true);
    }
}
public class OrderRepository
{
    public List<Order> Orders { get; } = new();
    public int NextId { get; set; } = 1;
}
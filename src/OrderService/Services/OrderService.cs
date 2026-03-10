using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;

namespace OrderService.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;

    public OrderService(AppDbContext context, HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
    }

    public async Task<List<Order>> GetAllAsync()
    {
        return await _context.Orders.ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders.FindAsync(id);
    }

    public async Task<List<Order>> GetByUserIdAsync(int userId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId)
            .ToListAsync();
    }

    public async Task<(Order? Order, string? Error)> CreateAsync(CreateOrderRequest request)
    {
        // ProductService'e sor
        ProductResponse? product = null;
        try
        {
            product = await _httpClient.GetFromJsonAsync<ProductResponse>(
                $"/api/products/{request.ProductId}");
        }
        catch
        {
            return (null, "ProductService could not be reached..");
        }

        if (product is null)
            return (null, $"Product not found. ProductId: {request.ProductId}");

        if (product.Stock < request.Quantity)
            return (null, $"Insufficient stock. Available: {product.Stock}, Request: {request.Quantity}");

        var order = new Order
        {
            UserId = request.UserId,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            TotalPrice = product.Price * request.Quantity,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return (order, null);
    }

    public async Task<Order?> UpdateStatusAsync(int id, OrderStatus status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order is null) return null;

        order.Status = status;
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order is null) return false;

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return true;
    }
}
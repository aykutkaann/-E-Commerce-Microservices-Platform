using OrderService.Models;

namespace OrderService.Services
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(int id);
        Task<List<Order>> GetByUserIdAsync(int userId);
        Task<(Order? Order, string? Error)> CreateAsync(CreateOrderRequest request);
        Task<Order?> UpdateStatusAsync(int id, OrderStatus status);
        Task<bool> DeleteAsync(int id);

    }
}

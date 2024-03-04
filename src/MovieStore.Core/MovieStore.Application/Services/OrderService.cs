using Microsoft.Extensions.Logging;
using MovieStore.Application.Interfaces;
using MovieStore.Domain;

namespace MovieStore.Application.Services;

public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;
    private readonly IOrderRepository _orderRepository;

    public OrderService (
        ILogger<OrderService> logger,
        IOrderRepository orderRepository
    )
    {
        _logger = logger;
        _orderRepository = orderRepository;
    }
    public async Task<Order> Add(Order order)
    {
        return await _orderRepository.Add(order);
    }
    public async Task<IEnumerable<Order>> GetAll()
    {
        return await _orderRepository.GetAll();
    }
    public async Task<Order?> GetById(Guid id)
    {
        return await _orderRepository.GetById(id);
    }
    public async Task<Order?> Update(Order order)
    {
        return await _orderRepository.Update(order);
    }
    public async Task<Order?> Delete(Guid id)
    {
        return await _orderRepository.Delete(id);
    }
}
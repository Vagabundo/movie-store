using MovieStore.Domain;

namespace MovieStore.Application.Interfaces;

public interface IOrderRepository
{
    Task<Order> Add(Order order);
    Task<IEnumerable<Order>> GetAll();
    Task<Order?> GetById(Guid id);
    Task<Order?> Modify(Order order);
    Task<Order?> Delete(Guid id);
}
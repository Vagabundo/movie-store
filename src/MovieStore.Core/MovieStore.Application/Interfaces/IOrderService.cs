using MovieStore.Domain;

namespace MovieStore.Application.Interfaces;

public interface IOrderService
{
    Task<Order> Add(Order movie);
    Task<IEnumerable<Order>> GetAll();
    Task<Order?> GetById(Guid id);
    Task<Order?> Modify(Order order);
    Task<Order?> Delete(Guid id);
}
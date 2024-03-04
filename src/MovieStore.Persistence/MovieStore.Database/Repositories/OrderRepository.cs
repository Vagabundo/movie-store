using Microsoft.EntityFrameworkCore;
using MovieStore.Application.Interfaces;
using MovieStore.Domain;

namespace MovieStore.Database;

public class OrderRepository : IOrderRepository
{
    private readonly MovieDbContext _dbContext;
    public OrderRepository(MovieDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region Create
    public async Task<Order> Add(Order order)
    {
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        return order;
    }
    #endregion

    #region Read
    public async Task<IEnumerable<Order>> GetAll()
    {
        return await _dbContext.Orders
        .AsNoTracking()
        .ToListAsync();
    }

    public async Task<Order?> GetById(Guid id)
    {
        return await _dbContext.Orders
        .AsNoTracking()
        .Where(x => x.Id == id)
        .FirstOrDefaultAsync();
    }

    // public async Task<IEnumerable<Order>> GetByBranch(Guid branchId)
    // {
    //     return await _dbContext.Orders
    //     .AsNoTracking()
    //     .Where(x => x.Id == id)
    //     .FirstOrDefaultAsync();
    // }
    #endregion

    #region Update
    public async Task<Order?> Update(Order order)
    {
        var dbOrder = await _dbContext.Orders
        .Where(x => x.Id == order.Id && !x.IsDeleted)
        .FirstOrDefaultAsync();

        if (dbOrder is not null)
        {
            dbOrder.Description = order.Description;
            dbOrder.Amount = order.Amount;
            dbOrder.PaymentSucceed = order.PaymentSucceed;

            await _dbContext.SaveChangesAsync();
        }

        return dbOrder;
    }
    #endregion

    #region Delete
    public async Task<Order?> Delete(Guid id)
    {
        var order = await _dbContext.Orders
        .Where(x => x.Id == id)
        .FirstOrDefaultAsync();

        if (order is not null)
        {
            order.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
        }

        return order;
    }
    #endregion
}

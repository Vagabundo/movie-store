using Microsoft.EntityFrameworkCore;
using MovieStore.Application.Interfaces;
using MovieStore.Domain;

namespace MovieStore.Database;

public class OrderRepository : IOrderRepository
{
    private MovieDbContext _dbContext;
    public OrderRepository(MovieDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region Create
    public async Task<Order> Add(Order order)
    {
        // await _dbContext.AuthUsers.AddAsync(
        //     new User
        //     {
        //         Id = Guid.Parse("5F9E3DE5-3BD2-4725-B8CC-B5E1FAB7165A"),
        //         Name = "Roberto",
        //         Email = "robertodelrior@gmail.com"
        //     });

        // await _dbContext.Branches.AddAsync(
        //     new Branch 
        //     {
        //         Id = Guid.Parse("AAF3DF04-B019-4D91-B8AF-7618DD6823E7"),
        //         UserId = Guid.Parse("5F9E3DE5-3BD2-4725-B8CC-B5E1FAB7165A"),
        //         Address = "Calle Paloma, 1",
        //         City = "Granada",
        //         Country = "ESPAÑA",
        //         PostalCode = "18001"
        //     });

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
    public async Task<Order?> Modify(Order order)
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

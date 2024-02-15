using Microsoft.EntityFrameworkCore;
using MovieStore.Application.Interfaces;
using MovieStore.Domain;

namespace MovieStore.Database;

public class BranchRepository : IBranchRepository
{
    private MovieDbContext _dbContext;

    public BranchRepository(MovieDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Branch?> GetByUser(Guid userId)
    {
        return await _dbContext.Branches
            .Include(x => x.BranchUser)
            .Where(x => x.BranchUser.Id == userId)
            .FirstAsync();
    }
}
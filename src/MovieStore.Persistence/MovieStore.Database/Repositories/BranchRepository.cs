using Microsoft.EntityFrameworkCore;
using MovieStore.Application.Interfaces;
using MovieStore.Database.Cache;
using MovieStore.Domain;

namespace MovieStore.Database;

public class BranchRepository : IBranchRepository
{
    private readonly MovieDbContext _dbContext;
    private readonly ICacheService _cache;

    public BranchRepository(MovieDbContext dbContext, ICacheService cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    #region Create

    public async Task<Branch> Add(Branch model)
    {
        await _dbContext.Branches.AddAsync(model);
        await _dbContext.SaveChangesAsync();
        // handle entry already exists

        return model;
    }

    public async Task<BranchMovie> AddMovie(Guid branchId, Guid movieId)
    {
        var newEntry = new BranchMovie
        {
            BranchId = branchId,
            MovieId = movieId
        };
        await _dbContext.BranchMovies.AddAsync(newEntry);
        await _dbContext.SaveChangesAsync();
        // handle entry already exists

        // this or create a bulk set and get by prefix
        var dbBranchMovies = await _dbContext.BranchMovies
            .AsNoTracking()
            .Include(x => x.Movie)
            .Where(x => x.BranchId == branchId && x.IsDeleted == false)
            .Select(x => x.Movie)
            .ToListAsync();

        await _cache.SetAsync($"branch/{branchId}/movies", dbBranchMovies);
      
        return newEntry;
    }

    #endregion

    #region Read

    public async Task<IEnumerable<Branch>> GetAll()
    {
        return await _dbContext.Branches
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .ToListAsync();
    }

    public async Task<Branch?> Get(Guid branchId)
    {
        return await _dbContext.Branches
            .Include(x => x.BranchUser)
            .AsNoTracking()
            .Where(x => x.Id == branchId)
            .FirstAsync();
    }

    public async Task<Branch?> GetByUser(Guid userId)
    {
        return await _dbContext.Branches
            .Include(x => x.BranchUser)
            .AsNoTracking()
            .Where(x => x.UserId == userId && !x.IsDeleted)
            .FirstAsync();
    }

    public async Task<IEnumerable<Movie>> GetMovies(Guid branchId)
    {
        return await _cache.GetAsync(
            $"branch/{branchId}/movies",
            async () =>
            {
                return await _dbContext.BranchMovies
                    .Include(x => x.Movie)
                    .AsNoTracking()
                    .Where(x => x.BranchId == branchId && x.IsDeleted == false)
                    .Select(x => x.Movie)
                    .ToListAsync();
            }
        );
    }

    #endregion

    #region Update

    public async Task<Branch?> Update(Branch branch)
    {
        var dbBranch = await _dbContext.Branches
        .Where(x => x.Id == branch.Id && !x.IsDeleted)
        .FirstOrDefaultAsync();

        if (dbBranch is not null)
        {
            dbBranch.UserId = branch.UserId;
            dbBranch.Address = branch.Address;
            dbBranch.City = branch.City;
            dbBranch.Country = branch.Country;
            dbBranch.PostalCode = branch.PostalCode;

            await _dbContext.SaveChangesAsync();
        }

        return dbBranch;
    }

    #endregion
    
    #region Delete

    public async Task<Branch?> Delete(Guid branchId)
    {
        var dbBranchMovie = await _dbContext.Branches
            .Where(x => x.Id == branchId && x.IsDeleted == false)
            .FirstAsync();

        if (dbBranchMovie is not null)
        {
            dbBranchMovie.IsDeleted = true;
            await _dbContext.SaveChangesAsync();

            // this or create a bulk set and get by prefix
            var dbBranchMovies = await _dbContext.BranchMovies
                .Include(x => x.Movie)
                .AsNoTracking()
                .Where(x => x.BranchId == branchId && x.IsDeleted == false)
                .Select(x => x.Movie)
                .ToListAsync();

            await _cache.RemoveAsync($"branch/{branchId}/movies");
        }

        return dbBranchMovie;
    }

    public async Task<BranchMovie?> RemoveMovie(Guid branchId, Guid movieId)
    {
        var dbBranchMovie = await _dbContext.BranchMovies
            .Where(x => x.BranchId == branchId && x.MovieId == movieId && x.IsDeleted == false)
            .FirstAsync();

        if (dbBranchMovie is not null)
        {
            dbBranchMovie.IsDeleted = true;
            await _dbContext.SaveChangesAsync();

            // this or create a bulk set and get by prefix
            var dbBranchMovies = await _dbContext.BranchMovies
                .Include(x => x.Movie)
                .AsNoTracking()
                .Where(x => x.BranchId == branchId && x.IsDeleted == false)
                .Select(x => x.Movie)
                .ToListAsync();

            await _cache.SetAsync($"branch/{branchId}/movies", dbBranchMovies);
        }

        return dbBranchMovie;
    }

    #endregion
}
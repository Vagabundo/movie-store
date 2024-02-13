using Microsoft.EntityFrameworkCore;
using MovieStore.Application.Interfaces;
using MovieStore.Domain;

namespace MovieStore.Database;

public class MovieRepository : IMovieRepository
{
    private MovieDbContext _dbContext;
    public MovieRepository(MovieDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region Create
    public async Task<Movie> Add(Movie movie)
    {
        await _dbContext.Movies.AddAsync(movie);
        await _dbContext.SaveChangesAsync();

        return movie;
    }
    #endregion

    #region Read
    public async Task<IEnumerable<Movie>> GetAll()
    {
        return await _dbContext.Movies
        .AsNoTracking()
        .ToListAsync();
    }

    public async Task<Movie?> GetById(Guid id)
    {
        return await _dbContext.Movies
        .AsNoTracking()
        .Where(x => x.Id == id)
        .FirstOrDefaultAsync();
    }

    // public async Task<IEnumerable<Movie>> GetByBranch(Guid branchId)
    // {
    //     return await _dbContext.Movies
    //     .AsNoTracking()
    //     .Where(x => x.Id == id)
    //     .FirstOrDefaultAsync();
    // }
    #endregion

    #region Update
    public async Task<Movie?> Modify(Movie movie)
    {
        var dbMovie = await _dbContext.Movies
        .Where(x => x.Id == movie.Id && !x.IsDeleted)
        .FirstOrDefaultAsync();

        if (dbMovie is not null)
        {
            dbMovie.Title = movie.Title;
            dbMovie.Description = movie.Description;
            dbMovie.Genre = movie.Genre;
            dbMovie.Country = movie.Country;
            dbMovie.Year = movie.Year;
            dbMovie.Cost = movie.Cost;

            await _dbContext.SaveChangesAsync();
        }

        return dbMovie;
    }
    #endregion

    #region Delete
    public async Task<Movie?> Delete(Guid id)
    {
        var movie = await _dbContext.Movies
        .Where(x => x.Id == id)
        .FirstOrDefaultAsync();

        if (movie is not null)
        {
            movie.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
        }

        return movie;
    }
    #endregion
}

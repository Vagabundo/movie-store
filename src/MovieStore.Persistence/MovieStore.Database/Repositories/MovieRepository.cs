using Microsoft.EntityFrameworkCore;
using MovieStore.Application.Interfaces;
using MovieStore.Database.Cache;
using MovieStore.Domain;

namespace MovieStore.Database;

public class MovieRepository : IMovieRepository
{
    private readonly MovieDbContext _dbContext;
    private readonly ICacheService _cache;
    
    public MovieRepository(MovieDbContext dbContext, ICacheService cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    #region Create
    public async Task<Movie> Add(Movie movie)
    {
        await _dbContext.Movies.AddAsync(movie);
        await _dbContext.SaveChangesAsync();

        // this or create a bulk set and get by prefix
        var dbMovies = await _dbContext.Movies
        .AsNoTracking()
        .ToListAsync();

        await _cache.SetAsync("movie_" + movie.Id.ToString(), movie);
        await _cache.SetAsync("movies", dbMovies);
        

        return movie;
    }
    #endregion

    #region Read
    public async Task<IEnumerable<Movie>> GetAll()
    {
        /* //longer, easier to understand version
        var cachedMovies = await _cache.GetAsync<IEnumerable<Movie>>("movies");

        if (cachedMovies is not null) return cachedMovies;

        var movies = await _dbContext.Movies
        .AsNoTracking()
        .ToListAsync();

        await _cache.SetAsync<IEnumerable<Movie>>("movies", movies);

        return movies;
        */

        return await _cache.GetAsync(
            "movies",
            async () =>
            {
                return await _dbContext.Movies
                    .AsNoTracking()
                    .ToListAsync();
            }
        );
    }

    public async Task<Movie?> GetById(Guid id)
    {
        return await _cache.GetAsync(
            $"movie_{id}",
            async () =>
            {
                return await _dbContext.Movies
                .AsNoTracking()
                .Where(x => x.Id == id)
                .FirstAsync();
            }
        );
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

        //this or create a bulk set and get by prefix
        var movies = await _dbContext.Movies
        .AsNoTracking()
        .ToListAsync();

        await _cache.SetAsync("movie_" + movie.Id.ToString(), movie);
        await _cache.SetAsync("movies", movies);

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

            // this or create a bulk set and get by prefix
            var movies = await _dbContext.Movies
            .AsNoTracking()
            .ToListAsync();

            await _cache.RemoveAsync("movie_" + id.ToString());
            await _cache.SetAsync("movies", movies);
        }

        return movie;
    }
    #endregion
}

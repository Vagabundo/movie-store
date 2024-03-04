using Microsoft.Extensions.Logging;
using MovieStore.Application.Interfaces;
using MovieStore.Domain;

namespace MovieStore.Application.Services;

public class MovieService : IMovieService
{
    private readonly ILogger<MovieService> _logger;
    private readonly IMovieRepository _movieRepository;

    public MovieService (
        ILogger<MovieService> logger,
        IMovieRepository movieRepository
    )
    {
        _logger = logger;
        _movieRepository = movieRepository;
    }
    public async Task<Movie> Add(Movie movie)
    {
        return await _movieRepository.Add(movie);
    }
    public async Task<IEnumerable<Movie>> GetAll()
    {
        return await _movieRepository.GetAll();
    }
    public async Task<Movie?> GetById(Guid id)
    {
        return await _movieRepository.GetById(id);
    }
    public async Task<Movie?> Update(Movie movie)
    {
        return await _movieRepository.Update(movie);
    }
    public async Task<Movie?> Delete(Guid id)
    {
        return await _movieRepository.Delete(id);
    }
}
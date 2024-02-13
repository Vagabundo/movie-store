using MovieStore.Domain;

namespace MovieStore.Application.Interfaces;

public interface IMovieRepository
{
    Task<Movie> Add(Movie movie);
    Task<IEnumerable<Movie>> GetAll();
    Task<Movie?> GetById(Guid id);
    Task<Movie?> Modify(Movie movie);
    Task<Movie?> Delete(Guid id);
}
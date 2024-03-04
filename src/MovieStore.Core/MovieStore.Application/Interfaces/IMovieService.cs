using MovieStore.Domain;

namespace MovieStore.Application.Interfaces;

public interface IMovieService
{
    Task<Movie> Add(Movie movie);
    Task<IEnumerable<Movie>> GetAll();
    Task<Movie?> GetById(Guid id);
    Task<Movie?> Update(Movie movie);
    Task<Movie?> Delete(Guid id);
}
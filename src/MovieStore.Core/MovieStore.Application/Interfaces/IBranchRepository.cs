using MovieStore.Domain;

namespace MovieStore.Application.Interfaces;

public interface IBranchRepository
{
    Task<Branch> Add(Branch branch);
    Task<BranchMovie> AddMovie(Guid branchId, Guid movieId);
    Task<Branch?> Get(Guid branchId);
    Task<Branch?> GetByUser (Guid userId);
    Task<IEnumerable<Movie>> GetMovies(Guid branchId);
    Task<Branch?> Update(Branch branch);
    Task<BranchMovie?> RemoveMovie(Guid branchId, Guid movieId);
    Task<Branch?> Delete(Guid branchId);
    Task<IEnumerable<Branch>> GetAll();
}
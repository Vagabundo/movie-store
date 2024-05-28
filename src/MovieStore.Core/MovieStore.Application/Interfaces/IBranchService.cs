using MovieStore.Domain;

namespace MovieStore.Application.Interfaces;

public interface IBranchService
{
    Task<Branch?> Add(Branch branch);
    Task<BranchMovie?> AddMovie(Guid branchId, Guid movieId);
    Task<IEnumerable<Branch>> GetAll();
    Task<Branch?> GetByUser(Guid userId);
    Task<IEnumerable<Movie>> GetMovies(Guid branchId);
    Task<Branch?> Update(Branch branch);
    Task<BranchMovie?> RemoveMovie(Guid branchId, Guid movieId);
    Task<Branch?> Delete(Guid branchId);
}
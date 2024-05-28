using Microsoft.Extensions.Logging;
using MovieStore.Application.Interfaces;
using MovieStore.Domain;

namespace MovieStore.Application.Services;

public class BranchService : IBranchService
{
    private readonly IBranchRepository _branchRepository;
    private readonly ILogger<BranchService> _logger;

    public BranchService(ILogger<BranchService> logger, IBranchRepository branchRepository)
    {
        _logger = logger;
        _branchRepository = branchRepository;
    }

    public async Task<Branch?> Add(Branch branch)
    {
        if (branch.UserId == Guid.Empty || string.IsNullOrEmpty(branch.Address) || string.IsNullOrEmpty(branch.City) ||
            string.IsNullOrEmpty(branch.Country) || string.IsNullOrEmpty(branch.PostalCode))
        {
            return null;
        }

        return await _branchRepository.Add(branch);
    }

    public async Task<BranchMovie?> AddMovie(Guid branchId, Guid movieId)
    {
        if (branchId == Guid.Empty || movieId == Guid.Empty)
        {
            return null;
        }
        return await _branchRepository.AddMovie(branchId, movieId);
    }

    public async Task<IEnumerable<Branch>> GetAll()
    {
        return await _branchRepository.GetAll();
    }

    public async Task<Branch?> GetByUser (Guid userId)
    {
        return await _branchRepository.GetByUser(userId);
    }

    public async Task<IEnumerable<Movie>> GetMovies(Guid branchId)
    {
        return await _branchRepository.GetMovies(branchId);
    }

    public async Task<Branch?> Update(Branch branch)
    {
        return await _branchRepository.Update(branch);
    }

    public async Task<BranchMovie?> RemoveMovie(Guid branchId, Guid movieId)
    {
        return await _branchRepository.RemoveMovie(branchId, movieId);
    }

    public async Task<Branch?> Delete(Guid branchId)
    {
        return await _branchRepository.Delete(branchId);
    }
}
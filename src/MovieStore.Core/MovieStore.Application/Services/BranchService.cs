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

    public async Task<Branch?> GetByUser (Guid userId)
    {
        return await _branchRepository.GetByUser(userId);
    }
}
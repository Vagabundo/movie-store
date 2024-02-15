using MovieStore.Domain;

namespace MovieStore.Application.Interfaces;

public interface IBranchRepository
{
    Task<Branch?> GetByUser (Guid userId);
}
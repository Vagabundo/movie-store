using MovieStore.Domain;

namespace MovieStore.Application.Interfaces;

public interface IBranchService
{
    Task<Branch?> GetByUser (Guid userId);
}
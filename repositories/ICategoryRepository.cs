using models;

namespace repositories;

public interface ICategoryRepository : IBaseRepository<Category>
{
    Task<List<Category>> GetAllAsync(string? search);
}

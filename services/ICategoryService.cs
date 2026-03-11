using models;

namespace services;

public interface ICategoryService
{
    Task<Category?> GetByIdAsync(int id);
    Task<List<Category>> GetAllAsync(string? search);
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(int id);
}

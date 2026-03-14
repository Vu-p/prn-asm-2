using models;

namespace services;

public interface ICategoryService
{
    Task<Category?> GetByIdAsync(short id);
    Task<List<Category>> GetAllAsync(string? search);
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(short id);
    Task<List<Category>> GetCategoryHierarchyAsync(short categoryId);
}

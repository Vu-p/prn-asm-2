using models;

namespace repositories;

public interface ITagRepository
{
    Task<Tag?> GetByIdAsync(int id);
    Task<List<Tag>> GetAllAsync();
    Task AddAsync(Tag tag);
}

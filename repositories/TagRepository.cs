using Microsoft.EntityFrameworkCore;
using models;

namespace repositories;

public class TagRepository : ITagRepository
{
    private readonly AppDbContext _db;

    public TagRepository(AppDbContext db) => _db = db;

    public Task<Tag?> GetByIdAsync(int id) => _db.Tags.FirstOrDefaultAsync(t => t.TagId == id);

    public Task<List<Tag>> GetAllAsync() => _db.Tags.OrderBy(t => t.TagName).ToListAsync();

    public async Task AddAsync(Tag tag)
    {
        _db.Tags.Add(tag);
        await _db.SaveChangesAsync();
    }
}

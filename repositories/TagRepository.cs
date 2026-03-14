using models;

namespace repositories;

public class TagRepository : BaseRepository<Tag>, ITagRepository
{
    public TagRepository(AppDbContext db) : base(db) { }
}

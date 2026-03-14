using Microsoft.EntityFrameworkCore;
using models;

namespace repositories;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<NewsArticle> NewsArticles => Set<NewsArticle>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<NewsTag> NewsTags => Set<NewsTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Account>()
            .HasIndex(a => a.AccountEmail)
            .IsUnique();

        modelBuilder.Entity<NewsTag>()
            .HasKey(nt => new { nt.NewsArticleId, nt.TagId });

        modelBuilder.Entity<NewsTag>()
            .HasOne(nt => nt.NewsArticle)
            .WithMany(n => n.NewsTags)
            .HasForeignKey(nt => nt.NewsArticleId);

        modelBuilder.Entity<NewsTag>()
            .HasOne(nt => nt.Tag)
            .WithMany(t => t.NewsTags)
            .HasForeignKey(nt => nt.TagId);

        modelBuilder.Entity<Category>()
            .HasOne(c => c.ParentCategory)
            .WithMany(c => c.ChildCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<NewsArticle>()
            .HasOne(n => n.CreatedBy)
            .WithMany(a => a.NewsArticles)
            .HasForeignKey(n => n.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<NewsArticle>()
            .HasOne(n => n.UpdatedBy)
            .WithMany()
            .HasForeignKey(n => n.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed minimal staff/lecturer for manual testing
        modelBuilder.Entity<Account>().HasData(
            new Account
            {
                AccountId = 1,
                AccountEmail = "staff@FUNewsManagementSystem.org",
                AccountPassword = "staff123",
                AccountName = "Default Staff",
                AccountRole = 1
            },
            new Account
            {
                AccountId = 2,
                AccountEmail = "lecturer@FUNewsManagementSystem.org",
                AccountPassword = "lecturer123",
                AccountName = "Default Lecturer",
                AccountRole = 2
            }
        );
    }
}

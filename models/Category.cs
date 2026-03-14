using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace models;

public class Category
{
    [Key]
    public short CategoryId { get; set; }

    [Required, StringLength(100)]
    public string CategoryName { get; set; } = string.Empty;

    [Required, StringLength(250)]
    public string CategoryDesciption { get; set; } = string.Empty;

    public short? ParentCategoryId { get; set; }

    [ForeignKey("ParentCategoryId")]
    public Category? ParentCategory { get; set; }

    public bool IsActive { get; set; }

    public ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
    public ICollection<Category> ChildCategories { get; set; } = new List<Category>();
}

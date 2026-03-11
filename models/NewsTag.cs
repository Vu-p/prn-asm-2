using System.ComponentModel.DataAnnotations;

namespace models;

public class NewsTag
{
    [Required]
    public string NewsArticleId { get; set; } = string.Empty;

    public NewsArticle? NewsArticle { get; set; }

    [Required]
    public int TagId { get; set; }

    public Tag? Tag { get; set; }
}

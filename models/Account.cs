using System.ComponentModel.DataAnnotations;

namespace models;

public class Account
{
    [Key]
    public short AccountId { get; set; }

    [Required, StringLength(100)]
    public string AccountName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(200)]
    public string AccountEmail { get; set; } = string.Empty;

    [Required]
    public int AccountRole { get; set; }

    [Required, StringLength(200)]
    public string AccountPassword { get; set; } = string.Empty;

    public ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
}

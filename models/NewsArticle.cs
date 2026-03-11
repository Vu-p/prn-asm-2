using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace models;

public class NewsArticle
{
    [Key]
    public string NewsArticleId { get; set; } = string.Empty;

    [Required, StringLength(400)]
    public string NewsTitle { get; set; } = string.Empty;

    [Required, StringLength(150)]
    public string Headline { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [Required]
    public string NewsContent { get; set; } = string.Empty;

    [StringLength(400)]
    public string? NewsSource { get; set; }

    [Required]
    public short CategoryId { get; set; }

    public Category? Category { get; set; }

    public byte? NewsStatus { get; set; }

    public short? CreatedById { get; set; }

    [ForeignKey("CreatedById")]
    public Account? CreatedBy { get; set; }

    public short? UpdatedById { get; set; }

    [ForeignKey("UpdatedById")]
    public Account? UpdatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();
}

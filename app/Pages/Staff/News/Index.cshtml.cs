using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using models;
using services;
using app.Hubs;
using System.Security.Claims;
using repositories;

namespace app.Pages.Staff.News;

[Authorize(Roles = "Staff,Admin")]
public class IndexModel : PageModel
{
    private readonly INewsService _newsService;
    private readonly ICategoryService _categoryService;
    private readonly ITagRepository _tagRepo; // Directly using for simplicity
    private readonly IHubContext<NewsHub> _hubContext;

    public IndexModel(INewsService newsService, ICategoryService categoryService, ITagRepository tagRepo, IHubContext<NewsHub> hubContext)
    {
        _newsService = newsService;
        _categoryService = categoryService;
        _tagRepo = tagRepo;
        _hubContext = hubContext;
    }

    public List<NewsArticle> Articles { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    [BindProperty]
    public string? TagsInput { get; set; }
    public List<Tag> AllTags { get; set; } = new();

    [BindProperty]
    public NewsArticle UpsertArticle { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public async Task OnGetAsync()
    {
        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            Articles = await _newsService.SearchAsync(SearchTerm);
        }
        else
        {
            Articles = await _newsService.GetAllAsync();
        }
        Categories = await _categoryService.GetAllAsync(null);
        AllTags = await _tagRepo.GetAllAsync();
    }

    public async Task<IActionResult> OnGetArticleDetailsAsync(string id)
    {
        var article = await _newsService.GetByIdWithDetailsAsync(id);
        if (article == null) return NotFound();

        return new JsonResult(new
        {
            article.NewsArticleId,
            article.NewsTitle,
            article.Headline,
            article.CategoryId,
            article.NewsStatus,
            article.NewsSource,
            NewsContent = article.NewsContent,
            TagsInput = string.Join(", ", article.NewsTags.Where(nt => nt.Tag != null).Select(nt => nt.Tag!.TagName))
        });
    }

    public async Task<IActionResult> OnPostUpsertAsync()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        short userId = short.Parse(userIdClaim!);
        short? finalUserId = userId == 0 ? null : userId;

        List<int> tagIds = new();
        if (!string.IsNullOrWhiteSpace(TagsInput))
        {
            var tagNames = TagsInput.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                    .Select(t => t.Trim())
                                    .Where(t => !string.IsNullOrEmpty(t))
                                    .Distinct(StringComparer.OrdinalIgnoreCase);
                                    
            AllTags = await _tagRepo.GetAllAsync();
            foreach (var name in tagNames)
            {
                var existingTag = AllTags.FirstOrDefault(t => t.TagName.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (existingTag != null)
                {
                    tagIds.Add(existingTag.TagId);
                }
                else
                {
                    var newTag = new Tag { TagName = name };
                    await _tagRepo.AddAsync(newTag);
                    tagIds.Add(newTag.TagId);
                }
            }
        }

        bool isNew = string.IsNullOrEmpty(UpsertArticle.NewsArticleId);
        if (isNew)
        {
            // Create
            UpsertArticle.NewsArticleId = Guid.NewGuid().ToString().Substring(0, 8); // Simple unique ID
            UpsertArticle.CreatedDate = DateTime.Now;
            UpsertArticle.CreatedById = finalUserId;
            await _newsService.AddAsync(UpsertArticle, tagIds);
        }
        else
        {
            // Update
            var existing = await _newsService.GetByIdWithDetailsAsync(UpsertArticle.NewsArticleId);
            if (existing == null) return NotFound();

            existing.NewsTitle = UpsertArticle.NewsTitle;
            existing.Headline = UpsertArticle.Headline;
            existing.CategoryId = UpsertArticle.CategoryId;
            existing.NewsStatus = UpsertArticle.NewsStatus;
            existing.NewsSource = UpsertArticle.NewsSource;
            existing.NewsContent = UpsertArticle.NewsContent;
            existing.UpdatedById = finalUserId;
            existing.ModifiedDate = DateTime.Now;

            await _newsService.UpdateAsync(existing, tagIds);
        }

        var finalArticle = await _newsService.GetByIdWithDetailsAsync(UpsertArticle.NewsArticleId);
        var broadcastData = new
        {
            finalArticle!.NewsArticleId,
            finalArticle.NewsTitle,
            finalArticle.Headline,
            categoryName = finalArticle.Category?.CategoryName,
            accountName = finalArticle.CreatedBy?.AccountName,
            finalArticle.NewsStatus,
            finalArticle.CreatedDate,
            finalArticle.NewsContent,
            finalArticle.NewsSource,
            finalArticle.CategoryId,
            tagsInput = string.Join(", ", finalArticle.NewsTags.Where(nt => nt.Tag != null).Select(nt => nt.Tag!.TagName))
        };

        if (isNew)
        {
            await _hubContext.Clients.All.SendAsync("ArticleCreated", broadcastData);
        }
        else
        {
            await _hubContext.Clients.All.SendAsync("ArticleUpdated", broadcastData);
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        await _newsService.DeleteAsync(id);
        await _hubContext.Clients.All.SendAsync("ArticleDeleted", id);
        return RedirectToPage();
    }
}

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

[Authorize(Roles = "Staff,Lecturer,Admin")]
public class IndexModel : PageModel
{
    private readonly INewsService _newsService;
    private readonly ICategoryService _categoryService;
    private readonly ITagRepository _tagRepo;
    private readonly ICloudinaryService _cloudinary;
    private readonly IHubContext<NewsHub> _hubContext;

    public IndexModel(INewsService newsService, 
                      ICategoryService categoryService, 
                      ITagRepository tagRepo, 
                      ICloudinaryService cloudinary,
                      IHubContext<NewsHub> hubContext)
    {
        _newsService = newsService;
        _categoryService = categoryService;
        _tagRepo = tagRepo;
        _cloudinary = cloudinary;
        _hubContext = hubContext;
    }

    public List<NewsArticle> Articles { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    
    [BindProperty]
    public string? TagsInput { get; set; }
    
    [BindProperty]
    public IFormFile? ThumbnailFile { get; set; }

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
            article.ThumbnailUrl,
            NewsContent = article.NewsContent,
            TagsInput = string.Join(", ", article.NewsTags.Where(nt => nt.Tag != null).Select(nt => nt.Tag!.TagName))
        });
    }

    public async Task<IActionResult> OnPostUpsertAsync()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        short userId = short.Parse(userIdClaim!);
        bool isAdmin = User.IsInRole("Admin");

        List<int> tagIds = await ProcessTagsAsync();

        // Handle Image Upload
        string? thumbUrl = null;
        if (ThumbnailFile != null)
        {
            thumbUrl = await _cloudinary.UploadImageAsync(ThumbnailFile);
        }

        bool isNew = string.IsNullOrEmpty(UpsertArticle.NewsArticleId);
        try
        {
            if (isNew)
            {
                UpsertArticle.NewsArticleId = Guid.NewGuid().ToString().Substring(0, 8);
                if (thumbUrl != null) UpsertArticle.ThumbnailUrl = thumbUrl;
                await _newsService.AddAsync(UpsertArticle, tagIds, userId, isAdmin);
            }
            else
            {
                if (thumbUrl != null) UpsertArticle.ThumbnailUrl = thumbUrl;
                await _newsService.UpdateAsync(UpsertArticle, tagIds, userId, isAdmin);
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToPage();
        }

        await BroadcastArticleUpdateAsync(UpsertArticle.NewsArticleId, isNew);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        short userId = short.Parse(userIdClaim!);
        bool isAdmin = User.IsInRole("Admin");

        try
        {
            await _newsService.DeleteAsync(id, userId, isAdmin);
            await _hubContext.Clients.All.SendAsync("ArticleDeleted", id);
        }
        catch (UnauthorizedAccessException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToPage();
    }

    private async Task<List<int>> ProcessTagsAsync()
    {
        List<int> tagIds = new();
        if (string.IsNullOrWhiteSpace(TagsInput)) return tagIds;

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
        return tagIds;
    }

    private async Task BroadcastArticleUpdateAsync(string articleId, bool isNew)
    {
        var finalArticle = await _newsService.GetByIdWithDetailsAsync(articleId);
        if (finalArticle == null) return;

        var broadcastData = new
        {
            finalArticle.NewsArticleId,
            finalArticle.NewsTitle,
            finalArticle.Headline,
            categoryName = finalArticle.Category?.CategoryName,
            accountName = finalArticle.CreatedBy?.AccountName,
            finalArticle.NewsStatus,
            finalArticle.CreatedDate,
            finalArticle.NewsContent,
            finalArticle.NewsSource,
            finalArticle.CategoryId,
            finalArticle.ThumbnailUrl,
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
    }
}

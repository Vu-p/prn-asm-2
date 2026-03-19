using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using models;
using services;
using System.Security.Claims;

namespace app.Pages.Staff;

[Authorize(Roles = "Staff,Lecturer,Admin")]
public class MyNewsModel : PageModel
{
    private readonly INewsService _newsService;

    public MyNewsModel(INewsService newsService)
    {
        _newsService = newsService;
    }

    public List<NewsArticle> Articles { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        if (User.IsInRole("Admin"))
        {
            Articles = await _newsService.GetAllAsync();
            return Page();
        }

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !short.TryParse(userIdClaim, out short userId))
        {
            return RedirectToPage("/Auth/Login");
        }

        Articles = await _newsService.GetHistoryByStaffAsync(userId);
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        var isAdmin = User.IsInRole("Admin");
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !short.TryParse(userIdClaim, out short userId))
        {
            return RedirectToPage("/Auth/Login");
        }

        try
        {
            await _newsService.DeleteAsync(id, userId, isAdmin);
        }
        catch (UnauthorizedAccessException)
        {
            TempData["ErrorMessage"] = "Bạn không có quyền xóa bài viết này.";
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Không thể xóa bài viết. Vui lòng thử lại.";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostApproveAsync(string id)
    {
        if (!User.IsInRole(AccountRole.Admin.ToString()))
        {
            return Forbid();
        }

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !short.TryParse(userIdClaim, out short userId))
        {
            return RedirectToPage("/Auth/Login");
        }

        var article = await _newsService.GetByIdWithDetailsAsync(id);
        if (article == null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy bài viết cần duyệt.";
            return RedirectToPage();
        }

        if (article.CreatedBy?.AccountRole == (int)AccountRole.Admin)
        {
            TempData["ErrorMessage"] = "Bài viết của Admin không cần thao tác duyệt tại đây.";
            return RedirectToPage();
        }

        if (article.NewsStatus == (byte)NewsStatus.Approved)
        {
            TempData["SuccessMessage"] = "Bài viết đã ở trạng thái Đã duyệt.";
            return RedirectToPage();
        }

        try
        {
            article.NewsStatus = (byte)NewsStatus.Approved;
            var tagIds = article.NewsTags
                .Where(nt => nt.TagId > 0)
                .Select(nt => nt.TagId)
                .Distinct()
                .ToList();

            await _newsService.UpdateAsync(article, tagIds, userId, isAdmin: true);
            TempData["SuccessMessage"] = "Duyệt bài viết thành công.";
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Không thể duyệt bài viết. Vui lòng thử lại.";
        }

        return RedirectToPage();
    }
}

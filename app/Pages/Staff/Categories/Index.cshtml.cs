using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using models;
using services;

namespace app.Pages.Staff.Categories;

[Authorize(Roles = "Staff,Admin")]
public class IndexModel : PageModel
{
    private readonly ICategoryService _categoryService;

    public IndexModel(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public List<Category> Categories { get; set; } = new();
    public List<Category> ParentCategories { get; set; } = new();

    [BindProperty]
    public Category UpsertCategory { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public async Task OnGetAsync()
    {
        Categories = await _categoryService.GetAllAsync(SearchTerm);
        ParentCategories = await _categoryService.GetAllAsync(null);
    }

    public async Task<IActionResult> OnPostUpsertAsync()
    {
        if (!ModelState.IsValid)
        {
            ParentCategories = await _categoryService.GetAllAsync(null);
            return Page();
        }

        if (UpsertCategory.CategoryId == 0)
        {
            await _categoryService.AddAsync(UpsertCategory);
        }
        else
        {
            await _categoryService.UpdateAsync(UpsertCategory);
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(short id)
    {
        try
        {
            await _categoryService.DeleteAsync(id);
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Cannot delete this category. It may have sub-categories or associated news articles.";
        }

        return RedirectToPage();
    }
}

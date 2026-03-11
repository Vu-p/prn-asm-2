using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using models;
using services;
using System.Security.Claims;

namespace app.Pages.Staff;

[Authorize(Roles = "Staff,Admin")]
public class ProfileModel : PageModel
{
    private readonly IAccountService _accountService;

    public ProfileModel(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [BindProperty]
    public Account Profile { get; set; } = default!;

    public string? Message { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return RedirectToPage("/Login");
        }

        var account = await _accountService.GetByIdAsync(userId);
        if (account == null)
        {
            return NotFound();
        }

        Profile = account;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return RedirectToPage("/Login");
        }

        var existingAccount = await _accountService.GetByIdAsync(userId);
        if (existingAccount == null)
        {
            return NotFound();
        }

        existingAccount.AccountName = Profile.AccountName;
        existingAccount.AccountPassword = Profile.AccountPassword;

        await _accountService.UpdateAsync(existingAccount);

        Message = "Profile updated successfully.";
        Profile = existingAccount;
        return Page();
    }
}

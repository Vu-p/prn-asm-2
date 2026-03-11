using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using models;
using services;

namespace app.Pages;

public class LoginModel : PageModel
{
    private readonly IAccountService _accounts;
    private readonly AdminAccountConfig _adminConfig;

    public LoginModel(IAccountService accounts, AdminAccountConfig adminConfig)
    {
        _accounts = accounts;
        _adminConfig = adminConfig;
    }

    [BindProperty, Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [BindProperty, Required]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToPage("/Index");
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (Email.Equals(_adminConfig.Email, StringComparison.OrdinalIgnoreCase)
            && Password == _adminConfig.Password)
        {
            await SignInAsync("Admin", "Admin", 0, Email);
            return RedirectToPage("/Admin/Accounts/Index");
        }

        var account = await _accounts.AuthenticateAsync(Email, Password);
        if (account == null)
        {
            ErrorMessage = "Invalid email or password.";
            return Page();
        }

        string role = account.AccountRole switch
        {
            1 => "Staff",
            2 => "Lecturer",
            _ => "Guest"
        };

        if (role == "Guest")
        {
             ErrorMessage = "You do not have permission to access the system.";
             return Page();
        }

        await SignInAsync(role, account.AccountName, account.AccountId, account.AccountEmail);

        if (role == "Staff")
        {
            return RedirectToPage("/Staff/News/Index");
        }

        return RedirectToPage("/News/Public");
    }

    private async Task SignInAsync(string role, string name, int accountId, string email)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, accountId.ToString()),
            new(ClaimTypes.Name, name),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }
}

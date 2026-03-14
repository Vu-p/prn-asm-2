using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using models;
using services;

namespace app.Pages.Auth;

public class LoginModel : PageModel
{
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string IdToken { get; set; } = string.Empty;
    }

    private readonly IAccountService _accounts;
    private readonly AdminAccountConfig _adminConfig;
    private readonly IConfiguration _config;

    public LoginModel(IAccountService accounts, AdminAccountConfig adminConfig, IConfiguration config)
    {
        _accounts = accounts;
        _adminConfig = adminConfig;
        _config = config;
    }

    [BindProperty, Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [BindProperty, Required]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public string FirebaseApiKey => _config["Firebase:ApiKey"] ?? "";
    public string FirebaseAuthDomain => _config["Firebase:AuthDomain"] ?? "";

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

        string role = (AccountRole)account.AccountRole switch
        {
            AccountRole.Staff => "Staff",
            AccountRole.Lecturer => "Lecturer",
            AccountRole.Admin => "Admin",
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

    public async Task<IActionResult> OnPostProcessGoogleLoginAsync([FromBody] LoginRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return new JsonResult(new { success = false, message = "Email không hợp lệ" });
            }

            // Nếu là admin
            if (request.Email.Equals(_adminConfig.Email, StringComparison.OrdinalIgnoreCase))
            {
                await SignInAsync("Admin", "Admin", 0, request.Email);
                return new JsonResult(new { success = true, redirect = "/Admin/Accounts/Index", message = "Đăng nhập thành công!" });
            }

            // Kiểm tra email trong hệ thống
            var account = await _accounts.GetByEmailAsync(request.Email);
            if (account == null)
            {
                return new JsonResult(new { success = false, message = "Email không được phép truy cập hệ thống. Vui lòng liên hệ Admin." });
            }

            string role = (AccountRole)account.AccountRole switch
            {
                AccountRole.Staff => "Staff",
                AccountRole.Lecturer => "Lecturer",
                AccountRole.Admin => "Admin",
                _ => "Guest"
            };

            if (role == "Guest")
            {
                return new JsonResult(new { success = false, message = "Tài khoản không có quyền truy cập." });
            }

            await SignInAsync(role, account.AccountName ?? "User", account.AccountId, account.AccountEmail);

            string redirectUrl = role == "Staff" ? "/Staff/News/Index" : "/News/Public";

            return new JsonResult(new { success = true, redirect = redirectUrl, message = "Đăng nhập thành công!" });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = $"Lỗi: {ex.Message}" });
        }
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

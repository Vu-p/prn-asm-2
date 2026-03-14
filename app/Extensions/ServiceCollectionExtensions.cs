using Microsoft.Extensions.DependencyInjection;
using repositories;
using services;
using models;
using Microsoft.EntityFrameworkCore;

namespace app.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProjectServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Repositories
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<INewsRepository, NewsRepository>();
        services.AddScoped<ITagRepository, TagRepository>();

        // Services
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<INewsService, NewsService>();
        services.AddScoped<ICloudinaryService, CloudinaryService>();

        // Config
        var adminConfig = configuration.GetSection("AdminAccount").Get<AdminAccountConfig>()
            ?? new AdminAccountConfig();
        services.AddSingleton(adminConfig);

        return services;
    }
}

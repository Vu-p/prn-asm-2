using Microsoft.AspNetCore.SignalR;

namespace app.Hubs;

public class NewsHub : Hub
{
    public async Task ArticleCreated(object article) => await Clients.All.SendAsync("ArticleCreated", article);
    public async Task ArticleUpdated(object article) => await Clients.All.SendAsync("ArticleUpdated", article);
    public async Task ArticleDeleted(string articleId) => await Clients.All.SendAsync("ArticleDeleted", articleId);
}

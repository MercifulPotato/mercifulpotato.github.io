using System.Net.Http.Json;
using System.Text.Json;
using ObserverMagazine.Web.Models;

namespace ObserverMagazine.Web.Services;

public sealed class BlogService(HttpClient http, ILogger<BlogService> logger) : IBlogService
{
    private const string PostsIndexPath = "blog-data/posts-index.json";
    private const string AuthorsIndexPath = "blog-data/authors.json";

    private BlogPostMetadata[]? _cachedIndex;
    private AuthorProfile[]? _cachedAuthors;

    public async Task<BlogPostMetadata[]> GetPostsAsync()
    {
        if (_cachedIndex is not null)
        {
            return _cachedIndex;
        }

        logger.LogInformation("Fetching blog posts index");
        var posts = await GetJsonAsync<BlogPostMetadata[]>(PostsIndexPath);
        if (posts is null)
        {
            return [];
        }

        _cachedIndex = posts.OrderByDescending(p => p.Date).ToArray();
        logger.LogInformation("Loaded {Count} blog posts", _cachedIndex.Length);
        return _cachedIndex;
    }

    public async Task<BlogPostMetadata?> GetPostMetadataAsync(string slug)
    {
        var posts = await GetPostsAsync();
        return posts.FirstOrDefault(p =>
            string.Equals(p.Slug, slug, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<string> GetPostHtmlAsync(string slug)
    {
        var path = $"blog-data/{slug}.html";
        logger.LogInformation("Fetching HTML for post: {Slug}", slug);
        try
        {
            return await http.GetStringAsync(path);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to download {Path} (HTTP {StatusCode})", path, (int?)ex.StatusCode);
            return "<p>Could not load post content.</p>";
        }
    }

    public async Task<AuthorProfile[]> GetAllAuthorsAsync()
    {
        if (_cachedAuthors is not null)
        {
            return _cachedAuthors;
        }

        logger.LogInformation("Fetching authors index");
        var authors = await GetJsonAsync<AuthorProfile[]>(AuthorsIndexPath);
        if (authors is null)
        {
            return [];
        }

        _cachedAuthors = authors;
        logger.LogInformation("Loaded {Count} author profiles", _cachedAuthors.Length);
        return _cachedAuthors;
    }

    public async Task<AuthorProfile?> GetAuthorAsync(string authorId)
    {
        var authors = await GetAllAuthorsAsync();
        return authors.FirstOrDefault(a =>
            string.Equals(a.Id, authorId, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<T?> GetJsonAsync<T>(string path) where T : class
    {
        try
        {
            return await http.GetFromJsonAsync<T>(path);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex,
                "Failed to download {Path} (HTTP {StatusCode}); the content processor may not have run",
                path, (int?)ex.StatusCode);
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "{Path} does not contain valid JSON", path);
        }
        catch (NotSupportedException ex)
        {
            logger.LogError(ex, "{Path} was not served as JSON", path);
        }

        return null;
    }
}

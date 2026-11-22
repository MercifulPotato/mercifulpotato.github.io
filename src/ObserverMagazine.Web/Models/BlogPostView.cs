namespace ObserverMagazine.Web.Models;

public closed record class BlogPostView;

public sealed record class BlogPostLoading : BlogPostView;

public sealed record class BlogPostMissing : BlogPostView;

public sealed record class BlogPostLoaded(BlogPostMetadata Metadata, string Html, AuthorProfile? Author) : BlogPostView;

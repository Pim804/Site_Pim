namespace OptiStock.Domain.Entities;

public sealed class Announcement
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? PublishedAtUtc { get; set; }
    public bool IsPublished { get; set; }
    public bool IsArchived { get; set; }
    public Guid AuthorId { get; set; }
}

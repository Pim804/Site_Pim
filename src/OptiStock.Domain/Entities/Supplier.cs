namespace OptiStock.Domain.Entities;

public sealed class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? TaxId { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Part> Parts { get; set; } = new List<Part>();
}

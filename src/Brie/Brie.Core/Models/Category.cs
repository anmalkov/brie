namespace Brie.Core.Models;

public record Category(
    Guid Id,
    string Name,
    string? Description,
    List<Category>? Children,
    List<Recommendation>? Recommendations
);

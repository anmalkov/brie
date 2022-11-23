namespace Brie.Core.Models;

public record Category(
    string Id,
    string Name,
    string? Description,
    List<Category>? Children,
    List<Recommendation>? Recommendations
) : IStorableItem;

using Brie.Core.Models;
using Brie.Core.Services;
using Brie.Ui.Requests;
using MediatR;
using System.Security.Cryptography.Xml;

namespace Brie.Ui.Handlers
{
    public record RecommendationDto(
        string Title,
        string Description
    );

    public record CategoryDto(
        string Name,
        string? Description,
        IEnumerable<CategoryDto>? Children,
        IEnumerable<RecommendationDto>? Recommendations
    );

    public class GetCategoriesHandler : IRequestHandler<GetCategoriesRequest, IResult>
    {
        private readonly ICategoriesService _categoriesService;

        public GetCategoriesHandler(ICategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
        }

        public async Task<IResult> Handle(GetCategoriesRequest request, CancellationToken cancellationToken)
        {
            var category = await _categoriesService.GetAsync();
            return Results.Ok(category is not null ? MapCategoryToDto(category) : null);
        }

        private static CategoryDto MapCategoryToDto(Category category)
        {
            return new CategoryDto(
                category.Name,
                category.Description,
                category.Children?.Select(MapCategoryToDto),
                category.Recommendations?.Select(MapRecommendationToDto)
            );
        }

        private static RecommendationDto MapRecommendationToDto(Recommendation recommendation)
        {
            return new RecommendationDto(
                recommendation.Title,
                recommendation.Description
            );
        }
    }
}

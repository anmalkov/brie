using Brie.Core.Models;
using Brie.Core.Services;
using Brie.Ui.Requests;
using MediatR;
using System.Security.Cryptography.Xml;

namespace Brie.Ui.Handlers
{
    public class GetThreatModelCategoriesHandler : IRequestHandler<GetThreatModelCategoriesRequest, IResult>
    {
        private readonly IThreatModelsService _threatModelsService;

        public GetThreatModelCategoriesHandler(IThreatModelsService threatModelsService)
        {
            _threatModelsService = threatModelsService;
        }

        public async Task<IResult> Handle(GetThreatModelCategoriesRequest request, CancellationToken cancellationToken)
        {
            var category = await _threatModelsService.GetCategoryAsync();
            return Results.Ok(MapCategoryToDto(category));
        }

        private static CategoryDto? MapCategoryToDto(Category? category)
        {
            if (category is null) {
                return null;
            }

            return new CategoryDto(
                category.Id,
                category.Name,
                category.Description,
                category.Children?.Select(MapCategoryToDto),
                category.Recommendations?.Select(MapRecommendationToDto)
            );
        }

        private static RecommendationDto MapRecommendationToDto(Recommendation recommendation)
        {
            return new RecommendationDto(
                recommendation.Id,
                recommendation.Title,
                recommendation.Description
            );
        }
    }
}

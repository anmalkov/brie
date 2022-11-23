using Brie.Core.Models;
using Brie.Core.Services;
using Brie.Ui.Requests;
using MediatR;

namespace Brie.Ui.Handlers
{
    public class CreateThreatModelHandler : IRequestHandler<CreateThreatModelRequest, IResult>
    {
        private readonly IThreatModelsService _threatModelsService;

        public CreateThreatModelHandler(IThreatModelsService threatModelsService)
        {
            _threatModelsService = threatModelsService;
        }

        public async Task<IResult> Handle(CreateThreatModelRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await _threatModelsService.CreateAsync(MapRequestToThreatModel(request.Body));
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }

        private static ThreatModel MapRequestToThreatModel(ThreatModelDto dto)
        {
            return new ThreatModel(
                Guid.NewGuid().ToString(),
                dto.ProjectName,
                dto.Description,
                dto.DataflowAttributes.Select(MapDtoToDataflowAttribute).ToArray(),
                dto.Threats.Select(MapDtoToRecommendation).ToArray()
            );
        }

        private static DataflowAttribute MapDtoToDataflowAttribute(DataflowAttributeDto dto)
        {
            return new DataflowAttribute(
                dto.Number,
                dto.Transport,
                dto.DataClassification,
                dto.Authentication,
                dto.Notes
            );
        }

        private static Recommendation MapDtoToRecommendation(RecommendationDto dto)
        {
            return new Recommendation(
                dto.Id,
                dto.Title,
                dto.Description
            );
        }

    }
}

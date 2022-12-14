using Brie.Core.Models;
using Brie.Core.Services;
using Brie.Ui.Requests;
using MediatR;

namespace Brie.Ui.Handlers
{
    public class UpdateThreatModelHandler : IRequestHandler<UpdateThreatModelRequest, IResult>
    {
        private readonly IThreatModelsService _threatModelsService;

        public UpdateThreatModelHandler(IThreatModelsService threatModelsService)
        {
            _threatModelsService = threatModelsService;
        }

        public async Task<IResult> Handle(UpdateThreatModelRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var oldThreatModel = await _threatModelsService.GetAsync(request.Id);
                if (oldThreatModel is null)
                {
                    return Results.NotFound();
                }
                var threatModel = MapRequestToThreatModel(request.Body, oldThreatModel);
                await _threatModelsService.UpdateAsync(threatModel);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }

        private static ThreatModel MapRequestToThreatModel(CreateThreatModelDto dto, ThreatModel oldThreatModel)
        {
            return new ThreatModel(
                oldThreatModel.Id,
                dto.ProjectName,
                dto.Description,
                oldThreatModel.CreatedAt,
                DateTime.Now,
                dto.DataflowAttributes.Select(MapDtoToDataflowAttribute).ToArray(),
                dto.Threats.Select(MapDtoToRecommendation).ToArray(),
                dto.Images?.ToDictionary(i => i.Key, i => i.Value)
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

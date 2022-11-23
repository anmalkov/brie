﻿using Brie.Core.Models;
using Brie.Core.Services;
using Brie.Ui.Requests;
using MediatR;
using System.Security.Cryptography.Xml;

namespace Brie.Ui.Handlers
{
    public record DataflowAttributeDto(
        string Number,
        string Transport,
        string DataClassification,
        string Authentication,
        string Notes
    );
    
    public record ThreatModelDto(
        string Id,
        string ProjectName,
        string? Description,
        IEnumerable<DataflowAttributeDto> DataflowAttributes,
        IEnumerable<RecommendationDto> Threats
    );

    public class GetThreatModelsHandler : IRequestHandler<GetThreatModelsRequest, IResult>
    {
        private readonly IThreatModelsService _threatModelsService;

        public GetThreatModelsHandler(IThreatModelsService threatModelsService)
        {
            _threatModelsService = threatModelsService;
        }

        public async Task<IResult> Handle(GetThreatModelsRequest request, CancellationToken cancellationToken)
        {
            var threatModels = await _threatModelsService.GetAllAsync();
            return Results.Ok(MapThreatModelsToDtos(threatModels));
        }

        private static IEnumerable<ThreatModelDto>? MapThreatModelsToDtos(IEnumerable<ThreatModel>? threatModels)
        {
            if (threatModels is null)
            {
                return new List<ThreatModelDto>();
            }
            
            return threatModels.Select(p => new ThreatModelDto(
                p.Id,
                p.ProjectName,
                p.Description,
                p.DataflowAttributes.Select(MapDataflowAttributeToDto).ToArray(),
                p.Threats.Select(MapRecommendationToDto).ToArray()
            )).ToArray();
        }

        private static DataflowAttributeDto MapDataflowAttributeToDto(DataflowAttribute dataflowAttribute)
        {
            return new DataflowAttributeDto(
                dataflowAttribute.Number,
                dataflowAttribute.Transport,
                dataflowAttribute.DataClassification,
                dataflowAttribute.Authentication,
                dataflowAttribute.Notes
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

using Brie.Core.Models;
using Brie.Core.Services;
using Brie.Ui.Requests;
using MediatR;
using System.Security.Cryptography.Xml;

namespace Brie.Ui.Handlers
{
    public record PlanDto(
        string Name,
        string? Description
    );

    public class GetPlansHandler : IRequestHandler<GetPlansRequest, IResult>
    {
        private readonly IPlansService _plansService;

        public GetPlansHandler(IPlansService plansService)
        {
            _plansService = plansService;
        }

        public async Task<IResult> Handle(GetPlansRequest request, CancellationToken cancellationToken)
        {
            var plans = await _plansService.GetAllAsync();
            return Results.Ok(MapPlansToDtos(plans));
        }

        private static IEnumerable<PlanDto>? MapPlansToDtos(IEnumerable<Plan>? plans)
        {
            if (plans is null)
            {
                return new List<PlanDto>();
            }
            
            return plans.Select(p => new PlanDto(
                p.Name,
                p.Description
            )).ToList();
        }
    }
}

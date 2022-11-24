using Brie.Core.Models;
using Brie.Core.Services;
using Brie.Ui.Requests;
using MediatR;
using System.Security.Cryptography.Xml;

namespace Brie.Ui.Handlers
{
    public class GetThreatModelReportHandler : IRequestHandler<GetThreatModelReportRequest, IResult>
    {
        private readonly IThreatModelsService _threatModelsService;

        public GetThreatModelReportHandler(IThreatModelsService threatModelsService)
        {
            _threatModelsService = threatModelsService;
        }

        public async Task<IResult> Handle(GetThreatModelReportRequest request, CancellationToken cancellationToken)
        {
            var report = await _threatModelsService.GetReportAsync(request.Id);
            return Results.File(report);
        }
    }
}

﻿using Brie.Core.Models;
using Brie.Core.Services;
using Brie.Ui.Requests;
using MediatR;
using System.Security.Cryptography.Xml;
using System.Text;

namespace Brie.Ui.Handlers
{
    public class GetThreatModelReportArchiveHandler : IRequestHandler<GetThreatModelReportArchiveRequest, IResult>
    {
        private readonly IThreatModelsService _threatModelsService;

        public GetThreatModelReportArchiveHandler(IThreatModelsService threatModelsService)
        {
            _threatModelsService = threatModelsService;
        }

        public async Task<IResult> Handle(GetThreatModelReportArchiveRequest request, CancellationToken cancellationToken)
        {
            try
            {
                (var archiveContent, var fileName) = await _threatModelsService.GetReportArchiveAsync(request.Id);
                return archiveContent is not null
                    ? Results.File(archiveContent, "application/zip", fileName)
                    : Results.NotFound();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }
    }
}

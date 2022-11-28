using Brie.Core.Models;
using Brie.Core.Services;
using Brie.Ui.Requests;
using MediatR;
using System.Security.Cryptography.Xml;

namespace Brie.Ui.Handlers
{
    public class DeleteThreatModelsHandler : IRequestHandler<DeleteThreatModelRequest, IResult>
    {
        private readonly IThreatModelsService _threatModelsService;

        public DeleteThreatModelsHandler(IThreatModelsService threatModelsService)
        {
            _threatModelsService = threatModelsService;
        }

        public async Task<IResult> Handle(DeleteThreatModelRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await _threatModelsService.DeleteAsync(request.Id);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }
    }
}

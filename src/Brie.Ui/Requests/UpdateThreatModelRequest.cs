using Brie.Ui.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace Brie.Ui.Requests;

public record struct UpdateThreatModelRequest(
    string Id,
    [FromBody]
    CreateThreatModelDto Body
) : IHttpRequest;

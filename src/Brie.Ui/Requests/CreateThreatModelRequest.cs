using Brie.Ui.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace Brie.Ui.Requests;

public record struct CreateThreatModelRequest(
    [FromBody]
    ThreatModelDto Body
) : IHttpRequest;

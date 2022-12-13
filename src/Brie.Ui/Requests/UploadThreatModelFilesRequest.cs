using Brie.Ui.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace Brie.Ui.Requests;

public record struct UploadThreatModelFilesRequest(
    string Id,
    [FromForm]
    IFormFileCollection Files
) : IHttpRequest;

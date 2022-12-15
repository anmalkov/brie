using Brie.Ui.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace Brie.Ui.Requests;

public record struct GetThreatModelFileRequest(
    string Id,
    string FileName
) : IHttpRequest;

namespace Brie.Ui.Requests;

public record struct DeleteThreatModelRequest(
    string Id
) : IHttpRequest;

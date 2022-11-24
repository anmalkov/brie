namespace Brie.Ui.Requests;

public record struct GetThreatModelReportRequest(
    string Id
) : IHttpRequest;

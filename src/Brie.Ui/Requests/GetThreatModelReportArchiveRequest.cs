namespace Brie.Ui.Requests;

public record struct GetThreatModelReportArchiveRequest(
    string Id
) : IHttpRequest;

using MediatR;

namespace Brie.Ui.Requests;

public interface IHttpRequest : IRequest<IResult> { }
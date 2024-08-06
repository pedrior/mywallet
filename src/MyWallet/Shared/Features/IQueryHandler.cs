namespace MyWallet.Shared.Features;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, ErrorOr<TResponse>> 
    where TQuery : IQuery<TResponse>;
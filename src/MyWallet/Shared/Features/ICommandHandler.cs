namespace MyWallet.Shared.Features;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, ErrorOr<TResponse>> 
    where TCommand : ICommand<TResponse>;
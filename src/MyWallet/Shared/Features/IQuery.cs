namespace MyWallet.Shared.Features;

public interface IQuery<T> : IRequest<ErrorOr<T>>;
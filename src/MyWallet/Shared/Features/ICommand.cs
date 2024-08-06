namespace MyWallet.Shared.Features;

public interface ICommand<T> : IRequest<ErrorOr<T>>, ITransactional;
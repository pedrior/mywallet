namespace MyWallet.Shared.Features;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : INotification;
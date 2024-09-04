namespace MyWallet.Shared.Persistence;

public sealed class MigrationException(string message, Exception innerException) : Exception(message, innerException);
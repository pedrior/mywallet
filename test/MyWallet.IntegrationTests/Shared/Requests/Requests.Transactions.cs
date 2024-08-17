namespace MyWallet.IntegrationTests.Shared.Requests;

internal static partial class Requests
{
    public static class Transactions
    {
        public static HttpRequestMessage CreateTransaction(
            Ulid walletId,
            Ulid categoryId,
            string type,
            string name,
            decimal amount,
            string currency,
            DateOnly date)
        {
            return new HttpRequestMessage(HttpMethod.Post, $"{BasePath}/transactions")
            {
                Content = ToJsonStringContent(new
                {
                    walletId,
                    categoryId,
                    type,
                    name,
                    amount,
                    currency,
                    date
                })
            };
        }
    }
}
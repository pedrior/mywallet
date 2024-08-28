using System.Text;

namespace MyWallet.IntegrationTests.Shared.Requests;

internal static partial class Requests
{
    public static class Transactions
    {
        public static HttpRequestMessage ListTransactions(
            Ulid walletId,
            DateOnly from,
            DateOnly? to = null,
            int page = 1,
            int limit = 10)
        {
            var url = new StringBuilder();

            url.Append($"{BasePath}/transactions");
            url.Append($"?walletId={walletId}");
            url.Append($"&from={from}");

            if (to is not null)
            {
                url.Append($"&to={to}");
            }

            url.Append($"&page={page}");
            url.Append($"&limit={limit}");

            return new HttpRequestMessage(HttpMethod.Get, url.ToString());
        }

        public static HttpRequestMessage GetTransaction(Ulid transactionId) =>
            new(HttpMethod.Get, $"{BasePath}/transactions/{transactionId}");

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

        public static HttpRequestMessage EditTransaction(
            Ulid transactionId,
            Ulid? walletId = null,
            Ulid? categoryId = null,
            string? name = null,
            decimal? amount = null,
            string? currency = null,
            DateOnly? date = null)
        {
            return new HttpRequestMessage(HttpMethod.Patch, $"{BasePath}/transactions/{transactionId}")
            {
                Content = ToJsonStringContent(new
                {
                    walletId,
                    categoryId,
                    name,
                    amount,
                    currency,
                    date
                })
            };
        }
    }
}
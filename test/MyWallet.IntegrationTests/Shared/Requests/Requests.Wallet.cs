namespace MyWallet.IntegrationTests.Shared.Requests;

internal static partial class Requests
{
    public static class Wallets
    {
        public static HttpRequestMessage ListWallets(int page, int limit) =>
            new(HttpMethod.Get, $"{BasePath}/wallets?page={page}&limit={limit}");
        
        public static HttpRequestMessage GetWallet(Ulid id) =>
            new(HttpMethod.Get, $"{BasePath}/wallets/{id}");

        public static HttpRequestMessage CreateWallet(string? name = null, string? color = null)
        {
            name ??= Constants.Wallet.Name.Value;
            color ??= Constants.Wallet.Color.Value;

            return new HttpRequestMessage(HttpMethod.Post, $"{BasePath}/wallets")
            {
                Content = ToJsonStringContent(new
                {
                    name,
                    color
                })
            };
        }
    }
}
namespace MyWallet.IntegrationTests.Shared.Requests;

internal static partial class Requests
{
    public static class Wallets
    {
        public static HttpRequestMessage ListWallets(int page, int limit) =>
            new(HttpMethod.Get, $"{BasePath}/wallets?page={page}&limit={limit}");

        public static HttpRequestMessage GetWallet(Ulid id) =>
            new(HttpMethod.Get, $"{BasePath}/wallets/{id}");

        public static HttpRequestMessage CreateWallet(string name, string color, string currency)
        {
            return new HttpRequestMessage(HttpMethod.Post, $"{BasePath}/wallets")
            {
                Content = ToJsonStringContent(new
                {
                    name,
                    color,
                    currency
                })
            };
        }
        
        public static HttpRequestMessage ArchiveWallet(Ulid id) => 
            new(HttpMethod.Post, $"{BasePath}/wallets/{id}/archive");
        
        public static HttpRequestMessage UnarchiveWallet(Ulid id) => 
            new(HttpMethod.Post, $"{BasePath}/wallets/{id}/unarchive");

        public static HttpRequestMessage EditWallet(Ulid id, string name, string color, string currency)
        {
            return new HttpRequestMessage(HttpMethod.Patch, $"{BasePath}/wallets/{id}")
            {
                Content = ToJsonStringContent(new
                {
                    name,
                    color,
                    currency
                })
            };
        }
        
        public static HttpRequestMessage DeleteWallet(Ulid id) =>
            new(HttpMethod.Delete, $"{BasePath}/wallets/{id}");
    }
}
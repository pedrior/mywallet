namespace MyWallet.IntegrationTests.Shared.Requests;

internal static partial class Requests
{
    public static class Wallets
    {
        public static HttpRequestMessage ListWallets(int page, int limit) =>
            new(HttpMethod.Get, $"{BasePath}/wallets?page={page}&limit={limit}");

        public static HttpRequestMessage GetWallet(Ulid id) =>
            new(HttpMethod.Get, $"{BasePath}/wallets/{id}");

        public static HttpRequestMessage CreateWallet(string name, string color)
        {
            return new HttpRequestMessage(HttpMethod.Post, $"{BasePath}/wallets")
            {
                Content = ToJsonStringContent(new
                {
                    name,
                    color
                })
            };
        }
        
        public static HttpRequestMessage ArchiveWallet(Ulid id) => 
            new(HttpMethod.Post, $"{BasePath}/wallets/{id}/archive");

        public static HttpRequestMessage RenameWallet(Ulid id, string name)
        {
            return new HttpRequestMessage(HttpMethod.Post, $"{BasePath}/wallets/{id}/rename")
            {
                Content = ToJsonStringContent(new
                {
                    name
                })
            };
        }
        
        public static HttpRequestMessage DeleteWallet(Ulid id) =>
            new(HttpMethod.Delete, $"{BasePath}/wallets/{id}");
    }
}
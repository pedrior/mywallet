using System.Net.Http.Headers;

namespace MyWallet.IntegrationTests.Shared.Extensions;

public static class HttpResponseHeadersExtensions
{
    public static string? FindLastResourceIdentifier(this HttpResponseHeaders headers)
    {
        return headers.Location?
            .ToString()
            .Split('/')
            .Last();
    }
}
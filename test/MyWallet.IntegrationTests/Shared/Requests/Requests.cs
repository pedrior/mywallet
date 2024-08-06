using System.Text;
using System.Text.Json;

namespace MyWallet.IntegrationTests.Shared.Requests;

internal static partial class Requests
{
    private const string BasePath = "/api/v1";

    private static StringContent ToJsonStringContent<TValue>(TValue value) => new(
        content: JsonSerializer.Serialize(value),
        encoding: Encoding.UTF8,
        mediaType: "application/json");
}
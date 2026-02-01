using System.Text.Json;
using Aya.Contract.Models;
using Aya.Contract.Services;
using Gaia.Models;
using Gaia.Services;

namespace Cai.Services;

public sealed class FilesHttpService(
    HttpClient httpClient,
    JsonSerializerOptions options,
    ITryPolicyService tryPolicyService,
    IFactory<Memory<HttpHeader>> headersFactory
)
    : HttpService<AyaGetRequest, AyaPostRequest, AyaGetResponse, AyaPostResponse>(
        httpClient,
        options,
        tryPolicyService,
        headersFactory
    ),
        IFilesHttpService
{
    protected override AyaGetRequest CreateHealthCheckGetRequest()
    {
        return new();
    }
}

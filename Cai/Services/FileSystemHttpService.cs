using System.Text.Json;
using Aya.Contract.Models;
using Aya.Contract.Services;
using Gaia.Models;
using Gaia.Services;

namespace Cai.Services;

public sealed class FileSystemHttpService(
    IFactory<HttpClient> httpClientFactory,
    JsonSerializerOptions options,
    ITryPolicyService tryPolicyService,
    IFactory<Memory<HttpHeader>> headersFactory
)
    : HttpService<AyaGetRequest, AyaPostRequest, AyaGetResponse, AyaPostResponse>(
        httpClientFactory,
        options,
        tryPolicyService,
        headersFactory
    ),
        IFileSystemHttpService
{
    protected override AyaGetRequest CreateHealthCheckGetRequest()
    {
        return new();
    }
}

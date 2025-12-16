using System.Text.Json;
using Aya.Contract.Models;
using Aya.Contract.Services;
using Gaia.Models;
using Gaia.Services;

namespace Cai.Services;

public class HttpFilesService(
    HttpClient httpClient,
    JsonSerializerOptions jsonSerializerOptions,
    ITryPolicyService tryPolicyService,
    IFactory<Memory<HttpHeader>> headersFactory
)
    : HttpService<AyaGetRequest, AyaPostRequest, AyaGetResponse, AyaPostResponse>(
        httpClient,
        jsonSerializerOptions,
        tryPolicyService,
        headersFactory
    ),
        IHttpFilesService;

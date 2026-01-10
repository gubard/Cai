using Aya.Contract.Models;
using Aya.Contract.Services;
using Inanna.Models;
using Inanna.Services;

namespace Cai.Services;

public interface IUiFilesService
    : IUiService<AyaGetRequest, AyaPostRequest, AyaGetResponse, AyaPostResponse>;

public class UiFilesService(
    IHttpFilesService httpService,
    IEfFilesService efService,
    AppState appState,
    IFilesMemoryCache memoryCache,
    INavigator navigator,
    string serviceName
)
    : UiService<
        AyaGetRequest,
        AyaPostRequest,
        AyaGetResponse,
        AyaPostResponse,
        IHttpFilesService,
        IEfFilesService,
        IFilesMemoryCache
    >(httpService, efService, appState, memoryCache, navigator, serviceName),
        IUiFilesService;

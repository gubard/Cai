using Aya.Contract.Models;
using Aya.Contract.Services;
using Inanna.Models;
using Inanna.Services;

namespace Cai.Services;

public interface IUiFilesService
    : IUiService<AyaGetRequest, AyaPostRequest, AyaGetResponse, AyaPostResponse>;

public class UiFilesService(
    IHttpFilesService service,
    IEfFilesService efService,
    AppState appState,
    IFilesCache cache,
    INavigator navigator
)
    : UiService<
        AyaGetRequest,
        AyaPostRequest,
        AyaGetResponse,
        AyaPostResponse,
        IHttpFilesService,
        IEfFilesService,
        IFilesCache
    >(service, efService, appState, cache, navigator),
        IUiFilesService;

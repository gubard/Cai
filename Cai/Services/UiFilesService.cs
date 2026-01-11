using Aya.Contract.Models;
using Aya.Contract.Services;
using Inanna.Models;
using Inanna.Services;

namespace Cai.Services;

public interface IUiFilesService
    : IUiService<AyaGetRequest, AyaPostRequest, AyaGetResponse, AyaPostResponse>;

public class UiFilesService(
    IHttpFilesService httpService,
    IDbFilesService dbService,
    AppState appState,
    IFilesUiCache uiCache,
    INavigator navigator,
    string serviceName
)
    : UiService<
        AyaGetRequest,
        AyaPostRequest,
        AyaGetResponse,
        AyaPostResponse,
        IHttpFilesService,
        IDbFilesService,
        IFilesUiCache
    >(httpService, dbService, appState, uiCache, navigator, serviceName),
        IUiFilesService;

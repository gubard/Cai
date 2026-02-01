using Aya.Contract.Models;
using Aya.Contract.Services;
using Inanna.Models;
using Inanna.Services;

namespace Cai.Services;

public interface IFilesUiService
    : IUiService<AyaGetRequest, AyaPostRequest, AyaGetResponse, AyaPostResponse>;

public class FilesUiService(
    IFilesHttpService filesHttpService,
    IFilesDbService filesDbService,
    AppState appState,
    IFilesUiCache uiCache,
    INavigator navigator,
    string serviceName,
    IResponseHandler responseHandler
)
    : UiService<
        AyaGetRequest,
        AyaPostRequest,
        AyaGetResponse,
        AyaPostResponse,
        IFilesHttpService,
        IFilesDbService,
        IFilesUiCache
    >(filesHttpService, filesDbService, appState, uiCache, navigator, serviceName, responseHandler),
        IFilesUiService;

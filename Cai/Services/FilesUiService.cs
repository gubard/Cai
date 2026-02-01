using Aya.Contract.Models;
using Aya.Contract.Services;
using Inanna.Models;
using Inanna.Services;

namespace Cai.Services;

public interface IFilesUiService
    : IUiService<AyaGetRequest, AyaPostRequest, AyaGetResponse, AyaPostResponse>;

public class FilesUiService(
    IFileSystemHttpService fileSystemHttpService,
    IFileSystemDbService fileSystemDbService,
    AppState appState,
    IFileSystemUiCache uiCache,
    INavigator navigator,
    string serviceName,
    IResponseHandler responseHandler
)
    : UiService<
        AyaGetRequest,
        AyaPostRequest,
        AyaGetResponse,
        AyaPostResponse,
        IFileSystemHttpService,
        IFileSystemDbService,
        IFileSystemUiCache
    >(
        fileSystemHttpService,
        fileSystemDbService,
        appState,
        uiCache,
        navigator,
        serviceName,
        responseHandler
    ),
        IFilesUiService;

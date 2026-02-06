using Aya.Contract.Models;
using Aya.Contract.Services;
using Inanna.Services;

namespace Cai.Services;

public interface IFileSystemUiService
    : IUiService<AyaGetRequest, AyaPostRequest, AyaGetResponse, AyaPostResponse>;

public sealed class FileSystemUiService(
    IFileSystemHttpService fileSystemHttpService,
    IFileSystemDbService fileSystemDbService,
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
    >(fileSystemHttpService, fileSystemDbService, uiCache, navigator, serviceName, responseHandler),
        IFileSystemUiService
{
    protected override AyaGetRequest CreateGetRequestRefresh()
    {
        return new() { IsGetFiles = true };
    }
}

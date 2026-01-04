using System.Text.Json;
using Aya.Contract.Models;
using Aya.Contract.Services;
using Cai.Models;
using Cai.Ui;
using Gaia.Helpers;
using Gaia.Models;
using Gaia.Services;
using Inanna.Models;
using Inanna.Services;
using Jab;
using Nestor.Db.Services;
using Nestor.Db.Sqlite.Helpers;

namespace Cai.Services;

[ServiceProviderModule]
[Transient(typeof(ICaiViewModelFactory), typeof(CaiViewModelFactory))]
[Transient(typeof(FilesPanelViewModel))]
[Singleton(typeof(IFilesCache), typeof(FilesCache))]
[Transient(typeof(IUiFilesService), Factory = nameof(GetUiFilesService))]
public interface ICaiServiceProvider
{
    public static IUiFilesService GetUiFilesService(
        FilesServiceOptions options,
        ITryPolicyService tryPolicyService,
        IFactory<Memory<HttpHeader>> headersFactory,
        AppState appState,
        IFilesCache toDoCache,
        INavigator navigator,
        IStorageService storageService,
        IMigrator migrator
    )
    {
        var user = appState.User.ThrowIfNull();

        return new UiFilesService(
            new HttpFilesService(
                new() { BaseAddress = new(options.Url) },
                new()
                {
                    TypeInfoResolver = AysJsonContext.Resolver,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                },
                tryPolicyService,
                headersFactory
            ),
            new EfFilesService(
                new FileInfo($"{storageService.GetAppDirectory()}/{user.Id}.db").InitDbContext(
                    migrator
                ),
                new(DateTimeOffset.UtcNow.Offset, user.Id)
            ),
            appState,
            toDoCache,
            navigator
        );
    }
}

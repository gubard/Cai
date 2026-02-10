using Aya.Contract.Models;
using Cai.Services;
using CommunityToolkit.Mvvm.Input;
using Gaia.Helpers;
using Gaia.Services;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;

namespace Cai.Ui;

public partial class FilesPanelHeaderViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly IAppResourceService _appResourceService;
    private readonly IStringFormater _stringFormater;
    private readonly ICaiViewModelFactory _factory;
    private readonly IFileSystemUiService _fileSystemUiService;

    public FilesPanelHeaderViewModel(
        IDialogService dialogService,
        IAppResourceService appResourceService,
        IStringFormater stringFormater,
        ICaiViewModelFactory factory,
        IFileSystemUiService fileSystemUiService
    )
    {
        _dialogService = dialogService;
        _appResourceService = appResourceService;
        _stringFormater = stringFormater;
        _factory = factory;
        _fileSystemUiService = fileSystemUiService;
    }

    [RelayCommand]
    private async Task ShowCreateFtpAsync(CancellationToken ct)
    {
        await WrapCommandAsync(
            () =>
            {
                var creatingNewItem = _appResourceService.GetResource<string>(
                    "Lang.CreatingNewItem"
                );
                var create = _appResourceService.GetResource<string>("Lang.Create");
                var viewModel = _factory.CreateFtpParameters();

                var createFtpButton = new DialogButton(
                    create,
                    CreateFtpCommand,
                    viewModel,
                    DialogButtonType.Primary
                );

                var buttons = new[] { createFtpButton, UiHelper.CancelButton };

                return _dialogService.ShowMessageBoxAsync(
                    new(
                        _stringFormater.Format(creatingNewItem, "FTP").DispatchToDialogHeader(),
                        viewModel,
                        buttons
                    ),
                    ct
                );
            },
            ct
        );
    }

    [RelayCommand]
    private async Task CreateFtpAsync(FtpParametersViewModel viewModel, CancellationToken ct)
    {
        await WrapCommandAsync(() => CreateFtpCore(viewModel, ct).ConfigureAwait(false), ct);
    }

    private async ValueTask<AyaPostResponse> CreateFtpCore(
        FtpParametersViewModel viewModel,
        CancellationToken ct
    )
    {
        var values = viewModel.Host.Split(':');

        await using var client = await FtpClientService.CreateAsync(
            values[0],
            values.Length > 1 ? int.Parse(values[1]) : 21,
            viewModel.Login,
            viewModel.Password,
            ct
        );

        var path = viewModel.Path.IsNullOrWhiteSpace()
            ? (await client.GetCurrenDirectoryAsync(ct)).Path
            : viewModel.Path;

        var response = await _fileSystemUiService.PostAsync(
            Guid.NewGuid(),
            new()
            {
                CreateFiles =
                [
                    new()
                    {
                        Name = viewModel.Name,
                        Type = FileType.Ftp,
                        Host = viewModel.Host,
                        Login = viewModel.Login,
                        Password = viewModel.Password,
                        Id = Guid.NewGuid(),
                        Path = path,
                    },
                ],
            },
            ct
        );

        await _dialogService.CloseMessageBoxAsync(ct);

        return response;
    }
}

using Aya.Contract.Models;
using Cai.Services;
using CommunityToolkit.Mvvm.Input;
using FluentFTP;
using Gaia.Helpers;
using Gaia.Services;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;
using Inanna.Ui;

namespace Cai.Ui;

public partial class FilesPanelHeaderViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly IAppResourceService _appResourceService;
    private readonly IStringFormater _stringFormater;
    private readonly ICaiViewModelFactory _factory;
    private readonly IUiFilesService _uiFilesService;

    public FilesPanelHeaderViewModel(
        IDialogService dialogService,
        IAppResourceService appResourceService,
        IStringFormater stringFormater,
        ICaiViewModelFactory factory,
        IUiFilesService uiFilesService
    )
    {
        _dialogService = dialogService;
        _appResourceService = appResourceService;
        _stringFormater = stringFormater;
        _factory = factory;
        _uiFilesService = uiFilesService;
    }

    [RelayCommand]
    private async Task ShowCreateFtpAsync()
    {
        await WrapCommand(() =>
        {
            var creatingNewItem = _appResourceService.GetResource<string>("Lang.CreatingNewItem");
            var create = _appResourceService.GetResource<string>("Lang.Create");
            var header = _stringFormater.Format(creatingNewItem, "FTP");
            var viewModel = _factory.CreateFtpParameters();

            var createFtpButton = new DialogButton(
                create,
                CreateFtpCommand,
                viewModel,
                DialogButtonType.Primary
            );

            var buttons = new[] { createFtpButton, UiHelper.CancelButton };
            var dialog = new DialogViewModel(header, viewModel, buttons);

            return _dialogService.ShowMessageBoxAsync(dialog);
        });
    }

    [RelayCommand]
    private async Task CreateFtp(FtpParametersViewModel viewModel, CancellationToken ct)
    {
        await WrapCommand(async () =>
        {
            using var client = new FtpClient(viewModel.Host, viewModel.Login, viewModel.Password);
            client.Connect();

            var path = viewModel.Path.IsNullOrWhiteSpace()
                ? client.GetWorkingDirectory()
                : viewModel.Path;

            var response = await _uiFilesService.PostAsync(
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

            _dialogService.CloseMessageBox();

            return response;
        });
    }
}

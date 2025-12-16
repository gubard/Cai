using Avalonia.Collections;
using Cai.Helpers;
using Cai.Models;
using Cai.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gaia.Services;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;
using Inanna.Ui;

namespace Cai.Ui;

public partial class FilesPanelViewModel : ViewModelBase
{
    private readonly AvaloniaList<object> _roots;
    private readonly IDialogService _dialogService;
    private readonly IAppResourceService _appResourceService;
    private readonly IStringFormater _stringFormater;
    private readonly ICaiViewModelFactory _factory;

    [ObservableProperty]
    private object _firstFiles;

    [ObservableProperty]
    private object _secondFiles;

    public FilesPanelViewModel(
        ICaiViewModelFactory factory,
        IStorageService storageService,
        IDialogService dialogService,
        IAppResourceService appResourceService,
        IStringFormater stringFormater
    )
    {
        _factory = factory;
        _dialogService = dialogService;
        _appResourceService = appResourceService;
        _stringFormater = stringFormater;
        _roots = new(RootHelper.Roots.ToArray().OfType<object>()) { AddMark.Instance };
        _firstFiles = factory.Create(storageService.GetDbDirectory());
        _secondFiles = factory.Create(storageService.GetDbDirectory());
    }

    public IEnumerable<object> Roots => _roots;

    [RelayCommand]
    private async Task ShowCreateFtpAsync()
    {
        await WrapCommand(() =>
        {
            var creatingNewItem = _appResourceService.GetResource<string>("Lang.CreatingNewItem");
            var create = _appResourceService.GetResource<string>("Lang.Create");
            var header = _stringFormater.Format(creatingNewItem, "FTP");
            var viewModel = _factory.Create();

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
    private void CreateFtp(CreateFtpViewModel viewModel)
    {
        WrapCommand(() =>
            _roots.Insert(
                _roots.Count - 1,
                new FtpRootDirectory(
                    viewModel.Name,
                    viewModel.Host,
                    viewModel.Login,
                    viewModel.Password
                )
            )
        );
    }

    [RelayCommand]
    private void OpenFirstRootDirectory(RootDirectory file)
    {
        WrapCommand(() =>
        {
            (FirstFiles as IDisposable)?.Dispose();
            FirstFiles = _factory.Create(file.Directory);
        });
    }

    [RelayCommand]
    private void OpenSecondRootDirectory(RootDirectory file)
    {
        WrapCommand(() =>
        {
            (SecondFiles as IDisposable)?.Dispose();
            SecondFiles = _factory.Create(file.Directory);
        });
    }

    [RelayCommand]
    private void OpenFirstFtpRootDirectory(FtpRootDirectory file)
    {
        WrapCommand(() =>
        {
            (FirstFiles as IDisposable)?.Dispose();
            FirstFiles = _factory.Create(file.CreateFtpClient());
        });
    }

    [RelayCommand]
    private void OpenSecondFtpRootDirectory(FtpRootDirectory file)
    {
        WrapCommand(() =>
        {
            (SecondFiles as IDisposable)?.Dispose();
            SecondFiles = _factory.Create(file.CreateFtpClient());
        });
    }
}

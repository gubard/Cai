using Cai.Models;
using Cai.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentFTP;
using Gaia.Services;
using Inanna.Models;
using Inanna.Services;

namespace Cai.Ui;

public partial class FilesPanelViewModel : ViewModelBase, IHeader
{
    private readonly ICaiViewModelFactory _factory;
    private readonly IUiFilesService _uiFilesService;

    [ObservableProperty]
    private object _firstFiles;

    [ObservableProperty]
    private object _secondFiles;

    public FilesPanelViewModel(
        ICaiViewModelFactory factory,
        IStorageService storageService,
        IFilesCache filesCache,
        IUiFilesService uiFilesService
    )
    {
        _factory = factory;
        _uiFilesService = uiFilesService;
        Roots = filesCache.Roots;
        _firstFiles = factory.Create(storageService.GetDbDirectory());
        _secondFiles = factory.Create(storageService.GetDbDirectory());
        Header = _factory.CreateFilesPanelHeader();
    }

    public object Header { get; }
    public IEnumerable<RootDirectory> Roots { get; }

    [RelayCommand]
    private async Task InitializedAsync(CancellationToken ct)
    {
        await WrapCommand(() => _uiFilesService.GetAsync(new() { IsGetFiles = true }, ct));
    }

    [RelayCommand]
    private void OpenFirstRootDirectory(RootDirectory file)
    {
        WrapCommand(() =>
        {
            (FirstFiles as IDisposable)?.Dispose();

            FirstFiles = file switch
            {
                DriveRootDirectory drive => _factory.Create(new DirectoryInfo(drive.Drive.Name)),
                FtpRootDirectory ftp => _factory.Create(
                    new FtpClient(ftp.Host, ftp.Login, ftp.Password)
                ),
                LocalRootDirectory local => _factory.Create(local.Directory),
                _ => throw new ArgumentOutOfRangeException(nameof(file)),
            };
        });
    }

    [RelayCommand]
    private void OpenSecondRootDirectory(RootDirectory file)
    {
        WrapCommand(() =>
        {
            (SecondFiles as IDisposable)?.Dispose();

            SecondFiles = file switch
            {
                DriveRootDirectory drive => _factory.Create(new DirectoryInfo(drive.Drive.Name)),
                FtpRootDirectory ftp => _factory.Create(
                    new FtpClient(ftp.Host, ftp.Login, ftp.Password)
                ),
                LocalRootDirectory local => _factory.Create(local.Directory),
                _ => throw new ArgumentOutOfRangeException(nameof(file)),
            };
        });
    }
}

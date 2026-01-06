using Cai.Models;
using Cai.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gaia.Services;
using Inanna.Models;
using Inanna.Services;
using File = Cai.Models.File;

namespace Cai.Ui;

public partial class FilesPanelViewModel : ViewModelBase, IHeader
{
    private readonly ICaiViewModelFactory _factory;
    private readonly IUiFilesService _uiFilesService;

    [ObservableProperty]
    private IFilesView _firstFiles;

    [ObservableProperty]
    private IFilesView _secondFiles;

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

        _firstFiles = factory.Create(
            (storageService.GetDbDirectory(), CopyFromFirstToSecondCommand)
        );

        _secondFiles = factory.Create(
            (storageService.GetDbDirectory(), CopyFromSecondToFirstCommand)
        );

        Header = _factory.CreateFilesPanelHeader();
    }

    public object Header { get; }
    public IEnumerable<RootDirectory> Roots { get; }

    [RelayCommand]
    private async Task InitializedAsync(CancellationToken ct)
    {
        await WrapCommandAsync(() => _uiFilesService.GetAsync(new() { IsGetFiles = true }, ct), ct);
    }

    [RelayCommand]
    private void OpenFirstRootDirectory(RootDirectory file)
    {
        WrapCommand(() =>
        {
            FirstFiles.Dispose();

            FirstFiles = file switch
            {
                DriveRootDirectory drive => _factory.Create(
                    (new(drive.Drive.Name), CopyFromFirstToSecondCommand)
                ),
                FtpRootDirectory ftp => _factory.Create(
                    (new(ftp.Host, ftp.Login, ftp.Password), ftp.Path, CopyFromFirstToSecondCommand)
                ),
                LocalRootDirectory local => _factory.Create(
                    (local.Directory, CopyFromFirstToSecondCommand)
                ),
                _ => throw new ArgumentOutOfRangeException(nameof(file)),
            };
        });
    }

    [RelayCommand]
    private void OpenSecondRootDirectory(RootDirectory file)
    {
        WrapCommand(() =>
        {
            SecondFiles.Dispose();

            SecondFiles = file switch
            {
                DriveRootDirectory drive => _factory.Create(
                    (new(drive.Drive.Name), CopyFromSecondToFirstCommand)
                ),
                FtpRootDirectory ftp => _factory.Create(
                    (new(ftp.Host, ftp.Login, ftp.Password), ftp.Path, CopyFromSecondToFirstCommand)
                ),
                LocalRootDirectory local => _factory.Create(
                    (local.Directory, CopyFromSecondToFirstCommand)
                ),
                _ => throw new ArgumentOutOfRangeException(nameof(file)),
            };
        });
    }

    [RelayCommand]
    private async Task CopyFromFirstToSecondAsync(IEnumerable<File> files, CancellationToken ct)
    {
        await WrapCommandAsync(
            () =>
            {
                var fileData = files.Where(x => x.Name != "..").SelectMany(x => x.GetFileData());

                return SecondFiles.SaveFilesAsync(fileData, ct);
            },
            ct
        );
    }

    [RelayCommand]
    private async Task CopyFromSecondToFirstAsync(IEnumerable<File> files, CancellationToken ct)
    {
        await WrapCommandAsync(
            () =>
            {
                var fileData = files.Where(x => x.Name != "..").SelectMany(x => x.GetFileData());

                return FirstFiles.SaveFilesAsync(fileData, ct);
            },
            ct
        );
    }

    [RelayCommand]
    private async Task DeleteRootDirectoryAsync(RootDirectory file, CancellationToken ct)
    {
        await WrapCommandAsync(
            () => _uiFilesService.PostAsync(Guid.NewGuid(), new() { DeleteIds = [file.Id] }, ct),
            ct
        );
    }
}

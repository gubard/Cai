using System.Runtime.CompilerServices;
using Aya.Contract.Models;
using Cai.Models;
using Cai.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gaia.Helpers;
using Gaia.Services;
using Inanna.Models;
using Inanna.Services;
using File = Cai.Models.File;

namespace Cai.Ui;

public partial class FilesPanelViewModel : ViewModelBase, IHeader, IInitUi
{
    public FilesPanelViewModel(
        ICaiViewModelFactory factory,
        IStorageService storageService,
        IFileSystemUiCache uiCache,
        IFileSystemUiService fileSystemUiService
    )
    {
        _factory = factory;
        _fileSystemUiService = fileSystemUiService;
        Roots = uiCache.Roots;

        _firstFiles = factory.Create(
            (storageService.GetDbDirectory(), CopyFromFirstToSecondCommand)
        );

        _secondFiles = factory.Create(
            (storageService.GetDbDirectory(), CopyFromSecondToFirstCommand)
        );

        Header = _factory.CreateFilesPanelHeader();
    }

    public object Header { get; }
    public IEnumerable<FileNotify> Roots { get; }

    public ConfiguredValueTaskAwaitable InitUiAsync(CancellationToken ct)
    {
        return InitCore(ct).ConfigureAwait(false);
    }

    private readonly ICaiViewModelFactory _factory;
    private readonly IFileSystemUiService _fileSystemUiService;

    [ObservableProperty]
    private IFilesView _firstFiles;

    [ObservableProperty]
    private IFilesView _secondFiles;

    public async ValueTask InitCore(CancellationToken ct)
    {
        await _fileSystemUiService.GetAsync(new() { IsGetFiles = true }, ct);
    }

    [RelayCommand]
    private void OpenFirstRootDirectory(FileNotify file)
    {
        WrapCommand(() =>
        {
            FirstFiles.Dispose();

            FirstFiles = file.Type switch
            {
                FileType.Ftp => _factory.Create(
                    (
                        new(file.Host, file.Login, file.Password),
                        file.Path,
                        CopyFromFirstToSecondCommand
                    )
                ),
                FileType.Local => _factory.Create(
                    (file.Path.ToDir(), CopyFromFirstToSecondCommand)
                ),
                _ => throw new ArgumentOutOfRangeException(nameof(file)),
            };
        });
    }

    [RelayCommand]
    private void OpenSecondRootDirectory(FileNotify file)
    {
        WrapCommand(() =>
        {
            SecondFiles.Dispose();

            SecondFiles = file.Type switch
            {
                FileType.Ftp => _factory.Create(
                    (
                        new(file.Host, file.Login, file.Password),
                        file.Path,
                        CopyFromFirstToSecondCommand
                    )
                ),
                FileType.Local => _factory.Create(
                    (file.Path.ToDir(), CopyFromFirstToSecondCommand)
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
    private async Task DeleteRootDirectoryAsync(FileNotify file, CancellationToken ct)
    {
        await WrapCommandAsync(
            () =>
                _fileSystemUiService.PostAsync(Guid.NewGuid(), new() { DeleteIds = [file.Id] }, ct),
            ct
        );
    }
}

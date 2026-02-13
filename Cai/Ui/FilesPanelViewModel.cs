using System.Runtime.CompilerServices;
using Aya.Contract.Models;
using Cai.Models;
using Cai.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gaia.Helpers;
using Gaia.Services;
using IconPacks.Avalonia.MaterialDesign;
using Inanna.Models;
using Inanna.Services;
using File = Cai.Models.File;

namespace Cai.Ui;

public sealed partial class FilesPanelViewModel : ViewModelBase, IHeader, IInitUi
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

        _firstFiles = factory.CreateFileSystem(
            storageService.GetDbDirectory(),
            CopyFromFirstToSecondCommand
        );

        _secondFiles = factory.CreateFileSystem(
            storageService.GetDbDirectory(),
            CopyFromSecondToFirstCommand
        );

        Header = _factory.CreateFilesPanelHeader();
    }

    public object Header { get; }
    public IEnumerable<FileNotify> Roots { get; }

    public ConfiguredValueTaskAwaitable InitUiAsync(CancellationToken ct)
    {
        return WrapCommandAsync(
            async () =>
            {
                await FirstFiles.InitUiAsync(ct);
                await SecondFiles.InitUiAsync(ct);

                return await _fileSystemUiService.GetAsync(new() { IsGetFiles = true }, ct);
            },
            ct
        );
    }

    private readonly ICaiViewModelFactory _factory;
    private readonly IFileSystemUiService _fileSystemUiService;

    [ObservableProperty]
    private IFilesView _firstFiles;

    [ObservableProperty]
    private IFilesView _secondFiles;

    [RelayCommand]
    private async Task OpenFirstRootDirectoryAsync(FileNotify file, CancellationToken ct)
    {
        await WrapCommandAsync(
            async () =>
            {
                FirstFiles.Dispose();
                var view = await CreateFilesView(file, ct);
                await view.InitUiAsync(ct);
                FirstFiles = view;
            },
            ct
        );
    }

    [RelayCommand]
    private async Task OpenSecondRootDirectoryAsync(FileNotify file, CancellationToken ct)
    {
        await WrapCommandAsync(
            async () =>
            {
                SecondFiles.Dispose();
                var view = await CreateFilesView(file, ct);
                await view.InitUiAsync(ct);
                SecondFiles = view;
            },
            ct
        );
    }

    private async ValueTask<IFilesView> CreateFilesView(FileNotify file, CancellationToken ct)
    {
        switch (file.Type)
        {
            case FileType.Ftp:
                var values = file.Host.Split(':');

                var client = await FtpClientService.CreateAsync(
                    values[0],
                    values.Length == 1 ? 21 : int.Parse(values[1]),
                    file.Login,
                    file.Password,
                    ct
                );

                var item = await client.GetCurrenDirectoryAsync(ct);

                return _factory.CreateFtpFiles(
                    client,
                    new(item, client),
                    CopyFromFirstToSecondCommand,
                    new(file.Host, file.Login, file.Password)
                );
            case FileType.Local:
                return _factory.CreateFileSystem(file.Path.ToDir(), CopyFromFirstToSecondCommand);
            default:
                throw new ArgumentOutOfRangeException(nameof(file));
        }
    }

    [RelayCommand]
    private async Task CopyFromFirstToSecondAsync(IEnumerable<File> files, CancellationToken ct)
    {
        await WrapCommandAsync(
            async () =>
            {
                var list = new List<FileData>();

                foreach (var file in files)
                {
                    if (file.Name == "..")
                    {
                        continue;
                    }

                    var items = await file.GetFileDataAsync(ct);
                    list.AddRange(items);
                }

                await SecondFiles.SaveFilesAsync(list, ct);
            },
            ct
        );
    }

    [RelayCommand]
    private async Task CopyFromSecondToFirstAsync(IEnumerable<File> files, CancellationToken ct)
    {
        await WrapCommandAsync(
            async () =>
            {
                var list = new List<FileData>();

                foreach (var file in files)
                {
                    if (file.Name == "..")
                    {
                        continue;
                    }

                    var items = await file.GetFileDataAsync(ct);
                    list.AddRange(items);
                }

                await SecondFiles.SaveFilesAsync(list, ct);
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

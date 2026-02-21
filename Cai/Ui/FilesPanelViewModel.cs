using System.Runtime.CompilerServices;
using System.Windows.Input;
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

public sealed partial class FilesPanelViewModel : ViewModelBase, IHeader, IInit
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

    public ConfiguredValueTaskAwaitable InitAsync(CancellationToken ct)
    {
        return WrapCommandAsync(
            async () =>
            {
                await FirstFiles.InitAsync(ct);
                await SecondFiles.InitAsync(ct);

                return await _fileSystemUiService.GetAsync(new() { IsGetFiles = true }, ct);
            },
            ct
        );
    }

    [ObservableProperty]
    private IFilesView _firstFiles;

    [ObservableProperty]
    private IFilesView _secondFiles;
    private readonly ICaiViewModelFactory _factory;
    private readonly IFileSystemUiService _fileSystemUiService;

    [RelayCommand]
    private async Task OpenFirstRootDirectoryAsync(FileNotify file, CancellationToken ct)
    {
        await WrapCommandAsync(
            async () =>
            {
                FirstFiles.Dispose();
                var view = await CreateFilesView(file, CopyFromFirstToSecondCommand, ct);
                await view.InitAsync(ct);
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
                var view = await CreateFilesView(file, CopyFromSecondToFirstCommand, ct);
                await view.InitAsync(ct);
                SecondFiles = view;
            },
            ct
        );
    }

    [RelayCommand]
    private async Task CopyFromFirstToSecondAsync(IEnumerable<File> files, CancellationToken ct)
    {
        await CopyFromToAsync(FirstFiles, SecondFiles, files, ct);
    }

    [RelayCommand]
    private async Task CopyFromSecondToFirstAsync(IEnumerable<File> files, CancellationToken ct)
    {
        await CopyFromToAsync(SecondFiles, FirstFiles, files, ct);
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

    private ConfiguredValueTaskAwaitable CopyFromToAsync(
        IFilesView baseView,
        IFilesView saveView,
        IEnumerable<File> files,
        CancellationToken ct
    )
    {
        return WrapCommandAsync(
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

                await saveView.SaveFilesAsync(list, baseView.BasePath, ct);
            },
            ct
        );
    }

    private async ValueTask<IFilesView> CreateFilesView(
        FileNotify file,
        ICommand copyCommand,
        CancellationToken ct
    )
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
                    copyCommand,
                    new(file.Host, file.Login, file.Password)
                );
            case FileType.Local:
                return _factory.CreateFileSystem(file.Path.ToDir(), copyCommand);
            default:
                throw new ArgumentOutOfRangeException(nameof(file));
        }
    }
}

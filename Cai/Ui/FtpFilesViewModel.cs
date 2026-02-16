using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Avalonia.Collections;
using Aya.Contract.Models;
using Cai.Models;
using Cai.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gaia.Models;
using Gaia.Services;
using IconPacks.Avalonia.MaterialDesign;
using Inanna.Models;
using Inanna.Services;

namespace Cai.Ui;

public sealed partial class FtpFilesViewModel : ViewModelBase, IFilesView
{
    public FtpFilesViewModel(
        IFtpClientService ftpClient,
        FtpFile directory,
        ICommand copyCommand,
        IFileSystemUiService fileSystemUiService,
        IClipboardService clipboardService,
        FtpParameters ftpParameters
    )
    {
        _files = [];
        _selectedFiles = [];
        _ftpClient = ftpClient;
        CopyCommand = copyCommand;
        _fileSystemUiService = fileSystemUiService;
        _clipboardService = clipboardService;
        _ftpParameters = ftpParameters;
        _directory = directory;
        BasePath = directory.Item.Path;
    }

    public IEnumerable<FtpFile> Files => _files;
    public ICommand CopyCommand { get; }
    public IAvaloniaReadOnlyList<FtpFile> SelectedFiles => _selectedFiles;
    public string BasePath { get; }

    public void Dispose()
    {
        _ftpClient.Dispose();
    }

    public ConfiguredValueTaskAwaitable SaveFilesAsync(
        IEnumerable<FileData> files,
        string basePath,
        CancellationToken ct
    )
    {
        return SaveFilesCore(files, basePath, ct).ConfigureAwait(false);
    }

    protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        switch (e.PropertyName)
        {
            case nameof(Directory):
            {
                await WrapCommandAsync(
                    () => UpdateAsync(CancellationToken.None),
                    CancellationToken.None
                );

                break;
            }
        }
    }

    public void OpenFile(FtpFile file)
    {
        WrapCommand(() =>
        {
            switch (file.Item.Type)
            {
                case FtpItemType.Directory:
                {
                    Directory = file;

                    break;
                }
            }
        });
    }

    [ObservableProperty]
    private FtpFile _directory;

    private readonly IFtpClientService _ftpClient;
    private readonly IFileSystemUiService _fileSystemUiService;
    private readonly AvaloniaList<FtpFile> _files;
    private readonly AvaloniaList<FtpFile> _selectedFiles;
    private readonly IClipboardService _clipboardService;
    private readonly FtpParameters _ftpParameters;

    private async ValueTask SaveFilesCore(
        IEnumerable<FileData> files,
        string basePath,
        CancellationToken ct
    )
    {
        await using var dis = new FinallyAsync(async () => await UpdateAsync(ct));

        foreach (var file in files)
        {
            await using var dispose = file;
            var fileSegment = file.Path.Substring(basePath.Length + 1);
            var remotePath = Path.Combine(Directory.Item.Path, fileSegment).Replace('\\', '/');
            await _ftpClient.UploadItemAsync(remotePath, file.Stream, ct);
        }
    }

    private async ValueTask UpdateAsync(CancellationToken ct)
    {
        _files.Clear();

        var lastIndex = Directory.Item.Path.LastIndexOf('\\');

        if (lastIndex == -1)
        {
            lastIndex = Directory.Item.Path.LastIndexOf('/');
        }

        if (lastIndex != -1)
        {
            var path = Directory.Item.Path.Substring(0, lastIndex);
            var isExists = await _ftpClient.IsExistsAsync(path, ct);

            if (isExists)
            {
                _files.Add(
                    new(
                        "..",
                        PackIconMaterialDesignKind.Undo,
                        await _ftpClient.GetItemAsync(path, ct),
                        _ftpClient
                    )
                );
            }
        }

        var items = (await _ftpClient.GetListItemAsync(Directory.Item.Path, ct)).ToArray();

        var directories = items
            .Where(x => x.Type == FtpItemType.Directory)
            .OrderBy(x => Path.GetFileName(x.Path))
            .Select(x => new FtpFile(x, _ftpClient));

        var files = items
            .Where(x => x.Type == FtpItemType.File)
            .OrderBy(x => Path.GetFileName(x.Path))
            .Select(x => new FtpFile(x, _ftpClient));

        _files.AddRange(directories);
        _files.AddRange(files);
    }

    [RelayCommand]
    private async Task SaveDirectoryAsync(CancellationToken ct)
    {
        await WrapCommandAsync(
            () =>
                _fileSystemUiService.PostAsync(
                    Guid.NewGuid(),
                    new()
                    {
                        CreateFiles =
                        [
                            new()
                            {
                                Name = Path.GetFileName(Directory.Item.Path),
                                Id = Guid.NewGuid(),
                                Path = Directory.Item.Path,
                                Type = FileType.Ftp,
                                Host = _ftpParameters.Host,
                                Login = _ftpParameters.Login,
                                Password = _ftpParameters.Password,
                            },
                        ],
                    },
                    ct
                ),
            ct
        );
    }

    [RelayCommand]
    private async Task CopyFullPathAsync(FtpFile ftpFile, CancellationToken ct)
    {
        await WrapCommandAsync(() => _clipboardService.SetTextAsync(ftpFile.Item.Path, ct), ct);
    }

    [RelayCommand]
    private async Task DeleteAsync(CancellationToken ct)
    {
        await WrapCommandAsync(
            async () =>
            {
                foreach (var selectedFile in SelectedFiles)
                {
                    if (selectedFile.Name == "..")
                    {
                        continue;
                    }

                    await _ftpClient.DeleteItemAsync(selectedFile.Item.Path, ct);
                }

                await UpdateAsync(ct);
            },
            ct
        );
    }

    public ConfiguredValueTaskAwaitable InitUiAsync(CancellationToken ct)
    {
        return WrapCommandAsync(() => UpdateAsync(ct), ct);
    }
}

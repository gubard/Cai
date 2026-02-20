using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Avalonia.Collections;
using Aya.Contract.Models;
using Cai.Models;
using Cai.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gaia.Helpers;
using Gaia.Models;
using IconPacks.Avalonia.MaterialDesign;
using Inanna.Models;
using Inanna.Services;

namespace Cai.Ui;

public sealed partial class FileSystemViewModel : ViewModelBase, IFilesView
{
    [ObservableProperty]
    private DirectoryInfo _directory;
    private readonly AvaloniaList<LocalFile> _files;
    private readonly AvaloniaList<LocalFile> _selectedFiles;
    private readonly IFileSystemUiService _fileSystemUiService;
    private readonly IClipboardService _clipboardService;

    public FileSystemViewModel(
        DirectoryInfo directory,
        ICommand copyCommand,
        IFileSystemUiService fileSystemUiService,
        IClipboardService clipboardService
    )
    {
        _directory = directory;
        CopyCommand = copyCommand;
        _fileSystemUiService = fileSystemUiService;
        _clipboardService = clipboardService;
        _files = [];
        _selectedFiles = [];
        BasePath = directory.FullName;
    }

    public IEnumerable<LocalFile> Files => _files;
    public ICommand CopyCommand { get; }
    public IAvaloniaReadOnlyList<LocalFile> SelectedFiles => _selectedFiles;
    public string BasePath { get; }

    public void Dispose() { }

    public ConfiguredValueTaskAwaitable SaveFilesAsync(
        IEnumerable<FileData> files,
        string basePath,
        CancellationToken ct
    )
    {
        return SaveFilesCore(files, basePath, ct).ConfigureAwait(false);
    }

    public void OpenFile(LocalFile localFile)
    {
        WrapCommand(() =>
        {
            switch (localFile.Item)
            {
                case DirectoryInfo directoryInfo:
                {
                    Directory = directoryInfo;

                    break;
                }
            }
        });
    }

    private async ValueTask SaveFilesCore(
        IEnumerable<FileData> files,
        string basePath,
        CancellationToken ct
    )
    {
        using var dis = new Finally(Update);

        foreach (var file in files)
        {
            await using var dispose = file;
            var fileSegment = file.Path.Substring(basePath.Length + 1);
            var path = Path.Combine(Directory.FullName, fileSegment);
            var localFile = new FileInfo(path.Replace('\\', '/'));

            if (localFile.Exists)
            {
                localFile.Delete();
            }

            if (localFile.Directory is not null && !localFile.Directory.Exists)
            {
                localFile.Directory.Create();
            }

            await using var stream = localFile.Create();
            await file.Stream.CopyToAsync(stream, ct);
        }
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        switch (e.PropertyName)
        {
            case nameof(Directory):
            {
                Update();
                break;
            }
        }
    }

    [RelayCommand]
    private void Delete()
    {
        WrapCommand(() =>
        {
            foreach (var selectedFile in SelectedFiles)
            {
                if (selectedFile.Name == "..")
                {
                    continue;
                }

                switch (selectedFile.Item)
                {
                    case DirectoryInfo directory:
                        directory.Delete(true);

                        break;
                    case FileInfo file:
                        file.Delete();

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            Update();
        });
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
                                Name = Directory.Name,
                                Id = Guid.NewGuid(),
                                Path = Directory.FullName,
                                Type = FileType.Local,
                                Icon = nameof(PackIconMaterialDesignKind.Folder),
                            },
                        ],
                    },
                    ct
                ),
            ct
        );
    }

    private void Update()
    {
        _files.Clear();

        if (Directory.Parent != null)
        {
            _files.Add(new("..", PackIconMaterialDesignKind.Undo, Directory.Parent));
        }

        var directories = Directory
            .GetDirectories()
            .OrderBy(x => x.Name)
            .Select(x => new LocalFile(x));

        _files.AddRange(directories);
        _files.AddRange(Directory.GetFiles().OrderBy(x => x.Name).Select(x => new LocalFile(x)));
    }

    [RelayCommand]
    private async Task CopyFullPathAsync(LocalFile localFile, CancellationToken ct)
    {
        await WrapCommandAsync(
            () => _clipboardService.SetTextAsync(localFile.Item.FullName, ct),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable InitAsync(CancellationToken ct)
    {
        WrapCommand(Update);

        return TaskHelper.ConfiguredCompletedTask;
    }
}

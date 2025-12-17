using System.ComponentModel;
using System.Windows.Input;
using Avalonia.Collections;
using Aya.Contract.Models;
using Cai.Models;
using Cai.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gaia.Models;
using IconPacks.Avalonia.MaterialDesign;
using Inanna.Models;
using Inanna.Services;

namespace Cai.Ui;

public partial class FilesViewModel : ViewModelBase, IFilesView
{
    [ObservableProperty]
    private DirectoryInfo _directory;
    private readonly AvaloniaList<LocalFile> _files;
    private readonly AvaloniaList<LocalFile> _selectedFiles;
    private readonly IUiFilesService _uiFilesService;
    private readonly IClipboardService _clipboardService;

    public FilesViewModel(
        DirectoryInfo directory,
        ICommand copyCommand,
        IUiFilesService uiFilesService,
        IClipboardService clipboardService
    )
    {
        _directory = directory;
        CopyCommand = copyCommand;
        _uiFilesService = uiFilesService;
        _clipboardService = clipboardService;
        _files = [];
        _selectedFiles = [];
        Update();
    }

    public IEnumerable<LocalFile> Files => _files;
    public ICommand CopyCommand { get; }
    public IAvaloniaReadOnlyList<LocalFile> SelectedFiles => _selectedFiles;

    public void Dispose() { }

    public async ValueTask SaveFilesAsync(IEnumerable<FileData> files, CancellationToken ct)
    {
        using var dis = new Dis(Update);

        foreach (var file in files)
        {
            await using var dispose = file;
            var localFile = new FileInfo(Path.Combine(Directory.FullName, file.Path));

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
                _uiFilesService.PostAsync(
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
                            },
                        ],
                    },
                    ct
                ),
            ct
        );
    }

    [RelayCommand]
    private void OpenFile(LocalFile localFile)
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
}

using System.ComponentModel;
using System.Windows.Input;
using Avalonia.Collections;
using Cai.Models;
using Cai.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gaia.Models;
using IconPacks.Avalonia.MaterialDesign;
using Inanna.Models;

namespace Cai.Ui;

public partial class FilesViewModel : ViewModelBase, IFilesView
{
    [ObservableProperty]
    private DirectoryInfo _directory;
    private readonly AvaloniaList<LocalFile> _files;
    private readonly AvaloniaList<LocalFile> _selected;

    public FilesViewModel(DirectoryInfo directory, ICommand copyCommand)
    {
        _directory = directory;
        CopyCommand = copyCommand;
        _files = [];
        _selected = [];
        Update();
    }

    public IEnumerable<LocalFile> Files => _files;
    public ICommand CopyCommand { get; }
    public IAvaloniaReadOnlyList<LocalFile> Selected => _selected;

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
}

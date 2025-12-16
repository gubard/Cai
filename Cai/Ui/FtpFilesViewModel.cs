using System.ComponentModel;
using System.Windows.Input;
using Avalonia.Collections;
using Cai.Models;
using Cai.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentFTP;
using Gaia.Models;
using IconPacks.Avalonia.MaterialDesign;
using Inanna.Models;

namespace Cai.Ui;

public partial class FtpFilesViewModel : ViewModelBase, IFilesView
{
    [ObservableProperty]
    private FtpFile _directory;
    private readonly FtpClient _ftpClient;
    private readonly AvaloniaList<FtpFile> _files;
    private readonly AvaloniaList<FtpFile> _selected;

    public FtpFilesViewModel(FtpClient ftpClient, ICommand copyCommand)
    {
        _files = [];
        _selected = [];
        _ftpClient = ftpClient;
        CopyCommand = copyCommand;
        ftpClient.Connect();
        var workingDirectory = ftpClient.GetWorkingDirectory();
        var item = ftpClient.GetObjectInfo(workingDirectory);
        _directory = new(item, ftpClient);
        Update();
    }

    public IEnumerable<FtpFile> Files => _files;
    public ICommand CopyCommand { get; }
    public IAvaloniaReadOnlyList<FtpFile> Selected => _selected;

    public void Dispose()
    {
        _ftpClient.Dispose();
    }

    public async ValueTask SaveFilesAsync(IEnumerable<FileData> files, CancellationToken ct)
    {
        using var dis = new Dis(Update);

        foreach (var file in files)
        {
            await using var dispose = file;
            var remotePath = Path.Combine(Directory.Item.FullName, file.Path);

            if (_ftpClient.FileExists(remotePath))
            {
                _ftpClient.DeleteFile(remotePath);
            }

            if (!_ftpClient.DirectoryExists(Path.GetDirectoryName(remotePath)))
            {
                _ftpClient.CreateDirectory(Path.GetDirectoryName(remotePath));
            }

            _ftpClient.UploadStream(file.Stream, remotePath);
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

    private void Update()
    {
        _files.Clear();

        var lastIndex = Directory.Item.FullName.LastIndexOf('\\');

        if (lastIndex == -1)
        {
            lastIndex = Directory.Item.FullName.LastIndexOf('/');
        }

        if (lastIndex != -1)
        {
            var item = _ftpClient.GetObjectInfo(Directory.Item.FullName.Substring(0, lastIndex));

            if (item is not null)
            {
                _files.Add(new("..", PackIconMaterialDesignKind.Undo, item, _ftpClient));
            }
        }

        var items = _ftpClient.GetListing(Directory.Item.FullName, FtpListOption.AllFiles);

        var directories = items
            .Where(x => x.Type == FtpObjectType.Directory)
            .OrderBy(x => x.Name)
            .Select(x => new FtpFile(x, _ftpClient));

        var files = items
            .Where(x => x.Type == FtpObjectType.File)
            .OrderBy(x => x.Name)
            .Select(x => new FtpFile(x, _ftpClient));

        _files.AddRange(directories);
        _files.AddRange(files);
    }

    [RelayCommand]
    private void OpenFile(FtpFile file)
    {
        WrapCommand(() =>
        {
            switch (file.Item.Type)
            {
                case FtpObjectType.Directory:
                {
                    Directory = file;

                    break;
                }
            }
        });
    }
}

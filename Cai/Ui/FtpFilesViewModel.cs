using System.ComponentModel;
using Avalonia.Collections;
using Cai.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentFTP;
using IconPacks.Avalonia.MaterialDesign;
using Inanna.Models;

namespace Cai.Ui;

public partial class FtpFilesViewModel : ViewModelBase, IDisposable
{
    [ObservableProperty]
    private FtpFile _directory;
    private readonly FtpClient _ftpClient;
    private readonly AvaloniaList<FtpFile> _files;

    public FtpFilesViewModel(FtpClient ftpClient)
    {
        _files = [];
        _ftpClient = ftpClient;
        ftpClient.Connect();
        var workingDirectory = ftpClient.GetWorkingDirectory();
        var item = ftpClient.GetObjectInfo(workingDirectory);
        _directory = new(item);
        Update();
    }

    public IEnumerable<FtpFile> Files => _files;

    public void Dispose()
    {
        _ftpClient.Dispose();
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

        if (Directory.Item.LinkObject != null)
        {
            _files.Add(new("..", PackIconMaterialDesignKind.Undo, Directory.Item.LinkObject));
        }

        var items = _ftpClient.GetListing(Directory.Item.FullName);

        var directories = items
            .Where(x => x.Type == FtpObjectType.Directory)
            .OrderBy(x => x.Name)
            .Select(x => new FtpFile(x));

        var files = items
            .Where(x => x.Type == FtpObjectType.File)
            .OrderBy(x => x.Name)
            .Select(x => new FtpFile(x));

        _files.AddRange(directories);
        _files.AddRange(files);
    }
}

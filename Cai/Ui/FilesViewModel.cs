using System.ComponentModel;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IconPacks.Avalonia.MaterialDesign;
using Inanna.Models;
using File = Cai.Models.File;

namespace Cai.Ui;

public partial class FilesViewModel : ViewModelBase
{
    [ObservableProperty]
    private DirectoryInfo _directory;
    private readonly AvaloniaList<File> _files;

    public FilesViewModel(DirectoryInfo directory)
    {
        _directory = directory;
        _files = [];
        Update();
    }

    public IEnumerable<File> Files => _files;

    [RelayCommand]
    private void OpenFile(File file)
    {
        WrapCommand(() =>
        {
            switch (file.Item)
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

        var directories = Directory.GetDirectories().OrderBy(x => x.Name).Select(x => new File(x));

        _files.AddRange(directories);
        _files.AddRange(Directory.GetFiles().OrderBy(x => x.Name).Select(x => new File(x)));
    }
}

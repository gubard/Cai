using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Cai.Models;

namespace Cai.Ui;

public partial class FilesView : UserControl
{
    public FilesView()
    {
        InitializeComponent();
    }

    public FilesViewModel ViewModel =>
        DataContext as FilesViewModel ?? throw new InvalidOperationException();

    private void FileBorderOnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not IDataContextProvider dataContextProvider)
        {
            return;
        }

        if (dataContextProvider.DataContext is not LocalFile localFile)
        {
            return;
        }

        e.Handled = true;
        ViewModel.OpenFile(localFile);
    }
}

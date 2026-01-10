using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Cai.Models;

namespace Cai.Ui;

public partial class FtpFilesView : UserControl
{
    public FtpFilesView()
    {
        InitializeComponent();
    }

    public FtpFilesViewModel ViewModel =>
        DataContext as FtpFilesViewModel ?? throw new InvalidOperationException();

    private void FileBorderOnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not IDataContextProvider dataContextProvider)
        {
            return;
        }

        if (dataContextProvider.DataContext is not FtpFile localFile)
        {
            return;
        }

        e.Handled = true;
        ViewModel.OpenFile(localFile);
    }
}

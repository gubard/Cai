using Avalonia.Controls;

namespace Cai.Ui;

public partial class FtpFilesView : UserControl
{
    public FtpFilesView()
    {
        InitializeComponent();
    }

    public FtpFilesViewModel ViewModel =>
        DataContext as FtpFilesViewModel ?? throw new InvalidOperationException();
}

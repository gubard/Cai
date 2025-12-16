using Avalonia.Controls;

namespace Cai.Ui;

public partial class FilesView : UserControl
{
    public FilesView()
    {
        InitializeComponent();
    }

    public FilesViewModel ViewModel =>
        DataContext as FilesViewModel ?? throw new InvalidOperationException();
}

using Avalonia.Controls;

namespace Cai.Ui;

public sealed partial class FilesPanelView : UserControl
{
    public FilesPanelView()
    {
        InitializeComponent();
    }

    public FilesPanelViewModel ViewModel =>
        DataContext as FilesPanelViewModel ?? throw new InvalidOperationException();
}

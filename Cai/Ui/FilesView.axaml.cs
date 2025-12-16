using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using File = Cai.Models.File;

namespace Cai.Ui;

public partial class FilesView : UserControl
{
    public FilesView()
    {
        InitializeComponent();
    }

    private void ItemsControlOnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.ClickCount != 2)
        {
            return;
        }

        if (DataContext is not FilesViewModel viewModel)
        {
            return;
        }

        if (e.Source is not Visual visual)
        {
            return;
        }

        if (visual is not Border border)
        {
            var ancestor = visual.FindAncestorOfType<Border>();

            if (ancestor is null)
            {
                return;
            }

            border = ancestor;
        }

        if (border.DataContext is not File file)
        {
            return;
        }

        viewModel.OpenFileCommand.Execute(file);
        e.Handled = true;
    }
}

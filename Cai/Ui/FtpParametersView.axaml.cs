using Avalonia.Controls;
using Avalonia.Input;
using Inanna.Helpers;

namespace Cai.Ui;

public sealed partial class FtpParametersView : UserControl
{
    public FtpParametersView()
    {
        InitializeComponent();
        Loaded += (_, _) => TextBoxName.FocusCaretIndex();
    }

    private void TextBoxNameOnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        TextBoxHost.FocusCaretIndex();
        e.Handled = true;
    }

    private void TextBoxHostOnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        TextBoxLogin.FocusCaretIndex();
        e.Handled = true;
    }

    private void TextBoxLoginOnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        TextBoxPassword.FocusCaretIndex();
        e.Handled = true;
    }

    private void TextBoxPasswordOnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        TextBoxPath.FocusCaretIndex();
        e.Handled = true;
    }
}

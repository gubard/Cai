using Avalonia.Controls;
using Avalonia.Input;

namespace Cai.Ui;

public partial class FtpParametersView : UserControl
{
    public FtpParametersView()
    {
        InitializeComponent();
    }

    private void TextBoxNameOnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        TextBoxHost.Focus();
        e.Handled = true;
    }

    private void TextBoxHostOnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        TextBoxLogin.Focus();
        e.Handled = true;
    }

    private void TextBoxLoginOnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        TextBoxPassword.Focus();
        e.Handled = true;
    }

    private void TextBoxPasswordOnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        TextBoxPath.Focus();
        e.Handled = true;
    }
}

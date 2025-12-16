using CommunityToolkit.Mvvm.ComponentModel;
using Inanna.Models;

namespace Cai.Ui;

public partial class CreateFtpViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _host;

    [ObservableProperty]
    private string _login;

    [ObservableProperty]
    private string _password;

    public CreateFtpViewModel()
    {
        _name = string.Empty;
        _host = string.Empty;
        _login = string.Empty;
        _password = string.Empty;
    }
}

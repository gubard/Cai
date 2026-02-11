using CommunityToolkit.Mvvm.ComponentModel;
using Inanna.Models;

namespace Cai.Ui;

public sealed partial class FtpParametersViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _host = string.Empty;

    [ObservableProperty]
    private string _login = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _path = string.Empty;
}

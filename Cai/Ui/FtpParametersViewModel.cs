using CommunityToolkit.Mvvm.ComponentModel;
using Inanna.Models;
using Inanna.Services;

namespace Cai.Ui;

public sealed partial class FtpParametersViewModel : ViewModelBase
{
    public FtpParametersViewModel(ISafeExecuteWrapper safeExecuteWrapper)
        : base(safeExecuteWrapper) { }

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

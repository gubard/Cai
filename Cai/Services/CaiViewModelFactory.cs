using System.Windows.Input;
using Cai.Ui;
using FluentFTP;
using Gaia.Services;
using Inanna.Services;

namespace Cai.Services;

public interface ICaiViewModelFactory
    : IFactory<(DirectoryInfo directory, ICommand copyCommand), FilesViewModel>,
        IFactory<FtpParametersViewModel>,
        IFactory<(FtpClient ftpClient, string path, ICommand copyCommand), FtpFilesViewModel>,
        IFactory<FilesPanelHeaderViewModel>
{
    FtpParametersViewModel CreateFtpParameters();
    FilesPanelHeaderViewModel CreateFilesPanelHeader();
}

public class CaiViewModelFactory : ICaiViewModelFactory
{
    private readonly IDialogService _dialogService;
    private readonly IAppResourceService _appResourceService;
    private readonly IStringFormater _stringFormater;
    private readonly IUiFilesService _uiFilesService;

    public CaiViewModelFactory(
        IDialogService dialogService,
        IAppResourceService appResourceService,
        IStringFormater stringFormater,
        IUiFilesService uiFilesService
    )
    {
        _dialogService = dialogService;
        _appResourceService = appResourceService;
        _stringFormater = stringFormater;
        _uiFilesService = uiFilesService;
    }

    public FilesViewModel Create((DirectoryInfo directory, ICommand copyCommand) input)
    {
        return new(input.directory, input.copyCommand, _uiFilesService);
    }

    FtpParametersViewModel IFactory<FtpParametersViewModel>.Create()
    {
        return CreateFtpParameters();
    }

    public FtpFilesViewModel Create((FtpClient ftpClient, string path, ICommand copyCommand) input)
    {
        return new(input.ftpClient, input.path, input.copyCommand, _uiFilesService);
    }

    FilesPanelHeaderViewModel IFactory<FilesPanelHeaderViewModel>.Create()
    {
        return CreateFilesPanelHeader();
    }

    public FtpParametersViewModel CreateFtpParameters()
    {
        return new();
    }

    public FilesPanelHeaderViewModel CreateFilesPanelHeader()
    {
        return new(_dialogService, _appResourceService, _stringFormater, this, _uiFilesService);
    }
}

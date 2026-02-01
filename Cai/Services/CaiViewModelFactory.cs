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
    private readonly IFilesUiService _filesUiService;
    private readonly IClipboardService _clipboardService;

    public CaiViewModelFactory(
        IDialogService dialogService,
        IAppResourceService appResourceService,
        IStringFormater stringFormater,
        IFilesUiService filesUiService,
        IClipboardService clipboardService
    )
    {
        _dialogService = dialogService;
        _appResourceService = appResourceService;
        _stringFormater = stringFormater;
        _filesUiService = filesUiService;
        _clipboardService = clipboardService;
    }

    public FilesViewModel Create((DirectoryInfo directory, ICommand copyCommand) input)
    {
        return new(input.directory, input.copyCommand, _filesUiService, _clipboardService);
    }

    FtpParametersViewModel IFactory<FtpParametersViewModel>.Create()
    {
        return CreateFtpParameters();
    }

    public FtpFilesViewModel Create((FtpClient ftpClient, string path, ICommand copyCommand) input)
    {
        return new(
            input.ftpClient,
            input.path,
            input.copyCommand,
            _filesUiService,
            _clipboardService
        );
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
        return new(_dialogService, _appResourceService, _stringFormater, this, _filesUiService);
    }
}

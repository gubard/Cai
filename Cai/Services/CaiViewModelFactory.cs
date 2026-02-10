using System.Windows.Input;
using Cai.Models;
using Cai.Ui;
using Gaia.Services;
using Inanna.Services;

namespace Cai.Services;

public interface ICaiViewModelFactory
{
    FtpParametersViewModel CreateFtpParameters();
    FilesPanelHeaderViewModel CreateFilesPanelHeader();
    FileSystemViewModel CreateFileSystem(DirectoryInfo directory, ICommand copyCommand);

    FtpFilesViewModel CreateFtpFiles(
        IFtpClientService ftpClient,
        FtpFile file,
        ICommand copyCommand,
        FtpParameters ftpParameters
    );
}

public sealed class CaiViewModelFactory : ICaiViewModelFactory
{
    private readonly IDialogService _dialogService;
    private readonly IAppResourceService _appResourceService;
    private readonly IStringFormater _stringFormater;
    private readonly IFileSystemUiService _fileSystemUiService;
    private readonly IClipboardService _clipboardService;

    public CaiViewModelFactory(
        IDialogService dialogService,
        IAppResourceService appResourceService,
        IStringFormater stringFormater,
        IFileSystemUiService fileSystemUiService,
        IClipboardService clipboardService
    )
    {
        _dialogService = dialogService;
        _appResourceService = appResourceService;
        _stringFormater = stringFormater;
        _fileSystemUiService = fileSystemUiService;
        _clipboardService = clipboardService;
    }

    public FtpFilesViewModel CreateFtpFiles(
        IFtpClientService ftpClient,
        FtpFile file,
        ICommand copyCommand,
        FtpParameters ftpParameters
    )
    {
        return new(
            ftpClient,
            file,
            copyCommand,
            _fileSystemUiService,
            _clipboardService,
            ftpParameters
        );
    }

    public FileSystemViewModel CreateFileSystem(DirectoryInfo directory, ICommand copyCommand)
    {
        return new(directory, copyCommand, _fileSystemUiService, _clipboardService);
    }

    public FtpParametersViewModel CreateFtpParameters()
    {
        return new();
    }

    public FilesPanelHeaderViewModel CreateFilesPanelHeader()
    {
        return new(
            _dialogService,
            _appResourceService,
            _stringFormater,
            this,
            _fileSystemUiService
        );
    }
}

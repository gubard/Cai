using System.Windows.Input;
using Cai.Models;
using Cai.Ui;
using Gaia.Services;
using Inanna.Services;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Cai.Services;

public interface ICaiViewModelFactory
{
    FtpParametersViewModel CreateFtpParameters();
    FilesPanelHeaderViewModel CreateFilesPanelHeader();
    FileSystemViewModel CreateFileSystem(DirectoryInfo directory, ICommand copyCommand);
    FilesPanelViewModel CreateFilesPanel();

    FtpFilesViewModel CreateFtpFiles(
        IFtpClientService ftpClient,
        FtpFile file,
        ICommand copyCommand,
        FtpParameters ftpParameters
    );
}

public sealed class CaiViewModelFactory : ICaiViewModelFactory
{
    public CaiViewModelFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public FilesPanelViewModel CreateFilesPanel()
    {
        return new(
            this,
            _serviceProvider.GetService<IStorageService>(),
            _serviceProvider.GetService<IFileSystemUiCache>(),
            _serviceProvider.GetService<IFileSystemUiService>(),
            _serviceProvider.GetService<ISafeExecuteWrapper>()
        );
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
            _serviceProvider.GetService<IFileSystemUiService>(),
            _serviceProvider.GetService<IClipboardService>(),
            ftpParameters,
            _serviceProvider.GetService<ISafeExecuteWrapper>()
        );
    }

    public FileSystemViewModel CreateFileSystem(DirectoryInfo directory, ICommand copyCommand)
    {
        return new(
            directory,
            copyCommand,
            _serviceProvider.GetService<IFileSystemUiService>(),
            _serviceProvider.GetService<IClipboardService>(),
            _serviceProvider.GetService<ISafeExecuteWrapper>()
        );
    }

    public FtpParametersViewModel CreateFtpParameters()
    {
        return new(_serviceProvider.GetService<ISafeExecuteWrapper>());
    }

    public FilesPanelHeaderViewModel CreateFilesPanelHeader()
    {
        return new(
            _serviceProvider.GetService<IDialogService>(),
            _serviceProvider.GetService<IAppResourceService>(),
            _serviceProvider.GetService<IStringFormater>(),
            this,
            _serviceProvider.GetService<IFileSystemUiService>(),
            _serviceProvider.GetService<ISafeExecuteWrapper>()
        );
    }

    private readonly IServiceProvider _serviceProvider;
}

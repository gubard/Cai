using Cai.Ui;
using FluentFTP;
using Gaia.Services;

namespace Cai.Services;

public interface ICaiViewModelFactory
    : IFactory<DirectoryInfo, FilesViewModel>,
        IFactory<CreateFtpViewModel>,
        IFactory<FtpClient, FtpFilesViewModel>;

public class CaiViewModelFactory : ICaiViewModelFactory
{
    public FilesViewModel Create(DirectoryInfo directory)
    {
        return new(directory);
    }

    public CreateFtpViewModel Create()
    {
        return new();
    }

    public FtpFilesViewModel Create(FtpClient input)
    {
        return new(input);
    }
}

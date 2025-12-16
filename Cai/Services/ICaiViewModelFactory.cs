using Cai.Ui;
using Gaia.Services;

namespace Cai.Services;

public interface ICaiViewModelFactory : IFactory<DirectoryInfo, FilesViewModel>;

public class CaiViewModelFactory : ICaiViewModelFactory
{
    public FilesViewModel Create(DirectoryInfo input)
    {
        return new(input);
    }
}

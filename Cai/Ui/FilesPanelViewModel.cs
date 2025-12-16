using Cai.Services;
using Gaia.Services;
using Inanna.Models;

namespace Cai.Ui;

public class FilesPanelViewModel : ViewModelBase
{
    public FilesPanelViewModel(ICaiViewModelFactory factory, IStorageService storageService)
    {
        FirstFiles = factory.Create(storageService.GetDbDirectory());
        SecondFiles = factory.Create(storageService.GetDbDirectory());
    }

    public FilesViewModel FirstFiles { get; }
    public FilesViewModel SecondFiles { get; }
}

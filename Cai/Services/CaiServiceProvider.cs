using Aya.Contract.Services;
using Cai.Ui;
using Jab;

namespace Cai.Services;

[ServiceProviderModule]
[Transient(typeof(ICaiViewModelFactory), typeof(CaiViewModelFactory))]
[Transient(typeof(FilesPanelViewModel))]
[Singleton(typeof(IFilesMemoryCache), typeof(FilesMemoryCache))]
public interface ICaiServiceProvider;

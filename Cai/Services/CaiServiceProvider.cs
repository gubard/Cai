using Cai.Ui;
using Jab;

namespace Cai.Services;

[ServiceProviderModule]
[Transient(typeof(ICaiViewModelFactory), typeof(CaiViewModelFactory))]
[Transient(typeof(FilesPanelViewModel))]
[Singleton(typeof(IFileSystemMemoryCache), typeof(FileSystemMemoryCache))]
public interface ICaiServiceProvider;

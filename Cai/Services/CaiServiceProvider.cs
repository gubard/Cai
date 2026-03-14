using Jab;

namespace Cai.Services;

[ServiceProviderModule]
[Transient(typeof(ICaiViewModelFactory), typeof(CaiViewModelFactory))]
[Singleton(typeof(IFileSystemMemoryCache), typeof(FileSystemMemoryCache))]
public interface ICaiServiceProvider;

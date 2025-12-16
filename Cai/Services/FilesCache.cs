using Avalonia.Collections;
using Aya.Contract.Models;
using Cai.Helpers;
using Cai.Models;
using Gaia.Services;
using Inanna.Helpers;

namespace Cai.Services;

public interface IFilesCache : ICache<AyaGetResponse>, ICache<AyaPostRequest>
{
    IEnumerable<RootDirectory> Roots { get; }
}

public class FilesCache : IFilesCache
{
    private readonly AvaloniaList<RootDirectory> _roots;

    public FilesCache()
    {
        _roots = new(DriveHelper.Roots.ToArray().OfType<RootDirectory>());
    }

    public IEnumerable<RootDirectory> Roots => _roots;

    public void Update(AyaGetResponse source)
    {
        _roots.UpdateOrder(
            DriveHelper
                .Roots.ToArray()
                .Concat(
                    source
                        .Files.Where(x =>
                            (x.Type == FileType.Local && Directory.Exists(x.Path))
                            || x.Type != FileType.Local
                        )
                        .Select(x =>
                            x.Type switch
                            {
                                FileType.Ftp => (RootDirectory)
                                    new FtpRootDirectory(
                                        x.Id,
                                        x.Name,
                                        x.Host,
                                        x.Login,
                                        x.Password,
                                        x.Path
                                    ),
                                FileType.Local => new LocalRootDirectory(x.Id, new(x.Path)),
                                _ => throw new ArgumentOutOfRangeException(),
                            }
                        )
                )
                .ToArray()
        );
    }

    public void Update(AyaPostRequest source)
    {
        _roots.AddRange(
            source.CreateFiles.Select(x =>
                x.Type switch
                {
                    FileType.Ftp => (RootDirectory)
                        new FtpRootDirectory(x.Id, x.Name, x.Host, x.Login, x.Password, x.Path),
                    FileType.Local => new LocalRootDirectory(x.Id, new(x.Path)),
                    _ => throw new ArgumentOutOfRangeException(),
                }
            )
        );

        _roots.RemoveAll(_roots.Where(x => source.DeleteIds.Contains(x.Id)));
    }
}

using Sonnenberg.Core.Models;

namespace Sonnenberg.Core.Interfaces
{
    /// <summary>
    /// Resolves the context of items clicked in Windows Explorer
    /// </summary>
    public interface IItemContextResolver
    {
        ItemContext ResolveContext(string folderPath, string[] selectedItemPaths);
    }
}

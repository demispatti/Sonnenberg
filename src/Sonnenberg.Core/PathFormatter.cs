using System;

namespace Sonnenberg.Core;

public static class PathFormatter
{
    public static string Format(string path, bool useForwardSlashes)
    {
        if (path is null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        return useForwardSlashes ? path.Replace('\\', '/') : path;
    }
}

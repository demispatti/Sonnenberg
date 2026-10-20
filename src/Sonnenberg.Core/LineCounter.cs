using System;
using System.IO;

namespace Sonnenberg.Core;

public static class LineCounter
{
    public static int Count(TextReader reader, bool omitEmptyLines)
    {
        if (reader is null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var count = 0;
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            if (!omitEmptyLines || line.Length > 0)
            {
                count++;
            }
        }

        return count;
    }
}

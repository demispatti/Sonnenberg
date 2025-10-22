using System;
using System.Windows.Forms;

namespace Sonnenberg.Features.CopyPath
{
    /// <summary>
    /// Service for copying file and folder paths to the clipboard
    /// </summary>
    public class PathCopyService
    {
        public void CopyPath(string path, bool useForwardSlashes = false)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path cannot be null or empty", nameof(path));

            var processedPath = useForwardSlashes
                ? path.Replace('\\', '/')
                : path;

            Clipboard.Clear();
            Clipboard.SetText(processedPath);
        }
    }
}

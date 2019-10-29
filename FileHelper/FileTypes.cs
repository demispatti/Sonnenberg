using System;
using System.Linq;

namespace Sonnenberg.FileHelper
{
    /// <summary>
    /// The class responsible for determining what type of file we're dealing with.
    /// By now, this method distinguishes between text files, image files and folder items.
    /// </summary>
    /// <see cref="Sonnenberg.FileHelper.FileExtensions" />
    public class FileTypes
    {
        private FileExtensions _fileExtensions;

        public FileTypes()
        {
            SetFileExtensions();
        }

        /// <summary>
        /// Sets an instance of the class that interacts with
        /// the JSON resource files containing lists of file extensions.
        /// </summary>
        private void SetFileExtensions()
        {
            _fileExtensions = new FileExtensions();
        }

        /// <summary>
        /// Returns a string containing the file type.
        /// "supported": all file types except the blacklisted ones.
        /// </summary>
        /// <returns>string "text" || "folder" || "supported" || "unsupported"</returns>
        private string FileType(string ext)
        {
            if (_fileExtensions.TextFileExtensions.Any(s => s.Equals(ext, StringComparison.OrdinalIgnoreCase))) return "text";
            
            if (0 == ext.Length) return "folder";

            return _fileExtensions.AllSupportedFileExtensions.Any(s => s.Equals(ext, StringComparison.OrdinalIgnoreCase)) ? "supported" : "unsupported";
        }

        /// <summary>
        /// Returns a string containing the file type.
        /// </summary>
        /// <param name="ext"></param>
        /// <returns>string "ext" || "unsupported"</returns>
        public string GetFileType(string ext)
        {
            return null == ext ? "unsupported" : FileType(ext);
        }
    }
}
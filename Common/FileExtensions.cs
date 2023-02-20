using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Log = log4net.LogManager;

namespace Sonnenberg.Common
{
    /// <summary>
    /// The class responsible for interacting with the file extensions library.
    /// The file extensions library consists of a bunch of JSON resource files that contain
    /// lists of file extensions grouped by file type, e.g. text, code, audio, video etc.
    /// So it's the main purpose of this class to read in lists of file extensions.
    /// These extensions are used to determine which functions will be made available
    /// from within the Windows Explorer context menu.
    /// </summary>
    /// <seealso cref="Logger" />
    public class FileExtensions
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FileExtensions));

        private readonly List<string> _allSupportedFileExtensions = new List<string>(new[]
        {
            "AudioFiles", "BackupFiles", "CadFiles", "CameraRawFiles", "CodeFiles", "CompressedFiles", "DatabaseFiles",
            "DataFiles", "DeveloperFiles",
            "DiskImageFiles", "EbookFiles", "EncodedFiles", "ExecutableFiles", "FontFiles", "GameFiles", "GisFiles",
            "MiscFiles", "PageLayoutFiles",
            "PluginFiles", "RasterImageFiles", "SettingsFiles", "SpreadsheetFiles", "SystemFiles", "TextFiles",
            "VectorImageFiles", "VideoFiles", "WebFiles"
        });

        // @todo: sort out *.lnk
        private readonly List<string> _blacklistedFileExtensions = new List<string>(new[]
        {
            ".lnks"
        });

        private readonly List<string> _textFileExtensions = new List<string>(new[]
        {
            "CodeFiles", "DatabaseFiles", "DataFiles", "DeveloperFiles", "DiskImageFiles", "EbookFiles", "EncodedFiles",
            "ExecutableFiles", "GisFiles", "MiscFiles",
            "PageLayoutFiles", "SettingsFiles", "SpreadsheetFiles", "SystemFiles", "TextFiles", "WebFiles"
        });

        public FileExtensions()
        {
            ConfigureLogger();
            SetFileExtensions();
            BlacklistedFileExtensions = _blacklistedFileExtensions;
        }

        internal List<string> TextFileExtensions { get; set; }

        internal List<string> AllSupportedFileExtensions { get; set; }

        public List<string> BlacklistedFileExtensions { get; }

        private void SetFileExtensions()
        {
            TextFileExtensions = SetFileTypesExtensionsList(_textFileExtensions);
            AllSupportedFileExtensions = SetFileTypesExtensionsList(_allSupportedFileExtensions);
        }

        private void ConfigureLogger()
        {
            Logger.Configure();
        }

        /// <summary>
        /// Sets the respective file extensions lists on the properties.
        /// </summary>
        /// <param name="fileTypesList"></param>
        /// <returns>List{string}</returns>
        private static List<string> SetFileTypesExtensionsList(IEnumerable<string> fileTypesList)
        {
            var list = new List<string>();
            if (fileTypesList != null)
                foreach (var fileType in fileTypesList)
                {
                    var listItem = GetFileTypeExtensionsList(fileType);
                    if (null != listItem)
                    {
                        list.AddRange(listItem);
                    }
                }

            return list;
        }

        /// <summary>
        /// Returns an Enum containing all resource names that are embedded in this application.
        /// It will be used to verify that we only process known JSON-files (of whom we know they are compatible).
        /// </summary>
        /// <returns>IEnumerable{string}</returns>
        private static IEnumerable<string> GetManifestResourcesList()
        {
            var resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            return resourceNames;
        }

        /// <summary>
        /// Returns an Enum containing all file extensions that are currently requested to be checked against.
        /// </summary>
        /// <returns>IEnumerable{string}</returns>
        private static IEnumerable<string> GetFileTypeExtensionsList(string type)
        {
            var extensions = new List<string>();
            var manifestResourcesList = GetManifestResourcesList();
            var resourceName = $"Sonnenberg.Common.Resources.Files.{type}Extensions.json";

            if (!manifestResourcesList.Contains(resourceName))
            {
                Log.Error(
                    $"Resource not found:  + resourceName + (FileExtensions/ type: {type}, ResourceName: {resourceName})");

                throw new FileNotFoundException(
                    $"Resource not found:  + resourceName + (FileExtensions/ type: {type}, ResourceName: {resourceName})");
            }

            try
            {
                dynamic str = GetDataFileStream(resourceName);
                JArray responseString = JsonConvert.DeserializeObject(str);

                extensions.AddRange(from item in responseString.Children()
                                    select item.Children<JProperty>()
                    into itemProperties
                                    select itemProperties.Last()
                    into element
                                    select element.Value.ToString());

                return extensions;
            }
            catch (FileNotFoundException ex)
            {
                Log.Error($"{ex.Message} (FileExtensions, {type})");

                throw;
            }
            catch (ArgumentNullException ex)
            {
                Log.Error($"{ex.Message} (FileExtensions, {type})");

                throw;
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message} (FileExtensions, {type})");

                throw;
            }
        }

        /// <summary>
        /// Returns a string containing a JSON object which hosts a group of file extensions associated with their names.
        /// A group can consist of any value given by <c>_allSupportedFileExtensions</c>.
        /// </summary>
        /// <see cref="_allSupportedFileExtensions" />
        /// <returns>string</returns>
        private static string GetDataFileStream(string resourceName)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var stream = assembly.GetManifestResourceStream(resourceName);

                if (null != stream)
                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }

                throw new ArgumentNullException($"(FileExtensions, {resourceName})");
            }
            catch (ArgumentNullException ex)
            {
                Log.Error($"{ex.Message} (FileExtensions, {resourceName})");

                throw;
            }
            catch (ObjectDisposedException ex)
            {
                Log.Error($"{ex.Message} (FileExtensions, {resourceName})");

                throw;
            }
            catch (IOException ex)
            {
                Log.Error($"{ex.Message} (FileExtensions, {resourceName})");

                throw;
            }
        }
    }
}
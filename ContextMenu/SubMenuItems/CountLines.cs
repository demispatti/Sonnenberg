using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using log4net;
using Sonnenberg.Common;
using Sonnenberg.ContextMenu.Properties;
using Sonnenberg.Language;
using Clipboard = System.Windows.Clipboard;
using Log = log4net.LogManager;
using MessageBox = System.Windows.MessageBox;

namespace Sonnenberg.ContextMenu.SubMenuItems
{
    /// <summary>
    ///     The class responsible the count lines functionality.
    ///     Optionally, blank lines can be omitted.
    ///     It is basically a wrapper for <c>File.ReadAllLines</c>
    /// </summary>
    /// <remarks>
    ///     - Creates a ToolStripMenuItem
    ///     - Creates a click action
    ///     - Counts all lines from any supported text file
    ///     - With or without blank lines
    ///     - Communicates the result via <c>MessageBox</c>
    /// </remarks>
    /// <seealso cref="ContextMenu" />
    /// <seealso cref="Logger" />
    internal class CountLines : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CountLines));
        private bool _disposedValue;

        internal CountLines()
        {
            ConfigureLogger();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private static void ConfigureLogger()
        {
            Logger.Configure();
        }

        internal ToolStripMenuItem ItemDisplay(ToolStripMenuItem toolStripMenuItem, string clickedItemPath,
            string selectedItemPath, bool isDarkTheme)
        {
            if ("Text" == new FileTypes().GetFileType(Path.GetExtension(clickedItemPath))) return toolStripMenuItem;

            var countLines = new CountLines();
            var countLinesMenuItem = countLines.CreateItem(selectedItemPath, false, isDarkTheme);
            toolStripMenuItem.DropDownItems.Add(countLinesMenuItem);

            var countCleanLinesMenuItem = countLines.CreateItem(selectedItemPath, true, isDarkTheme);
            toolStripMenuItem.DropDownItems.Add(countCleanLinesMenuItem);
            countLines.Dispose();

            return toolStripMenuItem;
        }

        private ToolStripMenuItem CreateItem(string selectedItemPath, bool doOmitEmptyLines,
            bool isDarkTheme)
        {
            var icon = GetIcon(doOmitEmptyLines, isDarkTheme);

            var toolStripMenuItem = new ToolStripMenuItem
            {
                Text = GetText(doOmitEmptyLines),
                Image = icon.ToBitmap()
            };

            //  Add click action.
            toolStripMenuItem.Click += (sender, args) => DoClickAction(selectedItemPath, doOmitEmptyLines);
            icon.Dispose();

            return toolStripMenuItem;
        }

        private string GetText(bool doOmitEmptyLines)
        {
            return doOmitEmptyLines ? Strings.countLinesNoEmptyOnes : Strings.countAllLines;
        }

        private Icon GetIcon(bool doOmitEmptyLines, bool isDarkTheme)
        {
            Icon icon;

            if (doOmitEmptyLines)
                icon = isDarkTheme
                    ? Resources.Countlines_Omit_Blank_Lines_dark_theme
                    : Resources.Countlines_Omit_Blank_Lines_light_theme;
            else
                icon = isDarkTheme ? Resources.Countlines_dark_theme : Resources.Countlines_light_theme;

            return new Icon(icon, 40, 40);
        }

        private static void DoClickAction(string selectedItemPath, bool clean)
        {
            var filePath = selectedItemPath;
            var builder = new StringBuilder();
            var lineCount = 0;
            try
            {
                if (clean)
                    using (var readerlines = File.OpenText(filePath))
                    {
                        string line;
                        while ((line = readerlines.ReadLine()) != null)
                            if (!string.IsNullOrEmpty(line))
                                lineCount++;
                    }
                else
                    lineCount = File.ReadAllLines(filePath).Length;
            }
            catch (PathTooLongException ex)
            {
                Log.Error($"{ex.Message} (CountLines)");

                throw;
            }
            catch (IOException ex)
            {
                Log.Error($"{ex.Message} (CountLines)");

                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error($"{ex.Message} (CountLines)");

                throw;
            }

            builder.AppendLine($"{Strings.fileName}: {Path.GetFileName(filePath)}\n {lineCount} {Strings.lines}");

            //  Show the output.
            ShowMessageBox(builder.ToString(), lineCount);
        }

        private static void ShowMessageBox(string text, int lineCount)
        {
            var result = MessageBox.Show(
                $"{text}\n\n{Strings.copyResultToClipboard}",
                Strings.linesCountMsgBoxTitle,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result != MessageBoxResult.Yes) return;
            Clipboard.Clear();
            Clipboard.SetText($"{lineCount}");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing)
            {
                _disposedValue = true;
                Dispose();
            }
        }
    }
}
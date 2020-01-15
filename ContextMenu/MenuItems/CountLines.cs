using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using log4net;
using Sonnenberg.Common;
using Sonnenberg.Language;
using Sonnenberg.ContextMenu.Properties;
using Clipboard = System.Windows.Clipboard;
using MessageBox = System.Windows.MessageBox;

namespace Sonnenberg.ContextMenu.MenuItems
{
	/// <summary>
	/// The class responsible the count lines functionality.
	/// Optionally, blank lines can be omitted.
	/// It is basically a wrapper for <c>File.ReadAllLines</c>
	/// </summary>
	/// <remarks>
	/// - Creates a ToolStripMenuItem
	/// - Creates a click action
	/// - Counts all lines from any supported text file
	/// - With or without blank lines
	/// - Communicates the result via <c>MessageBox</c>
	/// </remarks>
	/// <seealso cref="Program" />
	/// <seealso cref="Logger" />
	internal class CountLines
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(CountLines));

		internal CountLines()
		{
			ConfigureLogger();
		}

		private static void ConfigureLogger()
		{
			Logger.Configure();
		}

		internal ToolStripMenuItem CreateToolStripMenuItem(IEnumerable<string> selectedItemPaths, bool doOmitEmptyLines = false)
		{
			var toolStripMenuItem = new ToolStripMenuItem
			{
				Text = doOmitEmptyLines ? Strings.countLinesNoEmptyOnes : Strings.countAllLines,
				Image = doOmitEmptyLines ? Resources.imgCountLinesNoEmptyOnes : Resources.imgCountAllLines
			};

			//  Add click action.
			toolStripMenuItem.Click += (sender, args) => DoClickAction(selectedItemPaths, doOmitEmptyLines);

			return toolStripMenuItem;
		}

		private static void DoClickAction(IEnumerable<string> selectedItemPaths, bool clean)
		{
			var filePath = selectedItemPaths.First();

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
				log.Error($"{ex.Message} (CountLines)");

				throw;
			}
			catch (IOException ex)
			{
				log.Error($"{ex.Message} (CountLines)");

				throw;
			}
			catch (UnauthorizedAccessException ex)
			{
				log.Error($"{ex.Message} (CountLines)");

				throw;
			}

			builder.AppendLine($"{Strings.fileName}: {Path.GetFileName(filePath)}\n {lineCount.ToString()} {Strings.lines}");

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
			Clipboard.SetText($"{lineCount.ToString()}");
		}
	}
}
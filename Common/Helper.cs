using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using log4net;
using Microsoft.Win32;
using Sonnenberg.Language;
using Log = log4net.LogManager;

namespace Sonnenberg.Common
{
    public sealed class Helper
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Helper));

        public Bitmap CreateIcon(Icon icon)
        {
            return icon.ToBitmap();
        }

        /// <summary>
        ///     Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public bool IsDarkMode()
        {
            try
            {
                using (var key =
                       Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    var val = key?.GetValue("SystemUsesLightTheme");
                    if (val != null) return "0" == val.ToString();

                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Info(Strings.readWindowsThemeRegKeyFailed);
                MessageBox.Show($"{Strings.readWindowsThemeRegKeyFailed} ({ex.Message})");

                throw;
            }
        }
    }
}
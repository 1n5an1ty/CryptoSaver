using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CryptoSaver.Core.Logging;

namespace CryptoSaver.Core.ExtensionMethods
{
    public static class BitmapExtensions
    {
        private static readonly object This = typeof(BitmapExtensions);
        /// <summary>
        /// Converts an image to an image source.
        /// Returns null if exception
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <returns>System.Windows.Media.ImageSource.</returns>
        public static ImageSource ToImageSource(this Bitmap bitmap)
        {
            try
            {
                if (bitmap == null) return null;
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    bitmap.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Exception e)
            {
                This.Log().Error("Could not process image", e);
                return null;
            }
        }
    }
}

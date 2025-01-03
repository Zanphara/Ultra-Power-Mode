using System.Drawing;
using Image = System.Windows.Controls.Image;

namespace UltraPowerMode.Utils
{
    public static class ImageUtils
    {
        public static void UpdateSource(this Image image, Bitmap bitmap)
        {
            var bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
            using (var memory = new System.IO.MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            image.Source = bitmapImage;
        }
    }
}

using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SmurfUI.Converters
{
    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string imageName = value as string ?? "forest";
            
            // Ne pas charger d'image pour "forest" (case vide)
            if (imageName == "forest" || string.IsNullOrEmpty(imageName))
                return null;
                
            string imagePath = $"pack://application:,,,/Assets/Images/{imageName}.png";
            
            try
            {
                var uri = new Uri(imagePath, UriKind.Absolute);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = uri;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                bitmap.EndInit();
                
                // Rendre le fond transparent si l'image a un canal alpha
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
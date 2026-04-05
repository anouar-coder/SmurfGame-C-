using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SmurfUI.Converters
{
    /// <summary>
    /// Convertit la santé du Schtroumpf (0-500) en couleur pour la barre de vie.
    /// Vert  → santé > 350  (70 % de 500)
    /// Orange → santé > 150  (30 % de 500)
    /// Rouge  → santé ≤ 150
    /// </summary>
    public class HealthToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (value is int health)
            {
                if (health > 350)
                    return new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50));  // vert
                if (health > 150)
                    return new SolidColorBrush(Color.FromRgb(0xFF, 0x98, 0x00));  // orange
                return new SolidColorBrush(Color.FromRgb(0xF4, 0x43, 0x36));      // rouge
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
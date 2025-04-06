using System.Globalization;
using System.Windows.Data;

namespace XO.Converters;

public class FontSizeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double width)
        {
            // Adjust the scaling factor as needed
            return width * 0.08; // Example: FontSize = Width / 10
        }
        return 12; // Default font size
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
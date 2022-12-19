using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace OpenDiffEditor.WPF.UI.Converter;

public class ColorToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string colorCode)
        {
            return Binding.DoNothing;
        }

        var converter = new BrushConverter();
        return converter.ConvertFromString(colorCode);
    }

    public object? ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

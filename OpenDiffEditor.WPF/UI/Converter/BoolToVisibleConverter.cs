using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace OpenDiffEditor.WPF.UI.Converter;

public class BoolToVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool isVisible)
        {
            return Binding.DoNothing;
        }
        return isVisible ? Visibility.Visible : Visibility.Hidden;
    }

    public object? ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Visibility visible)
        {
            return Binding.DoNothing;
        }
        return visible switch
        {
            Visibility.Visible or Visibility.Collapsed => true,
            _ => (object)false,
        };
        ;

    }
}

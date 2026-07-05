using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using DeviceMonitor.Models;

namespace DeviceMonitor.Converters;

public class DeviceStatusToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not DeviceStatus status)
        {
            return new SolidColorBrush(Color.Parse("#93908c"));
        }

        return status switch
        {
            DeviceStatus.Disconnected => new SolidColorBrush(Color.Parse("#93908c")),
            DeviceStatus.InUse => new SolidColorBrush(Color.Parse("#9c2121")),
            DeviceStatus.NotInUse => new SolidColorBrush(Color.Parse("#22946e")),
            _ => new SolidColorBrush(Color.Parse("#93908c"))
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
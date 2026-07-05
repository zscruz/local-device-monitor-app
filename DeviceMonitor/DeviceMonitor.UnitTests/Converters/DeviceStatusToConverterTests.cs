using System.Globalization;
using Avalonia.Media;
using DeviceMonitor.Converters;
using DeviceMonitor.Models;

namespace DeviceMonitor.UnitTests.Converters;

public class DeviceStatusToConverterTests
{
    [Theory]
    [InlineData(DeviceStatus.Disconnected, "#ff93908c")]
    [InlineData(DeviceStatus.InUse, "#ff9c2121")]
    [InlineData(DeviceStatus.NotInUse, "#ff22946e")]
    public void Convert_ShouldConvertDeviceStatus_ToCorrectColor(DeviceStatus input, string expectedColorString)
    {
        var converter = new DeviceStatusToColorConverter();

        var colorObject = converter.Convert(input, typeof(DeviceStatus), null, CultureInfo.CurrentCulture);

        var color = colorObject as SolidColorBrush;
        Assert.Equal(expectedColorString, color?.ToString());
    }

    [Fact] 
    public void Convert_ShouldConvertDeviceStatusToDefaultColor_WhenUnknownStatus()
    {
        var converter = new DeviceStatusToColorConverter();

        var colorObject = converter.Convert(-1, typeof(DeviceStatus), null, CultureInfo.CurrentCulture);

        var color = colorObject as SolidColorBrush;
        Assert.Equal("#ff93908c", color?.ToString());
    }
}
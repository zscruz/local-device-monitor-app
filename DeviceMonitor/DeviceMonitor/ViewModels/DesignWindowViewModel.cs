using DeviceMonitor.Models;

namespace DeviceMonitor.ViewModels;

public class DesignWindowViewModel : MainWindowViewModel
{
    public DesignWindowViewModel()
    {
        Devices = [];
        const int deviceCount = 20;
        for (var i = 0; i < deviceCount; i++)
        {
            if (i % 2 == 0)
            {
                Devices.Add(new DeviceInfo
                {
                    Id = $"video{i}",
                    Name = $"Camera{i}",
                    Status = DeviceStatus.InUse
                });
            }
            else
            {
                Devices.Add(new DeviceInfo
                {
                    Id = $"video{i}",
                    Name = $"Camera{i}",
                    Status = DeviceStatus.NotInUse
                });
            }
        }
        Devices.Add(new DeviceInfo
        {
            Id = $"video{deviceCount + 1}",
            Name = $"Camera{deviceCount + 1}. The really long camera name. Not sure how to fit it",
            Status = DeviceStatus.InUse
        });
    }
}
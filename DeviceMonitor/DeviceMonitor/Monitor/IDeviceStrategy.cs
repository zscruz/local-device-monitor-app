using System.Collections.Generic;
using DeviceMonitor.Models;

namespace DeviceMonitor.Monitor;

public interface IDeviceStrategy
{
    DeviceType SupportedType { get; }
    List<DeviceInfo> RetrieveDevices();
}
using System;
using DeviceMonitor.Models;

namespace DeviceMonitor.Monitor;

public class DeviceChangedEventArgs : EventArgs
{
    public DeviceOperation Operation { get; }
    public DeviceInfo? Device { get; }
    public string? DeviceId { get; }

    public DeviceChangedEventArgs(DeviceOperation operation, DeviceInfo? device = null, string? deviceId = null)
    {
        Operation = operation;
        Device = device;
        DeviceId = deviceId;
    }
}
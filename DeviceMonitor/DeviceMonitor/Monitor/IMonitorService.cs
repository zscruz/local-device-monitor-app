using System;

namespace DeviceMonitor.Monitor;

public interface IMonitorService
{ 
    event EventHandler<DeviceChangedEventArgs>? DeviceStatusChanged;
    bool IsRunning { get; }
    void Start();
    void Stop();
}
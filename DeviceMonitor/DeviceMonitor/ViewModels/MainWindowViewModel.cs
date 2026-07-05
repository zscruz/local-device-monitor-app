using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using DeviceMonitor.Models;
using DeviceMonitor.Monitor;
using Microsoft.Extensions.Logging;

namespace DeviceMonitor.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IMonitorService? _monitorService;

    [ObservableProperty]
    private List<DeviceInfo> _devices;

    private readonly ILogger<MainWindowViewModel>? _logger;

    protected MainWindowViewModel()
    {
        _devices = [];
    }
    
    public MainWindowViewModel(IMonitorService monitorService, ILogger<MainWindowViewModel> logger)
    {
        _devices = [];
        _monitorService = monitorService;
        _logger = logger;
        _monitorService.DeviceStatusChanged += MonitorServiceOnDeviceStatusChanged;
        
        _monitorService.Start();
    }

    private void MonitorServiceOnDeviceStatusChanged(object? sender, DeviceChangedEventArgs e)
    {
        _logger?.LogDebug("Monitor service has triggered a device change event");
        var updatedList = new List<DeviceInfo>(Devices);
        var changed = false;
        switch (e.Operation)
        {
            case DeviceOperation.Added:
                if (e.Device != null)
                {
                    updatedList.Add(e.Device);
                    changed = true;
                    _logger?.LogDebug("This device has been added to the view: {Device}", e.Device);
                }
                break;
            case DeviceOperation.Removed:
                var deviceToRemove = Devices.FirstOrDefault(d => d.Id == e.DeviceId);
                if (deviceToRemove != null)
                {
                    updatedList.Remove(deviceToRemove);
                    changed = true;
                    _logger?.LogDebug("This device has been removed from the view: {Device}", e.Device);
                }
                break;
            case DeviceOperation.Modified:
                var toUpdate = updatedList.FirstOrDefault(d => d.Id == e.Device?.Id);
                if (toUpdate != null && e.Device != null)
                {
                    toUpdate.Status = e.Device.Status;
                    changed = true;
                    _logger?.LogDebug("This device has been modified in the view: {Device}", e.Device);
                }
                break;
            default:
                changed = false;
                break;
        }

        if (changed)
        {
            Devices = updatedList;
        }
    }

    public void StopMonitorService()
    {
        _logger?.LogDebug("The view requested to stop monitoring");
        _monitorService?.Stop();
    }
}
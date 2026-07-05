using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeviceMonitor.Models;
using Microsoft.Extensions.Logging;

namespace DeviceMonitor.Monitor;

public class MonitorService : IMonitorService
{
    private readonly IEnumerable<IDeviceStrategy> _strategies;
    private readonly int _pollInterval;

    private readonly Dictionary<string, DeviceInfo> _cachedDevices;
    private CancellationTokenSource? _cancellationTokenSource;
    private readonly ILogger<MonitorService> _logger;

    public MonitorService(IEnumerable<IDeviceStrategy> strategies, ILogger<MonitorService> logger, int pollIntervalMs = 2000)
    {
        _strategies = strategies;
        _logger = logger;
        _pollInterval = pollIntervalMs;
       _cachedDevices = [];
    }

    public bool IsRunning => _cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested;

    public event EventHandler<DeviceChangedEventArgs>? DeviceStatusChanged;

    private void PollHardware()
    {
        var foundDevices = new Dictionary<string, DeviceInfo>();
        foreach (var strategy in _strategies)
        {
            var devices = strategy.RetrieveDevices();

            foreach (var device in devices)
            {
                foundDevices.TryAdd(device.Id, device);
            }
        }
        
        foreach (var id in foundDevices.Keys.Except(_cachedDevices.Keys))
        {
            var foundDevice = foundDevices[id];
            _cachedDevices.Add(id, foundDevice);
            DeviceStatusChanged?.Invoke(this, new DeviceChangedEventArgs(DeviceOperation.Added, foundDevice));
            _logger.LogInformation("A device was newly found: {NewDeviceId}: ", foundDevice.Id);
        }
        
        foreach (var id in _cachedDevices.Keys.Except(foundDevices.Keys).ToList())
        {
            _cachedDevices.Remove(id);
            DeviceStatusChanged?.Invoke(this, new DeviceChangedEventArgs(DeviceOperation.Removed, deviceId: id));
            _logger.LogInformation("A device was removed: {DeviceId}: ", id);

        }

        foreach (var id in foundDevices.Keys.Intersect(_cachedDevices.Keys))
        {
            if (_cachedDevices[id].Status != foundDevices[id].Status)
            {
                _cachedDevices[id].Status = foundDevices[id].Status;
                DeviceStatusChanged?.Invoke(this, new DeviceChangedEventArgs(DeviceOperation.Modified, _cachedDevices[id]));
                _logger.LogInformation("A device was modified: {DeviceId}", id);
            }
        }
    }

    public void Start()
    {
        _logger.LogInformation("Starting to poll connected hardware every: {PollInterval} ms", _pollInterval);
        _cancellationTokenSource = new CancellationTokenSource();

        if (!IsRunning)
        {
            return;
        }

        _cancellationTokenSource = new CancellationTokenSource();

        Task.Run(async () =>
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                PollHardware();

                await Task.Delay(_pollInterval, _cancellationTokenSource.Token);
            }
        }, _cancellationTokenSource.Token);

    }

    public void Stop()
    {
        _logger.LogInformation("Stopped polling for connected hardware");
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = null;
    }
}
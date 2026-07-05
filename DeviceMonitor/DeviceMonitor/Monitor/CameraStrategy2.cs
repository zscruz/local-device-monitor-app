using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DeviceMonitor.Models;
using Microsoft.Extensions.Logging;

namespace DeviceMonitor.Monitor;

public partial class CameraStrategy2 : IDeviceStrategy
{
    private const string CameraPath = "/sys/class/video4linux";
    private readonly ILogger<CameraStrategy2> _logger;

    public DeviceType SupportedType => DeviceType.Camera;

    public CameraStrategy2(ILogger<CameraStrategy2> logger)
    {
        _logger = logger;
        _logger.LogDebug("Camera strategy initialized");
        _logger.LogInformation("Searching for cameras at this location: {Location}", CameraPath);
    }
    
    public List<DeviceInfo> RetrieveDevices()
    {
        if (!Directory.Exists(CameraPath))
        {
            _logger.LogError("This path does not exist: {Path}", CameraPath);
            return [];
        }
        
        var validDevices = new List<DeviceInfo>();
        var directories = Directory.GetDirectories(CameraPath);
        _logger.LogDebug("Found directories: {Count}", directories.Length);

        var discoveredDevices = new List<DeviceInfo>();
        var numberRegex = MyRegex();

        foreach (var directory in directories)
        {
            _logger.LogDebug("Checking directory: {Directory}", directory);
            var devNode = Path.GetFileName(directory);
            var match = numberRegex.Match(devNode);
            if (!match.Success || !int.TryParse(match.Value, out var nodeNumber))
            {
                continue;
            }

            var deviceLink = Path.Combine(directory, "device");
            var target = File.ResolveLinkTarget(deviceLink, true);

            if (target == null)
            {
                continue;
            }

            var devPath = $"/dev/{devNode}";
            var deviceNamePath = Path.Combine(directory, "name");
            var friendlyName = File.ReadAllText(deviceNamePath).Trim();
            var physicalBusPath = target.FullName;
            _logger.LogDebug("Camera name: {CameraName}", directory);
            
            if (File.Exists(devPath))
            {
                var newDevice = new DeviceInfo
                {
                    Id = devNode,
                    DeviceType = SupportedType,
                    Name = friendlyName,
                    NodeNumber = nodeNumber,
                    PhysicalBusPath = physicalBusPath
                };

                if (IsCameraInUse(devPath))
                {
                    newDevice.Status = DeviceStatus.InUse;
                }
                else
                {
                    newDevice.Status = DeviceStatus.NotInUse;
                }

                discoveredDevices.Add(newDevice);
                _logger.LogDebug("Found device: {FoundDevice}", newDevice);
            }
        }

        var devices = discoveredDevices
            .GroupBy(dn => dn.PhysicalBusPath)
            .Select(g => g.OrderBy(n => n.NodeNumber).First());
        
        validDevices.AddRange(devices);
        return validDevices;
    }
    
    private static bool IsCameraInUse(string devicePath)
    {
        // If the device file doesn't exist, it's not plugged in / available
        if (!System.IO.File.Exists(devicePath))
        {
            return false;
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = "fuser",
            Arguments = devicePath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);
        process.WaitForExit();
            
        // fuser returns 0 if at least one process has the file open (in use)
        // It returns non-zero if no processes are using it
        return process.ExitCode == 0;
    }
    
    private static bool IsFileLocked(string path)
    {
        try
        {
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
            return false;
        }
        catch (IOException)
        {
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            return true;
        }
    }

    [GeneratedRegex(@"\d+")]
    private static partial Regex MyRegex();
}
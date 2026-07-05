using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DeviceMonitor.Models;

namespace DeviceMonitor.Monitor;

public class CameraStrategy : IDeviceStrategy
{
    private const string CameraPath = "/sys/class/video4linux";
    
    public DeviceType SupportedType => DeviceType.Camera;

    public List<DeviceInfo> RetrieveDevices()
    {
        if (!Directory.Exists(CameraPath))
        {
            return [];
        }

        var directories = Directory.GetDirectories(CameraPath);
        
        var discoveredDevices = new List<DeviceInfo>();
        foreach (var directory in directories)
        {
            var devNode = Path.GetFileName(directory);
            
            // Extract the node number from video0 -> 0 or video2 -> 2
            var regex = new Regex(@"\d+");
            var match = regex.Match(devNode);
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
            
            var deviceNamePath = Path.Combine(directory, "name");
            var friendlyName = File.ReadAllText(deviceNamePath).Trim();
            // Group by this value
            var physicalBusPath = target.FullName;
            
            var newDevice = new DeviceInfo
            {
                Id = devNode,
                DeviceType = SupportedType,
                Name = friendlyName,
                NodeNumber = nodeNumber,
                PhysicalBusPath = physicalBusPath
            };
            
            discoveredDevices.Add(newDevice);
        }

        var validDevices = new List<DeviceInfo>();
        var devices = discoveredDevices
            .GroupBy(dn => dn.PhysicalBusPath)
            .Select(g => g.OrderBy(n => n.NodeNumber).First());
        
        validDevices.AddRange(devices);
        return validDevices;
    }
}
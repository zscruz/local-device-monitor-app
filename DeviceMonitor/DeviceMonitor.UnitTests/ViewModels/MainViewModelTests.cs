using DeviceMonitor.Models;
using DeviceMonitor.Monitor;
using DeviceMonitor.ViewModels;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace DeviceMonitor.UnitTests.ViewModels;

public class MainViewModelTests
{
    [Fact]
    public void Devices_ShouldAddNewDevice_WhenMonitorTriggersAddOperation()
    {
        var monitorService = Substitute.For<IMonitorService>();
        var logger = Substitute.For<ILogger<MainWindowViewModel>>();
        var expectedDevice = new DeviceInfo
        {
            Id = "1",
            Name = "Camera1",
            Status = DeviceStatus.InUse
        };
        var eventRaised = false;
        var mainViewModel = new MainWindowViewModel(monitorService, logger);
        monitorService.DeviceStatusChanged += (_, _) =>
        {
            eventRaised = true;
        };

        monitorService.DeviceStatusChanged +=
            Raise.EventWith(new DeviceChangedEventArgs(DeviceOperation.Added, expectedDevice));
        
        Assert.True(eventRaised);
        Assert.Single(mainViewModel.Devices);
        Assert.Equal(expectedDevice, mainViewModel.Devices.First());
    }

    [Fact]
    public void Devices_ShouldRemoveDevice_WhenMonitorTriggersRemoveOperation()
    {
        var monitorService = Substitute.For<IMonitorService>();
        var logger = Substitute.For<ILogger<MainWindowViewModel>>();

        var expectedDevice = new DeviceInfo
        {
            Id = "1",
            Name = "Camera1",
            Status = DeviceStatus.InUse
        };
        var expectedDeviceToRemove = new DeviceInfo
        {
            Id = "2",
            Name = "Camera2",
            Status = DeviceStatus.InUse
        };
        var eventRaised = false;
        var mainViewModel = new MainWindowViewModel(monitorService, logger);
        mainViewModel.Devices.Add(expectedDevice);
        mainViewModel.Devices.Add(expectedDeviceToRemove);
        monitorService.DeviceStatusChanged += (_, _) =>
        {
            eventRaised = true;
        };

        monitorService.DeviceStatusChanged +=
            Raise.EventWith(new DeviceChangedEventArgs(DeviceOperation.Removed, expectedDeviceToRemove, expectedDeviceToRemove.Id));
        
        Assert.True(eventRaised);
        Assert.Single(mainViewModel.Devices);
        Assert.Equal(expectedDevice, mainViewModel.Devices.First());
    }
    
    [Fact]
    public void Devices_ShouldUpdateDevice_WhenMonitorTriggersModifiedOperation()
    {
        var monitorService = Substitute.For<IMonitorService>();
        var logger = Substitute.For<ILogger<MainWindowViewModel>>();
        var expectedDevice = new DeviceInfo
        {
            Id = "1",
            Name = "Camera1",
            Status = DeviceStatus.InUse
        };
        var expectedDeviceToUpdate = new DeviceInfo
        {
            Id = "2",
            Name = "Camera2",
            Status = DeviceStatus.InUse
        };
        var eventRaised = false;
        var mainViewModel = new MainWindowViewModel(monitorService, logger);
        mainViewModel.Devices.Add(expectedDevice);
        mainViewModel.Devices.Add(expectedDeviceToUpdate);
        monitorService.DeviceStatusChanged += (_, _) =>
        {
            eventRaised = true;
        };

        var updatedDevice = new DeviceInfo
        {
            Id = "2",
            Name = "Camera2",
            Status = DeviceStatus.NotInUse
        };
        
        monitorService.DeviceStatusChanged +=
            Raise.EventWith(new DeviceChangedEventArgs(DeviceOperation.Modified, updatedDevice));
        
        Assert.True(eventRaised);
        Assert.Equal(2, mainViewModel.Devices.Count);
        Assert.Equal(DeviceStatus.NotInUse, mainViewModel.Devices[1].Status);
        Assert.Equal(DeviceStatus.InUse, mainViewModel.Devices.First().Status);

    }
}
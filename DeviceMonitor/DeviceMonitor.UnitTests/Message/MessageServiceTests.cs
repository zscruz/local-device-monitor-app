using DeviceMonitor.Message;
using DeviceMonitor.Models;
using NSubstitute;

namespace DeviceMonitor.UnitTests.Message;

public class MessageServiceTests
{
    [Fact]
    public async Task Send_PayloadSent_ContainsByteMarkers()
    {
        var client = Substitute.For<IBroadcastClient>();
        
        var devices = new List<DeviceInfo>
        {
            new DeviceInfo
            {
                Id = "1",
                DeviceType = DeviceType.Camera,
                Status = DeviceStatus.InUse
            },
            new DeviceInfo
            {
                Id = "2",
                DeviceType = DeviceType.Camera,
                Status = DeviceStatus.NotInUse
            }
        };

        var service = new MessageService(client);

        await service.Send(devices);

        await client.Received(1).SendAsync(
            Arg.Is<byte[]>(p => p[0] == (byte)'D'
                && p[1] == (byte)'M'));
    }
    
    [Fact]
    public async Task Send_PayloadSent_CorrectlySayDeviceIsInUse()
    {
        var client = Substitute.For<IBroadcastClient>();
        
        var devices = new List<DeviceInfo>
        {
            new DeviceInfo
            {
                Id = "1",
                DeviceType = DeviceType.Camera,
                Status = DeviceStatus.InUse
            },
            new DeviceInfo
            {
                Id = "1",
                DeviceType = DeviceType.Camera,
                Status = DeviceStatus.NotInUse
            }
        };

        var service = new MessageService(client);

        await service.Send(devices);

        await client.Received(1).SendAsync(
            Arg.Is<byte[]>(p => p[3] == 1));
    }
    
    [Fact]
    public async Task Send_PayloadSent_CorrectlySayDeviceIsNotInUse()
    {
        var client = Substitute.For<IBroadcastClient>();
        
        var devices = new List<DeviceInfo>
        {
            new DeviceInfo
            {
                Id = "1",
                DeviceType = DeviceType.Camera,
                Status = DeviceStatus.NotInUse
            },
            new DeviceInfo
            {
                Id = "2",
                DeviceType = DeviceType.Camera,
                Status = DeviceStatus.NotInUse
            }
        };

        var service = new MessageService(client);

        await service.Send(devices);

        await client.Received(1).SendAsync(
            Arg.Is<byte[]>(p => p[3] == 0));
    }
    
    [Fact]
    public async Task Send_PayloadSent_ContainsDeviceState()
    {
        var client = Substitute.For<IBroadcastClient>();
        
        var devices = new List<DeviceInfo>
        {
            new DeviceInfo
            {
                Id = "1",
                DeviceType = DeviceType.Camera,
                Status = DeviceStatus.NotInUse
            },
            new DeviceInfo
            {
                Id = "2",
                DeviceType = DeviceType.Camera,
                Status = DeviceStatus.InUse
            }
        };

        var service = new MessageService(client);

        await service.Send(devices);

        await client.Received(1).SendAsync(
            Arg.Is<byte[]>(
            p => p[4] == 2 &&
            p[5] == 0 &&
            p[6] == 0 &&
            p[7] == 0 &&
            p[8] == 1));
    }
    
    [Fact]
    public async Task Send_PayloadSent_ContainsCorrectChecksum()
    {
        var client = Substitute.For<IBroadcastClient>();
        
        var devices = new List<DeviceInfo>
        {
            new DeviceInfo
            {
                Id = "1",
                DeviceType = DeviceType.Camera,
                Status = DeviceStatus.InUse
            },
            new DeviceInfo
            {
                Id = "2",
                DeviceType = DeviceType.Camera,
                Status = DeviceStatus.NotInUse
            }
        };

        var service = new MessageService(client);

        await service.Send(devices);

        await client.Received(1).SendAsync(
            Arg.Is<byte[]>(p => p[9] == 10));
    }

}
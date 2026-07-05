using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeviceMonitor.Models;

namespace DeviceMonitor.Message;

public class MessageService : IMessageService
{
    private readonly IBroadcastClient _client;

    public MessageService(IBroadcastClient client)
    {
        _client = client;
    }
    
    public async Task Send(List<DeviceInfo> devices)
    {
        if (devices.Count <= 0)
        {
            return;
        }
        
        var payload = BuildPayload(devices).ToArray();
        
        await _client.SendAsync(payload);
    }

    private static byte[] BuildPayload(List<DeviceInfo> devices)
    {
        var isOn = devices.Any(d => d.Status == DeviceStatus.InUse);
        var count = Math.Min(devices.Count, byte.MaxValue);
        
        var payload = new List<byte>
        {
            (byte)'D',
            (byte)'M',
            1,
            isOn ? (byte)1: (byte)0,
            (byte)count
        };

        foreach (var device in devices)
        {
            payload.Add((byte)device.DeviceType);
            payload.Add((byte)device.Status);
        }

        byte checksum = 0;
        for (var i = 0; i < payload.Count - 1; i++)
        {
            checksum ^= payload[i];
        }
        payload.Add(checksum);

        return payload.ToArray();
    }
}
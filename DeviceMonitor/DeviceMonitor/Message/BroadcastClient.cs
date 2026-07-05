using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DeviceMonitor.Message;

public class BroadcastClient : IBroadcastClient, IDisposable
{
    private readonly UdpClient _udpClient;
    private readonly ILogger<BroadcastClient> _logger;

    public BroadcastClient(ILogger<BroadcastClient> logger)
    {
        _udpClient = new UdpClient();
        _logger = logger;
        _udpClient.EnableBroadcast = true;
    }

    public async Task<bool> SendAsync(byte[] message)
    {
        try
        {
            await _udpClient.SendAsync(message, message.Length);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Payload failed to send");
            return false;
        }
        _logger.LogInformation("Successfully sent payload");
        return true;
    }

    public void Dispose()
    {
        _udpClient.Dispose();
    }
}
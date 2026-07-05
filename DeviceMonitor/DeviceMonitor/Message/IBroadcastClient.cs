using System.Threading.Tasks;

namespace DeviceMonitor.Message;

public interface IBroadcastClient
{
    Task<bool> SendAsync(byte[] message);
}
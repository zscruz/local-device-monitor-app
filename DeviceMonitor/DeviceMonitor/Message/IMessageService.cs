using System.Collections.Generic;
using System.Threading.Tasks;
using DeviceMonitor.Models;

namespace DeviceMonitor.Message;

public interface IMessageService
{
    public Task Send(List<DeviceInfo> devices);
}
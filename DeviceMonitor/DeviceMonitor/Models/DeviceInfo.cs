using System;

namespace DeviceMonitor.Models;

public record DeviceInfo
{
    public string Id { get; init; } = "";
    public DeviceType DeviceType { get; init; }
    public string Name { get; init; } = "";
    public int NodeNumber { get; init; }
    public string PhysicalBusPath { get; init; } = "";
    public DeviceStatus Status { get; set; }
    
    
    public virtual bool Equals(DeviceInfo? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id && DeviceType == other.DeviceType && Name == other.Name && Status == other.Status;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, (int)DeviceType, Name);
    }
    
    public override string ToString()
    {
        return
            $"{nameof(Id)}: {Id}, {nameof(DeviceType)}: {DeviceType}, {nameof(Name)}: {Name}, {nameof(Status)}: {Status}";
    }
}
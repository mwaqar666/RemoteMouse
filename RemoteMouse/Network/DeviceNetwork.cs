using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.NetworkInformation;

namespace RemoteMouse.Network;

public class DeviceNetwork(NetworkInterface networkInterface, IPAddress localIpAddress) : IEquatable<DeviceNetwork>
{
    [SuppressMessage("ReactiveUI.SourceGenerators.CodeFixers.PropertyToReactiveFieldAnalyzer", "RXUISG0016:Property To Reactive Field, change to [Reactive] private type _fieldName;")]
    public IPAddress LocalIpAddress { get; } = localIpAddress;

    [SuppressMessage("ReactiveUI.SourceGenerators.CodeFixers.PropertyToReactiveFieldAnalyzer", "RXUISG0016:Property To Reactive Field, change to [Reactive] private type _fieldName;")]
    public NetworkInterface NetworkInterface { get; } = networkInterface;

    public bool Equals(DeviceNetwork? other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return LocalIpAddress.Equals(other.LocalIpAddress) && NetworkInterface.Equals(other.NetworkInterface);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        if (ReferenceEquals(this, obj)) return true;

        return obj.GetType() == GetType() && Equals((DeviceNetwork)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(LocalIpAddress, NetworkInterface);
    }

    public static bool operator ==(DeviceNetwork? left, DeviceNetwork? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(DeviceNetwork? left, DeviceNetwork? right)
    {
        return !Equals(left, right);
    }
}
namespace System.Net.Sockets;

public struct IPPacketInformation
{
	private IPAddress address;

	private int iface;

	public IPAddress Address => address;

	public int Interface => iface;

	internal IPPacketInformation(IPAddress address, int iface)
	{
		this.address = address;
		this.iface = iface;
	}

	public override bool Equals(object comparand)
	{
		if (!(comparand is IPPacketInformation iPPacketInformation))
		{
			return false;
		}
		if (iPPacketInformation.iface != iface)
		{
			return false;
		}
		return iPPacketInformation.address.Equals(address);
	}

	public override int GetHashCode()
	{
		return address.GetHashCode() + iface;
	}

	public static bool operator ==(IPPacketInformation p1, IPPacketInformation p2)
	{
		return p1.Equals(p2);
	}

	public static bool operator !=(IPPacketInformation p1, IPPacketInformation p2)
	{
		return !p1.Equals(p2);
	}
}

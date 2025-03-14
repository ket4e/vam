namespace System.Net.NetworkInformation;

internal sealed class MacOsIPv4InterfaceProperties : System.Net.NetworkInformation.UnixIPv4InterfaceProperties
{
	public override bool IsForwardingEnabled => false;

	public override int Mtu => 0;

	public MacOsIPv4InterfaceProperties(System.Net.NetworkInformation.MacOsNetworkInterface iface)
		: base(iface)
	{
	}
}

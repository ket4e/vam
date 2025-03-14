namespace System.Net.NetworkInformation;

internal abstract class UnixIPv4InterfaceProperties : IPv4InterfaceProperties
{
	protected System.Net.NetworkInformation.UnixNetworkInterface iface;

	public override int Index => System.Net.NetworkInformation.UnixNetworkInterface.IfNameToIndex(iface.Name);

	public override bool IsAutomaticPrivateAddressingActive => false;

	public override bool IsAutomaticPrivateAddressingEnabled => false;

	public override bool IsDhcpEnabled => false;

	public override bool UsesWins => false;

	public UnixIPv4InterfaceProperties(System.Net.NetworkInformation.UnixNetworkInterface iface)
	{
		this.iface = iface;
	}
}

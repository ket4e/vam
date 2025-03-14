using System.Runtime.InteropServices;

namespace System.Resources;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class SatelliteContractVersionAttribute : Attribute
{
	private Version ver;

	public string Version => ver.ToString();

	public SatelliteContractVersionAttribute(string version)
	{
		ver = new Version(version);
	}
}

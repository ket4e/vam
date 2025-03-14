using System.ComponentModel;

namespace UnityEngine.Networking.Types;

/// <summary>
///   <para>Network ID, used for match making.</para>
/// </summary>
[DefaultValue(NetworkID.Invalid)]
public enum NetworkID : ulong
{
	/// <summary>
	///   <para>Invalid NetworkID.</para>
	/// </summary>
	Invalid = ulong.MaxValue
}

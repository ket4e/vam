using System.ComponentModel;

namespace UnityEngine.Networking.Types;

/// <summary>
///   <para>Identifies a specific game instance.</para>
/// </summary>
[DefaultValue(SourceID.Invalid)]
public enum SourceID : ulong
{
	/// <summary>
	///   <para>Invalid SourceID.</para>
	/// </summary>
	Invalid = ulong.MaxValue
}

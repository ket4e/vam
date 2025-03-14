using System.ComponentModel;

namespace UnityEngine.Networking.Types;

/// <summary>
///   <para>The AppID identifies the application on the Unity Cloud or UNET servers.</para>
/// </summary>
[DefaultValue(AppID.Invalid)]
public enum AppID : ulong
{
	/// <summary>
	///   <para>Invalid AppID.</para>
	/// </summary>
	Invalid = ulong.MaxValue
}

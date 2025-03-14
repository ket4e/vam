using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEditor.Experimental;

/// <summary>
///   <para>Experimental render settings features.</para>
/// </summary>
public sealed class RenderSettings
{
	/// <summary>
	///   <para>If enabled, ambient trilight will be sampled using the old radiance sampling method.</para>
	/// </summary>
	public static extern bool useRadianceAmbientProbe
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}
}

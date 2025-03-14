using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Behaviours are Components that can be enabled or disabled.</para>
/// </summary>
[UsedByNativeCode]
public class Behaviour : Component
{
	/// <summary>
	///   <para>Enabled Behaviours are Updated, disabled Behaviours are not.</para>
	/// </summary>
	public extern bool enabled
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Has the Behaviour had enabled called.</para>
	/// </summary>
	public extern bool isActiveAndEnabled
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}
}

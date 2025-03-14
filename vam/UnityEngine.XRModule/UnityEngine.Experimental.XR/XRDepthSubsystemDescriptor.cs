using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR;

/// <summary>
///   <para>Class providing information about XRDepthSubsystem registration.</para>
/// </summary>
[NativeType(Header = "Modules/XR/Subsystems/Depth/XRDepthSubsystemDescriptor.h")]
[NativeHeader("Modules/XR/XRPrefix.h")]
[NativeConditional("ENABLE_XR")]
[UsedByNativeCode]
public class XRDepthSubsystemDescriptor : SubsystemDescriptor<XRDepthSubsystem>
{
	/// <summary>
	///   <para>When true, XRDepthSubsystem will provide list of feature points detected so far.</para>
	/// </summary>
	public extern bool SupportsFeaturePoints
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}
}

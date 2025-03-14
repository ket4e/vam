using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR;

[UsedByNativeCode]
[NativeType(Header = "Modules/XR/Subsystems/Example/XRExampleSubsystemDescriptor.h")]
public class XRExampleSubsystemDescriptor : SubsystemDescriptor<XRExampleSubsystem>
{
	public extern bool supportsEditorMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public extern bool disableBackbufferMSAA
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public extern bool stereoscopicBackbuffer
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public extern bool usePBufferEGL
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}
}

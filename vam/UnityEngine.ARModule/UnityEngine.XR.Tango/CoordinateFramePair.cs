using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR.Tango;

[StructLayout(LayoutKind.Explicit, Size = 8)]
[NativeHeader("ARScriptingClasses.h")]
[UsedByNativeCode]
internal struct CoordinateFramePair
{
	[FieldOffset(0)]
	public CoordinateFrame baseFrame;

	[FieldOffset(4)]
	public CoordinateFrame targetFrame;
}

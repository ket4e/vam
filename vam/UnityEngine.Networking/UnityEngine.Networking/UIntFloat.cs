using System.Runtime.InteropServices;

namespace UnityEngine.Networking;

[StructLayout(LayoutKind.Explicit)]
internal struct UIntFloat
{
	[FieldOffset(0)]
	public float floatValue;

	[FieldOffset(0)]
	public uint intValue;

	[FieldOffset(0)]
	public double doubleValue;

	[FieldOffset(0)]
	public ulong longValue;
}

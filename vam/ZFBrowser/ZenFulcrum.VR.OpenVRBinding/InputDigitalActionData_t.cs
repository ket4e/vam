using System.Runtime.InteropServices;

namespace ZenFulcrum.VR.OpenVRBinding;

public struct InputDigitalActionData_t
{
	[MarshalAs(UnmanagedType.I1)]
	public bool bActive;

	public ulong activeOrigin;

	[MarshalAs(UnmanagedType.I1)]
	public bool bState;

	[MarshalAs(UnmanagedType.I1)]
	public bool bChanged;

	public float fUpdateTime;
}

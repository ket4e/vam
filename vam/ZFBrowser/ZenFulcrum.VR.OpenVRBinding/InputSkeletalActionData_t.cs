using System.Runtime.InteropServices;

namespace ZenFulcrum.VR.OpenVRBinding;

public struct InputSkeletalActionData_t
{
	[MarshalAs(UnmanagedType.I1)]
	public bool bActive;

	public ulong activeOrigin;
}

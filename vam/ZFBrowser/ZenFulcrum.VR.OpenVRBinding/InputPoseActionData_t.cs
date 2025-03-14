using System.Runtime.InteropServices;

namespace ZenFulcrum.VR.OpenVRBinding;

public struct InputPoseActionData_t
{
	[MarshalAs(UnmanagedType.I1)]
	public bool bActive;

	public ulong activeOrigin;

	public TrackedDevicePose_t pose;
}

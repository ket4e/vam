using System.Runtime.InteropServices;

namespace ZenFulcrum.VR.OpenVRBinding;

public struct VREvent_SeatedZeroPoseReset_t
{
	[MarshalAs(UnmanagedType.I1)]
	public bool bResetBySystemMenu;
}

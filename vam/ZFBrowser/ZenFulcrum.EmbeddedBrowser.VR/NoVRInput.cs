using UnityEngine.XR;

namespace ZenFulcrum.EmbeddedBrowser.VR;

internal class NoVRInput : VRInput
{
	public override float GetAxis(XRNodeState node, InputAxis axis)
	{
		return 0f;
	}
}

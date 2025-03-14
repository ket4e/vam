using UnityEngine;
using UnityEngine.XR;

namespace ZenFulcrum.EmbeddedBrowser.VR;

public abstract class VRInput
{
	public struct Pose
	{
		public Vector3 pos;

		public Quaternion rot;
	}

	public static VRInput Impl { get; private set; }

	public static void Init()
	{
		if (Impl == null)
		{
			Impl = GetImpl();
		}
	}

	public abstract float GetAxis(XRNodeState node, InputAxis axis);

	public virtual Pose GetPose(XRNodeState node)
	{
		Pose result = default(Pose);
		node.TryGetPosition(out result.pos);
		node.TryGetRotation(out result.rot);
		return result;
	}

	public virtual JoyPadType GetJoypadTypes(XRNodeState node)
	{
		return JoyPadType.None;
	}

	public virtual string GetNodeName(XRNodeState node)
	{
		return InputTracking.GetNodeName(node.uniqueID);
	}

	private static VRInput GetImpl()
	{
		if (XRSettings.loadedDeviceName == "OpenVR")
		{
			return new OpenVRInput();
		}
		if (XRSettings.loadedDeviceName == "Oculus")
		{
			Debug.LogError("To use the Oculus API for input, import the Oculus SDK, delete ZFBrowser/**.asmdef (2 files), and define OCULUS_SDK");
			return new NoVRInput();
		}
		Debug.LogError("Unknown VR input system: " + XRSettings.loadedDeviceName);
		return new NoVRInput();
	}
}

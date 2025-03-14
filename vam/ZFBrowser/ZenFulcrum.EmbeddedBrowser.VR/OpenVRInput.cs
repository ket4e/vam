using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.XR;
using ZenFulcrum.VR.OpenVRBinding;

namespace ZenFulcrum.EmbeddedBrowser.VR;

internal class OpenVRInput : VRInput
{
	private enum InputMode
	{
		Direct,
		MappedActions
	}

	private InputMode mode;

	private const string PointPose = "/actions/ui/in/UIPointer";

	private const string LeftClickAction = "/actions/ui/in/InteractMain";

	private const string MiddleClickAction = "/actions/ui/in/InteractMiddle";

	private const string RightClickAction = "/actions/ui/in/InteractContext";

	private const string JoystickScrollAction = "/actions/ui/in/JoystickScroll";

	private const string TouchpadScrollTouch = "/actions/ui/in/TouchpadScrollTouch";

	private const string TouchpadScrollPosition = "/actions/ui/in/TouchpadScrollPosition";

	private VRControllerState_t direct_lastState;

	private ulong pointPose;

	private ulong leftClick;

	private ulong middleClick;

	private ulong rightClick;

	private ulong joystickScroll;

	private ulong touchPadScrollTouch;

	private ulong touchPadScrollPos;

	private ulong leftHand;

	private ulong rightHand;

	private JoyPadType jpType = JoyPadType.Unknown;

	public OpenVRInput()
	{
		if (mode == InputMode.MappedActions)
		{
			if (OpenVR.Input == null)
			{
				throw new ApplicationException("Cannot start VR input");
			}
			pointPose = GetActionHandle("/actions/ui/in/UIPointer", "PointPose");
			leftClick = GetActionHandle("/actions/ui/in/InteractMain", "LeftClickAction");
			middleClick = GetActionHandle("/actions/ui/in/InteractMiddle", "MiddleClickAction");
			rightClick = GetActionHandle("/actions/ui/in/InteractContext", "RightClickAction");
			joystickScroll = GetActionHandle("/actions/ui/in/JoystickScroll", "JoystickScrollAction");
			touchPadScrollTouch = GetActionHandle("/actions/ui/in/TouchpadScrollTouch", "TouchpadScrollTouch");
			touchPadScrollPos = GetActionHandle("/actions/ui/in/TouchpadScrollPosition", "TouchpadScrollPosition");
			EVRInputError inputSourceHandle = OpenVR.Input.GetInputSourceHandle("/user/hand/left", ref leftHand);
			if (inputSourceHandle != 0)
			{
				throw new ApplicationException("Failed to find hand " + inputSourceHandle);
			}
			inputSourceHandle = OpenVR.Input.GetInputSourceHandle("/user/hand/right", ref rightHand);
			if (inputSourceHandle != 0)
			{
				throw new ApplicationException("Failed to find hand " + inputSourceHandle);
			}
		}
	}

	private ulong GetActionHandle(string handleName, string name)
	{
		if (string.IsNullOrEmpty(handleName))
		{
			return 0uL;
		}
		ulong pHandle = 0uL;
		EVRInputError actionHandle = OpenVR.Input.GetActionHandle(handleName, ref pHandle);
		if (actionHandle != 0)
		{
			throw new ApplicationException("Failed to set up VR action " + actionHandle);
		}
		return pHandle;
	}

	public static string GetStringProperty(uint deviceId, ETrackedDeviceProperty prop)
	{
		StringBuilder stringBuilder = new StringBuilder(32768);
		ETrackedPropertyError pError = ETrackedPropertyError.TrackedProp_Success;
		OpenVR.System.GetStringTrackedDeviceProperty(deviceId, prop, stringBuilder, 32768u, ref pError);
		if (pError != 0)
		{
			throw new Exception(string.Concat("Failed to get property ", prop, " on device ", deviceId, ": ", pError));
		}
		return stringBuilder.ToString();
	}

	public override string GetNodeName(XRNodeState node)
	{
		uint deviceId = (uint)GetDeviceId(node);
		try
		{
			return GetStringProperty(deviceId, ETrackedDeviceProperty.Prop_ModelNumber_String);
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to get device name for device " + deviceId + ": " + ex.Message);
			return base.GetNodeName(node);
		}
	}

	protected void Direct_ReadState(XRNodeState node)
	{
		if (OpenVR.System == null)
		{
			Debug.LogWarning("OpenVR not active");
			direct_lastState = default(VRControllerState_t);
			return;
		}
		int deviceId = GetDeviceId(node);
		if (deviceId < 0)
		{
			direct_lastState = default(VRControllerState_t);
		}
		else if (!OpenVR.System.GetControllerState((uint)deviceId, ref direct_lastState, (uint)Marshal.SizeOf(typeof(VRControllerState_t))))
		{
			Debug.LogWarning("Failed to get controller state");
		}
	}

	private bool GetDigitalInput(ulong action, XRNodeState node)
	{
		InputDigitalActionData_t pActionData = default(InputDigitalActionData_t);
		EVRInputError digitalActionData = OpenVR.Input.GetDigitalActionData(action, ref pActionData, (uint)Marshal.SizeOf(typeof(InputDigitalActionData_t)), (node.nodeType != XRNode.LeftHand) ? rightHand : leftHand);
		if (digitalActionData != 0)
		{
			throw new ApplicationException(string.Concat("Failed to get digital input data ", node.nodeType, ": ", digitalActionData));
		}
		return pActionData.bState;
	}

	private Vector2 GetVector2Input(ulong action, XRNodeState node)
	{
		InputAnalogActionData_t pActionData = default(InputAnalogActionData_t);
		EVRInputError analogActionData = OpenVR.Input.GetAnalogActionData(action, ref pActionData, (uint)Marshal.SizeOf(typeof(InputAnalogActionData_t)), (node.nodeType != XRNode.LeftHand) ? rightHand : leftHand);
		if (analogActionData != 0)
		{
			throw new ApplicationException(string.Concat("Failed to get vector input data ", node.nodeType, ": ", analogActionData));
		}
		return new Vector2(pActionData.x, pActionData.y);
	}

	private Pose GetPoseInput(ulong action, XRNodeState node)
	{
		InputPoseActionData_t pActionData = default(InputPoseActionData_t);
		EVRInputError poseActionData = OpenVR.Input.GetPoseActionData(action, (XRDevice.GetTrackingSpaceType() == TrackingSpaceType.RoomScale) ? ETrackingUniverseOrigin.TrackingUniverseStanding : ETrackingUniverseOrigin.TrackingUniverseSeated, 0f, ref pActionData, (uint)Marshal.SizeOf(typeof(InputPoseActionData_t)), (node.nodeType != XRNode.LeftHand) ? rightHand : leftHand);
		if (poseActionData != 0)
		{
			throw new ApplicationException(string.Concat("Failed to get pose input data ", node.nodeType, ": ", poseActionData));
		}
		HmdMatrix34_t mDeviceToAbsoluteTracking = pActionData.pose.mDeviceToAbsoluteTracking;
		Quaternion rotation = new Matrix4x4(new Vector4(mDeviceToAbsoluteTracking.m0, mDeviceToAbsoluteTracking.m1, mDeviceToAbsoluteTracking.m2, 0f), new Vector4(mDeviceToAbsoluteTracking.m4, mDeviceToAbsoluteTracking.m5, mDeviceToAbsoluteTracking.m6, 0f), new Vector4(mDeviceToAbsoluteTracking.m8, mDeviceToAbsoluteTracking.m9, mDeviceToAbsoluteTracking.m10, 0f), new Vector4(0f, 0f, 0f, 1f)).rotation;
		rotation.z *= -1f;
		Pose result = default(Pose);
		result.pos = new Vector3(mDeviceToAbsoluteTracking.m3, mDeviceToAbsoluteTracking.m7, 0f - mDeviceToAbsoluteTracking.m11);
		result.rot = rotation;
		return result;
	}

	public override float GetAxis(XRNodeState node, InputAxis axis)
	{
		if (mode == InputMode.Direct)
		{
			Direct_ReadState(node);
			switch (axis)
			{
			case InputAxis.LeftClick:
				return direct_lastState.rAxis1.x;
			case InputAxis.MiddleClick:
				return ((direct_lastState.ulButtonPressed & 0x100000000L) != 0) ? 1 : 0;
			case InputAxis.RightClick:
				return ((direct_lastState.ulButtonPressed & 4) != 0) ? 1 : 0;
			case InputAxis.JoyStickX:
			case InputAxis.TouchPadX:
				return direct_lastState.rAxis0.x;
			case InputAxis.JoyStickY:
			case InputAxis.TouchPadY:
				return direct_lastState.rAxis0.y;
			case InputAxis.TouchPadTouch:
				return ((direct_lastState.ulButtonTouched & 0x100000000L) != 0) ? 1 : 0;
			default:
				throw new ArgumentOutOfRangeException("axis", axis, null);
			}
		}
		switch (axis)
		{
		case InputAxis.LeftClick:
			if (leftClick == 0)
			{
				return 0f;
			}
			return GetDigitalInput(leftClick, node) ? 1 : 0;
		case InputAxis.MiddleClick:
			if (middleClick == 0)
			{
				return 0f;
			}
			return GetDigitalInput(middleClick, node) ? 1 : 0;
		case InputAxis.RightClick:
			if (rightClick == 0)
			{
				return 0f;
			}
			return GetDigitalInput(rightClick, node) ? 1 : 0;
		case InputAxis.JoyStickX:
		case InputAxis.JoyStickY:
		{
			if (joystickScroll == 0)
			{
				return 0f;
			}
			Vector2 vector2Input2 = GetVector2Input(joystickScroll, node);
			if (axis == InputAxis.JoyStickX)
			{
				return vector2Input2.x;
			}
			return vector2Input2.y;
		}
		case InputAxis.TouchPadX:
		case InputAxis.TouchPadY:
		{
			if (touchPadScrollPos == 0)
			{
				return 0f;
			}
			Vector2 vector2Input = GetVector2Input(touchPadScrollPos, node);
			if (axis == InputAxis.TouchPadX)
			{
				return vector2Input.x;
			}
			return vector2Input.y;
		}
		case InputAxis.TouchPadTouch:
			if (touchPadScrollTouch == 0)
			{
				return 0f;
			}
			return GetDigitalInput(touchPadScrollTouch, node) ? 1 : 0;
		default:
			throw new ArgumentOutOfRangeException("axis", axis, null);
		}
	}

	public override Pose GetPose(XRNodeState node)
	{
		if (mode == InputMode.Direct)
		{
			return base.GetPose(node);
		}
		return GetPoseInput(pointPose, node);
	}

	private int GetDeviceId(XRNodeState node)
	{
		ETrackedControllerRole eTrackedControllerRole = ((node.nodeType == XRNode.LeftHand) ? ETrackedControllerRole.LeftHand : ETrackedControllerRole.RightHand);
		for (uint num = 0u; num < 64; num++)
		{
			ETrackedControllerRole controllerRoleForTrackedDeviceIndex = OpenVR.System.GetControllerRoleForTrackedDeviceIndex(num);
			if (controllerRoleForTrackedDeviceIndex == eTrackedControllerRole)
			{
				return (int)num;
			}
		}
		return -1;
	}

	public override JoyPadType GetJoypadTypes(XRNodeState node)
	{
		if (jpType != JoyPadType.Unknown)
		{
			return jpType;
		}
		if (mode == InputMode.Direct)
		{
			string nodeName = GetNodeName(node);
			if (nodeName.Contains("Oculus Touch Controller") || nodeName.StartsWith("Oculus Rift CV1"))
			{
				return jpType = JoyPadType.Joystick;
			}
			if (nodeName.StartsWith("Vive Controller"))
			{
				return jpType = JoyPadType.TouchPad;
			}
			Debug.LogWarning("Unknown controller type: " + nodeName);
			return jpType = JoyPadType.None;
		}
		return jpType = JoyPadType.Joystick | JoyPadType.TouchPad;
	}
}

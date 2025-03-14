using System.Runtime.CompilerServices;
using UnityEngine.Scripting;
using UnityEngine.XR;

namespace UnityEngine.Internal.VR;

public static class VRTestMock
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void Reset();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void AddTrackedDevice(XRNode nodeType);

	public static void UpdateTrackedDevice(XRNode nodeType, Vector3 position, Quaternion rotation)
	{
		INTERNAL_CALL_UpdateTrackedDevice(nodeType, ref position, ref rotation);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_UpdateTrackedDevice(XRNode nodeType, ref Vector3 position, ref Quaternion rotation);

	public static void UpdateLeftEye(Vector3 position, Quaternion rotation)
	{
		INTERNAL_CALL_UpdateLeftEye(ref position, ref rotation);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_UpdateLeftEye(ref Vector3 position, ref Quaternion rotation);

	public static void UpdateRightEye(Vector3 position, Quaternion rotation)
	{
		INTERNAL_CALL_UpdateRightEye(ref position, ref rotation);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_UpdateRightEye(ref Vector3 position, ref Quaternion rotation);

	public static void UpdateCenterEye(Vector3 position, Quaternion rotation)
	{
		INTERNAL_CALL_UpdateCenterEye(ref position, ref rotation);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_UpdateCenterEye(ref Vector3 position, ref Quaternion rotation);

	public static void UpdateHead(Vector3 position, Quaternion rotation)
	{
		INTERNAL_CALL_UpdateHead(ref position, ref rotation);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_UpdateHead(ref Vector3 position, ref Quaternion rotation);

	public static void UpdateLeftHand(Vector3 position, Quaternion rotation)
	{
		INTERNAL_CALL_UpdateLeftHand(ref position, ref rotation);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_UpdateLeftHand(ref Vector3 position, ref Quaternion rotation);

	public static void UpdateRightHand(Vector3 position, Quaternion rotation)
	{
		INTERNAL_CALL_UpdateRightHand(ref position, ref rotation);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_UpdateRightHand(ref Vector3 position, ref Quaternion rotation);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void AddController(string controllerName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void UpdateControllerAxis(string controllerName, int axis, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void UpdateControllerButton(string controllerName, int button, bool pressed);
}

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR;

/// <summary>
///   <para>Provides methods and properties that allow for querying portions of the physical environment that are near a provided specified ray. These trackables include planes and depth data.</para>
/// </summary>
[NativeConditional("ENABLE_XR")]
[NativeHeader("Modules/XR/Subsystems/Raycast/XRRaycastSubsystem.h")]
[NativeHeader("Modules/XR/XRPrefix.h")]
[UsedByNativeCode]
public class XRRaycastSubsystem : Subsystem<XRRaycastSubsystemDescriptor>
{
	public bool Raycast(Vector3 screenPoint, List<XRRaycastHit> hitResults, TrackableType trackableTypeMask = TrackableType.All)
	{
		if (hitResults == null)
		{
			throw new ArgumentNullException("hitResults");
		}
		float screenX = Mathf.Clamp01(screenPoint.x / (float)Screen.width);
		float screenY = Mathf.Clamp01(screenPoint.y / (float)Screen.height);
		return Internal_ScreenRaycastAsList(screenX, screenY, trackableTypeMask, hitResults);
	}

	public static void Raycast(Ray ray, XRDepthSubsystem depthSubsystem, XRPlaneSubsystem planeSubsystem, List<XRRaycastHit> hitResults, TrackableType trackableTypeMask = TrackableType.All, float pointCloudRaycastAngleInDegrees = 5f)
	{
		if (hitResults == null)
		{
			throw new ArgumentNullException("hitResults");
		}
		IntPtr depthSubsystem2 = depthSubsystem?.m_Ptr ?? IntPtr.Zero;
		IntPtr planeSubsystem2 = planeSubsystem?.m_Ptr ?? IntPtr.Zero;
		Internal_RaycastAsList(ray, pointCloudRaycastAngleInDegrees, depthSubsystem2, planeSubsystem2, trackableTypeMask, hitResults);
	}

	private static void Internal_RaycastAsList(Ray ray, float pointCloudRaycastAngleInDegrees, IntPtr depthSubsystem, IntPtr planeSubsystem, TrackableType trackableTypeMask, List<XRRaycastHit> hitResultsOut)
	{
		Internal_RaycastAsList_Injected(ref ray, pointCloudRaycastAngleInDegrees, depthSubsystem, planeSubsystem, trackableTypeMask, hitResultsOut);
	}

	private static XRRaycastHit[] Internal_RaycastAsFixedArray(Ray ray, float pointCloudRaycastAngleInDegrees, IntPtr depthSubsystem, IntPtr planeSubsystem, TrackableType trackableTypeMask)
	{
		return Internal_RaycastAsFixedArray_Injected(ref ray, pointCloudRaycastAngleInDegrees, depthSubsystem, planeSubsystem, trackableTypeMask);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern bool Internal_ScreenRaycastAsList(float screenX, float screenY, TrackableType hitMask, List<XRRaycastHit> hitResultsOut);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern XRRaycastHit[] Internal_ScreenRaycastAsFixedArray(float screenX, float screenY, TrackableType hitMask);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_RaycastAsList_Injected(ref Ray ray, float pointCloudRaycastAngleInDegrees, IntPtr depthSubsystem, IntPtr planeSubsystem, TrackableType trackableTypeMask, List<XRRaycastHit> hitResultsOut);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern XRRaycastHit[] Internal_RaycastAsFixedArray_Injected(ref Ray ray, float pointCloudRaycastAngleInDegrees, IntPtr depthSubsystem, IntPtr planeSubsystem, TrackableType trackableTypeMask);
}

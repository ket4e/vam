using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR.Tango;

[NativeHeader("Runtime/AR/Tango/TangoScriptApi.h")]
[UsedByNativeCode]
[NativeConditional("PLATFORM_ANDROID")]
internal static class TangoInputTracking
{
	private enum TrackingStateEventType
	{
		TrackingAcquired,
		TrackingLost
	}

	internal static event Action<CoordinateFrame> trackingAcquired;

	internal static event Action<CoordinateFrame> trackingLost;

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool Internal_TryGetPoseAtTime(double time, ScreenOrientation screenOrientation, CoordinateFrame baseFrame, CoordinateFrame targetFrame, out PoseData pose);

	internal static bool TryGetPoseAtTime(out PoseData pose, CoordinateFrame baseFrame, CoordinateFrame targetFrame, double time, ScreenOrientation screenOrientation)
	{
		return Internal_TryGetPoseAtTime(time, screenOrientation, baseFrame, targetFrame, out pose);
	}

	internal static bool TryGetPoseAtTime(out PoseData pose, CoordinateFrame baseFrame, CoordinateFrame targetFrame, double time = 0.0)
	{
		return Internal_TryGetPoseAtTime(time, Screen.orientation, baseFrame, targetFrame, out pose);
	}

	[UsedByNativeCode]
	private static void InvokeTangoTrackingEvent(TrackingStateEventType eventType, CoordinateFrame frame)
	{
		Action<CoordinateFrame> action = null;
		((Action<CoordinateFrame>)(eventType switch
		{
			TrackingStateEventType.TrackingAcquired => TangoInputTracking.trackingAcquired, 
			TrackingStateEventType.TrackingLost => TangoInputTracking.trackingLost, 
			_ => throw new ArgumentException("TrackingEventHandler - Invalid EventType: " + eventType), 
		}))?.Invoke(frame);
	}

	static TangoInputTracking()
	{
		TangoInputTracking.trackingAcquired = null;
		TangoInputTracking.trackingLost = null;
	}
}

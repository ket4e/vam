using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.XR;

/// <summary>
///   <para>Contains all functionality related to a XR device.</para>
/// </summary>
public static class XRDevice
{
	/// <summary>
	///   <para>Successfully detected a XR device in working order.</para>
	/// </summary>
	public static extern bool isPresent
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Indicates whether the user is present and interacting with the device.</para>
	/// </summary>
	public static extern UserPresenceState userPresence
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>The name of the family of the loaded XR device.</para>
	/// </summary>
	[Obsolete("family is deprecated.  Use XRSettings.loadedDeviceName instead.")]
	public static extern string family
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Specific model of loaded XR device.</para>
	/// </summary>
	public static extern string model
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Refresh rate of the display in Hertz.</para>
	/// </summary>
	public static extern float refreshRate
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Zooms the XR projection.</para>
	/// </summary>
	public static extern float fovZoomFactor
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Returns the device's current TrackingSpaceType. This value determines how the camera is positioned relative to its starting position. For more, see the section "Understanding the camera" in.</para>
	/// </summary>
	/// <returns>
	///   <para>The device's current TrackingSpaceType.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern TrackingSpaceType GetTrackingSpaceType();

	/// <summary>
	///   <para>Sets the device's current TrackingSpaceType. Returns true on success. Returns false if the given TrackingSpaceType is not supported or the device fails to switch.</para>
	/// </summary>
	/// <param name="TrackingSpaceType">The TrackingSpaceType the device should switch to.</param>
	/// <param name="trackingSpaceType"></param>
	/// <returns>
	///   <para>True on success. False if the given TrackingSpaceType is not supported or the device fails to switch.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern bool SetTrackingSpaceType(TrackingSpaceType trackingSpaceType);

	/// <summary>
	///   <para>This method returns an IntPtr representing the native pointer to the XR device if one is available, otherwise the value will be IntPtr.Zero.</para>
	/// </summary>
	/// <returns>
	///   <para>The native pointer to the XR device.</para>
	/// </returns>
	public static IntPtr GetNativePtr()
	{
		INTERNAL_CALL_GetNativePtr(out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetNativePtr(out IntPtr value);

	public static void DisableAutoXRCameraTracking(Camera camera, bool disabled)
	{
		if (camera == null)
		{
			throw new ArgumentNullException("camera");
		}
		DisableAutoXRCameraTrackingInternal(camera, disabled);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void DisableAutoXRCameraTrackingInternal(Camera camera, bool disabled);
}

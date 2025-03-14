using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.XR;

/// <summary>
///   <para>Global XR related settings.</para>
/// </summary>
public static class XRSettings
{
	/// <summary>
	///   <para>Globally enables or disables XR for the application.</para>
	/// </summary>
	public static extern bool enabled
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Read-only value that can be used to determine if the XR device is active.</para>
	/// </summary>
	public static extern bool isDeviceActive
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Mirror what is shown on the device to the main display, if possible.</para>
	/// </summary>
	public static extern bool showDeviceView
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>This field has been deprecated. Use XRSettings.eyeTextureResolutionScale instead.</para>
	/// </summary>
	[Obsolete("renderScale is deprecated, use XRSettings.eyeTextureResolutionScale instead (UnityUpgradable) -> eyeTextureResolutionScale")]
	public static extern float renderScale
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Controls the actual size of eye textures as a multiplier of the device's default resolution.</para>
	/// </summary>
	public static extern float eyeTextureResolutionScale
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The current width of an eye texture for the loaded device.</para>
	/// </summary>
	public static extern int eyeTextureWidth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>The current height of an eye texture for the loaded device.</para>
	/// </summary>
	public static extern int eyeTextureHeight
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	internal static extern float renderViewportScaleInternal
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Fetch the eye texture RenderTextureDescriptor from the active stereo device.</para>
	/// </summary>
	public static RenderTextureDescriptor eyeTextureDesc
	{
		get
		{
			INTERNAL_get_eyeTextureDesc(out var value);
			return value;
		}
	}

	/// <summary>
	///   <para>Controls how much of the allocated eye texture should be used for rendering.</para>
	/// </summary>
	public static float renderViewportScale
	{
		get
		{
			return renderViewportScaleInternal;
		}
		set
		{
			if (value < 0f || value > 1f)
			{
				throw new ArgumentOutOfRangeException("value", "Render viewport scale should be between 0 and 1.");
			}
			renderViewportScaleInternal = value;
		}
	}

	/// <summary>
	///   <para>A scale applied to the standard occulsion mask for each platform.</para>
	/// </summary>
	public static extern float occlusionMaskScale
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Specifies whether or not the occlusion mesh should be used when rendering. Enabled by default.</para>
	/// </summary>
	public static extern bool useOcclusionMesh
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Type of XR device that is currently loaded.</para>
	/// </summary>
	public static extern string loadedDeviceName
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Returns a list of supported XR devices that were included at build time.</para>
	/// </summary>
	public static extern string[] supportedDevices
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_get_eyeTextureDesc(out RenderTextureDescriptor value);

	/// <summary>
	///   <para>Loads the requested device at the beginning of the next frame.</para>
	/// </summary>
	/// <param name="deviceName">Name of the device from XRSettings.supportedDevices.</param>
	/// <param name="prioritizedDeviceNameList">Prioritized list of device names from XRSettings.supportedDevices.</param>
	public static void LoadDeviceByName(string deviceName)
	{
		LoadDeviceByName(new string[1] { deviceName });
	}

	/// <summary>
	///   <para>Loads the requested device at the beginning of the next frame.</para>
	/// </summary>
	/// <param name="deviceName">Name of the device from XRSettings.supportedDevices.</param>
	/// <param name="prioritizedDeviceNameList">Prioritized list of device names from XRSettings.supportedDevices.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void LoadDeviceByName(string[] prioritizedDeviceNameList);
}

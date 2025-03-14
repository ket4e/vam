using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.XR.Tango;

[NativeConditional("PLATFORM_ANDROID")]
[NativeHeader("Runtime/AR/Tango/TangoScriptApi.h")]
internal static class TangoDevice
{
	private static ARBackgroundRenderer m_BackgroundRenderer = null;

	private static string m_AreaDescriptionUUID = "";

	internal static extern CoordinateFrame baseCoordinateFrame
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeConditional(false)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeThrows]
		set;
	}

	internal static extern uint depthCameraRate
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	internal static extern bool synchronizeFramerateWithColorCamera
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	internal static extern bool isServiceConnected
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	internal static extern bool isServiceAvailable
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	internal static string areaDescriptionUUID
	{
		get
		{
			return m_AreaDescriptionUUID;
		}
		set
		{
			m_AreaDescriptionUUID = value;
		}
	}

	internal static ARBackgroundRenderer backgroundRenderer
	{
		get
		{
			return m_BackgroundRenderer;
		}
		set
		{
			if (value != null)
			{
				if (m_BackgroundRenderer != null)
				{
					m_BackgroundRenderer.backgroundRendererChanged -= OnBackgroundRendererChanged;
				}
				m_BackgroundRenderer = value;
				m_BackgroundRenderer.backgroundRendererChanged += OnBackgroundRendererChanged;
				OnBackgroundRendererChanged();
			}
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern bool Connect(string[] boolKeys, bool[] boolValues, string[] intKeys, int[] intValues, string[] longKeys, long[] longValues, string[] doubleKeys, double[] doubleValues, string[] stringKeys, string[] stringValues);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern void Disconnect();

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern bool TryGetHorizontalFov(out float fovOut);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern bool TryGetVerticalFov(out float fovOut);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern void SetRenderMode(ARRenderMode mode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern void SetBackgroundMaterial(Material material);

	internal static bool TryGetLatestPointCloud(ref PointCloudData pointCloudData)
	{
		if (pointCloudData.points == null)
		{
			pointCloudData.points = new List<Vector4>();
		}
		pointCloudData.points.Clear();
		return TryGetLatestPointCloudInternal(pointCloudData.points, out pointCloudData.version, out pointCloudData.timestamp);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool TryGetLatestPointCloudInternal(List<Vector4> pointCloudData, out uint version, out double timestamp);

	internal static bool TryGetLatestImageData(ref ImageData image)
	{
		if (image.planeData == null)
		{
			image.planeData = new List<byte>();
		}
		if (image.planeInfos == null)
		{
			image.planeInfos = new List<ImageData.PlaneInfo>();
		}
		image.planeData.Clear();
		return TryGetLatestImageDataInternal(image.planeData, image.planeInfos, out image.width, out image.height, out image.format, out image.timestampNs, out image.metadata);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool TryGetLatestImageDataInternal(List<byte> imageData, List<ImageData.PlaneInfo> planeInfos, out uint width, out uint height, out int format, out long timestampNs, out ImageData.CameraMetadata metadata);

	internal static bool TryAcquireLatestPointCloud(ref NativePointCloud pointCloud)
	{
		return Internal_TryAcquireLatestPointCloud(out pointCloud.version, out pointCloud.timestamp, out pointCloud.numPoints, out pointCloud.points, out pointCloud.nativePtr);
	}

	internal static void ReleasePointCloud(IntPtr pointCloudNativePtr)
	{
		Internal_ReleasePointCloud(pointCloudNativePtr);
	}

	internal static bool TryAcquireLatestImageBuffer(ref NativeImage nativeImage)
	{
		if (nativeImage.planeInfos == null)
		{
			nativeImage.planeInfos = new List<ImageData.PlaneInfo>();
		}
		return Internal_TryAcquireLatestImageBuffer(nativeImage.planeInfos, out nativeImage.width, out nativeImage.height, out nativeImage.format, out nativeImage.timestampNs, out nativeImage.planeData, out nativeImage.nativePtr, out nativeImage.metadata);
	}

	internal static void ReleaseImageBuffer(IntPtr imageBufferNativePtr)
	{
		Internal_ReleaseImageBuffer(imageBufferNativePtr);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool Internal_TryAcquireLatestImageBuffer(List<ImageData.PlaneInfo> planeInfos, out uint width, out uint height, out int format, out long timestampNs, out IntPtr planeData, out IntPtr nativePtr, out ImageData.CameraMetadata metadata);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool Internal_TryAcquireLatestPointCloud(out uint version, out double timestamp, out uint numPoints, out IntPtr points, out IntPtr nativePtr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	private static extern void Internal_ReleasePointCloud(IntPtr pointCloudPtr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	private static extern void Internal_ReleaseImageBuffer(IntPtr imageBufferPtr);

	private static void OnBackgroundRendererChanged()
	{
		SetBackgroundMaterial(m_BackgroundRenderer.backgroundMaterial);
		SetRenderMode(m_BackgroundRenderer.mode);
	}

	internal static bool Connect(TangoConfig config)
	{
		CopyDictionaryToArrays(config.m_boolParams, out var keys, out var values);
		CopyDictionaryToArrays(config.m_intParams, out var keys2, out var values2);
		CopyDictionaryToArrays(config.m_longParams, out var keys3, out var values3);
		CopyDictionaryToArrays(config.m_doubleParams, out var keys4, out var values4);
		CopyDictionaryToArrays(config.m_stringParams, out var keys5, out var values5);
		return Connect(keys, values, keys2, values2, keys3, values3, keys4, values4, keys5, values5);
	}

	private static void CopyDictionaryToArrays<T>(Dictionary<string, T> dictionary, out string[] keys, out T[] values)
	{
		if (dictionary.Count == 0)
		{
			keys = null;
			values = null;
			return;
		}
		keys = new string[dictionary.Count];
		values = new T[dictionary.Count];
		int num = 0;
		foreach (KeyValuePair<string, T> item in dictionary)
		{
			keys[num] = item.Key;
			values[num++] = item.Value;
		}
	}
}

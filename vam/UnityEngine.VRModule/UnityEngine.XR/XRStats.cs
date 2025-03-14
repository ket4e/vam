using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.XR;

/// <summary>
///   <para>Timing and other statistics from the XR subsystem.</para>
/// </summary>
public static class XRStats
{
	/// <summary>
	///   <para>Total GPU time utilized last frame as measured by the XR subsystem.</para>
	/// </summary>
	[Obsolete("gpuTimeLastFrame is deprecated. Use XRStats.TryGetGPUTimeLastFrame instead.")]
	public static float gpuTimeLastFrame
	{
		get
		{
			if (TryGetGPUTimeLastFrame(out var result))
			{
				return result;
			}
			return 0f;
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern bool TryGetGPUTimeLastFrame(out float gpuTimeLastFrame);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern bool TryGetDroppedFrameCount(out int droppedFrameCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern bool TryGetFramePresentCount(out int framePresentCount);
}

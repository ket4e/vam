using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine;

/// <summary>
///   <para>Script interface for.</para>
/// </summary>
[NativeHeader("Runtime/Graphics/QualitySettings.h")]
[StaticAccessor("GetQualitySettings()", StaticAccessorType.Dot)]
[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
[NativeHeader("Runtime/Misc/PlayerSettings.h")]
public sealed class QualitySettings : Object
{
	[Obsolete("Use GetQualityLevel and SetQualityLevel", false)]
	public static QualityLevel currentLevel
	{
		get
		{
			return (QualityLevel)GetQualityLevel();
		}
		set
		{
			SetQualityLevel((int)value, applyExpensiveChanges: true);
		}
	}

	/// <summary>
	///   <para>The maximum number of pixel lights that should affect any object.</para>
	/// </summary>
	public static extern int pixelLightCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Realtime Shadows type to be used.</para>
	/// </summary>
	[NativeProperty("ShadowQuality")]
	public static extern ShadowQuality shadows
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Directional light shadow projection.</para>
	/// </summary>
	public static extern ShadowProjection shadowProjection
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Number of cascades to use for directional light shadows.</para>
	/// </summary>
	public static extern int shadowCascades
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Shadow drawing distance.</para>
	/// </summary>
	public static extern float shadowDistance
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The default resolution of the shadow maps.</para>
	/// </summary>
	public static extern ShadowResolution shadowResolution
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The rendering mode of Shadowmask.</para>
	/// </summary>
	public static extern ShadowmaskMode shadowmaskMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Offset shadow frustum near plane.</para>
	/// </summary>
	public static extern float shadowNearPlaneOffset
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The normalized cascade distribution for a 2 cascade setup. The value defines the position of the cascade with respect to Zero.</para>
	/// </summary>
	public static extern float shadowCascade2Split
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The normalized cascade start position for a 4 cascade setup. Each member of the vector defines the normalized position of the coresponding cascade with respect to Zero.</para>
	/// </summary>
	public static Vector3 shadowCascade4Split
	{
		get
		{
			get_shadowCascade4Split_Injected(out var ret);
			return ret;
		}
		set
		{
			set_shadowCascade4Split_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>Global multiplier for the LOD's switching distance.</para>
	/// </summary>
	[NativeProperty("LODBias")]
	public static extern float lodBias
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Global anisotropic filtering mode.</para>
	/// </summary>
	[NativeProperty("AnisotropicTextures")]
	public static extern AnisotropicFiltering anisotropicFiltering
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>A texture size limit applied to all textures.</para>
	/// </summary>
	public static extern int masterTextureLimit
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>A maximum LOD level. All LOD groups.</para>
	/// </summary>
	public static extern int maximumLODLevel
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Budget for how many ray casts can be performed per frame for approximate collision testing.</para>
	/// </summary>
	public static extern int particleRaycastBudget
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Should soft blending be used for particles?</para>
	/// </summary>
	public static extern bool softParticles
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Use a two-pass shader for the vegetation in the terrain engine.</para>
	/// </summary>
	public static extern bool softVegetation
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The VSync Count.</para>
	/// </summary>
	public static extern int vSyncCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Set The AA Filtering option.</para>
	/// </summary>
	public static extern int antiAliasing
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Async texture upload provides timesliced async texture upload on the render thread with tight control over memory and timeslicing. There are no allocations except for the ones which driver has to do. To read data and upload texture data a ringbuffer whose size can be controlled is re-used.
	///
	/// Use asyncUploadTimeSlice to set the time-slice in milliseconds for asynchronous texture uploads per
	/// frame. Minimum value is 1 and maximum is 33.</para>
	/// </summary>
	public static extern int asyncUploadTimeSlice
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Async texture upload provides timesliced async texture upload on the render thread with tight control over memory and timeslicing. There are no allocations except for the ones which driver has to do. To read data and upload texture data a ringbuffer whose size can be controlled is re-used.
	///
	/// Use asyncUploadBufferSize to set the buffer size for asynchronous texture uploads. The size is in megabytes. Minimum value is 2 and maximum is 512. Although the buffer will resize automatically to fit the largest texture currently loading, it is recommended to set the value approximately to the size of biggest texture used in the scene to avoid re-sizing of the buffer which can incur performance cost.</para>
	/// </summary>
	public static extern int asyncUploadBufferSize
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Enables realtime reflection probes.</para>
	/// </summary>
	public static extern bool realtimeReflectionProbes
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>If enabled, billboards will face towards camera position rather than camera orientation.</para>
	/// </summary>
	public static extern bool billboardsFaceCameraPosition
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>In resolution scaling mode, this factor is used to multiply with the target Fixed DPI specified to get the actual Fixed DPI to use for this quality setting.</para>
	/// </summary>
	public static extern float resolutionScalingFixedDPIFactor
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Blend weights.</para>
	/// </summary>
	public static extern BlendWeights blendWeights
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The indexed list of available Quality Settings.</para>
	/// </summary>
	[NativeProperty("QualitySettingsNames")]
	public static extern string[] names
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	/// <summary>
	///   <para>Maximum number of frames queued up by graphics driver.</para>
	/// </summary>
	[StaticAccessor("QualitySettingsScripting", StaticAccessorType.DoubleColon)]
	public static extern int maxQueuedFrames
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Desired color space (Read Only).</para>
	/// </summary>
	public static extern ColorSpace desiredColorSpace
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[StaticAccessor("GetPlayerSettings()", StaticAccessorType.Dot)]
		[NativeName("GetColorSpace")]
		get;
	}

	/// <summary>
	///   <para>Active color space (Read Only).</para>
	/// </summary>
	public static extern ColorSpace activeColorSpace
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[StaticAccessor("GetPlayerSettings()", StaticAccessorType.Dot)]
		[NativeName("GetColorSpace")]
		get;
	}

	private QualitySettings()
	{
	}

	/// <summary>
	///   <para>Increase the current quality level.</para>
	/// </summary>
	/// <param name="applyExpensiveChanges">Should expensive changes be applied (Anti-aliasing etc).</param>
	public static void IncreaseLevel([DefaultValue("false")] bool applyExpensiveChanges)
	{
		SetQualityLevel(GetQualityLevel() + 1, applyExpensiveChanges);
	}

	/// <summary>
	///   <para>Decrease the current quality level.</para>
	/// </summary>
	/// <param name="applyExpensiveChanges">Should expensive changes be applied (Anti-aliasing etc).</param>
	public static void DecreaseLevel([DefaultValue("false")] bool applyExpensiveChanges)
	{
		SetQualityLevel(GetQualityLevel() - 1, applyExpensiveChanges);
	}

	public static void SetQualityLevel(int index)
	{
		SetQualityLevel(index, applyExpensiveChanges: true);
	}

	public static void IncreaseLevel()
	{
		IncreaseLevel(applyExpensiveChanges: false);
	}

	public static void DecreaseLevel()
	{
		DecreaseLevel(applyExpensiveChanges: false);
	}

	/// <summary>
	///   <para>Returns the current graphics quality level.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetCurrentIndex")]
	public static extern int GetQualityLevel();

	/// <summary>
	///   <para>Sets a new graphics quality level.</para>
	/// </summary>
	/// <param name="index">Quality index to set.</param>
	/// <param name="applyExpensiveChanges">Should expensive changes be applied (Anti-aliasing etc).</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("SetCurrentIndex")]
	public static extern void SetQualityLevel(int index, [DefaultValue("true")] bool applyExpensiveChanges);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void get_shadowCascade4Split_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private static extern void set_shadowCascade4Split_Injected(ref Vector3 value);
}

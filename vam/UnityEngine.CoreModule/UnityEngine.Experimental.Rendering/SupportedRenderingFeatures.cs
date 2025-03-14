using System;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering;

/// <summary>
///   <para>Describes the rendering features supported by a given render pipeline.</para>
/// </summary>
public class SupportedRenderingFeatures
{
	/// <summary>
	///   <para>Supported modes for ReflectionProbes.</para>
	/// </summary>
	[Flags]
	public enum ReflectionProbeSupportFlags
	{
		/// <summary>
		///   <para>Default reflection probe support.</para>
		/// </summary>
		None = 0,
		/// <summary>
		///   <para>Rotated reflection probes are supported.</para>
		/// </summary>
		Rotation = 1
	}

	/// <summary>
	///   <para>Same as MixedLightingMode for baking, but is used to determine what is supported by the pipeline.</para>
	/// </summary>
	[Flags]
	public enum LightmapMixedBakeMode
	{
		/// <summary>
		///   <para>No mode is supported.</para>
		/// </summary>
		None = 0,
		/// <summary>
		///   <para>Same as MixedLightingMode.IndirectOnly but determines if it is supported by the pipeline.</para>
		/// </summary>
		IndirectOnly = 1,
		/// <summary>
		///   <para>Same as MixedLightingMode.Subtractive but determines if it is supported by the pipeline.</para>
		/// </summary>
		Subtractive = 2,
		/// <summary>
		///   <para>Same as MixedLightingMode.Shadowmask but determines if it is supported by the pipeline.</para>
		/// </summary>
		Shadowmask = 4
	}

	private static SupportedRenderingFeatures s_Active = new SupportedRenderingFeatures();

	/// <summary>
	///   <para>Get / Set a SupportedRenderingFeatures.</para>
	/// </summary>
	public static SupportedRenderingFeatures active
	{
		get
		{
			if (s_Active == null)
			{
				s_Active = new SupportedRenderingFeatures();
			}
			return s_Active;
		}
		set
		{
			s_Active = value;
		}
	}

	/// <summary>
	///   <para>Flags for supported reflection probes.</para>
	/// </summary>
	public ReflectionProbeSupportFlags reflectionProbeSupportFlags { get; set; } = ReflectionProbeSupportFlags.None;


	/// <summary>
	///   <para>This is the fallback mode if the mode the user had previously selected is no longer available. See SupportedRenderingFeatures.supportedMixedLightingModes.</para>
	/// </summary>
	public LightmapMixedBakeMode defaultMixedLightingMode { get; set; } = LightmapMixedBakeMode.None;


	/// <summary>
	///   <para>Specifies what LightmapMixedBakeMode that are supported. Please define a SupportedRenderingFeatures.defaultMixedLightingMode in case multiple modes are supported.</para>
	/// </summary>
	public LightmapMixedBakeMode supportedMixedLightingModes { get; set; } = LightmapMixedBakeMode.IndirectOnly | LightmapMixedBakeMode.Subtractive | LightmapMixedBakeMode.Shadowmask;


	/// <summary>
	///   <para>What baking types are supported. The unsupported ones will be hidden from the UI. See LightmapBakeType.</para>
	/// </summary>
	public LightmapBakeType supportedLightmapBakeTypes { get; set; } = LightmapBakeType.Realtime | LightmapBakeType.Baked | LightmapBakeType.Mixed;


	/// <summary>
	///   <para>Specifies what modes are supported. Has to be at least one. See LightmapsMode.</para>
	/// </summary>
	public LightmapsMode supportedLightmapsModes { get; set; } = LightmapsMode.CombinedDirectional;


	/// <summary>
	///   <para>Are light probe proxy volumes supported?</para>
	/// </summary>
	public bool rendererSupportsLightProbeProxyVolumes { get; set; } = true;


	/// <summary>
	///   <para>Are motion vectors supported?</para>
	/// </summary>
	public bool rendererSupportsMotionVectors { get; set; } = true;


	/// <summary>
	///   <para>Can renderers support receiving shadows?</para>
	/// </summary>
	public bool rendererSupportsReceiveShadows { get; set; } = true;


	/// <summary>
	///   <para>Are reflection probes supported?</para>
	/// </summary>
	public bool rendererSupportsReflectionProbes { get; set; } = true;


	internal unsafe static MixedLightingMode FallbackMixedLightingMode()
	{
		MixedLightingMode result = default(MixedLightingMode);
		FallbackMixedLightingModeByRef(new IntPtr(&result));
		return result;
	}

	[RequiredByNativeCode]
	internal unsafe static void FallbackMixedLightingModeByRef(IntPtr fallbackModePtr)
	{
		MixedLightingMode* ptr = (MixedLightingMode*)(void*)fallbackModePtr;
		if (active.defaultMixedLightingMode != 0 && (active.supportedMixedLightingModes & active.defaultMixedLightingMode) == active.defaultMixedLightingMode)
		{
			switch (active.defaultMixedLightingMode)
			{
			case LightmapMixedBakeMode.Shadowmask:
				*ptr = MixedLightingMode.Shadowmask;
				break;
			case LightmapMixedBakeMode.Subtractive:
				*ptr = MixedLightingMode.Subtractive;
				break;
			default:
				*ptr = MixedLightingMode.IndirectOnly;
				break;
			}
		}
		else if (IsMixedLightingModeSupported(MixedLightingMode.Shadowmask))
		{
			*ptr = MixedLightingMode.Shadowmask;
		}
		else if (IsMixedLightingModeSupported(MixedLightingMode.Subtractive))
		{
			*ptr = MixedLightingMode.Subtractive;
		}
		else
		{
			*ptr = MixedLightingMode.IndirectOnly;
		}
	}

	internal unsafe static bool IsMixedLightingModeSupported(MixedLightingMode mixedMode)
	{
		bool result = default(bool);
		IsMixedLightingModeSupportedByRef(mixedMode, new IntPtr(&result));
		return result;
	}

	[RequiredByNativeCode]
	internal unsafe static void IsMixedLightingModeSupportedByRef(MixedLightingMode mixedMode, IntPtr isSupportedPtr)
	{
		bool* ptr = (bool*)(void*)isSupportedPtr;
		if (!IsLightmapBakeTypeSupported(LightmapBakeType.Mixed))
		{
			*ptr = false;
		}
		else
		{
			*ptr = (mixedMode == MixedLightingMode.IndirectOnly && (active.supportedMixedLightingModes & LightmapMixedBakeMode.IndirectOnly) == LightmapMixedBakeMode.IndirectOnly) || (mixedMode == MixedLightingMode.Subtractive && (active.supportedMixedLightingModes & LightmapMixedBakeMode.Subtractive) == LightmapMixedBakeMode.Subtractive) || (mixedMode == MixedLightingMode.Shadowmask && (active.supportedMixedLightingModes & LightmapMixedBakeMode.Shadowmask) == LightmapMixedBakeMode.Shadowmask);
		}
	}

	internal unsafe static bool IsLightmapBakeTypeSupported(LightmapBakeType bakeType)
	{
		bool result = default(bool);
		IsLightmapBakeTypeSupportedByRef(bakeType, new IntPtr(&result));
		return result;
	}

	[RequiredByNativeCode]
	internal unsafe static void IsLightmapBakeTypeSupportedByRef(LightmapBakeType bakeType, IntPtr isSupportedPtr)
	{
		bool* ptr = (bool*)(void*)isSupportedPtr;
		if (bakeType == LightmapBakeType.Mixed && (!IsLightmapBakeTypeSupported(LightmapBakeType.Baked) || active.supportedMixedLightingModes == LightmapMixedBakeMode.None))
		{
			*ptr = false;
		}
		else
		{
			*ptr = (active.supportedLightmapBakeTypes & bakeType) == bakeType;
		}
	}

	internal unsafe static bool IsLightmapsModeSupported(LightmapsMode mode)
	{
		bool result = default(bool);
		IsLightmapsModeSupportedByRef(mode, new IntPtr(&result));
		return result;
	}

	[RequiredByNativeCode]
	internal unsafe static void IsLightmapsModeSupportedByRef(LightmapsMode mode, IntPtr isSupportedPtr)
	{
		bool* ptr = (bool*)(void*)isSupportedPtr;
		*ptr = (active.supportedLightmapsModes & mode) == mode;
	}
}

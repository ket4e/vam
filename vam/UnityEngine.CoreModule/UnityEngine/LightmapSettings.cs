using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Stores lightmaps of the scene.</para>
/// </summary>
public sealed class LightmapSettings : Object
{
	/// <summary>
	///   <para>Lightmap array.</para>
	/// </summary>
	public static extern LightmapData[] lightmaps
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Non-directional, Directional or Directional Specular lightmaps rendering mode.</para>
	/// </summary>
	public static extern LightmapsMode lightmapsMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Holds all data needed by the light probes.</para>
	/// </summary>
	public static extern LightProbes lightProbes
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	[Obsolete("Use lightmapsMode instead.", false)]
	public static LightmapsModeLegacy lightmapsModeLegacy
	{
		get
		{
			return LightmapsModeLegacy.Single;
		}
		set
		{
		}
	}

	[Obsolete("Use QualitySettings.desiredColorSpace instead.", false)]
	public static ColorSpace bakedColorSpace
	{
		get
		{
			return QualitySettings.desiredColorSpace;
		}
		set
		{
		}
	}

	private LightmapSettings()
	{
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal static extern void Reset();
}

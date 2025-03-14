using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Allows to control the dynamic Global Illumination.</para>
/// </summary>
public sealed class DynamicGI
{
	/// <summary>
	///   <para>Allows for scaling the contribution coming from realtime &amp; static  lightmaps.</para>
	/// </summary>
	public static extern float indirectScale
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Threshold for limiting updates of realtime GI. The unit of measurement is "percentage intensity change".</para>
	/// </summary>
	public static extern float updateThreshold
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>When enabled, new dynamic Global Illumination output is shown in each frame.</para>
	/// </summary>
	public static extern bool synchronousMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Is precomputed realtime Global Illumination output converged?</para>
	/// </summary>
	public static extern bool isConverged
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Allows to set an emissive color for a given renderer quickly, without the need to render the emissive input for the entire system.</para>
	/// </summary>
	/// <param name="renderer">The Renderer that should get a new color.</param>
	/// <param name="color">The emissive Color.</param>
	public static void SetEmissive(Renderer renderer, Color color)
	{
		INTERNAL_CALL_SetEmissive(renderer, ref color);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_SetEmissive(Renderer renderer, ref Color color);

	/// <summary>
	///   <para>Allows overriding the distant environment lighting for Realtime GI, without changing the Skybox Material.</para>
	/// </summary>
	/// <param name="input">Array of float values to be used for Realtime GI environment lighting.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void SetEnvironmentData(float[] input);

	/// <summary>
	///   <para>Schedules an update of the albedo and emissive textures of a system that contains the renderer or the terrain.</para>
	/// </summary>
	/// <param name="renderer">The Renderer to use when searching for a system to update.</param>
	/// <param name="terrain">The Terrain to use when searching for systems to update.</param>
	[Obsolete("DynamicGI.UpdateMaterials(Renderer) is deprecated; instead, use extension method from RendererExtensions: 'renderer.UpdateGIMaterials()' (UnityUpgradable).", true)]
	public static void UpdateMaterials(Renderer renderer)
	{
	}

	[Obsolete("DynamicGI.UpdateMaterials(Terrain) is deprecated; instead, use extension method from TerrainExtensions: 'terrain.UpdateGIMaterials()' (UnityUpgradable).", true)]
	public static void UpdateMaterials(Object renderer)
	{
	}

	[Obsolete("DynamicGI.UpdateMaterials(Terrain, int, int, int, int) is deprecated; instead, use extension method from TerrainExtensions: 'terrain.UpdateGIMaterials(x, y, width, height)' (UnityUpgradable).", true)]
	public static void UpdateMaterials(Object renderer, int x, int y, int width, int height)
	{
	}

	/// <summary>
	///   <para>Schedules an update of the environment texture.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void UpdateEnvironment();
}

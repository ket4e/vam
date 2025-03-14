using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering;

/// <summary>
///   <para>Culling results (visible objects, lights, reflection probes).</para>
/// </summary>
[UsedByNativeCode]
public struct CullResults
{
	/// <summary>
	///   <para>Array of visible lights.</para>
	/// </summary>
	public List<VisibleLight> visibleLights;

	/// <summary>
	///   <para>Off screen lights that still effect visible scene vertices.</para>
	/// </summary>
	public List<VisibleLight> visibleOffscreenVertexLights;

	/// <summary>
	///   <para>Array of visible reflection probes.</para>
	/// </summary>
	public List<VisibleReflectionProbe> visibleReflectionProbes;

	/// <summary>
	///   <para>Visible renderers.</para>
	/// </summary>
	public FilterResults visibleRenderers;

	internal IntPtr cullResults;

	private void Init()
	{
		visibleLights = new List<VisibleLight>();
		visibleOffscreenVertexLights = new List<VisibleLight>();
		visibleReflectionProbes = new List<VisibleReflectionProbe>();
		visibleRenderers = default(FilterResults);
		cullResults = IntPtr.Zero;
	}

	public unsafe static bool GetCullingParameters(Camera camera, out ScriptableCullingParameters cullingParameters)
	{
		return GetCullingParameters_Internal(camera, stereoAware: false, out cullingParameters, sizeof(ScriptableCullingParameters));
	}

	public unsafe static bool GetCullingParameters(Camera camera, bool stereoAware, out ScriptableCullingParameters cullingParameters)
	{
		return GetCullingParameters_Internal(camera, stereoAware, out cullingParameters, sizeof(ScriptableCullingParameters));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool GetCullingParameters_Internal(Camera camera, bool stereoAware, out ScriptableCullingParameters cullingParameters, int managedCullingParametersSize);

	internal static void Internal_Cull(ref ScriptableCullingParameters parameters, ScriptableRenderContext renderLoop, ref CullResults results)
	{
		INTERNAL_CALL_Internal_Cull(ref parameters, ref renderLoop, ref results);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Internal_Cull(ref ScriptableCullingParameters parameters, ref ScriptableRenderContext renderLoop, ref CullResults results);

	public static CullResults Cull(ref ScriptableCullingParameters parameters, ScriptableRenderContext renderLoop)
	{
		CullResults results = default(CullResults);
		Cull(ref parameters, renderLoop, ref results);
		return results;
	}

	public static void Cull(ref ScriptableCullingParameters parameters, ScriptableRenderContext renderLoop, ref CullResults results)
	{
		if (results.visibleLights == null || results.visibleOffscreenVertexLights == null || results.visibleReflectionProbes == null)
		{
			results.Init();
		}
		Internal_Cull(ref parameters, renderLoop, ref results);
	}

	public static bool Cull(Camera camera, ScriptableRenderContext renderLoop, out CullResults results)
	{
		results.cullResults = IntPtr.Zero;
		results.visibleLights = null;
		results.visibleOffscreenVertexLights = null;
		results.visibleReflectionProbes = null;
		results.visibleRenderers = default(FilterResults);
		if (!GetCullingParameters(camera, out var cullingParameters))
		{
			return false;
		}
		results = Cull(ref cullingParameters, renderLoop);
		return true;
	}

	public bool GetShadowCasterBounds(int lightIndex, out Bounds outBounds)
	{
		return GetShadowCasterBounds(cullResults, lightIndex, out outBounds);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool GetShadowCasterBounds(IntPtr cullResults, int lightIndex, out Bounds bounds);

	/// <summary>
	///   <para>Gets the number of per-object light indices.</para>
	/// </summary>
	/// <returns>
	///   <para>The number of per-object light indices.</para>
	/// </returns>
	public int GetLightIndicesCount()
	{
		return GetLightIndicesCount(cullResults);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern int GetLightIndicesCount(IntPtr cullResults);

	/// <summary>
	///   <para>Fills a compute buffer with per-object light indices.</para>
	/// </summary>
	/// <param name="computeBuffer">The compute buffer object to fill.</param>
	public void FillLightIndices(ComputeBuffer computeBuffer)
	{
		FillLightIndices(cullResults, computeBuffer);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void FillLightIndices(IntPtr cullResults, ComputeBuffer computeBuffer);

	/// <summary>
	///   <para>If a RenderPipeline sorts or otherwise modifies the VisibleLight list, an index remap will be necessary to properly make use of per-object light lists.</para>
	/// </summary>
	/// <returns>
	///   <para>Array of indices that map from VisibleLight indices to internal per-object light list indices.</para>
	/// </returns>
	public int[] GetLightIndexMap()
	{
		return GetLightIndexMap(cullResults);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern int[] GetLightIndexMap(IntPtr cullResults);

	/// <summary>
	///   <para>If a RenderPipeline sorts or otherwise modifies the VisibleLight list, an index remap will be necessary to properly make use of per-object light lists.
	/// If an element of the array is set to -1, the light corresponding to that element will be disabled.</para>
	/// </summary>
	/// <param name="mapping">Array with light indices that map from VisibleLight to internal per-object light lists.</param>
	public void SetLightIndexMap(int[] mapping)
	{
		SetLightIndexMap(cullResults, mapping);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void SetLightIndexMap(IntPtr cullResults, int[] mapping);

	public bool ComputeSpotShadowMatricesAndCullingPrimitives(int activeLightIndex, out Matrix4x4 viewMatrix, out Matrix4x4 projMatrix, out ShadowSplitData shadowSplitData)
	{
		return ComputeSpotShadowMatricesAndCullingPrimitives(cullResults, activeLightIndex, out viewMatrix, out projMatrix, out shadowSplitData);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool ComputeSpotShadowMatricesAndCullingPrimitives(IntPtr cullResults, int activeLightIndex, out Matrix4x4 viewMatrix, out Matrix4x4 projMatrix, out ShadowSplitData shadowSplitData);

	public bool ComputePointShadowMatricesAndCullingPrimitives(int activeLightIndex, CubemapFace cubemapFace, float fovBias, out Matrix4x4 viewMatrix, out Matrix4x4 projMatrix, out ShadowSplitData shadowSplitData)
	{
		return ComputePointShadowMatricesAndCullingPrimitives(cullResults, activeLightIndex, cubemapFace, fovBias, out viewMatrix, out projMatrix, out shadowSplitData);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool ComputePointShadowMatricesAndCullingPrimitives(IntPtr cullResults, int activeLightIndex, CubemapFace cubemapFace, float fovBias, out Matrix4x4 viewMatrix, out Matrix4x4 projMatrix, out ShadowSplitData shadowSplitData);

	public bool ComputeDirectionalShadowMatricesAndCullingPrimitives(int activeLightIndex, int splitIndex, int splitCount, Vector3 splitRatio, int shadowResolution, float shadowNearPlaneOffset, out Matrix4x4 viewMatrix, out Matrix4x4 projMatrix, out ShadowSplitData shadowSplitData)
	{
		return ComputeDirectionalShadowMatricesAndCullingPrimitives(cullResults, activeLightIndex, splitIndex, splitCount, splitRatio, shadowResolution, shadowNearPlaneOffset, out viewMatrix, out projMatrix, out shadowSplitData);
	}

	private static bool ComputeDirectionalShadowMatricesAndCullingPrimitives(IntPtr cullResults, int activeLightIndex, int splitIndex, int splitCount, Vector3 splitRatio, int shadowResolution, float shadowNearPlaneOffset, out Matrix4x4 viewMatrix, out Matrix4x4 projMatrix, out ShadowSplitData shadowSplitData)
	{
		return INTERNAL_CALL_ComputeDirectionalShadowMatricesAndCullingPrimitives(cullResults, activeLightIndex, splitIndex, splitCount, ref splitRatio, shadowResolution, shadowNearPlaneOffset, out viewMatrix, out projMatrix, out shadowSplitData);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool INTERNAL_CALL_ComputeDirectionalShadowMatricesAndCullingPrimitives(IntPtr cullResults, int activeLightIndex, int splitIndex, int splitCount, ref Vector3 splitRatio, int shadowResolution, float shadowNearPlaneOffset, out Matrix4x4 viewMatrix, out Matrix4x4 projMatrix, out ShadowSplitData shadowSplitData);
}

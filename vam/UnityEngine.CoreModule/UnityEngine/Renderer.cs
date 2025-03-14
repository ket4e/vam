using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;
using UnityEngineInternal;

namespace UnityEngine;

/// <summary>
///   <para>General functionality for all renderers.</para>
/// </summary>
[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
[NativeHeader("Runtime/Graphics/Renderer.h")]
[RequireComponent(typeof(Transform))]
public class Renderer : Component
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Use shadowCastingMode instead.", false)]
	public bool castShadows
	{
		get
		{
			return shadowCastingMode != ShadowCastingMode.Off;
		}
		set
		{
			shadowCastingMode = (value ? ShadowCastingMode.On : ShadowCastingMode.Off);
		}
	}

	/// <summary>
	///   <para>Specifies whether this renderer has a per-object motion vector pass.</para>
	/// </summary>
	[Obsolete("Use motionVectorGenerationMode instead.", false)]
	public bool motionVectors
	{
		get
		{
			return motionVectorGenerationMode == MotionVectorGenerationMode.Object;
		}
		set
		{
			motionVectorGenerationMode = (value ? MotionVectorGenerationMode.Object : MotionVectorGenerationMode.Camera);
		}
	}

	/// <summary>
	///   <para>Should light probes be used for this Renderer?</para>
	/// </summary>
	[Obsolete("Use lightProbeUsage instead.", false)]
	public bool useLightProbes
	{
		get
		{
			return lightProbeUsage != LightProbeUsage.Off;
		}
		set
		{
			lightProbeUsage = (value ? LightProbeUsage.BlendProbes : LightProbeUsage.Off);
		}
	}

	/// <summary>
	///   <para>The bounding volume of the renderer (Read Only).</para>
	/// </summary>
	public Bounds bounds
	{
		[FreeFunction(Name = "RendererScripting::GetBounds", HasExplicitThis = true)]
		get
		{
			get_bounds_Injected(out var ret);
			return ret;
		}
	}

	/// <summary>
	///   <para>Makes the rendered 3D object visible if enabled.</para>
	/// </summary>
	public extern bool enabled
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Is this renderer visible in any camera? (Read Only)</para>
	/// </summary>
	public extern bool isVisible
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("IsVisibleInScene")]
		get;
	}

	/// <summary>
	///   <para>Does this object cast shadows?</para>
	/// </summary>
	public extern ShadowCastingMode shadowCastingMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Does this object receive shadows?</para>
	/// </summary>
	public extern bool receiveShadows
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Specifies the mode for motion vector rendering.</para>
	/// </summary>
	public extern MotionVectorGenerationMode motionVectorGenerationMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The light probe interpolation type.</para>
	/// </summary>
	public extern LightProbeUsage lightProbeUsage
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Should reflection probes be used for this Renderer?</para>
	/// </summary>
	public extern ReflectionProbeUsage reflectionProbeUsage
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Determines which rendering layer this renderer lives on.</para>
	/// </summary>
	public extern uint renderingLayerMask
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Name of the Renderer's sorting layer.</para>
	/// </summary>
	public extern string sortingLayerName
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Unique ID of the Renderer's sorting layer.</para>
	/// </summary>
	public extern int sortingLayerID
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Renderer's order within a sorting layer.</para>
	/// </summary>
	public extern int sortingOrder
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	internal extern int sortingGroupID
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	internal extern int sortingGroupOrder
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Controls if dynamic occlusion culling should be performed for this renderer.</para>
	/// </summary>
	[NativeProperty("IsDynamicOccludee")]
	public extern bool allowOcclusionWhenDynamic
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[NativeProperty("StaticBatchRoot")]
	internal extern Transform staticBatchRootTransform
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	internal extern int staticBatchIndex
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	/// <summary>
	///   <para>Has this renderer been statically batched with any other renderers?</para>
	/// </summary>
	public extern bool isPartOfStaticBatch
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("IsPartOfStaticBatch")]
		get;
	}

	/// <summary>
	///   <para>Matrix that transforms a point from world space into local space (Read Only).</para>
	/// </summary>
	public Matrix4x4 worldToLocalMatrix
	{
		get
		{
			get_worldToLocalMatrix_Injected(out var ret);
			return ret;
		}
	}

	/// <summary>
	///   <para>Matrix that transforms a point from local space into world space (Read Only).</para>
	/// </summary>
	public Matrix4x4 localToWorldMatrix
	{
		get
		{
			get_localToWorldMatrix_Injected(out var ret);
			return ret;
		}
	}

	/// <summary>
	///   <para>If set, the Renderer will use the Light Probe Proxy Volume component attached to the source GameObject.</para>
	/// </summary>
	public extern GameObject lightProbeProxyVolumeOverride
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>If set, Renderer will use this Transform's position to find the light or reflection probe.</para>
	/// </summary>
	public extern Transform probeAnchor
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The index of the baked lightmap applied to this renderer.</para>
	/// </summary>
	public int lightmapIndex
	{
		get
		{
			return GetLightmapIndex(LightmapType.StaticLightmap);
		}
		set
		{
			SetLightmapIndex(value, LightmapType.StaticLightmap);
		}
	}

	/// <summary>
	///   <para>The index of the realtime lightmap applied to this renderer.</para>
	/// </summary>
	public int realtimeLightmapIndex
	{
		get
		{
			return GetLightmapIndex(LightmapType.DynamicLightmap);
		}
		set
		{
			SetLightmapIndex(value, LightmapType.DynamicLightmap);
		}
	}

	/// <summary>
	///   <para>The UV scale &amp; offset used for a lightmap.</para>
	/// </summary>
	public Vector4 lightmapScaleOffset
	{
		get
		{
			return GetLightmapST(LightmapType.StaticLightmap);
		}
		set
		{
			SetStaticLightmapST(value);
		}
	}

	/// <summary>
	///   <para>The UV scale &amp; offset used for a realtime lightmap.</para>
	/// </summary>
	public Vector4 realtimeLightmapScaleOffset
	{
		get
		{
			return GetLightmapST(LightmapType.DynamicLightmap);
		}
		set
		{
			SetLightmapST(value, LightmapType.DynamicLightmap);
		}
	}

	/// <summary>
	///   <para>Returns all the instantiated materials of this object.</para>
	/// </summary>
	public Material[] materials
	{
		get
		{
			return GetMaterialArray();
		}
		set
		{
			SetMaterialArray(value);
		}
	}

	/// <summary>
	///   <para>Returns the first instantiated Material assigned to the renderer.</para>
	/// </summary>
	public Material material
	{
		get
		{
			return GetMaterial();
		}
		set
		{
			SetMaterial(value);
		}
	}

	/// <summary>
	///   <para>The shared material of this object.</para>
	/// </summary>
	public Material sharedMaterial
	{
		get
		{
			return GetSharedMaterial();
		}
		set
		{
			SetMaterial(value);
		}
	}

	/// <summary>
	///   <para>All the shared materials of this object.</para>
	/// </summary>
	public Material[] sharedMaterials
	{
		get
		{
			return GetSharedMaterialArray();
		}
		set
		{
			SetMaterialArray(value);
		}
	}

	[FreeFunction(Name = "RendererScripting::SetStaticLightmapST", HasExplicitThis = true)]
	private void SetStaticLightmapST(Vector4 st)
	{
		SetStaticLightmapST_Injected(ref st);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "RendererScripting::GetMaterial", HasExplicitThis = true)]
	private extern Material GetMaterial();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "RendererScripting::GetSharedMaterial", HasExplicitThis = true)]
	private extern Material GetSharedMaterial();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "RendererScripting::SetMaterial", HasExplicitThis = true)]
	private extern void SetMaterial(Material m);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "RendererScripting::GetMaterialArray", HasExplicitThis = true)]
	private extern Material[] GetMaterialArray();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "RendererScripting::GetSharedMaterialArray", HasExplicitThis = true)]
	private extern Material[] GetSharedMaterialArray();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "RendererScripting::SetMaterialArray", HasExplicitThis = true)]
	private extern void SetMaterialArray([NotNull] Material[] m);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "RendererScripting::SetPropertyBlock", HasExplicitThis = true)]
	internal extern void Internal_SetPropertyBlock(MaterialPropertyBlock properties);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "RendererScripting::GetPropertyBlock", HasExplicitThis = true)]
	internal extern void Internal_GetPropertyBlock([NotNull] MaterialPropertyBlock dest);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "RendererScripting::SetPropertyBlockMaterialIndex", HasExplicitThis = true)]
	internal extern void Internal_SetPropertyBlockMaterialIndex(MaterialPropertyBlock properties, int materialIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "RendererScripting::GetPropertyBlockMaterialIndex", HasExplicitThis = true)]
	internal extern void Internal_GetPropertyBlockMaterialIndex([NotNull] MaterialPropertyBlock dest, int materialIndex);

	/// <summary>
	///   <para>Returns true if the Renderer has a material property block attached via SetPropertyBlock.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "RendererScripting::HasPropertyBlock", HasExplicitThis = true)]
	public extern bool HasPropertyBlock();

	/// <summary>
	///   <para>Lets you set or clear per-renderer or per-material parameter overrides.</para>
	/// </summary>
	/// <param name="properties">Property block with values you want to override.</param>
	/// <param name="materialIndex">The index of the Material you want to override the parameters of. The index ranges from 0 to Renderer.sharedMaterial.Length-1.</param>
	public void SetPropertyBlock(MaterialPropertyBlock properties)
	{
		Internal_SetPropertyBlock(properties);
	}

	/// <summary>
	///   <para>Lets you set or clear per-renderer or per-material parameter overrides.</para>
	/// </summary>
	/// <param name="properties">Property block with values you want to override.</param>
	/// <param name="materialIndex">The index of the Material you want to override the parameters of. The index ranges from 0 to Renderer.sharedMaterial.Length-1.</param>
	public void SetPropertyBlock(MaterialPropertyBlock properties, int materialIndex)
	{
		Internal_SetPropertyBlockMaterialIndex(properties, materialIndex);
	}

	/// <summary>
	///   <para>Get per-Renderer or per-Material property block.</para>
	/// </summary>
	/// <param name="properties">Material parameters to retrieve.</param>
	/// <param name="materialIndex">The index of the Material you want to get overridden parameters from. The index ranges from 0 to Renderer.sharedMaterials.Length-1.</param>
	public void GetPropertyBlock(MaterialPropertyBlock properties)
	{
		Internal_GetPropertyBlock(properties);
	}

	/// <summary>
	///   <para>Get per-Renderer or per-Material property block.</para>
	/// </summary>
	/// <param name="properties">Material parameters to retrieve.</param>
	/// <param name="materialIndex">The index of the Material you want to get overridden parameters from. The index ranges from 0 to Renderer.sharedMaterials.Length-1.</param>
	public void GetPropertyBlock(MaterialPropertyBlock properties, int materialIndex)
	{
		Internal_GetPropertyBlockMaterialIndex(properties, materialIndex);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "RendererScripting::GetClosestReflectionProbes", HasExplicitThis = true)]
	private extern void GetClosestReflectionProbesInternal(object result);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern void SetStaticBatchInfo(int firstSubMesh, int subMeshCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetLightmapIndexInt")]
	private extern int GetLightmapIndex(LightmapType lt);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("SetLightmapIndexInt")]
	private extern void SetLightmapIndex(int index, LightmapType lt);

	[NativeName("GetLightmapST")]
	private Vector4 GetLightmapST(LightmapType lt)
	{
		GetLightmapST_Injected(lt, out var ret);
		return ret;
	}

	[NativeName("SetLightmapST")]
	private void SetLightmapST(Vector4 st, LightmapType lt)
	{
		SetLightmapST_Injected(ref st, lt);
	}

	public void GetClosestReflectionProbes(List<ReflectionProbeBlendInfo> result)
	{
		GetClosestReflectionProbesInternal(result);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_bounds_Injected(out Bounds ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetStaticLightmapST_Injected(ref Vector4 st);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_worldToLocalMatrix_Injected(out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_localToWorldMatrix_Injected(out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetLightmapST_Injected(LightmapType lt, out Vector4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetLightmapST_Injected(ref Vector4 st, LightmapType lt);
}

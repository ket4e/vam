using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Custom Render Textures are an extension to Render Textures, enabling you to render directly to the Texture using a Shader.</para>
/// </summary>
[UsedByNativeCode]
public sealed class CustomRenderTexture : RenderTexture
{
	/// <summary>
	///   <para>Material with which the content of the Custom Render Texture is updated.</para>
	/// </summary>
	public extern Material material
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Material with which the Custom Render Texture is initialized. Initialization texture and color are ignored if this parameter is set.</para>
	/// </summary>
	public extern Material initializationMaterial
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Texture with which the Custom Render Texture is initialized (multiplied by the initialization color). This parameter will be ignored if an initializationMaterial is set.</para>
	/// </summary>
	public extern Texture initializationTexture
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Specify if the texture should be initialized with a Texture and a Color or a Material.</para>
	/// </summary>
	public extern CustomRenderTextureInitializationSource initializationSource
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Color with which the Custom Render Texture is initialized. This parameter will be ignored if an initializationMaterial is set.</para>
	/// </summary>
	public Color initializationColor
	{
		get
		{
			INTERNAL_get_initializationColor(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_initializationColor(ref value);
		}
	}

	/// <summary>
	///   <para>Specify how the texture should be updated.</para>
	/// </summary>
	public extern CustomRenderTextureUpdateMode updateMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Specify how the texture should be initialized.</para>
	/// </summary>
	public extern CustomRenderTextureUpdateMode initializationMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Space in which the update zones are expressed (Normalized or Pixel space).</para>
	/// </summary>
	public extern CustomRenderTextureUpdateZoneSpace updateZoneSpace
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Shader Pass used to update the Custom Render Texture.</para>
	/// </summary>
	public extern int shaderPass
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Bitfield that allows to enable or disable update on each of the cubemap faces. Order from least significant bit is +X, -X, +Y, -Y, +Z, -Z.</para>
	/// </summary>
	public extern uint cubemapFaceMask
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>If true, the Custom Render Texture is double buffered so that you can access it during its own update. otherwise the Custom Render Texture will be not be double buffered.</para>
	/// </summary>
	public extern bool doubleBuffered
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>If true, Update zones will wrap around the border of the Custom Render Texture. Otherwise, Update zones will be clamped at the border of the Custom Render Texture.</para>
	/// </summary>
	public extern bool wrapUpdateZones
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Create a new Custom Render Texture.</para>
	/// </summary>
	/// <param name="width"></param>
	/// <param name="height"></param>
	/// <param name="format"></param>
	/// <param name="readWrite"></param>
	public CustomRenderTexture(int width, int height, RenderTextureFormat format, RenderTextureReadWrite readWrite)
	{
		Internal_CreateCustomRenderTexture(this, readWrite);
		this.width = width;
		this.height = height;
		base.format = format;
	}

	/// <summary>
	///   <para>Create a new Custom Render Texture.</para>
	/// </summary>
	/// <param name="width"></param>
	/// <param name="height"></param>
	/// <param name="format"></param>
	/// <param name="readWrite"></param>
	public CustomRenderTexture(int width, int height, RenderTextureFormat format)
	{
		Internal_CreateCustomRenderTexture(this, RenderTextureReadWrite.Default);
		this.width = width;
		this.height = height;
		base.format = format;
	}

	/// <summary>
	///   <para>Create a new Custom Render Texture.</para>
	/// </summary>
	/// <param name="width"></param>
	/// <param name="height"></param>
	/// <param name="format"></param>
	/// <param name="readWrite"></param>
	public CustomRenderTexture(int width, int height)
	{
		Internal_CreateCustomRenderTexture(this, RenderTextureReadWrite.Default);
		this.width = width;
		this.height = height;
		base.format = RenderTextureFormat.Default;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_CreateCustomRenderTexture([Writable] CustomRenderTexture rt, RenderTextureReadWrite readWrite);

	/// <summary>
	///   <para>Triggers the update of the Custom Render Texture.</para>
	/// </summary>
	/// <param name="count">Number of upate pass to perform.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void Update([DefaultValue("1")] int count);

	[ExcludeFromDocs]
	public void Update()
	{
		int count = 1;
		Update(count);
	}

	/// <summary>
	///   <para>Triggers an initialization of the Custom Render Texture.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void Initialize();

	/// <summary>
	///   <para>Clear all Update Zones.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void ClearUpdateZones();

	private bool IsCubemapFaceEnabled(CubemapFace face)
	{
		return (cubemapFaceMask & (1 << (int)face)) != 0;
	}

	private void EnableCubemapFace(CubemapFace face, bool value)
	{
		uint num = cubemapFaceMask;
		uint num2 = (uint)(1 << (int)face);
		num = ((!value) ? (num & ~num2) : (num | num2));
		cubemapFaceMask = num;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern void GetUpdateZonesInternal(object updateZones);

	public void GetUpdateZones(List<CustomRenderTextureUpdateZone> updateZones)
	{
		GetUpdateZonesInternal(updateZones);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void SetUpdateZonesInternal(CustomRenderTextureUpdateZone[] updateZones);

	/// <summary>
	///   <para>Setup the list of Update Zones for the Custom Render Texture.</para>
	/// </summary>
	/// <param name="updateZones"></param>
	public void SetUpdateZones(CustomRenderTextureUpdateZone[] updateZones)
	{
		if (updateZones == null)
		{
			throw new ArgumentNullException("updateZones");
		}
		SetUpdateZonesInternal(updateZones);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_initializationColor(out Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_initializationColor(ref Color value);
}

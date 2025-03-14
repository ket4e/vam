using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Class for handling 3D Textures, Use this to create.</para>
/// </summary>
[ExcludeFromPreset]
[NativeHeader("Runtime/Graphics/Texture3D.h")]
public sealed class Texture3D : Texture
{
	/// <summary>
	///   <para>The depth of the texture (Read Only).</para>
	/// </summary>
	public extern int depth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetTextureLayerCount")]
		get;
	}

	/// <summary>
	///   <para>The format of the pixel data in the texture (Read Only).</para>
	/// </summary>
	public extern TextureFormat format
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetTextureFormat")]
		get;
	}

	/// <summary>
	///   <para>Create a new empty 3D Texture.</para>
	/// </summary>
	/// <param name="width">Width of texture in pixels.</param>
	/// <param name="height">Height of texture in pixels.</param>
	/// <param name="depth">Depth of texture in pixels.</param>
	/// <param name="format">Texture data format.</param>
	/// <param name="mipmap">Should the texture have mipmaps?</param>
	public Texture3D(int width, int height, int depth, TextureFormat format, bool mipmap)
	{
		Internal_Create(this, width, height, depth, format, mipmap);
	}

	/// <summary>
	///   <para>Returns an array of pixel colors representing one mip level of the 3D texture.</para>
	/// </summary>
	/// <param name="miplevel"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern Color[] GetPixels([DefaultValue("0")] int miplevel);

	[ExcludeFromDocs]
	public Color[] GetPixels()
	{
		int miplevel = 0;
		return GetPixels(miplevel);
	}

	/// <summary>
	///   <para>Returns an array of pixel colors representing one mip level of the 3D texture.</para>
	/// </summary>
	/// <param name="miplevel"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern Color32[] GetPixels32([DefaultValue("0")] int miplevel);

	[ExcludeFromDocs]
	public Color32[] GetPixels32()
	{
		int miplevel = 0;
		return GetPixels32(miplevel);
	}

	/// <summary>
	///   <para>Sets pixel colors of a 3D texture.</para>
	/// </summary>
	/// <param name="colors">The colors to set the pixels to.</param>
	/// <param name="miplevel">The mipmap level to be affected by the new colors.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetPixels(Color[] colors, [DefaultValue("0")] int miplevel);

	[ExcludeFromDocs]
	public void SetPixels(Color[] colors)
	{
		int miplevel = 0;
		SetPixels(colors, miplevel);
	}

	/// <summary>
	///   <para>Sets pixel colors of a 3D texture.</para>
	/// </summary>
	/// <param name="colors">The colors to set the pixels to.</param>
	/// <param name="miplevel">The mipmap level to be affected by the new colors.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetPixels32(Color32[] colors, [DefaultValue("0")] int miplevel);

	[ExcludeFromDocs]
	public void SetPixels32(Color32[] colors)
	{
		int miplevel = 0;
		SetPixels32(colors, miplevel);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetIsReadable")]
	private extern bool IsReadable();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Texture3DScripting::Create")]
	private static extern bool Internal_CreateImpl([Writable] Texture3D mono, int w, int h, int d, TextureFormat format, bool mipmap);

	private static void Internal_Create([Writable] Texture3D mono, int w, int h, int d, TextureFormat format, bool mipmap)
	{
		if (!Internal_CreateImpl(mono, w, h, d, format, mipmap))
		{
			throw new UnityException("Failed to create texture because of invalid parameters.");
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "Texture3DScripting::Apply", HasExplicitThis = true)]
	private extern void ApplyImpl(bool updateMipmaps, bool makeNoLongerReadable);

	/// <summary>
	///   <para>Actually apply all previous SetPixels changes.</para>
	/// </summary>
	/// <param name="updateMipmaps">When set to true, mipmap levels are recalculated.</param>
	/// <param name="makeNoLongerReadable">When set to true, system memory copy of a texture is released.</param>
	public void Apply([DefaultValue("true")] bool updateMipmaps, [DefaultValue("false")] bool makeNoLongerReadable)
	{
		if (!IsReadable())
		{
			throw CreateNonReadableException(this);
		}
		ApplyImpl(updateMipmaps, makeNoLongerReadable);
	}

	public void Apply(bool updateMipmaps)
	{
		Apply(updateMipmaps, makeNoLongerReadable: false);
	}

	public void Apply()
	{
		Apply(updateMipmaps: true, makeNoLongerReadable: false);
	}
}

using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Class for handling 2D texture arrays.</para>
/// </summary>
[NativeHeader("Runtime/Graphics/Texture2DArray.h")]
public sealed class Texture2DArray : Texture
{
	/// <summary>
	///   <para>Number of elements in a texture array (Read Only).</para>
	/// </summary>
	public extern int depth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetTextureLayerCount")]
		get;
	}

	/// <summary>
	///   <para>Texture format (Read Only).</para>
	/// </summary>
	public extern TextureFormat format
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetTextureFormat")]
		get;
	}

	/// <summary>
	///   <para>Create a new texture array.</para>
	/// </summary>
	/// <param name="width">Width of texture array in pixels.</param>
	/// <param name="height">Height of texture array in pixels.</param>
	/// <param name="depth">Number of elements in the texture array.</param>
	/// <param name="format">Format of the texture.</param>
	/// <param name="mipmap">Should mipmaps be created?</param>
	/// <param name="linear">Does the texture contain non-color data (i.e. don't do any color space conversions when sampling)? Default is false.</param>
	public Texture2DArray(int width, int height, int depth, TextureFormat format, bool mipmap, [DefaultValue("false")] bool linear)
	{
		Internal_Create(this, width, height, depth, format, mipmap, linear);
	}

	/// <summary>
	///   <para>Create a new texture array.</para>
	/// </summary>
	/// <param name="width">Width of texture array in pixels.</param>
	/// <param name="height">Height of texture array in pixels.</param>
	/// <param name="depth">Number of elements in the texture array.</param>
	/// <param name="format">Format of the texture.</param>
	/// <param name="mipmap">Should mipmaps be created?</param>
	/// <param name="linear">Does the texture contain non-color data (i.e. don't do any color space conversions when sampling)? Default is false.</param>
	public Texture2DArray(int width, int height, int depth, TextureFormat format, bool mipmap)
		: this(width, height, depth, format, mipmap, linear: false)
	{
	}

	/// <summary>
	///   <para>Set pixel colors for the whole mip level.</para>
	/// </summary>
	/// <param name="colors">An array of pixel colors.</param>
	/// <param name="arrayElement">The texture array element index.</param>
	/// <param name="miplevel">The mip level.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetPixels(Color[] colors, int arrayElement, [DefaultValue("0")] int miplevel);

	[ExcludeFromDocs]
	public void SetPixels(Color[] colors, int arrayElement)
	{
		int miplevel = 0;
		SetPixels(colors, arrayElement, miplevel);
	}

	/// <summary>
	///   <para>Set pixel colors for the whole mip level.</para>
	/// </summary>
	/// <param name="colors">An array of pixel colors.</param>
	/// <param name="arrayElement">The texture array element index.</param>
	/// <param name="miplevel">The mip level.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetPixels32(Color32[] colors, int arrayElement, [DefaultValue("0")] int miplevel);

	[ExcludeFromDocs]
	public void SetPixels32(Color32[] colors, int arrayElement)
	{
		int miplevel = 0;
		SetPixels32(colors, arrayElement, miplevel);
	}

	/// <summary>
	///   <para>Returns pixel colors of a single array slice.</para>
	/// </summary>
	/// <param name="arrayElement">Array slice to read pixels from.</param>
	/// <param name="miplevel">Mipmap level to read pixels from.</param>
	/// <returns>
	///   <para>Array of pixel colors.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern Color[] GetPixels(int arrayElement, [DefaultValue("0")] int miplevel);

	[ExcludeFromDocs]
	public Color[] GetPixels(int arrayElement)
	{
		int miplevel = 0;
		return GetPixels(arrayElement, miplevel);
	}

	/// <summary>
	///   <para>Returns pixel colors of a single array slice.</para>
	/// </summary>
	/// <param name="arrayElement">Array slice to read pixels from.</param>
	/// <param name="miplevel">Mipmap level to read pixels from.</param>
	/// <returns>
	///   <para>Array of pixel colors in low precision (8 bits/channel) format.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern Color32[] GetPixels32(int arrayElement, [DefaultValue("0")] int miplevel);

	[ExcludeFromDocs]
	public Color32[] GetPixels32(int arrayElement)
	{
		int miplevel = 0;
		return GetPixels32(arrayElement, miplevel);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetIsReadable")]
	private extern bool IsReadable();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Texture2DArrayScripting::Create")]
	private static extern bool Internal_CreateImpl([Writable] Texture2DArray mono, int w, int h, int d, TextureFormat format, bool mipmap, bool linear);

	private static void Internal_Create([Writable] Texture2DArray mono, int w, int h, int d, TextureFormat format, bool mipmap, bool linear)
	{
		if (!Internal_CreateImpl(mono, w, h, d, format, mipmap, linear))
		{
			throw new UnityException("Failed to create 2D array texture because of invalid parameters.");
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "Texture2DArrayScripting::Apply", HasExplicitThis = true)]
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

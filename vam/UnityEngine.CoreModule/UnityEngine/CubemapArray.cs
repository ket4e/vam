using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Class for handling Cubemap arrays.</para>
/// </summary>
[NativeHeader("Runtime/Graphics/CubemapArrayTexture.h")]
public sealed class CubemapArray : Texture
{
	/// <summary>
	///   <para>Number of cubemaps in the array (Read Only).</para>
	/// </summary>
	public extern int cubemapCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
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
	///   <para>Create a new cubemap array.</para>
	/// </summary>
	/// <param name="faceSize">Cubemap face size in pixels.</param>
	/// <param name="cubemapCount">Number of elements in the cubemap array.</param>
	/// <param name="format">Format of the pixel data.</param>
	/// <param name="mipmap">Should mipmaps be created?</param>
	/// <param name="linear">Does the texture contain non-color data (i.e. don't do any color space conversions when sampling)? Default is false.</param>
	public CubemapArray(int faceSize, int cubemapCount, TextureFormat format, bool mipmap, [DefaultValue("false")] bool linear)
	{
		Internal_Create(this, faceSize, cubemapCount, format, mipmap, linear);
	}

	/// <summary>
	///   <para>Create a new cubemap array.</para>
	/// </summary>
	/// <param name="faceSize">Cubemap face size in pixels.</param>
	/// <param name="cubemapCount">Number of elements in the cubemap array.</param>
	/// <param name="format">Format of the pixel data.</param>
	/// <param name="mipmap">Should mipmaps be created?</param>
	/// <param name="linear">Does the texture contain non-color data (i.e. don't do any color space conversions when sampling)? Default is false.</param>
	public CubemapArray(int faceSize, int cubemapCount, TextureFormat format, bool mipmap)
		: this(faceSize, cubemapCount, format, mipmap, linear: false)
	{
	}

	/// <summary>
	///   <para>Set pixel colors for a single array slice/face.</para>
	/// </summary>
	/// <param name="colors">An array of pixel colors.</param>
	/// <param name="face">Cubemap face to set pixels for.</param>
	/// <param name="arrayElement">Array element index to set pixels for.</param>
	/// <param name="miplevel">Mipmap level to set pixels for.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetPixels(Color[] colors, CubemapFace face, int arrayElement, [DefaultValue("0")] int miplevel);

	[ExcludeFromDocs]
	public void SetPixels(Color[] colors, CubemapFace face, int arrayElement)
	{
		int miplevel = 0;
		SetPixels(colors, face, arrayElement, miplevel);
	}

	/// <summary>
	///   <para>Set pixel colors for a single array slice/face.</para>
	/// </summary>
	/// <param name="colors">An array of pixel colors in low precision (8 bits/channel) format.</param>
	/// <param name="face">Cubemap face to set pixels for.</param>
	/// <param name="arrayElement">Array element index to set pixels for.</param>
	/// <param name="miplevel">Mipmap level to set pixels for.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetPixels32(Color32[] colors, CubemapFace face, int arrayElement, [DefaultValue("0")] int miplevel);

	[ExcludeFromDocs]
	public void SetPixels32(Color32[] colors, CubemapFace face, int arrayElement)
	{
		int miplevel = 0;
		SetPixels32(colors, face, arrayElement, miplevel);
	}

	/// <summary>
	///   <para>Returns pixel colors of a single array slice/face.</para>
	/// </summary>
	/// <param name="face">Cubemap face to read pixels from.</param>
	/// <param name="arrayElement">Array slice to read pixels from.</param>
	/// <param name="miplevel">Mipmap level to read pixels from.</param>
	/// <returns>
	///   <para>Array of pixel colors.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern Color[] GetPixels(CubemapFace face, int arrayElement, [DefaultValue("0")] int miplevel);

	[ExcludeFromDocs]
	public Color[] GetPixels(CubemapFace face, int arrayElement)
	{
		int miplevel = 0;
		return GetPixels(face, arrayElement, miplevel);
	}

	/// <summary>
	///   <para>Returns pixel colors of a single array slice/face.</para>
	/// </summary>
	/// <param name="face">Cubemap face to read pixels from.</param>
	/// <param name="arrayElement">Array slice to read pixels from.</param>
	/// <param name="miplevel">Mipmap level to read pixels from.</param>
	/// <returns>
	///   <para>Array of pixel colors in low precision (8 bits/channel) format.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern Color32[] GetPixels32(CubemapFace face, int arrayElement, [DefaultValue("0")] int miplevel);

	[ExcludeFromDocs]
	public Color32[] GetPixels32(CubemapFace face, int arrayElement)
	{
		int miplevel = 0;
		return GetPixels32(face, arrayElement, miplevel);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetIsReadable")]
	private extern bool IsReadable();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("CubemapArrayScripting::Create")]
	private static extern bool Internal_CreateImpl([Writable] CubemapArray mono, int ext, int count, TextureFormat format, bool mipmap, bool linear);

	private static void Internal_Create([Writable] CubemapArray mono, int ext, int count, TextureFormat format, bool mipmap, bool linear)
	{
		if (!Internal_CreateImpl(mono, ext, count, format, mipmap, linear))
		{
			throw new UnityException("Failed to create cubemap array texture because of invalid parameters.");
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "CubemapArrayScripting::Apply", HasExplicitThis = true)]
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

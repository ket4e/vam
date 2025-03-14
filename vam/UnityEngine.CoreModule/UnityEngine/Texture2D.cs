using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Class for texture handling.</para>
/// </summary>
[UsedByNativeCode]
[NativeHeader("Runtime/Graphics/Texture2D.h")]
[NativeHeader("Runtime/Graphics/GeneratedTextures.h")]
public sealed class Texture2D : Texture
{
	/// <summary>
	///   <para>Flags used to control the encoding to an EXR file.</para>
	/// </summary>
	[Flags]
	public enum EXRFlags
	{
		/// <summary>
		///   <para>No flag. This will result in an uncompressed 16-bit float EXR file.</para>
		/// </summary>
		None = 0,
		/// <summary>
		///   <para>The texture will be exported as a 32-bit float EXR file (default is 16-bit).</para>
		/// </summary>
		OutputAsFloat = 1,
		/// <summary>
		///   <para>The texture will use the EXR ZIP compression format.</para>
		/// </summary>
		CompressZIP = 2,
		/// <summary>
		///   <para>The texture will use RLE (Run Length Encoding) EXR compression format (similar to Targa RLE compression).</para>
		/// </summary>
		CompressRLE = 4,
		/// <summary>
		///   <para>This texture will use Wavelet compression. This is best used for grainy images.</para>
		/// </summary>
		CompressPIZ = 8
	}

	/// <summary>
	///   <para>How many mipmap levels are in this texture (Read Only).</para>
	/// </summary>
	public extern int mipmapCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("CountDataMipmaps")]
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
	///   <para>Get a small texture with all white pixels.</para>
	/// </summary>
	[StaticAccessor("builtintex", StaticAccessorType.DoubleColon)]
	public static extern Texture2D whiteTexture
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	/// <summary>
	///   <para>Get a small texture with all black pixels.</para>
	/// </summary>
	[StaticAccessor("builtintex", StaticAccessorType.DoubleColon)]
	public static extern Texture2D blackTexture
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	internal Texture2D(int width, int height, TextureFormat format, bool mipmap, bool linear, IntPtr nativeTex)
	{
		Internal_Create(this, width, height, format, mipmap, linear, nativeTex);
	}

	/// <summary>
	///   <para>Create a new empty texture.</para>
	/// </summary>
	/// <param name="width"></param>
	/// <param name="height"></param>
	/// <param name="format"></param>
	/// <param name="mipmap"></param>
	/// <param name="linear"></param>
	public Texture2D(int width, int height, [DefaultValue("TextureFormat.RGBA32")] TextureFormat format, [DefaultValue("true")] bool mipmap, [DefaultValue("false")] bool linear)
		: this(width, height, format, mipmap, linear, IntPtr.Zero)
	{
	}

	/// <summary>
	///   <para>Create a new empty texture.</para>
	/// </summary>
	/// <param name="width"></param>
	/// <param name="height"></param>
	/// <param name="format"></param>
	/// <param name="mipmap"></param>
	public Texture2D(int width, int height, TextureFormat format, bool mipmap)
		: this(width, height, format, mipmap, linear: false, IntPtr.Zero)
	{
	}

	/// <summary>
	///   <para>Create a new empty texture.</para>
	/// </summary>
	/// <param name="width"></param>
	/// <param name="height"></param>
	public Texture2D(int width, int height)
		: this(width, height, TextureFormat.RGBA32, mipmap: true, linear: false, IntPtr.Zero)
	{
	}

	/// <summary>
	///   <para>Updates Unity texture to use different native texture object.</para>
	/// </summary>
	/// <param name="nativeTex">Native 2D texture object.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void UpdateExternalTexture(IntPtr nativeTex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void SetAllPixels32(Color32[] colors, int miplevel);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void SetBlockOfPixels32(int x, int y, int blockWidth, int blockHeight, Color32[] colors, int miplevel);

	[ExcludeFromDocs]
	public void SetPixels32(Color32[] colors)
	{
		int miplevel = 0;
		SetPixels32(colors, miplevel);
	}

	/// <summary>
	///   <para>Set a block of pixel colors.</para>
	/// </summary>
	/// <param name="colors"></param>
	/// <param name="miplevel"></param>
	public void SetPixels32(Color32[] colors, [DefaultValue("0")] int miplevel)
	{
		SetAllPixels32(colors, miplevel);
	}

	[ExcludeFromDocs]
	public void SetPixels32(int x, int y, int blockWidth, int blockHeight, Color32[] colors)
	{
		int miplevel = 0;
		SetPixels32(x, y, blockWidth, blockHeight, colors, miplevel);
	}

	/// <summary>
	///   <para>Set a block of pixel colors.</para>
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="blockWidth"></param>
	/// <param name="blockHeight"></param>
	/// <param name="colors"></param>
	/// <param name="miplevel"></param>
	public void SetPixels32(int x, int y, int blockWidth, int blockHeight, Color32[] colors, [DefaultValue("0")] int miplevel)
	{
		SetBlockOfPixels32(x, y, blockWidth, blockHeight, colors, miplevel);
	}

	/// <summary>
	///   <para>Get raw data from a texture.</para>
	/// </summary>
	/// <returns>
	///   <para>Raw texture data as a byte array.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern byte[] GetRawTextureData();

	[ExcludeFromDocs]
	public Color[] GetPixels()
	{
		int miplevel = 0;
		return GetPixels(miplevel);
	}

	/// <summary>
	///   <para>Get the pixel colors from the texture.</para>
	/// </summary>
	/// <param name="miplevel">The mipmap level to fetch the pixels from. Defaults to zero.</param>
	/// <returns>
	///   <para>The array of all pixels in the mipmap level of the texture.</para>
	/// </returns>
	public Color[] GetPixels([DefaultValue("0")] int miplevel)
	{
		int num = width >> miplevel;
		if (num < 1)
		{
			num = 1;
		}
		int num2 = height >> miplevel;
		if (num2 < 1)
		{
			num2 = 1;
		}
		return GetPixels(0, 0, num, num2, miplevel);
	}

	/// <summary>
	///   <para>Get a block of pixel colors.</para>
	/// </summary>
	/// <param name="x">The x position of the pixel array to fetch.</param>
	/// <param name="y">The y position of the pixel array to fetch.</param>
	/// <param name="blockWidth">The width length of the pixel array to fetch.</param>
	/// <param name="blockHeight">The height length of the pixel array to fetch.</param>
	/// <param name="miplevel">The mipmap level to fetch the pixels. Defaults to zero, and is
	///   optional.</param>
	/// <returns>
	///   <para>The array of pixels in the texture that have been selected.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern Color[] GetPixels(int x, int y, int blockWidth, int blockHeight, [DefaultValue("0")] int miplevel);

	[ExcludeFromDocs]
	public Color[] GetPixels(int x, int y, int blockWidth, int blockHeight)
	{
		int miplevel = 0;
		return GetPixels(x, y, blockWidth, blockHeight, miplevel);
	}

	/// <summary>
	///   <para>Get a block of pixel colors in Color32 format.</para>
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
	///   <para>Resizes the texture.</para>
	/// </summary>
	/// <param name="width"></param>
	/// <param name="height"></param>
	/// <param name="format"></param>
	/// <param name="hasMipMap"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern bool Resize(int width, int height, TextureFormat format, bool hasMipMap);

	/// <summary>
	///   <para>Packs multiple Textures into a texture atlas.</para>
	/// </summary>
	/// <param name="textures">Array of textures to pack into the atlas.</param>
	/// <param name="padding">Padding in pixels between the packed textures.</param>
	/// <param name="maximumAtlasSize">Maximum size of the resulting texture.</param>
	/// <param name="makeNoLongerReadable">Should the texture be marked as no longer readable?</param>
	/// <returns>
	///   <para>An array of rectangles containing the UV coordinates in the atlas for each input texture, or null if packing fails.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern Rect[] PackTextures(Texture2D[] textures, int padding, [DefaultValue("2048")] int maximumAtlasSize, [DefaultValue("false")] bool makeNoLongerReadable);

	[ExcludeFromDocs]
	public Rect[] PackTextures(Texture2D[] textures, int padding, int maximumAtlasSize)
	{
		bool makeNoLongerReadable = false;
		return PackTextures(textures, padding, maximumAtlasSize, makeNoLongerReadable);
	}

	[ExcludeFromDocs]
	public Rect[] PackTextures(Texture2D[] textures, int padding)
	{
		bool makeNoLongerReadable = false;
		int maximumAtlasSize = 2048;
		return PackTextures(textures, padding, maximumAtlasSize, makeNoLongerReadable);
	}

	public static bool GenerateAtlas(Vector2[] sizes, int padding, int atlasSize, List<Rect> results)
	{
		if (sizes == null)
		{
			throw new ArgumentException("sizes array can not be null");
		}
		if (results == null)
		{
			throw new ArgumentException("results list cannot be null");
		}
		if (padding < 0)
		{
			throw new ArgumentException("padding can not be negative");
		}
		if (atlasSize <= 0)
		{
			throw new ArgumentException("atlas size must be positive");
		}
		results.Clear();
		if (sizes.Length == 0)
		{
			return true;
		}
		GenerateAtlasInternal(sizes, padding, atlasSize, results);
		return results.Count != 0;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void GenerateAtlasInternal(Vector2[] sizes, int padding, int atlasSize, object resultList);

	/// <summary>
	///   <para>Read pixels from screen into the saved texture data.</para>
	/// </summary>
	/// <param name="source">Rectangular region of the view to read from. Pixels are read from current render target.</param>
	/// <param name="destX">Horizontal pixel position in the texture to place the pixels that are read.</param>
	/// <param name="destY">Vertical pixel position in the texture to place the pixels that are read.</param>
	/// <param name="recalculateMipMaps">Should the texture's mipmaps be recalculated after reading?</param>
	public void ReadPixels(Rect source, int destX, int destY, [DefaultValue("true")] bool recalculateMipMaps)
	{
		INTERNAL_CALL_ReadPixels(this, ref source, destX, destY, recalculateMipMaps);
	}

	[ExcludeFromDocs]
	public void ReadPixels(Rect source, int destX, int destY)
	{
		bool recalculateMipMaps = true;
		INTERNAL_CALL_ReadPixels(this, ref source, destX, destY, recalculateMipMaps);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_ReadPixels(Texture2D self, ref Rect source, int destX, int destY, bool recalculateMipMaps);

	/// <summary>
	///   <para>Compress texture into DXT format.</para>
	/// </summary>
	/// <param name="highQuality"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void Compress(bool highQuality);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Texture2DScripting::Create")]
	private static extern bool Internal_CreateImpl([Writable] Texture2D mono, int w, int h, TextureFormat format, bool mipmap, bool linear, IntPtr nativeTex);

	private static void Internal_Create([Writable] Texture2D mono, int w, int h, TextureFormat format, bool mipmap, bool linear, IntPtr nativeTex)
	{
		if (!Internal_CreateImpl(mono, w, h, format, mipmap, linear, nativeTex))
		{
			throw new UnityException("Failed to create texture because of invalid parameters.");
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetIsReadable")]
	private extern bool IsReadable();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("Apply")]
	private extern void ApplyImpl(bool updateMipmaps, bool makeNoLongerReadable);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("Resize")]
	private extern bool ResizeImpl(int width, int height);

	[NativeName("SetPixel")]
	private void SetPixelImpl(int image, int x, int y, Color color)
	{
		SetPixelImpl_Injected(image, x, y, ref color);
	}

	[NativeName("GetPixel")]
	private Color GetPixelImpl(int image, int x, int y)
	{
		GetPixelImpl_Injected(image, x, y, out var ret);
		return ret;
	}

	[NativeName("GetPixelBilinear")]
	private Color GetPixelBilinearImpl(int image, float x, float y)
	{
		GetPixelBilinearImpl_Injected(image, x, y, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "Texture2DScripting::SetPixels", HasExplicitThis = true)]
	private extern void SetPixelsImpl(int x, int y, int w, int h, Color[] pixel, int miplevel, int frame);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "Texture2DScripting::LoadRawData", HasExplicitThis = true)]
	private extern bool LoadRawTextureDataImpl(IntPtr data, int size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "Texture2DScripting::LoadRawData", HasExplicitThis = true)]
	private extern bool LoadRawTextureDataImplArray(byte[] data);

	/// <summary>
	///   <para>Creates Unity Texture out of externally created native texture object.</para>
	/// </summary>
	/// <param name="nativeTex">Native 2D texture object.</param>
	/// <param name="width">Width of texture in pixels.</param>
	/// <param name="height">Height of texture in pixels.</param>
	/// <param name="format">Format of underlying texture object.</param>
	/// <param name="mipmap">Does the texture have mipmaps?</param>
	/// <param name="linear">Is texture using linear color space?</param>
	public static Texture2D CreateExternalTexture(int width, int height, TextureFormat format, bool mipmap, bool linear, IntPtr nativeTex)
	{
		if (nativeTex == IntPtr.Zero)
		{
			throw new ArgumentException("nativeTex can not be null");
		}
		return new Texture2D(width, height, format, mipmap, linear, nativeTex);
	}

	/// <summary>
	///   <para>Sets pixel color at coordinates (x,y).</para>
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="color"></param>
	public void SetPixel(int x, int y, Color color)
	{
		if (!IsReadable())
		{
			throw CreateNonReadableException(this);
		}
		SetPixelImpl(0, x, y, color);
	}

	/// <summary>
	///   <para>Set a block of pixel colors.</para>
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="blockWidth"></param>
	/// <param name="blockHeight"></param>
	/// <param name="colors"></param>
	/// <param name="miplevel"></param>
	public void SetPixels(int x, int y, int blockWidth, int blockHeight, Color[] colors, [DefaultValue("0")] int miplevel)
	{
		if (!IsReadable())
		{
			throw CreateNonReadableException(this);
		}
		SetPixelsImpl(x, y, blockWidth, blockHeight, colors, miplevel, 0);
	}

	public void SetPixels(int x, int y, int blockWidth, int blockHeight, Color[] colors)
	{
		SetPixels(x, y, blockWidth, blockHeight, colors, 0);
	}

	/// <summary>
	///   <para>Set a block of pixel colors.</para>
	/// </summary>
	/// <param name="colors">The array of pixel colours to assign (a 2D image flattened to a 1D array).</param>
	/// <param name="miplevel">The mip level of the texture to write to.</param>
	public void SetPixels(Color[] colors, [DefaultValue("0")] int miplevel)
	{
		int num = width >> miplevel;
		if (num < 1)
		{
			num = 1;
		}
		int num2 = height >> miplevel;
		if (num2 < 1)
		{
			num2 = 1;
		}
		SetPixels(0, 0, num, num2, colors, miplevel);
	}

	public void SetPixels(Color[] colors)
	{
		SetPixels(0, 0, width, height, colors, 0);
	}

	/// <summary>
	///   <para>Returns pixel color at coordinates (x, y).</para>
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public Color GetPixel(int x, int y)
	{
		if (!IsReadable())
		{
			throw CreateNonReadableException(this);
		}
		return GetPixelImpl(0, x, y);
	}

	/// <summary>
	///   <para>Returns filtered pixel color at normalized coordinates (u, v).</para>
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public Color GetPixelBilinear(float x, float y)
	{
		if (!IsReadable())
		{
			throw CreateNonReadableException(this);
		}
		return GetPixelBilinearImpl(0, x, y);
	}

	/// <summary>
	///   <para>Fills texture pixels with raw preformatted data.</para>
	/// </summary>
	/// <param name="data">Byte array to initialize texture pixels with.</param>
	/// <param name="size">Size of data in bytes.</param>
	public void LoadRawTextureData(IntPtr data, int size)
	{
		if (!IsReadable())
		{
			throw CreateNonReadableException(this);
		}
		if (data == IntPtr.Zero || size == 0)
		{
			Debug.LogError("No texture data provided to LoadRawTextureData", this);
		}
		else if (!LoadRawTextureDataImpl(data, size))
		{
			throw new UnityException("LoadRawTextureData: not enough data provided (will result in overread).");
		}
	}

	/// <summary>
	///   <para>Fills texture pixels with raw preformatted data.</para>
	/// </summary>
	/// <param name="data">Byte array to initialize texture pixels with.</param>
	/// <param name="size">Size of data in bytes.</param>
	public void LoadRawTextureData(byte[] data)
	{
		if (!IsReadable())
		{
			throw CreateNonReadableException(this);
		}
		if (data == null || data.Length == 0)
		{
			Debug.LogError("No texture data provided to LoadRawTextureData", this);
		}
		else if (!LoadRawTextureDataImplArray(data))
		{
			throw new UnityException("LoadRawTextureData: not enough data provided (will result in overread).");
		}
	}

	/// <summary>
	///   <para>Actually apply all previous SetPixel and SetPixels changes.</para>
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

	/// <summary>
	///   <para>Resizes the texture.</para>
	/// </summary>
	/// <param name="width"></param>
	/// <param name="height"></param>
	public bool Resize(int width, int height)
	{
		if (!IsReadable())
		{
			throw CreateNonReadableException(this);
		}
		return ResizeImpl(width, height);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetPixelImpl_Injected(int image, int x, int y, ref Color color);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetPixelImpl_Injected(int image, int x, int y, out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetPixelBilinearImpl_Injected(int image, float x, float y, out Color ret);
}

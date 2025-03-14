using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Class for handling Sparse Textures.</para>
/// </summary>
public sealed class SparseTexture : Texture
{
	/// <summary>
	///   <para>Get sparse texture tile width (Read Only).</para>
	/// </summary>
	public extern int tileWidth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Get sparse texture tile height (Read Only).</para>
	/// </summary>
	public extern int tileHeight
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Is the sparse texture actually created? (Read Only)</para>
	/// </summary>
	public extern bool isCreated
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Create a sparse texture.</para>
	/// </summary>
	/// <param name="width">Texture width in pixels.</param>
	/// <param name="height">Texture height in pixels.</param>
	/// <param name="format">Texture format.</param>
	/// <param name="mipCount">Mipmap count. Pass -1 to create full mipmap chain.</param>
	/// <param name="linear">Whether texture data will be in linear or sRGB color space (default is sRGB).</param>
	public SparseTexture(int width, int height, TextureFormat format, int mipCount)
	{
		Internal_Create(this, width, height, format, mipCount, linear: false);
	}

	/// <summary>
	///   <para>Create a sparse texture.</para>
	/// </summary>
	/// <param name="width">Texture width in pixels.</param>
	/// <param name="height">Texture height in pixels.</param>
	/// <param name="format">Texture format.</param>
	/// <param name="mipCount">Mipmap count. Pass -1 to create full mipmap chain.</param>
	/// <param name="linear">Whether texture data will be in linear or sRGB color space (default is sRGB).</param>
	public SparseTexture(int width, int height, TextureFormat format, int mipCount, bool linear)
	{
		Internal_Create(this, width, height, format, mipCount, linear);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_Create([Writable] SparseTexture mono, int width, int height, TextureFormat format, int mipCount, bool linear);

	/// <summary>
	///   <para>Update sparse texture tile with color values.</para>
	/// </summary>
	/// <param name="tileX">Tile X coordinate.</param>
	/// <param name="tileY">Tile Y coordinate.</param>
	/// <param name="miplevel">Mipmap level of the texture.</param>
	/// <param name="data">Tile color data.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void UpdateTile(int tileX, int tileY, int miplevel, Color32[] data);

	/// <summary>
	///   <para>Update sparse texture tile with raw pixel values.</para>
	/// </summary>
	/// <param name="tileX">Tile X coordinate.</param>
	/// <param name="tileY">Tile Y coordinate.</param>
	/// <param name="miplevel">Mipmap level of the texture.</param>
	/// <param name="data">Tile raw pixel data.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void UpdateTileRaw(int tileX, int tileY, int miplevel, byte[] data);

	/// <summary>
	///   <para>Unload sparse texture tile.</para>
	/// </summary>
	/// <param name="tileX">Tile X coordinate.</param>
	/// <param name="tileY">Tile Y coordinate.</param>
	/// <param name="miplevel">Mipmap level of the texture.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void UnloadTile(int tileX, int tileY, int miplevel);
}

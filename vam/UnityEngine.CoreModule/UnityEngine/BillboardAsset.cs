using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>BillboardAsset describes how a billboard is rendered.</para>
/// </summary>
public sealed class BillboardAsset : Object
{
	/// <summary>
	///   <para>Width of the billboard.</para>
	/// </summary>
	public extern float width
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Height of the billboard.</para>
	/// </summary>
	public extern float height
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Height of the billboard that is below ground.</para>
	/// </summary>
	public extern float bottom
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Number of pre-rendered images that can be switched when the billboard is viewed from different angles.</para>
	/// </summary>
	public extern int imageCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Number of vertices in the billboard mesh.</para>
	/// </summary>
	public extern int vertexCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Number of indices in the billboard mesh.</para>
	/// </summary>
	public extern int indexCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>The material used for rendering.</para>
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
	///   <para>Constructs a new BillboardAsset.</para>
	/// </summary>
	public BillboardAsset()
	{
		Internal_Create(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_Create([Writable] BillboardAsset obj);

	public void GetImageTexCoords(List<Vector4> imageTexCoords)
	{
		if (imageTexCoords == null)
		{
			throw new ArgumentNullException("imageTexCoords");
		}
		GetImageTexCoordsInternal(imageTexCoords);
	}

	/// <summary>
	///   <para>Get the array of billboard image texture coordinate data.</para>
	/// </summary>
	/// <param name="imageTexCoords">The list that receives the array.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern Vector4[] GetImageTexCoords();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern void GetImageTexCoordsInternal(object list);

	public void SetImageTexCoords(List<Vector4> imageTexCoords)
	{
		if (imageTexCoords == null)
		{
			throw new ArgumentNullException("imageTexCoords");
		}
		SetImageTexCoordsInternalList(imageTexCoords);
	}

	/// <summary>
	///   <para>Set the array of billboard image texture coordinate data.</para>
	/// </summary>
	/// <param name="imageTexCoords">The array of data to set.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetImageTexCoords(Vector4[] imageTexCoords);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern void SetImageTexCoordsInternalList(object list);

	public void GetVertices(List<Vector2> vertices)
	{
		if (vertices == null)
		{
			throw new ArgumentNullException("vertices");
		}
		GetVerticesInternal(vertices);
	}

	/// <summary>
	///   <para>Get the vertices of the billboard mesh.</para>
	/// </summary>
	/// <param name="vertices">The list that receives the array.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern Vector2[] GetVertices();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern void GetVerticesInternal(object list);

	public void SetVertices(List<Vector2> vertices)
	{
		if (vertices == null)
		{
			throw new ArgumentNullException("vertices");
		}
		SetVerticesInternalList(vertices);
	}

	/// <summary>
	///   <para>Set the vertices of the billboard mesh.</para>
	/// </summary>
	/// <param name="vertices">The array of data to set.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetVertices(Vector2[] vertices);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern void SetVerticesInternalList(object list);

	public void GetIndices(List<ushort> indices)
	{
		if (indices == null)
		{
			throw new ArgumentNullException("indices");
		}
		GetIndicesInternal(indices);
	}

	/// <summary>
	///   <para>Get the indices of the billboard mesh.</para>
	/// </summary>
	/// <param name="indices">The list that receives the array.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern ushort[] GetIndices();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern void GetIndicesInternal(object list);

	public void SetIndices(List<ushort> indices)
	{
		if (indices == null)
		{
			throw new ArgumentNullException("indices");
		}
		SetIndicesInternalList(indices);
	}

	/// <summary>
	///   <para>Set the indices of the billboard mesh.</para>
	/// </summary>
	/// <param name="indices">The array of data to set.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetIndices(ushort[] indices);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern void SetIndicesInternalList(object list);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern void MakeMaterialProperties(MaterialPropertyBlock properties, Camera camera);
}

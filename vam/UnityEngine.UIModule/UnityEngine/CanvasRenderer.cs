using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>A component that will render to the screen after all normal rendering has completed when attached to a Canvas. Designed for GUI application.</para>
/// </summary>
[NativeClass("UI::CanvasRenderer")]
public sealed class CanvasRenderer : Component
{
	/// <summary>
	///   <para>Is the UIRenderer a mask component.</para>
	/// </summary>
	[Obsolete("isMask is no longer supported. See EnableClipping for vertex clipping configuration")]
	public extern bool isMask
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>True if rect clipping has been enabled on this renderer.
	/// See Also: CanvasRenderer.EnableRectClipping, CanvasRenderer.DisableRectClipping.</para>
	/// </summary>
	public extern bool hasRectClipping
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Enable 'render stack' pop draw call.</para>
	/// </summary>
	public extern bool hasPopInstruction
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The number of materials usable by this renderer.</para>
	/// </summary>
	public extern int materialCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The number of materials usable by this renderer. Used internally for masking.</para>
	/// </summary>
	public extern int popMaterialCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Depth of the renderer realative to the parent canvas.</para>
	/// </summary>
	public extern int relativeDepth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Indicates whether geometry emitted by this renderer is ignored.</para>
	/// </summary>
	public extern bool cull
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Depth of the renderer relative to the root canvas.</para>
	/// </summary>
	public extern int absoluteDepth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>True if any change has occured that would invalidate the positions of generated geometry.</para>
	/// </summary>
	public extern bool hasMoved
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Set the color of the renderer. Will be multiplied with the UIVertex color and the Canvas color.</para>
	/// </summary>
	/// <param name="color">Renderer multiply color.</param>
	public void SetColor(Color color)
	{
		INTERNAL_CALL_SetColor(this, ref color);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_SetColor(CanvasRenderer self, ref Color color);

	/// <summary>
	///   <para>Get the current color of the renderer.</para>
	/// </summary>
	public Color GetColor()
	{
		INTERNAL_CALL_GetColor(this, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetColor(CanvasRenderer self, out Color value);

	/// <summary>
	///   <para>Get the current alpha of the renderer.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern float GetAlpha();

	/// <summary>
	///   <para>Set the alpha of the renderer. Will be multiplied with the UIVertex alpha and the Canvas alpha.</para>
	/// </summary>
	/// <param name="alpha">Alpha.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetAlpha(float alpha);

	[Obsolete("UI System now uses meshes. Generate a mesh and use 'SetMesh' instead")]
	public void SetVertices(List<UIVertex> vertices)
	{
		SetVertices(vertices.ToArray(), vertices.Count);
	}

	/// <summary>
	///   <para>Set the vertices for the UIRenderer.</para>
	/// </summary>
	/// <param name="vertices">Array of vertices to set.</param>
	/// <param name="size">Number of vertices to set.</param>
	[Obsolete("UI System now uses meshes. Generate a mesh and use 'SetMesh' instead")]
	public void SetVertices(UIVertex[] vertices, int size)
	{
		Mesh mesh = new Mesh();
		List<Vector3> list = new List<Vector3>();
		List<Color32> list2 = new List<Color32>();
		List<Vector2> list3 = new List<Vector2>();
		List<Vector2> list4 = new List<Vector2>();
		List<Vector2> list5 = new List<Vector2>();
		List<Vector2> list6 = new List<Vector2>();
		List<Vector3> list7 = new List<Vector3>();
		List<Vector4> list8 = new List<Vector4>();
		List<int> list9 = new List<int>();
		for (int i = 0; i < size; i += 4)
		{
			for (int j = 0; j < 4; j++)
			{
				list.Add(vertices[i + j].position);
				list2.Add(vertices[i + j].color);
				list3.Add(vertices[i + j].uv0);
				list4.Add(vertices[i + j].uv1);
				list5.Add(vertices[i + j].uv2);
				list6.Add(vertices[i + j].uv3);
				list7.Add(vertices[i + j].normal);
				list8.Add(vertices[i + j].tangent);
			}
			list9.Add(i);
			list9.Add(i + 1);
			list9.Add(i + 2);
			list9.Add(i + 2);
			list9.Add(i + 3);
			list9.Add(i);
		}
		mesh.SetVertices(list);
		mesh.SetColors(list2);
		mesh.SetNormals(list7);
		mesh.SetTangents(list8);
		mesh.SetUVs(0, list3);
		mesh.SetUVs(1, list4);
		mesh.SetUVs(2, list5);
		mesh.SetUVs(3, list6);
		mesh.SetIndices(list9.ToArray(), MeshTopology.Triangles, 0);
		SetMesh(mesh);
		Object.DestroyImmediate(mesh);
	}

	/// <summary>
	///   <para>Enables rect clipping on the CanvasRendered. Geometry outside of the specified rect will be clipped (not rendered).</para>
	/// </summary>
	/// <param name="rect"></param>
	public void EnableRectClipping(Rect rect)
	{
		INTERNAL_CALL_EnableRectClipping(this, ref rect);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_EnableRectClipping(CanvasRenderer self, ref Rect rect);

	/// <summary>
	///   <para>Disables rectangle clipping for this CanvasRenderer.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void DisableRectClipping();

	/// <summary>
	///   <para>Set the material for the canvas renderer. If a texture is specified then it will be used as the 'MainTex' instead of the material's 'MainTex'.
	/// See Also: CanvasRenderer.SetMaterialCount, CanvasRenderer.SetTexture.</para>
	/// </summary>
	/// <param name="material">Material for rendering.</param>
	/// <param name="texture">Material texture overide.</param>
	/// <param name="index">Material index.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetMaterial(Material material, int index);

	/// <summary>
	///   <para>Set the material for the canvas renderer. If a texture is specified then it will be used as the 'MainTex' instead of the material's 'MainTex'.
	/// See Also: CanvasRenderer.SetMaterialCount, CanvasRenderer.SetTexture.</para>
	/// </summary>
	/// <param name="material">Material for rendering.</param>
	/// <param name="texture">Material texture overide.</param>
	/// <param name="index">Material index.</param>
	public void SetMaterial(Material material, Texture texture)
	{
		materialCount = Math.Max(1, materialCount);
		SetMaterial(material, 0);
		SetTexture(texture);
	}

	/// <summary>
	///   <para>Gets the current Material assigned to the CanvasRenderer.</para>
	/// </summary>
	/// <param name="index">The material index to retrieve (0 if this parameter is omitted).</param>
	/// <returns>
	///   <para>Result.</para>
	/// </returns>
	public Material GetMaterial()
	{
		return GetMaterial(0);
	}

	/// <summary>
	///   <para>Gets the current Material assigned to the CanvasRenderer.</para>
	/// </summary>
	/// <param name="index">The material index to retrieve (0 if this parameter is omitted).</param>
	/// <returns>
	///   <para>Result.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern Material GetMaterial(int index);

	/// <summary>
	///   <para>Set the material for the canvas renderer. Used internally for masking.</para>
	/// </summary>
	/// <param name="material"></param>
	/// <param name="index"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetPopMaterial(Material material, int index);

	/// <summary>
	///   <para>Gets the current Material assigned to the CanvasRenderer. Used internally for masking.</para>
	/// </summary>
	/// <param name="index"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern Material GetPopMaterial(int index);

	/// <summary>
	///   <para>Sets the texture used by this renderer's material.</para>
	/// </summary>
	/// <param name="texture"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetTexture(Texture texture);

	/// <summary>
	///   <para>The Alpha Texture that will be passed to the Shader under the _AlphaTex property.</para>
	/// </summary>
	/// <param name="texture">The Texture to be passed.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetAlphaTexture(Texture texture);

	/// <summary>
	///   <para>Sets the Mesh used by this renderer.</para>
	/// </summary>
	/// <param name="mesh"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetMesh(Mesh mesh);

	/// <summary>
	///   <para>Remove all cached vertices.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void Clear();

	public static void SplitUIVertexStreams(List<UIVertex> verts, List<Vector3> positions, List<Color32> colors, List<Vector2> uv0S, List<Vector2> uv1S, List<Vector3> normals, List<Vector4> tangents, List<int> indices)
	{
		SplitUIVertexStreams(verts, positions, colors, uv0S, uv1S, new List<Vector2>(), new List<Vector2>(), normals, tangents, indices);
	}

	public static void SplitUIVertexStreams(List<UIVertex> verts, List<Vector3> positions, List<Color32> colors, List<Vector2> uv0S, List<Vector2> uv1S, List<Vector2> uv2S, List<Vector2> uv3S, List<Vector3> normals, List<Vector4> tangents, List<int> indices)
	{
		SplitUIVertexStreamsInternal(verts, positions, colors, uv0S, uv1S, uv2S, uv3S, normals, tangents);
		SplitIndicesStreamsInternal(verts, indices);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void SplitUIVertexStreamsInternal(object verts, object positions, object colors, object uv0S, object uv1S, object uv2S, object uv3S, object normals, object tangents);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void SplitIndicesStreamsInternal(object verts, object indices);

	public static void CreateUIVertexStream(List<UIVertex> verts, List<Vector3> positions, List<Color32> colors, List<Vector2> uv0S, List<Vector2> uv1S, List<Vector3> normals, List<Vector4> tangents, List<int> indices)
	{
		CreateUIVertexStream(verts, positions, colors, uv0S, uv1S, new List<Vector2>(), new List<Vector2>(), normals, tangents, indices);
	}

	public static void CreateUIVertexStream(List<UIVertex> verts, List<Vector3> positions, List<Color32> colors, List<Vector2> uv0S, List<Vector2> uv1S, List<Vector2> uv2S, List<Vector2> uv3S, List<Vector3> normals, List<Vector4> tangents, List<int> indices)
	{
		CreateUIVertexStreamInternal(verts, positions, colors, uv0S, uv1S, uv2S, uv3S, normals, tangents, indices);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void CreateUIVertexStreamInternal(object verts, object positions, object colors, object uv0S, object uv1S, object uv2S, object uv3S, object normals, object tangents, object indices);

	public static void AddUIVertexStream(List<UIVertex> verts, List<Vector3> positions, List<Color32> colors, List<Vector2> uv0S, List<Vector2> uv1S, List<Vector3> normals, List<Vector4> tangents)
	{
		AddUIVertexStream(verts, positions, colors, uv0S, uv1S, new List<Vector2>(), new List<Vector2>(), normals, tangents);
	}

	public static void AddUIVertexStream(List<UIVertex> verts, List<Vector3> positions, List<Color32> colors, List<Vector2> uv0S, List<Vector2> uv1S, List<Vector2> uv2S, List<Vector2> uv3S, List<Vector3> normals, List<Vector4> tangents)
	{
		SplitUIVertexStreamsInternal(verts, positions, colors, uv0S, uv1S, uv2S, uv3S, normals, tangents);
	}
}

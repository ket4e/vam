using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Rendering;

namespace UnityEngine;

/// <summary>
///   <para>A class that allows creating or modifying meshes from scripts.</para>
/// </summary>
[NativeHeader("Runtime/Graphics/Mesh/MeshScriptBindings.h")]
public sealed class Mesh : Object
{
	internal enum InternalShaderChannel
	{
		Vertex,
		Normal,
		Tangent,
		Color,
		TexCoord0,
		TexCoord1,
		TexCoord2,
		TexCoord3,
		TexCoord4,
		TexCoord5,
		TexCoord6,
		TexCoord7
	}

	internal enum InternalVertexChannelType
	{
		Float = 0,
		Color = 2
	}

	/// <summary>
	///   <para>Format of the mesh index buffer data.</para>
	/// </summary>
	public extern IndexFormat indexFormat
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Gets the number of vertex buffers present in the Mesh. (Read Only)</para>
	/// </summary>
	public extern int vertexBufferCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction(Name = "MeshScripting::GetVertexBufferCount", HasExplicitThis = true)]
		get;
	}

	/// <summary>
	///   <para>Returns BlendShape count on this mesh.</para>
	/// </summary>
	public extern int blendShapeCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(Name = "GetBlendShapeChannelCount")]
		get;
	}

	/// <summary>
	///   <para>The bone weights of each vertex.</para>
	/// </summary>
	[NativeName("BoneWeightsFromScript")]
	public extern BoneWeight[] boneWeights
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The bind poses. The bind pose at each index refers to the bone with the same index.</para>
	/// </summary>
	[NativeName("BindPosesFromScript")]
	public extern Matrix4x4[] bindposes
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Returns state of the Read/Write Enabled checkbox when model was imported.</para>
	/// </summary>
	public extern bool isReadable
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("GetIsReadable")]
		get;
	}

	internal extern bool canAccess
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("CanAccessFromScript")]
		get;
	}

	/// <summary>
	///   <para>Returns the number of vertices in the Mesh (Read Only).</para>
	/// </summary>
	public extern int vertexCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("GetVertexCount")]
		get;
	}

	/// <summary>
	///   <para>The number of sub-meshes inside the Mesh object.</para>
	/// </summary>
	public extern int subMeshCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(Name = "GetSubMeshCount")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction(Name = "MeshScripting::SetSubMeshCount", HasExplicitThis = true)]
		set;
	}

	/// <summary>
	///   <para>The bounding volume of the mesh.</para>
	/// </summary>
	public Bounds bounds
	{
		get
		{
			get_bounds_Injected(out var ret);
			return ret;
		}
		set
		{
			set_bounds_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>Returns a copy of the vertex positions or assigns a new vertex positions array.</para>
	/// </summary>
	public Vector3[] vertices
	{
		get
		{
			return GetAllocArrayFromChannel<Vector3>(InternalShaderChannel.Vertex);
		}
		set
		{
			SetArrayForChannel(InternalShaderChannel.Vertex, value);
		}
	}

	/// <summary>
	///   <para>The normals of the Mesh.</para>
	/// </summary>
	public Vector3[] normals
	{
		get
		{
			return GetAllocArrayFromChannel<Vector3>(InternalShaderChannel.Normal);
		}
		set
		{
			SetArrayForChannel(InternalShaderChannel.Normal, value);
		}
	}

	/// <summary>
	///   <para>The tangents of the Mesh.</para>
	/// </summary>
	public Vector4[] tangents
	{
		get
		{
			return GetAllocArrayFromChannel<Vector4>(InternalShaderChannel.Tangent);
		}
		set
		{
			SetArrayForChannel(InternalShaderChannel.Tangent, value);
		}
	}

	/// <summary>
	///   <para>The base texture coordinates of the Mesh.</para>
	/// </summary>
	public Vector2[] uv
	{
		get
		{
			return GetAllocArrayFromChannel<Vector2>(InternalShaderChannel.TexCoord0);
		}
		set
		{
			SetArrayForChannel(InternalShaderChannel.TexCoord0, value);
		}
	}

	/// <summary>
	///   <para>The second texture coordinate set of the mesh, if present.</para>
	/// </summary>
	public Vector2[] uv2
	{
		get
		{
			return GetAllocArrayFromChannel<Vector2>(InternalShaderChannel.TexCoord1);
		}
		set
		{
			SetArrayForChannel(InternalShaderChannel.TexCoord1, value);
		}
	}

	/// <summary>
	///   <para>The third texture coordinate set of the mesh, if present.</para>
	/// </summary>
	public Vector2[] uv3
	{
		get
		{
			return GetAllocArrayFromChannel<Vector2>(InternalShaderChannel.TexCoord2);
		}
		set
		{
			SetArrayForChannel(InternalShaderChannel.TexCoord2, value);
		}
	}

	/// <summary>
	///   <para>The fourth texture coordinate set of the mesh, if present.</para>
	/// </summary>
	public Vector2[] uv4
	{
		get
		{
			return GetAllocArrayFromChannel<Vector2>(InternalShaderChannel.TexCoord3);
		}
		set
		{
			SetArrayForChannel(InternalShaderChannel.TexCoord3, value);
		}
	}

	/// <summary>
	///   <para>Vertex colors of the Mesh.</para>
	/// </summary>
	public Color[] colors
	{
		get
		{
			return GetAllocArrayFromChannel<Color>(InternalShaderChannel.Color);
		}
		set
		{
			SetArrayForChannel(InternalShaderChannel.Color, value);
		}
	}

	/// <summary>
	///   <para>Vertex colors of the Mesh.</para>
	/// </summary>
	public Color32[] colors32
	{
		get
		{
			return GetAllocArrayFromChannel<Color32>(InternalShaderChannel.Color, InternalVertexChannelType.Color, 1);
		}
		set
		{
			SetArrayForChannel(InternalShaderChannel.Color, InternalVertexChannelType.Color, 1, value);
		}
	}

	/// <summary>
	///   <para>An array containing all triangles in the Mesh.</para>
	/// </summary>
	public int[] triangles
	{
		get
		{
			if (canAccess)
			{
				return GetTrianglesImpl(-1, applyBaseVertex: true);
			}
			PrintErrorCantAccessIndices();
			return new int[0];
		}
		set
		{
			if (canAccess)
			{
				SetTrianglesImpl(-1, value, NoAllocHelpers.SafeLength(value), calculateBounds: true, 0);
			}
			else
			{
				PrintErrorCantAccessIndices();
			}
		}
	}

	/// <summary>
	///   <para>Creates an empty Mesh.</para>
	/// </summary>
	public Mesh()
	{
		Internal_Create(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("MeshScripting::CreateMesh")]
	private static extern void Internal_Create([Writable] Mesh mono);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("MeshScripting::MeshFromInstanceId")]
	internal static extern Mesh FromInstanceID(int id);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MeshScripting::GetIndexStart", HasExplicitThis = true)]
	private extern uint GetIndexStartImpl(int submesh);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MeshScripting::GetIndexCount", HasExplicitThis = true)]
	private extern uint GetIndexCountImpl(int submesh);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MeshScripting::GetBaseVertex", HasExplicitThis = true)]
	private extern uint GetBaseVertexImpl(int submesh);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MeshScripting::GetTriangles", HasExplicitThis = true)]
	private extern int[] GetTrianglesImpl(int submesh, bool applyBaseVertex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MeshScripting::GetIndices", HasExplicitThis = true)]
	private extern int[] GetIndicesImpl(int submesh, bool applyBaseVertex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "SetMeshIndicesFromScript", HasExplicitThis = true)]
	private extern void SetIndicesImpl(int submesh, MeshTopology topology, Array indices, int arraySize, bool calculateBounds, int baseVertex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MeshScripting::ExtractTrianglesToArray", HasExplicitThis = true)]
	private extern void GetTrianglesNonAllocImpl([Out] int[] values, int submesh, bool applyBaseVertex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MeshScripting::ExtractIndicesToArray", HasExplicitThis = true)]
	private extern void GetIndicesNonAllocImpl([Out] int[] values, int submesh, bool applyBaseVertex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MeshScripting::PrintErrorCantAccessChannel", HasExplicitThis = true)]
	private extern void PrintErrorCantAccessChannel(InternalShaderChannel ch);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MeshScripting::HasChannel", HasExplicitThis = true)]
	internal extern bool HasChannel(InternalShaderChannel ch);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "SetMeshComponentFromArrayFromScript", HasExplicitThis = true)]
	private extern void SetArrayForChannelImpl(InternalShaderChannel channel, InternalVertexChannelType format, int dim, Array values, int arraySize);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AllocExtractMeshComponentFromScript", HasExplicitThis = true)]
	private extern Array GetAllocArrayFromChannelImpl(InternalShaderChannel channel, InternalVertexChannelType format, int dim);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "ExtractMeshComponentFromScript", HasExplicitThis = true)]
	private extern void GetArrayFromChannelImpl(InternalShaderChannel channel, InternalVertexChannelType format, int dim, Array values);

	/// <summary>
	///   <para>Retrieves a native (underlying graphics API) pointer to the vertex buffer.</para>
	/// </summary>
	/// <param name="bufferIndex">Which vertex buffer to get (some Meshes might have more than one). See vertexBufferCount.</param>
	/// <param name="index"></param>
	/// <returns>
	///   <para>Pointer to the underlying graphics API vertex buffer.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeThrows]
	[FreeFunction(Name = "MeshScripting::GetNativeVertexBufferPtr", HasExplicitThis = true)]
	public extern IntPtr GetNativeVertexBufferPtr(int index);

	/// <summary>
	///   <para>Retrieves a native (underlying graphics API) pointer to the index buffer.</para>
	/// </summary>
	/// <returns>
	///   <para>Pointer to the underlying graphics API index buffer.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MeshScripting::GetNativeIndexBufferPtr", HasExplicitThis = true)]
	public extern IntPtr GetNativeIndexBufferPtr();

	/// <summary>
	///   <para>Clears all blend shapes from Mesh.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MeshScripting::ClearBlendShapes", HasExplicitThis = true)]
	public extern void ClearBlendShapes();

	/// <summary>
	///   <para>Returns name of BlendShape by given index.</para>
	/// </summary>
	/// <param name="shapeIndex"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MeshScripting::GetBlendShapeName", HasExplicitThis = true)]
	public extern string GetBlendShapeName(int shapeIndex);

	/// <summary>
	///   <para>Returns index of BlendShape by given name.</para>
	/// </summary>
	/// <param name="blendShapeName"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MeshScripting::GetBlendShapeIndex", HasExplicitThis = true)]
	public extern int GetBlendShapeIndex(string blendShapeName);

	/// <summary>
	///   <para>Returns the frame count for a blend shape.</para>
	/// </summary>
	/// <param name="shapeIndex">The shape index to get frame count from.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MeshScripting::GetBlendShapeFrameCount", HasExplicitThis = true)]
	public extern int GetBlendShapeFrameCount(int shapeIndex);

	/// <summary>
	///   <para>Returns the weight of a blend shape frame.</para>
	/// </summary>
	/// <param name="shapeIndex">The shape index of the frame.</param>
	/// <param name="frameIndex">The frame index to get the weight from.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MeshScripting::GetBlendShapeFrameWeight", HasExplicitThis = true)]
	public extern float GetBlendShapeFrameWeight(int shapeIndex, int frameIndex);

	/// <summary>
	///   <para>Retreives deltaVertices, deltaNormals and deltaTangents of a blend shape frame.</para>
	/// </summary>
	/// <param name="shapeIndex">The shape index of the frame.</param>
	/// <param name="frameIndex">The frame index to get the weight from.</param>
	/// <param name="deltaVertices">Delta vertices output array for the frame being retreived.</param>
	/// <param name="deltaNormals">Delta normals output array for the frame being retreived.</param>
	/// <param name="deltaTangents">Delta tangents output array for the frame being retreived.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "GetBlendShapeFrameVerticesFromScript", HasExplicitThis = true)]
	public extern void GetBlendShapeFrameVertices(int shapeIndex, int frameIndex, Vector3[] deltaVertices, Vector3[] deltaNormals, Vector3[] deltaTangents);

	/// <summary>
	///   <para>Adds a new blend shape frame.</para>
	/// </summary>
	/// <param name="shapeName">Name of the blend shape to add a frame to.</param>
	/// <param name="frameWeight">Weight for the frame being added.</param>
	/// <param name="deltaVertices">Delta vertices for the frame being added.</param>
	/// <param name="deltaNormals">Delta normals for the frame being added.</param>
	/// <param name="deltaTangents">Delta tangents for the frame being added.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AddBlendShapeFrameFromScript", HasExplicitThis = true)]
	public extern void AddBlendShapeFrame(string shapeName, float frameWeight, Vector3[] deltaVertices, Vector3[] deltaNormals, Vector3[] deltaTangents);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern int GetBoneWeightCount();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern int GetBindposeCount();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MeshScripting::ExtractBoneWeightsIntoArray", HasExplicitThis = true)]
	private extern void GetBoneWeightsNonAllocImpl([Out] BoneWeight[] values);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MeshScripting::ExtractBindPosesIntoArray", HasExplicitThis = true)]
	private extern void GetBindposesNonAllocImpl([Out] Matrix4x4[] values);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("Clear")]
	private extern void ClearImpl(bool keepVertexLayout);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("RecalculateBounds")]
	private extern void RecalculateBoundsImpl();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("RecalculateNormals")]
	private extern void RecalculateNormalsImpl();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("RecalculateTangents")]
	private extern void RecalculateTangentsImpl();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("MarkDynamic")]
	private extern void MarkDynamicImpl();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("UploadMeshData")]
	private extern void UploadMeshDataImpl(bool markNoLongerReadable);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MeshScripting::GetPrimitiveType", HasExplicitThis = true)]
	private extern MeshTopology GetTopologyImpl(int submesh);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MeshScripting::CombineMeshes", HasExplicitThis = true)]
	private extern void CombineMeshesImpl(CombineInstance[] combine, bool mergeSubMeshes, bool useMatrices, bool hasLightmapData);

	internal InternalShaderChannel GetUVChannel(int uvIndex)
	{
		if (uvIndex < 0 || uvIndex > 3)
		{
			throw new ArgumentException("GetUVChannel called for bad uvIndex", "uvIndex");
		}
		return (InternalShaderChannel)(4 + uvIndex);
	}

	internal static int DefaultDimensionForChannel(InternalShaderChannel channel)
	{
		switch (channel)
		{
		case InternalShaderChannel.Vertex:
		case InternalShaderChannel.Normal:
			return 3;
		case InternalShaderChannel.TexCoord0:
		case InternalShaderChannel.TexCoord1:
		case InternalShaderChannel.TexCoord2:
		case InternalShaderChannel.TexCoord3:
			return 2;
		default:
			if (channel == InternalShaderChannel.Tangent || channel == InternalShaderChannel.Color)
			{
				return 4;
			}
			throw new ArgumentException("DefaultDimensionForChannel called for bad channel", "channel");
		}
	}

	private T[] GetAllocArrayFromChannel<T>(InternalShaderChannel channel, InternalVertexChannelType format, int dim)
	{
		if (canAccess)
		{
			if (HasChannel(channel))
			{
				return (T[])GetAllocArrayFromChannelImpl(channel, format, dim);
			}
		}
		else
		{
			PrintErrorCantAccessChannel(channel);
		}
		return new T[0];
	}

	private T[] GetAllocArrayFromChannel<T>(InternalShaderChannel channel)
	{
		return GetAllocArrayFromChannel<T>(channel, InternalVertexChannelType.Float, DefaultDimensionForChannel(channel));
	}

	private void SetSizedArrayForChannel(InternalShaderChannel channel, InternalVertexChannelType format, int dim, Array values, int valuesCount)
	{
		if (canAccess)
		{
			SetArrayForChannelImpl(channel, format, dim, values, valuesCount);
		}
		else
		{
			PrintErrorCantAccessChannel(channel);
		}
	}

	private void SetArrayForChannel<T>(InternalShaderChannel channel, InternalVertexChannelType format, int dim, T[] values)
	{
		SetSizedArrayForChannel(channel, format, dim, values, NoAllocHelpers.SafeLength(values));
	}

	private void SetArrayForChannel<T>(InternalShaderChannel channel, T[] values)
	{
		SetSizedArrayForChannel(channel, InternalVertexChannelType.Float, DefaultDimensionForChannel(channel), values, NoAllocHelpers.SafeLength(values));
	}

	private void SetListForChannel<T>(InternalShaderChannel channel, InternalVertexChannelType format, int dim, List<T> values)
	{
		SetSizedArrayForChannel(channel, format, dim, NoAllocHelpers.ExtractArrayFromList(values), NoAllocHelpers.SafeLength(values));
	}

	private void SetListForChannel<T>(InternalShaderChannel channel, List<T> values)
	{
		SetSizedArrayForChannel(channel, InternalVertexChannelType.Float, DefaultDimensionForChannel(channel), NoAllocHelpers.ExtractArrayFromList(values), NoAllocHelpers.SafeLength(values));
	}

	private void GetListForChannel<T>(List<T> buffer, int capacity, InternalShaderChannel channel, int dim)
	{
		GetListForChannel(buffer, capacity, channel, dim, InternalVertexChannelType.Float);
	}

	private void GetListForChannel<T>(List<T> buffer, int capacity, InternalShaderChannel channel, int dim, InternalVertexChannelType channelType)
	{
		buffer.Clear();
		if (!canAccess)
		{
			PrintErrorCantAccessChannel(channel);
		}
		else if (HasChannel(channel))
		{
			NoAllocHelpers.EnsureListElemCount(buffer, capacity);
			GetArrayFromChannelImpl(channel, channelType, dim, NoAllocHelpers.ExtractArrayFromList(buffer));
		}
	}

	public void GetVertices(List<Vector3> vertices)
	{
		if (vertices == null)
		{
			throw new ArgumentNullException("The result vertices list cannot be null.", "vertices");
		}
		GetListForChannel(vertices, vertexCount, InternalShaderChannel.Vertex, DefaultDimensionForChannel(InternalShaderChannel.Vertex));
	}

	public void SetVertices(List<Vector3> inVertices)
	{
		SetListForChannel(InternalShaderChannel.Vertex, inVertices);
	}

	public void GetNormals(List<Vector3> normals)
	{
		if (normals == null)
		{
			throw new ArgumentNullException("The result normals list cannot be null.", "normals");
		}
		GetListForChannel(normals, vertexCount, InternalShaderChannel.Normal, DefaultDimensionForChannel(InternalShaderChannel.Normal));
	}

	public void SetNormals(List<Vector3> inNormals)
	{
		SetListForChannel(InternalShaderChannel.Normal, inNormals);
	}

	public void GetTangents(List<Vector4> tangents)
	{
		if (tangents == null)
		{
			throw new ArgumentNullException("The result tangents list cannot be null.", "tangents");
		}
		GetListForChannel(tangents, vertexCount, InternalShaderChannel.Tangent, DefaultDimensionForChannel(InternalShaderChannel.Tangent));
	}

	public void SetTangents(List<Vector4> inTangents)
	{
		SetListForChannel(InternalShaderChannel.Tangent, inTangents);
	}

	public void GetColors(List<Color> colors)
	{
		if (colors == null)
		{
			throw new ArgumentNullException("The result colors list cannot be null.", "colors");
		}
		GetListForChannel(colors, vertexCount, InternalShaderChannel.Color, DefaultDimensionForChannel(InternalShaderChannel.Color));
	}

	public void SetColors(List<Color> inColors)
	{
		SetListForChannel(InternalShaderChannel.Color, inColors);
	}

	public void GetColors(List<Color32> colors)
	{
		if (colors == null)
		{
			throw new ArgumentNullException("The result colors list cannot be null.", "colors");
		}
		GetListForChannel(colors, vertexCount, InternalShaderChannel.Color, 1, InternalVertexChannelType.Color);
	}

	public void SetColors(List<Color32> inColors)
	{
		SetListForChannel(InternalShaderChannel.Color, InternalVertexChannelType.Color, 1, inColors);
	}

	private void SetUvsImpl<T>(int uvIndex, int dim, List<T> uvs)
	{
		if (uvIndex < 0 || uvIndex > 3)
		{
			Debug.LogError("The uv index is invalid (must be in [0..3]");
		}
		else
		{
			SetListForChannel(GetUVChannel(uvIndex), InternalVertexChannelType.Float, dim, uvs);
		}
	}

	public void SetUVs(int channel, List<Vector2> uvs)
	{
		SetUvsImpl(channel, 2, uvs);
	}

	public void SetUVs(int channel, List<Vector3> uvs)
	{
		SetUvsImpl(channel, 3, uvs);
	}

	public void SetUVs(int channel, List<Vector4> uvs)
	{
		SetUvsImpl(channel, 4, uvs);
	}

	private void GetUVsImpl<T>(int uvIndex, List<T> uvs, int dim)
	{
		if (uvs == null)
		{
			throw new ArgumentNullException("The result uvs list cannot be null.", "uvs");
		}
		if (uvIndex < 0 || uvIndex > 3)
		{
			throw new IndexOutOfRangeException("Specified uv index is out of range. Must be in the range [0, 3].");
		}
		GetListForChannel(uvs, vertexCount, GetUVChannel(uvIndex), dim);
	}

	public void GetUVs(int channel, List<Vector2> uvs)
	{
		GetUVsImpl(channel, uvs, 2);
	}

	public void GetUVs(int channel, List<Vector3> uvs)
	{
		GetUVsImpl(channel, uvs, 3);
	}

	public void GetUVs(int channel, List<Vector4> uvs)
	{
		GetUVsImpl(channel, uvs, 4);
	}

	private void PrintErrorCantAccessIndices()
	{
		Debug.LogError($"Not allowed to access triangles/indices on mesh '{base.name}' (isReadable is false; Read/Write must be enabled in import settings)");
	}

	private bool CheckCanAccessSubmesh(int submesh, bool errorAboutTriangles)
	{
		if (!canAccess)
		{
			PrintErrorCantAccessIndices();
			return false;
		}
		if (submesh < 0 || submesh >= subMeshCount)
		{
			Debug.LogError(string.Format("Failed getting {0}. Submesh index is out of bounds.", (!errorAboutTriangles) ? "indices" : "triangles"), this);
			return false;
		}
		return true;
	}

	private bool CheckCanAccessSubmeshTriangles(int submesh)
	{
		return CheckCanAccessSubmesh(submesh, errorAboutTriangles: true);
	}

	private bool CheckCanAccessSubmeshIndices(int submesh)
	{
		return CheckCanAccessSubmesh(submesh, errorAboutTriangles: false);
	}

	/// <summary>
	///   <para>Fetches the triangle list for the specified sub-mesh on this object.</para>
	/// </summary>
	/// <param name="triangles">A list of vertex indices to populate.</param>
	/// <param name="submesh">The sub-mesh index. See subMeshCount.</param>
	/// <param name="applyBaseVertex">True (default value) will apply base vertex offset to returned indices.</param>
	public int[] GetTriangles(int submesh)
	{
		return GetTriangles(submesh, applyBaseVertex: true);
	}

	/// <summary>
	///   <para>Fetches the triangle list for the specified sub-mesh on this object.</para>
	/// </summary>
	/// <param name="triangles">A list of vertex indices to populate.</param>
	/// <param name="submesh">The sub-mesh index. See subMeshCount.</param>
	/// <param name="applyBaseVertex">True (default value) will apply base vertex offset to returned indices.</param>
	public int[] GetTriangles(int submesh, [UnityEngine.Internal.DefaultValue("true")] bool applyBaseVertex)
	{
		return (!CheckCanAccessSubmeshTriangles(submesh)) ? new int[0] : GetTrianglesImpl(submesh, applyBaseVertex);
	}

	public void GetTriangles(List<int> triangles, int submesh)
	{
		GetTriangles(triangles, submesh, applyBaseVertex: true);
	}

	public void GetTriangles(List<int> triangles, int submesh, [UnityEngine.Internal.DefaultValue("true")] bool applyBaseVertex)
	{
		if (triangles == null)
		{
			throw new ArgumentNullException("The result triangles list cannot be null.", "triangles");
		}
		if (submesh < 0 || submesh >= subMeshCount)
		{
			throw new IndexOutOfRangeException("Specified sub mesh is out of range. Must be greater or equal to 0 and less than subMeshCount.");
		}
		NoAllocHelpers.EnsureListElemCount(triangles, (int)GetIndexCount(submesh));
		GetTrianglesNonAllocImpl(NoAllocHelpers.ExtractArrayFromListT(triangles), submesh, applyBaseVertex);
	}

	/// <summary>
	///   <para>Fetches the index list for the specified sub-mesh.</para>
	/// </summary>
	/// <param name="indices">A list of indices to populate.</param>
	/// <param name="submesh">The sub-mesh index. See subMeshCount.</param>
	/// <param name="applyBaseVertex">True (default value) will apply base vertex offset to returned indices.</param>
	public int[] GetIndices(int submesh)
	{
		return GetIndices(submesh, applyBaseVertex: true);
	}

	public int[] GetIndices(int submesh, [UnityEngine.Internal.DefaultValue("true")] bool applyBaseVertex)
	{
		return (!CheckCanAccessSubmeshIndices(submesh)) ? new int[0] : GetIndicesImpl(submesh, applyBaseVertex);
	}

	public void GetIndices(List<int> indices, int submesh)
	{
		GetIndices(indices, submesh, applyBaseVertex: true);
	}

	public void GetIndices(List<int> indices, int submesh, [UnityEngine.Internal.DefaultValue("true")] bool applyBaseVertex)
	{
		if (indices == null)
		{
			throw new ArgumentNullException("The result indices list cannot be null.", "indices");
		}
		if (submesh < 0 || submesh >= subMeshCount)
		{
			throw new IndexOutOfRangeException("Specified sub mesh is out of range. Must be greater or equal to 0 and less than subMeshCount.");
		}
		NoAllocHelpers.EnsureListElemCount(indices, (int)GetIndexCount(submesh));
		GetIndicesNonAllocImpl(NoAllocHelpers.ExtractArrayFromListT(indices), submesh, applyBaseVertex);
	}

	/// <summary>
	///   <para>Gets the starting index location within the Mesh's index buffer, for the given sub-mesh.</para>
	/// </summary>
	/// <param name="submesh"></param>
	public uint GetIndexStart(int submesh)
	{
		if (submesh < 0 || submesh >= subMeshCount)
		{
			throw new IndexOutOfRangeException("Specified sub mesh is out of range. Must be greater or equal to 0 and less than subMeshCount.");
		}
		return GetIndexStartImpl(submesh);
	}

	/// <summary>
	///   <para>Gets the index count of the given sub-mesh.</para>
	/// </summary>
	/// <param name="submesh"></param>
	public uint GetIndexCount(int submesh)
	{
		if (submesh < 0 || submesh >= subMeshCount)
		{
			throw new IndexOutOfRangeException("Specified sub mesh is out of range. Must be greater or equal to 0 and less than subMeshCount.");
		}
		return GetIndexCountImpl(submesh);
	}

	/// <summary>
	///   <para>Gets the base vertex index of the given sub-mesh.</para>
	/// </summary>
	/// <param name="submesh">The sub-mesh index. See subMeshCount.</param>
	/// <returns>
	///   <para>The offset applied to all vertex indices of this sub-mesh.</para>
	/// </returns>
	public uint GetBaseVertex(int submesh)
	{
		if (submesh < 0 || submesh >= subMeshCount)
		{
			throw new IndexOutOfRangeException("Specified sub mesh is out of range. Must be greater or equal to 0 and less than subMeshCount.");
		}
		return GetBaseVertexImpl(submesh);
	}

	private void SetTrianglesImpl(int submesh, Array triangles, int arraySize, bool calculateBounds, int baseVertex)
	{
		SetIndicesImpl(submesh, MeshTopology.Triangles, triangles, arraySize, calculateBounds, baseVertex);
	}

	/// <summary>
	///   <para>Sets the triangle list for the sub-mesh.</para>
	/// </summary>
	/// <param name="triangles">The list of indices that define the triangles.</param>
	/// <param name="submesh">The sub-mesh to modify.</param>
	/// <param name="calculateBounds">Calculate the bounding box of the Mesh after setting the triangles. This is done by default.
	/// Use false when you want to use the existing bounding box and reduce the CPU cost of setting the triangles.</param>
	/// <param name="baseVertex">Optional vertex offset that is added to all triangle vertex indices.</param>
	public void SetTriangles(int[] triangles, int submesh)
	{
		SetTriangles(triangles, submesh, calculateBounds: true, 0);
	}

	public void SetTriangles(int[] triangles, int submesh, bool calculateBounds)
	{
		SetTriangles(triangles, submesh, calculateBounds, 0);
	}

	/// <summary>
	///   <para>Sets the triangle list for the sub-mesh.</para>
	/// </summary>
	/// <param name="triangles">The list of indices that define the triangles.</param>
	/// <param name="submesh">The sub-mesh to modify.</param>
	/// <param name="calculateBounds">Calculate the bounding box of the Mesh after setting the triangles. This is done by default.
	/// Use false when you want to use the existing bounding box and reduce the CPU cost of setting the triangles.</param>
	/// <param name="baseVertex">Optional vertex offset that is added to all triangle vertex indices.</param>
	public void SetTriangles(int[] triangles, int submesh, [UnityEngine.Internal.DefaultValue("true")] bool calculateBounds, [UnityEngine.Internal.DefaultValue("0")] int baseVertex)
	{
		if (CheckCanAccessSubmeshTriangles(submesh))
		{
			SetTrianglesImpl(submesh, triangles, NoAllocHelpers.SafeLength(triangles), calculateBounds, baseVertex);
		}
	}

	public void SetTriangles(List<int> triangles, int submesh)
	{
		SetTriangles(triangles, submesh, calculateBounds: true, 0);
	}

	public void SetTriangles(List<int> triangles, int submesh, bool calculateBounds)
	{
		SetTriangles(triangles, submesh, calculateBounds, 0);
	}

	public void SetTriangles(List<int> triangles, int submesh, [UnityEngine.Internal.DefaultValue("true")] bool calculateBounds, [UnityEngine.Internal.DefaultValue("0")] int baseVertex)
	{
		if (CheckCanAccessSubmeshTriangles(submesh))
		{
			SetTrianglesImpl(submesh, NoAllocHelpers.ExtractArrayFromList(triangles), NoAllocHelpers.SafeLength(triangles), calculateBounds, baseVertex);
		}
	}

	/// <summary>
	///   <para>Sets the index buffer for the sub-mesh.</para>
	/// </summary>
	/// <param name="indices">The array of indices that define the Mesh.</param>
	/// <param name="topology">The topology of the Mesh, e.g: Triangles, Lines, Quads, Points, etc. See MeshTopology.</param>
	/// <param name="submesh">The sub-mesh to modify.</param>
	/// <param name="calculateBounds">Calculate the bounding box of the Mesh after setting the indices. This is done by default.
	/// Use false when you want to use the existing bounding box and reduce the CPU cost of setting the indices.</param>
	/// <param name="baseVertex">Optional vertex offset that is added to all triangle vertex indices.</param>
	public void SetIndices(int[] indices, MeshTopology topology, int submesh)
	{
		SetIndices(indices, topology, submesh, calculateBounds: true, 0);
	}

	/// <summary>
	///   <para>Sets the index buffer for the sub-mesh.</para>
	/// </summary>
	/// <param name="indices">The array of indices that define the Mesh.</param>
	/// <param name="topology">The topology of the Mesh, e.g: Triangles, Lines, Quads, Points, etc. See MeshTopology.</param>
	/// <param name="submesh">The sub-mesh to modify.</param>
	/// <param name="calculateBounds">Calculate the bounding box of the Mesh after setting the indices. This is done by default.
	/// Use false when you want to use the existing bounding box and reduce the CPU cost of setting the indices.</param>
	/// <param name="baseVertex">Optional vertex offset that is added to all triangle vertex indices.</param>
	public void SetIndices(int[] indices, MeshTopology topology, int submesh, bool calculateBounds)
	{
		SetIndices(indices, topology, submesh, calculateBounds, 0);
	}

	/// <summary>
	///   <para>Sets the index buffer for the sub-mesh.</para>
	/// </summary>
	/// <param name="indices">The array of indices that define the Mesh.</param>
	/// <param name="topology">The topology of the Mesh, e.g: Triangles, Lines, Quads, Points, etc. See MeshTopology.</param>
	/// <param name="submesh">The sub-mesh to modify.</param>
	/// <param name="calculateBounds">Calculate the bounding box of the Mesh after setting the indices. This is done by default.
	/// Use false when you want to use the existing bounding box and reduce the CPU cost of setting the indices.</param>
	/// <param name="baseVertex">Optional vertex offset that is added to all triangle vertex indices.</param>
	public void SetIndices(int[] indices, MeshTopology topology, int submesh, [UnityEngine.Internal.DefaultValue("true")] bool calculateBounds, [UnityEngine.Internal.DefaultValue("0")] int baseVertex)
	{
		if (CheckCanAccessSubmeshIndices(submesh))
		{
			SetIndicesImpl(submesh, topology, indices, NoAllocHelpers.SafeLength(indices), calculateBounds, baseVertex);
		}
	}

	public void GetBindposes(List<Matrix4x4> bindposes)
	{
		if (bindposes == null)
		{
			throw new ArgumentNullException("The result bindposes list cannot be null.", "bindposes");
		}
		NoAllocHelpers.EnsureListElemCount(bindposes, GetBindposeCount());
		GetBindposesNonAllocImpl(NoAllocHelpers.ExtractArrayFromListT(bindposes));
	}

	public void GetBoneWeights(List<BoneWeight> boneWeights)
	{
		if (boneWeights == null)
		{
			throw new ArgumentNullException("The result boneWeights list cannot be null.", "boneWeights");
		}
		NoAllocHelpers.EnsureListElemCount(boneWeights, GetBoneWeightCount());
		GetBoneWeightsNonAllocImpl(NoAllocHelpers.ExtractArrayFromListT(boneWeights));
	}

	/// <summary>
	///   <para>Clears all vertex data and all triangle indices.</para>
	/// </summary>
	/// <param name="keepVertexLayout"></param>
	public void Clear(bool keepVertexLayout)
	{
		ClearImpl(keepVertexLayout);
	}

	public void Clear()
	{
		ClearImpl(keepVertexLayout: true);
	}

	/// <summary>
	///   <para>Recalculate the bounding volume of the Mesh from the vertices.</para>
	/// </summary>
	public void RecalculateBounds()
	{
		if (canAccess)
		{
			RecalculateBoundsImpl();
		}
		else
		{
			Debug.LogError($"Not allowed to call RecalculateBounds() on mesh '{base.name}'");
		}
	}

	/// <summary>
	///   <para>Recalculates the normals of the Mesh from the triangles and vertices.</para>
	/// </summary>
	public void RecalculateNormals()
	{
		if (canAccess)
		{
			RecalculateNormalsImpl();
		}
		else
		{
			Debug.LogError($"Not allowed to call RecalculateNormals() on mesh '{base.name}'");
		}
	}

	/// <summary>
	///   <para>Recalculates the tangents of the Mesh from the normals and texture coordinates.</para>
	/// </summary>
	public void RecalculateTangents()
	{
		if (canAccess)
		{
			RecalculateTangentsImpl();
		}
		else
		{
			Debug.LogError($"Not allowed to call RecalculateTangents() on mesh '{base.name}'");
		}
	}

	/// <summary>
	///   <para>Optimize mesh for frequent updates.</para>
	/// </summary>
	public void MarkDynamic()
	{
		if (canAccess)
		{
			MarkDynamicImpl();
		}
	}

	/// <summary>
	///   <para>Upload previously done Mesh modifications to the graphics API.</para>
	/// </summary>
	/// <param name="markNoLongerReadable">Frees up system memory copy of mesh data when set to true.</param>
	public void UploadMeshData(bool markNoLongerReadable)
	{
		if (canAccess)
		{
			UploadMeshDataImpl(markNoLongerReadable);
		}
	}

	/// <summary>
	///   <para>Gets the topology of a sub-mesh.</para>
	/// </summary>
	/// <param name="submesh"></param>
	public MeshTopology GetTopology(int submesh)
	{
		if (submesh < 0 || submesh >= subMeshCount)
		{
			Debug.LogError($"Failed getting topology. Submesh index is out of bounds.", this);
			return MeshTopology.Triangles;
		}
		return GetTopologyImpl(submesh);
	}

	/// <summary>
	///   <para>Combines several Meshes into this Mesh.</para>
	/// </summary>
	/// <param name="combine">Descriptions of the Meshes to combine.</param>
	/// <param name="mergeSubMeshes">Defines whether Meshes should be combined into a single sub-mesh.</param>
	/// <param name="useMatrices">Defines whether the transforms supplied in the CombineInstance array should be used or ignored.</param>
	/// <param name="hasLightmapData"></param>
	public void CombineMeshes(CombineInstance[] combine, [UnityEngine.Internal.DefaultValue("true")] bool mergeSubMeshes, [UnityEngine.Internal.DefaultValue("true")] bool useMatrices, [UnityEngine.Internal.DefaultValue("false")] bool hasLightmapData)
	{
		CombineMeshesImpl(combine, mergeSubMeshes, useMatrices, hasLightmapData);
	}

	public void CombineMeshes(CombineInstance[] combine, bool mergeSubMeshes, bool useMatrices)
	{
		CombineMeshesImpl(combine, mergeSubMeshes, useMatrices, hasLightmapData: false);
	}

	public void CombineMeshes(CombineInstance[] combine, bool mergeSubMeshes)
	{
		CombineMeshesImpl(combine, mergeSubMeshes, useMatrices: true, hasLightmapData: false);
	}

	public void CombineMeshes(CombineInstance[] combine)
	{
		CombineMeshesImpl(combine, mergeSubMeshes: true, useMatrices: true, hasLightmapData: false);
	}

	/// <summary>
	///   <para>Optimizes the Mesh for display.</para>
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("This method is no longer supported (UnityUpgradable)", true)]
	public void Optimize()
	{
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_bounds_Injected(out Bounds ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_bounds_Injected(ref Bounds value);
}

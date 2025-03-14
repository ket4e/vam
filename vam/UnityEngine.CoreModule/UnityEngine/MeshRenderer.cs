using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

/// <summary>
///   <para>Renders meshes inserted by the MeshFilter or TextMesh.</para>
/// </summary>
[NativeHeader("Runtime/Graphics/Mesh/MeshRenderer.h")]
public class MeshRenderer : Renderer
{
	/// <summary>
	///   <para>Vertex attributes in this mesh will override or add attributes of the primary mesh in the MeshRenderer.</para>
	/// </summary>
	public extern Mesh additionalVertexStreams
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Index of the first sub-mesh to use from the Mesh associated with this MeshRenderer (Read Only).</para>
	/// </summary>
	public extern int subMeshStartIndex
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetSubMeshStartIndex")]
		get;
	}
}

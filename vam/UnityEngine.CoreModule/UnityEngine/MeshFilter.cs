using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

/// <summary>
///   <para>A class to access the Mesh of the.</para>
/// </summary>
[RequireComponent(typeof(Transform))]
[NativeHeader("Runtime/Graphics/Mesh/MeshFilter.h")]
public sealed class MeshFilter : Component
{
	/// <summary>
	///   <para>Returns the shared mesh of the mesh filter.</para>
	/// </summary>
	public extern Mesh sharedMesh
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Returns the instantiated Mesh assigned to the mesh filter.</para>
	/// </summary>
	public extern Mesh mesh
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetInstantiatedMeshFromScript")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("SetInstantiatedMesh")]
		set;
	}
}

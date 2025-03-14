using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>A mesh collider allows you to do between meshes and primitives.</para>
/// </summary>
[RequiredByNativeCode]
public sealed class MeshCollider : Collider
{
	/// <summary>
	///   <para>The mesh object used for collision detection.</para>
	/// </summary>
	public extern Mesh sharedMesh
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Use a convex collider from the mesh.</para>
	/// </summary>
	public extern bool convex
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Options used to enable or disable certain features in mesh cooking.</para>
	/// </summary>
	public extern MeshColliderCookingOptions cookingOptions
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Allow the physics engine to increase the volume of the input mesh in attempt to generate a valid convex mesh.</para>
	/// </summary>
	public bool inflateMesh
	{
		get
		{
			return (cookingOptions & MeshColliderCookingOptions.InflateConvexMesh) != 0;
		}
		set
		{
			MeshColliderCookingOptions meshColliderCookingOptions = cookingOptions & ~MeshColliderCookingOptions.InflateConvexMesh;
			if (value)
			{
				meshColliderCookingOptions |= MeshColliderCookingOptions.InflateConvexMesh;
			}
			cookingOptions = meshColliderCookingOptions;
		}
	}

	/// <summary>
	///   <para>Used when set to inflateMesh to determine how much inflation is acceptable.</para>
	/// </summary>
	public extern float skinWidth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Uses interpolated normals for sphere collisions instead of flat polygonal normals.</para>
	/// </summary>
	[Obsolete("Configuring smooth sphere collisions is no longer needed. PhysX3 has a better behaviour in place.")]
	public bool smoothSphereCollisions
	{
		get
		{
			return true;
		}
		set
		{
		}
	}
}

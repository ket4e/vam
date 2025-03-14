using System;
using System.Runtime.CompilerServices;
using Unity.Jobs;
using UnityEngine.Bindings;

namespace UnityEngine.Experimental.AI;

/// <summary>
///   <para>Assembles together a collection of NavMesh surfaces and links that are used as a whole for performing navigation operations.</para>
/// </summary>
[StaticAccessor("NavMeshWorldBindings", StaticAccessorType.DoubleColon)]
public struct NavMeshWorld
{
	internal IntPtr world;

	/// <summary>
	///   <para>Returns true if the NavMeshWorld has been properly initialized.</para>
	/// </summary>
	public bool IsValid()
	{
		return world != IntPtr.Zero;
	}

	/// <summary>
	///   <para>Returns a reference to the single NavMeshWorld that can currently exist and be used in Unity.</para>
	/// </summary>
	public static NavMeshWorld GetDefaultWorld()
	{
		GetDefaultWorld_Injected(out var ret);
		return ret;
	}

	private static void AddDependencyInternal(IntPtr navmesh, JobHandle handle)
	{
		AddDependencyInternal_Injected(navmesh, ref handle);
	}

	/// <summary>
	///   <para>Tells the NavMesh world to halt any changes until the specified job is completed.</para>
	/// </summary>
	/// <param name="job">The job that needs to be completed before the NavMesh world can be modified in any way.</param>
	public void AddDependency(JobHandle job)
	{
		AddDependencyInternal(world, job);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetDefaultWorld_Injected(out NavMeshWorld ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddDependencyInternal_Injected(IntPtr navmesh, ref JobHandle handle);
}

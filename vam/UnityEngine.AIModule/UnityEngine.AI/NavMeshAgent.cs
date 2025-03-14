using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.AI;

/// <summary>
///   <para>Navigation mesh agent.</para>
/// </summary>
[MovedFrom("UnityEngine")]
public sealed class NavMeshAgent : Behaviour
{
	/// <summary>
	///   <para>Gets or attempts to set the destination of the agent in world-space units.</para>
	/// </summary>
	public Vector3 destination
	{
		get
		{
			INTERNAL_get_destination(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_destination(ref value);
		}
	}

	/// <summary>
	///   <para>Stop within this distance from the target position.</para>
	/// </summary>
	public extern float stoppingDistance
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Access the current velocity of the NavMeshAgent component, or set a velocity to control the agent manually.</para>
	/// </summary>
	public Vector3 velocity
	{
		get
		{
			INTERNAL_get_velocity(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_velocity(ref value);
		}
	}

	/// <summary>
	///   <para>Gets or sets the simulation position of the navmesh agent.</para>
	/// </summary>
	public Vector3 nextPosition
	{
		get
		{
			INTERNAL_get_nextPosition(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_nextPosition(ref value);
		}
	}

	/// <summary>
	///   <para>Get the current steering target along the path. (Read Only)</para>
	/// </summary>
	public Vector3 steeringTarget
	{
		get
		{
			INTERNAL_get_steeringTarget(out var value);
			return value;
		}
	}

	/// <summary>
	///   <para>The desired velocity of the agent including any potential contribution from avoidance. (Read Only)</para>
	/// </summary>
	public Vector3 desiredVelocity
	{
		get
		{
			INTERNAL_get_desiredVelocity(out var value);
			return value;
		}
	}

	/// <summary>
	///   <para>The distance between the agent's position and the destination on the current path. (Read Only)</para>
	/// </summary>
	public extern float remainingDistance
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>The relative vertical displacement of the owning GameObject.</para>
	/// </summary>
	public extern float baseOffset
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Is the agent currently positioned on an OffMeshLink? (Read Only)</para>
	/// </summary>
	public extern bool isOnOffMeshLink
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>The current OffMeshLinkData.</para>
	/// </summary>
	public OffMeshLinkData currentOffMeshLinkData => GetCurrentOffMeshLinkDataInternal();

	/// <summary>
	///   <para>The next OffMeshLinkData on the current path.</para>
	/// </summary>
	public OffMeshLinkData nextOffMeshLinkData => GetNextOffMeshLinkDataInternal();

	/// <summary>
	///   <para>Should the agent move across OffMeshLinks automatically?</para>
	/// </summary>
	public extern bool autoTraverseOffMeshLink
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Should the agent brake automatically to avoid overshooting the destination point?</para>
	/// </summary>
	public extern bool autoBraking
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Should the agent attempt to acquire a new path if the existing path becomes invalid?</para>
	/// </summary>
	public extern bool autoRepath
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Does the agent currently have a path? (Read Only)</para>
	/// </summary>
	public extern bool hasPath
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Is a path in the process of being computed but not yet ready? (Read Only)</para>
	/// </summary>
	public extern bool pathPending
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Is the current path stale. (Read Only)</para>
	/// </summary>
	public extern bool isPathStale
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>The status of the current path (complete, partial or invalid).</para>
	/// </summary>
	public extern NavMeshPathStatus pathStatus
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public Vector3 pathEndPosition
	{
		get
		{
			INTERNAL_get_pathEndPosition(out var value);
			return value;
		}
	}

	/// <summary>
	///   <para>This property holds the stop or resume condition of the NavMesh agent.</para>
	/// </summary>
	public extern bool isStopped
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Property to get and set the current path.</para>
	/// </summary>
	public NavMeshPath path
	{
		get
		{
			NavMeshPath result = new NavMeshPath();
			CopyPathTo(result);
			return result;
		}
		set
		{
			if (value == null)
			{
				throw new NullReferenceException();
			}
			SetPath(value);
		}
	}

	/// <summary>
	///   <para>Returns the owning object of the NavMesh the agent is currently placed on (Read Only).</para>
	/// </summary>
	public Object navMeshOwner => GetOwnerInternal();

	/// <summary>
	///   <para>The type ID for the agent.</para>
	/// </summary>
	public extern int agentTypeID
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Specifies which NavMesh layers are passable (bitfield). Changing walkableMask will make the path stale (see isPathStale).</para>
	/// </summary>
	[Obsolete("Use areaMask instead.")]
	public extern int walkableMask
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Specifies which NavMesh areas are passable. Changing areaMask will make the path stale (see isPathStale).</para>
	/// </summary>
	public extern int areaMask
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Maximum movement speed when following a path.</para>
	/// </summary>
	public extern float speed
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Maximum turning speed in (deg/s) while following a path.</para>
	/// </summary>
	public extern float angularSpeed
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The maximum acceleration of an agent as it follows a path, given in units / sec^2.</para>
	/// </summary>
	public extern float acceleration
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Gets or sets whether the transform position is synchronized with the simulated agent position. The default value is true.</para>
	/// </summary>
	public extern bool updatePosition
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Should the agent update the transform orientation?</para>
	/// </summary>
	public extern bool updateRotation
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Allows you to specify whether the agent should be aligned to the up-axis of the NavMesh or link that it is placed on.</para>
	/// </summary>
	public extern bool updateUpAxis
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The avoidance radius for the agent.</para>
	/// </summary>
	public extern float radius
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The height of the agent for purposes of passing under obstacles, etc.</para>
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
	///   <para>The level of quality of avoidance.</para>
	/// </summary>
	public extern ObstacleAvoidanceType obstacleAvoidanceType
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The avoidance priority level.</para>
	/// </summary>
	public extern int avoidancePriority
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Is the agent currently bound to the navmesh? (Read Only)</para>
	/// </summary>
	public extern bool isOnNavMesh
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Sets or updates the destination thus triggering the calculation for a new path.</para>
	/// </summary>
	/// <param name="target">The target point to navigate to.</param>
	/// <returns>
	///   <para>True if the destination was requested successfully, otherwise false.</para>
	/// </returns>
	public bool SetDestination(Vector3 target)
	{
		return INTERNAL_CALL_SetDestination(this, ref target);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool INTERNAL_CALL_SetDestination(NavMeshAgent self, ref Vector3 target);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_destination(out Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_destination(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_velocity(out Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_velocity(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_nextPosition(out Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_nextPosition(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_steeringTarget(out Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_desiredVelocity(out Vector3 value);

	/// <summary>
	///   <para>Enables or disables the current off-mesh link.</para>
	/// </summary>
	/// <param name="activated">Is the link activated?</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void ActivateCurrentOffMeshLink(bool activated);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern OffMeshLinkData GetCurrentOffMeshLinkDataInternal();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern OffMeshLinkData GetNextOffMeshLinkDataInternal();

	/// <summary>
	///   <para>Completes the movement on the current OffMeshLink.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void CompleteOffMeshLink();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_pathEndPosition(out Vector3 value);

	/// <summary>
	///   <para>Warps agent to the provided position.</para>
	/// </summary>
	/// <param name="newPosition">New position to warp the agent to.</param>
	/// <returns>
	///   <para>True if agent is successfully warped, otherwise false.</para>
	/// </returns>
	public bool Warp(Vector3 newPosition)
	{
		return INTERNAL_CALL_Warp(this, ref newPosition);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool INTERNAL_CALL_Warp(NavMeshAgent self, ref Vector3 newPosition);

	/// <summary>
	///   <para>Apply relative movement to current position.</para>
	/// </summary>
	/// <param name="offset">The relative movement vector.</param>
	public void Move(Vector3 offset)
	{
		INTERNAL_CALL_Move(this, ref offset);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Move(NavMeshAgent self, ref Vector3 offset);

	/// <summary>
	///   <para>Stop movement of this agent along its current path.</para>
	/// </summary>
	[Obsolete("Set isStopped to true instead")]
	public void Stop()
	{
		StopInternal();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern void StopInternal();

	[Obsolete("Set isStopped to true instead")]
	public void Stop(bool stopUpdates)
	{
		StopInternal();
	}

	/// <summary>
	///   <para>Resumes the movement along the current path after a pause.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[Obsolete("Set isStopped to false instead")]
	[GeneratedByOldBindingsGenerator]
	public extern void Resume();

	/// <summary>
	///   <para>Clears the current path.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void ResetPath();

	/// <summary>
	///   <para>Assign a new path to this agent.</para>
	/// </summary>
	/// <param name="path">New path to follow.</param>
	/// <returns>
	///   <para>True if the path is succesfully assigned.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern bool SetPath(NavMeshPath path);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal extern void CopyPathTo(NavMeshPath path);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern bool FindClosestEdge(out NavMeshHit hit);

	public bool Raycast(Vector3 targetPosition, out NavMeshHit hit)
	{
		return INTERNAL_CALL_Raycast(this, ref targetPosition, out hit);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool INTERNAL_CALL_Raycast(NavMeshAgent self, ref Vector3 targetPosition, out NavMeshHit hit);

	/// <summary>
	///   <para>Calculate a path to a specified point and store the resulting path.</para>
	/// </summary>
	/// <param name="targetPosition">The final position of the path requested.</param>
	/// <param name="path">The resulting path.</param>
	/// <returns>
	///   <para>True if a path is found.</para>
	/// </returns>
	public bool CalculatePath(Vector3 targetPosition, NavMeshPath path)
	{
		path.ClearCorners();
		return CalculatePathInternal(targetPosition, path);
	}

	private bool CalculatePathInternal(Vector3 targetPosition, NavMeshPath path)
	{
		return INTERNAL_CALL_CalculatePathInternal(this, ref targetPosition, path);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool INTERNAL_CALL_CalculatePathInternal(NavMeshAgent self, ref Vector3 targetPosition, NavMeshPath path);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern bool SamplePathPosition(int areaMask, float maxDistance, out NavMeshHit hit);

	/// <summary>
	///   <para>Sets the cost for traversing over geometry of the layer type.</para>
	/// </summary>
	/// <param name="layer">Layer index.</param>
	/// <param name="cost">New cost for the specified layer.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[Obsolete("Use SetAreaCost instead.")]
	[GeneratedByOldBindingsGenerator]
	public extern void SetLayerCost(int layer, float cost);

	/// <summary>
	///   <para>Gets the cost for crossing ground of a particular type.</para>
	/// </summary>
	/// <param name="layer">Layer index.</param>
	/// <returns>
	///   <para>Current cost of specified layer.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[Obsolete("Use GetAreaCost instead.")]
	public extern float GetLayerCost(int layer);

	/// <summary>
	///   <para>Sets the cost for traversing over areas of the area type.</para>
	/// </summary>
	/// <param name="areaIndex">Area cost.</param>
	/// <param name="areaCost">New cost for the specified area index.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void SetAreaCost(int areaIndex, float areaCost);

	/// <summary>
	///   <para>Gets the cost for path calculation when crossing area of a particular type.</para>
	/// </summary>
	/// <param name="areaIndex">Area Index.</param>
	/// <returns>
	///   <para>Current cost for specified area index.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern float GetAreaCost(int areaIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern Object GetOwnerInternal();
}

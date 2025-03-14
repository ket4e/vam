using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>A set of options that control how physics operates when using the job system to multithread the physics simulation.</para>
/// </summary>
[UsedByNativeCode]
public struct PhysicsJobOptions2D
{
	private bool m_UseMultithreading;

	private bool m_UseConsistencySorting;

	private int m_InterpolationPosesPerJob;

	private int m_NewContactsPerJob;

	private int m_CollideContactsPerJob;

	private int m_ClearFlagsPerJob;

	private int m_ClearBodyForcesPerJob;

	private int m_SyncDiscreteFixturesPerJob;

	private int m_SyncContinuousFixturesPerJob;

	private int m_FindNearestContactsPerJob;

	private int m_UpdateTriggerContactsPerJob;

	private int m_IslandSolverCostThreshold;

	private int m_IslandSolverBodyCostScale;

	private int m_IslandSolverContactCostScale;

	private int m_IslandSolverJointCostScale;

	private int m_IslandSolverBodiesPerJob;

	private int m_IslandSolverContactsPerJob;

	/// <summary>
	///   <para>Should physics simulation use multithreading?</para>
	/// </summary>
	public bool useMultithreading
	{
		get
		{
			return m_UseMultithreading;
		}
		set
		{
			m_UseMultithreading = value;
		}
	}

	/// <summary>
	///   <para>Should physics simulation sort multi-threaded results to maintain processing order consistency?</para>
	/// </summary>
	public bool useConsistencySorting
	{
		get
		{
			return m_UseConsistencySorting;
		}
		set
		{
			m_UseConsistencySorting = value;
		}
	}

	/// <summary>
	///   <para>Controls the minimum number of Rigidbody2D being interpolated in each simulation job.</para>
	/// </summary>
	public int interpolationPosesPerJob
	{
		get
		{
			return m_InterpolationPosesPerJob;
		}
		set
		{
			m_InterpolationPosesPerJob = value;
		}
	}

	/// <summary>
	///   <para>Controls the minimum number of new contacts to find in each simulation job.</para>
	/// </summary>
	public int newContactsPerJob
	{
		get
		{
			return m_NewContactsPerJob;
		}
		set
		{
			m_NewContactsPerJob = value;
		}
	}

	/// <summary>
	///   <para>Controls the minimum number of contacts to collide in each simulation job.</para>
	/// </summary>
	public int collideContactsPerJob
	{
		get
		{
			return m_CollideContactsPerJob;
		}
		set
		{
			m_CollideContactsPerJob = value;
		}
	}

	/// <summary>
	///   <para>Controls the minimum number of flags to be cleared in each simulation job.</para>
	/// </summary>
	public int clearFlagsPerJob
	{
		get
		{
			return m_ClearFlagsPerJob;
		}
		set
		{
			m_ClearFlagsPerJob = value;
		}
	}

	/// <summary>
	///   <para>Controls the minimum number of bodies to be cleared in each simulation job.</para>
	/// </summary>
	public int clearBodyForcesPerJob
	{
		get
		{
			return m_ClearBodyForcesPerJob;
		}
		set
		{
			m_ClearBodyForcesPerJob = value;
		}
	}

	/// <summary>
	///   <para>Controls the minimum number of fixtures to synchronize in the broadphase during discrete island solving in each simulation job.</para>
	/// </summary>
	public int syncDiscreteFixturesPerJob
	{
		get
		{
			return m_SyncDiscreteFixturesPerJob;
		}
		set
		{
			m_SyncDiscreteFixturesPerJob = value;
		}
	}

	/// <summary>
	///   <para>Controls the minimum number of fixtures to synchronize in the broadphase during continuous island solving in each simulation job.</para>
	/// </summary>
	public int syncContinuousFixturesPerJob
	{
		get
		{
			return m_SyncContinuousFixturesPerJob;
		}
		set
		{
			m_SyncContinuousFixturesPerJob = value;
		}
	}

	/// <summary>
	///   <para>Controls the minimum number of nearest contacts to find in each simulation job.</para>
	/// </summary>
	public int findNearestContactsPerJob
	{
		get
		{
			return m_FindNearestContactsPerJob;
		}
		set
		{
			m_FindNearestContactsPerJob = value;
		}
	}

	/// <summary>
	///   <para>Controls the minimum number of trigger contacts to update in each simulation job.</para>
	/// </summary>
	public int updateTriggerContactsPerJob
	{
		get
		{
			return m_UpdateTriggerContactsPerJob;
		}
		set
		{
			m_UpdateTriggerContactsPerJob = value;
		}
	}

	/// <summary>
	///   <para>The minimum threshold cost of all bodies, contacts and joints in an island during discrete island solving.</para>
	/// </summary>
	public int islandSolverCostThreshold
	{
		get
		{
			return m_IslandSolverCostThreshold;
		}
		set
		{
			m_IslandSolverCostThreshold = value;
		}
	}

	/// <summary>
	///   <para>Scales the cost of each body during discrete island solving.</para>
	/// </summary>
	public int islandSolverBodyCostScale
	{
		get
		{
			return m_IslandSolverBodyCostScale;
		}
		set
		{
			m_IslandSolverBodyCostScale = value;
		}
	}

	/// <summary>
	///   <para>Scales the cost of each contact during discrete island solving.</para>
	/// </summary>
	public int islandSolverContactCostScale
	{
		get
		{
			return m_IslandSolverContactCostScale;
		}
		set
		{
			m_IslandSolverContactCostScale = value;
		}
	}

	/// <summary>
	///   <para>Scales the cost of each joint during discrete island solving.</para>
	/// </summary>
	public int islandSolverJointCostScale
	{
		get
		{
			return m_IslandSolverJointCostScale;
		}
		set
		{
			m_IslandSolverJointCostScale = value;
		}
	}

	/// <summary>
	///   <para>Controls the minimum number of bodies to solve in each simulation job when performing island solving.</para>
	/// </summary>
	public int islandSolverBodiesPerJob
	{
		get
		{
			return m_IslandSolverBodiesPerJob;
		}
		set
		{
			m_IslandSolverBodiesPerJob = value;
		}
	}

	/// <summary>
	///   <para>Controls the minimum number of contacts to solve in each simulation job when performing island solving.</para>
	/// </summary>
	public int islandSolverContactsPerJob
	{
		get
		{
			return m_IslandSolverContactsPerJob;
		}
		set
		{
			m_IslandSolverContactsPerJob = value;
		}
	}
}

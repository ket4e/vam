using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.AI;

/// <summary>
///   <para>The NavMeshBuildSettings struct allows you to specify a collection of settings which describe the dimensions and limitations of a particular agent type.</para>
/// </summary>
public struct NavMeshBuildSettings
{
	private int m_AgentTypeID;

	private float m_AgentRadius;

	private float m_AgentHeight;

	private float m_AgentSlope;

	private float m_AgentClimb;

	private float m_LedgeDropHeight;

	private float m_MaxJumpAcrossDistance;

	private float m_MinRegionArea;

	private int m_OverrideVoxelSize;

	private float m_VoxelSize;

	private int m_OverrideTileSize;

	private int m_TileSize;

	private int m_AccuratePlacement;

	private NavMeshBuildDebugSettings m_Debug;

	/// <summary>
	///   <para>The agent type ID the NavMesh will be baked for.</para>
	/// </summary>
	public int agentTypeID
	{
		get
		{
			return m_AgentTypeID;
		}
		set
		{
			m_AgentTypeID = value;
		}
	}

	/// <summary>
	///   <para>The radius of the agent for baking in world units.</para>
	/// </summary>
	public float agentRadius
	{
		get
		{
			return m_AgentRadius;
		}
		set
		{
			m_AgentRadius = value;
		}
	}

	/// <summary>
	///   <para>The height of the agent for baking in world units.</para>
	/// </summary>
	public float agentHeight
	{
		get
		{
			return m_AgentHeight;
		}
		set
		{
			m_AgentHeight = value;
		}
	}

	/// <summary>
	///   <para>The maximum slope angle which is walkable (angle in degrees).</para>
	/// </summary>
	public float agentSlope
	{
		get
		{
			return m_AgentSlope;
		}
		set
		{
			m_AgentSlope = value;
		}
	}

	/// <summary>
	///   <para>The maximum vertical step size an agent can take.</para>
	/// </summary>
	public float agentClimb
	{
		get
		{
			return m_AgentClimb;
		}
		set
		{
			m_AgentClimb = value;
		}
	}

	/// <summary>
	///   <para>The approximate minimum area of individual NavMesh regions.</para>
	/// </summary>
	public float minRegionArea
	{
		get
		{
			return m_MinRegionArea;
		}
		set
		{
			m_MinRegionArea = value;
		}
	}

	/// <summary>
	///   <para>Enables overriding the default voxel size. See Also: voxelSize.</para>
	/// </summary>
	public bool overrideVoxelSize
	{
		get
		{
			return m_OverrideVoxelSize != 0;
		}
		set
		{
			m_OverrideVoxelSize = (value ? 1 : 0);
		}
	}

	/// <summary>
	///   <para>Sets the voxel size in world length units.</para>
	/// </summary>
	public float voxelSize
	{
		get
		{
			return m_VoxelSize;
		}
		set
		{
			m_VoxelSize = value;
		}
	}

	/// <summary>
	///   <para>Enables overriding the default tile size. See Also: tileSize.</para>
	/// </summary>
	public bool overrideTileSize
	{
		get
		{
			return m_OverrideTileSize != 0;
		}
		set
		{
			m_OverrideTileSize = (value ? 1 : 0);
		}
	}

	/// <summary>
	///   <para>Sets the tile size in voxel units.</para>
	/// </summary>
	public int tileSize
	{
		get
		{
			return m_TileSize;
		}
		set
		{
			m_TileSize = value;
		}
	}

	/// <summary>
	///   <para>Options for collecting debug data during the build process.</para>
	/// </summary>
	public NavMeshBuildDebugSettings debug
	{
		get
		{
			return m_Debug;
		}
		set
		{
			m_Debug = value;
		}
	}

	/// <summary>
	///   <para>Validates the properties of NavMeshBuildSettings.</para>
	/// </summary>
	/// <param name="buildBounds">Describes the volume to build NavMesh for.</param>
	/// <returns>
	///   <para>The list of violated constraints.</para>
	/// </returns>
	public string[] ValidationReport(Bounds buildBounds)
	{
		return InternalValidationReport(this, buildBounds);
	}

	private static string[] InternalValidationReport(NavMeshBuildSettings buildSettings, Bounds buildBounds)
	{
		return INTERNAL_CALL_InternalValidationReport(ref buildSettings, ref buildBounds);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern string[] INTERNAL_CALL_InternalValidationReport(ref NavMeshBuildSettings buildSettings, ref Bounds buildBounds);
}

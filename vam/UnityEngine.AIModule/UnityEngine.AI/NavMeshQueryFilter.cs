using System;

namespace UnityEngine.AI;

/// <summary>
///   <para>Specifies which agent type and areas to consider when searching the NavMesh.</para>
/// </summary>
public struct NavMeshQueryFilter
{
	private const int AREA_COST_ELEMENT_COUNT = 32;

	private int m_AreaMask;

	private int m_AgentTypeID;

	private float[] m_AreaCost;

	internal float[] costs => m_AreaCost;

	/// <summary>
	///   <para>A bitmask representing the traversable area types.</para>
	/// </summary>
	public int areaMask
	{
		get
		{
			return m_AreaMask;
		}
		set
		{
			m_AreaMask = value;
		}
	}

	/// <summary>
	///   <para>The agent type ID, specifying which navigation meshes to consider for the query functions.</para>
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
	///   <para>Returns the area cost multiplier for the given area type for this filter.</para>
	/// </summary>
	/// <param name="areaIndex">Index to retreive the cost for.</param>
	/// <returns>
	///   <para>The cost multiplier for the supplied area index.</para>
	/// </returns>
	public float GetAreaCost(int areaIndex)
	{
		if (m_AreaCost == null)
		{
			if (areaIndex < 0 || areaIndex >= 32)
			{
				string message = $"The valid range is [0:{31}]";
				throw new IndexOutOfRangeException(message);
			}
			return 1f;
		}
		return m_AreaCost[areaIndex];
	}

	/// <summary>
	///   <para>Sets the pathfinding cost multiplier for this filter for a given area type.</para>
	/// </summary>
	/// <param name="areaIndex">The area index to set the cost for.</param>
	/// <param name="cost">The cost for the supplied area index.</param>
	public void SetAreaCost(int areaIndex, float cost)
	{
		if (m_AreaCost == null)
		{
			m_AreaCost = new float[32];
			for (int i = 0; i < 32; i++)
			{
				m_AreaCost[i] = 1f;
			}
		}
		m_AreaCost[areaIndex] = cost;
	}
}

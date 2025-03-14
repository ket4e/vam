namespace UnityEngine.AI;

/// <summary>
///   <para>Used for runtime manipulation of links connecting polygons of the NavMesh.</para>
/// </summary>
public struct NavMeshLinkData
{
	private Vector3 m_StartPosition;

	private Vector3 m_EndPosition;

	private float m_CostModifier;

	private int m_Bidirectional;

	private float m_Width;

	private int m_Area;

	private int m_AgentTypeID;

	/// <summary>
	///   <para>Start position of the link.</para>
	/// </summary>
	public Vector3 startPosition
	{
		get
		{
			return m_StartPosition;
		}
		set
		{
			m_StartPosition = value;
		}
	}

	/// <summary>
	///   <para>End position of the link.</para>
	/// </summary>
	public Vector3 endPosition
	{
		get
		{
			return m_EndPosition;
		}
		set
		{
			m_EndPosition = value;
		}
	}

	/// <summary>
	///   <para>If positive, overrides the pathfinder cost to traverse the link.</para>
	/// </summary>
	public float costModifier
	{
		get
		{
			return m_CostModifier;
		}
		set
		{
			m_CostModifier = value;
		}
	}

	/// <summary>
	///   <para>If true, the link can be traversed in both directions, otherwise only from start to end position.</para>
	/// </summary>
	public bool bidirectional
	{
		get
		{
			return m_Bidirectional != 0;
		}
		set
		{
			m_Bidirectional = (value ? 1 : 0);
		}
	}

	/// <summary>
	///   <para>If positive, the link will be rectangle aligned along the line from start to end.</para>
	/// </summary>
	public float width
	{
		get
		{
			return m_Width;
		}
		set
		{
			m_Width = value;
		}
	}

	/// <summary>
	///   <para>Area type of the link.</para>
	/// </summary>
	public int area
	{
		get
		{
			return m_Area;
		}
		set
		{
			m_Area = value;
		}
	}

	/// <summary>
	///   <para>Specifies which agent type this link is available for.</para>
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
}

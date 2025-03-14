using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.AI;

/// <summary>
///   <para>Result information for NavMesh queries.</para>
/// </summary>
[MovedFrom("UnityEngine")]
public struct NavMeshHit
{
	private Vector3 m_Position;

	private Vector3 m_Normal;

	private float m_Distance;

	private int m_Mask;

	private int m_Hit;

	/// <summary>
	///   <para>Position of hit.</para>
	/// </summary>
	public Vector3 position
	{
		get
		{
			return m_Position;
		}
		set
		{
			m_Position = value;
		}
	}

	/// <summary>
	///   <para>Normal at the point of hit.</para>
	/// </summary>
	public Vector3 normal
	{
		get
		{
			return m_Normal;
		}
		set
		{
			m_Normal = value;
		}
	}

	/// <summary>
	///   <para>Distance to the point of hit.</para>
	/// </summary>
	public float distance
	{
		get
		{
			return m_Distance;
		}
		set
		{
			m_Distance = value;
		}
	}

	/// <summary>
	///   <para>Mask specifying NavMesh area at point of hit.</para>
	/// </summary>
	public int mask
	{
		get
		{
			return m_Mask;
		}
		set
		{
			m_Mask = value;
		}
	}

	/// <summary>
	///   <para>Flag set when hit.</para>
	/// </summary>
	public bool hit
	{
		get
		{
			return m_Hit != 0;
		}
		set
		{
			m_Hit = (value ? 1 : 0);
		}
	}
}

namespace UnityEngine;

/// <summary>
///   <para>Angular limits on the rotation of a Rigidbody2D object around a HingeJoint2D.</para>
/// </summary>
public struct JointAngleLimits2D
{
	private float m_LowerAngle;

	private float m_UpperAngle;

	/// <summary>
	///   <para>Lower angular limit of rotation.</para>
	/// </summary>
	public float min
	{
		get
		{
			return m_LowerAngle;
		}
		set
		{
			m_LowerAngle = value;
		}
	}

	/// <summary>
	///   <para>Upper angular limit of rotation.</para>
	/// </summary>
	public float max
	{
		get
		{
			return m_UpperAngle;
		}
		set
		{
			m_UpperAngle = value;
		}
	}
}

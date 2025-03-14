namespace UnityEngine;

/// <summary>
///   <para>The configuration of the spring attached to the joint's limits: linear and angular. Used by CharacterJoint and ConfigurableJoint.</para>
/// </summary>
public struct SoftJointLimitSpring
{
	private float m_Spring;

	private float m_Damper;

	/// <summary>
	///   <para>The stiffness of the spring limit. When stiffness is zero the limit is hard, otherwise soft.</para>
	/// </summary>
	public float spring
	{
		get
		{
			return m_Spring;
		}
		set
		{
			m_Spring = value;
		}
	}

	/// <summary>
	///   <para>The damping of the spring limit. In effect when the stiffness of the sprint limit is not zero.</para>
	/// </summary>
	public float damper
	{
		get
		{
			return m_Damper;
		}
		set
		{
			m_Damper = value;
		}
	}
}

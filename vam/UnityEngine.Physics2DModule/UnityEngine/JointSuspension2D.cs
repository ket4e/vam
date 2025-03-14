namespace UnityEngine;

/// <summary>
///   <para>Joint suspension is used to define how suspension works on a WheelJoint2D.</para>
/// </summary>
public struct JointSuspension2D
{
	private float m_DampingRatio;

	private float m_Frequency;

	private float m_Angle;

	/// <summary>
	///   <para>The amount by which the suspension spring force is reduced in proportion to the movement speed.</para>
	/// </summary>
	public float dampingRatio
	{
		get
		{
			return m_DampingRatio;
		}
		set
		{
			m_DampingRatio = value;
		}
	}

	/// <summary>
	///   <para>The frequency at which the suspension spring oscillates.</para>
	/// </summary>
	public float frequency
	{
		get
		{
			return m_Frequency;
		}
		set
		{
			m_Frequency = value;
		}
	}

	/// <summary>
	///   <para>The world angle (in degrees) along which the suspension will move.</para>
	/// </summary>
	public float angle
	{
		get
		{
			return m_Angle;
		}
		set
		{
			m_Angle = value;
		}
	}
}

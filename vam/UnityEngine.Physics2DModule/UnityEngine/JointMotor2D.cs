namespace UnityEngine;

/// <summary>
///   <para>Parameters for the optional motor force applied to a Joint2D.</para>
/// </summary>
public struct JointMotor2D
{
	private float m_MotorSpeed;

	private float m_MaximumMotorTorque;

	/// <summary>
	///   <para>The desired speed for the Rigidbody2D to reach as it moves with the joint.</para>
	/// </summary>
	public float motorSpeed
	{
		get
		{
			return m_MotorSpeed;
		}
		set
		{
			m_MotorSpeed = value;
		}
	}

	/// <summary>
	///   <para>The maximum force that can be applied to the Rigidbody2D at the joint to attain the target speed.</para>
	/// </summary>
	public float maxMotorTorque
	{
		get
		{
			return m_MaximumMotorTorque;
		}
		set
		{
			m_MaximumMotorTorque = value;
		}
	}
}

namespace UnityEngine;

/// <summary>
///   <para>The JointMotor is used to motorize a joint.</para>
/// </summary>
public struct JointMotor
{
	private float m_TargetVelocity;

	private float m_Force;

	private int m_FreeSpin;

	/// <summary>
	///   <para>The motor will apply a force up to force to achieve targetVelocity.</para>
	/// </summary>
	public float targetVelocity
	{
		get
		{
			return m_TargetVelocity;
		}
		set
		{
			m_TargetVelocity = value;
		}
	}

	/// <summary>
	///   <para>The motor will apply a force.</para>
	/// </summary>
	public float force
	{
		get
		{
			return m_Force;
		}
		set
		{
			m_Force = value;
		}
	}

	/// <summary>
	///   <para>If freeSpin is enabled the motor will only accelerate but never slow down.</para>
	/// </summary>
	public bool freeSpin
	{
		get
		{
			return m_FreeSpin == 1;
		}
		set
		{
			m_FreeSpin = (value ? 1 : 0);
		}
	}
}

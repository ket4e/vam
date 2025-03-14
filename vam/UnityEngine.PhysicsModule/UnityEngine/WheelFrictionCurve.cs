namespace UnityEngine;

/// <summary>
///   <para>WheelFrictionCurve is used by the WheelCollider to describe friction properties of the wheel tire.</para>
/// </summary>
public struct WheelFrictionCurve
{
	private float m_ExtremumSlip;

	private float m_ExtremumValue;

	private float m_AsymptoteSlip;

	private float m_AsymptoteValue;

	private float m_Stiffness;

	/// <summary>
	///   <para>Extremum point slip (default 1).</para>
	/// </summary>
	public float extremumSlip
	{
		get
		{
			return m_ExtremumSlip;
		}
		set
		{
			m_ExtremumSlip = value;
		}
	}

	/// <summary>
	///   <para>Force at the extremum slip (default 20000).</para>
	/// </summary>
	public float extremumValue
	{
		get
		{
			return m_ExtremumValue;
		}
		set
		{
			m_ExtremumValue = value;
		}
	}

	/// <summary>
	///   <para>Asymptote point slip (default 2).</para>
	/// </summary>
	public float asymptoteSlip
	{
		get
		{
			return m_AsymptoteSlip;
		}
		set
		{
			m_AsymptoteSlip = value;
		}
	}

	/// <summary>
	///   <para>Force at the asymptote slip (default 10000).</para>
	/// </summary>
	public float asymptoteValue
	{
		get
		{
			return m_AsymptoteValue;
		}
		set
		{
			m_AsymptoteValue = value;
		}
	}

	/// <summary>
	///   <para>Multiplier for the extremumValue and asymptoteValue values (default 1).</para>
	/// </summary>
	public float stiffness
	{
		get
		{
			return m_Stiffness;
		}
		set
		{
			m_Stiffness = value;
		}
	}
}

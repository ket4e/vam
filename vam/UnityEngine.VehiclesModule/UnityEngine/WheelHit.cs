using UnityEngine.Bindings;

namespace UnityEngine;

/// <summary>
///   <para>Contact information for the wheel, reported by WheelCollider.</para>
/// </summary>
[NativeHeader("Runtime/Vehicles/WheelCollider.h")]
public struct WheelHit
{
	[NativeName("point")]
	private Vector3 m_Point;

	[NativeName("normal")]
	private Vector3 m_Normal;

	[NativeName("forwardDir")]
	private Vector3 m_ForwardDir;

	[NativeName("sidewaysDir")]
	private Vector3 m_SidewaysDir;

	[NativeName("force")]
	private float m_Force;

	[NativeName("forwardSlip")]
	private float m_ForwardSlip;

	[NativeName("sidewaysSlip")]
	private float m_SidewaysSlip;

	[NativeName("collider")]
	private Collider m_Collider;

	/// <summary>
	///   <para>The other Collider the wheel is hitting.</para>
	/// </summary>
	public Collider collider
	{
		get
		{
			return m_Collider;
		}
		set
		{
			m_Collider = value;
		}
	}

	/// <summary>
	///   <para>The point of contact between the wheel and the ground.</para>
	/// </summary>
	public Vector3 point
	{
		get
		{
			return m_Point;
		}
		set
		{
			m_Point = value;
		}
	}

	/// <summary>
	///   <para>The normal at the point of contact.</para>
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
	///   <para>The direction the wheel is pointing in.</para>
	/// </summary>
	public Vector3 forwardDir
	{
		get
		{
			return m_ForwardDir;
		}
		set
		{
			m_ForwardDir = value;
		}
	}

	/// <summary>
	///   <para>The sideways direction of the wheel.</para>
	/// </summary>
	public Vector3 sidewaysDir
	{
		get
		{
			return m_SidewaysDir;
		}
		set
		{
			m_SidewaysDir = value;
		}
	}

	/// <summary>
	///   <para>The magnitude of the force being applied for the contact.</para>
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
	///   <para>Tire slip in the rolling direction. Acceleration slip is negative, braking slip is positive.</para>
	/// </summary>
	public float forwardSlip
	{
		get
		{
			return m_ForwardSlip;
		}
		set
		{
			m_ForwardSlip = value;
		}
	}

	/// <summary>
	///   <para>Tire slip in the sideways direction.</para>
	/// </summary>
	public float sidewaysSlip
	{
		get
		{
			return m_SidewaysSlip;
		}
		set
		{
			m_SidewaysSlip = value;
		}
	}
}

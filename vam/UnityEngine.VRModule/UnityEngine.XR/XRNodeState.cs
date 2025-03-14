using UnityEngine.Scripting;

namespace UnityEngine.XR;

[UsedByNativeCode]
public struct XRNodeState
{
	private XRNode m_Type;

	private AvailableTrackingData m_AvailableFields;

	private Vector3 m_Position;

	private Quaternion m_Rotation;

	private Vector3 m_Velocity;

	private Vector3 m_AngularVelocity;

	private Vector3 m_Acceleration;

	private Vector3 m_AngularAcceleration;

	private int m_Tracked;

	private ulong m_UniqueID;

	/// <summary>
	///   <para>The unique identifier of the tracked node.</para>
	/// </summary>
	public ulong uniqueID
	{
		get
		{
			return m_UniqueID;
		}
		set
		{
			m_UniqueID = value;
		}
	}

	/// <summary>
	///   <para>The type of the tracked node as specified in XR.XRNode.</para>
	/// </summary>
	public XRNode nodeType
	{
		get
		{
			return m_Type;
		}
		set
		{
			m_Type = value;
		}
	}

	/// <summary>
	///   <para>
	///     Set to true if the node is presently being tracked by the underlying XR system,
	/// and false if the node is not presently being tracked by the underlying XR system.</para>
	/// </summary>
	public bool tracked
	{
		get
		{
			return m_Tracked == 1;
		}
		set
		{
			m_Tracked = (value ? 1 : 0);
		}
	}

	/// <summary>
	///   <para>Sets the vector representing the current position of the tracked node.</para>
	/// </summary>
	public Vector3 position
	{
		set
		{
			m_Position = value;
			m_AvailableFields |= AvailableTrackingData.PositionAvailable;
		}
	}

	/// <summary>
	///   <para>Sets the quaternion representing the current rotation of the tracked node.</para>
	/// </summary>
	public Quaternion rotation
	{
		set
		{
			m_Rotation = value;
			m_AvailableFields |= AvailableTrackingData.RotationAvailable;
		}
	}

	/// <summary>
	///   <para>Sets the vector representing the current velocity of the tracked node.</para>
	/// </summary>
	public Vector3 velocity
	{
		set
		{
			m_Velocity = value;
			m_AvailableFields |= AvailableTrackingData.VelocityAvailable;
		}
	}

	/// <summary>
	///   <para>Sets the vector representing the current angular velocity of the tracked node.</para>
	/// </summary>
	public Vector3 angularVelocity
	{
		set
		{
			m_AngularVelocity = value;
			m_AvailableFields |= AvailableTrackingData.AngularVelocityAvailable;
		}
	}

	/// <summary>
	///   <para>Sets the vector representing the current acceleration of the tracked node.</para>
	/// </summary>
	public Vector3 acceleration
	{
		set
		{
			m_Acceleration = value;
			m_AvailableFields |= AvailableTrackingData.AccelerationAvailable;
		}
	}

	/// <summary>
	///   <para>Sets the vector representing the current angular acceleration of the tracked node.</para>
	/// </summary>
	public Vector3 angularAcceleration
	{
		set
		{
			m_AngularAcceleration = value;
			m_AvailableFields |= AvailableTrackingData.AngularAccelerationAvailable;
		}
	}

	public bool TryGetPosition(out Vector3 position)
	{
		return TryGet(m_Position, AvailableTrackingData.PositionAvailable, out position);
	}

	public bool TryGetRotation(out Quaternion rotation)
	{
		return TryGet(m_Rotation, AvailableTrackingData.RotationAvailable, out rotation);
	}

	public bool TryGetVelocity(out Vector3 velocity)
	{
		return TryGet(m_Velocity, AvailableTrackingData.VelocityAvailable, out velocity);
	}

	public bool TryGetAngularVelocity(out Vector3 angularVelocity)
	{
		return TryGet(m_AngularVelocity, AvailableTrackingData.AngularVelocityAvailable, out angularVelocity);
	}

	public bool TryGetAcceleration(out Vector3 acceleration)
	{
		return TryGet(m_Acceleration, AvailableTrackingData.AccelerationAvailable, out acceleration);
	}

	public bool TryGetAngularAcceleration(out Vector3 angularAcceleration)
	{
		return TryGet(m_AngularAcceleration, AvailableTrackingData.AngularAccelerationAvailable, out angularAcceleration);
	}

	private bool TryGet<T>(T inValue, AvailableTrackingData availabilityFlag, out T outValue) where T : new()
	{
		if (m_Tracked == 1 && (m_AvailableFields & availabilityFlag) > AvailableTrackingData.None)
		{
			outValue = inValue;
			return true;
		}
		outValue = new T();
		return false;
	}
}

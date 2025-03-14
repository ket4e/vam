namespace UnityEngine;

/// <summary>
///   <para>Structure describing the status of a finger touching the screen.</para>
/// </summary>
public struct Touch
{
	private int m_FingerId;

	private Vector2 m_Position;

	private Vector2 m_RawPosition;

	private Vector2 m_PositionDelta;

	private float m_TimeDelta;

	private int m_TapCount;

	private TouchPhase m_Phase;

	private TouchType m_Type;

	private float m_Pressure;

	private float m_maximumPossiblePressure;

	private float m_Radius;

	private float m_RadiusVariance;

	private float m_AltitudeAngle;

	private float m_AzimuthAngle;

	/// <summary>
	///   <para>The unique index for the touch.</para>
	/// </summary>
	public int fingerId
	{
		get
		{
			return m_FingerId;
		}
		set
		{
			m_FingerId = value;
		}
	}

	/// <summary>
	///   <para>The position of the touch in pixel coordinates.</para>
	/// </summary>
	public Vector2 position
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
	///   <para>The raw position used for the touch.</para>
	/// </summary>
	public Vector2 rawPosition
	{
		get
		{
			return m_RawPosition;
		}
		set
		{
			m_RawPosition = value;
		}
	}

	/// <summary>
	///   <para>The position delta since last change.</para>
	/// </summary>
	public Vector2 deltaPosition
	{
		get
		{
			return m_PositionDelta;
		}
		set
		{
			m_PositionDelta = value;
		}
	}

	/// <summary>
	///   <para>Amount of time that has passed since the last recorded change in Touch values.</para>
	/// </summary>
	public float deltaTime
	{
		get
		{
			return m_TimeDelta;
		}
		set
		{
			m_TimeDelta = value;
		}
	}

	/// <summary>
	///   <para>Number of taps.</para>
	/// </summary>
	public int tapCount
	{
		get
		{
			return m_TapCount;
		}
		set
		{
			m_TapCount = value;
		}
	}

	/// <summary>
	///   <para>Describes the phase of the touch.</para>
	/// </summary>
	public TouchPhase phase
	{
		get
		{
			return m_Phase;
		}
		set
		{
			m_Phase = value;
		}
	}

	/// <summary>
	///   <para>The current amount of pressure being applied to a touch.  1.0f is considered to be the pressure of an average touch.  If Input.touchPressureSupported returns false, the value of this property will always be 1.0f.</para>
	/// </summary>
	public float pressure
	{
		get
		{
			return m_Pressure;
		}
		set
		{
			m_Pressure = value;
		}
	}

	/// <summary>
	///   <para>The maximum possible pressure value for a platform.  If Input.touchPressureSupported returns false, the value of this property will always be 1.0f.</para>
	/// </summary>
	public float maximumPossiblePressure
	{
		get
		{
			return m_maximumPossiblePressure;
		}
		set
		{
			m_maximumPossiblePressure = value;
		}
	}

	/// <summary>
	///   <para>A value that indicates whether a touch was of Direct, Indirect (or remote), or Stylus type.</para>
	/// </summary>
	public TouchType type
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
	///   <para>Value of 0 radians indicates that the stylus is parallel to the surface, pi/2 indicates that it is perpendicular.</para>
	/// </summary>
	public float altitudeAngle
	{
		get
		{
			return m_AltitudeAngle;
		}
		set
		{
			m_AltitudeAngle = value;
		}
	}

	/// <summary>
	///   <para>Value of 0 radians indicates that the stylus is pointed along the x-axis of the device.</para>
	/// </summary>
	public float azimuthAngle
	{
		get
		{
			return m_AzimuthAngle;
		}
		set
		{
			m_AzimuthAngle = value;
		}
	}

	/// <summary>
	///   <para>An estimated value of the radius of a touch.  Add radiusVariance to get the maximum touch size, subtract it to get the minimum touch size.</para>
	/// </summary>
	public float radius
	{
		get
		{
			return m_Radius;
		}
		set
		{
			m_Radius = value;
		}
	}

	/// <summary>
	///   <para>This value determines the accuracy of the touch radius. Add this value to the radius to get the maximum touch size, subtract it to get the minimum touch size.</para>
	/// </summary>
	public float radiusVariance
	{
		get
		{
			return m_RadiusVariance;
		}
		set
		{
			m_RadiusVariance = value;
		}
	}
}

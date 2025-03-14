namespace UnityEngine;

/// <summary>
///   <para>Structure describing acceleration status of the device.</para>
/// </summary>
public struct AccelerationEvent
{
	private float x;

	private float y;

	private float z;

	private float m_TimeDelta;

	/// <summary>
	///   <para>Value of acceleration.</para>
	/// </summary>
	public Vector3 acceleration => new Vector3(x, y, z);

	/// <summary>
	///   <para>Amount of time passed since last accelerometer measurement.</para>
	/// </summary>
	public float deltaTime => m_TimeDelta;
}

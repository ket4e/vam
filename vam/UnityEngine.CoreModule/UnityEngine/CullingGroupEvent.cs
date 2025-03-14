namespace UnityEngine;

/// <summary>
///   <para>Provides information about the current and previous states of one sphere in a CullingGroup.</para>
/// </summary>
public struct CullingGroupEvent
{
	private int m_Index;

	private byte m_PrevState;

	private byte m_ThisState;

	private const byte kIsVisibleMask = 128;

	private const byte kDistanceMask = 127;

	/// <summary>
	///   <para>The index of the sphere that has changed.</para>
	/// </summary>
	public int index => m_Index;

	/// <summary>
	///   <para>Was the sphere considered visible by the most recent culling pass?</para>
	/// </summary>
	public bool isVisible => (m_ThisState & 0x80) != 0;

	/// <summary>
	///   <para>Was the sphere visible before the most recent culling pass?</para>
	/// </summary>
	public bool wasVisible => (m_PrevState & 0x80) != 0;

	/// <summary>
	///   <para>Did this sphere change from being invisible to being visible in the most recent culling pass?</para>
	/// </summary>
	public bool hasBecomeVisible => isVisible && !wasVisible;

	/// <summary>
	///   <para>Did this sphere change from being visible to being invisible in the most recent culling pass?</para>
	/// </summary>
	public bool hasBecomeInvisible => !isVisible && wasVisible;

	/// <summary>
	///   <para>The current distance band index of the sphere, after the most recent culling pass.</para>
	/// </summary>
	public int currentDistance => m_ThisState & 0x7F;

	/// <summary>
	///   <para>The distance band index of the sphere before the most recent culling pass.</para>
	/// </summary>
	public int previousDistance => m_PrevState & 0x7F;
}

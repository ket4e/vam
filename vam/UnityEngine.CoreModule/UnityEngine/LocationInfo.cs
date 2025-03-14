namespace UnityEngine;

/// <summary>
///   <para>Structure describing device location.</para>
/// </summary>
public struct LocationInfo
{
	private double m_Timestamp;

	private float m_Latitude;

	private float m_Longitude;

	private float m_Altitude;

	private float m_HorizontalAccuracy;

	private float m_VerticalAccuracy;

	/// <summary>
	///   <para>Geographical device location latitude.</para>
	/// </summary>
	public float latitude => m_Latitude;

	/// <summary>
	///   <para>Geographical device location latitude.</para>
	/// </summary>
	public float longitude => m_Longitude;

	/// <summary>
	///   <para>Geographical device location altitude.</para>
	/// </summary>
	public float altitude => m_Altitude;

	/// <summary>
	///   <para>Horizontal accuracy of the location.</para>
	/// </summary>
	public float horizontalAccuracy => m_HorizontalAccuracy;

	/// <summary>
	///   <para>Vertical accuracy of the location.</para>
	/// </summary>
	public float verticalAccuracy => m_VerticalAccuracy;

	/// <summary>
	///   <para>Timestamp (in seconds since 1970) when location was last time updated.</para>
	/// </summary>
	public double timestamp => m_Timestamp;
}

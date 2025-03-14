using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.XR.WSA;

/// <summary>
///   <para>Indicates the lifecycle state of the device's spatial location system.</para>
/// </summary>
[MovedFrom("UnityEngine.VR.WSA")]
public enum PositionalLocatorState
{
	/// <summary>
	///   <para>The device's spatial location system is not available.</para>
	/// </summary>
	Unavailable,
	/// <summary>
	///   <para>The device is reporting its orientation and has not been asked to report its position in the user's surroundings.</para>
	/// </summary>
	OrientationOnly,
	/// <summary>
	///   <para>The device is reporting its orientation and is preparing to locate its position in the user's surroundings.</para>
	/// </summary>
	Activating,
	/// <summary>
	///   <para>The device is reporting its orientation and position in the user's surroundings.</para>
	/// </summary>
	Active,
	/// <summary>
	///   <para>The device is reporting its orientation but cannot locate its position in the user's surroundings due to external factors like poor lighting conditions.</para>
	/// </summary>
	Inhibited
}

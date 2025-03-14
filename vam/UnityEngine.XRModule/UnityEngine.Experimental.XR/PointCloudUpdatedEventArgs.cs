namespace UnityEngine.Experimental.XR;

/// <summary>
///   <para>Contains data supplied to a XRDepth.PointCloudUpdated event.</para>
/// </summary>
public struct PointCloudUpdatedEventArgs
{
	internal XRDepthSubsystem m_DepthSubsystem;

	/// <summary>
	///   <para>A reference to the XRDepthSubsystem object from which the event originated.</para>
	/// </summary>
	public XRDepthSubsystem DepthSubsystem => m_DepthSubsystem;
}

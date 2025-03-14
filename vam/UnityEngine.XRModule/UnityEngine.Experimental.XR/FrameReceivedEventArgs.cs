namespace UnityEngine.Experimental.XR;

/// <summary>
///   <para>Structure containing data passed during Frame Received Event.</para>
/// </summary>
public struct FrameReceivedEventArgs
{
	internal XRCameraSubsystem m_CameraSubsystem;

	/// <summary>
	///   <para>Reference to the XRCameraSubsystem associated with this event.</para>
	/// </summary>
	public XRCameraSubsystem CameraSubsystem => m_CameraSubsystem;
}

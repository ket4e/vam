namespace ZenFulcrum.VR.OpenVRBinding;

public enum ETrackingResult
{
	Uninitialized = 1,
	Calibrating_InProgress = 100,
	Calibrating_OutOfRange = 101,
	Running_OK = 200,
	Running_OutOfRange = 201,
	Fallback_RotationOnly = 300
}

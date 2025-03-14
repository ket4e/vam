using System;

namespace UnityEngine.XR;

[Flags]
internal enum AvailableTrackingData
{
	None = 0,
	PositionAvailable = 1,
	RotationAvailable = 2,
	VelocityAvailable = 4,
	AngularVelocityAvailable = 8,
	AccelerationAvailable = 0x10,
	AngularAccelerationAvailable = 0x20
}

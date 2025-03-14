using System.ComponentModel;

namespace Valve.VR;

public enum SteamVR_Input_Sources
{
	[Description("/unrestricted")]
	Any,
	[Description("/user/hand/left")]
	LeftHand,
	[Description("/user/hand/right")]
	RightHand,
	[Description("/user/foot/left")]
	LeftFoot,
	[Description("/user/foot/right")]
	RightFoot,
	[Description("/user/shoulder/left")]
	LeftShoulder,
	[Description("/user/shoulder/right")]
	RightShoulder,
	[Description("/user/waist")]
	Waist,
	[Description("/user/chest")]
	Chest,
	[Description("/user/head")]
	Head,
	[Description("/user/gamepad")]
	Gamepad,
	[Description("/user/camera")]
	Camera,
	[Description("/user/keyboard")]
	Keyboard
}

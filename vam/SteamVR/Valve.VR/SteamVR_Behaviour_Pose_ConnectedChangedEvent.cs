using System;
using UnityEngine.Events;

namespace Valve.VR;

[Serializable]
public class SteamVR_Behaviour_Pose_ConnectedChangedEvent : UnityEvent<SteamVR_Behaviour_Pose, SteamVR_Input_Sources, bool>
{
}

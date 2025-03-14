namespace Valve.VR;

public class SteamVR_ExternalCamera_LegacyManager
{
	public static int cameraIndex = -1;

	private static SteamVR_Events.Action newPosesAction;

	public static bool hasCamera => cameraIndex != -1;

	public static void SubscribeToNewPoses()
	{
		if (newPosesAction == null)
		{
			newPosesAction = SteamVR_Events.NewPosesAction(OnNewPoses);
		}
		newPosesAction.enabled = true;
	}

	private static void OnNewPoses(TrackedDevicePose_t[] poses)
	{
		if (cameraIndex != -1)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < poses.Length; i++)
		{
			if (!poses[i].bDeviceIsConnected)
			{
				continue;
			}
			ETrackedDeviceClass trackedDeviceClass = OpenVR.System.GetTrackedDeviceClass((uint)i);
			if (trackedDeviceClass == ETrackedDeviceClass.Controller || trackedDeviceClass == ETrackedDeviceClass.GenericTracker)
			{
				num++;
				if (num >= 3)
				{
					cameraIndex = i;
					break;
				}
			}
		}
	}
}

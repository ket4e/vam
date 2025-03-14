namespace Valve.VR;

public class SteamVR_Input_ActionSet_default : SteamVR_ActionSet
{
	public virtual SteamVR_Action_Boolean Menu => SteamVR_Actions.default_Menu;

	public virtual SteamVR_Action_Boolean UIInteract => SteamVR_Actions.default_UIInteract;

	public virtual SteamVR_Action_Vector2 UIScroll => SteamVR_Actions.default_UIScroll;

	public virtual SteamVR_Action_Boolean TargetShow => SteamVR_Actions.default_TargetShow;

	public virtual SteamVR_Action_Boolean CycleEngage => SteamVR_Actions.default_CycleEngage;

	public virtual SteamVR_Action_Vector2 CycleUsingXAxis => SteamVR_Actions.default_CycleUsingXAxis;

	public virtual SteamVR_Action_Vector2 CycleUsingYAxis => SteamVR_Actions.default_CycleUsingYAxis;

	public virtual SteamVR_Action_Boolean Select => SteamVR_Actions.default_Select;

	public virtual SteamVR_Action_Vector2 PushPullNode => SteamVR_Actions.default_PushPullNode;

	public virtual SteamVR_Action_Boolean TeleportShow => SteamVR_Actions.default_TeleportShow;

	public virtual SteamVR_Action_Boolean Teleport => SteamVR_Actions.default_Teleport;

	public virtual SteamVR_Action_Boolean GrabNavigate => SteamVR_Actions.default_GrabNavigate;

	public virtual SteamVR_Action_Vector2 FreeMove => SteamVR_Actions.default_FreeMove;

	public virtual SteamVR_Action_Vector2 FreeModeMove => SteamVR_Actions.default_FreeModeMove;

	public virtual SteamVR_Action_Boolean SwapFreeMoveAxis => SteamVR_Actions.default_SwapFreeMoveAxis;

	public virtual SteamVR_Action_Boolean Grab => SteamVR_Actions.default_Grab;

	public virtual SteamVR_Action_Boolean HoldGrab => SteamVR_Actions.default_HoldGrab;

	public virtual SteamVR_Action_Single GrabVal => SteamVR_Actions.default_GrabVal;

	public virtual SteamVR_Action_Boolean RemoteGrab => SteamVR_Actions.default_RemoteGrab;

	public virtual SteamVR_Action_Boolean RemoteHoldGrab => SteamVR_Actions.default_RemoteHoldGrab;

	public virtual SteamVR_Action_Boolean ToggleHand => SteamVR_Actions.default_ToggleHand;

	public virtual SteamVR_Action_Pose Pose => SteamVR_Actions.default_Pose;

	public virtual SteamVR_Action_Skeleton SkeletonLeftHand => SteamVR_Actions.default_SkeletonLeftHand;

	public virtual SteamVR_Action_Skeleton SkeletonRightHand => SteamVR_Actions.default_SkeletonRightHand;

	public virtual SteamVR_Action_Boolean HeadsetOnHead => SteamVR_Actions.default_HeadsetOnHead;

	public virtual SteamVR_Action_Vibration Haptic => SteamVR_Actions.default_Haptic;
}

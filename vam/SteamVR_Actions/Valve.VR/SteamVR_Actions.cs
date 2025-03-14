namespace Valve.VR;

public class SteamVR_Actions
{
	private static SteamVR_Action_Boolean p_default_Menu;

	private static SteamVR_Action_Boolean p_default_UIInteract;

	private static SteamVR_Action_Vector2 p_default_UIScroll;

	private static SteamVR_Action_Boolean p_default_TargetShow;

	private static SteamVR_Action_Boolean p_default_CycleEngage;

	private static SteamVR_Action_Vector2 p_default_CycleUsingXAxis;

	private static SteamVR_Action_Vector2 p_default_CycleUsingYAxis;

	private static SteamVR_Action_Boolean p_default_Select;

	private static SteamVR_Action_Vector2 p_default_PushPullNode;

	private static SteamVR_Action_Boolean p_default_TeleportShow;

	private static SteamVR_Action_Boolean p_default_Teleport;

	private static SteamVR_Action_Boolean p_default_GrabNavigate;

	private static SteamVR_Action_Vector2 p_default_FreeMove;

	private static SteamVR_Action_Vector2 p_default_FreeModeMove;

	private static SteamVR_Action_Boolean p_default_SwapFreeMoveAxis;

	private static SteamVR_Action_Boolean p_default_Grab;

	private static SteamVR_Action_Boolean p_default_HoldGrab;

	private static SteamVR_Action_Single p_default_GrabVal;

	private static SteamVR_Action_Boolean p_default_RemoteGrab;

	private static SteamVR_Action_Boolean p_default_RemoteHoldGrab;

	private static SteamVR_Action_Boolean p_default_ToggleHand;

	private static SteamVR_Action_Pose p_default_Pose;

	private static SteamVR_Action_Skeleton p_default_SkeletonLeftHand;

	private static SteamVR_Action_Skeleton p_default_SkeletonRightHand;

	private static SteamVR_Action_Boolean p_default_HeadsetOnHead;

	private static SteamVR_Action_Vibration p_default_Haptic;

	private static SteamVR_Input_ActionSet_default p__default;

	public static SteamVR_Action_Boolean default_Menu => p_default_Menu.GetCopy<SteamVR_Action_Boolean>();

	public static SteamVR_Action_Boolean default_UIInteract => p_default_UIInteract.GetCopy<SteamVR_Action_Boolean>();

	public static SteamVR_Action_Vector2 default_UIScroll => p_default_UIScroll.GetCopy<SteamVR_Action_Vector2>();

	public static SteamVR_Action_Boolean default_TargetShow => p_default_TargetShow.GetCopy<SteamVR_Action_Boolean>();

	public static SteamVR_Action_Boolean default_CycleEngage => p_default_CycleEngage.GetCopy<SteamVR_Action_Boolean>();

	public static SteamVR_Action_Vector2 default_CycleUsingXAxis => p_default_CycleUsingXAxis.GetCopy<SteamVR_Action_Vector2>();

	public static SteamVR_Action_Vector2 default_CycleUsingYAxis => p_default_CycleUsingYAxis.GetCopy<SteamVR_Action_Vector2>();

	public static SteamVR_Action_Boolean default_Select => p_default_Select.GetCopy<SteamVR_Action_Boolean>();

	public static SteamVR_Action_Vector2 default_PushPullNode => p_default_PushPullNode.GetCopy<SteamVR_Action_Vector2>();

	public static SteamVR_Action_Boolean default_TeleportShow => p_default_TeleportShow.GetCopy<SteamVR_Action_Boolean>();

	public static SteamVR_Action_Boolean default_Teleport => p_default_Teleport.GetCopy<SteamVR_Action_Boolean>();

	public static SteamVR_Action_Boolean default_GrabNavigate => p_default_GrabNavigate.GetCopy<SteamVR_Action_Boolean>();

	public static SteamVR_Action_Vector2 default_FreeMove => p_default_FreeMove.GetCopy<SteamVR_Action_Vector2>();

	public static SteamVR_Action_Vector2 default_FreeModeMove => p_default_FreeModeMove.GetCopy<SteamVR_Action_Vector2>();

	public static SteamVR_Action_Boolean default_SwapFreeMoveAxis => p_default_SwapFreeMoveAxis.GetCopy<SteamVR_Action_Boolean>();

	public static SteamVR_Action_Boolean default_Grab => p_default_Grab.GetCopy<SteamVR_Action_Boolean>();

	public static SteamVR_Action_Boolean default_HoldGrab => p_default_HoldGrab.GetCopy<SteamVR_Action_Boolean>();

	public static SteamVR_Action_Single default_GrabVal => p_default_GrabVal.GetCopy<SteamVR_Action_Single>();

	public static SteamVR_Action_Boolean default_RemoteGrab => p_default_RemoteGrab.GetCopy<SteamVR_Action_Boolean>();

	public static SteamVR_Action_Boolean default_RemoteHoldGrab => p_default_RemoteHoldGrab.GetCopy<SteamVR_Action_Boolean>();

	public static SteamVR_Action_Boolean default_ToggleHand => p_default_ToggleHand.GetCopy<SteamVR_Action_Boolean>();

	public static SteamVR_Action_Pose default_Pose => p_default_Pose.GetCopy<SteamVR_Action_Pose>();

	public static SteamVR_Action_Skeleton default_SkeletonLeftHand => p_default_SkeletonLeftHand.GetCopy<SteamVR_Action_Skeleton>();

	public static SteamVR_Action_Skeleton default_SkeletonRightHand => p_default_SkeletonRightHand.GetCopy<SteamVR_Action_Skeleton>();

	public static SteamVR_Action_Boolean default_HeadsetOnHead => p_default_HeadsetOnHead.GetCopy<SteamVR_Action_Boolean>();

	public static SteamVR_Action_Vibration default_Haptic => p_default_Haptic.GetCopy<SteamVR_Action_Vibration>();

	public static SteamVR_Input_ActionSet_default _default => p__default.GetCopy<SteamVR_Input_ActionSet_default>();

	private static void InitializeActionArrays()
	{
		SteamVR_Input.actions = new SteamVR_Action[26]
		{
			default_Menu, default_UIInteract, default_UIScroll, default_TargetShow, default_CycleEngage, default_CycleUsingXAxis, default_CycleUsingYAxis, default_Select, default_PushPullNode, default_TeleportShow,
			default_Teleport, default_GrabNavigate, default_FreeMove, default_FreeModeMove, default_SwapFreeMoveAxis, default_Grab, default_HoldGrab, default_GrabVal, default_RemoteGrab, default_RemoteHoldGrab,
			default_ToggleHand, default_Pose, default_SkeletonLeftHand, default_SkeletonRightHand, default_HeadsetOnHead, default_Haptic
		};
		SteamVR_Input.actionsIn = new ISteamVR_Action_In[25]
		{
			default_Menu, default_UIInteract, default_UIScroll, default_TargetShow, default_CycleEngage, default_CycleUsingXAxis, default_CycleUsingYAxis, default_Select, default_PushPullNode, default_TeleportShow,
			default_Teleport, default_GrabNavigate, default_FreeMove, default_FreeModeMove, default_SwapFreeMoveAxis, default_Grab, default_HoldGrab, default_GrabVal, default_RemoteGrab, default_RemoteHoldGrab,
			default_ToggleHand, default_Pose, default_SkeletonLeftHand, default_SkeletonRightHand, default_HeadsetOnHead
		};
		SteamVR_Input.actionsOut = new ISteamVR_Action_Out[1] { default_Haptic };
		SteamVR_Input.actionsVibration = new SteamVR_Action_Vibration[1] { default_Haptic };
		SteamVR_Input.actionsPose = new SteamVR_Action_Pose[1] { default_Pose };
		SteamVR_Input.actionsBoolean = new SteamVR_Action_Boolean[15]
		{
			default_Menu, default_UIInteract, default_TargetShow, default_CycleEngage, default_Select, default_TeleportShow, default_Teleport, default_GrabNavigate, default_SwapFreeMoveAxis, default_Grab,
			default_HoldGrab, default_RemoteGrab, default_RemoteHoldGrab, default_ToggleHand, default_HeadsetOnHead
		};
		SteamVR_Input.actionsSingle = new SteamVR_Action_Single[1] { default_GrabVal };
		SteamVR_Input.actionsVector2 = new SteamVR_Action_Vector2[6] { default_UIScroll, default_CycleUsingXAxis, default_CycleUsingYAxis, default_PushPullNode, default_FreeMove, default_FreeModeMove };
		SteamVR_Input.actionsVector3 = new SteamVR_Action_Vector3[0];
		SteamVR_Input.actionsSkeleton = new SteamVR_Action_Skeleton[2] { default_SkeletonLeftHand, default_SkeletonRightHand };
		SteamVR_Input.actionsNonPoseNonSkeletonIn = new ISteamVR_Action_In[22]
		{
			default_Menu, default_UIInteract, default_UIScroll, default_TargetShow, default_CycleEngage, default_CycleUsingXAxis, default_CycleUsingYAxis, default_Select, default_PushPullNode, default_TeleportShow,
			default_Teleport, default_GrabNavigate, default_FreeMove, default_FreeModeMove, default_SwapFreeMoveAxis, default_Grab, default_HoldGrab, default_GrabVal, default_RemoteGrab, default_RemoteHoldGrab,
			default_ToggleHand, default_HeadsetOnHead
		};
	}

	private static void PreInitActions()
	{
		p_default_Menu = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/Menu");
		p_default_UIInteract = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/UIInteract");
		p_default_UIScroll = SteamVR_Action.Create<SteamVR_Action_Vector2>("/actions/default/in/UIScroll");
		p_default_TargetShow = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/TargetShow");
		p_default_CycleEngage = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/CycleEngage");
		p_default_CycleUsingXAxis = SteamVR_Action.Create<SteamVR_Action_Vector2>("/actions/default/in/CycleUsingXAxis");
		p_default_CycleUsingYAxis = SteamVR_Action.Create<SteamVR_Action_Vector2>("/actions/default/in/CycleUsingYAxis");
		p_default_Select = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/Select");
		p_default_PushPullNode = SteamVR_Action.Create<SteamVR_Action_Vector2>("/actions/default/in/PushPullNode");
		p_default_TeleportShow = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/TeleportShow");
		p_default_Teleport = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/Teleport");
		p_default_GrabNavigate = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/GrabNavigate");
		p_default_FreeMove = SteamVR_Action.Create<SteamVR_Action_Vector2>("/actions/default/in/FreeMove");
		p_default_FreeModeMove = SteamVR_Action.Create<SteamVR_Action_Vector2>("/actions/default/in/FreeModeMove");
		p_default_SwapFreeMoveAxis = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/SwapFreeMoveAxis");
		p_default_Grab = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/Grab");
		p_default_HoldGrab = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/HoldGrab");
		p_default_GrabVal = SteamVR_Action.Create<SteamVR_Action_Single>("/actions/default/in/GrabVal");
		p_default_RemoteGrab = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/RemoteGrab");
		p_default_RemoteHoldGrab = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/RemoteHoldGrab");
		p_default_ToggleHand = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/ToggleHand");
		p_default_Pose = SteamVR_Action.Create<SteamVR_Action_Pose>("/actions/default/in/Pose");
		p_default_SkeletonLeftHand = SteamVR_Action.Create<SteamVR_Action_Skeleton>("/actions/default/in/SkeletonLeftHand");
		p_default_SkeletonRightHand = SteamVR_Action.Create<SteamVR_Action_Skeleton>("/actions/default/in/SkeletonRightHand");
		p_default_HeadsetOnHead = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/HeadsetOnHead");
		p_default_Haptic = SteamVR_Action.Create<SteamVR_Action_Vibration>("/actions/default/out/Haptic");
	}

	private static void StartPreInitActionSets()
	{
		p__default = SteamVR_ActionSet.Create<SteamVR_Input_ActionSet_default>("/actions/default");
		SteamVR_Input.actionSets = new SteamVR_ActionSet[1] { _default };
	}

	public static void PreInitialize()
	{
		StartPreInitActionSets();
		SteamVR_Input.PreinitializeActionSetDictionaries();
		PreInitActions();
		InitializeActionArrays();
		SteamVR_Input.PreinitializeActionDictionaries();
		SteamVR_Input.PreinitializeFinishActionSets();
	}
}

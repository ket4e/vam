using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Valve.VR;

public class SteamVR_Render : MonoBehaviour
{
	public SteamVR_ExternalCamera externalCamera;

	public string externalCameraConfigPath = "externalcamera.cfg";

	private static bool isQuitting;

	private SteamVR_Camera[] cameras = new SteamVR_Camera[0];

	public TrackedDevicePose_t[] poses = new TrackedDevicePose_t[64];

	public TrackedDevicePose_t[] gamePoses = new TrackedDevicePose_t[0];

	private static bool _pauseRendering;

	private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

	private bool? doesPathExist;

	private float sceneResolutionScale = 1f;

	private float timeScale = 1f;

	private EVRScreenshotType[] screenshotTypes = new EVRScreenshotType[1] { EVRScreenshotType.StereoPanorama };

	public static EVREye eye { get; private set; }

	public static SteamVR_Render instance => SteamVR_Behaviour.instance.steamvr_render;

	public static bool pauseRendering
	{
		get
		{
			return _pauseRendering;
		}
		set
		{
			_pauseRendering = value;
			OpenVR.Compositor?.SuspendRendering(value);
		}
	}

	private void OnApplicationQuit()
	{
		isQuitting = true;
		SteamVR.SafeDispose();
	}

	public static void Add(SteamVR_Camera vrcam)
	{
		if (!isQuitting)
		{
			instance.AddInternal(vrcam);
		}
	}

	public static void Remove(SteamVR_Camera vrcam)
	{
		if (!isQuitting && instance != null)
		{
			instance.RemoveInternal(vrcam);
		}
	}

	public static SteamVR_Camera Top()
	{
		if (!isQuitting)
		{
			return instance.TopInternal();
		}
		return null;
	}

	private void AddInternal(SteamVR_Camera vrcam)
	{
		Camera component = vrcam.GetComponent<Camera>();
		int num = cameras.Length;
		SteamVR_Camera[] array = new SteamVR_Camera[num + 1];
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			Camera component2 = cameras[i].GetComponent<Camera>();
			if (i == num2 && component2.depth > component.depth)
			{
				array[num2++] = vrcam;
			}
			array[num2++] = cameras[i];
		}
		if (num2 == num)
		{
			array[num2] = vrcam;
		}
		cameras = array;
	}

	private void RemoveInternal(SteamVR_Camera vrcam)
	{
		int num = cameras.Length;
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			SteamVR_Camera steamVR_Camera = cameras[i];
			if (steamVR_Camera == vrcam)
			{
				num2++;
			}
		}
		if (num2 == 0)
		{
			return;
		}
		SteamVR_Camera[] array = new SteamVR_Camera[num - num2];
		int num3 = 0;
		for (int j = 0; j < num; j++)
		{
			SteamVR_Camera steamVR_Camera2 = cameras[j];
			if (steamVR_Camera2 != vrcam)
			{
				array[num3++] = steamVR_Camera2;
			}
		}
		cameras = array;
	}

	private SteamVR_Camera TopInternal()
	{
		if (cameras.Length > 0)
		{
			return cameras[cameras.Length - 1];
		}
		return null;
	}

	private IEnumerator RenderLoop()
	{
		while (Application.isPlaying)
		{
			yield return waitForEndOfFrame;
			if (pauseRendering)
			{
				continue;
			}
			CVRCompositor compositor = OpenVR.Compositor;
			if (compositor != null)
			{
				if (!compositor.CanRenderScene())
				{
					continue;
				}
				compositor.SetTrackingSpace(SteamVR.settings.trackingSpace);
			}
			SteamVR_Overlay overlay = SteamVR_Overlay.instance;
			if (overlay != null)
			{
				overlay.UpdateOverlay();
			}
			if (CheckExternalCamera())
			{
				RenderExternalCamera();
			}
		}
	}

	private bool CheckExternalCamera()
	{
		if (doesPathExist == false)
		{
			return false;
		}
		bool? flag = doesPathExist;
		if (!flag.HasValue)
		{
			doesPathExist = File.Exists(externalCameraConfigPath);
		}
		if (externalCamera == null && doesPathExist == true)
		{
			GameObject gameObject = Resources.Load<GameObject>("SteamVR_ExternalCamera");
			if (gameObject == null)
			{
				doesPathExist = false;
				return false;
			}
			if (SteamVR_Settings.instance.legacyMixedRealityCamera)
			{
				if (!SteamVR_ExternalCamera_LegacyManager.hasCamera)
				{
					return false;
				}
				GameObject gameObject2 = Object.Instantiate(gameObject);
				gameObject2.gameObject.name = "External Camera";
				externalCamera = gameObject2.transform.GetChild(0).GetComponent<SteamVR_ExternalCamera>();
				externalCamera.configPath = externalCameraConfigPath;
				externalCamera.ReadConfig();
				externalCamera.SetupDeviceIndex(SteamVR_ExternalCamera_LegacyManager.cameraIndex);
			}
			else
			{
				SteamVR_Action_Pose mixedRealityCameraPose = SteamVR_Settings.instance.mixedRealityCameraPose;
				SteamVR_Input_Sources mixedRealityCameraInputSource = SteamVR_Settings.instance.mixedRealityCameraInputSource;
				if (mixedRealityCameraPose != null && SteamVR_Settings.instance.mixedRealityActionSetAutoEnable && mixedRealityCameraPose.actionSet != null && !mixedRealityCameraPose.actionSet.IsActive(mixedRealityCameraInputSource))
				{
					mixedRealityCameraPose.actionSet.Activate(mixedRealityCameraInputSource);
				}
				if (mixedRealityCameraPose == null)
				{
					doesPathExist = false;
					return false;
				}
				if (mixedRealityCameraPose != null && mixedRealityCameraPose[mixedRealityCameraInputSource].active && mixedRealityCameraPose[mixedRealityCameraInputSource].deviceIsConnected)
				{
					GameObject gameObject3 = Object.Instantiate(gameObject);
					gameObject3.gameObject.name = "External Camera";
					externalCamera = gameObject3.transform.GetChild(0).GetComponent<SteamVR_ExternalCamera>();
					externalCamera.configPath = externalCameraConfigPath;
					externalCamera.ReadConfig();
					externalCamera.SetupPose(mixedRealityCameraPose, mixedRealityCameraInputSource);
				}
			}
		}
		return externalCamera != null;
	}

	private void RenderExternalCamera()
	{
		if (!(externalCamera == null) && externalCamera.gameObject.activeInHierarchy)
		{
			int num = (int)Mathf.Max(externalCamera.config.frameSkip, 0f);
			if (Time.frameCount % (num + 1) == 0)
			{
				externalCamera.AttachToCamera(TopInternal());
				externalCamera.RenderNear();
				externalCamera.RenderFar();
			}
		}
	}

	private void OnInputFocus(bool hasFocus)
	{
		if (!SteamVR.active)
		{
			return;
		}
		if (hasFocus)
		{
			if (SteamVR.settings.pauseGameWhenDashboardVisible)
			{
				Time.timeScale = timeScale;
			}
			SteamVR_Camera.sceneResolutionScale = sceneResolutionScale;
			return;
		}
		if (SteamVR.settings.pauseGameWhenDashboardVisible)
		{
			timeScale = Time.timeScale;
			Time.timeScale = 0f;
		}
		sceneResolutionScale = SteamVR_Camera.sceneResolutionScale;
		SteamVR_Camera.sceneResolutionScale = 0.5f;
	}

	private string GetScreenshotFilename(uint screenshotHandle, EVRScreenshotPropertyFilenames screenshotPropertyFilename)
	{
		EVRScreenshotError pError = EVRScreenshotError.None;
		uint screenshotPropertyFilename2 = OpenVR.Screenshots.GetScreenshotPropertyFilename(screenshotHandle, screenshotPropertyFilename, null, 0u, ref pError);
		if (pError != 0 && pError != EVRScreenshotError.BufferTooSmall)
		{
			return null;
		}
		if (screenshotPropertyFilename2 > 1)
		{
			StringBuilder stringBuilder = new StringBuilder((int)screenshotPropertyFilename2);
			OpenVR.Screenshots.GetScreenshotPropertyFilename(screenshotHandle, screenshotPropertyFilename, stringBuilder, screenshotPropertyFilename2, ref pError);
			if (pError != 0)
			{
				return null;
			}
			return stringBuilder.ToString();
		}
		return null;
	}

	private void OnRequestScreenshot(VREvent_t vrEvent)
	{
		uint handle = vrEvent.data.screenshot.handle;
		EVRScreenshotType type = (EVRScreenshotType)vrEvent.data.screenshot.type;
		if (type == EVRScreenshotType.StereoPanorama)
		{
			string previewFilename = GetScreenshotFilename(handle, EVRScreenshotPropertyFilenames.Preview);
			string VRFilename = GetScreenshotFilename(handle, EVRScreenshotPropertyFilenames.VR);
			if (previewFilename != null && VRFilename != null)
			{
				GameObject gameObject = new GameObject("screenshotPosition");
				gameObject.transform.position = Top().transform.position;
				gameObject.transform.rotation = Top().transform.rotation;
				gameObject.transform.localScale = Top().transform.lossyScale;
				SteamVR_Utils.TakeStereoScreenshot(handle, gameObject, 32, 0.064f, ref previewFilename, ref VRFilename);
				OpenVR.Screenshots.SubmitScreenshot(handle, type, previewFilename, VRFilename);
			}
		}
	}

	private void OnEnable()
	{
		StartCoroutine(RenderLoop());
		SteamVR_Events.InputFocus.Listen(OnInputFocus);
		SteamVR_Events.System(EVREventType.VREvent_RequestScreenshot).Listen(OnRequestScreenshot);
		if (SteamVR_Settings.instance.legacyMixedRealityCamera)
		{
			SteamVR_ExternalCamera_LegacyManager.SubscribeToNewPoses();
		}
		Application.onBeforeRender += OnBeforeRender;
		if (SteamVR.initializedState == SteamVR.InitializedStates.InitializeSuccess)
		{
			OpenVR.Screenshots.HookScreenshot(screenshotTypes);
		}
		else
		{
			SteamVR_Events.Initialized.AddListener(OnSteamVRInitialized);
		}
	}

	private void OnSteamVRInitialized(bool success)
	{
		if (success)
		{
			OpenVR.Screenshots.HookScreenshot(screenshotTypes);
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		SteamVR_Events.InputFocus.Remove(OnInputFocus);
		SteamVR_Events.System(EVREventType.VREvent_RequestScreenshot).Remove(OnRequestScreenshot);
		Application.onBeforeRender -= OnBeforeRender;
		if (SteamVR.initializedState != SteamVR.InitializedStates.InitializeSuccess)
		{
			SteamVR_Events.Initialized.RemoveListener(OnSteamVRInitialized);
		}
	}

	public void UpdatePoses()
	{
		CVRCompositor compositor = OpenVR.Compositor;
		if (compositor != null)
		{
			compositor.GetLastPoses(poses, gamePoses);
			SteamVR_Events.NewPoses.Send(poses);
			SteamVR_Events.NewPosesApplied.Send();
		}
	}

	private void OnBeforeRender()
	{
		if (SteamVR.active && SteamVR.settings.IsPoseUpdateMode(SteamVR_UpdateModes.OnPreCull))
		{
			UpdatePoses();
		}
	}

	private void Update()
	{
		if (!SteamVR.active)
		{
			return;
		}
		UpdatePoses();
		CVRSystem system = OpenVR.System;
		if (system != null)
		{
			VREvent_t pEvent = default(VREvent_t);
			uint uncbVREvent = (uint)Marshal.SizeOf(typeof(VREvent_t));
			for (int i = 0; i < 64; i++)
			{
				if (!system.PollNextEvent(ref pEvent, uncbVREvent))
				{
					break;
				}
				switch ((EVREventType)pEvent.eventType)
				{
				case EVREventType.VREvent_InputFocusCaptured:
					if (pEvent.data.process.oldPid == 0)
					{
						SteamVR_Events.InputFocus.Send(arg0: false);
					}
					break;
				case EVREventType.VREvent_InputFocusReleased:
					if (pEvent.data.process.pid == 0)
					{
						SteamVR_Events.InputFocus.Send(arg0: true);
					}
					break;
				case EVREventType.VREvent_ShowRenderModels:
					SteamVR_Events.HideRenderModels.Send(arg0: false);
					break;
				case EVREventType.VREvent_HideRenderModels:
					SteamVR_Events.HideRenderModels.Send(arg0: true);
					break;
				default:
					SteamVR_Events.System((EVREventType)pEvent.eventType).Send(pEvent);
					break;
				}
			}
		}
		Application.targetFrameRate = -1;
		Application.runInBackground = true;
		QualitySettings.maxQueuedFrames = -1;
		QualitySettings.vSyncCount = 0;
		if (SteamVR.settings.lockPhysicsUpdateRateToRenderFrequency && Time.timeScale > 0f)
		{
			SteamVR steamVR = SteamVR.instance;
			if (steamVR != null)
			{
				Time.fixedDeltaTime = Time.timeScale / steamVR.hmd_DisplayFrequency;
			}
		}
	}
}

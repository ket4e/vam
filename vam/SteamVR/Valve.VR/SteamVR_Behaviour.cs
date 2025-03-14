using System.Collections;
using UnityEngine;
using UnityEngine.XR;

namespace Valve.VR;

public class SteamVR_Behaviour : MonoBehaviour
{
	private const string openVRDeviceName = "OpenVR";

	public static bool forcingInitialization;

	private static SteamVR_Behaviour _instance;

	public bool initializeSteamVROnAwake = true;

	public bool doNotDestroy = true;

	[HideInInspector]
	public SteamVR_Render steamvr_render;

	internal static bool isPlaying;

	private static bool initializing;

	private Coroutine initializeCoroutine;

	protected static int lastFrameCount = -1;

	public static SteamVR_Behaviour instance
	{
		get
		{
			if (_instance == null)
			{
				Initialize();
			}
			return _instance;
		}
	}

	public static void Initialize(bool forceUnityVRToOpenVR = false)
	{
		if (!(_instance == null) || initializing)
		{
			return;
		}
		initializing = true;
		GameObject gameObject = null;
		if (forceUnityVRToOpenVR)
		{
			forcingInitialization = true;
		}
		SteamVR_Render steamVR_Render = Object.FindObjectOfType<SteamVR_Render>();
		if (steamVR_Render != null)
		{
			gameObject = steamVR_Render.gameObject;
		}
		SteamVR_Behaviour steamVR_Behaviour = Object.FindObjectOfType<SteamVR_Behaviour>();
		if (steamVR_Behaviour != null)
		{
			gameObject = steamVR_Behaviour.gameObject;
		}
		if (gameObject == null)
		{
			GameObject gameObject2 = new GameObject("[SteamVR]");
			_instance = gameObject2.AddComponent<SteamVR_Behaviour>();
			_instance.steamvr_render = gameObject2.AddComponent<SteamVR_Render>();
		}
		else
		{
			steamVR_Behaviour = gameObject.GetComponent<SteamVR_Behaviour>();
			if (steamVR_Behaviour == null)
			{
				steamVR_Behaviour = gameObject.AddComponent<SteamVR_Behaviour>();
			}
			if (steamVR_Render != null)
			{
				steamVR_Behaviour.steamvr_render = steamVR_Render;
			}
			else
			{
				steamVR_Behaviour.steamvr_render = gameObject.GetComponent<SteamVR_Render>();
				if (steamVR_Behaviour.steamvr_render == null)
				{
					steamVR_Behaviour.steamvr_render = gameObject.AddComponent<SteamVR_Render>();
				}
			}
			_instance = steamVR_Behaviour;
		}
		if (_instance != null && _instance.doNotDestroy)
		{
			Object.DontDestroyOnLoad(_instance.transform.root.gameObject);
		}
		initializing = false;
	}

	protected void Awake()
	{
		isPlaying = true;
		if (initializeSteamVROnAwake && !forcingInitialization)
		{
			InitializeSteamVR();
		}
	}

	public void InitializeSteamVR(bool forceUnityVRToOpenVR = false)
	{
		if (forceUnityVRToOpenVR)
		{
			forcingInitialization = true;
			if (initializeCoroutine != null)
			{
				StopCoroutine(initializeCoroutine);
			}
			if (XRSettings.loadedDeviceName == "OpenVR")
			{
				EnableOpenVR();
			}
			else
			{
				initializeCoroutine = StartCoroutine(DoInitializeSteamVR(forceUnityVRToOpenVR));
			}
		}
		else
		{
			SteamVR.Initialize();
		}
	}

	private IEnumerator DoInitializeSteamVR(bool forceUnityVRToOpenVR = false)
	{
		XRSettings.LoadDeviceByName("OpenVR");
		yield return null;
		EnableOpenVR();
	}

	private void EnableOpenVR()
	{
		XRSettings.enabled = true;
		SteamVR.Initialize();
		initializeCoroutine = null;
		forcingInitialization = false;
	}

	protected void OnEnable()
	{
		Application.onBeforeRender += OnBeforeRender;
		SteamVR_Events.System(EVREventType.VREvent_Quit).Listen(OnQuit);
	}

	protected void OnDisable()
	{
		Application.onBeforeRender -= OnBeforeRender;
		SteamVR_Events.System(EVREventType.VREvent_Quit).Remove(OnQuit);
	}

	protected void OnBeforeRender()
	{
		PreCull();
	}

	protected void PreCull()
	{
		if (Time.frameCount != lastFrameCount)
		{
			lastFrameCount = Time.frameCount;
			SteamVR_Input.OnPreCull();
		}
	}

	protected void FixedUpdate()
	{
		SteamVR_Input.FixedUpdate();
	}

	protected void LateUpdate()
	{
		SteamVR_Input.LateUpdate();
	}

	protected void Update()
	{
		SteamVR_Input.Update();
	}

	protected void OnQuit(VREvent_t vrEvent)
	{
		Application.Quit();
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace ZenFulcrum.EmbeddedBrowser;

[RequireComponent(typeof(Browser))]
public abstract class PointerUIBase : MonoBehaviour, IBrowserUI
{
	public struct PointerState
	{
		public int id;

		public bool is2D;

		public Vector2 position2D;

		public Ray position3D;

		public MouseButton activeButtons;

		public Vector2 scrollDelta;
	}

	public readonly KeyEvents keyEvents = new KeyEvents();

	protected Browser browser;

	protected bool appFocused = true;

	protected int currentPointerId;

	protected readonly List<PointerState> currentPointers = new List<PointerState>();

	[Tooltip("When clicking, how far (in browser-space pixels) must the cursor be moved before we send this movement to the browser backend? This helps keep us from unintentionally dragging when we meant to just click, esp. under VR where it's hard to hold the cursor still.")]
	public float dragMovementThreshold;

	protected Vector2 mouseDownPosition;

	protected bool dragging;

	protected int p_currentDown;

	protected int p_anyDown;

	protected int p_currentOver;

	protected int p_anyOver;

	protected bool mouseWasOver;

	protected int focusForceCount;

	[Tooltip("Camera to use to interpret 2D inputs and to originate FPS rays from. Defaults to Camera.main.")]
	public Camera viewCamera;

	public bool enableMouseInput = true;

	public bool enableTouchInput = true;

	public bool enableFPSInput;

	public bool enableVRInput;

	[Tooltip("(For ray-based interaction) How close must you be to the browser to be able to interact with it?")]
	public float maxDistance = float.PositiveInfinity;

	[Space(5f)]
	[Tooltip("Disable Input.simulateMouseWithTouches globally. This will prevent touches from appearing as mouse events.")]
	public bool disableMouseEmulation;

	protected VRBrowserHand[] vrHands;

	public virtual bool MouseHasFocus { get; protected set; }

	public virtual Vector2 MousePosition { get; protected set; }

	public virtual MouseButton MouseButtons { get; protected set; }

	public virtual Vector2 MouseScroll { get; protected set; }

	public virtual bool KeyboardHasFocus { get; protected set; }

	public virtual List<Event> KeyEvents => keyEvents.Events;

	public virtual BrowserCursor BrowserCursor { get; protected set; }

	public virtual BrowserInputSettings InputSettings { get; protected set; }

	public event Action onHandlePointers = delegate
	{
	};

	public event Action onClick = delegate
	{
	};

	public virtual void Awake()
	{
		BrowserCursor = new BrowserCursor();
		BrowserCursor.cursorChange += CursorUpdated;
		InputSettings = new BrowserInputSettings();
		browser = GetComponent<Browser>();
		browser.UIHandler = this;
		onHandlePointers += OnHandlePointers;
		if (disableMouseEmulation)
		{
			Input.simulateMouseWithTouches = false;
		}
	}

	public virtual void InputUpdate()
	{
		keyEvents.InputUpdate();
		p_currentDown = (p_anyDown = (p_currentOver = (p_anyOver = -1)));
		currentPointers.Clear();
		this.onHandlePointers();
		CalculatePointer();
	}

	public void OnApplicationFocus(bool focused)
	{
		appFocused = focused;
	}

	protected abstract Vector2 MapPointerToBrowser(Vector2 screenPosition, int pointerId);

	protected abstract Vector2 MapRayToBrowser(Ray worldRay, int pointerId);

	public abstract void GetCurrentHitLocation(out Vector3 pos, out Quaternion rot);

	public virtual void FeedPointerState(PointerState state)
	{
		if (state.is2D)
		{
			state.position2D = MapPointerToBrowser(state.position2D, state.id);
		}
		else
		{
			Debug.DrawRay(state.position3D.origin, state.position3D.direction * Mathf.Min(500f, maxDistance), Color.cyan);
			state.position2D = MapRayToBrowser(state.position3D, state.id);
		}
		if (float.IsNaN(state.position2D.x))
		{
			return;
		}
		if (state.id == currentPointerId)
		{
			p_currentOver = currentPointers.Count;
			if (state.activeButtons != 0)
			{
				p_currentDown = currentPointers.Count;
			}
		}
		else
		{
			p_anyOver = currentPointers.Count;
			if (state.activeButtons != 0)
			{
				p_anyDown = currentPointers.Count;
			}
		}
		currentPointers.Add(state);
	}

	protected virtual void CalculatePointer()
	{
		PointerState pointerState;
		if (p_currentDown >= 0)
		{
			pointerState = currentPointers[p_currentDown];
		}
		else if (p_anyDown >= 0)
		{
			pointerState = currentPointers[p_anyDown];
		}
		else if (p_currentOver >= 0)
		{
			pointerState = currentPointers[p_currentOver];
		}
		else
		{
			if (p_anyOver < 0)
			{
				MouseIsOff();
				return;
			}
			pointerState = currentPointers[p_anyOver];
		}
		MouseIsOver();
		if (MouseButtons == (MouseButton)0 && pointerState.activeButtons != 0)
		{
			this.onClick();
			dragging = false;
			mouseDownPosition = pointerState.position2D;
		}
		if (float.IsNaN(pointerState.position2D.x))
		{
			Debug.LogError("Using an invalid pointer");
		}
		if (pointerState.activeButtons != 0 || MouseButtons != 0)
		{
			if (!dragging && pointerState.activeButtons != 0)
			{
				Vector2 size = browser.Size;
				float num = Vector2.Distance(Vector2.Scale(pointerState.position2D, size), Vector2.Scale(mouseDownPosition, size));
				if (num > dragMovementThreshold)
				{
					dragging = true;
				}
			}
			if (dragging)
			{
				MousePosition = pointerState.position2D;
			}
			else
			{
				MousePosition = mouseDownPosition;
			}
		}
		else
		{
			MousePosition = pointerState.position2D;
		}
		MouseButtons = pointerState.activeButtons;
		MouseScroll = pointerState.scrollDelta;
		currentPointerId = pointerState.id;
	}

	public void OnGUI()
	{
		keyEvents.Feed(Event.current);
	}

	protected void MouseIsOver()
	{
		MouseHasFocus = true;
		KeyboardHasFocus = true;
		if (BrowserCursor != null)
		{
			CursorUpdated();
		}
		mouseWasOver = true;
	}

	protected void MouseIsOff()
	{
		mouseWasOver = false;
		MouseHasFocus = false;
		if (focusForceCount <= 0)
		{
			KeyboardHasFocus = false;
		}
		MouseButtons = (MouseButton)0;
		MouseScroll = Vector2.zero;
		currentPointerId = 0;
	}

	protected void CursorUpdated()
	{
	}

	public void ForceKeyboardHasFocus(bool force)
	{
		if (force)
		{
			focusForceCount++;
		}
		else
		{
			focusForceCount--;
		}
		if (focusForceCount == 1)
		{
			KeyboardHasFocus = true;
		}
		else if (focusForceCount == 0)
		{
			KeyboardHasFocus = false;
		}
	}

	protected virtual void OnHandlePointers()
	{
		if (enableFPSInput)
		{
			FeedFPS();
		}
		if (enableMouseInput && enableTouchInput && Input.simulateMouseWithTouches && Input.touchCount > 0)
		{
			FeedTouchPointers();
			return;
		}
		if (enableMouseInput)
		{
			FeedMousePointer();
		}
		if (enableTouchInput)
		{
			FeedTouchPointers();
		}
		if (enableVRInput)
		{
			FeedVRPointers();
		}
	}

	protected virtual void FeedTouchPointers()
	{
		for (int i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);
			FeedPointerState(new PointerState
			{
				id = 10 + touch.fingerId,
				is2D = true,
				position2D = touch.position,
				activeButtons = ((touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) ? MouseButton.Left : ((MouseButton)0))
			});
		}
	}

	protected virtual void FeedMousePointer()
	{
		MouseButton mouseButton = (MouseButton)0;
		if (Input.GetMouseButton(0))
		{
			mouseButton |= MouseButton.Left;
		}
		if (Input.GetMouseButton(1))
		{
			mouseButton |= MouseButton.Right;
		}
		if (Input.GetMouseButton(2))
		{
			mouseButton |= MouseButton.Middle;
		}
		FeedPointerState(new PointerState
		{
			id = 1,
			is2D = true,
			position2D = Input.mousePosition,
			activeButtons = mouseButton,
			scrollDelta = Input.mouseScrollDelta
		});
	}

	protected virtual void FeedFPS()
	{
		MouseButton activeButtons = (Input.GetButton("Fire1") ? MouseButton.Left : ((MouseButton)0)) | (Input.GetButton("Fire2") ? MouseButton.Right : ((MouseButton)0)) | (Input.GetButton("Fire3") ? MouseButton.Middle : ((MouseButton)0));
		Camera camera = ((!viewCamera) ? Camera.main : viewCamera);
		Vector2 scrollDelta = Input.mouseScrollDelta;
		if (enableMouseInput)
		{
			scrollDelta = Vector2.zero;
		}
		FeedPointerState(new PointerState
		{
			id = 2,
			is2D = false,
			position3D = new Ray(camera.transform.position, camera.transform.forward),
			activeButtons = activeButtons,
			scrollDelta = scrollDelta
		});
	}

	protected virtual void FeedVRPointers()
	{
		if (vrHands == null)
		{
			vrHands = UnityEngine.Object.FindObjectsOfType<VRBrowserHand>();
			if (vrHands.Length == 0 && XRSettings.enabled)
			{
				Debug.LogWarning("VR input is enabled, but no VRBrowserHands were found in the scene", this);
			}
		}
		for (int i = 0; i < vrHands.Length; i++)
		{
			if (vrHands[i].Tracked)
			{
				FeedPointerState(new PointerState
				{
					id = 100 + i,
					is2D = false,
					position3D = new Ray(vrHands[i].transform.position, vrHands[i].transform.forward),
					activeButtons = vrHands[i].DepressedButtons,
					scrollDelta = vrHands[i].ScrollDelta
				});
			}
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ZenFulcrum.EmbeddedBrowser;

[RequireComponent(typeof(RawImage))]
[RequireComponent(typeof(Browser))]
[Obsolete("Use PointerUIGUI and CursorRendererOS instead.")]
public class GUIBrowserUI : MonoBehaviour, IBrowserUI, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IEventSystemHandler
{
	protected RawImage myImage;

	protected Browser browser;

	public bool enableInput = true;

	public bool autoResize = true;

	protected List<Event> keyEvents = new List<Event>();

	protected List<Event> keyEventsLast = new List<Event>();

	protected BaseRaycaster raycaster;

	protected RectTransform rTransform;

	protected bool _mouseHasFocus;

	protected bool _keyboardHasFocus;

	public bool MouseHasFocus => _mouseHasFocus && enableInput;

	public Vector2 MousePosition { get; private set; }

	public MouseButton MouseButtons { get; private set; }

	public Vector2 MouseScroll { get; private set; }

	public bool KeyboardHasFocus => _keyboardHasFocus && enableInput;

	public List<Event> KeyEvents => keyEventsLast;

	public BrowserCursor BrowserCursor { get; private set; }

	public BrowserInputSettings InputSettings { get; private set; }

	protected void Awake()
	{
		BrowserCursor = new BrowserCursor();
		InputSettings = new BrowserInputSettings();
		browser = GetComponent<Browser>();
		myImage = GetComponent<RawImage>();
		browser.afterResize += UpdateTexture;
		browser.UIHandler = this;
		BrowserCursor.cursorChange += delegate
		{
			SetCursor(BrowserCursor);
		};
		rTransform = GetComponent<RectTransform>();
	}

	protected void OnEnable()
	{
		if (autoResize)
		{
			StartCoroutine(WatchResize());
		}
	}

	private IEnumerator WatchResize()
	{
		Rect currentSize = default(Rect);
		while (base.enabled)
		{
			Rect rect = rTransform.rect;
			if (rect.size.x <= 0f || rect.size.y <= 0f)
			{
				rect.size = new Vector2(512f, 512f);
			}
			if (rect.size != currentSize.size)
			{
				browser.Resize((int)rect.size.x, (int)rect.size.y);
				currentSize = rect;
			}
			yield return null;
		}
	}

	protected void UpdateTexture(Texture2D texture)
	{
		myImage.texture = texture;
		myImage.uvRect = new Rect(0f, 0f, 1f, 1f);
	}

	public virtual void InputUpdate()
	{
		List<Event> list = keyEvents;
		keyEvents = keyEventsLast;
		keyEventsLast = list;
		keyEvents.Clear();
		if (MouseHasFocus)
		{
			if (!raycaster)
			{
				raycaster = GetComponentInParent<BaseRaycaster>();
			}
			RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)base.transform, Input.mousePosition, raycaster.eventCamera, out var localPoint);
			localPoint.x = localPoint.x / rTransform.rect.width + rTransform.pivot.x;
			localPoint.y = localPoint.y / rTransform.rect.height + rTransform.pivot.y;
			MousePosition = localPoint;
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
			MouseButtons = mouseButton;
			MouseScroll = Input.mouseScrollDelta;
		}
		else
		{
			MouseButtons = (MouseButton)0;
		}
		if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
		{
			keyEventsLast.Insert(0, new Event
			{
				type = EventType.KeyDown,
				keyCode = KeyCode.LeftShift
			});
		}
		if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
		{
			keyEventsLast.Add(new Event
			{
				type = EventType.KeyUp,
				keyCode = KeyCode.LeftShift
			});
		}
	}

	public void OnGUI()
	{
		Event current = Event.current;
		if (current.type == EventType.KeyDown || current.type == EventType.KeyUp)
		{
			keyEvents.Add(new Event(current));
		}
	}

	protected virtual void SetCursor(BrowserCursor newCursor)
	{
		if (_mouseHasFocus || newCursor == null)
		{
			if (newCursor == null)
			{
				Cursor.visible = true;
				Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
			}
			else if (newCursor.Texture != null)
			{
				Cursor.visible = true;
				Cursor.SetCursor(newCursor.Texture, newCursor.Hotspot, CursorMode.Auto);
			}
			else
			{
				Cursor.visible = false;
				Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
			}
		}
	}

	public void OnSelect(BaseEventData eventData)
	{
		_keyboardHasFocus = true;
	}

	public void OnDeselect(BaseEventData eventData)
	{
		_keyboardHasFocus = false;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		_mouseHasFocus = true;
		SetCursor(BrowserCursor);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		_mouseHasFocus = false;
		SetCursor(null);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		EventSystem.current.SetSelectedGameObject(base.gameObject);
	}
}

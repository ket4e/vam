using System.Collections.Generic;
using UnityEngine;

namespace ZenFulcrum.EmbeddedBrowser;

internal class BrowserInput
{
	private class ButtonHistory
	{
		public float lastPressTime;

		public int repeatCount;

		public Vector3 lastPosition;

		public void ButtonPress(Vector3 mousePos, IBrowserUI uiHandler, Vector2 browserSize)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (realtimeSinceStartup - lastPressTime > uiHandler.InputSettings.multiclickSpeed)
			{
				repeatCount = 0;
			}
			if (repeatCount > 0)
			{
				Vector2 a = Vector2.Scale(mousePos, browserSize);
				Vector2 b = Vector2.Scale(lastPosition, browserSize);
				if (Vector2.Distance(a, b) > uiHandler.InputSettings.multiclickTolerance)
				{
					repeatCount = 0;
				}
			}
			repeatCount++;
			lastPressTime = realtimeSinceStartup;
			lastPosition = mousePos;
		}
	}

	private readonly Browser browser;

	private bool kbWasFocused;

	private bool mouseWasFocused;

	private static HashSet<KeyCode> keysToReleaseOnFocusLoss = new HashSet<KeyCode>();

	public List<Event> extraEventsToInject = new List<Event>();

	private MouseButton prevButtons;

	private Vector2 prevPos;

	private readonly ButtonHistory leftClickHistory = new ButtonHistory();

	private Vector2 accumulatedScroll;

	private float lastScrollEvent;

	private const float maxScrollEventRate = 1f / 15f;

	public BrowserInput(Browser browser)
	{
		this.browser = browser;
	}

	public void HandleInput()
	{
		browser.UIHandler.InputUpdate();
		bool flag = false;
		if (browser.UIHandler.MouseHasFocus || mouseWasFocused)
		{
			HandleMouseInput();
		}
		if (browser.UIHandler.MouseHasFocus != mouseWasFocused)
		{
			browser.UIHandler.BrowserCursor.HasMouse = browser.UIHandler.MouseHasFocus;
			flag = true;
		}
		mouseWasFocused = browser.UIHandler.MouseHasFocus;
		if (kbWasFocused != browser.UIHandler.KeyboardHasFocus)
		{
			flag = true;
		}
		if (browser.UIHandler.KeyboardHasFocus)
		{
			if (!kbWasFocused)
			{
				BrowserNative.zfb_setFocused(browser.browserId, kbWasFocused = true);
			}
			HandleKeyInput();
		}
		else if (kbWasFocused)
		{
			BrowserNative.zfb_setFocused(browser.browserId, kbWasFocused = false);
		}
		if (flag)
		{
			browser._RaiseFocusEvent(browser.UIHandler.MouseHasFocus, browser.UIHandler.KeyboardHasFocus);
		}
	}

	private void HandleMouseInput()
	{
		IBrowserUI uIHandler = browser.UIHandler;
		Vector2 mousePosition = uIHandler.MousePosition;
		MouseButton mouseButtons = uIHandler.MouseButtons;
		Vector2 mouseScroll = uIHandler.MouseScroll;
		if (mousePosition != prevPos)
		{
			BrowserNative.zfb_mouseMove(browser.browserId, mousePosition.x, 1f - mousePosition.y);
		}
		FeedScrolling(mouseScroll, uIHandler.InputSettings.scrollSpeed);
		bool flag = (prevButtons & MouseButton.Left) != (mouseButtons & MouseButton.Left);
		bool flag2 = (mouseButtons & MouseButton.Left) == MouseButton.Left;
		bool flag3 = (prevButtons & MouseButton.Middle) != (mouseButtons & MouseButton.Middle);
		bool down = (mouseButtons & MouseButton.Middle) == MouseButton.Middle;
		bool flag4 = (prevButtons & MouseButton.Right) != (mouseButtons & MouseButton.Right);
		bool down2 = (mouseButtons & MouseButton.Right) == MouseButton.Right;
		if (flag)
		{
			if (flag2)
			{
				leftClickHistory.ButtonPress(mousePosition, uIHandler, browser.Size);
			}
			BrowserNative.zfb_mouseButton(browser.browserId, BrowserNative.MouseButton.MBT_LEFT, flag2, flag2 ? leftClickHistory.repeatCount : 0);
		}
		if (flag3)
		{
			BrowserNative.zfb_mouseButton(browser.browserId, BrowserNative.MouseButton.MBT_MIDDLE, down, 1);
		}
		if (flag4)
		{
			BrowserNative.zfb_mouseButton(browser.browserId, BrowserNative.MouseButton.MBT_RIGHT, down2, 1);
		}
		prevPos = mousePosition;
		prevButtons = mouseButtons;
	}

	private void FeedScrolling(Vector2 mouseScroll, float scrollSpeed)
	{
		accumulatedScroll += mouseScroll * scrollSpeed;
		if (accumulatedScroll.sqrMagnitude != 0f && Time.realtimeSinceStartup > lastScrollEvent + 1f / 15f)
		{
			if (Mathf.Abs(accumulatedScroll.x) > Mathf.Abs(accumulatedScroll.y))
			{
				BrowserNative.zfb_mouseScroll(browser.browserId, (int)accumulatedScroll.x, 0);
				accumulatedScroll.x = 0f;
				accumulatedScroll.y = Mathf.Round(accumulatedScroll.y * 0.5f);
			}
			else
			{
				BrowserNative.zfb_mouseScroll(browser.browserId, 0, (int)accumulatedScroll.y);
				accumulatedScroll.x = Mathf.Round(accumulatedScroll.x * 0.5f);
				accumulatedScroll.y = 0f;
			}
			lastScrollEvent = Time.realtimeSinceStartup;
		}
	}

	private void HandleKeyInput()
	{
		List<Event> keyEvents = browser.UIHandler.KeyEvents;
		if (keyEvents.Count > 0)
		{
			HandleKeyInput(keyEvents);
		}
		if (extraEventsToInject.Count > 0)
		{
			HandleKeyInput(extraEventsToInject);
			extraEventsToInject.Clear();
		}
	}

	private void HandleKeyInput(List<Event> keyEvents)
	{
		foreach (Event keyEvent in keyEvents)
		{
			int windowsKeyCode = KeyMappings.GetWindowsKeyCode(keyEvent);
			if (keyEvent.character == '\n')
			{
				keyEvent.character = '\r';
			}
			if (keyEvent.character == '\0')
			{
				if (keyEvent.type == EventType.KeyDown)
				{
					keysToReleaseOnFocusLoss.Add(keyEvent.keyCode);
				}
				else
				{
					keysToReleaseOnFocusLoss.Remove(keyEvent.keyCode);
				}
			}
			FireCommands(keyEvent);
			if (keyEvent.character != 0 && keyEvent.type == EventType.KeyDown)
			{
				BrowserNative.zfb_characterEvent(browser.browserId, keyEvent.character, windowsKeyCode);
			}
			else
			{
				BrowserNative.zfb_keyEvent(browser.browserId, keyEvent.type == EventType.KeyDown, windowsKeyCode);
			}
		}
	}

	public void HandleFocusLoss()
	{
		foreach (KeyCode item in keysToReleaseOnFocusLoss)
		{
			Event @event = new Event();
			@event.keyCode = item;
			int windowsKeyCode = KeyMappings.GetWindowsKeyCode(@event);
			BrowserNative.zfb_keyEvent(browser.browserId, down: false, windowsKeyCode);
		}
		keysToReleaseOnFocusLoss.Clear();
	}

	protected void FireCommands(Event ev)
	{
		if (ev.type == EventType.KeyUp && ev.control)
		{
			KeyCode keyCode = ev.keyCode;
			if (keyCode == KeyCode.A)
			{
				browser.SendFrameCommand(BrowserNative.FrameCommand.SelectAll);
			}
		}
	}
}

using System;
using UnityEngine;
using UnityEngine.UI;

namespace ZenFulcrum.EmbeddedBrowser;

[RequireComponent(typeof(Browser))]
[RequireComponent(typeof(PointerUIBase))]
public class ExternalKeyboard : MonoBehaviour
{
	[Tooltip("Set to true before startup to have the keyboard automatically hook to the browser with the most recently focused text field.")]
	public bool automaticFocus;

	[Tooltip("Browser to start as the focused browser for this keyboard. Not really needed if automaticFocus is on.")]
	public Browser initialBrowser;

	[Tooltip("Set to true to have the keyboard automatically hide when we don't have a text entry box to type into.")]
	public bool hideWhenUnneeded = true;

	protected PointerUIBase activeBrowserUI;

	protected Browser keyboardBrowser;

	protected bool forcingFocus;

	protected Browser _activeBrowser;

	protected virtual Browser ActiveBrowser
	{
		get
		{
			return _activeBrowser;
		}
		set
		{
			_SetActiveBrowser(value);
			DoFocus(_activeBrowser);
		}
	}

	public event Action<Browser, bool> onFocusChange = delegate
	{
	};

	protected void _SetActiveBrowser(Browser browser)
	{
		if ((bool)ActiveBrowser && (bool)activeBrowserUI && forcingFocus)
		{
			activeBrowserUI.ForceKeyboardHasFocus(force: false);
			forcingFocus = false;
		}
		_activeBrowser = browser;
		activeBrowserUI = ActiveBrowser.GetComponent<PointerUIBase>();
		if (!activeBrowserUI)
		{
			Debug.LogWarning("Browser does not have a PointerUI, external keyboard may not work properly.");
		}
	}

	public void Awake()
	{
		string text = Resources.Load<TextAsset>("Browser/Keyboard").text;
		keyboardBrowser = GetComponent<Browser>();
		keyboardBrowser.onBrowserFocus += OnKeyboardFocus;
		keyboardBrowser.LoadHTML(text);
		keyboardBrowser.RegisterFunction("textTyped", TextTyped);
		keyboardBrowser.RegisterFunction("commandEntered", CommandEntered);
		if ((bool)initialBrowser)
		{
			ActiveBrowser = initialBrowser;
		}
		if (automaticFocus)
		{
			Browser[] array = UnityEngine.Object.FindObjectsOfType<Browser>();
			foreach (Browser browser in array)
			{
				ObserveBrowser(browser);
			}
			Browser.onAnyBrowserCreated += ObserveBrowser;
		}
		if (hideWhenUnneeded)
		{
			SetVisible(visible: false);
		}
	}

	public void OnDisable()
	{
		Browser.onAnyBrowserCreated -= ObserveBrowser;
	}

	protected void ObserveBrowser(Browser browser)
	{
		if (browser == keyboardBrowser)
		{
			return;
		}
		browser.onNodeFocus += delegate
		{
			if ((bool)this && (!(browser != ActiveBrowser) || browser.focusState.hasMouseFocus))
			{
				DoFocus(browser);
			}
		};
		PointerUIBase component = browser.GetComponent<PointerUIBase>();
		if ((bool)component)
		{
			component.onClick += delegate
			{
				DoFocus(browser);
			};
		}
	}

	protected void DoFocus(Browser browser)
	{
		if (browser != ActiveBrowser)
		{
			_SetActiveBrowser(browser);
		}
		bool flag = (bool)browser && browser.focusState.focusedNodeEditable;
		if (hideWhenUnneeded)
		{
			SetVisible(flag);
		}
		this.onFocusChange(_activeBrowser, flag);
	}

	protected void SetVisible(bool visible)
	{
		Renderer component = GetComponent<Renderer>();
		if ((bool)component)
		{
			component.enabled = visible;
		}
		Collider component2 = GetComponent<Collider>();
		if ((bool)component2)
		{
			component2.enabled = visible;
		}
		RawImage component3 = GetComponent<RawImage>();
		if ((bool)component3)
		{
			component3.enabled = visible;
		}
		PointerUIGUI component4 = GetComponent<PointerUIGUI>();
		if ((bool)component4)
		{
			component4.enableInput = visible;
		}
	}

	protected void OnKeyboardFocus(bool mouseFocused, bool kbFocused)
	{
		if ((bool)activeBrowserUI)
		{
			if ((mouseFocused || kbFocused) && !forcingFocus)
			{
				activeBrowserUI.ForceKeyboardHasFocus(force: true);
				forcingFocus = true;
			}
			if (!mouseFocused && !kbFocused && forcingFocus)
			{
				activeBrowserUI.ForceKeyboardHasFocus(force: false);
				forcingFocus = false;
			}
		}
	}

	protected void CommandEntered(JSONNode args)
	{
		if ((bool)ActiveBrowser)
		{
			string text = args[0];
			bool flag = args[1];
			if (flag)
			{
				ActiveBrowser.PressKey(KeyCode.LeftShift, KeyAction.Press);
			}
			switch (text)
			{
			case "backspace":
				ActiveBrowser.PressKey(KeyCode.Backspace);
				break;
			case "copy":
				ActiveBrowser.SendFrameCommand(BrowserNative.FrameCommand.Copy);
				break;
			case "cut":
				ActiveBrowser.SendFrameCommand(BrowserNative.FrameCommand.Cut);
				break;
			case "delete":
				ActiveBrowser.PressKey(KeyCode.Delete);
				break;
			case "down":
				ActiveBrowser.PressKey(KeyCode.DownArrow);
				break;
			case "end":
				ActiveBrowser.PressKey(KeyCode.End);
				break;
			case "home":
				ActiveBrowser.PressKey(KeyCode.Home);
				break;
			case "insert":
				ActiveBrowser.PressKey(KeyCode.Insert);
				break;
			case "left":
				ActiveBrowser.PressKey(KeyCode.LeftArrow);
				break;
			case "pageDown":
				ActiveBrowser.PressKey(KeyCode.PageDown);
				break;
			case "pageUp":
				ActiveBrowser.PressKey(KeyCode.PageUp);
				break;
			case "paste":
				ActiveBrowser.SendFrameCommand(BrowserNative.FrameCommand.Paste);
				break;
			case "redo":
				ActiveBrowser.SendFrameCommand(BrowserNative.FrameCommand.Redo);
				break;
			case "right":
				ActiveBrowser.PressKey(KeyCode.RightArrow);
				break;
			case "selectAll":
				ActiveBrowser.SendFrameCommand(BrowserNative.FrameCommand.SelectAll);
				break;
			case "undo":
				ActiveBrowser.SendFrameCommand(BrowserNative.FrameCommand.Undo);
				break;
			case "up":
				ActiveBrowser.PressKey(KeyCode.UpArrow);
				break;
			case "wordLeft":
				ActiveBrowser.PressKey(KeyCode.LeftControl, KeyAction.Press);
				ActiveBrowser.PressKey(KeyCode.LeftArrow);
				ActiveBrowser.PressKey(KeyCode.LeftControl, KeyAction.Release);
				break;
			case "wordRight":
				ActiveBrowser.PressKey(KeyCode.LeftControl, KeyAction.Press);
				ActiveBrowser.PressKey(KeyCode.RightArrow);
				ActiveBrowser.PressKey(KeyCode.LeftControl, KeyAction.Release);
				break;
			}
			if (flag)
			{
				ActiveBrowser.PressKey(KeyCode.LeftShift, KeyAction.Release);
			}
		}
	}

	protected void TextTyped(JSONNode args)
	{
		if ((bool)ActiveBrowser)
		{
			ActiveBrowser.TypeText(args[0]);
		}
	}
}

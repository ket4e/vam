using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>A UnityGUI event.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public sealed class Event
{
	[NonSerialized]
	internal IntPtr m_Ptr;

	private static Event s_Current;

	private static Event s_MasterEvent;

	public extern EventType rawType
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>The type of event.</para>
	/// </summary>
	public extern EventType type
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Which mouse button was pressed.</para>
	/// </summary>
	public extern int button
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Which modifier keys are held down.</para>
	/// </summary>
	public extern EventModifiers modifiers
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public extern float pressure
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>How many consecutive mouse clicks have we received.</para>
	/// </summary>
	public extern int clickCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The character typed.</para>
	/// </summary>
	public extern char character
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The name of an ExecuteCommand or ValidateCommand Event.</para>
	/// </summary>
	public extern string commandName
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The raw key code for keyboard events.</para>
	/// </summary>
	public extern KeyCode keyCode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Index of display that the event belongs to.</para>
	/// </summary>
	public extern int displayIndex
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The mouse position.</para>
	/// </summary>
	public Vector2 mousePosition
	{
		get
		{
			Internal_GetMousePosition(out var value);
			return value;
		}
		set
		{
			Internal_SetMousePosition(value);
		}
	}

	/// <summary>
	///   <para>The relative movement of the mouse compared to last event.</para>
	/// </summary>
	public Vector2 delta
	{
		get
		{
			Internal_GetMouseDelta(out var value);
			return value;
		}
		set
		{
			Internal_SetMouseDelta(value);
		}
	}

	[Obsolete("Use HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);", true)]
	public Ray mouseRay
	{
		get
		{
			return new Ray(Vector3.up, Vector3.up);
		}
		set
		{
		}
	}

	/// <summary>
	///   <para>Is Shift held down? (Read Only)</para>
	/// </summary>
	public bool shift
	{
		get
		{
			return (modifiers & EventModifiers.Shift) != 0;
		}
		set
		{
			if (!value)
			{
				modifiers &= ~EventModifiers.Shift;
			}
			else
			{
				modifiers |= EventModifiers.Shift;
			}
		}
	}

	/// <summary>
	///   <para>Is Control key held down? (Read Only)</para>
	/// </summary>
	public bool control
	{
		get
		{
			return (modifiers & EventModifiers.Control) != 0;
		}
		set
		{
			if (!value)
			{
				modifiers &= ~EventModifiers.Control;
			}
			else
			{
				modifiers |= EventModifiers.Control;
			}
		}
	}

	/// <summary>
	///   <para>Is Alt/Option key held down? (Read Only)</para>
	/// </summary>
	public bool alt
	{
		get
		{
			return (modifiers & EventModifiers.Alt) != 0;
		}
		set
		{
			if (!value)
			{
				modifiers &= ~EventModifiers.Alt;
			}
			else
			{
				modifiers |= EventModifiers.Alt;
			}
		}
	}

	/// <summary>
	///   <para>Is Command/Windows key held down? (Read Only)</para>
	/// </summary>
	public bool command
	{
		get
		{
			return (modifiers & EventModifiers.Command) != 0;
		}
		set
		{
			if (!value)
			{
				modifiers &= ~EventModifiers.Command;
			}
			else
			{
				modifiers |= EventModifiers.Command;
			}
		}
	}

	/// <summary>
	///   <para>Is Caps Lock on? (Read Only)</para>
	/// </summary>
	public bool capsLock
	{
		get
		{
			return (modifiers & EventModifiers.CapsLock) != 0;
		}
		set
		{
			if (!value)
			{
				modifiers &= ~EventModifiers.CapsLock;
			}
			else
			{
				modifiers |= EventModifiers.CapsLock;
			}
		}
	}

	/// <summary>
	///   <para>Is the current keypress on the numeric keyboard? (Read Only)</para>
	/// </summary>
	public bool numeric
	{
		get
		{
			return (modifiers & EventModifiers.Numeric) != 0;
		}
		set
		{
			if (!value)
			{
				modifiers &= ~EventModifiers.Numeric;
			}
			else
			{
				modifiers |= EventModifiers.Numeric;
			}
		}
	}

	/// <summary>
	///   <para>Is the current keypress a function key? (Read Only)</para>
	/// </summary>
	public bool functionKey => (modifiers & EventModifiers.FunctionKey) != 0;

	/// <summary>
	///   <para>The current event that's being processed right now.</para>
	/// </summary>
	public static Event current
	{
		get
		{
			return s_Current;
		}
		set
		{
			if (value != null)
			{
				s_Current = value;
			}
			else
			{
				s_Current = s_MasterEvent;
			}
			Internal_SetNativeEvent(s_Current.m_Ptr);
		}
	}

	/// <summary>
	///   <para>Is this event a keyboard event? (Read Only)</para>
	/// </summary>
	public bool isKey
	{
		get
		{
			EventType eventType = type;
			return eventType == EventType.KeyDown || eventType == EventType.KeyUp;
		}
	}

	/// <summary>
	///   <para>Is this event a mouse event? (Read Only)</para>
	/// </summary>
	public bool isMouse
	{
		get
		{
			EventType eventType = type;
			return eventType == EventType.MouseMove || eventType == EventType.MouseDown || eventType == EventType.MouseUp || eventType == EventType.MouseDrag || eventType == EventType.ContextClick || eventType == EventType.MouseEnterWindow || eventType == EventType.MouseLeaveWindow;
		}
	}

	public bool isScrollWheel
	{
		get
		{
			EventType eventType = type;
			return eventType == EventType.ScrollWheel;
		}
	}

	public Event()
	{
		Init(0);
	}

	public Event(int displayIndex)
	{
		Init(displayIndex);
	}

	public Event(Event other)
	{
		if (other == null)
		{
			throw new ArgumentException("Event to copy from is null.");
		}
		InitCopy(other);
	}

	private Event(IntPtr ptr)
	{
		InitPtr(ptr);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	private extern void Init(int displayIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	private extern void Cleanup();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	private extern void InitCopy(Event other);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	private extern void InitPtr(IntPtr ptr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	internal extern void CopyFromPtr(IntPtr ptr);

	/// <summary>
	///   <para>Get a filtered event type for a given control ID.</para>
	/// </summary>
	/// <param name="controlID">The ID of the control you are querying from.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern EventType GetTypeForControl(int controlID);

	private void Internal_SetMousePosition(Vector2 value)
	{
		INTERNAL_CALL_Internal_SetMousePosition(this, ref value);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Internal_SetMousePosition(Event self, ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void Internal_GetMousePosition(out Vector2 value);

	private void Internal_SetMouseDelta(Vector2 value)
	{
		INTERNAL_CALL_Internal_SetMouseDelta(this, ref value);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Internal_SetMouseDelta(Event self, ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void Internal_GetMouseDelta(out Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_SetNativeEvent(IntPtr ptr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void Internal_Use();

	/// <summary>
	///   <para>Get the next queued [Event] from the event system.</para>
	/// </summary>
	/// <param name="outEvent">Next Event.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern bool PopEvent(Event outEvent);

	/// <summary>
	///   <para>Returns the current number of events that are stored in the event queue.</para>
	/// </summary>
	/// <returns>
	///   <para>Current number of events currently in the event queue.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern int GetEventCount();

	~Event()
	{
		Cleanup();
	}

	internal static void CleanupRoots()
	{
		s_Current = null;
		s_MasterEvent = null;
	}

	[RequiredByNativeCode]
	private static void Internal_MakeMasterEventCurrent(int displayIndex)
	{
		if (s_MasterEvent == null)
		{
			s_MasterEvent = new Event(displayIndex);
		}
		s_MasterEvent.displayIndex = displayIndex;
		s_Current = s_MasterEvent;
		Internal_SetNativeEvent(s_MasterEvent.m_Ptr);
	}

	/// <summary>
	///   <para>Create a keyboard event.</para>
	/// </summary>
	/// <param name="key"></param>
	public static Event KeyboardEvent(string key)
	{
		Event @event = new Event(0);
		@event.type = EventType.KeyDown;
		if (string.IsNullOrEmpty(key))
		{
			return @event;
		}
		int num = 0;
		bool flag = false;
		do
		{
			flag = true;
			if (num >= key.Length)
			{
				flag = false;
				break;
			}
			switch (key[num])
			{
			case '&':
				@event.modifiers |= EventModifiers.Alt;
				num++;
				break;
			case '^':
				@event.modifiers |= EventModifiers.Control;
				num++;
				break;
			case '%':
				@event.modifiers |= EventModifiers.Command;
				num++;
				break;
			case '#':
				@event.modifiers |= EventModifiers.Shift;
				num++;
				break;
			default:
				flag = false;
				break;
			}
		}
		while (flag);
		string text = key.Substring(num, key.Length - num).ToLower();
		switch (text)
		{
		case "[0]":
			@event.character = '0';
			@event.keyCode = KeyCode.Keypad0;
			break;
		case "[1]":
			@event.character = '1';
			@event.keyCode = KeyCode.Keypad1;
			break;
		case "[2]":
			@event.character = '2';
			@event.keyCode = KeyCode.Keypad2;
			break;
		case "[3]":
			@event.character = '3';
			@event.keyCode = KeyCode.Keypad3;
			break;
		case "[4]":
			@event.character = '4';
			@event.keyCode = KeyCode.Keypad4;
			break;
		case "[5]":
			@event.character = '5';
			@event.keyCode = KeyCode.Keypad5;
			break;
		case "[6]":
			@event.character = '6';
			@event.keyCode = KeyCode.Keypad6;
			break;
		case "[7]":
			@event.character = '7';
			@event.keyCode = KeyCode.Keypad7;
			break;
		case "[8]":
			@event.character = '8';
			@event.keyCode = KeyCode.Keypad8;
			break;
		case "[9]":
			@event.character = '9';
			@event.keyCode = KeyCode.Keypad9;
			break;
		case "[.]":
			@event.character = '.';
			@event.keyCode = KeyCode.KeypadPeriod;
			break;
		case "[/]":
			@event.character = '/';
			@event.keyCode = KeyCode.KeypadDivide;
			break;
		case "[-]":
			@event.character = '-';
			@event.keyCode = KeyCode.KeypadMinus;
			break;
		case "[+]":
			@event.character = '+';
			@event.keyCode = KeyCode.KeypadPlus;
			break;
		case "[=]":
			@event.character = '=';
			@event.keyCode = KeyCode.KeypadEquals;
			break;
		case "[equals]":
			@event.character = '=';
			@event.keyCode = KeyCode.KeypadEquals;
			break;
		case "[enter]":
			@event.character = '\n';
			@event.keyCode = KeyCode.KeypadEnter;
			break;
		case "up":
			@event.keyCode = KeyCode.UpArrow;
			@event.modifiers |= EventModifiers.FunctionKey;
			break;
		case "down":
			@event.keyCode = KeyCode.DownArrow;
			@event.modifiers |= EventModifiers.FunctionKey;
			break;
		case "left":
			@event.keyCode = KeyCode.LeftArrow;
			@event.modifiers |= EventModifiers.FunctionKey;
			break;
		case "right":
			@event.keyCode = KeyCode.RightArrow;
			@event.modifiers |= EventModifiers.FunctionKey;
			break;
		case "insert":
			@event.keyCode = KeyCode.Insert;
			@event.modifiers |= EventModifiers.FunctionKey;
			break;
		case "home":
			@event.keyCode = KeyCode.Home;
			@event.modifiers |= EventModifiers.FunctionKey;
			break;
		case "end":
			@event.keyCode = KeyCode.End;
			@event.modifiers |= EventModifiers.FunctionKey;
			break;
		case "pgup":
			@event.keyCode = KeyCode.PageDown;
			@event.modifiers |= EventModifiers.FunctionKey;
			break;
		case "page up":
			@event.keyCode = KeyCode.PageUp;
			@event.modifiers |= EventModifiers.FunctionKey;
			break;
		case "pgdown":
			@event.keyCode = KeyCode.PageUp;
			@event.modifiers |= EventModifiers.FunctionKey;
			break;
		case "page down":
			@event.keyCode = KeyCode.PageDown;
			@event.modifiers |= EventModifiers.FunctionKey;
			break;
		case "backspace":
			@event.keyCode = KeyCode.Backspace;
			@event.modifiers |= EventModifiers.FunctionKey;
			break;
		case "delete":
			@event.keyCode = KeyCode.Delete;
			@event.modifiers |= EventModifiers.FunctionKey;
			break;
		case "tab":
			@event.keyCode = KeyCode.Tab;
			break;
		case "f1":
			@event.keyCode = KeyCode.F1;
			@event.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f2":
			@event.keyCode = KeyCode.F2;
			@event.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f3":
			@event.keyCode = KeyCode.F3;
			@event.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f4":
			@event.keyCode = KeyCode.F4;
			@event.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f5":
			@event.keyCode = KeyCode.F5;
			@event.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f6":
			@event.keyCode = KeyCode.F6;
			@event.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f7":
			@event.keyCode = KeyCode.F7;
			@event.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f8":
			@event.keyCode = KeyCode.F8;
			@event.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f9":
			@event.keyCode = KeyCode.F9;
			@event.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f10":
			@event.keyCode = KeyCode.F10;
			@event.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f11":
			@event.keyCode = KeyCode.F11;
			@event.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f12":
			@event.keyCode = KeyCode.F12;
			@event.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f13":
			@event.keyCode = KeyCode.F13;
			@event.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f14":
			@event.keyCode = KeyCode.F14;
			@event.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f15":
			@event.keyCode = KeyCode.F15;
			@event.modifiers |= EventModifiers.FunctionKey;
			break;
		case "[esc]":
			@event.keyCode = KeyCode.Escape;
			break;
		case "return":
			@event.character = '\n';
			@event.keyCode = KeyCode.Return;
			@event.modifiers &= ~EventModifiers.FunctionKey;
			break;
		case "space":
			@event.keyCode = KeyCode.Space;
			@event.character = ' ';
			@event.modifiers &= ~EventModifiers.FunctionKey;
			break;
		default:
			if (text.Length != 1)
			{
				try
				{
					@event.keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), text, ignoreCase: true);
				}
				catch (ArgumentException)
				{
					Debug.LogError(UnityString.Format("Unable to find key name that matches '{0}'", text));
				}
			}
			else
			{
				@event.character = text.ToLower()[0];
				@event.keyCode = (KeyCode)@event.character;
				if (@event.modifiers != 0)
				{
					@event.character = '\0';
				}
			}
			break;
		}
		return @event;
	}

	public override int GetHashCode()
	{
		int num = 1;
		if (isKey)
		{
			num = (ushort)keyCode;
		}
		if (isMouse)
		{
			num = mousePosition.GetHashCode();
		}
		return (num * 37) | (int)modifiers;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (object.ReferenceEquals(this, obj))
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		Event @event = (Event)obj;
		if (type != @event.type || (modifiers & ~EventModifiers.CapsLock) != (@event.modifiers & ~EventModifiers.CapsLock))
		{
			return false;
		}
		if (isKey)
		{
			return keyCode == @event.keyCode;
		}
		if (isMouse)
		{
			return mousePosition == @event.mousePosition;
		}
		return false;
	}

	public override string ToString()
	{
		if (isKey)
		{
			if (character == '\0')
			{
				return UnityString.Format("Event:{0}   Character:\\0   Modifiers:{1}   KeyCode:{2}", type, modifiers, keyCode);
			}
			return string.Concat("Event:", type, "   Character:", (int)character, "   Modifiers:", modifiers, "   KeyCode:", keyCode);
		}
		if (isMouse)
		{
			return UnityString.Format("Event: {0}   Position: {1} Modifiers: {2}", type, mousePosition, modifiers);
		}
		if (type == EventType.ExecuteCommand || type == EventType.ValidateCommand)
		{
			return UnityString.Format("Event: {0}  \"{1}\"", type, commandName);
		}
		return "" + type;
	}

	/// <summary>
	///   <para>Use this event.</para>
	/// </summary>
	public void Use()
	{
		if (type == EventType.Repaint || type == EventType.Layout)
		{
			Debug.LogWarning(UnityString.Format("Event.Use() should not be called for events of type {0}", type));
		}
		Internal_Use();
	}
}

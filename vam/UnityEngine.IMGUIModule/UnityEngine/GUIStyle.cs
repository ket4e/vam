using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Styling information for GUI elements.</para>
/// </summary>
[Serializable]
[StructLayout(LayoutKind.Sequential)]
[RequiredByNativeCode]
public sealed class GUIStyle
{
	[NonSerialized]
	internal IntPtr m_Ptr;

	[NonSerialized]
	private GUIStyleState m_Normal;

	[NonSerialized]
	private GUIStyleState m_Hover;

	[NonSerialized]
	private GUIStyleState m_Active;

	[NonSerialized]
	private GUIStyleState m_Focused;

	[NonSerialized]
	private GUIStyleState m_OnNormal;

	[NonSerialized]
	private GUIStyleState m_OnHover;

	[NonSerialized]
	private GUIStyleState m_OnActive;

	[NonSerialized]
	private GUIStyleState m_OnFocused;

	[NonSerialized]
	private RectOffset m_Border;

	[NonSerialized]
	private RectOffset m_Padding;

	[NonSerialized]
	private RectOffset m_Margin;

	[NonSerialized]
	private RectOffset m_Overflow;

	[NonSerialized]
	private Font m_FontInternal;

	internal static bool showKeyboardFocus = true;

	private static GUIStyle s_None;

	/// <summary>
	///   <para>The name of this GUIStyle. Used for getting them based on name.</para>
	/// </summary>
	public extern string name
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>How image and text of the GUIContent is combined.</para>
	/// </summary>
	public extern ImagePosition imagePosition
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Text alignment.</para>
	/// </summary>
	public extern TextAnchor alignment
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Should the text be wordwrapped?</para>
	/// </summary>
	public extern bool wordWrap
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>What to do when the contents to be rendered is too large to fit within the area given.</para>
	/// </summary>
	public extern TextClipping clipping
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Pixel offset to apply to the content of this GUIstyle.</para>
	/// </summary>
	public Vector2 contentOffset
	{
		get
		{
			INTERNAL_get_contentOffset(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_contentOffset(ref value);
		}
	}

	internal Vector2 Internal_clipOffset
	{
		get
		{
			INTERNAL_get_Internal_clipOffset(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_Internal_clipOffset(ref value);
		}
	}

	/// <summary>
	///   <para>If non-0, any GUI elements rendered with this style will have the width specified here.</para>
	/// </summary>
	public extern float fixedWidth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>If non-0, any GUI elements rendered with this style will have the height specified here.</para>
	/// </summary>
	public extern float fixedHeight
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Can GUI elements of this style be stretched horizontally for better layouting?</para>
	/// </summary>
	public extern bool stretchWidth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Can GUI elements of this style be stretched vertically for better layout?</para>
	/// </summary>
	public extern bool stretchHeight
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The font size to use (for dynamic fonts).</para>
	/// </summary>
	public extern int fontSize
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The font style to use (for dynamic fonts).</para>
	/// </summary>
	public extern FontStyle fontStyle
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Enable HTML-style tags for Text Formatting Markup.</para>
	/// </summary>
	public extern bool richText
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Rendering settings for when the component is displayed normally.</para>
	/// </summary>
	public GUIStyleState normal
	{
		get
		{
			if (m_Normal == null)
			{
				m_Normal = GUIStyleState.GetGUIStyleState(this, GetStyleStatePtr(0));
			}
			return m_Normal;
		}
		set
		{
			AssignStyleState(0, value.m_Ptr);
		}
	}

	/// <summary>
	///   <para>Rendering settings for when the mouse is hovering over the control.</para>
	/// </summary>
	public GUIStyleState hover
	{
		get
		{
			if (m_Hover == null)
			{
				m_Hover = GUIStyleState.GetGUIStyleState(this, GetStyleStatePtr(1));
			}
			return m_Hover;
		}
		set
		{
			AssignStyleState(1, value.m_Ptr);
		}
	}

	/// <summary>
	///   <para>Rendering settings for when the control is pressed down.</para>
	/// </summary>
	public GUIStyleState active
	{
		get
		{
			if (m_Active == null)
			{
				m_Active = GUIStyleState.GetGUIStyleState(this, GetStyleStatePtr(2));
			}
			return m_Active;
		}
		set
		{
			AssignStyleState(2, value.m_Ptr);
		}
	}

	/// <summary>
	///   <para>Rendering settings for when the control is turned on.</para>
	/// </summary>
	public GUIStyleState onNormal
	{
		get
		{
			if (m_OnNormal == null)
			{
				m_OnNormal = GUIStyleState.GetGUIStyleState(this, GetStyleStatePtr(4));
			}
			return m_OnNormal;
		}
		set
		{
			AssignStyleState(4, value.m_Ptr);
		}
	}

	/// <summary>
	///   <para>Rendering settings for when the control is turned on and the mouse is hovering it.</para>
	/// </summary>
	public GUIStyleState onHover
	{
		get
		{
			if (m_OnHover == null)
			{
				m_OnHover = GUIStyleState.GetGUIStyleState(this, GetStyleStatePtr(5));
			}
			return m_OnHover;
		}
		set
		{
			AssignStyleState(5, value.m_Ptr);
		}
	}

	/// <summary>
	///   <para>Rendering settings for when the element is turned on and pressed down.</para>
	/// </summary>
	public GUIStyleState onActive
	{
		get
		{
			if (m_OnActive == null)
			{
				m_OnActive = GUIStyleState.GetGUIStyleState(this, GetStyleStatePtr(6));
			}
			return m_OnActive;
		}
		set
		{
			AssignStyleState(6, value.m_Ptr);
		}
	}

	/// <summary>
	///   <para>Rendering settings for when the element has keyboard focus.</para>
	/// </summary>
	public GUIStyleState focused
	{
		get
		{
			if (m_Focused == null)
			{
				m_Focused = GUIStyleState.GetGUIStyleState(this, GetStyleStatePtr(3));
			}
			return m_Focused;
		}
		set
		{
			AssignStyleState(3, value.m_Ptr);
		}
	}

	/// <summary>
	///   <para>Rendering settings for when the element has keyboard and is turned on.</para>
	/// </summary>
	public GUIStyleState onFocused
	{
		get
		{
			if (m_OnFocused == null)
			{
				m_OnFocused = GUIStyleState.GetGUIStyleState(this, GetStyleStatePtr(7));
			}
			return m_OnFocused;
		}
		set
		{
			AssignStyleState(7, value.m_Ptr);
		}
	}

	/// <summary>
	///   <para>The borders of all background images.</para>
	/// </summary>
	public RectOffset border
	{
		get
		{
			if (m_Border == null)
			{
				m_Border = new RectOffset(this, GetRectOffsetPtr(0));
			}
			return m_Border;
		}
		set
		{
			AssignRectOffset(0, value.m_Ptr);
		}
	}

	/// <summary>
	///   <para>The margins between elements rendered in this style and any other GUI elements.</para>
	/// </summary>
	public RectOffset margin
	{
		get
		{
			if (m_Margin == null)
			{
				m_Margin = new RectOffset(this, GetRectOffsetPtr(1));
			}
			return m_Margin;
		}
		set
		{
			AssignRectOffset(1, value.m_Ptr);
		}
	}

	/// <summary>
	///   <para>Space from the edge of GUIStyle to the start of the contents.</para>
	/// </summary>
	public RectOffset padding
	{
		get
		{
			if (m_Padding == null)
			{
				m_Padding = new RectOffset(this, GetRectOffsetPtr(2));
			}
			return m_Padding;
		}
		set
		{
			AssignRectOffset(2, value.m_Ptr);
		}
	}

	/// <summary>
	///   <para>Extra space to be added to the background image.</para>
	/// </summary>
	public RectOffset overflow
	{
		get
		{
			if (m_Overflow == null)
			{
				m_Overflow = new RectOffset(this, GetRectOffsetPtr(3));
			}
			return m_Overflow;
		}
		set
		{
			AssignRectOffset(3, value.m_Ptr);
		}
	}

	[Obsolete("warning Don't use clipOffset - put things inside BeginGroup instead. This functionality will be removed in a later version.")]
	public Vector2 clipOffset
	{
		get
		{
			return Internal_clipOffset;
		}
		set
		{
			Internal_clipOffset = value;
		}
	}

	/// <summary>
	///   <para>The font to use for rendering. If null, the default font for the current GUISkin is used instead.</para>
	/// </summary>
	public Font font
	{
		get
		{
			return GetFontInternal();
		}
		set
		{
			SetFontInternal(value);
			m_FontInternal = value;
		}
	}

	/// <summary>
	///   <para>The height of one line of text with this style, measured in pixels. (Read Only)</para>
	/// </summary>
	public float lineHeight => Mathf.Round(Internal_GetLineHeight(m_Ptr));

	/// <summary>
	///   <para>Shortcut for an empty GUIStyle.</para>
	/// </summary>
	public static GUIStyle none
	{
		get
		{
			if (s_None == null)
			{
				s_None = new GUIStyle();
			}
			return s_None;
		}
	}

	public bool isHeightDependantOnWidth => fixedHeight == 0f && wordWrap && imagePosition != ImagePosition.ImageOnly;

	/// <summary>
	///   <para>Constructor for empty GUIStyle.</para>
	/// </summary>
	public GUIStyle()
	{
		Init();
	}

	/// <summary>
	///   <para>Constructs GUIStyle identical to given other GUIStyle.</para>
	/// </summary>
	/// <param name="other"></param>
	public GUIStyle(GUIStyle other)
	{
		InitCopy(other);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	private extern void Init();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	private extern void InitCopy(GUIStyle other);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	private extern void Cleanup();

	[ThreadAndSerializationSafe]
	private IntPtr GetStyleStatePtr(int idx)
	{
		INTERNAL_CALL_GetStyleStatePtr(this, idx, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetStyleStatePtr(GUIStyle self, int idx, out IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void AssignStyleState(int idx, IntPtr srcStyleState);

	private IntPtr GetRectOffsetPtr(int idx)
	{
		INTERNAL_CALL_GetRectOffsetPtr(this, idx, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetRectOffsetPtr(GUIStyle self, int idx, out IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void AssignRectOffset(int idx, IntPtr srcRectOffset);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_contentOffset(out Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_contentOffset(ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_Internal_clipOffset(out Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_set_Internal_clipOffset(ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern float Internal_GetLineHeight(IntPtr target);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void SetFontInternal(Font value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	private extern Font GetFontInternalDuringLoadingThread();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern Font GetFontInternal();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_Draw(GUIContent content, ref Internal_DrawArguments arguments);

	private static void Internal_Draw2(IntPtr style, Rect position, GUIContent content, int controlID, bool on)
	{
		INTERNAL_CALL_Internal_Draw2(style, ref position, content, controlID, on);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Internal_Draw2(IntPtr style, ref Rect position, GUIContent content, int controlID, bool on);

	internal static void SetMouseTooltip(string tooltip, Rect screenRect)
	{
		INTERNAL_CALL_SetMouseTooltip(tooltip, ref screenRect);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_SetMouseTooltip(string tooltip, ref Rect screenRect);

	private static void Internal_DrawPrefixLabel(IntPtr style, Rect position, GUIContent content, int controlID, bool on)
	{
		INTERNAL_CALL_Internal_DrawPrefixLabel(style, ref position, content, controlID, on);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Internal_DrawPrefixLabel(IntPtr style, ref Rect position, GUIContent content, int controlID, bool on);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern float Internal_GetCursorFlashOffset();

	private static void Internal_DrawCursor(IntPtr target, Rect position, GUIContent content, int pos, Color cursorColor)
	{
		INTERNAL_CALL_Internal_DrawCursor(target, ref position, content, pos, ref cursorColor);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Internal_DrawCursor(IntPtr target, ref Rect position, GUIContent content, int pos, ref Color cursorColor);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_DrawWithTextSelection(GUIContent content, ref Internal_DrawWithTextSelectionArguments arguments);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal static extern void SetDefaultFont(Font font);

	internal static void Internal_GetCursorPixelPosition(IntPtr target, Rect position, GUIContent content, int cursorStringIndex, out Vector2 ret)
	{
		INTERNAL_CALL_Internal_GetCursorPixelPosition(target, ref position, content, cursorStringIndex, out ret);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Internal_GetCursorPixelPosition(IntPtr target, ref Rect position, GUIContent content, int cursorStringIndex, out Vector2 ret);

	internal static int Internal_GetCursorStringIndex(IntPtr target, Rect position, GUIContent content, Vector2 cursorPixelPosition)
	{
		return INTERNAL_CALL_Internal_GetCursorStringIndex(target, ref position, content, ref cursorPixelPosition);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern int INTERNAL_CALL_Internal_GetCursorStringIndex(IntPtr target, ref Rect position, GUIContent content, ref Vector2 cursorPixelPosition);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal static extern int Internal_GetNumCharactersThatFitWithinWidth(IntPtr target, string text, float width);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal static extern void Internal_CalcSize(IntPtr target, GUIContent content, out Vector2 ret);

	internal static void Internal_CalcSizeWithConstraints(IntPtr target, GUIContent content, Vector2 maxSize, out Vector2 ret)
	{
		INTERNAL_CALL_Internal_CalcSizeWithConstraints(target, content, ref maxSize, out ret);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Internal_CalcSizeWithConstraints(IntPtr target, GUIContent content, ref Vector2 maxSize, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern float Internal_CalcHeight(IntPtr target, GUIContent content, float width);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_CalcMinMaxWidth(IntPtr target, GUIContent content, out float minWidth, out float maxWidth);

	~GUIStyle()
	{
		Cleanup();
	}

	internal static void CleanupRoots()
	{
		s_None = null;
	}

	internal void InternalOnAfterDeserialize()
	{
		m_FontInternal = GetFontInternalDuringLoadingThread();
		m_Normal = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, GetStyleStatePtr(0));
		m_Hover = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, GetStyleStatePtr(1));
		m_Active = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, GetStyleStatePtr(2));
		m_Focused = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, GetStyleStatePtr(3));
		m_OnNormal = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, GetStyleStatePtr(4));
		m_OnHover = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, GetStyleStatePtr(5));
		m_OnActive = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, GetStyleStatePtr(6));
		m_OnFocused = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, GetStyleStatePtr(7));
	}

	private static void Internal_Draw(IntPtr target, Rect position, GUIContent content, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
	{
		Internal_DrawArguments arguments = default(Internal_DrawArguments);
		arguments.target = target;
		arguments.position = position;
		arguments.isHover = (isHover ? 1 : 0);
		arguments.isActive = (isActive ? 1 : 0);
		arguments.on = (on ? 1 : 0);
		arguments.hasKeyboardFocus = (hasKeyboardFocus ? 1 : 0);
		Internal_Draw(content, ref arguments);
	}

	/// <summary>
	///   <para>Draw this GUIStyle on to the screen, internal version.</para>
	/// </summary>
	/// <param name="position"></param>
	/// <param name="isHover"></param>
	/// <param name="isActive"></param>
	/// <param name="on"></param>
	/// <param name="hasKeyboardFocus"></param>
	public void Draw(Rect position, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
	{
		Internal_Draw(m_Ptr, position, GUIContent.none, isHover, isActive, on, hasKeyboardFocus);
	}

	/// <summary>
	///   <para>Draw the GUIStyle with a text string inside.</para>
	/// </summary>
	/// <param name="position"></param>
	/// <param name="text"></param>
	/// <param name="isHover"></param>
	/// <param name="isActive"></param>
	/// <param name="on"></param>
	/// <param name="hasKeyboardFocus"></param>
	public void Draw(Rect position, string text, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
	{
		Internal_Draw(m_Ptr, position, GUIContent.Temp(text), isHover, isActive, on, hasKeyboardFocus);
	}

	/// <summary>
	///   <para>Draw the GUIStyle with an image inside. If the image is too large to fit within the content area of the style it is scaled down.</para>
	/// </summary>
	/// <param name="position"></param>
	/// <param name="image"></param>
	/// <param name="isHover"></param>
	/// <param name="isActive"></param>
	/// <param name="on"></param>
	/// <param name="hasKeyboardFocus"></param>
	public void Draw(Rect position, Texture image, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
	{
		Internal_Draw(m_Ptr, position, GUIContent.Temp(image), isHover, isActive, on, hasKeyboardFocus);
	}

	/// <summary>
	///   <para>Draw the GUIStyle with text and an image inside. If the image is too large to fit within the content area of the style it is scaled down.</para>
	/// </summary>
	/// <param name="position"></param>
	/// <param name="content"></param>
	/// <param name="controlID"></param>
	/// <param name="on"></param>
	/// <param name="isHover"></param>
	/// <param name="isActive"></param>
	/// <param name="hasKeyboardFocus"></param>
	public void Draw(Rect position, GUIContent content, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
	{
		Internal_Draw(m_Ptr, position, content, isHover, isActive, on, hasKeyboardFocus);
	}

	/// <summary>
	///   <para>Draw the GUIStyle with text and an image inside. If the image is too large to fit within the content area of the style it is scaled down.</para>
	/// </summary>
	/// <param name="position"></param>
	/// <param name="content"></param>
	/// <param name="controlID"></param>
	/// <param name="on"></param>
	/// <param name="isHover"></param>
	/// <param name="isActive"></param>
	/// <param name="hasKeyboardFocus"></param>
	public void Draw(Rect position, GUIContent content, int controlID)
	{
		Draw(position, content, controlID, on: false);
	}

	/// <summary>
	///   <para>Draw the GUIStyle with text and an image inside. If the image is too large to fit within the content area of the style it is scaled down.</para>
	/// </summary>
	/// <param name="position"></param>
	/// <param name="content"></param>
	/// <param name="controlID"></param>
	/// <param name="on"></param>
	/// <param name="isHover"></param>
	/// <param name="isActive"></param>
	/// <param name="hasKeyboardFocus"></param>
	public void Draw(Rect position, GUIContent content, int controlID, bool on)
	{
		if (content != null)
		{
			Internal_Draw2(m_Ptr, position, content, controlID, on);
		}
		else
		{
			Debug.LogError("Style.Draw may not be called with GUIContent that is null.");
		}
	}

	/// <summary>
	///   <para>Draw this GUIStyle with selected content.</para>
	/// </summary>
	/// <param name="position"></param>
	/// <param name="content"></param>
	/// <param name="controlID"></param>
	/// <param name="Character"></param>
	public void DrawCursor(Rect position, GUIContent content, int controlID, int Character)
	{
		Event current = Event.current;
		if (current.type == EventType.Repaint)
		{
			Color cursorColor = new Color(0f, 0f, 0f, 0f);
			float cursorFlashSpeed = GUI.skin.settings.cursorFlashSpeed;
			float num = (Time.realtimeSinceStartup - Internal_GetCursorFlashOffset()) % cursorFlashSpeed / cursorFlashSpeed;
			if (cursorFlashSpeed == 0f || num < 0.5f)
			{
				cursorColor = GUI.skin.settings.cursorColor;
			}
			Internal_DrawCursor(m_Ptr, position, content, Character, cursorColor);
		}
	}

	internal void DrawWithTextSelection(Rect position, GUIContent content, bool isActive, bool hasKeyboardFocus, int firstSelectedCharacter, int lastSelectedCharacter, bool drawSelectionAsComposition)
	{
		Event current = Event.current;
		Color cursorColor = new Color(0f, 0f, 0f, 0f);
		float cursorFlashSpeed = GUI.skin.settings.cursorFlashSpeed;
		float num = (Time.realtimeSinceStartup - Internal_GetCursorFlashOffset()) % cursorFlashSpeed / cursorFlashSpeed;
		if (cursorFlashSpeed == 0f || num < 0.5f)
		{
			cursorColor = GUI.skin.settings.cursorColor;
		}
		Internal_DrawWithTextSelectionArguments arguments = default(Internal_DrawWithTextSelectionArguments);
		arguments.target = m_Ptr;
		arguments.position = position;
		arguments.firstPos = firstSelectedCharacter;
		arguments.lastPos = lastSelectedCharacter;
		arguments.cursorColor = cursorColor;
		arguments.selectionColor = GUI.skin.settings.selectionColor;
		arguments.isHover = (position.Contains(current.mousePosition) ? 1 : 0);
		arguments.isActive = (isActive ? 1 : 0);
		arguments.on = 0;
		arguments.hasKeyboardFocus = (hasKeyboardFocus ? 1 : 0);
		arguments.drawSelectionAsComposition = (drawSelectionAsComposition ? 1 : 0);
		Internal_DrawWithTextSelection(content, ref arguments);
	}

	internal void DrawWithTextSelection(Rect position, GUIContent content, int controlID, int firstSelectedCharacter, int lastSelectedCharacter, bool drawSelectionAsComposition)
	{
		DrawWithTextSelection(position, content, controlID == GUIUtility.hotControl, controlID == GUIUtility.keyboardControl && showKeyboardFocus, firstSelectedCharacter, lastSelectedCharacter, drawSelectionAsComposition);
	}

	/// <summary>
	///   <para>Draw this GUIStyle with selected content.</para>
	/// </summary>
	/// <param name="position"></param>
	/// <param name="content"></param>
	/// <param name="controlID"></param>
	/// <param name="firstSelectedCharacter"></param>
	/// <param name="lastSelectedCharacter"></param>
	public void DrawWithTextSelection(Rect position, GUIContent content, int controlID, int firstSelectedCharacter, int lastSelectedCharacter)
	{
		DrawWithTextSelection(position, content, controlID, firstSelectedCharacter, lastSelectedCharacter, drawSelectionAsComposition: false);
	}

	public static implicit operator GUIStyle(string str)
	{
		if (GUISkin.current == null)
		{
			Debug.LogError("Unable to use a named GUIStyle without a current skin. Most likely you need to move your GUIStyle initialization code to OnGUI");
			return GUISkin.error;
		}
		return GUISkin.current.GetStyle(str);
	}

	/// <summary>
	///   <para>Get the pixel position of a given string index.</para>
	/// </summary>
	/// <param name="position"></param>
	/// <param name="content"></param>
	/// <param name="cursorStringIndex"></param>
	public Vector2 GetCursorPixelPosition(Rect position, GUIContent content, int cursorStringIndex)
	{
		Internal_GetCursorPixelPosition(m_Ptr, position, content, cursorStringIndex, out var ret);
		return ret;
	}

	/// <summary>
	///   <para>Get the cursor position (indexing into contents.text) when the user clicked at cursorPixelPosition.</para>
	/// </summary>
	/// <param name="position"></param>
	/// <param name="content"></param>
	/// <param name="cursorPixelPosition"></param>
	public int GetCursorStringIndex(Rect position, GUIContent content, Vector2 cursorPixelPosition)
	{
		return Internal_GetCursorStringIndex(m_Ptr, position, content, cursorPixelPosition);
	}

	internal int GetNumCharactersThatFitWithinWidth(string text, float width)
	{
		return Internal_GetNumCharactersThatFitWithinWidth(m_Ptr, text, width);
	}

	/// <summary>
	///   <para>Calculate the size of some content if it is rendered with this style.</para>
	/// </summary>
	/// <param name="content"></param>
	public Vector2 CalcSize(GUIContent content)
	{
		Internal_CalcSize(m_Ptr, content, out var ret);
		return ret;
	}

	internal Vector2 CalcSizeWithConstraints(GUIContent content, Vector2 constraints)
	{
		Internal_CalcSizeWithConstraints(m_Ptr, content, constraints, out var ret);
		return ret;
	}

	/// <summary>
	///   <para>Calculate the size of an element formatted with this style, and a given space to content.</para>
	/// </summary>
	/// <param name="contentSize"></param>
	public Vector2 CalcScreenSize(Vector2 contentSize)
	{
		return new Vector2((fixedWidth == 0f) ? Mathf.Ceil(contentSize.x + (float)padding.left + (float)padding.right) : fixedWidth, (fixedHeight == 0f) ? Mathf.Ceil(contentSize.y + (float)padding.top + (float)padding.bottom) : fixedHeight);
	}

	/// <summary>
	///   <para>How tall this element will be when rendered with content and a specific width.</para>
	/// </summary>
	/// <param name="content"></param>
	/// <param name="width"></param>
	public float CalcHeight(GUIContent content, float width)
	{
		return Internal_CalcHeight(m_Ptr, content, width);
	}

	public void CalcMinMaxWidth(GUIContent content, out float minWidth, out float maxWidth)
	{
		Internal_CalcMinMaxWidth(m_Ptr, content, out minWidth, out maxWidth);
	}

	public override string ToString()
	{
		return UnityString.Format("GUIStyle '{0}'", name);
	}
}

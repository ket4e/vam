using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEngine;

/// <summary>
///   <para>The GUI class is the interface for Unity's GUI with manual positioning.</para>
/// </summary>
public class GUI
{
	/// <summary>
	///   <para>Determines how toolbar button size is calculated.</para>
	/// </summary>
	public enum ToolbarButtonSize
	{
		/// <summary>
		///   <para>Calculates the button size by dividing the available width by the number of buttons. The minimum size is the maximum content width.</para>
		/// </summary>
		Fixed,
		/// <summary>
		///   <para>The width of each toolbar button is calculated based on the width of its content.</para>
		/// </summary>
		FitToContents
	}

	/// <summary>
	///   <para>Callback to draw GUI within a window (used with GUI.Window).</para>
	/// </summary>
	/// <param name="id"></param>
	public delegate void WindowFunction(int id);

	public abstract class Scope : IDisposable
	{
		private bool m_Disposed;

		protected abstract void CloseScope();

		~Scope()
		{
			if (!m_Disposed)
			{
				Debug.LogError("Scope was not disposed! You should use the 'using' keyword or manually call Dispose.");
			}
		}

		public void Dispose()
		{
			if (!m_Disposed)
			{
				m_Disposed = true;
				if (!GUIUtility.guiIsExiting)
				{
					CloseScope();
				}
			}
		}
	}

	/// <summary>
	///   <para>Disposable helper class for managing BeginGroup / EndGroup.</para>
	/// </summary>
	public class GroupScope : Scope
	{
		/// <summary>
		///   <para>Create a new GroupScope and begin the corresponding group.</para>
		/// </summary>
		/// <param name="position">Rectangle on the screen to use for the group.</param>
		/// <param name="text">Text to display on the group.</param>
		/// <param name="image">Texture to display on the group.</param>
		/// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
		/// <param name="style">The style to use for the background.</param>
		public GroupScope(Rect position)
		{
			BeginGroup(position);
		}

		/// <summary>
		///   <para>Create a new GroupScope and begin the corresponding group.</para>
		/// </summary>
		/// <param name="position">Rectangle on the screen to use for the group.</param>
		/// <param name="text">Text to display on the group.</param>
		/// <param name="image">Texture to display on the group.</param>
		/// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
		/// <param name="style">The style to use for the background.</param>
		public GroupScope(Rect position, string text)
		{
			BeginGroup(position, text);
		}

		/// <summary>
		///   <para>Create a new GroupScope and begin the corresponding group.</para>
		/// </summary>
		/// <param name="position">Rectangle on the screen to use for the group.</param>
		/// <param name="text">Text to display on the group.</param>
		/// <param name="image">Texture to display on the group.</param>
		/// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
		/// <param name="style">The style to use for the background.</param>
		public GroupScope(Rect position, Texture image)
		{
			BeginGroup(position, image);
		}

		/// <summary>
		///   <para>Create a new GroupScope and begin the corresponding group.</para>
		/// </summary>
		/// <param name="position">Rectangle on the screen to use for the group.</param>
		/// <param name="text">Text to display on the group.</param>
		/// <param name="image">Texture to display on the group.</param>
		/// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
		/// <param name="style">The style to use for the background.</param>
		public GroupScope(Rect position, GUIContent content)
		{
			BeginGroup(position, content);
		}

		/// <summary>
		///   <para>Create a new GroupScope and begin the corresponding group.</para>
		/// </summary>
		/// <param name="position">Rectangle on the screen to use for the group.</param>
		/// <param name="text">Text to display on the group.</param>
		/// <param name="image">Texture to display on the group.</param>
		/// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
		/// <param name="style">The style to use for the background.</param>
		public GroupScope(Rect position, GUIStyle style)
		{
			BeginGroup(position, style);
		}

		/// <summary>
		///   <para>Create a new GroupScope and begin the corresponding group.</para>
		/// </summary>
		/// <param name="position">Rectangle on the screen to use for the group.</param>
		/// <param name="text">Text to display on the group.</param>
		/// <param name="image">Texture to display on the group.</param>
		/// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
		/// <param name="style">The style to use for the background.</param>
		public GroupScope(Rect position, string text, GUIStyle style)
		{
			BeginGroup(position, text, style);
		}

		/// <summary>
		///   <para>Create a new GroupScope and begin the corresponding group.</para>
		/// </summary>
		/// <param name="position">Rectangle on the screen to use for the group.</param>
		/// <param name="text">Text to display on the group.</param>
		/// <param name="image">Texture to display on the group.</param>
		/// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
		/// <param name="style">The style to use for the background.</param>
		public GroupScope(Rect position, Texture image, GUIStyle style)
		{
			BeginGroup(position, image, style);
		}

		protected override void CloseScope()
		{
			EndGroup();
		}
	}

	/// <summary>
	///   <para>Disposable helper class for managing BeginScrollView / EndScrollView.</para>
	/// </summary>
	public class ScrollViewScope : Scope
	{
		/// <summary>
		///   <para>The modified scrollPosition. Feed this back into the variable you pass in, as shown in the example.</para>
		/// </summary>
		public Vector2 scrollPosition { get; private set; }

		/// <summary>
		///   <para>Whether this ScrollView should handle scroll wheel events. (default: true).</para>
		/// </summary>
		public bool handleScrollWheel { get; set; }

		/// <summary>
		///   <para>Create a new ScrollViewScope and begin the corresponding ScrollView.</para>
		/// </summary>
		/// <param name="position">Rectangle on the screen to use for the ScrollView.</param>
		/// <param name="scrollPosition">The pixel distance that the view is scrolled in the X and Y directions.</param>
		/// <param name="viewRect">The rectangle used inside the scrollview.</param>
		/// <param name="alwaysShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when clientRect is wider than position.</param>
		/// <param name="alwaysShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when clientRect is taller than position.</param>
		/// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
		/// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
		public ScrollViewScope(Rect position, Vector2 scrollPosition, Rect viewRect)
		{
			handleScrollWheel = true;
			this.scrollPosition = BeginScrollView(position, scrollPosition, viewRect);
		}

		/// <summary>
		///   <para>Create a new ScrollViewScope and begin the corresponding ScrollView.</para>
		/// </summary>
		/// <param name="position">Rectangle on the screen to use for the ScrollView.</param>
		/// <param name="scrollPosition">The pixel distance that the view is scrolled in the X and Y directions.</param>
		/// <param name="viewRect">The rectangle used inside the scrollview.</param>
		/// <param name="alwaysShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when clientRect is wider than position.</param>
		/// <param name="alwaysShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when clientRect is taller than position.</param>
		/// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
		/// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
		public ScrollViewScope(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical)
		{
			handleScrollWheel = true;
			this.scrollPosition = BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical);
		}

		/// <summary>
		///   <para>Create a new ScrollViewScope and begin the corresponding ScrollView.</para>
		/// </summary>
		/// <param name="position">Rectangle on the screen to use for the ScrollView.</param>
		/// <param name="scrollPosition">The pixel distance that the view is scrolled in the X and Y directions.</param>
		/// <param name="viewRect">The rectangle used inside the scrollview.</param>
		/// <param name="alwaysShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when clientRect is wider than position.</param>
		/// <param name="alwaysShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when clientRect is taller than position.</param>
		/// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
		/// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
		public ScrollViewScope(Rect position, Vector2 scrollPosition, Rect viewRect, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar)
		{
			handleScrollWheel = true;
			this.scrollPosition = BeginScrollView(position, scrollPosition, viewRect, horizontalScrollbar, verticalScrollbar);
		}

		/// <summary>
		///   <para>Create a new ScrollViewScope and begin the corresponding ScrollView.</para>
		/// </summary>
		/// <param name="position">Rectangle on the screen to use for the ScrollView.</param>
		/// <param name="scrollPosition">The pixel distance that the view is scrolled in the X and Y directions.</param>
		/// <param name="viewRect">The rectangle used inside the scrollview.</param>
		/// <param name="alwaysShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when clientRect is wider than position.</param>
		/// <param name="alwaysShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when clientRect is taller than position.</param>
		/// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
		/// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
		public ScrollViewScope(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar)
		{
			handleScrollWheel = true;
			this.scrollPosition = BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar);
		}

		internal ScrollViewScope(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background)
		{
			handleScrollWheel = true;
			this.scrollPosition = BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, background);
		}

		protected override void CloseScope()
		{
			EndScrollView(handleScrollWheel);
		}
	}

	public class ClipScope : Scope
	{
		public ClipScope(Rect position)
		{
			BeginClip(position);
		}

		protected override void CloseScope()
		{
			EndClip();
		}
	}

	private static float s_ScrollStepSize;

	private static int s_ScrollControlId;

	private static int s_HotTextField;

	private static readonly int s_BoxHash;

	private static readonly int s_RepeatButtonHash;

	private static readonly int s_ToggleHash;

	private static readonly int s_ButtonGridHash;

	private static readonly int s_SliderHash;

	private static readonly int s_BeginGroupHash;

	private static readonly int s_ScrollviewHash;

	private static GUISkin s_Skin;

	internal static Rect s_ToolTipRect;

	private static GenericStack s_ScrollViewStates;

	/// <summary>
	///   <para>Global tinting color for the GUI.</para>
	/// </summary>
	public static Color color
	{
		get
		{
			INTERNAL_get_color(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_color(ref value);
		}
	}

	/// <summary>
	///   <para>Global tinting color for all background elements rendered by the GUI.</para>
	/// </summary>
	public static Color backgroundColor
	{
		get
		{
			INTERNAL_get_backgroundColor(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_backgroundColor(ref value);
		}
	}

	/// <summary>
	///   <para>Tinting color for all text rendered by the GUI.</para>
	/// </summary>
	public static Color contentColor
	{
		get
		{
			INTERNAL_get_contentColor(out var value);
			return value;
		}
		set
		{
			INTERNAL_set_contentColor(ref value);
		}
	}

	/// <summary>
	///   <para>Returns true if any controls changed the value of the input data.</para>
	/// </summary>
	public static extern bool changed
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Is the GUI enabled?</para>
	/// </summary>
	public static extern bool enabled
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>The sorting depth of the currently executing GUI behaviour.</para>
	/// </summary>
	public static extern int depth
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	internal static extern Material blendMaterial
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	internal static extern Material blitMaterial
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	internal static extern Material roundedRectMaterial
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	internal static extern bool usePageScrollbars
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	internal static int scrollTroughSide { get; set; }

	internal static DateTime nextScrollStepTime { get; set; }

	/// <summary>
	///   <para>The global skin to use.</para>
	/// </summary>
	public static GUISkin skin
	{
		get
		{
			GUIUtility.CheckOnGUI();
			return s_Skin;
		}
		set
		{
			GUIUtility.CheckOnGUI();
			DoSetSkin(value);
		}
	}

	/// <summary>
	///   <para>The GUI transform matrix.</para>
	/// </summary>
	public static Matrix4x4 matrix
	{
		get
		{
			return GUIClip.GetMatrix();
		}
		set
		{
			GUIClip.SetMatrix(value);
		}
	}

	/// <summary>
	///   <para>The tooltip of the control the mouse is currently over, or which has keyboard focus. (Read Only).</para>
	/// </summary>
	public static string tooltip
	{
		get
		{
			string text = Internal_GetTooltip();
			if (text != null)
			{
				return text;
			}
			return "";
		}
		set
		{
			Internal_SetTooltip(value);
		}
	}

	protected static string mouseTooltip => Internal_GetMouseTooltip();

	protected static Rect tooltipRect
	{
		get
		{
			return s_ToolTipRect;
		}
		set
		{
			s_ToolTipRect = value;
		}
	}

	static GUI()
	{
		s_ScrollStepSize = 10f;
		s_HotTextField = -1;
		s_BoxHash = "Box".GetHashCode();
		s_RepeatButtonHash = "repeatButton".GetHashCode();
		s_ToggleHash = "Toggle".GetHashCode();
		s_ButtonGridHash = "ButtonGrid".GetHashCode();
		s_SliderHash = "Slider".GetHashCode();
		s_BeginGroupHash = "BeginGroup".GetHashCode();
		s_ScrollviewHash = "scrollView".GetHashCode();
		s_ScrollViewStates = new GenericStack();
		nextScrollStepTime = DateTime.Now;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_get_color(out Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_set_color(ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_get_backgroundColor(out Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_set_backgroundColor(ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_get_contentColor(out Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_set_contentColor(ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern string Internal_GetTooltip();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_SetTooltip(string value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern string Internal_GetMouseTooltip();

	private static void DoLabel(Rect position, GUIContent content, IntPtr style)
	{
		INTERNAL_CALL_DoLabel(ref position, content, style);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_DoLabel(ref Rect position, GUIContent content, IntPtr style);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void InitializeGUIClipTexture();

	private static bool DoButton(Rect position, GUIContent content, IntPtr style)
	{
		return INTERNAL_CALL_DoButton(ref position, content, style);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool INTERNAL_CALL_DoButton(ref Rect position, GUIContent content, IntPtr style);

	/// <summary>
	///   <para>Set the name of the next control.</para>
	/// </summary>
	/// <param name="name"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void SetNextControlName(string name);

	/// <summary>
	///   <para>Get the name of named control that has focus.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern string GetNameOfFocusedControl();

	/// <summary>
	///   <para>Move keyboard focus to a named control.</para>
	/// </summary>
	/// <param name="name">Name set using SetNextControlName.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void FocusControl(string name);

	internal static bool DoToggle(Rect position, int id, bool value, GUIContent content, IntPtr style)
	{
		return INTERNAL_CALL_DoToggle(ref position, id, value, content, style);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool INTERNAL_CALL_DoToggle(ref Rect position, int id, bool value, GUIContent content, IntPtr style);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal static extern void InternalRepaintEditorWindow();

	private static Rect Internal_DoModalWindow(int id, int instanceID, Rect clientRect, WindowFunction func, GUIContent content, GUIStyle style, GUISkin skin)
	{
		INTERNAL_CALL_Internal_DoModalWindow(id, instanceID, ref clientRect, func, content, style, skin, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Internal_DoModalWindow(int id, int instanceID, ref Rect clientRect, WindowFunction func, GUIContent content, GUIStyle style, GUISkin skin, out Rect value);

	private static Rect Internal_DoWindow(int id, int instanceID, Rect clientRect, WindowFunction func, GUIContent title, GUIStyle style, GUISkin skin, bool forceRectOnLayout)
	{
		INTERNAL_CALL_Internal_DoWindow(id, instanceID, ref clientRect, func, title, style, skin, forceRectOnLayout, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Internal_DoWindow(int id, int instanceID, ref Rect clientRect, WindowFunction func, GUIContent title, GUIStyle style, GUISkin skin, bool forceRectOnLayout, out Rect value);

	/// <summary>
	///   <para>Make a window draggable.</para>
	/// </summary>
	/// <param name="position">The part of the window that can be dragged. This is clipped to the actual window.</param>
	public static void DragWindow(Rect position)
	{
		INTERNAL_CALL_DragWindow(ref position);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_DragWindow(ref Rect position);

	/// <summary>
	///   <para>Bring a specific window to front of the floating windows.</para>
	/// </summary>
	/// <param name="windowID">The identifier used when you created the window in the Window call.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void BringWindowToFront(int windowID);

	/// <summary>
	///   <para>Bring a specific window to back of the floating windows.</para>
	/// </summary>
	/// <param name="windowID">The identifier used when you created the window in the Window call.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void BringWindowToBack(int windowID);

	/// <summary>
	///   <para>Make a window become the active window.</para>
	/// </summary>
	/// <param name="windowID">The identifier used when you created the window in the Window call.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void FocusWindow(int windowID);

	/// <summary>
	///   <para>Remove focus from all windows.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public static extern void UnfocusWindow();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_BeginWindows();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_EndWindows();

	internal static void DoSetSkin(GUISkin newSkin)
	{
		if (!newSkin)
		{
			newSkin = GUIUtility.GetDefaultSkin();
		}
		s_Skin = newSkin;
		newSkin.MakeCurrent();
	}

	internal static void CleanupRoots()
	{
		s_Skin = null;
		GUIUtility.CleanupRoots();
		GUILayoutUtility.CleanupRoots();
		GUISkin.CleanupRoots();
		GUIStyle.CleanupRoots();
	}

	/// <summary>
	///   <para>Make a text or texture label on screen.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the label.</param>
	/// <param name="text">Text to display on the label.</param>
	/// <param name="image">Texture to display on the label.</param>
	/// <param name="content">Text, image and tooltip for this label.</param>
	/// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.</param>
	public static void Label(Rect position, string text)
	{
		Label(position, GUIContent.Temp(text), s_Skin.label);
	}

	/// <summary>
	///   <para>Make a text or texture label on screen.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the label.</param>
	/// <param name="text">Text to display on the label.</param>
	/// <param name="image">Texture to display on the label.</param>
	/// <param name="content">Text, image and tooltip for this label.</param>
	/// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.</param>
	public static void Label(Rect position, Texture image)
	{
		Label(position, GUIContent.Temp(image), s_Skin.label);
	}

	/// <summary>
	///   <para>Make a text or texture label on screen.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the label.</param>
	/// <param name="text">Text to display on the label.</param>
	/// <param name="image">Texture to display on the label.</param>
	/// <param name="content">Text, image and tooltip for this label.</param>
	/// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.</param>
	public static void Label(Rect position, GUIContent content)
	{
		Label(position, content, s_Skin.label);
	}

	/// <summary>
	///   <para>Make a text or texture label on screen.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the label.</param>
	/// <param name="text">Text to display on the label.</param>
	/// <param name="image">Texture to display on the label.</param>
	/// <param name="content">Text, image and tooltip for this label.</param>
	/// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.</param>
	public static void Label(Rect position, string text, GUIStyle style)
	{
		Label(position, GUIContent.Temp(text), style);
	}

	/// <summary>
	///   <para>Make a text or texture label on screen.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the label.</param>
	/// <param name="text">Text to display on the label.</param>
	/// <param name="image">Texture to display on the label.</param>
	/// <param name="content">Text, image and tooltip for this label.</param>
	/// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.</param>
	public static void Label(Rect position, Texture image, GUIStyle style)
	{
		Label(position, GUIContent.Temp(image), style);
	}

	/// <summary>
	///   <para>Make a text or texture label on screen.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the label.</param>
	/// <param name="text">Text to display on the label.</param>
	/// <param name="image">Texture to display on the label.</param>
	/// <param name="content">Text, image and tooltip for this label.</param>
	/// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.</param>
	public static void Label(Rect position, GUIContent content, GUIStyle style)
	{
		GUIUtility.CheckOnGUI();
		DoLabel(position, content, style.m_Ptr);
	}

	/// <summary>
	///   <para>Draw a texture within a rectangle.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to draw the texture within.</param>
	/// <param name="image">Texture to display.</param>
	/// <param name="scaleMode">How to scale the image when the aspect ratio of it doesn't fit the aspect ratio to be drawn within.</param>
	/// <param name="alphaBlend">Whether to apply alpha blending when drawing the image (enabled by default).</param>
	/// <param name="imageAspect">Aspect ratio to use for the source image. If 0 (the default), the aspect ratio from the image is used.  Pass in w/h for the desired aspect ratio.  This allows the aspect ratio of the source image to be adjusted without changing the pixel width and height.</param>
	public static void DrawTexture(Rect position, Texture image)
	{
		DrawTexture(position, image, ScaleMode.StretchToFill);
	}

	/// <summary>
	///   <para>Draw a texture within a rectangle.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to draw the texture within.</param>
	/// <param name="image">Texture to display.</param>
	/// <param name="scaleMode">How to scale the image when the aspect ratio of it doesn't fit the aspect ratio to be drawn within.</param>
	/// <param name="alphaBlend">Whether to apply alpha blending when drawing the image (enabled by default).</param>
	/// <param name="imageAspect">Aspect ratio to use for the source image. If 0 (the default), the aspect ratio from the image is used.  Pass in w/h for the desired aspect ratio.  This allows the aspect ratio of the source image to be adjusted without changing the pixel width and height.</param>
	public static void DrawTexture(Rect position, Texture image, ScaleMode scaleMode)
	{
		DrawTexture(position, image, scaleMode, alphaBlend: true);
	}

	/// <summary>
	///   <para>Draw a texture within a rectangle.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to draw the texture within.</param>
	/// <param name="image">Texture to display.</param>
	/// <param name="scaleMode">How to scale the image when the aspect ratio of it doesn't fit the aspect ratio to be drawn within.</param>
	/// <param name="alphaBlend">Whether to apply alpha blending when drawing the image (enabled by default).</param>
	/// <param name="imageAspect">Aspect ratio to use for the source image. If 0 (the default), the aspect ratio from the image is used.  Pass in w/h for the desired aspect ratio.  This allows the aspect ratio of the source image to be adjusted without changing the pixel width and height.</param>
	public static void DrawTexture(Rect position, Texture image, ScaleMode scaleMode, bool alphaBlend)
	{
		DrawTexture(position, image, scaleMode, alphaBlend, 0f);
	}

	/// <summary>
	///   <para>Draw a texture within a rectangle.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to draw the texture within.</param>
	/// <param name="image">Texture to display.</param>
	/// <param name="scaleMode">How to scale the image when the aspect ratio of it doesn't fit the aspect ratio to be drawn within.</param>
	/// <param name="alphaBlend">Whether to apply alpha blending when drawing the image (enabled by default).</param>
	/// <param name="imageAspect">Aspect ratio to use for the source image. If 0 (the default), the aspect ratio from the image is used.  Pass in w/h for the desired aspect ratio.  This allows the aspect ratio of the source image to be adjusted without changing the pixel width and height.</param>
	public static void DrawTexture(Rect position, Texture image, ScaleMode scaleMode, bool alphaBlend, float imageAspect)
	{
		DrawTexture(position, image, scaleMode, alphaBlend, imageAspect, color, 0f, 0f);
	}

	/// <summary>
	///   <para>Draws a border with rounded corners within a rectangle. The texture is used to pattern the border.  Note that this method only works on shader model 2.5 and above.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to draw the texture within.</param>
	/// <param name="image">Texture to display.</param>
	/// <param name="scaleMode">How to scale the image when the aspect ratio of it doesn't fit the aspect ratio to be drawn within.</param>
	/// <param name="alphaBlend">Whether to apply alpha blending when drawing the image (enabled by default).</param>
	/// <param name="imageAspect">Aspect ratio to use for the source image. If 0 (the default), the aspect ratio from the image is used.  Pass in w/h for the desired aspect ratio.  This allows the aspect ratio of the source image to be adjusted without changing the pixel width and height.</param>
	/// <param name="color">A tint color to apply on the texture.</param>
	/// <param name="borderWidth">The width of the border. If 0, the full texture is drawn.</param>
	/// <param name="borderWidths">The width of the borders (left, top, right and bottom). If Vector4.zero, the full texture is drawn.</param>
	/// <param name="borderRadius">The radius for rounded corners. If 0, corners will not be rounded.</param>
	/// <param name="borderRadiuses">The radiuses for rounded corners (top-left, top-right, bottom-right and bottom-left). If Vector4.zero, corners will not be rounded.</param>
	public static void DrawTexture(Rect position, Texture image, ScaleMode scaleMode, bool alphaBlend, float imageAspect, Color color, float borderWidth, float borderRadius)
	{
		Vector4 borderWidths = Vector4.one * borderWidth;
		DrawTexture(position, image, scaleMode, alphaBlend, imageAspect, color, borderWidths, borderRadius);
	}

	/// <summary>
	///   <para>Draws a border with rounded corners within a rectangle. The texture is used to pattern the border.  Note that this method only works on shader model 2.5 and above.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to draw the texture within.</param>
	/// <param name="image">Texture to display.</param>
	/// <param name="scaleMode">How to scale the image when the aspect ratio of it doesn't fit the aspect ratio to be drawn within.</param>
	/// <param name="alphaBlend">Whether to apply alpha blending when drawing the image (enabled by default).</param>
	/// <param name="imageAspect">Aspect ratio to use for the source image. If 0 (the default), the aspect ratio from the image is used.  Pass in w/h for the desired aspect ratio.  This allows the aspect ratio of the source image to be adjusted without changing the pixel width and height.</param>
	/// <param name="color">A tint color to apply on the texture.</param>
	/// <param name="borderWidth">The width of the border. If 0, the full texture is drawn.</param>
	/// <param name="borderWidths">The width of the borders (left, top, right and bottom). If Vector4.zero, the full texture is drawn.</param>
	/// <param name="borderRadius">The radius for rounded corners. If 0, corners will not be rounded.</param>
	/// <param name="borderRadiuses">The radiuses for rounded corners (top-left, top-right, bottom-right and bottom-left). If Vector4.zero, corners will not be rounded.</param>
	public static void DrawTexture(Rect position, Texture image, ScaleMode scaleMode, bool alphaBlend, float imageAspect, Color color, Vector4 borderWidths, float borderRadius)
	{
		Vector4 borderRadiuses = Vector4.one * borderRadius;
		DrawTexture(position, image, scaleMode, alphaBlend, imageAspect, color, borderWidths, borderRadiuses);
	}

	public static void DrawTexture(Rect position, Texture image, ScaleMode scaleMode, bool alphaBlend, float imageAspect, Color color, Vector4 borderWidths, Vector4 borderRadiuses)
	{
		GUIUtility.CheckOnGUI();
		if (Event.current.type != EventType.Repaint)
		{
			return;
		}
		if (image == null)
		{
			Debug.LogWarning("null texture passed to GUI.DrawTexture");
			return;
		}
		if (imageAspect == 0f)
		{
			imageAspect = (float)image.width / (float)image.height;
		}
		Material material = null;
		material = ((!(borderWidths != Vector4.zero) && !(borderRadiuses != Vector4.zero)) ? ((!alphaBlend) ? blitMaterial : blendMaterial) : roundedRectMaterial);
		Internal_DrawTextureArguments args = default(Internal_DrawTextureArguments);
		args.leftBorder = 0;
		args.rightBorder = 0;
		args.topBorder = 0;
		args.bottomBorder = 0;
		args.color = color;
		args.borderWidths = borderWidths;
		args.cornerRadiuses = borderRadiuses;
		args.texture = image;
		args.mat = material;
		CalculateScaledTextureRects(position, scaleMode, imageAspect, ref args.screenRect, ref args.sourceRect);
		Graphics.Internal_DrawTexture(ref args);
	}

	internal static bool CalculateScaledTextureRects(Rect position, ScaleMode scaleMode, float imageAspect, ref Rect outScreenRect, ref Rect outSourceRect)
	{
		float num = position.width / position.height;
		bool result = false;
		switch (scaleMode)
		{
		case ScaleMode.StretchToFill:
			outScreenRect = position;
			outSourceRect = new Rect(0f, 0f, 1f, 1f);
			result = true;
			break;
		case ScaleMode.ScaleAndCrop:
			if (num > imageAspect)
			{
				float num4 = imageAspect / num;
				outScreenRect = position;
				outSourceRect = new Rect(0f, (1f - num4) * 0.5f, 1f, num4);
				result = true;
			}
			else
			{
				float num5 = num / imageAspect;
				outScreenRect = position;
				outSourceRect = new Rect(0.5f - num5 * 0.5f, 0f, num5, 1f);
				result = true;
			}
			break;
		case ScaleMode.ScaleToFit:
			if (num > imageAspect)
			{
				float num2 = imageAspect / num;
				outScreenRect = new Rect(position.xMin + position.width * (1f - num2) * 0.5f, position.yMin, num2 * position.width, position.height);
				outSourceRect = new Rect(0f, 0f, 1f, 1f);
				result = true;
			}
			else
			{
				float num3 = num / imageAspect;
				outScreenRect = new Rect(position.xMin, position.yMin + position.height * (1f - num3) * 0.5f, position.width, num3 * position.height);
				outSourceRect = new Rect(0f, 0f, 1f, 1f);
				result = true;
			}
			break;
		}
		return result;
	}

	/// <summary>
	///   <para>Draw a texture within a rectangle with the given texture coordinates.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to draw the texture within.</param>
	/// <param name="image">Texture to display.</param>
	/// <param name="texCoords">How to scale the image when the aspect ratio of it doesn't fit the aspect ratio to be drawn within.</param>
	/// <param name="alphaBlend">Whether to alpha blend the image on to the display (the default). If false, the picture is drawn on to the display.</param>
	public static void DrawTextureWithTexCoords(Rect position, Texture image, Rect texCoords)
	{
		DrawTextureWithTexCoords(position, image, texCoords, alphaBlend: true);
	}

	/// <summary>
	///   <para>Draw a texture within a rectangle with the given texture coordinates.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to draw the texture within.</param>
	/// <param name="image">Texture to display.</param>
	/// <param name="texCoords">How to scale the image when the aspect ratio of it doesn't fit the aspect ratio to be drawn within.</param>
	/// <param name="alphaBlend">Whether to alpha blend the image on to the display (the default). If false, the picture is drawn on to the display.</param>
	public static void DrawTextureWithTexCoords(Rect position, Texture image, Rect texCoords, bool alphaBlend)
	{
		GUIUtility.CheckOnGUI();
		if (Event.current.type == EventType.Repaint)
		{
			Material mat = ((!alphaBlend) ? blitMaterial : blendMaterial);
			Internal_DrawTextureArguments args = default(Internal_DrawTextureArguments);
			args.texture = image;
			args.mat = mat;
			args.leftBorder = 0;
			args.rightBorder = 0;
			args.topBorder = 0;
			args.bottomBorder = 0;
			args.color = color;
			args.screenRect = position;
			args.sourceRect = texCoords;
			Graphics.Internal_DrawTexture(ref args);
		}
	}

	/// <summary>
	///   <para>Create a Box on the GUI Layer.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the box.</param>
	/// <param name="text">Text to display on the box.</param>
	/// <param name="image">Texture to display on the box.</param>
	/// <param name="content">Text, image and tooltip for this box.</param>
	/// <param name="style">The style to use. If left out, the box style from the current GUISkin is used.</param>
	public static void Box(Rect position, string text)
	{
		Box(position, GUIContent.Temp(text), s_Skin.box);
	}

	/// <summary>
	///   <para>Create a Box on the GUI Layer.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the box.</param>
	/// <param name="text">Text to display on the box.</param>
	/// <param name="image">Texture to display on the box.</param>
	/// <param name="content">Text, image and tooltip for this box.</param>
	/// <param name="style">The style to use. If left out, the box style from the current GUISkin is used.</param>
	public static void Box(Rect position, Texture image)
	{
		Box(position, GUIContent.Temp(image), s_Skin.box);
	}

	/// <summary>
	///   <para>Create a Box on the GUI Layer.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the box.</param>
	/// <param name="text">Text to display on the box.</param>
	/// <param name="image">Texture to display on the box.</param>
	/// <param name="content">Text, image and tooltip for this box.</param>
	/// <param name="style">The style to use. If left out, the box style from the current GUISkin is used.</param>
	public static void Box(Rect position, GUIContent content)
	{
		Box(position, content, s_Skin.box);
	}

	/// <summary>
	///   <para>Create a Box on the GUI Layer.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the box.</param>
	/// <param name="text">Text to display on the box.</param>
	/// <param name="image">Texture to display on the box.</param>
	/// <param name="content">Text, image and tooltip for this box.</param>
	/// <param name="style">The style to use. If left out, the box style from the current GUISkin is used.</param>
	public static void Box(Rect position, string text, GUIStyle style)
	{
		Box(position, GUIContent.Temp(text), style);
	}

	/// <summary>
	///   <para>Create a Box on the GUI Layer.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the box.</param>
	/// <param name="text">Text to display on the box.</param>
	/// <param name="image">Texture to display on the box.</param>
	/// <param name="content">Text, image and tooltip for this box.</param>
	/// <param name="style">The style to use. If left out, the box style from the current GUISkin is used.</param>
	public static void Box(Rect position, Texture image, GUIStyle style)
	{
		Box(position, GUIContent.Temp(image), style);
	}

	/// <summary>
	///   <para>Create a Box on the GUI Layer.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the box.</param>
	/// <param name="text">Text to display on the box.</param>
	/// <param name="image">Texture to display on the box.</param>
	/// <param name="content">Text, image and tooltip for this box.</param>
	/// <param name="style">The style to use. If left out, the box style from the current GUISkin is used.</param>
	public static void Box(Rect position, GUIContent content, GUIStyle style)
	{
		GUIUtility.CheckOnGUI();
		int controlID = GUIUtility.GetControlID(s_BoxHash, FocusType.Passive);
		if (Event.current.type == EventType.Repaint)
		{
			style.Draw(position, content, controlID);
		}
	}

	/// <summary>
	///   <para>Make a single press button. The user clicks them and something happens immediately.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the button.</param>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>true when the users clicks the button.</para>
	/// </returns>
	public static bool Button(Rect position, string text)
	{
		return Button(position, GUIContent.Temp(text), s_Skin.button);
	}

	/// <summary>
	///   <para>Make a single press button. The user clicks them and something happens immediately.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the button.</param>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>true when the users clicks the button.</para>
	/// </returns>
	public static bool Button(Rect position, Texture image)
	{
		return Button(position, GUIContent.Temp(image), s_Skin.button);
	}

	/// <summary>
	///   <para>Make a single press button. The user clicks them and something happens immediately.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the button.</param>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>true when the users clicks the button.</para>
	/// </returns>
	public static bool Button(Rect position, GUIContent content)
	{
		return Button(position, content, s_Skin.button);
	}

	/// <summary>
	///   <para>Make a single press button. The user clicks them and something happens immediately.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the button.</param>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>true when the users clicks the button.</para>
	/// </returns>
	public static bool Button(Rect position, string text, GUIStyle style)
	{
		return Button(position, GUIContent.Temp(text), style);
	}

	/// <summary>
	///   <para>Make a single press button. The user clicks them and something happens immediately.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the button.</param>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>true when the users clicks the button.</para>
	/// </returns>
	public static bool Button(Rect position, Texture image, GUIStyle style)
	{
		return Button(position, GUIContent.Temp(image), style);
	}

	/// <summary>
	///   <para>Make a single press button. The user clicks them and something happens immediately.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the button.</param>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>true when the users clicks the button.</para>
	/// </returns>
	public static bool Button(Rect position, GUIContent content, GUIStyle style)
	{
		GUIUtility.CheckOnGUI();
		return DoButton(position, content, style.m_Ptr);
	}

	/// <summary>
	///   <para>Make a button that is active as long as the user holds it down.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the button.</param>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>True when the users clicks the button.</para>
	/// </returns>
	public static bool RepeatButton(Rect position, string text)
	{
		return DoRepeatButton(position, GUIContent.Temp(text), s_Skin.button, FocusType.Passive);
	}

	/// <summary>
	///   <para>Make a button that is active as long as the user holds it down.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the button.</param>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>True when the users clicks the button.</para>
	/// </returns>
	public static bool RepeatButton(Rect position, Texture image)
	{
		return DoRepeatButton(position, GUIContent.Temp(image), s_Skin.button, FocusType.Passive);
	}

	/// <summary>
	///   <para>Make a button that is active as long as the user holds it down.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the button.</param>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>True when the users clicks the button.</para>
	/// </returns>
	public static bool RepeatButton(Rect position, GUIContent content)
	{
		return DoRepeatButton(position, content, s_Skin.button, FocusType.Passive);
	}

	/// <summary>
	///   <para>Make a button that is active as long as the user holds it down.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the button.</param>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>True when the users clicks the button.</para>
	/// </returns>
	public static bool RepeatButton(Rect position, string text, GUIStyle style)
	{
		return DoRepeatButton(position, GUIContent.Temp(text), style, FocusType.Passive);
	}

	/// <summary>
	///   <para>Make a button that is active as long as the user holds it down.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the button.</param>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>True when the users clicks the button.</para>
	/// </returns>
	public static bool RepeatButton(Rect position, Texture image, GUIStyle style)
	{
		return DoRepeatButton(position, GUIContent.Temp(image), style, FocusType.Passive);
	}

	/// <summary>
	///   <para>Make a button that is active as long as the user holds it down.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the button.</param>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>True when the users clicks the button.</para>
	/// </returns>
	public static bool RepeatButton(Rect position, GUIContent content, GUIStyle style)
	{
		return DoRepeatButton(position, content, style, FocusType.Passive);
	}

	private static bool DoRepeatButton(Rect position, GUIContent content, GUIStyle style, FocusType focusType)
	{
		GUIUtility.CheckOnGUI();
		int controlID = GUIUtility.GetControlID(s_RepeatButtonHash, focusType, position);
		switch (Event.current.GetTypeForControl(controlID))
		{
		case EventType.MouseDown:
			if (position.Contains(Event.current.mousePosition))
			{
				GUIUtility.hotControl = controlID;
				Event.current.Use();
			}
			return false;
		case EventType.MouseUp:
			if (GUIUtility.hotControl == controlID)
			{
				GUIUtility.hotControl = 0;
				Event.current.Use();
				return position.Contains(Event.current.mousePosition);
			}
			return false;
		case EventType.Repaint:
			style.Draw(position, content, controlID);
			return controlID == GUIUtility.hotControl && position.Contains(Event.current.mousePosition);
		default:
			return false;
		}
	}

	/// <summary>
	///   <para>Make a single-line text field where the user can edit a string.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the text field.</param>
	/// <param name="text">Text to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
	/// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
	/// <param name="style">The style to use. If left out, the textField style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>The edited string.</para>
	/// </returns>
	public static string TextField(Rect position, string text)
	{
		GUIContent gUIContent = GUIContent.Temp(text);
		DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), gUIContent, multiline: false, -1, skin.textField);
		return gUIContent.text;
	}

	/// <summary>
	///   <para>Make a single-line text field where the user can edit a string.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the text field.</param>
	/// <param name="text">Text to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
	/// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
	/// <param name="style">The style to use. If left out, the textField style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>The edited string.</para>
	/// </returns>
	public static string TextField(Rect position, string text, int maxLength)
	{
		GUIContent gUIContent = GUIContent.Temp(text);
		DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), gUIContent, multiline: false, maxLength, skin.textField);
		return gUIContent.text;
	}

	/// <summary>
	///   <para>Make a single-line text field where the user can edit a string.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the text field.</param>
	/// <param name="text">Text to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
	/// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
	/// <param name="style">The style to use. If left out, the textField style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>The edited string.</para>
	/// </returns>
	public static string TextField(Rect position, string text, GUIStyle style)
	{
		GUIContent gUIContent = GUIContent.Temp(text);
		DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), gUIContent, multiline: false, -1, style);
		return gUIContent.text;
	}

	/// <summary>
	///   <para>Make a single-line text field where the user can edit a string.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the text field.</param>
	/// <param name="text">Text to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
	/// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
	/// <param name="style">The style to use. If left out, the textField style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>The edited string.</para>
	/// </returns>
	public static string TextField(Rect position, string text, int maxLength, GUIStyle style)
	{
		GUIContent gUIContent = GUIContent.Temp(text);
		DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), gUIContent, multiline: false, maxLength, style);
		return gUIContent.text;
	}

	/// <summary>
	///   <para>Make a text field where the user can enter a password.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the text field.</param>
	/// <param name="password">Password to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
	/// <param name="maskChar">Character to mask the password with.</param>
	/// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
	/// <param name="style">The style to use. If left out, the textField style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>The edited password.</para>
	/// </returns>
	public static string PasswordField(Rect position, string password, char maskChar)
	{
		return PasswordField(position, password, maskChar, -1, skin.textField);
	}

	/// <summary>
	///   <para>Make a text field where the user can enter a password.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the text field.</param>
	/// <param name="password">Password to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
	/// <param name="maskChar">Character to mask the password with.</param>
	/// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
	/// <param name="style">The style to use. If left out, the textField style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>The edited password.</para>
	/// </returns>
	public static string PasswordField(Rect position, string password, char maskChar, int maxLength)
	{
		return PasswordField(position, password, maskChar, maxLength, skin.textField);
	}

	/// <summary>
	///   <para>Make a text field where the user can enter a password.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the text field.</param>
	/// <param name="password">Password to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
	/// <param name="maskChar">Character to mask the password with.</param>
	/// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
	/// <param name="style">The style to use. If left out, the textField style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>The edited password.</para>
	/// </returns>
	public static string PasswordField(Rect position, string password, char maskChar, GUIStyle style)
	{
		return PasswordField(position, password, maskChar, -1, style);
	}

	/// <summary>
	///   <para>Make a text field where the user can enter a password.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the text field.</param>
	/// <param name="password">Password to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
	/// <param name="maskChar">Character to mask the password with.</param>
	/// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
	/// <param name="style">The style to use. If left out, the textField style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>The edited password.</para>
	/// </returns>
	public static string PasswordField(Rect position, string password, char maskChar, int maxLength, GUIStyle style)
	{
		GUIUtility.CheckOnGUI();
		string t = PasswordFieldGetStrToShow(password, maskChar);
		GUIContent gUIContent = GUIContent.Temp(t);
		bool flag = changed;
		changed = false;
		if (TouchScreenKeyboard.isSupported)
		{
			DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard), gUIContent, multiline: false, maxLength, style, password, maskChar);
		}
		else
		{
			DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), gUIContent, multiline: false, maxLength, style);
		}
		t = ((!changed) ? password : gUIContent.text);
		changed |= flag;
		return t;
	}

	internal static string PasswordFieldGetStrToShow(string password, char maskChar)
	{
		return (Event.current.type != EventType.Repaint && Event.current.type != 0) ? password : "".PadRight(password.Length, maskChar);
	}

	/// <summary>
	///   <para>Make a Multi-line text area where the user can edit a string.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the text field.</param>
	/// <param name="text">Text to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
	/// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
	/// <param name="style">The style to use. If left out, the textArea style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>The edited string.</para>
	/// </returns>
	public static string TextArea(Rect position, string text)
	{
		GUIContent gUIContent = GUIContent.Temp(text);
		DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), gUIContent, multiline: true, -1, skin.textArea);
		return gUIContent.text;
	}

	/// <summary>
	///   <para>Make a Multi-line text area where the user can edit a string.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the text field.</param>
	/// <param name="text">Text to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
	/// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
	/// <param name="style">The style to use. If left out, the textArea style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>The edited string.</para>
	/// </returns>
	public static string TextArea(Rect position, string text, int maxLength)
	{
		GUIContent gUIContent = GUIContent.Temp(text);
		DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), gUIContent, multiline: true, maxLength, skin.textArea);
		return gUIContent.text;
	}

	/// <summary>
	///   <para>Make a Multi-line text area where the user can edit a string.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the text field.</param>
	/// <param name="text">Text to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
	/// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
	/// <param name="style">The style to use. If left out, the textArea style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>The edited string.</para>
	/// </returns>
	public static string TextArea(Rect position, string text, GUIStyle style)
	{
		GUIContent gUIContent = GUIContent.Temp(text);
		DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), gUIContent, multiline: true, -1, style);
		return gUIContent.text;
	}

	/// <summary>
	///   <para>Make a Multi-line text area where the user can edit a string.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the text field.</param>
	/// <param name="text">Text to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
	/// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
	/// <param name="style">The style to use. If left out, the textArea style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>The edited string.</para>
	/// </returns>
	public static string TextArea(Rect position, string text, int maxLength, GUIStyle style)
	{
		GUIContent gUIContent = GUIContent.Temp(text);
		DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), gUIContent, multiline: true, maxLength, style);
		return gUIContent.text;
	}

	private static string TextArea(Rect position, GUIContent content, int maxLength, GUIStyle style)
	{
		GUIContent gUIContent = GUIContent.Temp(content.text, content.image);
		DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), gUIContent, multiline: true, maxLength, style);
		return gUIContent.text;
	}

	internal static void DoTextField(Rect position, int id, GUIContent content, bool multiline, int maxLength, GUIStyle style)
	{
		DoTextField(position, id, content, multiline, maxLength, style, null);
	}

	internal static void DoTextField(Rect position, int id, GUIContent content, bool multiline, int maxLength, GUIStyle style, string secureText)
	{
		DoTextField(position, id, content, multiline, maxLength, style, secureText, '\0');
	}

	internal static void DoTextField(Rect position, int id, GUIContent content, bool multiline, int maxLength, GUIStyle style, string secureText, char maskChar)
	{
		GUIUtility.CheckOnGUI();
		if (maxLength >= 0 && content.text.Length > maxLength)
		{
			content.text = content.text.Substring(0, maxLength);
		}
		TextEditor textEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), id);
		textEditor.text = content.text;
		textEditor.SaveBackup();
		textEditor.position = position;
		textEditor.style = style;
		textEditor.multiline = multiline;
		textEditor.controlID = id;
		textEditor.DetectFocusChange();
		if (TouchScreenKeyboard.isSupported)
		{
			HandleTextFieldEventForTouchscreen(position, id, content, multiline, maxLength, style, secureText, maskChar, textEditor);
		}
		else
		{
			HandleTextFieldEventForDesktop(position, id, content, multiline, maxLength, style, textEditor);
		}
		textEditor.UpdateScrollOffsetIfNeeded(Event.current);
	}

	private static void HandleTextFieldEventForTouchscreen(Rect position, int id, GUIContent content, bool multiline, int maxLength, GUIStyle style, string secureText, char maskChar, TextEditor editor)
	{
		Event current = Event.current;
		switch (current.type)
		{
		case EventType.MouseDown:
			if (position.Contains(current.mousePosition))
			{
				GUIUtility.hotControl = id;
				if (s_HotTextField != -1 && s_HotTextField != id)
				{
					TextEditor textEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), s_HotTextField);
					textEditor.keyboardOnScreen = null;
				}
				s_HotTextField = id;
				if (GUIUtility.keyboardControl != id)
				{
					GUIUtility.keyboardControl = id;
				}
				editor.keyboardOnScreen = TouchScreenKeyboard.Open((secureText == null) ? content.text : secureText, TouchScreenKeyboardType.Default, autocorrection: true, multiline, secureText != null);
				current.Use();
			}
			break;
		case EventType.Repaint:
		{
			if (editor.keyboardOnScreen != null)
			{
				content.text = editor.keyboardOnScreen.text;
				if (maxLength >= 0 && content.text.Length > maxLength)
				{
					content.text = content.text.Substring(0, maxLength);
				}
				if (editor.keyboardOnScreen.status != 0)
				{
					editor.keyboardOnScreen = null;
					changed = true;
				}
			}
			string text = content.text;
			if (secureText != null)
			{
				content.text = PasswordFieldGetStrToShow(text, maskChar);
			}
			style.Draw(position, content, id, on: false);
			content.text = text;
			break;
		}
		}
	}

	private static void HandleTextFieldEventForDesktop(Rect position, int id, GUIContent content, bool multiline, int maxLength, GUIStyle style, TextEditor editor)
	{
		Event current = Event.current;
		bool flag = false;
		switch (current.type)
		{
		case EventType.MouseDown:
			if (position.Contains(current.mousePosition))
			{
				GUIUtility.hotControl = id;
				GUIUtility.keyboardControl = id;
				editor.m_HasFocus = true;
				editor.MoveCursorToPosition(Event.current.mousePosition);
				if (Event.current.clickCount == 2 && skin.settings.doubleClickSelectsWord)
				{
					editor.SelectCurrentWord();
					editor.DblClickSnap(TextEditor.DblClickSnapping.WORDS);
					editor.MouseDragSelectsWholeWords(on: true);
				}
				if (Event.current.clickCount == 3 && skin.settings.tripleClickSelectsLine)
				{
					editor.SelectCurrentParagraph();
					editor.MouseDragSelectsWholeWords(on: true);
					editor.DblClickSnap(TextEditor.DblClickSnapping.PARAGRAPHS);
				}
				current.Use();
			}
			break;
		case EventType.MouseDrag:
			if (GUIUtility.hotControl == id)
			{
				if (current.shift)
				{
					editor.MoveCursorToPosition(Event.current.mousePosition);
				}
				else
				{
					editor.SelectToPosition(Event.current.mousePosition);
				}
				current.Use();
			}
			break;
		case EventType.MouseUp:
			if (GUIUtility.hotControl == id)
			{
				editor.MouseDragSelectsWholeWords(on: false);
				GUIUtility.hotControl = 0;
				current.Use();
			}
			break;
		case EventType.KeyDown:
		{
			if (GUIUtility.keyboardControl != id)
			{
				return;
			}
			if (editor.HandleKeyEvent(current))
			{
				current.Use();
				flag = true;
				content.text = editor.text;
				break;
			}
			if (current.keyCode == KeyCode.Tab || current.character == '\t')
			{
				return;
			}
			char character = current.character;
			if (character == '\n' && !multiline && !current.alt)
			{
				return;
			}
			Font font = style.font;
			if (!font)
			{
				font = skin.font;
			}
			if (font.HasCharacter(character) || character == '\n')
			{
				editor.Insert(character);
				flag = true;
			}
			else if (character == '\0')
			{
				if (Input.compositionString.Length > 0)
				{
					editor.ReplaceSelection("");
					flag = true;
				}
				current.Use();
			}
			break;
		}
		case EventType.Repaint:
			if (GUIUtility.keyboardControl != id)
			{
				style.Draw(position, content, id, on: false);
			}
			else
			{
				editor.DrawCursor(content.text);
			}
			break;
		}
		if (GUIUtility.keyboardControl == id)
		{
			GUIUtility.textFieldInput = true;
		}
		if (flag)
		{
			changed = true;
			content.text = editor.text;
			if (maxLength >= 0 && content.text.Length > maxLength)
			{
				content.text = content.text.Substring(0, maxLength);
			}
			current.Use();
		}
	}

	/// <summary>
	///   <para>Make an on/off toggle button.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the button.</param>
	/// <param name="value">Is this button on or off?</param>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the toggle style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>The new value of the button.</para>
	/// </returns>
	public static bool Toggle(Rect position, bool value, string text)
	{
		return Toggle(position, value, GUIContent.Temp(text), s_Skin.toggle);
	}

	/// <summary>
	///   <para>Make an on/off toggle button.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the button.</param>
	/// <param name="value">Is this button on or off?</param>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the toggle style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>The new value of the button.</para>
	/// </returns>
	public static bool Toggle(Rect position, bool value, Texture image)
	{
		return Toggle(position, value, GUIContent.Temp(image), s_Skin.toggle);
	}

	/// <summary>
	///   <para>Make an on/off toggle button.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the button.</param>
	/// <param name="value">Is this button on or off?</param>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the toggle style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>The new value of the button.</para>
	/// </returns>
	public static bool Toggle(Rect position, bool value, GUIContent content)
	{
		return Toggle(position, value, content, s_Skin.toggle);
	}

	/// <summary>
	///   <para>Make an on/off toggle button.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the button.</param>
	/// <param name="value">Is this button on or off?</param>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the toggle style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>The new value of the button.</para>
	/// </returns>
	public static bool Toggle(Rect position, bool value, string text, GUIStyle style)
	{
		return Toggle(position, value, GUIContent.Temp(text), style);
	}

	/// <summary>
	///   <para>Make an on/off toggle button.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the button.</param>
	/// <param name="value">Is this button on or off?</param>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the toggle style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>The new value of the button.</para>
	/// </returns>
	public static bool Toggle(Rect position, bool value, Texture image, GUIStyle style)
	{
		return Toggle(position, value, GUIContent.Temp(image), style);
	}

	/// <summary>
	///   <para>Make an on/off toggle button.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the button.</param>
	/// <param name="value">Is this button on or off?</param>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the toggle style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>The new value of the button.</para>
	/// </returns>
	public static bool Toggle(Rect position, bool value, GUIContent content, GUIStyle style)
	{
		GUIUtility.CheckOnGUI();
		return DoToggle(position, GUIUtility.GetControlID(s_ToggleHash, FocusType.Passive, position), value, content, style.m_Ptr);
	}

	public static bool Toggle(Rect position, int id, bool value, GUIContent content, GUIStyle style)
	{
		GUIUtility.CheckOnGUI();
		return DoToggle(position, id, value, content, style.m_Ptr);
	}

	/// <summary>
	///   <para>Make a toolbar.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the toolbar.</param>
	/// <param name="selected">The index of the selected button.</param>
	/// <param name="texts">An array of strings to show on the toolbar buttons.</param>
	/// <param name="images">An array of textures on the toolbar buttons.</param>
	/// <param name="contents">An array of text, image and tooltips for the toolbar buttons.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="buttonSize">Determines how toolbar button size is calculated.</param>
	/// <returns>
	///   <para>The index of the selected button.</para>
	/// </returns>
	public static int Toolbar(Rect position, int selected, string[] texts)
	{
		return Toolbar(position, selected, GUIContent.Temp(texts), s_Skin.button);
	}

	/// <summary>
	///   <para>Make a toolbar.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the toolbar.</param>
	/// <param name="selected">The index of the selected button.</param>
	/// <param name="texts">An array of strings to show on the toolbar buttons.</param>
	/// <param name="images">An array of textures on the toolbar buttons.</param>
	/// <param name="contents">An array of text, image and tooltips for the toolbar buttons.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="buttonSize">Determines how toolbar button size is calculated.</param>
	/// <returns>
	///   <para>The index of the selected button.</para>
	/// </returns>
	public static int Toolbar(Rect position, int selected, Texture[] images)
	{
		return Toolbar(position, selected, GUIContent.Temp(images), s_Skin.button);
	}

	/// <summary>
	///   <para>Make a toolbar.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the toolbar.</param>
	/// <param name="selected">The index of the selected button.</param>
	/// <param name="texts">An array of strings to show on the toolbar buttons.</param>
	/// <param name="images">An array of textures on the toolbar buttons.</param>
	/// <param name="contents">An array of text, image and tooltips for the toolbar buttons.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="buttonSize">Determines how toolbar button size is calculated.</param>
	/// <returns>
	///   <para>The index of the selected button.</para>
	/// </returns>
	public static int Toolbar(Rect position, int selected, GUIContent[] contents)
	{
		return Toolbar(position, selected, contents, s_Skin.button);
	}

	/// <summary>
	///   <para>Make a toolbar.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the toolbar.</param>
	/// <param name="selected">The index of the selected button.</param>
	/// <param name="texts">An array of strings to show on the toolbar buttons.</param>
	/// <param name="images">An array of textures on the toolbar buttons.</param>
	/// <param name="contents">An array of text, image and tooltips for the toolbar buttons.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="buttonSize">Determines how toolbar button size is calculated.</param>
	/// <returns>
	///   <para>The index of the selected button.</para>
	/// </returns>
	public static int Toolbar(Rect position, int selected, string[] texts, GUIStyle style)
	{
		return Toolbar(position, selected, GUIContent.Temp(texts), style);
	}

	/// <summary>
	///   <para>Make a toolbar.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the toolbar.</param>
	/// <param name="selected">The index of the selected button.</param>
	/// <param name="texts">An array of strings to show on the toolbar buttons.</param>
	/// <param name="images">An array of textures on the toolbar buttons.</param>
	/// <param name="contents">An array of text, image and tooltips for the toolbar buttons.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="buttonSize">Determines how toolbar button size is calculated.</param>
	/// <returns>
	///   <para>The index of the selected button.</para>
	/// </returns>
	public static int Toolbar(Rect position, int selected, Texture[] images, GUIStyle style)
	{
		return Toolbar(position, selected, GUIContent.Temp(images), style);
	}

	/// <summary>
	///   <para>Make a toolbar.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the toolbar.</param>
	/// <param name="selected">The index of the selected button.</param>
	/// <param name="texts">An array of strings to show on the toolbar buttons.</param>
	/// <param name="images">An array of textures on the toolbar buttons.</param>
	/// <param name="contents">An array of text, image and tooltips for the toolbar buttons.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="buttonSize">Determines how toolbar button size is calculated.</param>
	/// <returns>
	///   <para>The index of the selected button.</para>
	/// </returns>
	public static int Toolbar(Rect position, int selected, GUIContent[] contents, GUIStyle style)
	{
		return Toolbar(position, selected, contents, null, style, ToolbarButtonSize.Fixed);
	}

	public static int Toolbar(Rect position, int selected, GUIContent[] contents, GUIStyle style, ToolbarButtonSize buttonSize)
	{
		return Toolbar(position, selected, contents, null, style, buttonSize);
	}

	internal static int Toolbar(Rect position, int selected, GUIContent[] contents, string[] controlNames, GUIStyle style, ToolbarButtonSize buttonSize)
	{
		GUIUtility.CheckOnGUI();
		FindStyles(ref style, out var firstStyle, out var midStyle, out var lastStyle, "left", "mid", "right");
		return DoButtonGrid(position, selected, contents, controlNames, contents.Length, style, firstStyle, midStyle, lastStyle, buttonSize);
	}

	/// <summary>
	///   <para>Make a grid of buttons.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the grid.</param>
	/// <param name="selected">The index of the selected grid button.</param>
	/// <param name="texts">An array of strings to show on the grid buttons.</param>
	/// <param name="images">An array of textures on the grid buttons.</param>
	/// <param name="contents">An array of text, image and tooltips for the grid button.</param>
	/// <param name="xCount">How many elements to fit in the horizontal direction. The controls will be scaled to fit unless the style defines a fixedWidth to use.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="content"></param>
	/// <returns>
	///   <para>The index of the selected button.</para>
	/// </returns>
	public static int SelectionGrid(Rect position, int selected, string[] texts, int xCount)
	{
		return SelectionGrid(position, selected, GUIContent.Temp(texts), xCount, null);
	}

	/// <summary>
	///   <para>Make a grid of buttons.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the grid.</param>
	/// <param name="selected">The index of the selected grid button.</param>
	/// <param name="texts">An array of strings to show on the grid buttons.</param>
	/// <param name="images">An array of textures on the grid buttons.</param>
	/// <param name="contents">An array of text, image and tooltips for the grid button.</param>
	/// <param name="xCount">How many elements to fit in the horizontal direction. The controls will be scaled to fit unless the style defines a fixedWidth to use.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="content"></param>
	/// <returns>
	///   <para>The index of the selected button.</para>
	/// </returns>
	public static int SelectionGrid(Rect position, int selected, Texture[] images, int xCount)
	{
		return SelectionGrid(position, selected, GUIContent.Temp(images), xCount, null);
	}

	/// <summary>
	///   <para>Make a grid of buttons.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the grid.</param>
	/// <param name="selected">The index of the selected grid button.</param>
	/// <param name="texts">An array of strings to show on the grid buttons.</param>
	/// <param name="images">An array of textures on the grid buttons.</param>
	/// <param name="contents">An array of text, image and tooltips for the grid button.</param>
	/// <param name="xCount">How many elements to fit in the horizontal direction. The controls will be scaled to fit unless the style defines a fixedWidth to use.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="content"></param>
	/// <returns>
	///   <para>The index of the selected button.</para>
	/// </returns>
	public static int SelectionGrid(Rect position, int selected, GUIContent[] content, int xCount)
	{
		return SelectionGrid(position, selected, content, xCount, null);
	}

	/// <summary>
	///   <para>Make a grid of buttons.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the grid.</param>
	/// <param name="selected">The index of the selected grid button.</param>
	/// <param name="texts">An array of strings to show on the grid buttons.</param>
	/// <param name="images">An array of textures on the grid buttons.</param>
	/// <param name="contents">An array of text, image and tooltips for the grid button.</param>
	/// <param name="xCount">How many elements to fit in the horizontal direction. The controls will be scaled to fit unless the style defines a fixedWidth to use.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="content"></param>
	/// <returns>
	///   <para>The index of the selected button.</para>
	/// </returns>
	public static int SelectionGrid(Rect position, int selected, string[] texts, int xCount, GUIStyle style)
	{
		return SelectionGrid(position, selected, GUIContent.Temp(texts), xCount, style);
	}

	/// <summary>
	///   <para>Make a grid of buttons.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the grid.</param>
	/// <param name="selected">The index of the selected grid button.</param>
	/// <param name="texts">An array of strings to show on the grid buttons.</param>
	/// <param name="images">An array of textures on the grid buttons.</param>
	/// <param name="contents">An array of text, image and tooltips for the grid button.</param>
	/// <param name="xCount">How many elements to fit in the horizontal direction. The controls will be scaled to fit unless the style defines a fixedWidth to use.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="content"></param>
	/// <returns>
	///   <para>The index of the selected button.</para>
	/// </returns>
	public static int SelectionGrid(Rect position, int selected, Texture[] images, int xCount, GUIStyle style)
	{
		return SelectionGrid(position, selected, GUIContent.Temp(images), xCount, style);
	}

	/// <summary>
	///   <para>Make a grid of buttons.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the grid.</param>
	/// <param name="selected">The index of the selected grid button.</param>
	/// <param name="texts">An array of strings to show on the grid buttons.</param>
	/// <param name="images">An array of textures on the grid buttons.</param>
	/// <param name="contents">An array of text, image and tooltips for the grid button.</param>
	/// <param name="xCount">How many elements to fit in the horizontal direction. The controls will be scaled to fit unless the style defines a fixedWidth to use.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="content"></param>
	/// <returns>
	///   <para>The index of the selected button.</para>
	/// </returns>
	public static int SelectionGrid(Rect position, int selected, GUIContent[] contents, int xCount, GUIStyle style)
	{
		if (style == null)
		{
			style = s_Skin.button;
		}
		return DoButtonGrid(position, selected, contents, null, xCount, style, style, style, style, ToolbarButtonSize.Fixed);
	}

	internal static void FindStyles(ref GUIStyle style, out GUIStyle firstStyle, out GUIStyle midStyle, out GUIStyle lastStyle, string first, string mid, string last)
	{
		if (style == null)
		{
			style = skin.button;
		}
		string name = style.name;
		midStyle = skin.FindStyle(name + mid);
		if (midStyle == null)
		{
			midStyle = style;
		}
		firstStyle = skin.FindStyle(name + first);
		if (firstStyle == null)
		{
			firstStyle = midStyle;
		}
		lastStyle = skin.FindStyle(name + last);
		if (lastStyle == null)
		{
			lastStyle = midStyle;
		}
	}

	internal static int CalcTotalHorizSpacing(int xCount, GUIStyle style, GUIStyle firstStyle, GUIStyle midStyle, GUIStyle lastStyle)
	{
		if (xCount < 2)
		{
			return 0;
		}
		if (xCount == 2)
		{
			return Mathf.Max(firstStyle.margin.right, lastStyle.margin.left);
		}
		int num = Mathf.Max(midStyle.margin.left, midStyle.margin.right);
		return Mathf.Max(firstStyle.margin.right, midStyle.margin.left) + Mathf.Max(midStyle.margin.right, lastStyle.margin.left) + num * (xCount - 3);
	}

	private static int DoButtonGrid(Rect position, int selected, GUIContent[] contents, string[] controlNames, int xCount, GUIStyle style, GUIStyle firstStyle, GUIStyle midStyle, GUIStyle lastStyle, ToolbarButtonSize buttonSize)
	{
		GUIUtility.CheckOnGUI();
		int num = contents.Length;
		if (num == 0)
		{
			return selected;
		}
		if (xCount <= 0)
		{
			Debug.LogWarning("You are trying to create a SelectionGrid with zero or less elements to be displayed in the horizontal direction. Set xCount to a positive value.");
			return selected;
		}
		int num2 = num / xCount;
		if (num % xCount != 0)
		{
			num2++;
		}
		float num3 = CalcTotalHorizSpacing(xCount, style, firstStyle, midStyle, lastStyle);
		float num4 = Mathf.Max(style.margin.top, style.margin.bottom) * (num2 - 1);
		float elemWidth = (position.width - num3) / (float)xCount;
		float elemHeight = (position.height - num4) / (float)num2;
		if (style.fixedWidth != 0f)
		{
			elemWidth = style.fixedWidth;
		}
		if (style.fixedHeight != 0f)
		{
			elemHeight = style.fixedHeight;
		}
		Rect[] array = CalcMouseRects(position, contents, xCount, elemWidth, elemHeight, style, firstStyle, midStyle, lastStyle, addBorders: false, buttonSize);
		GUIStyle gUIStyle = null;
		int num5 = 0;
		for (int i = 0; i < num; i++)
		{
			Rect rect = array[i];
			GUIContent gUIContent = contents[i];
			if (controlNames != null)
			{
				SetNextControlName(controlNames[i]);
			}
			int controlID = GUIUtility.GetControlID(s_ButtonGridHash, FocusType.Passive, rect);
			if (i == selected)
			{
				num5 = controlID;
			}
			switch (Event.current.GetTypeForControl(controlID))
			{
			case EventType.MouseDown:
				if (rect.Contains(Event.current.mousePosition))
				{
					GUIUtility.hotControl = controlID;
					Event.current.Use();
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID)
				{
					Event.current.Use();
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID)
				{
					GUIUtility.hotControl = 0;
					Event.current.Use();
					changed = true;
					return i;
				}
				break;
			case EventType.Repaint:
			{
				GUIStyle gUIStyle2 = ((num == 1) ? style : ((i == 0) ? firstStyle : ((i != num - 1) ? midStyle : lastStyle)));
				bool flag = rect.Contains(Event.current.mousePosition);
				bool flag2 = GUIUtility.hotControl == controlID;
				if (selected != i)
				{
					gUIStyle2.Draw(rect, gUIContent, flag && (enabled || flag2) && (flag2 || GUIUtility.hotControl == 0), enabled && flag2, on: false, hasKeyboardFocus: false);
				}
				else
				{
					gUIStyle = gUIStyle2;
				}
				if (flag)
				{
					GUIUtility.mouseUsed = true;
					if (!string.IsNullOrEmpty(gUIContent.tooltip))
					{
						GUIStyle.SetMouseTooltip(gUIContent.tooltip, rect);
					}
				}
				break;
			}
			}
		}
		if (gUIStyle != null)
		{
			Rect position2 = array[selected];
			GUIContent content = contents[selected];
			bool flag3 = position2.Contains(Event.current.mousePosition);
			bool flag4 = GUIUtility.hotControl == num5;
			gUIStyle.Draw(position2, content, flag3 && (enabled || flag4) && (flag4 || GUIUtility.hotControl == 0), enabled && flag4, on: true, hasKeyboardFocus: false);
		}
		return selected;
	}

	private static Rect[] CalcMouseRects(Rect position, GUIContent[] contents, int xCount, float elemWidth, float elemHeight, GUIStyle style, GUIStyle firstStyle, GUIStyle midStyle, GUIStyle lastStyle, bool addBorders, ToolbarButtonSize buttonSize)
	{
		int num = contents.Length;
		int num2 = 0;
		int num3 = 0;
		float num4 = position.xMin;
		float num5 = position.yMin;
		GUIStyle gUIStyle = style;
		Rect[] array = new Rect[num];
		if (num > 1)
		{
			gUIStyle = firstStyle;
		}
		for (int i = 0; i < num; i++)
		{
			float num6 = 0f;
			switch (buttonSize)
			{
			case ToolbarButtonSize.Fixed:
				num6 = elemWidth;
				break;
			case ToolbarButtonSize.FitToContents:
				num6 = gUIStyle.CalcSize(contents[i]).x;
				break;
			}
			if (!addBorders)
			{
				ref Rect reference = ref array[i];
				reference = new Rect(num4, num5, num6, elemHeight);
			}
			else
			{
				ref Rect reference2 = ref array[i];
				reference2 = gUIStyle.margin.Add(new Rect(num4, num5, num6, elemHeight));
			}
			array[i].width = Mathf.Round(array[i].xMax) - Mathf.Round(array[i].x);
			array[i].x = Mathf.Round(array[i].x);
			GUIStyle gUIStyle2 = midStyle;
			if (i == num - 2 || i == xCount - 2)
			{
				gUIStyle2 = lastStyle;
			}
			num4 += num6 + (float)Mathf.Max(gUIStyle.margin.right, gUIStyle2.margin.left);
			num3++;
			if (num3 >= xCount)
			{
				num2++;
				num3 = 0;
				num5 += elemHeight + (float)Mathf.Max(style.margin.top, style.margin.bottom);
				num4 = position.xMin;
				gUIStyle2 = firstStyle;
			}
			gUIStyle = gUIStyle2;
		}
		return array;
	}

	/// <summary>
	///   <para>A horizontal slider the user can drag to change a value between a min and a max.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the slider.</param>
	/// <param name="value">The value the slider shows. This determines the position of the draggable thumb.</param>
	/// <param name="leftValue">The value at the left end of the slider.</param>
	/// <param name="rightValue">The value at the right end of the slider.</param>
	/// <param name="slider">The GUIStyle to use for displaying the dragging area. If left out, the horizontalSlider style from the current GUISkin is used.</param>
	/// <param name="thumb">The GUIStyle to use for displaying draggable thumb. If left out, the horizontalSliderThumb style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>The value that has been set by the user.</para>
	/// </returns>
	public static float HorizontalSlider(Rect position, float value, float leftValue, float rightValue)
	{
		return Slider(position, value, 0f, leftValue, rightValue, skin.horizontalSlider, skin.horizontalSliderThumb, horiz: true, 0);
	}

	/// <summary>
	///   <para>A horizontal slider the user can drag to change a value between a min and a max.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the slider.</param>
	/// <param name="value">The value the slider shows. This determines the position of the draggable thumb.</param>
	/// <param name="leftValue">The value at the left end of the slider.</param>
	/// <param name="rightValue">The value at the right end of the slider.</param>
	/// <param name="slider">The GUIStyle to use for displaying the dragging area. If left out, the horizontalSlider style from the current GUISkin is used.</param>
	/// <param name="thumb">The GUIStyle to use for displaying draggable thumb. If left out, the horizontalSliderThumb style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>The value that has been set by the user.</para>
	/// </returns>
	public static float HorizontalSlider(Rect position, float value, float leftValue, float rightValue, GUIStyle slider, GUIStyle thumb)
	{
		return Slider(position, value, 0f, leftValue, rightValue, slider, thumb, horiz: true, 0);
	}

	/// <summary>
	///   <para>A vertical slider the user can drag to change a value between a min and a max.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the slider.</param>
	/// <param name="value">The value the slider shows. This determines the position of the draggable thumb.</param>
	/// <param name="topValue">The value at the top end of the slider.</param>
	/// <param name="bottomValue">The value at the bottom end of the slider.</param>
	/// <param name="slider">The GUIStyle to use for displaying the dragging area. If left out, the horizontalSlider style from the current GUISkin is used.</param>
	/// <param name="thumb">The GUIStyle to use for displaying draggable thumb. If left out, the horizontalSliderThumb style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>The value that has been set by the user.</para>
	/// </returns>
	public static float VerticalSlider(Rect position, float value, float topValue, float bottomValue)
	{
		return Slider(position, value, 0f, topValue, bottomValue, skin.verticalSlider, skin.verticalSliderThumb, horiz: false, 0);
	}

	/// <summary>
	///   <para>A vertical slider the user can drag to change a value between a min and a max.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the slider.</param>
	/// <param name="value">The value the slider shows. This determines the position of the draggable thumb.</param>
	/// <param name="topValue">The value at the top end of the slider.</param>
	/// <param name="bottomValue">The value at the bottom end of the slider.</param>
	/// <param name="slider">The GUIStyle to use for displaying the dragging area. If left out, the horizontalSlider style from the current GUISkin is used.</param>
	/// <param name="thumb">The GUIStyle to use for displaying draggable thumb. If left out, the horizontalSliderThumb style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>The value that has been set by the user.</para>
	/// </returns>
	public static float VerticalSlider(Rect position, float value, float topValue, float bottomValue, GUIStyle slider, GUIStyle thumb)
	{
		return Slider(position, value, 0f, topValue, bottomValue, slider, thumb, horiz: false, 0);
	}

	public static float Slider(Rect position, float value, float size, float start, float end, GUIStyle slider, GUIStyle thumb, bool horiz, int id)
	{
		GUIUtility.CheckOnGUI();
		if (id == 0)
		{
			id = GUIUtility.GetControlID(s_SliderHash, FocusType.Passive, position);
		}
		return new SliderHandler(position, value, size, start, end, slider, thumb, horiz, id).Handle();
	}

	/// <summary>
	///   <para>Make a horizontal scrollbar. Scrollbars are what you use to scroll through a document. Most likely, you want to use scrollViews instead.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the scrollbar.</param>
	/// <param name="value">The position between min and max.</param>
	/// <param name="size">How much can we see?</param>
	/// <param name="leftValue">The value at the left end of the scrollbar.</param>
	/// <param name="rightValue">The value at the right end of the scrollbar.</param>
	/// <param name="style">The style to use for the scrollbar background. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>The modified value. This can be changed by the user by dragging the scrollbar, or clicking the arrows at the end.</para>
	/// </returns>
	public static float HorizontalScrollbar(Rect position, float value, float size, float leftValue, float rightValue)
	{
		return Scroller(position, value, size, leftValue, rightValue, skin.horizontalScrollbar, skin.horizontalScrollbarThumb, skin.horizontalScrollbarLeftButton, skin.horizontalScrollbarRightButton, horiz: true);
	}

	/// <summary>
	///   <para>Make a horizontal scrollbar. Scrollbars are what you use to scroll through a document. Most likely, you want to use scrollViews instead.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the scrollbar.</param>
	/// <param name="value">The position between min and max.</param>
	/// <param name="size">How much can we see?</param>
	/// <param name="leftValue">The value at the left end of the scrollbar.</param>
	/// <param name="rightValue">The value at the right end of the scrollbar.</param>
	/// <param name="style">The style to use for the scrollbar background. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>The modified value. This can be changed by the user by dragging the scrollbar, or clicking the arrows at the end.</para>
	/// </returns>
	public static float HorizontalScrollbar(Rect position, float value, float size, float leftValue, float rightValue, GUIStyle style)
	{
		return Scroller(position, value, size, leftValue, rightValue, style, skin.GetStyle(style.name + "thumb"), skin.GetStyle(style.name + "leftbutton"), skin.GetStyle(style.name + "rightbutton"), horiz: true);
	}

	internal static bool ScrollerRepeatButton(int scrollerID, Rect rect, GUIStyle style)
	{
		bool result = false;
		if (DoRepeatButton(rect, GUIContent.none, style, FocusType.Passive))
		{
			bool flag = s_ScrollControlId != scrollerID;
			s_ScrollControlId = scrollerID;
			if (flag)
			{
				result = true;
				nextScrollStepTime = DateTime.Now.AddMilliseconds(250.0);
			}
			else if (DateTime.Now >= nextScrollStepTime)
			{
				result = true;
				nextScrollStepTime = DateTime.Now.AddMilliseconds(30.0);
			}
			if (Event.current.type == EventType.Repaint)
			{
				InternalRepaintEditorWindow();
			}
		}
		return result;
	}

	/// <summary>
	///   <para>Make a vertical scrollbar. Scrollbars are what you use to scroll through a document. Most likely, you want to use scrollViews instead.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the scrollbar.</param>
	/// <param name="value">The position between min and max.</param>
	/// <param name="size">How much can we see?</param>
	/// <param name="topValue">The value at the top of the scrollbar.</param>
	/// <param name="bottomValue">The value at the bottom of the scrollbar.</param>
	/// <param name="style">The style to use for the scrollbar background. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>The modified value. This can be changed by the user by dragging the scrollbar, or clicking the arrows at the end.</para>
	/// </returns>
	public static float VerticalScrollbar(Rect position, float value, float size, float topValue, float bottomValue)
	{
		return Scroller(position, value, size, topValue, bottomValue, skin.verticalScrollbar, skin.verticalScrollbarThumb, skin.verticalScrollbarUpButton, skin.verticalScrollbarDownButton, horiz: false);
	}

	/// <summary>
	///   <para>Make a vertical scrollbar. Scrollbars are what you use to scroll through a document. Most likely, you want to use scrollViews instead.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the scrollbar.</param>
	/// <param name="value">The position between min and max.</param>
	/// <param name="size">How much can we see?</param>
	/// <param name="topValue">The value at the top of the scrollbar.</param>
	/// <param name="bottomValue">The value at the bottom of the scrollbar.</param>
	/// <param name="style">The style to use for the scrollbar background. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
	/// <returns>
	///   <para>The modified value. This can be changed by the user by dragging the scrollbar, or clicking the arrows at the end.</para>
	/// </returns>
	public static float VerticalScrollbar(Rect position, float value, float size, float topValue, float bottomValue, GUIStyle style)
	{
		return Scroller(position, value, size, topValue, bottomValue, style, skin.GetStyle(style.name + "thumb"), skin.GetStyle(style.name + "upbutton"), skin.GetStyle(style.name + "downbutton"), horiz: false);
	}

	internal static float Scroller(Rect position, float value, float size, float leftValue, float rightValue, GUIStyle slider, GUIStyle thumb, GUIStyle leftButton, GUIStyle rightButton, bool horiz)
	{
		GUIUtility.CheckOnGUI();
		int controlID = GUIUtility.GetControlID(s_SliderHash, FocusType.Passive, position);
		Rect position2;
		Rect rect;
		Rect rect2;
		if (horiz)
		{
			position2 = new Rect(position.x + leftButton.fixedWidth, position.y, position.width - leftButton.fixedWidth - rightButton.fixedWidth, position.height);
			rect = new Rect(position.x, position.y, leftButton.fixedWidth, position.height);
			rect2 = new Rect(position.xMax - rightButton.fixedWidth, position.y, rightButton.fixedWidth, position.height);
		}
		else
		{
			position2 = new Rect(position.x, position.y + leftButton.fixedHeight, position.width, position.height - leftButton.fixedHeight - rightButton.fixedHeight);
			rect = new Rect(position.x, position.y, position.width, leftButton.fixedHeight);
			rect2 = new Rect(position.x, position.yMax - rightButton.fixedHeight, position.width, rightButton.fixedHeight);
		}
		value = Slider(position2, value, size, leftValue, rightValue, slider, thumb, horiz, controlID);
		bool flag = false;
		if (Event.current.type == EventType.MouseUp)
		{
			flag = true;
		}
		if (ScrollerRepeatButton(controlID, rect, leftButton))
		{
			value -= s_ScrollStepSize * ((!(leftValue < rightValue)) ? (-1f) : 1f);
		}
		if (ScrollerRepeatButton(controlID, rect2, rightButton))
		{
			value += s_ScrollStepSize * ((!(leftValue < rightValue)) ? (-1f) : 1f);
		}
		if (flag && Event.current.type == EventType.Used)
		{
			s_ScrollControlId = 0;
		}
		value = ((!(leftValue < rightValue)) ? Mathf.Clamp(value, rightValue, leftValue - size) : Mathf.Clamp(value, leftValue, rightValue - size));
		return value;
	}

	public static void BeginClip(Rect position, Vector2 scrollOffset, Vector2 renderOffset, bool resetOffset)
	{
		GUIUtility.CheckOnGUI();
		GUIClip.Push(position, scrollOffset, renderOffset, resetOffset);
	}

	/// <summary>
	///   <para>Begin a group. Must be matched with a call to EndGroup.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the group.</param>
	/// <param name="text">Text to display on the group.</param>
	/// <param name="image">Texture to display on the group.</param>
	/// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
	/// <param name="style">The style to use for the background.</param>
	public static void BeginGroup(Rect position)
	{
		BeginGroup(position, GUIContent.none, GUIStyle.none);
	}

	/// <summary>
	///   <para>Begin a group. Must be matched with a call to EndGroup.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the group.</param>
	/// <param name="text">Text to display on the group.</param>
	/// <param name="image">Texture to display on the group.</param>
	/// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
	/// <param name="style">The style to use for the background.</param>
	public static void BeginGroup(Rect position, string text)
	{
		BeginGroup(position, GUIContent.Temp(text), GUIStyle.none);
	}

	/// <summary>
	///   <para>Begin a group. Must be matched with a call to EndGroup.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the group.</param>
	/// <param name="text">Text to display on the group.</param>
	/// <param name="image">Texture to display on the group.</param>
	/// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
	/// <param name="style">The style to use for the background.</param>
	public static void BeginGroup(Rect position, Texture image)
	{
		BeginGroup(position, GUIContent.Temp(image), GUIStyle.none);
	}

	/// <summary>
	///   <para>Begin a group. Must be matched with a call to EndGroup.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the group.</param>
	/// <param name="text">Text to display on the group.</param>
	/// <param name="image">Texture to display on the group.</param>
	/// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
	/// <param name="style">The style to use for the background.</param>
	public static void BeginGroup(Rect position, GUIContent content)
	{
		BeginGroup(position, content, GUIStyle.none);
	}

	/// <summary>
	///   <para>Begin a group. Must be matched with a call to EndGroup.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the group.</param>
	/// <param name="text">Text to display on the group.</param>
	/// <param name="image">Texture to display on the group.</param>
	/// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
	/// <param name="style">The style to use for the background.</param>
	public static void BeginGroup(Rect position, GUIStyle style)
	{
		BeginGroup(position, GUIContent.none, style);
	}

	/// <summary>
	///   <para>Begin a group. Must be matched with a call to EndGroup.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the group.</param>
	/// <param name="text">Text to display on the group.</param>
	/// <param name="image">Texture to display on the group.</param>
	/// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
	/// <param name="style">The style to use for the background.</param>
	public static void BeginGroup(Rect position, string text, GUIStyle style)
	{
		BeginGroup(position, GUIContent.Temp(text), style);
	}

	/// <summary>
	///   <para>Begin a group. Must be matched with a call to EndGroup.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the group.</param>
	/// <param name="text">Text to display on the group.</param>
	/// <param name="image">Texture to display on the group.</param>
	/// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
	/// <param name="style">The style to use for the background.</param>
	public static void BeginGroup(Rect position, Texture image, GUIStyle style)
	{
		BeginGroup(position, GUIContent.Temp(image), style);
	}

	/// <summary>
	///   <para>Begin a group. Must be matched with a call to EndGroup.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the group.</param>
	/// <param name="text">Text to display on the group.</param>
	/// <param name="image">Texture to display on the group.</param>
	/// <param name="content">Text, image and tooltip for this group. If supplied, any mouse clicks are "captured" by the group and not If left out, no background is rendered, and mouse clicks are passed.</param>
	/// <param name="style">The style to use for the background.</param>
	public static void BeginGroup(Rect position, GUIContent content, GUIStyle style)
	{
		BeginGroup(position, content, style, Vector2.zero);
	}

	internal static void BeginGroup(Rect position, GUIContent content, GUIStyle style, Vector2 scrollOffset)
	{
		GUIUtility.CheckOnGUI();
		int controlID = GUIUtility.GetControlID(s_BeginGroupHash, FocusType.Passive);
		if (content != GUIContent.none || style != GUIStyle.none)
		{
			EventType type = Event.current.type;
			if (type == EventType.Repaint)
			{
				style.Draw(position, content, controlID);
			}
			else if (position.Contains(Event.current.mousePosition))
			{
				GUIUtility.mouseUsed = true;
			}
		}
		GUIClip.Push(position, scrollOffset, Vector2.zero, resetOffset: false);
	}

	/// <summary>
	///   <para>End a group.</para>
	/// </summary>
	public static void EndGroup()
	{
		GUIUtility.CheckOnGUI();
		GUIClip.Internal_Pop();
	}

	public static void BeginClip(Rect position)
	{
		GUIUtility.CheckOnGUI();
		GUIClip.Push(position, Vector2.zero, Vector2.zero, resetOffset: false);
	}

	public static void EndClip()
	{
		GUIUtility.CheckOnGUI();
		GUIClip.Pop();
	}

	/// <summary>
	///   <para>Begin a scrolling view inside your GUI.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the ScrollView.</param>
	/// <param name="scrollPosition">The pixel distance that the view is scrolled in the X and Y directions.</param>
	/// <param name="viewRect">The rectangle used inside the scrollview.</param>
	/// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
	/// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
	/// <param name="alwaysShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when viewRect is wider than position.</param>
	/// <param name="alwaysShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when viewRect is taller than position.</param>
	/// <returns>
	///   <para>The modified scrollPosition. Feed this back into the variable you pass in, as shown in the example.</para>
	/// </returns>
	public static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect)
	{
		return BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal: false, alwaysShowVertical: false, skin.horizontalScrollbar, skin.verticalScrollbar, skin.scrollView);
	}

	/// <summary>
	///   <para>Begin a scrolling view inside your GUI.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the ScrollView.</param>
	/// <param name="scrollPosition">The pixel distance that the view is scrolled in the X and Y directions.</param>
	/// <param name="viewRect">The rectangle used inside the scrollview.</param>
	/// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
	/// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
	/// <param name="alwaysShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when viewRect is wider than position.</param>
	/// <param name="alwaysShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when viewRect is taller than position.</param>
	/// <returns>
	///   <para>The modified scrollPosition. Feed this back into the variable you pass in, as shown in the example.</para>
	/// </returns>
	public static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical)
	{
		return BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical, skin.horizontalScrollbar, skin.verticalScrollbar, skin.scrollView);
	}

	/// <summary>
	///   <para>Begin a scrolling view inside your GUI.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the ScrollView.</param>
	/// <param name="scrollPosition">The pixel distance that the view is scrolled in the X and Y directions.</param>
	/// <param name="viewRect">The rectangle used inside the scrollview.</param>
	/// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
	/// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
	/// <param name="alwaysShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when viewRect is wider than position.</param>
	/// <param name="alwaysShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when viewRect is taller than position.</param>
	/// <returns>
	///   <para>The modified scrollPosition. Feed this back into the variable you pass in, as shown in the example.</para>
	/// </returns>
	public static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar)
	{
		return BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal: false, alwaysShowVertical: false, horizontalScrollbar, verticalScrollbar, skin.scrollView);
	}

	/// <summary>
	///   <para>Begin a scrolling view inside your GUI.</para>
	/// </summary>
	/// <param name="position">Rectangle on the screen to use for the ScrollView.</param>
	/// <param name="scrollPosition">The pixel distance that the view is scrolled in the X and Y directions.</param>
	/// <param name="viewRect">The rectangle used inside the scrollview.</param>
	/// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
	/// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
	/// <param name="alwaysShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when viewRect is wider than position.</param>
	/// <param name="alwaysShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when viewRect is taller than position.</param>
	/// <returns>
	///   <para>The modified scrollPosition. Feed this back into the variable you pass in, as shown in the example.</para>
	/// </returns>
	public static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar)
	{
		return BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, skin.scrollView);
	}

	protected static Vector2 DoBeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background)
	{
		return BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, background);
	}

	internal static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background)
	{
		GUIUtility.CheckOnGUI();
		int controlID = GUIUtility.GetControlID(s_ScrollviewHash, FocusType.Passive);
		ScrollViewState scrollViewState = (ScrollViewState)GUIUtility.GetStateObject(typeof(ScrollViewState), controlID);
		if (scrollViewState.apply)
		{
			scrollPosition = scrollViewState.scrollPosition;
			scrollViewState.apply = false;
		}
		scrollViewState.position = position;
		scrollViewState.scrollPosition = scrollPosition;
		scrollViewState.visibleRect = (scrollViewState.viewRect = viewRect);
		scrollViewState.visibleRect.width = position.width;
		scrollViewState.visibleRect.height = position.height;
		s_ScrollViewStates.Push(scrollViewState);
		Rect screenRect = new Rect(position);
		switch (Event.current.type)
		{
		case EventType.Layout:
			GUIUtility.GetControlID(s_SliderHash, FocusType.Passive);
			GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
			GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
			GUIUtility.GetControlID(s_SliderHash, FocusType.Passive);
			GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
			GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
			break;
		default:
		{
			bool flag = alwaysShowVertical;
			bool flag2 = alwaysShowHorizontal;
			if (flag2 || viewRect.width > screenRect.width)
			{
				scrollViewState.visibleRect.height = position.height - horizontalScrollbar.fixedHeight + (float)horizontalScrollbar.margin.top;
				screenRect.height -= horizontalScrollbar.fixedHeight + (float)horizontalScrollbar.margin.top;
				flag2 = true;
			}
			if (flag || viewRect.height > screenRect.height)
			{
				scrollViewState.visibleRect.width = position.width - verticalScrollbar.fixedWidth + (float)verticalScrollbar.margin.left;
				screenRect.width -= verticalScrollbar.fixedWidth + (float)verticalScrollbar.margin.left;
				flag = true;
				if (!flag2 && viewRect.width > screenRect.width)
				{
					scrollViewState.visibleRect.height = position.height - horizontalScrollbar.fixedHeight + (float)horizontalScrollbar.margin.top;
					screenRect.height -= horizontalScrollbar.fixedHeight + (float)horizontalScrollbar.margin.top;
					flag2 = true;
				}
			}
			if (Event.current.type == EventType.Repaint && background != GUIStyle.none)
			{
				background.Draw(position, position.Contains(Event.current.mousePosition), isActive: false, flag2 && flag, hasKeyboardFocus: false);
			}
			if (flag2 && horizontalScrollbar != GUIStyle.none)
			{
				scrollPosition.x = HorizontalScrollbar(new Rect(position.x, position.yMax - horizontalScrollbar.fixedHeight, screenRect.width, horizontalScrollbar.fixedHeight), scrollPosition.x, Mathf.Min(screenRect.width, viewRect.width), 0f, viewRect.width, horizontalScrollbar);
			}
			else
			{
				GUIUtility.GetControlID(s_SliderHash, FocusType.Passive);
				GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
				GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
				if (horizontalScrollbar != GUIStyle.none)
				{
					scrollPosition.x = 0f;
				}
				else
				{
					scrollPosition.x = Mathf.Clamp(scrollPosition.x, 0f, Mathf.Max(viewRect.width - position.width, 0f));
				}
			}
			if (flag && verticalScrollbar != GUIStyle.none)
			{
				scrollPosition.y = VerticalScrollbar(new Rect(screenRect.xMax + (float)verticalScrollbar.margin.left, screenRect.y, verticalScrollbar.fixedWidth, screenRect.height), scrollPosition.y, Mathf.Min(screenRect.height, viewRect.height), 0f, viewRect.height, verticalScrollbar);
				break;
			}
			GUIUtility.GetControlID(s_SliderHash, FocusType.Passive);
			GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
			GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
			if (verticalScrollbar != GUIStyle.none)
			{
				scrollPosition.y = 0f;
			}
			else
			{
				scrollPosition.y = Mathf.Clamp(scrollPosition.y, 0f, Mathf.Max(viewRect.height - position.height, 0f));
			}
			break;
		}
		case EventType.Used:
			break;
		}
		GUIClip.Push(screenRect, new Vector2(Mathf.Round(0f - scrollPosition.x - viewRect.x), Mathf.Round(0f - scrollPosition.y - viewRect.y)), Vector2.zero, resetOffset: false);
		return scrollPosition;
	}

	/// <summary>
	///   <para>Ends a scrollview started with a call to BeginScrollView.</para>
	/// </summary>
	/// <param name="handleScrollWheel"></param>
	public static void EndScrollView()
	{
		EndScrollView(handleScrollWheel: true);
	}

	/// <summary>
	///   <para>Ends a scrollview started with a call to BeginScrollView.</para>
	/// </summary>
	/// <param name="handleScrollWheel"></param>
	public static void EndScrollView(bool handleScrollWheel)
	{
		GUIUtility.CheckOnGUI();
		ScrollViewState scrollViewState = (ScrollViewState)s_ScrollViewStates.Peek();
		GUIClip.Pop();
		s_ScrollViewStates.Pop();
		if (handleScrollWheel && Event.current.type == EventType.ScrollWheel && scrollViewState.position.Contains(Event.current.mousePosition))
		{
			scrollViewState.scrollPosition.x = Mathf.Clamp(scrollViewState.scrollPosition.x + Event.current.delta.x * 20f, 0f, scrollViewState.viewRect.width - scrollViewState.visibleRect.width);
			scrollViewState.scrollPosition.y = Mathf.Clamp(scrollViewState.scrollPosition.y + Event.current.delta.y * 20f, 0f, scrollViewState.viewRect.height - scrollViewState.visibleRect.height);
			if (scrollViewState.scrollPosition.x < 0f)
			{
				scrollViewState.scrollPosition.x = 0f;
			}
			if (scrollViewState.scrollPosition.y < 0f)
			{
				scrollViewState.scrollPosition.y = 0f;
			}
			scrollViewState.apply = true;
			Event.current.Use();
		}
	}

	internal static ScrollViewState GetTopScrollView()
	{
		if (s_ScrollViewStates.Count != 0)
		{
			return (ScrollViewState)s_ScrollViewStates.Peek();
		}
		return null;
	}

	/// <summary>
	///   <para>Scrolls all enclosing scrollviews so they try to make position visible.</para>
	/// </summary>
	/// <param name="position"></param>
	public static void ScrollTo(Rect position)
	{
		GetTopScrollView()?.ScrollTo(position);
	}

	public static bool ScrollTowards(Rect position, float maxDelta)
	{
		return GetTopScrollView()?.ScrollTowards(position, maxDelta) ?? false;
	}

	public static Rect Window(int id, Rect clientRect, WindowFunction func, string text)
	{
		GUIUtility.CheckOnGUI();
		return DoWindow(id, clientRect, func, GUIContent.Temp(text), skin.window, skin, forceRectOnLayout: true);
	}

	public static Rect Window(int id, Rect clientRect, WindowFunction func, Texture image)
	{
		GUIUtility.CheckOnGUI();
		return DoWindow(id, clientRect, func, GUIContent.Temp(image), skin.window, skin, forceRectOnLayout: true);
	}

	public static Rect Window(int id, Rect clientRect, WindowFunction func, GUIContent content)
	{
		GUIUtility.CheckOnGUI();
		return DoWindow(id, clientRect, func, content, skin.window, skin, forceRectOnLayout: true);
	}

	public static Rect Window(int id, Rect clientRect, WindowFunction func, string text, GUIStyle style)
	{
		GUIUtility.CheckOnGUI();
		return DoWindow(id, clientRect, func, GUIContent.Temp(text), style, skin, forceRectOnLayout: true);
	}

	public static Rect Window(int id, Rect clientRect, WindowFunction func, Texture image, GUIStyle style)
	{
		GUIUtility.CheckOnGUI();
		return DoWindow(id, clientRect, func, GUIContent.Temp(image), style, skin, forceRectOnLayout: true);
	}

	public static Rect Window(int id, Rect clientRect, WindowFunction func, GUIContent title, GUIStyle style)
	{
		GUIUtility.CheckOnGUI();
		return DoWindow(id, clientRect, func, title, style, skin, forceRectOnLayout: true);
	}

	public static Rect ModalWindow(int id, Rect clientRect, WindowFunction func, string text)
	{
		GUIUtility.CheckOnGUI();
		return DoModalWindow(id, clientRect, func, GUIContent.Temp(text), skin.window, skin);
	}

	public static Rect ModalWindow(int id, Rect clientRect, WindowFunction func, Texture image)
	{
		GUIUtility.CheckOnGUI();
		return DoModalWindow(id, clientRect, func, GUIContent.Temp(image), skin.window, skin);
	}

	public static Rect ModalWindow(int id, Rect clientRect, WindowFunction func, GUIContent content)
	{
		GUIUtility.CheckOnGUI();
		return DoModalWindow(id, clientRect, func, content, skin.window, skin);
	}

	public static Rect ModalWindow(int id, Rect clientRect, WindowFunction func, string text, GUIStyle style)
	{
		GUIUtility.CheckOnGUI();
		return DoModalWindow(id, clientRect, func, GUIContent.Temp(text), style, skin);
	}

	public static Rect ModalWindow(int id, Rect clientRect, WindowFunction func, Texture image, GUIStyle style)
	{
		GUIUtility.CheckOnGUI();
		return DoModalWindow(id, clientRect, func, GUIContent.Temp(image), style, skin);
	}

	public static Rect ModalWindow(int id, Rect clientRect, WindowFunction func, GUIContent content, GUIStyle style)
	{
		GUIUtility.CheckOnGUI();
		return DoModalWindow(id, clientRect, func, content, style, skin);
	}

	private static Rect DoWindow(int id, Rect clientRect, WindowFunction func, GUIContent title, GUIStyle style, GUISkin skin, bool forceRectOnLayout)
	{
		return Internal_DoWindow(id, GUIUtility.s_OriginalID, clientRect, func, title, style, skin, forceRectOnLayout);
	}

	private static Rect DoModalWindow(int id, Rect clientRect, WindowFunction func, GUIContent content, GUIStyle style, GUISkin skin)
	{
		return Internal_DoModalWindow(id, GUIUtility.s_OriginalID, clientRect, func, content, style, skin);
	}

	[RequiredByNativeCode]
	internal static void CallWindowDelegate(WindowFunction func, int id, int instanceID, GUISkin _skin, int forceRect, float width, float height, GUIStyle style)
	{
		GUILayoutUtility.SelectIDList(id, isWindow: true);
		GUISkin gUISkin = skin;
		if (Event.current.type == EventType.Layout)
		{
			if (forceRect != 0)
			{
				GUILayoutOption[] options = new GUILayoutOption[2]
				{
					GUILayout.Width(width),
					GUILayout.Height(height)
				};
				GUILayoutUtility.BeginWindow(id, style, options);
			}
			else
			{
				GUILayoutUtility.BeginWindow(id, style, null);
			}
		}
		else
		{
			GUILayoutUtility.BeginWindow(id, GUIStyle.none, null);
		}
		skin = _skin;
		func(id);
		if (Event.current.type == EventType.Layout)
		{
			GUILayoutUtility.Layout();
		}
		skin = gUISkin;
	}

	/// <summary>
	///   <para>If you want to have the entire window background to act as a drag area, use the version of DragWindow that takes no parameters and put it at the end of the window function.</para>
	/// </summary>
	public static void DragWindow()
	{
		DragWindow(new Rect(0f, 0f, 10000f, 10000f));
	}

	internal static void BeginWindows(int skinMode, int editorWindowInstanceID)
	{
		GUILayoutGroup topLevel = GUILayoutUtility.current.topLevel;
		GenericStack layoutGroups = GUILayoutUtility.current.layoutGroups;
		GUILayoutGroup windows = GUILayoutUtility.current.windows;
		Matrix4x4 matrix4x = matrix;
		Internal_BeginWindows();
		matrix = matrix4x;
		GUILayoutUtility.current.topLevel = topLevel;
		GUILayoutUtility.current.layoutGroups = layoutGroups;
		GUILayoutUtility.current.windows = windows;
	}

	internal static void EndWindows()
	{
		GUILayoutGroup topLevel = GUILayoutUtility.current.topLevel;
		GenericStack layoutGroups = GUILayoutUtility.current.layoutGroups;
		GUILayoutGroup windows = GUILayoutUtility.current.windows;
		Internal_EndWindows();
		GUILayoutUtility.current.topLevel = topLevel;
		GUILayoutUtility.current.layoutGroups = layoutGroups;
		GUILayoutUtility.current.windows = windows;
	}
}

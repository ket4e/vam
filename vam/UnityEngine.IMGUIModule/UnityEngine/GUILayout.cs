namespace UnityEngine;

/// <summary>
///   <para>The GUILayout class is the interface for Unity gui with automatic layout.</para>
/// </summary>
public class GUILayout
{
	private sealed class LayoutedWindow
	{
		private readonly GUI.WindowFunction m_Func;

		private readonly Rect m_ScreenRect;

		private readonly GUILayoutOption[] m_Options;

		private readonly GUIStyle m_Style;

		internal LayoutedWindow(GUI.WindowFunction f, Rect screenRect, GUIContent content, GUILayoutOption[] options, GUIStyle style)
		{
			m_Func = f;
			m_ScreenRect = screenRect;
			m_Options = options;
			m_Style = style;
		}

		public void DoWindow(int windowID)
		{
			GUILayoutGroup topLevel = GUILayoutUtility.current.topLevel;
			EventType type = Event.current.type;
			if (type == EventType.Layout)
			{
				topLevel.resetCoords = true;
				topLevel.rect = m_ScreenRect;
				if (m_Options != null)
				{
					topLevel.ApplyOptions(m_Options);
				}
				topLevel.isWindow = true;
				topLevel.windowID = windowID;
				topLevel.style = m_Style;
			}
			else
			{
				topLevel.ResetCursor();
			}
			m_Func(windowID);
		}
	}

	/// <summary>
	///   <para>Disposable helper class for managing BeginHorizontal / EndHorizontal.</para>
	/// </summary>
	public class HorizontalScope : GUI.Scope
	{
		/// <summary>
		///   <para>Create a new HorizontalScope and begin the corresponding horizontal group.</para>
		/// </summary>
		/// <param name="text">Text to display on group.</param>
		/// <param name="image">Texture to display on group.</param>
		/// <param name="content">Text, image, and tooltip for this group.</param>
		/// <param name="style">The style to use for background image and padding values. If left out, the background is transparent.</param>
		/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
		/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
		/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
		public HorizontalScope(params GUILayoutOption[] options)
		{
			BeginHorizontal(options);
		}

		/// <summary>
		///   <para>Create a new HorizontalScope and begin the corresponding horizontal group.</para>
		/// </summary>
		/// <param name="text">Text to display on group.</param>
		/// <param name="image">Texture to display on group.</param>
		/// <param name="content">Text, image, and tooltip for this group.</param>
		/// <param name="style">The style to use for background image and padding values. If left out, the background is transparent.</param>
		/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
		/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
		/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
		public HorizontalScope(GUIStyle style, params GUILayoutOption[] options)
		{
			BeginHorizontal(style, options);
		}

		/// <summary>
		///   <para>Create a new HorizontalScope and begin the corresponding horizontal group.</para>
		/// </summary>
		/// <param name="text">Text to display on group.</param>
		/// <param name="image">Texture to display on group.</param>
		/// <param name="content">Text, image, and tooltip for this group.</param>
		/// <param name="style">The style to use for background image and padding values. If left out, the background is transparent.</param>
		/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
		/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
		/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
		public HorizontalScope(string text, GUIStyle style, params GUILayoutOption[] options)
		{
			BeginHorizontal(text, style, options);
		}

		/// <summary>
		///   <para>Create a new HorizontalScope and begin the corresponding horizontal group.</para>
		/// </summary>
		/// <param name="text">Text to display on group.</param>
		/// <param name="image">Texture to display on group.</param>
		/// <param name="content">Text, image, and tooltip for this group.</param>
		/// <param name="style">The style to use for background image and padding values. If left out, the background is transparent.</param>
		/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
		/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
		/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
		public HorizontalScope(Texture image, GUIStyle style, params GUILayoutOption[] options)
		{
			BeginHorizontal(image, style, options);
		}

		/// <summary>
		///   <para>Create a new HorizontalScope and begin the corresponding horizontal group.</para>
		/// </summary>
		/// <param name="text">Text to display on group.</param>
		/// <param name="image">Texture to display on group.</param>
		/// <param name="content">Text, image, and tooltip for this group.</param>
		/// <param name="style">The style to use for background image and padding values. If left out, the background is transparent.</param>
		/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
		/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
		/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
		public HorizontalScope(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
		{
			BeginHorizontal(content, style, options);
		}

		protected override void CloseScope()
		{
			EndHorizontal();
		}
	}

	/// <summary>
	///   <para>Disposable helper class for managing BeginVertical / EndVertical.</para>
	/// </summary>
	public class VerticalScope : GUI.Scope
	{
		/// <summary>
		///   <para>Create a new VerticalScope and begin the corresponding vertical group.</para>
		/// </summary>
		/// <param name="text">Text to display on group.</param>
		/// <param name="image">Texture to display on group.</param>
		/// <param name="content">Text, image, and tooltip for this group.</param>
		/// <param name="style">The style to use for background image and padding values. If left out, the background is transparent.</param>
		/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
		/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
		/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
		public VerticalScope(params GUILayoutOption[] options)
		{
			BeginVertical(options);
		}

		/// <summary>
		///   <para>Create a new VerticalScope and begin the corresponding vertical group.</para>
		/// </summary>
		/// <param name="text">Text to display on group.</param>
		/// <param name="image">Texture to display on group.</param>
		/// <param name="content">Text, image, and tooltip for this group.</param>
		/// <param name="style">The style to use for background image and padding values. If left out, the background is transparent.</param>
		/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
		/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
		/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
		public VerticalScope(GUIStyle style, params GUILayoutOption[] options)
		{
			BeginVertical(style, options);
		}

		/// <summary>
		///   <para>Create a new VerticalScope and begin the corresponding vertical group.</para>
		/// </summary>
		/// <param name="text">Text to display on group.</param>
		/// <param name="image">Texture to display on group.</param>
		/// <param name="content">Text, image, and tooltip for this group.</param>
		/// <param name="style">The style to use for background image and padding values. If left out, the background is transparent.</param>
		/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
		/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
		/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
		public VerticalScope(string text, GUIStyle style, params GUILayoutOption[] options)
		{
			BeginVertical(text, style, options);
		}

		/// <summary>
		///   <para>Create a new VerticalScope and begin the corresponding vertical group.</para>
		/// </summary>
		/// <param name="text">Text to display on group.</param>
		/// <param name="image">Texture to display on group.</param>
		/// <param name="content">Text, image, and tooltip for this group.</param>
		/// <param name="style">The style to use for background image and padding values. If left out, the background is transparent.</param>
		/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
		/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
		/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
		public VerticalScope(Texture image, GUIStyle style, params GUILayoutOption[] options)
		{
			BeginVertical(image, style, options);
		}

		/// <summary>
		///   <para>Create a new VerticalScope and begin the corresponding vertical group.</para>
		/// </summary>
		/// <param name="text">Text to display on group.</param>
		/// <param name="image">Texture to display on group.</param>
		/// <param name="content">Text, image, and tooltip for this group.</param>
		/// <param name="style">The style to use for background image and padding values. If left out, the background is transparent.</param>
		/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
		/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
		/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
		public VerticalScope(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
		{
			BeginVertical(content, style, options);
		}

		protected override void CloseScope()
		{
			EndVertical();
		}
	}

	/// <summary>
	///   <para>Disposable helper class for managing BeginArea / EndArea.</para>
	/// </summary>
	public class AreaScope : GUI.Scope
	{
		/// <summary>
		///   <para>Create a new AreaScope and begin the corresponding Area.</para>
		/// </summary>
		/// <param name="text">Optional text to display in the area.</param>
		/// <param name="image">Optional texture to display in the area.</param>
		/// <param name="content">Optional text, image and tooltip top display for this area.</param>
		/// <param name="style">The style to use. If left out, the empty GUIStyle (GUIStyle.none) is used, giving a transparent background.</param>
		/// <param name="screenRect"></param>
		public AreaScope(Rect screenRect)
		{
			BeginArea(screenRect);
		}

		/// <summary>
		///   <para>Create a new AreaScope and begin the corresponding Area.</para>
		/// </summary>
		/// <param name="text">Optional text to display in the area.</param>
		/// <param name="image">Optional texture to display in the area.</param>
		/// <param name="content">Optional text, image and tooltip top display for this area.</param>
		/// <param name="style">The style to use. If left out, the empty GUIStyle (GUIStyle.none) is used, giving a transparent background.</param>
		/// <param name="screenRect"></param>
		public AreaScope(Rect screenRect, string text)
		{
			BeginArea(screenRect, text);
		}

		/// <summary>
		///   <para>Create a new AreaScope and begin the corresponding Area.</para>
		/// </summary>
		/// <param name="text">Optional text to display in the area.</param>
		/// <param name="image">Optional texture to display in the area.</param>
		/// <param name="content">Optional text, image and tooltip top display for this area.</param>
		/// <param name="style">The style to use. If left out, the empty GUIStyle (GUIStyle.none) is used, giving a transparent background.</param>
		/// <param name="screenRect"></param>
		public AreaScope(Rect screenRect, Texture image)
		{
			BeginArea(screenRect, image);
		}

		/// <summary>
		///   <para>Create a new AreaScope and begin the corresponding Area.</para>
		/// </summary>
		/// <param name="text">Optional text to display in the area.</param>
		/// <param name="image">Optional texture to display in the area.</param>
		/// <param name="content">Optional text, image and tooltip top display for this area.</param>
		/// <param name="style">The style to use. If left out, the empty GUIStyle (GUIStyle.none) is used, giving a transparent background.</param>
		/// <param name="screenRect"></param>
		public AreaScope(Rect screenRect, GUIContent content)
		{
			BeginArea(screenRect, content);
		}

		/// <summary>
		///   <para>Create a new AreaScope and begin the corresponding Area.</para>
		/// </summary>
		/// <param name="text">Optional text to display in the area.</param>
		/// <param name="image">Optional texture to display in the area.</param>
		/// <param name="content">Optional text, image and tooltip top display for this area.</param>
		/// <param name="style">The style to use. If left out, the empty GUIStyle (GUIStyle.none) is used, giving a transparent background.</param>
		/// <param name="screenRect"></param>
		public AreaScope(Rect screenRect, string text, GUIStyle style)
		{
			BeginArea(screenRect, text, style);
		}

		/// <summary>
		///   <para>Create a new AreaScope and begin the corresponding Area.</para>
		/// </summary>
		/// <param name="text">Optional text to display in the area.</param>
		/// <param name="image">Optional texture to display in the area.</param>
		/// <param name="content">Optional text, image and tooltip top display for this area.</param>
		/// <param name="style">The style to use. If left out, the empty GUIStyle (GUIStyle.none) is used, giving a transparent background.</param>
		/// <param name="screenRect"></param>
		public AreaScope(Rect screenRect, Texture image, GUIStyle style)
		{
			BeginArea(screenRect, image, style);
		}

		/// <summary>
		///   <para>Create a new AreaScope and begin the corresponding Area.</para>
		/// </summary>
		/// <param name="text">Optional text to display in the area.</param>
		/// <param name="image">Optional texture to display in the area.</param>
		/// <param name="content">Optional text, image and tooltip top display for this area.</param>
		/// <param name="style">The style to use. If left out, the empty GUIStyle (GUIStyle.none) is used, giving a transparent background.</param>
		/// <param name="screenRect"></param>
		public AreaScope(Rect screenRect, GUIContent content, GUIStyle style)
		{
			BeginArea(screenRect, content, style);
		}

		protected override void CloseScope()
		{
			EndArea();
		}
	}

	/// <summary>
	///   <para>Disposable helper class for managing BeginScrollView / EndScrollView.</para>
	/// </summary>
	public class ScrollViewScope : GUI.Scope
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
		/// <param name="scrollPosition">The position to use display.</param>
		/// <param name="alwaysShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when the content inside the ScrollView is wider than the scrollview itself.</param>
		/// <param name="alwaysShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when content inside the ScrollView is taller than the scrollview itself.</param>
		/// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
		/// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
		/// <param name="options"></param>
		/// <param name="style"></param>
		/// <param name="background"></param>
		public ScrollViewScope(Vector2 scrollPosition, params GUILayoutOption[] options)
		{
			handleScrollWheel = true;
			this.scrollPosition = BeginScrollView(scrollPosition, options);
		}

		/// <summary>
		///   <para>Create a new ScrollViewScope and begin the corresponding ScrollView.</para>
		/// </summary>
		/// <param name="scrollPosition">The position to use display.</param>
		/// <param name="alwaysShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when the content inside the ScrollView is wider than the scrollview itself.</param>
		/// <param name="alwaysShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when content inside the ScrollView is taller than the scrollview itself.</param>
		/// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
		/// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
		/// <param name="options"></param>
		/// <param name="style"></param>
		/// <param name="background"></param>
		public ScrollViewScope(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, params GUILayoutOption[] options)
		{
			handleScrollWheel = true;
			this.scrollPosition = BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, options);
		}

		/// <summary>
		///   <para>Create a new ScrollViewScope and begin the corresponding ScrollView.</para>
		/// </summary>
		/// <param name="scrollPosition">The position to use display.</param>
		/// <param name="alwaysShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when the content inside the ScrollView is wider than the scrollview itself.</param>
		/// <param name="alwaysShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when content inside the ScrollView is taller than the scrollview itself.</param>
		/// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
		/// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
		/// <param name="options"></param>
		/// <param name="style"></param>
		/// <param name="background"></param>
		public ScrollViewScope(Vector2 scrollPosition, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, params GUILayoutOption[] options)
		{
			handleScrollWheel = true;
			this.scrollPosition = BeginScrollView(scrollPosition, horizontalScrollbar, verticalScrollbar, options);
		}

		/// <summary>
		///   <para>Create a new ScrollViewScope and begin the corresponding ScrollView.</para>
		/// </summary>
		/// <param name="scrollPosition">The position to use display.</param>
		/// <param name="alwaysShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when the content inside the ScrollView is wider than the scrollview itself.</param>
		/// <param name="alwaysShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when content inside the ScrollView is taller than the scrollview itself.</param>
		/// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
		/// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
		/// <param name="options"></param>
		/// <param name="style"></param>
		/// <param name="background"></param>
		public ScrollViewScope(Vector2 scrollPosition, GUIStyle style, params GUILayoutOption[] options)
		{
			handleScrollWheel = true;
			this.scrollPosition = BeginScrollView(scrollPosition, style, options);
		}

		/// <summary>
		///   <para>Create a new ScrollViewScope and begin the corresponding ScrollView.</para>
		/// </summary>
		/// <param name="scrollPosition">The position to use display.</param>
		/// <param name="alwaysShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when the content inside the ScrollView is wider than the scrollview itself.</param>
		/// <param name="alwaysShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when content inside the ScrollView is taller than the scrollview itself.</param>
		/// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
		/// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
		/// <param name="options"></param>
		/// <param name="style"></param>
		/// <param name="background"></param>
		public ScrollViewScope(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, params GUILayoutOption[] options)
		{
			handleScrollWheel = true;
			this.scrollPosition = BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, options);
		}

		/// <summary>
		///   <para>Create a new ScrollViewScope and begin the corresponding ScrollView.</para>
		/// </summary>
		/// <param name="scrollPosition">The position to use display.</param>
		/// <param name="alwaysShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when the content inside the ScrollView is wider than the scrollview itself.</param>
		/// <param name="alwaysShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when content inside the ScrollView is taller than the scrollview itself.</param>
		/// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
		/// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
		/// <param name="options"></param>
		/// <param name="style"></param>
		/// <param name="background"></param>
		public ScrollViewScope(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background, params GUILayoutOption[] options)
		{
			handleScrollWheel = true;
			this.scrollPosition = BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, background, options);
		}

		protected override void CloseScope()
		{
			EndScrollView(handleScrollWheel);
		}
	}

	/// <summary>
	///   <para>Make an auto-layout label.</para>
	/// </summary>
	/// <param name="text">Text to display on the label.</param>
	/// <param name="image">Texture to display on the label.</param>
	/// <param name="content">Text, image and tooltip for this label.</param>
	/// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	public static void Label(Texture image, params GUILayoutOption[] options)
	{
		DoLabel(GUIContent.Temp(image), GUI.skin.label, options);
	}

	/// <summary>
	///   <para>Make an auto-layout label.</para>
	/// </summary>
	/// <param name="text">Text to display on the label.</param>
	/// <param name="image">Texture to display on the label.</param>
	/// <param name="content">Text, image and tooltip for this label.</param>
	/// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	public static void Label(string text, params GUILayoutOption[] options)
	{
		DoLabel(GUIContent.Temp(text), GUI.skin.label, options);
	}

	/// <summary>
	///   <para>Make an auto-layout label.</para>
	/// </summary>
	/// <param name="text">Text to display on the label.</param>
	/// <param name="image">Texture to display on the label.</param>
	/// <param name="content">Text, image and tooltip for this label.</param>
	/// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	public static void Label(GUIContent content, params GUILayoutOption[] options)
	{
		DoLabel(content, GUI.skin.label, options);
	}

	/// <summary>
	///   <para>Make an auto-layout label.</para>
	/// </summary>
	/// <param name="text">Text to display on the label.</param>
	/// <param name="image">Texture to display on the label.</param>
	/// <param name="content">Text, image and tooltip for this label.</param>
	/// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	public static void Label(Texture image, GUIStyle style, params GUILayoutOption[] options)
	{
		DoLabel(GUIContent.Temp(image), style, options);
	}

	/// <summary>
	///   <para>Make an auto-layout label.</para>
	/// </summary>
	/// <param name="text">Text to display on the label.</param>
	/// <param name="image">Texture to display on the label.</param>
	/// <param name="content">Text, image and tooltip for this label.</param>
	/// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	public static void Label(string text, GUIStyle style, params GUILayoutOption[] options)
	{
		DoLabel(GUIContent.Temp(text), style, options);
	}

	/// <summary>
	///   <para>Make an auto-layout label.</para>
	/// </summary>
	/// <param name="text">Text to display on the label.</param>
	/// <param name="image">Texture to display on the label.</param>
	/// <param name="content">Text, image and tooltip for this label.</param>
	/// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	public static void Label(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
	{
		DoLabel(content, style, options);
	}

	private static void DoLabel(GUIContent content, GUIStyle style, GUILayoutOption[] options)
	{
		GUI.Label(GUILayoutUtility.GetRect(content, style, options), content, style);
	}

	/// <summary>
	///   <para>Make an auto-layout box.</para>
	/// </summary>
	/// <param name="text">Text to display on the box.</param>
	/// <param name="image">Texture to display on the box.</param>
	/// <param name="content">Text, image and tooltip for this box.</param>
	/// <param name="style">The style to use. If left out, the box style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	public static void Box(Texture image, params GUILayoutOption[] options)
	{
		DoBox(GUIContent.Temp(image), GUI.skin.box, options);
	}

	/// <summary>
	///   <para>Make an auto-layout box.</para>
	/// </summary>
	/// <param name="text">Text to display on the box.</param>
	/// <param name="image">Texture to display on the box.</param>
	/// <param name="content">Text, image and tooltip for this box.</param>
	/// <param name="style">The style to use. If left out, the box style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	public static void Box(string text, params GUILayoutOption[] options)
	{
		DoBox(GUIContent.Temp(text), GUI.skin.box, options);
	}

	/// <summary>
	///   <para>Make an auto-layout box.</para>
	/// </summary>
	/// <param name="text">Text to display on the box.</param>
	/// <param name="image">Texture to display on the box.</param>
	/// <param name="content">Text, image and tooltip for this box.</param>
	/// <param name="style">The style to use. If left out, the box style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	public static void Box(GUIContent content, params GUILayoutOption[] options)
	{
		DoBox(content, GUI.skin.box, options);
	}

	/// <summary>
	///   <para>Make an auto-layout box.</para>
	/// </summary>
	/// <param name="text">Text to display on the box.</param>
	/// <param name="image">Texture to display on the box.</param>
	/// <param name="content">Text, image and tooltip for this box.</param>
	/// <param name="style">The style to use. If left out, the box style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	public static void Box(Texture image, GUIStyle style, params GUILayoutOption[] options)
	{
		DoBox(GUIContent.Temp(image), style, options);
	}

	/// <summary>
	///   <para>Make an auto-layout box.</para>
	/// </summary>
	/// <param name="text">Text to display on the box.</param>
	/// <param name="image">Texture to display on the box.</param>
	/// <param name="content">Text, image and tooltip for this box.</param>
	/// <param name="style">The style to use. If left out, the box style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	public static void Box(string text, GUIStyle style, params GUILayoutOption[] options)
	{
		DoBox(GUIContent.Temp(text), style, options);
	}

	/// <summary>
	///   <para>Make an auto-layout box.</para>
	/// </summary>
	/// <param name="text">Text to display on the box.</param>
	/// <param name="image">Texture to display on the box.</param>
	/// <param name="content">Text, image and tooltip for this box.</param>
	/// <param name="style">The style to use. If left out, the box style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	public static void Box(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
	{
		DoBox(content, style, options);
	}

	private static void DoBox(GUIContent content, GUIStyle style, GUILayoutOption[] options)
	{
		GUI.Box(GUILayoutUtility.GetRect(content, style, options), content, style);
	}

	/// <summary>
	///   <para>Make a single press button.</para>
	/// </summary>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>true when the users clicks the button.</para>
	/// </returns>
	public static bool Button(Texture image, params GUILayoutOption[] options)
	{
		return DoButton(GUIContent.Temp(image), GUI.skin.button, options);
	}

	/// <summary>
	///   <para>Make a single press button.</para>
	/// </summary>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>true when the users clicks the button.</para>
	/// </returns>
	public static bool Button(string text, params GUILayoutOption[] options)
	{
		return DoButton(GUIContent.Temp(text), GUI.skin.button, options);
	}

	/// <summary>
	///   <para>Make a single press button.</para>
	/// </summary>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>true when the users clicks the button.</para>
	/// </returns>
	public static bool Button(GUIContent content, params GUILayoutOption[] options)
	{
		return DoButton(content, GUI.skin.button, options);
	}

	/// <summary>
	///   <para>Make a single press button.</para>
	/// </summary>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>true when the users clicks the button.</para>
	/// </returns>
	public static bool Button(Texture image, GUIStyle style, params GUILayoutOption[] options)
	{
		return DoButton(GUIContent.Temp(image), style, options);
	}

	/// <summary>
	///   <para>Make a single press button.</para>
	/// </summary>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>true when the users clicks the button.</para>
	/// </returns>
	public static bool Button(string text, GUIStyle style, params GUILayoutOption[] options)
	{
		return DoButton(GUIContent.Temp(text), style, options);
	}

	/// <summary>
	///   <para>Make a single press button.</para>
	/// </summary>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>true when the users clicks the button.</para>
	/// </returns>
	public static bool Button(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
	{
		return DoButton(content, style, options);
	}

	private static bool DoButton(GUIContent content, GUIStyle style, GUILayoutOption[] options)
	{
		return GUI.Button(GUILayoutUtility.GetRect(content, style, options), content, style);
	}

	/// <summary>
	///   <para>Make a repeating button. The button returns true as long as the user holds down the mouse.</para>
	/// </summary>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>true when the holds down the mouse.</para>
	/// </returns>
	public static bool RepeatButton(Texture image, params GUILayoutOption[] options)
	{
		return DoRepeatButton(GUIContent.Temp(image), GUI.skin.button, options);
	}

	/// <summary>
	///   <para>Make a repeating button. The button returns true as long as the user holds down the mouse.</para>
	/// </summary>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>true when the holds down the mouse.</para>
	/// </returns>
	public static bool RepeatButton(string text, params GUILayoutOption[] options)
	{
		return DoRepeatButton(GUIContent.Temp(text), GUI.skin.button, options);
	}

	/// <summary>
	///   <para>Make a repeating button. The button returns true as long as the user holds down the mouse.</para>
	/// </summary>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>true when the holds down the mouse.</para>
	/// </returns>
	public static bool RepeatButton(GUIContent content, params GUILayoutOption[] options)
	{
		return DoRepeatButton(content, GUI.skin.button, options);
	}

	/// <summary>
	///   <para>Make a repeating button. The button returns true as long as the user holds down the mouse.</para>
	/// </summary>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>true when the holds down the mouse.</para>
	/// </returns>
	public static bool RepeatButton(Texture image, GUIStyle style, params GUILayoutOption[] options)
	{
		return DoRepeatButton(GUIContent.Temp(image), style, options);
	}

	/// <summary>
	///   <para>Make a repeating button. The button returns true as long as the user holds down the mouse.</para>
	/// </summary>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>true when the holds down the mouse.</para>
	/// </returns>
	public static bool RepeatButton(string text, GUIStyle style, params GUILayoutOption[] options)
	{
		return DoRepeatButton(GUIContent.Temp(text), style, options);
	}

	/// <summary>
	///   <para>Make a repeating button. The button returns true as long as the user holds down the mouse.</para>
	/// </summary>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>true when the holds down the mouse.</para>
	/// </returns>
	public static bool RepeatButton(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
	{
		return DoRepeatButton(content, style, options);
	}

	private static bool DoRepeatButton(GUIContent content, GUIStyle style, GUILayoutOption[] options)
	{
		return GUI.RepeatButton(GUILayoutUtility.GetRect(content, style, options), content, style);
	}

	/// <summary>
	///   <para>Make a single-line text field where the user can edit a string.</para>
	/// </summary>
	/// <param name="text">Text to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
	/// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
	/// <param name="style">The style to use. If left out, the textArea style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>The edited string.</para>
	/// </returns>
	public static string TextField(string text, params GUILayoutOption[] options)
	{
		return DoTextField(text, -1, multiline: false, GUI.skin.textField, options);
	}

	/// <summary>
	///   <para>Make a single-line text field where the user can edit a string.</para>
	/// </summary>
	/// <param name="text">Text to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
	/// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
	/// <param name="style">The style to use. If left out, the textArea style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>The edited string.</para>
	/// </returns>
	public static string TextField(string text, int maxLength, params GUILayoutOption[] options)
	{
		return DoTextField(text, maxLength, multiline: false, GUI.skin.textField, options);
	}

	/// <summary>
	///   <para>Make a single-line text field where the user can edit a string.</para>
	/// </summary>
	/// <param name="text">Text to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
	/// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
	/// <param name="style">The style to use. If left out, the textArea style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>The edited string.</para>
	/// </returns>
	public static string TextField(string text, GUIStyle style, params GUILayoutOption[] options)
	{
		return DoTextField(text, -1, multiline: false, style, options);
	}

	/// <summary>
	///   <para>Make a single-line text field where the user can edit a string.</para>
	/// </summary>
	/// <param name="text">Text to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
	/// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
	/// <param name="style">The style to use. If left out, the textArea style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>The edited string.</para>
	/// </returns>
	public static string TextField(string text, int maxLength, GUIStyle style, params GUILayoutOption[] options)
	{
		return DoTextField(text, maxLength, multiline: false, style, options);
	}

	/// <summary>
	///   <para>Make a text field where the user can enter a password.</para>
	/// </summary>
	/// <param name="password">Password to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
	/// <param name="maskChar">Character to mask the password with.</param>
	/// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
	/// <param name="style">The style to use. If left out, the textField style from the current GUISkin is used.</param>
	/// <param name="options"></param>
	/// <returns>
	///   <para>The edited password.</para>
	/// </returns>
	public static string PasswordField(string password, char maskChar, params GUILayoutOption[] options)
	{
		return PasswordField(password, maskChar, -1, GUI.skin.textField, options);
	}

	/// <summary>
	///   <para>Make a text field where the user can enter a password.</para>
	/// </summary>
	/// <param name="password">Password to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
	/// <param name="maskChar">Character to mask the password with.</param>
	/// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
	/// <param name="style">The style to use. If left out, the textField style from the current GUISkin is used.</param>
	/// <param name="options"></param>
	/// <returns>
	///   <para>The edited password.</para>
	/// </returns>
	public static string PasswordField(string password, char maskChar, int maxLength, params GUILayoutOption[] options)
	{
		return PasswordField(password, maskChar, maxLength, GUI.skin.textField, options);
	}

	/// <summary>
	///   <para>Make a text field where the user can enter a password.</para>
	/// </summary>
	/// <param name="password">Password to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
	/// <param name="maskChar">Character to mask the password with.</param>
	/// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
	/// <param name="style">The style to use. If left out, the textField style from the current GUISkin is used.</param>
	/// <param name="options"></param>
	/// <returns>
	///   <para>The edited password.</para>
	/// </returns>
	public static string PasswordField(string password, char maskChar, GUIStyle style, params GUILayoutOption[] options)
	{
		return PasswordField(password, maskChar, -1, style, options);
	}

	/// <summary>
	///   <para>Make a text field where the user can enter a password.</para>
	/// </summary>
	/// <param name="password">Password to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
	/// <param name="maskChar">Character to mask the password with.</param>
	/// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
	/// <param name="style">The style to use. If left out, the textField style from the current GUISkin is used.</param>
	/// <param name="options"></param>
	/// <returns>
	///   <para>The edited password.</para>
	/// </returns>
	public static string PasswordField(string password, char maskChar, int maxLength, GUIStyle style, params GUILayoutOption[] options)
	{
		GUIContent content = GUIContent.Temp(GUI.PasswordFieldGetStrToShow(password, maskChar));
		return GUI.PasswordField(GUILayoutUtility.GetRect(content, GUI.skin.textField, options), password, maskChar, maxLength, style);
	}

	/// <summary>
	///   <para>Make a multi-line text field where the user can edit a string.</para>
	/// </summary>
	/// <param name="text">Text to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
	/// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
	/// <param name="style">The style to use. If left out, the textField style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&amp;amp;lt;br&amp;amp;gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>The edited string.</para>
	/// </returns>
	public static string TextArea(string text, params GUILayoutOption[] options)
	{
		return DoTextField(text, -1, multiline: true, GUI.skin.textArea, options);
	}

	/// <summary>
	///   <para>Make a multi-line text field where the user can edit a string.</para>
	/// </summary>
	/// <param name="text">Text to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
	/// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
	/// <param name="style">The style to use. If left out, the textField style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&amp;amp;lt;br&amp;amp;gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>The edited string.</para>
	/// </returns>
	public static string TextArea(string text, int maxLength, params GUILayoutOption[] options)
	{
		return DoTextField(text, maxLength, multiline: true, GUI.skin.textArea, options);
	}

	/// <summary>
	///   <para>Make a multi-line text field where the user can edit a string.</para>
	/// </summary>
	/// <param name="text">Text to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
	/// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
	/// <param name="style">The style to use. If left out, the textField style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&amp;amp;lt;br&amp;amp;gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>The edited string.</para>
	/// </returns>
	public static string TextArea(string text, GUIStyle style, params GUILayoutOption[] options)
	{
		return DoTextField(text, -1, multiline: true, style, options);
	}

	/// <summary>
	///   <para>Make a multi-line text field where the user can edit a string.</para>
	/// </summary>
	/// <param name="text">Text to edit. The return value of this function should be assigned back to the string as shown in the example.</param>
	/// <param name="maxLength">The maximum length of the string. If left out, the user can type for ever and ever.</param>
	/// <param name="style">The style to use. If left out, the textField style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&amp;amp;lt;br&amp;amp;gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>The edited string.</para>
	/// </returns>
	public static string TextArea(string text, int maxLength, GUIStyle style, params GUILayoutOption[] options)
	{
		return DoTextField(text, maxLength, multiline: true, style, options);
	}

	private static string DoTextField(string text, int maxLength, bool multiline, GUIStyle style, GUILayoutOption[] options)
	{
		int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
		GUIContent gUIContent = GUIContent.Temp(text);
		gUIContent = ((GUIUtility.keyboardControl == controlID) ? GUIContent.Temp(text + Input.compositionString) : GUIContent.Temp(text));
		Rect rect = GUILayoutUtility.GetRect(gUIContent, style, options);
		if (GUIUtility.keyboardControl == controlID)
		{
			gUIContent = GUIContent.Temp(text);
		}
		GUI.DoTextField(rect, controlID, gUIContent, multiline, maxLength, style);
		return gUIContent.text;
	}

	/// <summary>
	///   <para>Make an on/off toggle button.</para>
	/// </summary>
	/// <param name="value">Is the button on or off?</param>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>The new value of the button.</para>
	/// </returns>
	public static bool Toggle(bool value, Texture image, params GUILayoutOption[] options)
	{
		return DoToggle(value, GUIContent.Temp(image), GUI.skin.toggle, options);
	}

	/// <summary>
	///   <para>Make an on/off toggle button.</para>
	/// </summary>
	/// <param name="value">Is the button on or off?</param>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>The new value of the button.</para>
	/// </returns>
	public static bool Toggle(bool value, string text, params GUILayoutOption[] options)
	{
		return DoToggle(value, GUIContent.Temp(text), GUI.skin.toggle, options);
	}

	/// <summary>
	///   <para>Make an on/off toggle button.</para>
	/// </summary>
	/// <param name="value">Is the button on or off?</param>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>The new value of the button.</para>
	/// </returns>
	public static bool Toggle(bool value, GUIContent content, params GUILayoutOption[] options)
	{
		return DoToggle(value, content, GUI.skin.toggle, options);
	}

	/// <summary>
	///   <para>Make an on/off toggle button.</para>
	/// </summary>
	/// <param name="value">Is the button on or off?</param>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>The new value of the button.</para>
	/// </returns>
	public static bool Toggle(bool value, Texture image, GUIStyle style, params GUILayoutOption[] options)
	{
		return DoToggle(value, GUIContent.Temp(image), style, options);
	}

	/// <summary>
	///   <para>Make an on/off toggle button.</para>
	/// </summary>
	/// <param name="value">Is the button on or off?</param>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>The new value of the button.</para>
	/// </returns>
	public static bool Toggle(bool value, string text, GUIStyle style, params GUILayoutOption[] options)
	{
		return DoToggle(value, GUIContent.Temp(text), style, options);
	}

	/// <summary>
	///   <para>Make an on/off toggle button.</para>
	/// </summary>
	/// <param name="value">Is the button on or off?</param>
	/// <param name="text">Text to display on the button.</param>
	/// <param name="image">Texture to display on the button.</param>
	/// <param name="content">Text, image and tooltip for this button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>The new value of the button.</para>
	/// </returns>
	public static bool Toggle(bool value, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
	{
		return DoToggle(value, content, style, options);
	}

	private static bool DoToggle(bool value, GUIContent content, GUIStyle style, GUILayoutOption[] options)
	{
		return GUI.Toggle(GUILayoutUtility.GetRect(content, style, options), value, content, style);
	}

	/// <summary>
	///   <para>Make a toolbar.</para>
	/// </summary>
	/// <param name="selected">The index of the selected button.</param>
	/// <param name="texts">An array of strings to show on the buttons.</param>
	/// <param name="images">An array of textures on the buttons.</param>
	/// <param name="contents">An array of text, image and tooltips for the button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <param name="buttonSize">Determines how toolbar button size is calculated.</param>
	/// <returns>
	///   <para>The index of the selected button.</para>
	/// </returns>
	public static int Toolbar(int selected, string[] texts, params GUILayoutOption[] options)
	{
		return Toolbar(selected, GUIContent.Temp(texts), GUI.skin.button, options);
	}

	/// <summary>
	///   <para>Make a toolbar.</para>
	/// </summary>
	/// <param name="selected">The index of the selected button.</param>
	/// <param name="texts">An array of strings to show on the buttons.</param>
	/// <param name="images">An array of textures on the buttons.</param>
	/// <param name="contents">An array of text, image and tooltips for the button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <param name="buttonSize">Determines how toolbar button size is calculated.</param>
	/// <returns>
	///   <para>The index of the selected button.</para>
	/// </returns>
	public static int Toolbar(int selected, Texture[] images, params GUILayoutOption[] options)
	{
		return Toolbar(selected, GUIContent.Temp(images), GUI.skin.button, options);
	}

	/// <summary>
	///   <para>Make a toolbar.</para>
	/// </summary>
	/// <param name="selected">The index of the selected button.</param>
	/// <param name="texts">An array of strings to show on the buttons.</param>
	/// <param name="images">An array of textures on the buttons.</param>
	/// <param name="contents">An array of text, image and tooltips for the button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <param name="buttonSize">Determines how toolbar button size is calculated.</param>
	/// <returns>
	///   <para>The index of the selected button.</para>
	/// </returns>
	public static int Toolbar(int selected, GUIContent[] contents, params GUILayoutOption[] options)
	{
		return Toolbar(selected, contents, GUI.skin.button, options);
	}

	/// <summary>
	///   <para>Make a toolbar.</para>
	/// </summary>
	/// <param name="selected">The index of the selected button.</param>
	/// <param name="texts">An array of strings to show on the buttons.</param>
	/// <param name="images">An array of textures on the buttons.</param>
	/// <param name="contents">An array of text, image and tooltips for the button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <param name="buttonSize">Determines how toolbar button size is calculated.</param>
	/// <returns>
	///   <para>The index of the selected button.</para>
	/// </returns>
	public static int Toolbar(int selected, string[] texts, GUIStyle style, params GUILayoutOption[] options)
	{
		return Toolbar(selected, GUIContent.Temp(texts), style, options);
	}

	/// <summary>
	///   <para>Make a toolbar.</para>
	/// </summary>
	/// <param name="selected">The index of the selected button.</param>
	/// <param name="texts">An array of strings to show on the buttons.</param>
	/// <param name="images">An array of textures on the buttons.</param>
	/// <param name="contents">An array of text, image and tooltips for the button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <param name="buttonSize">Determines how toolbar button size is calculated.</param>
	/// <returns>
	///   <para>The index of the selected button.</para>
	/// </returns>
	public static int Toolbar(int selected, Texture[] images, GUIStyle style, params GUILayoutOption[] options)
	{
		return Toolbar(selected, GUIContent.Temp(images), style, options);
	}

	public static int Toolbar(int selected, string[] texts, GUIStyle style, GUI.ToolbarButtonSize buttonSize, params GUILayoutOption[] options)
	{
		return Toolbar(selected, GUIContent.Temp(texts), style, buttonSize, options);
	}

	public static int Toolbar(int selected, Texture[] images, GUIStyle style, GUI.ToolbarButtonSize buttonSize, params GUILayoutOption[] options)
	{
		return Toolbar(selected, GUIContent.Temp(images), style, buttonSize, options);
	}

	/// <summary>
	///   <para>Make a toolbar.</para>
	/// </summary>
	/// <param name="selected">The index of the selected button.</param>
	/// <param name="texts">An array of strings to show on the buttons.</param>
	/// <param name="images">An array of textures on the buttons.</param>
	/// <param name="contents">An array of text, image and tooltips for the button.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <param name="buttonSize">Determines how toolbar button size is calculated.</param>
	/// <returns>
	///   <para>The index of the selected button.</para>
	/// </returns>
	public static int Toolbar(int selected, GUIContent[] contents, GUIStyle style, params GUILayoutOption[] options)
	{
		return Toolbar(selected, contents, style, GUI.ToolbarButtonSize.Fixed, options);
	}

	public static int Toolbar(int selected, GUIContent[] contents, GUIStyle style, GUI.ToolbarButtonSize buttonSize, params GUILayoutOption[] options)
	{
		GUI.FindStyles(ref style, out var firstStyle, out var midStyle, out var lastStyle, "left", "mid", "right");
		Vector2 vector = default(Vector2);
		int num = contents.Length;
		GUIStyle gUIStyle = ((num <= 1) ? style : firstStyle);
		GUIStyle gUIStyle2 = ((num <= 1) ? style : midStyle);
		GUIStyle gUIStyle3 = ((num <= 1) ? style : lastStyle);
		float num2 = 0f;
		for (int i = 0; i < contents.Length; i++)
		{
			if (i == num - 2)
			{
				gUIStyle2 = gUIStyle3;
			}
			Vector2 vector2 = gUIStyle.CalcSize(contents[i]);
			switch (buttonSize)
			{
			case GUI.ToolbarButtonSize.Fixed:
				if (vector2.x > vector.x)
				{
					vector.x = vector2.x;
				}
				break;
			case GUI.ToolbarButtonSize.FitToContents:
				vector.x += vector2.x;
				break;
			}
			if (vector2.y > vector.y)
			{
				vector.y = vector2.y;
			}
			num2 = ((i != num - 1) ? (num2 + (float)Mathf.Max(gUIStyle.margin.right, gUIStyle2.margin.left)) : (num2 + (float)gUIStyle.margin.right));
			gUIStyle = gUIStyle2;
		}
		switch (buttonSize)
		{
		case GUI.ToolbarButtonSize.Fixed:
			vector.x = vector.x * (float)contents.Length + num2;
			break;
		case GUI.ToolbarButtonSize.FitToContents:
			vector.x += num2;
			break;
		}
		return GUI.Toolbar(GUILayoutUtility.GetRect(vector.x, vector.y, style, options), selected, contents, style, buttonSize);
	}

	/// <summary>
	///   <para>Make a Selection Grid.</para>
	/// </summary>
	/// <param name="selected">The index of the selected button.</param>
	/// <param name="texts">An array of strings to show on the buttons.</param>
	/// <param name="images">An array of textures on the buttons.</param>
	/// <param name="contents">An array of text, image and tooltips for the button.</param>
	/// <param name="xCount">How many elements to fit in the horizontal direction. The elements will be scaled to fit unless the style defines a fixedWidth to use. The height of the control will be determined from the number of elements.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <param name="content"></param>
	/// <returns>
	///   <para>The index of the selected button.</para>
	/// </returns>
	public static int SelectionGrid(int selected, string[] texts, int xCount, params GUILayoutOption[] options)
	{
		return SelectionGrid(selected, GUIContent.Temp(texts), xCount, GUI.skin.button, options);
	}

	/// <summary>
	///   <para>Make a Selection Grid.</para>
	/// </summary>
	/// <param name="selected">The index of the selected button.</param>
	/// <param name="texts">An array of strings to show on the buttons.</param>
	/// <param name="images">An array of textures on the buttons.</param>
	/// <param name="contents">An array of text, image and tooltips for the button.</param>
	/// <param name="xCount">How many elements to fit in the horizontal direction. The elements will be scaled to fit unless the style defines a fixedWidth to use. The height of the control will be determined from the number of elements.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <param name="content"></param>
	/// <returns>
	///   <para>The index of the selected button.</para>
	/// </returns>
	public static int SelectionGrid(int selected, Texture[] images, int xCount, params GUILayoutOption[] options)
	{
		return SelectionGrid(selected, GUIContent.Temp(images), xCount, GUI.skin.button, options);
	}

	/// <summary>
	///   <para>Make a Selection Grid.</para>
	/// </summary>
	/// <param name="selected">The index of the selected button.</param>
	/// <param name="texts">An array of strings to show on the buttons.</param>
	/// <param name="images">An array of textures on the buttons.</param>
	/// <param name="contents">An array of text, image and tooltips for the button.</param>
	/// <param name="xCount">How many elements to fit in the horizontal direction. The elements will be scaled to fit unless the style defines a fixedWidth to use. The height of the control will be determined from the number of elements.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <param name="content"></param>
	/// <returns>
	///   <para>The index of the selected button.</para>
	/// </returns>
	public static int SelectionGrid(int selected, GUIContent[] content, int xCount, params GUILayoutOption[] options)
	{
		return SelectionGrid(selected, content, xCount, GUI.skin.button, options);
	}

	/// <summary>
	///   <para>Make a Selection Grid.</para>
	/// </summary>
	/// <param name="selected">The index of the selected button.</param>
	/// <param name="texts">An array of strings to show on the buttons.</param>
	/// <param name="images">An array of textures on the buttons.</param>
	/// <param name="contents">An array of text, image and tooltips for the button.</param>
	/// <param name="xCount">How many elements to fit in the horizontal direction. The elements will be scaled to fit unless the style defines a fixedWidth to use. The height of the control will be determined from the number of elements.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <param name="content"></param>
	/// <returns>
	///   <para>The index of the selected button.</para>
	/// </returns>
	public static int SelectionGrid(int selected, string[] texts, int xCount, GUIStyle style, params GUILayoutOption[] options)
	{
		return SelectionGrid(selected, GUIContent.Temp(texts), xCount, style, options);
	}

	/// <summary>
	///   <para>Make a Selection Grid.</para>
	/// </summary>
	/// <param name="selected">The index of the selected button.</param>
	/// <param name="texts">An array of strings to show on the buttons.</param>
	/// <param name="images">An array of textures on the buttons.</param>
	/// <param name="contents">An array of text, image and tooltips for the button.</param>
	/// <param name="xCount">How many elements to fit in the horizontal direction. The elements will be scaled to fit unless the style defines a fixedWidth to use. The height of the control will be determined from the number of elements.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <param name="content"></param>
	/// <returns>
	///   <para>The index of the selected button.</para>
	/// </returns>
	public static int SelectionGrid(int selected, Texture[] images, int xCount, GUIStyle style, params GUILayoutOption[] options)
	{
		return SelectionGrid(selected, GUIContent.Temp(images), xCount, style, options);
	}

	/// <summary>
	///   <para>Make a Selection Grid.</para>
	/// </summary>
	/// <param name="selected">The index of the selected button.</param>
	/// <param name="texts">An array of strings to show on the buttons.</param>
	/// <param name="images">An array of textures on the buttons.</param>
	/// <param name="contents">An array of text, image and tooltips for the button.</param>
	/// <param name="xCount">How many elements to fit in the horizontal direction. The elements will be scaled to fit unless the style defines a fixedWidth to use. The height of the control will be determined from the number of elements.</param>
	/// <param name="style">The style to use. If left out, the button style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <param name="content"></param>
	/// <returns>
	///   <para>The index of the selected button.</para>
	/// </returns>
	public static int SelectionGrid(int selected, GUIContent[] contents, int xCount, GUIStyle style, params GUILayoutOption[] options)
	{
		return GUI.SelectionGrid(GUIGridSizer.GetRect(contents, xCount, style, options), selected, contents, xCount, style);
	}

	/// <summary>
	///   <para>A horizontal slider the user can drag to change a value between a min and a max.</para>
	/// </summary>
	/// <param name="value">The value the slider shows. This determines the position of the draggable thumb.</param>
	/// <param name="leftValue">The value at the left end of the slider.</param>
	/// <param name="rightValue">The value at the right end of the slider.</param>
	/// <param name="slider">The GUIStyle to use for displaying the dragging area. If left out, the horizontalSlider style from the current GUISkin is used.</param>
	/// <param name="thumb">The GUIStyle to use for displaying draggable thumb. If left out, the horizontalSliderThumb style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.</param>
	/// <returns>
	///   <para>The value that has been set by the user.</para>
	/// </returns>
	public static float HorizontalSlider(float value, float leftValue, float rightValue, params GUILayoutOption[] options)
	{
		return DoHorizontalSlider(value, leftValue, rightValue, GUI.skin.horizontalSlider, GUI.skin.horizontalSliderThumb, options);
	}

	/// <summary>
	///   <para>A horizontal slider the user can drag to change a value between a min and a max.</para>
	/// </summary>
	/// <param name="value">The value the slider shows. This determines the position of the draggable thumb.</param>
	/// <param name="leftValue">The value at the left end of the slider.</param>
	/// <param name="rightValue">The value at the right end of the slider.</param>
	/// <param name="slider">The GUIStyle to use for displaying the dragging area. If left out, the horizontalSlider style from the current GUISkin is used.</param>
	/// <param name="thumb">The GUIStyle to use for displaying draggable thumb. If left out, the horizontalSliderThumb style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.</param>
	/// <returns>
	///   <para>The value that has been set by the user.</para>
	/// </returns>
	public static float HorizontalSlider(float value, float leftValue, float rightValue, GUIStyle slider, GUIStyle thumb, params GUILayoutOption[] options)
	{
		return DoHorizontalSlider(value, leftValue, rightValue, slider, thumb, options);
	}

	private static float DoHorizontalSlider(float value, float leftValue, float rightValue, GUIStyle slider, GUIStyle thumb, GUILayoutOption[] options)
	{
		return GUI.HorizontalSlider(GUILayoutUtility.GetRect(GUIContent.Temp("mmmm"), slider, options), value, leftValue, rightValue, slider, thumb);
	}

	/// <summary>
	///   <para>A vertical slider the user can drag to change a value between a min and a max.</para>
	/// </summary>
	/// <param name="value">The value the slider shows. This determines the position of the draggable thumb.</param>
	/// <param name="topValue">The value at the top end of the slider.</param>
	/// <param name="bottomValue">The value at the bottom end of the slider.</param>
	/// <param name="slider">The GUIStyle to use for displaying the dragging area. If left out, the horizontalSlider style from the current GUISkin is used.</param>
	/// <param name="thumb">The GUIStyle to use for displaying draggable thumb. If left out, the horizontalSliderThumb style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.</param>
	/// <param name="leftValue"></param>
	/// <param name="rightValue"></param>
	/// <returns>
	///   <para>The value that has been set by the user.</para>
	/// </returns>
	public static float VerticalSlider(float value, float leftValue, float rightValue, params GUILayoutOption[] options)
	{
		return DoVerticalSlider(value, leftValue, rightValue, GUI.skin.verticalSlider, GUI.skin.verticalSliderThumb, options);
	}

	/// <summary>
	///   <para>A vertical slider the user can drag to change a value between a min and a max.</para>
	/// </summary>
	/// <param name="value">The value the slider shows. This determines the position of the draggable thumb.</param>
	/// <param name="topValue">The value at the top end of the slider.</param>
	/// <param name="bottomValue">The value at the bottom end of the slider.</param>
	/// <param name="slider">The GUIStyle to use for displaying the dragging area. If left out, the horizontalSlider style from the current GUISkin is used.</param>
	/// <param name="thumb">The GUIStyle to use for displaying draggable thumb. If left out, the horizontalSliderThumb style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.</param>
	/// <param name="leftValue"></param>
	/// <param name="rightValue"></param>
	/// <returns>
	///   <para>The value that has been set by the user.</para>
	/// </returns>
	public static float VerticalSlider(float value, float leftValue, float rightValue, GUIStyle slider, GUIStyle thumb, params GUILayoutOption[] options)
	{
		return DoVerticalSlider(value, leftValue, rightValue, slider, thumb, options);
	}

	private static float DoVerticalSlider(float value, float leftValue, float rightValue, GUIStyle slider, GUIStyle thumb, params GUILayoutOption[] options)
	{
		return GUI.VerticalSlider(GUILayoutUtility.GetRect(GUIContent.Temp("\n\n\n\n\n"), slider, options), value, leftValue, rightValue, slider, thumb);
	}

	/// <summary>
	///   <para>Make a horizontal scrollbar.</para>
	/// </summary>
	/// <param name="value">The position between min and max.</param>
	/// <param name="size">How much can we see?</param>
	/// <param name="leftValue">The value at the left end of the scrollbar.</param>
	/// <param name="rightValue">The value at the right end of the scrollbar.</param>
	/// <param name="style">The style to use for the scrollbar background. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.</param>
	/// <returns>
	///   <para>The modified value. This can be changed by the user by dragging the scrollbar, or clicking the arrows at the end.</para>
	/// </returns>
	public static float HorizontalScrollbar(float value, float size, float leftValue, float rightValue, params GUILayoutOption[] options)
	{
		return HorizontalScrollbar(value, size, leftValue, rightValue, GUI.skin.horizontalScrollbar, options);
	}

	/// <summary>
	///   <para>Make a horizontal scrollbar.</para>
	/// </summary>
	/// <param name="value">The position between min and max.</param>
	/// <param name="size">How much can we see?</param>
	/// <param name="leftValue">The value at the left end of the scrollbar.</param>
	/// <param name="rightValue">The value at the right end of the scrollbar.</param>
	/// <param name="style">The style to use for the scrollbar background. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.</param>
	/// <returns>
	///   <para>The modified value. This can be changed by the user by dragging the scrollbar, or clicking the arrows at the end.</para>
	/// </returns>
	public static float HorizontalScrollbar(float value, float size, float leftValue, float rightValue, GUIStyle style, params GUILayoutOption[] options)
	{
		return GUI.HorizontalScrollbar(GUILayoutUtility.GetRect(GUIContent.Temp("mmmm"), style, options), value, size, leftValue, rightValue, style);
	}

	/// <summary>
	///   <para>Make a vertical scrollbar.</para>
	/// </summary>
	/// <param name="value">The position between min and max.</param>
	/// <param name="size">How much can we see?</param>
	/// <param name="topValue">The value at the top end of the scrollbar.</param>
	/// <param name="bottomValue">The value at the bottom end of the scrollbar.</param>
	/// <param name="style">The style to use for the scrollbar background. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.</param>
	/// <returns>
	///   <para>The modified value. This can be changed by the user by dragging the scrollbar, or clicking the arrows at the end.</para>
	/// </returns>
	public static float VerticalScrollbar(float value, float size, float topValue, float bottomValue, params GUILayoutOption[] options)
	{
		return VerticalScrollbar(value, size, topValue, bottomValue, GUI.skin.verticalScrollbar, options);
	}

	/// <summary>
	///   <para>Make a vertical scrollbar.</para>
	/// </summary>
	/// <param name="value">The position between min and max.</param>
	/// <param name="size">How much can we see?</param>
	/// <param name="topValue">The value at the top end of the scrollbar.</param>
	/// <param name="bottomValue">The value at the bottom end of the scrollbar.</param>
	/// <param name="style">The style to use for the scrollbar background. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.</param>
	/// <returns>
	///   <para>The modified value. This can be changed by the user by dragging the scrollbar, or clicking the arrows at the end.</para>
	/// </returns>
	public static float VerticalScrollbar(float value, float size, float topValue, float bottomValue, GUIStyle style, params GUILayoutOption[] options)
	{
		return GUI.VerticalScrollbar(GUILayoutUtility.GetRect(GUIContent.Temp("\n\n\n\n"), style, options), value, size, topValue, bottomValue, style);
	}

	/// <summary>
	///   <para>Insert a space in the current layout group.</para>
	/// </summary>
	/// <param name="pixels"></param>
	public static void Space(float pixels)
	{
		GUIUtility.CheckOnGUI();
		if (GUILayoutUtility.current.topLevel.isVertical)
		{
			GUILayoutUtility.GetRect(0f, pixels, GUILayoutUtility.spaceStyle, Height(pixels));
		}
		else
		{
			GUILayoutUtility.GetRect(pixels, 0f, GUILayoutUtility.spaceStyle, Width(pixels));
		}
	}

	/// <summary>
	///   <para>Insert a flexible space element.</para>
	/// </summary>
	public static void FlexibleSpace()
	{
		GUIUtility.CheckOnGUI();
		GUILayoutOption gUILayoutOption = ((!GUILayoutUtility.current.topLevel.isVertical) ? ExpandWidth(expand: true) : ExpandHeight(expand: true));
		gUILayoutOption.value = 10000;
		GUILayoutUtility.GetRect(0f, 0f, GUILayoutUtility.spaceStyle, gUILayoutOption);
	}

	/// <summary>
	///   <para>Begin a Horizontal control group.</para>
	/// </summary>
	/// <param name="text">Text to display on group.</param>
	/// <param name="image">Texture to display on group.</param>
	/// <param name="content">Text, image, and tooltip for this group.</param>
	/// <param name="style">The style to use for background image and padding values. If left out, the background is transparent.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	public static void BeginHorizontal(params GUILayoutOption[] options)
	{
		BeginHorizontal(GUIContent.none, GUIStyle.none, options);
	}

	/// <summary>
	///   <para>Begin a Horizontal control group.</para>
	/// </summary>
	/// <param name="text">Text to display on group.</param>
	/// <param name="image">Texture to display on group.</param>
	/// <param name="content">Text, image, and tooltip for this group.</param>
	/// <param name="style">The style to use for background image and padding values. If left out, the background is transparent.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	public static void BeginHorizontal(GUIStyle style, params GUILayoutOption[] options)
	{
		BeginHorizontal(GUIContent.none, style, options);
	}

	/// <summary>
	///   <para>Begin a Horizontal control group.</para>
	/// </summary>
	/// <param name="text">Text to display on group.</param>
	/// <param name="image">Texture to display on group.</param>
	/// <param name="content">Text, image, and tooltip for this group.</param>
	/// <param name="style">The style to use for background image and padding values. If left out, the background is transparent.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	public static void BeginHorizontal(string text, GUIStyle style, params GUILayoutOption[] options)
	{
		BeginHorizontal(GUIContent.Temp(text), style, options);
	}

	/// <summary>
	///   <para>Begin a Horizontal control group.</para>
	/// </summary>
	/// <param name="text">Text to display on group.</param>
	/// <param name="image">Texture to display on group.</param>
	/// <param name="content">Text, image, and tooltip for this group.</param>
	/// <param name="style">The style to use for background image and padding values. If left out, the background is transparent.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	public static void BeginHorizontal(Texture image, GUIStyle style, params GUILayoutOption[] options)
	{
		BeginHorizontal(GUIContent.Temp(image), style, options);
	}

	/// <summary>
	///   <para>Begin a Horizontal control group.</para>
	/// </summary>
	/// <param name="text">Text to display on group.</param>
	/// <param name="image">Texture to display on group.</param>
	/// <param name="content">Text, image, and tooltip for this group.</param>
	/// <param name="style">The style to use for background image and padding values. If left out, the background is transparent.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	public static void BeginHorizontal(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
	{
		GUILayoutGroup gUILayoutGroup = GUILayoutUtility.BeginLayoutGroup(style, options, typeof(GUILayoutGroup));
		gUILayoutGroup.isVertical = false;
		if (style != GUIStyle.none || content != GUIContent.none)
		{
			GUI.Box(gUILayoutGroup.rect, content, style);
		}
	}

	/// <summary>
	///   <para>Close a group started with BeginHorizontal.</para>
	/// </summary>
	public static void EndHorizontal()
	{
		GUILayoutUtility.EndLayoutGroup();
	}

	/// <summary>
	///   <para>Begin a vertical control group.</para>
	/// </summary>
	/// <param name="text">Text to display on group.</param>
	/// <param name="image">Texture to display on group.</param>
	/// <param name="content">Text, image, and tooltip for this group.</param>
	/// <param name="style">The style to use for background image and padding values. If left out, the background is transparent.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	public static void BeginVertical(params GUILayoutOption[] options)
	{
		BeginVertical(GUIContent.none, GUIStyle.none, options);
	}

	/// <summary>
	///   <para>Begin a vertical control group.</para>
	/// </summary>
	/// <param name="text">Text to display on group.</param>
	/// <param name="image">Texture to display on group.</param>
	/// <param name="content">Text, image, and tooltip for this group.</param>
	/// <param name="style">The style to use for background image and padding values. If left out, the background is transparent.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	public static void BeginVertical(GUIStyle style, params GUILayoutOption[] options)
	{
		BeginVertical(GUIContent.none, style, options);
	}

	/// <summary>
	///   <para>Begin a vertical control group.</para>
	/// </summary>
	/// <param name="text">Text to display on group.</param>
	/// <param name="image">Texture to display on group.</param>
	/// <param name="content">Text, image, and tooltip for this group.</param>
	/// <param name="style">The style to use for background image and padding values. If left out, the background is transparent.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	public static void BeginVertical(string text, GUIStyle style, params GUILayoutOption[] options)
	{
		BeginVertical(GUIContent.Temp(text), style, options);
	}

	/// <summary>
	///   <para>Begin a vertical control group.</para>
	/// </summary>
	/// <param name="text">Text to display on group.</param>
	/// <param name="image">Texture to display on group.</param>
	/// <param name="content">Text, image, and tooltip for this group.</param>
	/// <param name="style">The style to use for background image and padding values. If left out, the background is transparent.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	public static void BeginVertical(Texture image, GUIStyle style, params GUILayoutOption[] options)
	{
		BeginVertical(GUIContent.Temp(image), style, options);
	}

	/// <summary>
	///   <para>Begin a vertical control group.</para>
	/// </summary>
	/// <param name="text">Text to display on group.</param>
	/// <param name="image">Texture to display on group.</param>
	/// <param name="content">Text, image, and tooltip for this group.</param>
	/// <param name="style">The style to use for background image and padding values. If left out, the background is transparent.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	public static void BeginVertical(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
	{
		GUILayoutGroup gUILayoutGroup = GUILayoutUtility.BeginLayoutGroup(style, options, typeof(GUILayoutGroup));
		gUILayoutGroup.isVertical = true;
		if (style != GUIStyle.none || content != GUIContent.none)
		{
			GUI.Box(gUILayoutGroup.rect, content, style);
		}
	}

	/// <summary>
	///   <para>Close a group started with BeginVertical.</para>
	/// </summary>
	public static void EndVertical()
	{
		GUILayoutUtility.EndLayoutGroup();
	}

	/// <summary>
	///   <para>Begin a GUILayout block of GUI controls in a fixed screen area.</para>
	/// </summary>
	/// <param name="text">Optional text to display in the area.</param>
	/// <param name="image">Optional texture to display in the area.</param>
	/// <param name="content">Optional text, image and tooltip top display for this area.</param>
	/// <param name="style">The style to use. If left out, the empty GUIStyle (GUIStyle.none) is used, giving a transparent background.</param>
	/// <param name="screenRect"></param>
	public static void BeginArea(Rect screenRect)
	{
		BeginArea(screenRect, GUIContent.none, GUIStyle.none);
	}

	/// <summary>
	///   <para>Begin a GUILayout block of GUI controls in a fixed screen area.</para>
	/// </summary>
	/// <param name="text">Optional text to display in the area.</param>
	/// <param name="image">Optional texture to display in the area.</param>
	/// <param name="content">Optional text, image and tooltip top display for this area.</param>
	/// <param name="style">The style to use. If left out, the empty GUIStyle (GUIStyle.none) is used, giving a transparent background.</param>
	/// <param name="screenRect"></param>
	public static void BeginArea(Rect screenRect, string text)
	{
		BeginArea(screenRect, GUIContent.Temp(text), GUIStyle.none);
	}

	/// <summary>
	///   <para>Begin a GUILayout block of GUI controls in a fixed screen area.</para>
	/// </summary>
	/// <param name="text">Optional text to display in the area.</param>
	/// <param name="image">Optional texture to display in the area.</param>
	/// <param name="content">Optional text, image and tooltip top display for this area.</param>
	/// <param name="style">The style to use. If left out, the empty GUIStyle (GUIStyle.none) is used, giving a transparent background.</param>
	/// <param name="screenRect"></param>
	public static void BeginArea(Rect screenRect, Texture image)
	{
		BeginArea(screenRect, GUIContent.Temp(image), GUIStyle.none);
	}

	/// <summary>
	///   <para>Begin a GUILayout block of GUI controls in a fixed screen area.</para>
	/// </summary>
	/// <param name="text">Optional text to display in the area.</param>
	/// <param name="image">Optional texture to display in the area.</param>
	/// <param name="content">Optional text, image and tooltip top display for this area.</param>
	/// <param name="style">The style to use. If left out, the empty GUIStyle (GUIStyle.none) is used, giving a transparent background.</param>
	/// <param name="screenRect"></param>
	public static void BeginArea(Rect screenRect, GUIContent content)
	{
		BeginArea(screenRect, content, GUIStyle.none);
	}

	/// <summary>
	///   <para>Begin a GUILayout block of GUI controls in a fixed screen area.</para>
	/// </summary>
	/// <param name="text">Optional text to display in the area.</param>
	/// <param name="image">Optional texture to display in the area.</param>
	/// <param name="content">Optional text, image and tooltip top display for this area.</param>
	/// <param name="style">The style to use. If left out, the empty GUIStyle (GUIStyle.none) is used, giving a transparent background.</param>
	/// <param name="screenRect"></param>
	public static void BeginArea(Rect screenRect, GUIStyle style)
	{
		BeginArea(screenRect, GUIContent.none, style);
	}

	/// <summary>
	///   <para>Begin a GUILayout block of GUI controls in a fixed screen area.</para>
	/// </summary>
	/// <param name="text">Optional text to display in the area.</param>
	/// <param name="image">Optional texture to display in the area.</param>
	/// <param name="content">Optional text, image and tooltip top display for this area.</param>
	/// <param name="style">The style to use. If left out, the empty GUIStyle (GUIStyle.none) is used, giving a transparent background.</param>
	/// <param name="screenRect"></param>
	public static void BeginArea(Rect screenRect, string text, GUIStyle style)
	{
		BeginArea(screenRect, GUIContent.Temp(text), style);
	}

	/// <summary>
	///   <para>Begin a GUILayout block of GUI controls in a fixed screen area.</para>
	/// </summary>
	/// <param name="text">Optional text to display in the area.</param>
	/// <param name="image">Optional texture to display in the area.</param>
	/// <param name="content">Optional text, image and tooltip top display for this area.</param>
	/// <param name="style">The style to use. If left out, the empty GUIStyle (GUIStyle.none) is used, giving a transparent background.</param>
	/// <param name="screenRect"></param>
	public static void BeginArea(Rect screenRect, Texture image, GUIStyle style)
	{
		BeginArea(screenRect, GUIContent.Temp(image), style);
	}

	/// <summary>
	///   <para>Begin a GUILayout block of GUI controls in a fixed screen area.</para>
	/// </summary>
	/// <param name="text">Optional text to display in the area.</param>
	/// <param name="image">Optional texture to display in the area.</param>
	/// <param name="content">Optional text, image and tooltip top display for this area.</param>
	/// <param name="style">The style to use. If left out, the empty GUIStyle (GUIStyle.none) is used, giving a transparent background.</param>
	/// <param name="screenRect"></param>
	public static void BeginArea(Rect screenRect, GUIContent content, GUIStyle style)
	{
		GUIUtility.CheckOnGUI();
		GUILayoutGroup gUILayoutGroup = GUILayoutUtility.BeginLayoutArea(style, typeof(GUILayoutGroup));
		if (Event.current.type == EventType.Layout)
		{
			gUILayoutGroup.resetCoords = true;
			gUILayoutGroup.minWidth = (gUILayoutGroup.maxWidth = screenRect.width);
			gUILayoutGroup.minHeight = (gUILayoutGroup.maxHeight = screenRect.height);
			gUILayoutGroup.rect = Rect.MinMaxRect(screenRect.xMin, screenRect.yMin, gUILayoutGroup.rect.xMax, gUILayoutGroup.rect.yMax);
		}
		GUI.BeginGroup(gUILayoutGroup.rect, content, style);
	}

	/// <summary>
	///   <para>Close a GUILayout block started with BeginArea.</para>
	/// </summary>
	public static void EndArea()
	{
		GUIUtility.CheckOnGUI();
		if (Event.current.type != EventType.Used)
		{
			GUILayoutUtility.current.layoutGroups.Pop();
			GUILayoutUtility.current.topLevel = (GUILayoutGroup)GUILayoutUtility.current.layoutGroups.Peek();
			GUI.EndGroup();
		}
	}

	/// <summary>
	///   <para>Begin an automatically laid out scrollview.</para>
	/// </summary>
	/// <param name="scrollPosition">The position to use display.</param>
	/// <param name="alwayShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when the content inside the ScrollView is wider than the scrollview itself.</param>
	/// <param name="alwayShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when content inside the ScrollView is taller than the scrollview itself.</param>
	/// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
	/// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
	/// <param name="options"></param>
	/// <param name="alwaysShowHorizontal"></param>
	/// <param name="alwaysShowVertical"></param>
	/// <param name="style"></param>
	/// <param name="background"></param>
	/// <returns>
	///   <para>The modified scrollPosition. Feed this back into the variable you pass in, as shown in the example.</para>
	/// </returns>
	public static Vector2 BeginScrollView(Vector2 scrollPosition, params GUILayoutOption[] options)
	{
		return BeginScrollView(scrollPosition, alwaysShowHorizontal: false, alwaysShowVertical: false, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, GUI.skin.scrollView, options);
	}

	/// <summary>
	///   <para>Begin an automatically laid out scrollview.</para>
	/// </summary>
	/// <param name="scrollPosition">The position to use display.</param>
	/// <param name="alwayShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when the content inside the ScrollView is wider than the scrollview itself.</param>
	/// <param name="alwayShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when content inside the ScrollView is taller than the scrollview itself.</param>
	/// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
	/// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
	/// <param name="options"></param>
	/// <param name="alwaysShowHorizontal"></param>
	/// <param name="alwaysShowVertical"></param>
	/// <param name="style"></param>
	/// <param name="background"></param>
	/// <returns>
	///   <para>The modified scrollPosition. Feed this back into the variable you pass in, as shown in the example.</para>
	/// </returns>
	public static Vector2 BeginScrollView(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, params GUILayoutOption[] options)
	{
		return BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, GUI.skin.scrollView, options);
	}

	/// <summary>
	///   <para>Begin an automatically laid out scrollview.</para>
	/// </summary>
	/// <param name="scrollPosition">The position to use display.</param>
	/// <param name="alwayShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when the content inside the ScrollView is wider than the scrollview itself.</param>
	/// <param name="alwayShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when content inside the ScrollView is taller than the scrollview itself.</param>
	/// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
	/// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
	/// <param name="options"></param>
	/// <param name="alwaysShowHorizontal"></param>
	/// <param name="alwaysShowVertical"></param>
	/// <param name="style"></param>
	/// <param name="background"></param>
	/// <returns>
	///   <para>The modified scrollPosition. Feed this back into the variable you pass in, as shown in the example.</para>
	/// </returns>
	public static Vector2 BeginScrollView(Vector2 scrollPosition, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, params GUILayoutOption[] options)
	{
		return BeginScrollView(scrollPosition, alwaysShowHorizontal: false, alwaysShowVertical: false, horizontalScrollbar, verticalScrollbar, GUI.skin.scrollView, options);
	}

	/// <summary>
	///   <para>Begin an automatically laid out scrollview.</para>
	/// </summary>
	/// <param name="scrollPosition">The position to use display.</param>
	/// <param name="alwayShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when the content inside the ScrollView is wider than the scrollview itself.</param>
	/// <param name="alwayShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when content inside the ScrollView is taller than the scrollview itself.</param>
	/// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
	/// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
	/// <param name="options"></param>
	/// <param name="alwaysShowHorizontal"></param>
	/// <param name="alwaysShowVertical"></param>
	/// <param name="style"></param>
	/// <param name="background"></param>
	/// <returns>
	///   <para>The modified scrollPosition. Feed this back into the variable you pass in, as shown in the example.</para>
	/// </returns>
	public static Vector2 BeginScrollView(Vector2 scrollPosition, GUIStyle style)
	{
		GUILayoutOption[] options = null;
		return BeginScrollView(scrollPosition, style, options);
	}

	/// <summary>
	///   <para>Begin an automatically laid out scrollview.</para>
	/// </summary>
	/// <param name="scrollPosition">The position to use display.</param>
	/// <param name="alwayShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when the content inside the ScrollView is wider than the scrollview itself.</param>
	/// <param name="alwayShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when content inside the ScrollView is taller than the scrollview itself.</param>
	/// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
	/// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
	/// <param name="options"></param>
	/// <param name="alwaysShowHorizontal"></param>
	/// <param name="alwaysShowVertical"></param>
	/// <param name="style"></param>
	/// <param name="background"></param>
	/// <returns>
	///   <para>The modified scrollPosition. Feed this back into the variable you pass in, as shown in the example.</para>
	/// </returns>
	public static Vector2 BeginScrollView(Vector2 scrollPosition, GUIStyle style, params GUILayoutOption[] options)
	{
		string name = style.name;
		GUIStyle gUIStyle = GUI.skin.FindStyle(name + "VerticalScrollbar");
		if (gUIStyle == null)
		{
			gUIStyle = GUI.skin.verticalScrollbar;
		}
		GUIStyle gUIStyle2 = GUI.skin.FindStyle(name + "HorizontalScrollbar");
		if (gUIStyle2 == null)
		{
			gUIStyle2 = GUI.skin.horizontalScrollbar;
		}
		return BeginScrollView(scrollPosition, alwaysShowHorizontal: false, alwaysShowVertical: false, gUIStyle2, gUIStyle, style, options);
	}

	/// <summary>
	///   <para>Begin an automatically laid out scrollview.</para>
	/// </summary>
	/// <param name="scrollPosition">The position to use display.</param>
	/// <param name="alwayShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when the content inside the ScrollView is wider than the scrollview itself.</param>
	/// <param name="alwayShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when content inside the ScrollView is taller than the scrollview itself.</param>
	/// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
	/// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
	/// <param name="options"></param>
	/// <param name="alwaysShowHorizontal"></param>
	/// <param name="alwaysShowVertical"></param>
	/// <param name="style"></param>
	/// <param name="background"></param>
	/// <returns>
	///   <para>The modified scrollPosition. Feed this back into the variable you pass in, as shown in the example.</para>
	/// </returns>
	public static Vector2 BeginScrollView(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, params GUILayoutOption[] options)
	{
		return BeginScrollView(scrollPosition, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, GUI.skin.scrollView, options);
	}

	/// <summary>
	///   <para>Begin an automatically laid out scrollview.</para>
	/// </summary>
	/// <param name="scrollPosition">The position to use display.</param>
	/// <param name="alwayShowHorizontal">Optional parameter to always show the horizontal scrollbar. If false or left out, it is only shown when the content inside the ScrollView is wider than the scrollview itself.</param>
	/// <param name="alwayShowVertical">Optional parameter to always show the vertical scrollbar. If false or left out, it is only shown when content inside the ScrollView is taller than the scrollview itself.</param>
	/// <param name="horizontalScrollbar">Optional GUIStyle to use for the horizontal scrollbar. If left out, the horizontalScrollbar style from the current GUISkin is used.</param>
	/// <param name="verticalScrollbar">Optional GUIStyle to use for the vertical scrollbar. If left out, the verticalScrollbar style from the current GUISkin is used.</param>
	/// <param name="options"></param>
	/// <param name="alwaysShowHorizontal"></param>
	/// <param name="alwaysShowVertical"></param>
	/// <param name="style"></param>
	/// <param name="background"></param>
	/// <returns>
	///   <para>The modified scrollPosition. Feed this back into the variable you pass in, as shown in the example.</para>
	/// </returns>
	public static Vector2 BeginScrollView(Vector2 scrollPosition, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background, params GUILayoutOption[] options)
	{
		GUIUtility.CheckOnGUI();
		GUIScrollGroup gUIScrollGroup = (GUIScrollGroup)GUILayoutUtility.BeginLayoutGroup(background, null, typeof(GUIScrollGroup));
		EventType type = Event.current.type;
		if (type == EventType.Layout)
		{
			gUIScrollGroup.resetCoords = true;
			gUIScrollGroup.isVertical = true;
			gUIScrollGroup.stretchWidth = 1;
			gUIScrollGroup.stretchHeight = 1;
			gUIScrollGroup.verticalScrollbar = verticalScrollbar;
			gUIScrollGroup.horizontalScrollbar = horizontalScrollbar;
			gUIScrollGroup.needsVerticalScrollbar = alwaysShowVertical;
			gUIScrollGroup.needsHorizontalScrollbar = alwaysShowHorizontal;
			gUIScrollGroup.ApplyOptions(options);
		}
		return GUI.BeginScrollView(gUIScrollGroup.rect, scrollPosition, new Rect(0f, 0f, gUIScrollGroup.clientWidth, gUIScrollGroup.clientHeight), alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, background);
	}

	/// <summary>
	///   <para>End a scroll view begun with a call to BeginScrollView.</para>
	/// </summary>
	public static void EndScrollView()
	{
		EndScrollView(handleScrollWheel: true);
	}

	internal static void EndScrollView(bool handleScrollWheel)
	{
		GUILayoutUtility.EndLayoutGroup();
		GUI.EndScrollView(handleScrollWheel);
	}

	public static Rect Window(int id, Rect screenRect, GUI.WindowFunction func, string text, params GUILayoutOption[] options)
	{
		return DoWindow(id, screenRect, func, GUIContent.Temp(text), GUI.skin.window, options);
	}

	public static Rect Window(int id, Rect screenRect, GUI.WindowFunction func, Texture image, params GUILayoutOption[] options)
	{
		return DoWindow(id, screenRect, func, GUIContent.Temp(image), GUI.skin.window, options);
	}

	public static Rect Window(int id, Rect screenRect, GUI.WindowFunction func, GUIContent content, params GUILayoutOption[] options)
	{
		return DoWindow(id, screenRect, func, content, GUI.skin.window, options);
	}

	public static Rect Window(int id, Rect screenRect, GUI.WindowFunction func, string text, GUIStyle style, params GUILayoutOption[] options)
	{
		return DoWindow(id, screenRect, func, GUIContent.Temp(text), style, options);
	}

	public static Rect Window(int id, Rect screenRect, GUI.WindowFunction func, Texture image, GUIStyle style, params GUILayoutOption[] options)
	{
		return DoWindow(id, screenRect, func, GUIContent.Temp(image), style, options);
	}

	public static Rect Window(int id, Rect screenRect, GUI.WindowFunction func, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
	{
		return DoWindow(id, screenRect, func, content, style, options);
	}

	private static Rect DoWindow(int id, Rect screenRect, GUI.WindowFunction func, GUIContent content, GUIStyle style, GUILayoutOption[] options)
	{
		GUIUtility.CheckOnGUI();
		LayoutedWindow @object = new LayoutedWindow(func, screenRect, content, options, style);
		return GUI.Window(id, screenRect, @object.DoWindow, content, style);
	}

	/// <summary>
	///   <para>Option passed to a control to give it an absolute width.</para>
	/// </summary>
	/// <param name="width"></param>
	public static GUILayoutOption Width(float width)
	{
		return new GUILayoutOption(GUILayoutOption.Type.fixedWidth, width);
	}

	/// <summary>
	///   <para>Option passed to a control to specify a minimum width.
	/// </para>
	/// </summary>
	/// <param name="minWidth"></param>
	public static GUILayoutOption MinWidth(float minWidth)
	{
		return new GUILayoutOption(GUILayoutOption.Type.minWidth, minWidth);
	}

	/// <summary>
	///   <para>Option passed to a control to specify a maximum width.</para>
	/// </summary>
	/// <param name="maxWidth"></param>
	public static GUILayoutOption MaxWidth(float maxWidth)
	{
		return new GUILayoutOption(GUILayoutOption.Type.maxWidth, maxWidth);
	}

	/// <summary>
	///   <para>Option passed to a control to give it an absolute height.</para>
	/// </summary>
	/// <param name="height"></param>
	public static GUILayoutOption Height(float height)
	{
		return new GUILayoutOption(GUILayoutOption.Type.fixedHeight, height);
	}

	/// <summary>
	///   <para>Option passed to a control to specify a minimum height.</para>
	/// </summary>
	/// <param name="minHeight"></param>
	public static GUILayoutOption MinHeight(float minHeight)
	{
		return new GUILayoutOption(GUILayoutOption.Type.minHeight, minHeight);
	}

	/// <summary>
	///   <para>Option passed to a control to specify a maximum height.</para>
	/// </summary>
	/// <param name="maxHeight"></param>
	public static GUILayoutOption MaxHeight(float maxHeight)
	{
		return new GUILayoutOption(GUILayoutOption.Type.maxHeight, maxHeight);
	}

	/// <summary>
	///   <para>Option passed to a control to allow or disallow horizontal expansion.</para>
	/// </summary>
	/// <param name="expand"></param>
	public static GUILayoutOption ExpandWidth(bool expand)
	{
		return new GUILayoutOption(GUILayoutOption.Type.stretchWidth, expand ? 1 : 0);
	}

	/// <summary>
	///   <para>Option passed to a control to allow or disallow vertical expansion.</para>
	/// </summary>
	/// <param name="expand"></param>
	public static GUILayoutOption ExpandHeight(bool expand)
	{
		return new GUILayoutOption(GUILayoutOption.Type.stretchHeight, expand ? 1 : 0);
	}
}

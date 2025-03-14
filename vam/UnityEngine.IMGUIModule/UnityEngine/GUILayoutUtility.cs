using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEngine;

/// <summary>
///   <para>Utility functions for implementing and extending the GUILayout class.</para>
/// </summary>
public class GUILayoutUtility
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal sealed class LayoutCache
	{
		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal GUILayoutGroup topLevel = new GUILayoutGroup();

		internal GenericStack layoutGroups = new GenericStack();

		internal GUILayoutGroup windows = new GUILayoutGroup();

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal LayoutCache()
		{
			layoutGroups.Push(topLevel);
		}

		internal LayoutCache(LayoutCache other)
		{
			topLevel = other.topLevel;
			layoutGroups = other.layoutGroups;
			windows = other.windows;
		}
	}

	private static readonly Dictionary<int, LayoutCache> s_StoredLayouts = new Dictionary<int, LayoutCache>();

	private static readonly Dictionary<int, LayoutCache> s_StoredWindows = new Dictionary<int, LayoutCache>();

	internal static LayoutCache current = new LayoutCache();

	internal static readonly Rect kDummyRect = new Rect(0f, 0f, 1f, 1f);

	private static GUIStyle s_SpaceStyle;

	internal static GUILayoutGroup topLevel
	{
		[CompilerGenerated]
		get
		{
			return current.topLevel;
		}
	}

	internal static GUIStyle spaceStyle
	{
		get
		{
			if (s_SpaceStyle == null)
			{
				s_SpaceStyle = new GUIStyle();
			}
			s_SpaceStyle.stretchWidth = false;
			return s_SpaceStyle;
		}
	}

	private static Rect Internal_GetWindowRect(int windowID)
	{
		INTERNAL_CALL_Internal_GetWindowRect(windowID, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Internal_GetWindowRect(int windowID, out Rect value);

	private static void Internal_MoveWindow(int windowID, Rect r)
	{
		INTERNAL_CALL_Internal_MoveWindow(windowID, ref r);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Internal_MoveWindow(int windowID, ref Rect r);

	internal static Rect GetWindowsBounds()
	{
		INTERNAL_CALL_GetWindowsBounds(out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetWindowsBounds(out Rect value);

	internal static void CleanupRoots()
	{
		s_SpaceStyle = null;
		s_StoredLayouts.Clear();
		s_StoredWindows.Clear();
		current = new LayoutCache();
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static LayoutCache SelectIDList(int instanceID, bool isWindow)
	{
		Dictionary<int, LayoutCache> dictionary = ((!isWindow) ? s_StoredLayouts : s_StoredWindows);
		if (!dictionary.TryGetValue(instanceID, out var value))
		{
			value = (dictionary[instanceID] = new LayoutCache());
		}
		current.topLevel = value.topLevel;
		current.layoutGroups = value.layoutGroups;
		current.windows = value.windows;
		return value;
	}

	internal static void Begin(int instanceID)
	{
		LayoutCache layoutCache = SelectIDList(instanceID, isWindow: false);
		if (Event.current.type == EventType.Layout)
		{
			current.topLevel = (layoutCache.topLevel = new GUILayoutGroup());
			current.layoutGroups.Clear();
			current.layoutGroups.Push(current.topLevel);
			current.windows = (layoutCache.windows = new GUILayoutGroup());
		}
		else
		{
			current.topLevel = layoutCache.topLevel;
			current.layoutGroups = layoutCache.layoutGroups;
			current.windows = layoutCache.windows;
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static void BeginContainer(LayoutCache cache)
	{
		if (Event.current.type == EventType.Layout)
		{
			cache.topLevel = new GUILayoutGroup();
			cache.layoutGroups.Clear();
			cache.layoutGroups.Push(cache.topLevel);
			cache.windows = new GUILayoutGroup();
		}
		current.topLevel = cache.topLevel;
		current.layoutGroups = cache.layoutGroups;
		current.windows = cache.windows;
	}

	internal static void BeginWindow(int windowID, GUIStyle style, GUILayoutOption[] options)
	{
		LayoutCache layoutCache = SelectIDList(windowID, isWindow: true);
		if (Event.current.type == EventType.Layout)
		{
			current.topLevel = (layoutCache.topLevel = new GUILayoutGroup());
			current.topLevel.style = style;
			current.topLevel.windowID = windowID;
			if (options != null)
			{
				current.topLevel.ApplyOptions(options);
			}
			current.layoutGroups.Clear();
			current.layoutGroups.Push(current.topLevel);
			current.windows = (layoutCache.windows = new GUILayoutGroup());
		}
		else
		{
			current.topLevel = layoutCache.topLevel;
			current.layoutGroups = layoutCache.layoutGroups;
			current.windows = layoutCache.windows;
		}
	}

	[Obsolete("BeginGroup has no effect and will be removed", false)]
	public static void BeginGroup(string GroupName)
	{
	}

	[Obsolete("EndGroup has no effect and will be removed", false)]
	public static void EndGroup(string groupName)
	{
	}

	internal static void Layout()
	{
		if (current.topLevel.windowID == -1)
		{
			current.topLevel.CalcWidth();
			current.topLevel.SetHorizontal(0f, Mathf.Min((float)Screen.width / GUIUtility.pixelsPerPoint, current.topLevel.maxWidth));
			current.topLevel.CalcHeight();
			current.topLevel.SetVertical(0f, Mathf.Min((float)Screen.height / GUIUtility.pixelsPerPoint, current.topLevel.maxHeight));
			LayoutFreeGroup(current.windows);
		}
		else
		{
			LayoutSingleGroup(current.topLevel);
			LayoutFreeGroup(current.windows);
		}
	}

	internal static void LayoutFromEditorWindow()
	{
		if (current.topLevel != null)
		{
			current.topLevel.CalcWidth();
			current.topLevel.SetHorizontal(0f, (float)Screen.width / GUIUtility.pixelsPerPoint);
			current.topLevel.CalcHeight();
			current.topLevel.SetVertical(0f, (float)Screen.height / GUIUtility.pixelsPerPoint);
			LayoutFreeGroup(current.windows);
		}
		else
		{
			Debug.LogError("GUILayout state invalid. Verify that all layout begin/end calls match.");
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static void LayoutFromContainer(float w, float h)
	{
		if (current.topLevel != null)
		{
			current.topLevel.CalcWidth();
			current.topLevel.SetHorizontal(0f, w);
			current.topLevel.CalcHeight();
			current.topLevel.SetVertical(0f, h);
			LayoutFreeGroup(current.windows);
		}
		else
		{
			Debug.LogError("GUILayout state invalid. Verify that all layout begin/end calls match.");
		}
	}

	internal static float LayoutFromInspector(float width)
	{
		if (current.topLevel != null && current.topLevel.windowID == -1)
		{
			current.topLevel.CalcWidth();
			current.topLevel.SetHorizontal(0f, width);
			current.topLevel.CalcHeight();
			current.topLevel.SetVertical(0f, Mathf.Min((float)Screen.height / GUIUtility.pixelsPerPoint, current.topLevel.maxHeight));
			float minHeight = current.topLevel.minHeight;
			LayoutFreeGroup(current.windows);
			return minHeight;
		}
		if (current.topLevel != null)
		{
			LayoutSingleGroup(current.topLevel);
		}
		return 0f;
	}

	internal static void LayoutFreeGroup(GUILayoutGroup toplevel)
	{
		foreach (GUILayoutGroup entry in toplevel.entries)
		{
			LayoutSingleGroup(entry);
		}
		toplevel.ResetCursor();
	}

	private static void LayoutSingleGroup(GUILayoutGroup i)
	{
		if (!i.isWindow)
		{
			float minWidth = i.minWidth;
			float maxWidth = i.maxWidth;
			i.CalcWidth();
			i.SetHorizontal(i.rect.x, Mathf.Clamp(i.maxWidth, minWidth, maxWidth));
			float minHeight = i.minHeight;
			float maxHeight = i.maxHeight;
			i.CalcHeight();
			i.SetVertical(i.rect.y, Mathf.Clamp(i.maxHeight, minHeight, maxHeight));
		}
		else
		{
			i.CalcWidth();
			Rect rect = Internal_GetWindowRect(i.windowID);
			i.SetHorizontal(rect.x, Mathf.Clamp(rect.width, i.minWidth, i.maxWidth));
			i.CalcHeight();
			i.SetVertical(rect.y, Mathf.Clamp(rect.height, i.minHeight, i.maxHeight));
			Internal_MoveWindow(i.windowID, i.rect);
		}
	}

	[SecuritySafeCritical]
	private static GUILayoutGroup CreateGUILayoutGroupInstanceOfType(Type LayoutType)
	{
		if (!typeof(GUILayoutGroup).IsAssignableFrom(LayoutType))
		{
			throw new ArgumentException("LayoutType needs to be of type GUILayoutGroup");
		}
		return (GUILayoutGroup)Activator.CreateInstance(LayoutType);
	}

	internal static GUILayoutGroup BeginLayoutGroup(GUIStyle style, GUILayoutOption[] options, Type layoutType)
	{
		EventType type = Event.current.type;
		GUILayoutGroup gUILayoutGroup;
		if (type == EventType.Used || type == EventType.Layout)
		{
			gUILayoutGroup = CreateGUILayoutGroupInstanceOfType(layoutType);
			gUILayoutGroup.style = style;
			if (options != null)
			{
				gUILayoutGroup.ApplyOptions(options);
			}
			current.topLevel.Add(gUILayoutGroup);
		}
		else
		{
			gUILayoutGroup = current.topLevel.GetNext() as GUILayoutGroup;
			if (gUILayoutGroup == null)
			{
				throw new ArgumentException("GUILayout: Mismatched LayoutGroup." + Event.current.type);
			}
			gUILayoutGroup.ResetCursor();
		}
		current.layoutGroups.Push(gUILayoutGroup);
		current.topLevel = gUILayoutGroup;
		return gUILayoutGroup;
	}

	internal static void EndLayoutGroup()
	{
		if (current.layoutGroups.Count == 0)
		{
			Debug.LogError("EndLayoutGroup: BeginLayoutGroup must be called first.");
			return;
		}
		current.layoutGroups.Pop();
		if (0 < current.layoutGroups.Count)
		{
			current.topLevel = (GUILayoutGroup)current.layoutGroups.Peek();
		}
		else
		{
			current.topLevel = new GUILayoutGroup();
		}
	}

	internal static GUILayoutGroup BeginLayoutArea(GUIStyle style, Type layoutType)
	{
		EventType type = Event.current.type;
		GUILayoutGroup gUILayoutGroup;
		if (type == EventType.Used || type == EventType.Layout)
		{
			gUILayoutGroup = CreateGUILayoutGroupInstanceOfType(layoutType);
			gUILayoutGroup.style = style;
			current.windows.Add(gUILayoutGroup);
		}
		else
		{
			gUILayoutGroup = current.windows.GetNext() as GUILayoutGroup;
			if (gUILayoutGroup == null)
			{
				throw new ArgumentException("GUILayout: Mismatched LayoutGroup." + Event.current.type);
			}
			gUILayoutGroup.ResetCursor();
		}
		current.layoutGroups.Push(gUILayoutGroup);
		current.topLevel = gUILayoutGroup;
		return gUILayoutGroup;
	}

	internal static GUILayoutGroup DoBeginLayoutArea(GUIStyle style, Type layoutType)
	{
		return BeginLayoutArea(style, layoutType);
	}

	/// <summary>
	///   <para>Reserve layout space for a rectangle for displaying some contents with a specific style.</para>
	/// </summary>
	/// <param name="content">The content to make room for displaying.</param>
	/// <param name="style">The GUIStyle to layout for.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>A rectangle that is large enough to contain content when rendered in style.</para>
	/// </returns>
	public static Rect GetRect(GUIContent content, GUIStyle style)
	{
		return DoGetRect(content, style, null);
	}

	/// <summary>
	///   <para>Reserve layout space for a rectangle for displaying some contents with a specific style.</para>
	/// </summary>
	/// <param name="content">The content to make room for displaying.</param>
	/// <param name="style">The GUIStyle to layout for.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>A rectangle that is large enough to contain content when rendered in style.</para>
	/// </returns>
	public static Rect GetRect(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
	{
		return DoGetRect(content, style, options);
	}

	private static Rect DoGetRect(GUIContent content, GUIStyle style, GUILayoutOption[] options)
	{
		GUIUtility.CheckOnGUI();
		switch (Event.current.type)
		{
		case EventType.Layout:
			if (style.isHeightDependantOnWidth)
			{
				current.topLevel.Add(new GUIWordWrapSizer(style, content, options));
			}
			else
			{
				Vector2 constraints = new Vector2(0f, 0f);
				if (options != null)
				{
					foreach (GUILayoutOption gUILayoutOption in options)
					{
						switch (gUILayoutOption.type)
						{
						case GUILayoutOption.Type.maxHeight:
							constraints.y = (float)gUILayoutOption.value;
							break;
						case GUILayoutOption.Type.maxWidth:
							constraints.x = (float)gUILayoutOption.value;
							break;
						}
					}
				}
				Vector2 vector = style.CalcSizeWithConstraints(content, constraints);
				current.topLevel.Add(new GUILayoutEntry(vector.x, vector.x, vector.y, vector.y, style, options));
			}
			return kDummyRect;
		case EventType.Used:
			return kDummyRect;
		default:
		{
			GUILayoutEntry next = current.topLevel.GetNext();
			return next.rect;
		}
		}
	}

	/// <summary>
	///   <para>Reserve layout space for a rectangle with a fixed content area.</para>
	/// </summary>
	/// <param name="width">The width of the area you want.</param>
	/// <param name="height">The height of the area you want.</param>
	/// <param name="style">An optional GUIStyle to layout for. If specified, the style's padding value will be added to your sizes &amp; its margin value will be used for spacing.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>The rectanlge to put your control in.</para>
	/// </returns>
	public static Rect GetRect(float width, float height)
	{
		return DoGetRect(width, width, height, height, GUIStyle.none, null);
	}

	/// <summary>
	///   <para>Reserve layout space for a rectangle with a fixed content area.</para>
	/// </summary>
	/// <param name="width">The width of the area you want.</param>
	/// <param name="height">The height of the area you want.</param>
	/// <param name="style">An optional GUIStyle to layout for. If specified, the style's padding value will be added to your sizes &amp; its margin value will be used for spacing.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>The rectanlge to put your control in.</para>
	/// </returns>
	public static Rect GetRect(float width, float height, GUIStyle style)
	{
		return DoGetRect(width, width, height, height, style, null);
	}

	/// <summary>
	///   <para>Reserve layout space for a rectangle with a fixed content area.</para>
	/// </summary>
	/// <param name="width">The width of the area you want.</param>
	/// <param name="height">The height of the area you want.</param>
	/// <param name="style">An optional GUIStyle to layout for. If specified, the style's padding value will be added to your sizes &amp; its margin value will be used for spacing.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>The rectanlge to put your control in.</para>
	/// </returns>
	public static Rect GetRect(float width, float height, params GUILayoutOption[] options)
	{
		return DoGetRect(width, width, height, height, GUIStyle.none, options);
	}

	/// <summary>
	///   <para>Reserve layout space for a rectangle with a fixed content area.</para>
	/// </summary>
	/// <param name="width">The width of the area you want.</param>
	/// <param name="height">The height of the area you want.</param>
	/// <param name="style">An optional GUIStyle to layout for. If specified, the style's padding value will be added to your sizes &amp; its margin value will be used for spacing.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>The rectanlge to put your control in.</para>
	/// </returns>
	public static Rect GetRect(float width, float height, GUIStyle style, params GUILayoutOption[] options)
	{
		return DoGetRect(width, width, height, height, style, options);
	}

	/// <summary>
	///   <para>Reserve layout space for a flexible rect.</para>
	/// </summary>
	/// <param name="minWidth">The minimum width of the area passed back.</param>
	/// <param name="maxWidth">The maximum width of the area passed back.</param>
	/// <param name="minHeight">The minimum width of the area passed back.</param>
	/// <param name="maxHeight">The maximum width of the area passed back.</param>
	/// <param name="style">An optional style. If specified, the style's padding value will be added to the sizes requested &amp; the style's margin values will be used for spacing.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>A rectangle with size between minWidth &amp; maxWidth on both axes.</para>
	/// </returns>
	public static Rect GetRect(float minWidth, float maxWidth, float minHeight, float maxHeight)
	{
		return DoGetRect(minWidth, maxWidth, minHeight, maxHeight, GUIStyle.none, null);
	}

	/// <summary>
	///   <para>Reserve layout space for a flexible rect.</para>
	/// </summary>
	/// <param name="minWidth">The minimum width of the area passed back.</param>
	/// <param name="maxWidth">The maximum width of the area passed back.</param>
	/// <param name="minHeight">The minimum width of the area passed back.</param>
	/// <param name="maxHeight">The maximum width of the area passed back.</param>
	/// <param name="style">An optional style. If specified, the style's padding value will be added to the sizes requested &amp; the style's margin values will be used for spacing.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>A rectangle with size between minWidth &amp; maxWidth on both axes.</para>
	/// </returns>
	public static Rect GetRect(float minWidth, float maxWidth, float minHeight, float maxHeight, GUIStyle style)
	{
		return DoGetRect(minWidth, maxWidth, minHeight, maxHeight, style, null);
	}

	/// <summary>
	///   <para>Reserve layout space for a flexible rect.</para>
	/// </summary>
	/// <param name="minWidth">The minimum width of the area passed back.</param>
	/// <param name="maxWidth">The maximum width of the area passed back.</param>
	/// <param name="minHeight">The minimum width of the area passed back.</param>
	/// <param name="maxHeight">The maximum width of the area passed back.</param>
	/// <param name="style">An optional style. If specified, the style's padding value will be added to the sizes requested &amp; the style's margin values will be used for spacing.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>A rectangle with size between minWidth &amp; maxWidth on both axes.</para>
	/// </returns>
	public static Rect GetRect(float minWidth, float maxWidth, float minHeight, float maxHeight, params GUILayoutOption[] options)
	{
		return DoGetRect(minWidth, maxWidth, minHeight, maxHeight, GUIStyle.none, options);
	}

	/// <summary>
	///   <para>Reserve layout space for a flexible rect.</para>
	/// </summary>
	/// <param name="minWidth">The minimum width of the area passed back.</param>
	/// <param name="maxWidth">The maximum width of the area passed back.</param>
	/// <param name="minHeight">The minimum width of the area passed back.</param>
	/// <param name="maxHeight">The maximum width of the area passed back.</param>
	/// <param name="style">An optional style. If specified, the style's padding value will be added to the sizes requested &amp; the style's margin values will be used for spacing.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>A rectangle with size between minWidth &amp; maxWidth on both axes.</para>
	/// </returns>
	public static Rect GetRect(float minWidth, float maxWidth, float minHeight, float maxHeight, GUIStyle style, params GUILayoutOption[] options)
	{
		return DoGetRect(minWidth, maxWidth, minHeight, maxHeight, style, options);
	}

	private static Rect DoGetRect(float minWidth, float maxWidth, float minHeight, float maxHeight, GUIStyle style, GUILayoutOption[] options)
	{
		switch (Event.current.type)
		{
		case EventType.Layout:
			current.topLevel.Add(new GUILayoutEntry(minWidth, maxWidth, minHeight, maxHeight, style, options));
			return kDummyRect;
		case EventType.Used:
			return kDummyRect;
		default:
			return current.topLevel.GetNext().rect;
		}
	}

	/// <summary>
	///   <para>Get the rectangle last used by GUILayout for a control.</para>
	/// </summary>
	/// <returns>
	///   <para>The last used rectangle.</para>
	/// </returns>
	public static Rect GetLastRect()
	{
		EventType type = Event.current.type;
		if (type == EventType.Layout || type == EventType.Used)
		{
			return kDummyRect;
		}
		return current.topLevel.GetLast();
	}

	/// <summary>
	///   <para>Reserve layout space for a rectangle with a specific aspect ratio.</para>
	/// </summary>
	/// <param name="aspect">The aspect ratio of the element (width / height).</param>
	/// <param name="style">An optional style. If specified, the style's padding value will be added to the sizes of the returned rectangle &amp; the style's margin values will be used for spacing.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>The rect for the control.</para>
	/// </returns>
	public static Rect GetAspectRect(float aspect)
	{
		return DoGetAspectRect(aspect, null);
	}

	/// <summary>
	///   <para>Reserve layout space for a rectangle with a specific aspect ratio.</para>
	/// </summary>
	/// <param name="aspect">The aspect ratio of the element (width / height).</param>
	/// <param name="style">An optional style. If specified, the style's padding value will be added to the sizes of the returned rectangle &amp; the style's margin values will be used for spacing.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>The rect for the control.</para>
	/// </returns>
	public static Rect GetAspectRect(float aspect, GUIStyle style)
	{
		return DoGetAspectRect(aspect, null);
	}

	/// <summary>
	///   <para>Reserve layout space for a rectangle with a specific aspect ratio.</para>
	/// </summary>
	/// <param name="aspect">The aspect ratio of the element (width / height).</param>
	/// <param name="style">An optional style. If specified, the style's padding value will be added to the sizes of the returned rectangle &amp; the style's margin values will be used for spacing.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>The rect for the control.</para>
	/// </returns>
	public static Rect GetAspectRect(float aspect, params GUILayoutOption[] options)
	{
		return DoGetAspectRect(aspect, options);
	}

	/// <summary>
	///   <para>Reserve layout space for a rectangle with a specific aspect ratio.</para>
	/// </summary>
	/// <param name="aspect">The aspect ratio of the element (width / height).</param>
	/// <param name="style">An optional style. If specified, the style's padding value will be added to the sizes of the returned rectangle &amp; the style's margin values will be used for spacing.</param>
	/// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.&lt;br&gt;
	/// See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight,
	/// GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
	/// <returns>
	///   <para>The rect for the control.</para>
	/// </returns>
	public static Rect GetAspectRect(float aspect, GUIStyle style, params GUILayoutOption[] options)
	{
		return DoGetAspectRect(aspect, options);
	}

	private static Rect DoGetAspectRect(float aspect, GUILayoutOption[] options)
	{
		switch (Event.current.type)
		{
		case EventType.Layout:
			current.topLevel.Add(new GUIAspectSizer(aspect, options));
			return kDummyRect;
		case EventType.Used:
			return kDummyRect;
		default:
			return current.topLevel.GetNext().rect;
		}
	}
}

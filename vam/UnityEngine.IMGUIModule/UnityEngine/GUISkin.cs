using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Defines how GUI looks and behaves.</para>
/// </summary>
[Serializable]
[ExecuteInEditMode]
[RequiredByNativeCode]
[AssetFileNameExtension("guiskin", new string[] { })]
public sealed class GUISkin : ScriptableObject
{
	internal delegate void SkinChangedDelegate();

	[SerializeField]
	private Font m_Font;

	[SerializeField]
	private GUIStyle m_box;

	[SerializeField]
	private GUIStyle m_button;

	[SerializeField]
	private GUIStyle m_toggle;

	[SerializeField]
	private GUIStyle m_label;

	[SerializeField]
	private GUIStyle m_textField;

	[SerializeField]
	private GUIStyle m_textArea;

	[SerializeField]
	private GUIStyle m_window;

	[SerializeField]
	private GUIStyle m_horizontalSlider;

	[SerializeField]
	private GUIStyle m_horizontalSliderThumb;

	[SerializeField]
	private GUIStyle m_verticalSlider;

	[SerializeField]
	private GUIStyle m_verticalSliderThumb;

	[SerializeField]
	private GUIStyle m_horizontalScrollbar;

	[SerializeField]
	private GUIStyle m_horizontalScrollbarThumb;

	[SerializeField]
	private GUIStyle m_horizontalScrollbarLeftButton;

	[SerializeField]
	private GUIStyle m_horizontalScrollbarRightButton;

	[SerializeField]
	private GUIStyle m_verticalScrollbar;

	[SerializeField]
	private GUIStyle m_verticalScrollbarThumb;

	[SerializeField]
	private GUIStyle m_verticalScrollbarUpButton;

	[SerializeField]
	private GUIStyle m_verticalScrollbarDownButton;

	[SerializeField]
	private GUIStyle m_ScrollView;

	[SerializeField]
	internal GUIStyle[] m_CustomStyles;

	[SerializeField]
	private GUISettings m_Settings = new GUISettings();

	internal static GUIStyle ms_Error;

	private Dictionary<string, GUIStyle> m_Styles = null;

	internal static SkinChangedDelegate m_SkinChanged;

	internal static GUISkin current;

	/// <summary>
	///   <para>The default font to use for all styles.</para>
	/// </summary>
	public Font font
	{
		get
		{
			return m_Font;
		}
		set
		{
			m_Font = value;
			if (current == this)
			{
				GUIStyle.SetDefaultFont(m_Font);
			}
			Apply();
		}
	}

	/// <summary>
	///   <para>Style used by default for GUI.Box controls.</para>
	/// </summary>
	public GUIStyle box
	{
		get
		{
			return m_box;
		}
		set
		{
			m_box = value;
			Apply();
		}
	}

	/// <summary>
	///   <para>Style used by default for GUI.Label controls.</para>
	/// </summary>
	public GUIStyle label
	{
		get
		{
			return m_label;
		}
		set
		{
			m_label = value;
			Apply();
		}
	}

	/// <summary>
	///   <para>Style used by default for GUI.TextField controls.</para>
	/// </summary>
	public GUIStyle textField
	{
		get
		{
			return m_textField;
		}
		set
		{
			m_textField = value;
			Apply();
		}
	}

	/// <summary>
	///   <para>Style used by default for GUI.TextArea controls.</para>
	/// </summary>
	public GUIStyle textArea
	{
		get
		{
			return m_textArea;
		}
		set
		{
			m_textArea = value;
			Apply();
		}
	}

	/// <summary>
	///   <para>Style used by default for GUI.Button controls.</para>
	/// </summary>
	public GUIStyle button
	{
		get
		{
			return m_button;
		}
		set
		{
			m_button = value;
			Apply();
		}
	}

	/// <summary>
	///   <para>Style used by default for GUI.Toggle controls.</para>
	/// </summary>
	public GUIStyle toggle
	{
		get
		{
			return m_toggle;
		}
		set
		{
			m_toggle = value;
			Apply();
		}
	}

	/// <summary>
	///   <para>Style used by default for Window controls (SA GUI.Window).</para>
	/// </summary>
	public GUIStyle window
	{
		get
		{
			return m_window;
		}
		set
		{
			m_window = value;
			Apply();
		}
	}

	/// <summary>
	///   <para>Style used by default for the background part of GUI.HorizontalSlider controls.</para>
	/// </summary>
	public GUIStyle horizontalSlider
	{
		get
		{
			return m_horizontalSlider;
		}
		set
		{
			m_horizontalSlider = value;
			Apply();
		}
	}

	/// <summary>
	///   <para>Style used by default for the thumb that is dragged in GUI.HorizontalSlider controls.</para>
	/// </summary>
	public GUIStyle horizontalSliderThumb
	{
		get
		{
			return m_horizontalSliderThumb;
		}
		set
		{
			m_horizontalSliderThumb = value;
			Apply();
		}
	}

	/// <summary>
	///   <para>Style used by default for the background part of GUI.VerticalSlider controls.</para>
	/// </summary>
	public GUIStyle verticalSlider
	{
		get
		{
			return m_verticalSlider;
		}
		set
		{
			m_verticalSlider = value;
			Apply();
		}
	}

	/// <summary>
	///   <para>Style used by default for the thumb that is dragged in GUI.VerticalSlider controls.</para>
	/// </summary>
	public GUIStyle verticalSliderThumb
	{
		get
		{
			return m_verticalSliderThumb;
		}
		set
		{
			m_verticalSliderThumb = value;
			Apply();
		}
	}

	/// <summary>
	///   <para>Style used by default for the background part of GUI.HorizontalScrollbar controls.</para>
	/// </summary>
	public GUIStyle horizontalScrollbar
	{
		get
		{
			return m_horizontalScrollbar;
		}
		set
		{
			m_horizontalScrollbar = value;
			Apply();
		}
	}

	/// <summary>
	///   <para>Style used by default for the thumb that is dragged in GUI.HorizontalScrollbar controls.</para>
	/// </summary>
	public GUIStyle horizontalScrollbarThumb
	{
		get
		{
			return m_horizontalScrollbarThumb;
		}
		set
		{
			m_horizontalScrollbarThumb = value;
			Apply();
		}
	}

	/// <summary>
	///   <para>Style used by default for the left button on GUI.HorizontalScrollbar controls.</para>
	/// </summary>
	public GUIStyle horizontalScrollbarLeftButton
	{
		get
		{
			return m_horizontalScrollbarLeftButton;
		}
		set
		{
			m_horizontalScrollbarLeftButton = value;
			Apply();
		}
	}

	/// <summary>
	///   <para>Style used by default for the right button on GUI.HorizontalScrollbar controls.</para>
	/// </summary>
	public GUIStyle horizontalScrollbarRightButton
	{
		get
		{
			return m_horizontalScrollbarRightButton;
		}
		set
		{
			m_horizontalScrollbarRightButton = value;
			Apply();
		}
	}

	/// <summary>
	///   <para>Style used by default for the background part of GUI.VerticalScrollbar controls.</para>
	/// </summary>
	public GUIStyle verticalScrollbar
	{
		get
		{
			return m_verticalScrollbar;
		}
		set
		{
			m_verticalScrollbar = value;
			Apply();
		}
	}

	/// <summary>
	///   <para>Style used by default for the thumb that is dragged in GUI.VerticalScrollbar controls.</para>
	/// </summary>
	public GUIStyle verticalScrollbarThumb
	{
		get
		{
			return m_verticalScrollbarThumb;
		}
		set
		{
			m_verticalScrollbarThumb = value;
			Apply();
		}
	}

	/// <summary>
	///   <para>Style used by default for the up button on GUI.VerticalScrollbar controls.</para>
	/// </summary>
	public GUIStyle verticalScrollbarUpButton
	{
		get
		{
			return m_verticalScrollbarUpButton;
		}
		set
		{
			m_verticalScrollbarUpButton = value;
			Apply();
		}
	}

	/// <summary>
	///   <para>Style used by default for the down button on GUI.VerticalScrollbar controls.</para>
	/// </summary>
	public GUIStyle verticalScrollbarDownButton
	{
		get
		{
			return m_verticalScrollbarDownButton;
		}
		set
		{
			m_verticalScrollbarDownButton = value;
			Apply();
		}
	}

	/// <summary>
	///   <para>Style used by default for the background of ScrollView controls (see GUI.BeginScrollView).</para>
	/// </summary>
	public GUIStyle scrollView
	{
		get
		{
			return m_ScrollView;
		}
		set
		{
			m_ScrollView = value;
			Apply();
		}
	}

	/// <summary>
	///   <para>Array of GUI styles for specific needs.</para>
	/// </summary>
	public GUIStyle[] customStyles
	{
		get
		{
			return m_CustomStyles;
		}
		set
		{
			m_CustomStyles = value;
			Apply();
		}
	}

	/// <summary>
	///   <para>Generic settings for how controls should behave with this skin.</para>
	/// </summary>
	public GUISettings settings => m_Settings;

	internal static GUIStyle error
	{
		get
		{
			if (ms_Error == null)
			{
				ms_Error = new GUIStyle();
			}
			return ms_Error;
		}
	}

	public GUISkin()
	{
		m_CustomStyles = new GUIStyle[1];
	}

	internal void OnEnable()
	{
		Apply();
	}

	internal static void CleanupRoots()
	{
		current = null;
		ms_Error = null;
	}

	internal void Apply()
	{
		if (m_CustomStyles == null)
		{
			Debug.Log("custom styles is null");
		}
		BuildStyleCache();
	}

	private void BuildStyleCache()
	{
		if (m_box == null)
		{
			m_box = new GUIStyle();
		}
		if (m_button == null)
		{
			m_button = new GUIStyle();
		}
		if (m_toggle == null)
		{
			m_toggle = new GUIStyle();
		}
		if (m_label == null)
		{
			m_label = new GUIStyle();
		}
		if (m_window == null)
		{
			m_window = new GUIStyle();
		}
		if (m_textField == null)
		{
			m_textField = new GUIStyle();
		}
		if (m_textArea == null)
		{
			m_textArea = new GUIStyle();
		}
		if (m_horizontalSlider == null)
		{
			m_horizontalSlider = new GUIStyle();
		}
		if (m_horizontalSliderThumb == null)
		{
			m_horizontalSliderThumb = new GUIStyle();
		}
		if (m_verticalSlider == null)
		{
			m_verticalSlider = new GUIStyle();
		}
		if (m_verticalSliderThumb == null)
		{
			m_verticalSliderThumb = new GUIStyle();
		}
		if (m_horizontalScrollbar == null)
		{
			m_horizontalScrollbar = new GUIStyle();
		}
		if (m_horizontalScrollbarThumb == null)
		{
			m_horizontalScrollbarThumb = new GUIStyle();
		}
		if (m_horizontalScrollbarLeftButton == null)
		{
			m_horizontalScrollbarLeftButton = new GUIStyle();
		}
		if (m_horizontalScrollbarRightButton == null)
		{
			m_horizontalScrollbarRightButton = new GUIStyle();
		}
		if (m_verticalScrollbar == null)
		{
			m_verticalScrollbar = new GUIStyle();
		}
		if (m_verticalScrollbarThumb == null)
		{
			m_verticalScrollbarThumb = new GUIStyle();
		}
		if (m_verticalScrollbarUpButton == null)
		{
			m_verticalScrollbarUpButton = new GUIStyle();
		}
		if (m_verticalScrollbarDownButton == null)
		{
			m_verticalScrollbarDownButton = new GUIStyle();
		}
		if (m_ScrollView == null)
		{
			m_ScrollView = new GUIStyle();
		}
		m_Styles = new Dictionary<string, GUIStyle>(StringComparer.OrdinalIgnoreCase);
		m_Styles["box"] = m_box;
		m_box.name = "box";
		m_Styles["button"] = m_button;
		m_button.name = "button";
		m_Styles["toggle"] = m_toggle;
		m_toggle.name = "toggle";
		m_Styles["label"] = m_label;
		m_label.name = "label";
		m_Styles["window"] = m_window;
		m_window.name = "window";
		m_Styles["textfield"] = m_textField;
		m_textField.name = "textfield";
		m_Styles["textarea"] = m_textArea;
		m_textArea.name = "textarea";
		m_Styles["horizontalslider"] = m_horizontalSlider;
		m_horizontalSlider.name = "horizontalslider";
		m_Styles["horizontalsliderthumb"] = m_horizontalSliderThumb;
		m_horizontalSliderThumb.name = "horizontalsliderthumb";
		m_Styles["verticalslider"] = m_verticalSlider;
		m_verticalSlider.name = "verticalslider";
		m_Styles["verticalsliderthumb"] = m_verticalSliderThumb;
		m_verticalSliderThumb.name = "verticalsliderthumb";
		m_Styles["horizontalscrollbar"] = m_horizontalScrollbar;
		m_horizontalScrollbar.name = "horizontalscrollbar";
		m_Styles["horizontalscrollbarthumb"] = m_horizontalScrollbarThumb;
		m_horizontalScrollbarThumb.name = "horizontalscrollbarthumb";
		m_Styles["horizontalscrollbarleftbutton"] = m_horizontalScrollbarLeftButton;
		m_horizontalScrollbarLeftButton.name = "horizontalscrollbarleftbutton";
		m_Styles["horizontalscrollbarrightbutton"] = m_horizontalScrollbarRightButton;
		m_horizontalScrollbarRightButton.name = "horizontalscrollbarrightbutton";
		m_Styles["verticalscrollbar"] = m_verticalScrollbar;
		m_verticalScrollbar.name = "verticalscrollbar";
		m_Styles["verticalscrollbarthumb"] = m_verticalScrollbarThumb;
		m_verticalScrollbarThumb.name = "verticalscrollbarthumb";
		m_Styles["verticalscrollbarupbutton"] = m_verticalScrollbarUpButton;
		m_verticalScrollbarUpButton.name = "verticalscrollbarupbutton";
		m_Styles["verticalscrollbardownbutton"] = m_verticalScrollbarDownButton;
		m_verticalScrollbarDownButton.name = "verticalscrollbardownbutton";
		m_Styles["scrollview"] = m_ScrollView;
		m_ScrollView.name = "scrollview";
		if (m_CustomStyles != null)
		{
			for (int i = 0; i < m_CustomStyles.Length; i++)
			{
				if (m_CustomStyles[i] != null)
				{
					m_Styles[m_CustomStyles[i].name] = m_CustomStyles[i];
				}
			}
		}
		error.stretchHeight = true;
		error.normal.textColor = Color.red;
	}

	/// <summary>
	///   <para>Get a named GUIStyle.</para>
	/// </summary>
	/// <param name="styleName"></param>
	public GUIStyle GetStyle(string styleName)
	{
		GUIStyle gUIStyle = FindStyle(styleName);
		if (gUIStyle != null)
		{
			return gUIStyle;
		}
		Debug.LogWarning("Unable to find style '" + styleName + "' in skin '" + base.name + "' " + Event.current.type);
		return error;
	}

	/// <summary>
	///   <para>Try to search for a GUIStyle. This functions returns NULL and does not give an error.</para>
	/// </summary>
	/// <param name="styleName"></param>
	public GUIStyle FindStyle(string styleName)
	{
		if (this == null)
		{
			Debug.LogError("GUISkin is NULL");
			return null;
		}
		if (m_Styles == null)
		{
			BuildStyleCache();
		}
		if (m_Styles.TryGetValue(styleName, out var value))
		{
			return value;
		}
		return null;
	}

	internal void MakeCurrent()
	{
		current = this;
		GUIStyle.SetDefaultFont(font);
		if (m_SkinChanged != null)
		{
			m_SkinChanged();
		}
	}

	public IEnumerator GetEnumerator()
	{
		if (m_Styles == null)
		{
			BuildStyleCache();
		}
		return m_Styles.Values.GetEnumerator();
	}
}

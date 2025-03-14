using System;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine;

/// <summary>
///   <para>The contents of a GUI element.</para>
/// </summary>
[Serializable]
[StructLayout(LayoutKind.Sequential)]
public class GUIContent
{
	[SerializeField]
	private string m_Text = string.Empty;

	[SerializeField]
	private Texture m_Image;

	[SerializeField]
	private string m_Tooltip = string.Empty;

	private static readonly GUIContent s_Text = new GUIContent();

	private static readonly GUIContent s_Image = new GUIContent();

	private static readonly GUIContent s_TextImage = new GUIContent();

	/// <summary>
	///   <para>Shorthand for empty content.</para>
	/// </summary>
	public static GUIContent none = new GUIContent("");

	/// <summary>
	///   <para>The text contained.</para>
	/// </summary>
	public string text
	{
		get
		{
			return m_Text;
		}
		set
		{
			m_Text = value;
		}
	}

	/// <summary>
	///   <para>The icon image contained.</para>
	/// </summary>
	public Texture image
	{
		get
		{
			return m_Image;
		}
		set
		{
			m_Image = value;
		}
	}

	/// <summary>
	///   <para>The tooltip of this element.</para>
	/// </summary>
	public string tooltip
	{
		get
		{
			return m_Tooltip;
		}
		set
		{
			m_Tooltip = value;
		}
	}

	internal int hash
	{
		get
		{
			int result = 0;
			if (!string.IsNullOrEmpty(m_Text))
			{
				result = m_Text.GetHashCode() * 37;
			}
			return result;
		}
	}

	/// <summary>
	///   <para>Constructor for GUIContent in all shapes and sizes.</para>
	/// </summary>
	public GUIContent()
	{
	}

	/// <summary>
	///   <para>Build a GUIContent object containing only text.</para>
	/// </summary>
	/// <param name="text"></param>
	public GUIContent(string text)
		: this(text, null, string.Empty)
	{
	}

	/// <summary>
	///   <para>Build a GUIContent object containing only an image.</para>
	/// </summary>
	/// <param name="image"></param>
	public GUIContent(Texture image)
		: this(string.Empty, image, string.Empty)
	{
	}

	/// <summary>
	///   <para>Build a GUIContent object containing both text and an image.</para>
	/// </summary>
	/// <param name="text"></param>
	/// <param name="image"></param>
	public GUIContent(string text, Texture image)
		: this(text, image, string.Empty)
	{
	}

	/// <summary>
	///   <para>Build a GUIContent containing some text. When the user hovers the mouse over it, the global GUI.tooltip is set to the tooltip.</para>
	/// </summary>
	/// <param name="text"></param>
	/// <param name="tooltip"></param>
	public GUIContent(string text, string tooltip)
		: this(text, null, tooltip)
	{
	}

	/// <summary>
	///   <para>Build a GUIContent containing an image. When the user hovers the mouse over it, the global GUI.tooltip is set to the tooltip.</para>
	/// </summary>
	/// <param name="image"></param>
	/// <param name="tooltip"></param>
	public GUIContent(Texture image, string tooltip)
		: this(string.Empty, image, tooltip)
	{
	}

	/// <summary>
	///   <para>Build a GUIContent that contains both text, an image and has a tooltip defined. When the user hovers the mouse over it, the global GUI.tooltip is set to the tooltip.</para>
	/// </summary>
	/// <param name="text"></param>
	/// <param name="image"></param>
	/// <param name="tooltip"></param>
	public GUIContent(string text, Texture image, string tooltip)
	{
		this.text = text;
		this.image = image;
		this.tooltip = tooltip;
	}

	/// <summary>
	///   <para>Build a GUIContent as a copy of another GUIContent.</para>
	/// </summary>
	/// <param name="src"></param>
	public GUIContent(GUIContent src)
	{
		text = src.m_Text;
		image = src.m_Image;
		tooltip = src.m_Tooltip;
	}

	internal static GUIContent Temp(string t)
	{
		s_Text.m_Text = t;
		s_Text.m_Tooltip = string.Empty;
		return s_Text;
	}

	internal static GUIContent Temp(string t, string tooltip)
	{
		s_Text.m_Text = t;
		s_Text.m_Tooltip = tooltip;
		return s_Text;
	}

	internal static GUIContent Temp(Texture i)
	{
		s_Image.m_Image = i;
		s_Image.m_Tooltip = string.Empty;
		return s_Image;
	}

	internal static GUIContent Temp(Texture i, string tooltip)
	{
		s_Image.m_Image = i;
		s_Image.m_Tooltip = tooltip;
		return s_Image;
	}

	internal static GUIContent Temp(string t, Texture i)
	{
		s_TextImage.m_Text = t;
		s_TextImage.m_Image = i;
		return s_TextImage;
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static void ClearStaticCache()
	{
		s_Text.m_Text = null;
		s_Text.m_Tooltip = string.Empty;
		s_Image.m_Image = null;
		s_Image.m_Tooltip = string.Empty;
		s_TextImage.m_Text = null;
		s_TextImage.m_Image = null;
	}

	internal static GUIContent[] Temp(string[] texts)
	{
		GUIContent[] array = new GUIContent[texts.Length];
		for (int i = 0; i < texts.Length; i++)
		{
			array[i] = new GUIContent(texts[i]);
		}
		return array;
	}

	internal static GUIContent[] Temp(Texture[] images)
	{
		GUIContent[] array = new GUIContent[images.Length];
		for (int i = 0; i < images.Length; i++)
		{
			array[i] = new GUIContent(images[i]);
		}
		return array;
	}
}

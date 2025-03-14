using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Class that can be used to generate text for rendering.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[UsedByNativeCode]
public sealed class TextGenerator : IDisposable
{
	internal IntPtr m_Ptr;

	private string m_LastString;

	private TextGenerationSettings m_LastSettings;

	private bool m_HasGenerated;

	private TextGenerationError m_LastValid;

	private readonly List<UIVertex> m_Verts;

	private readonly List<UICharInfo> m_Characters;

	private readonly List<UILineInfo> m_Lines;

	private bool m_CachedVerts;

	private bool m_CachedCharacters;

	private bool m_CachedLines;

	/// <summary>
	///   <para>Extents of the generated text in rect format.</para>
	/// </summary>
	public Rect rectExtents
	{
		get
		{
			INTERNAL_get_rectExtents(out var value);
			return value;
		}
	}

	/// <summary>
	///   <para>Number of vertices generated.</para>
	/// </summary>
	public extern int vertexCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>The number of characters that have been generated.</para>
	/// </summary>
	public extern int characterCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>The number of characters that have been generated and are included in the visible lines.</para>
	/// </summary>
	public int characterCountVisible => characterCount - 1;

	/// <summary>
	///   <para>Number of text lines generated.</para>
	/// </summary>
	public extern int lineCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>The size of the font that was found if using best fit mode.</para>
	/// </summary>
	public extern int fontSizeUsedForBestFit
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Array of generated vertices.</para>
	/// </summary>
	public IList<UIVertex> verts
	{
		get
		{
			if (!m_CachedVerts)
			{
				GetVertices(m_Verts);
				m_CachedVerts = true;
			}
			return m_Verts;
		}
	}

	/// <summary>
	///   <para>Array of generated characters.</para>
	/// </summary>
	public IList<UICharInfo> characters
	{
		get
		{
			if (!m_CachedCharacters)
			{
				GetCharacters(m_Characters);
				m_CachedCharacters = true;
			}
			return m_Characters;
		}
	}

	/// <summary>
	///   <para>Information about each generated text line.</para>
	/// </summary>
	public IList<UILineInfo> lines
	{
		get
		{
			if (!m_CachedLines)
			{
				GetLines(m_Lines);
				m_CachedLines = true;
			}
			return m_Lines;
		}
	}

	/// <summary>
	///   <para>Create a TextGenerator.</para>
	/// </summary>
	/// <param name="initialCapacity"></param>
	public TextGenerator()
		: this(50)
	{
	}

	/// <summary>
	///   <para>Create a TextGenerator.</para>
	/// </summary>
	/// <param name="initialCapacity"></param>
	public TextGenerator(int initialCapacity)
	{
		m_Verts = new List<UIVertex>((initialCapacity + 1) * 4);
		m_Characters = new List<UICharInfo>(initialCapacity + 1);
		m_Lines = new List<UILineInfo>(20);
		Init();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void Init();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	private extern void Dispose_cpp();

	internal bool Populate_Internal(string str, Font font, Color color, int fontSize, float scaleFactor, float lineSpacing, FontStyle style, bool richText, bool resizeTextForBestFit, int resizeTextMinSize, int resizeTextMaxSize, VerticalWrapMode verticalOverFlow, HorizontalWrapMode horizontalOverflow, bool updateBounds, TextAnchor anchor, Vector2 extents, Vector2 pivot, bool generateOutOfBounds, bool alignByGeometry, out TextGenerationError error)
	{
		uint error2 = 0u;
		if (font == null)
		{
			error = TextGenerationError.NoFont;
			return false;
		}
		bool result = Populate_Internal_cpp(str, font, color, fontSize, scaleFactor, lineSpacing, style, richText, resizeTextForBestFit, resizeTextMinSize, resizeTextMaxSize, (int)verticalOverFlow, (int)horizontalOverflow, updateBounds, anchor, extents.x, extents.y, pivot.x, pivot.y, generateOutOfBounds, alignByGeometry, out error2);
		error = (TextGenerationError)error2;
		return result;
	}

	internal bool Populate_Internal_cpp(string str, Font font, Color color, int fontSize, float scaleFactor, float lineSpacing, FontStyle style, bool richText, bool resizeTextForBestFit, int resizeTextMinSize, int resizeTextMaxSize, int verticalOverFlow, int horizontalOverflow, bool updateBounds, TextAnchor anchor, float extentsX, float extentsY, float pivotX, float pivotY, bool generateOutOfBounds, bool alignByGeometry, out uint error)
	{
		return INTERNAL_CALL_Populate_Internal_cpp(this, str, font, ref color, fontSize, scaleFactor, lineSpacing, style, richText, resizeTextForBestFit, resizeTextMinSize, resizeTextMaxSize, verticalOverFlow, horizontalOverflow, updateBounds, anchor, extentsX, extentsY, pivotX, pivotY, generateOutOfBounds, alignByGeometry, out error);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool INTERNAL_CALL_Populate_Internal_cpp(TextGenerator self, string str, Font font, ref Color color, int fontSize, float scaleFactor, float lineSpacing, FontStyle style, bool richText, bool resizeTextForBestFit, int resizeTextMinSize, int resizeTextMaxSize, int verticalOverFlow, int horizontalOverflow, bool updateBounds, TextAnchor anchor, float extentsX, float extentsY, float pivotX, float pivotY, bool generateOutOfBounds, bool alignByGeometry, out uint error);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void INTERNAL_get_rectExtents(out Rect value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void GetVerticesInternal(object vertices);

	/// <summary>
	///   <para>Returns the current UILineInfo.</para>
	/// </summary>
	/// <returns>
	///   <para>Vertices.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern UIVertex[] GetVerticesArray();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void GetCharactersInternal(object characters);

	/// <summary>
	///   <para>Returns the current UICharInfo.</para>
	/// </summary>
	/// <returns>
	///   <para>Character information.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern UICharInfo[] GetCharactersArray();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private extern void GetLinesInternal(object lines);

	/// <summary>
	///   <para>Returns the current UILineInfo.</para>
	/// </summary>
	/// <returns>
	///   <para>Line information.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern UILineInfo[] GetLinesArray();

	~TextGenerator()
	{
		((IDisposable)this).Dispose();
	}

	void IDisposable.Dispose()
	{
		Dispose_cpp();
	}

	private TextGenerationSettings ValidatedSettings(TextGenerationSettings settings)
	{
		if (settings.font != null && settings.font.dynamic)
		{
			return settings;
		}
		if (settings.fontSize != 0 || settings.fontStyle != 0)
		{
			if (settings.font != null)
			{
				Debug.LogWarningFormat(settings.font, "Font size and style overrides are only supported for dynamic fonts. Font '{0}' is not dynamic.", settings.font.name);
			}
			settings.fontSize = 0;
			settings.fontStyle = FontStyle.Normal;
		}
		if (settings.resizeTextForBestFit)
		{
			if (settings.font != null)
			{
				Debug.LogWarningFormat(settings.font, "BestFit is only supported for dynamic fonts. Font '{0}' is not dynamic.", settings.font.name);
			}
			settings.resizeTextForBestFit = false;
		}
		return settings;
	}

	/// <summary>
	///   <para>Mark the text generator as invalid. This will force a full text generation the next time Populate is called.</para>
	/// </summary>
	public void Invalidate()
	{
		m_HasGenerated = false;
	}

	public void GetCharacters(List<UICharInfo> characters)
	{
		GetCharactersInternal(characters);
	}

	public void GetLines(List<UILineInfo> lines)
	{
		GetLinesInternal(lines);
	}

	public void GetVertices(List<UIVertex> vertices)
	{
		GetVerticesInternal(vertices);
	}

	/// <summary>
	///   <para>Given a string and settings, returns the preferred width for a container that would hold this text.</para>
	/// </summary>
	/// <param name="str">Generation text.</param>
	/// <param name="settings">Settings for generation.</param>
	/// <returns>
	///   <para>Preferred width.</para>
	/// </returns>
	public float GetPreferredWidth(string str, TextGenerationSettings settings)
	{
		settings.horizontalOverflow = HorizontalWrapMode.Overflow;
		settings.verticalOverflow = VerticalWrapMode.Overflow;
		settings.updateBounds = true;
		Populate(str, settings);
		return rectExtents.width;
	}

	/// <summary>
	///   <para>Given a string and settings, returns the preferred height for a container that would hold this text.</para>
	/// </summary>
	/// <param name="str">Generation text.</param>
	/// <param name="settings">Settings for generation.</param>
	/// <returns>
	///   <para>Preferred height.</para>
	/// </returns>
	public float GetPreferredHeight(string str, TextGenerationSettings settings)
	{
		settings.verticalOverflow = VerticalWrapMode.Overflow;
		settings.updateBounds = true;
		Populate(str, settings);
		return rectExtents.height;
	}

	/// <summary>
	///   <para>Will generate the vertices and other data for the given string with the given settings.</para>
	/// </summary>
	/// <param name="str">String to generate.</param>
	/// <param name="settings">Generation settings.</param>
	/// <param name="context">The object used as context of the error log message, if necessary.</param>
	/// <returns>
	///   <para>True if the generation is a success, false otherwise.</para>
	/// </returns>
	public bool PopulateWithErrors(string str, TextGenerationSettings settings, GameObject context)
	{
		TextGenerationError textGenerationError = PopulateWithError(str, settings);
		if (textGenerationError == TextGenerationError.None)
		{
			return true;
		}
		if ((textGenerationError & TextGenerationError.CustomSizeOnNonDynamicFont) != 0)
		{
			Debug.LogErrorFormat(context, "Font '{0}' is not dynamic, which is required to override its size", settings.font);
		}
		if ((textGenerationError & TextGenerationError.CustomStyleOnNonDynamicFont) != 0)
		{
			Debug.LogErrorFormat(context, "Font '{0}' is not dynamic, which is required to override its style", settings.font);
		}
		return false;
	}

	/// <summary>
	///   <para>Will generate the vertices and other data for the given string with the given settings.</para>
	/// </summary>
	/// <param name="str">String to generate.</param>
	/// <param name="settings">Settings.</param>
	public bool Populate(string str, TextGenerationSettings settings)
	{
		TextGenerationError textGenerationError = PopulateWithError(str, settings);
		return textGenerationError == TextGenerationError.None;
	}

	private TextGenerationError PopulateWithError(string str, TextGenerationSettings settings)
	{
		if (m_HasGenerated && str == m_LastString && settings.Equals(m_LastSettings))
		{
			return m_LastValid;
		}
		m_LastValid = PopulateAlways(str, settings);
		return m_LastValid;
	}

	private TextGenerationError PopulateAlways(string str, TextGenerationSettings settings)
	{
		m_LastString = str;
		m_HasGenerated = true;
		m_CachedVerts = false;
		m_CachedCharacters = false;
		m_CachedLines = false;
		m_LastSettings = settings;
		TextGenerationSettings textGenerationSettings = ValidatedSettings(settings);
		Populate_Internal(str, textGenerationSettings.font, textGenerationSettings.color, textGenerationSettings.fontSize, textGenerationSettings.scaleFactor, textGenerationSettings.lineSpacing, textGenerationSettings.fontStyle, textGenerationSettings.richText, textGenerationSettings.resizeTextForBestFit, textGenerationSettings.resizeTextMinSize, textGenerationSettings.resizeTextMaxSize, textGenerationSettings.verticalOverflow, textGenerationSettings.horizontalOverflow, textGenerationSettings.updateBounds, textGenerationSettings.textAnchor, textGenerationSettings.generationExtents, textGenerationSettings.pivot, textGenerationSettings.generateOutOfBounds, textGenerationSettings.alignByGeometry, out var error);
		m_LastValid = error;
		return error;
	}
}

using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>General settings for how the GUI behaves.</para>
/// </summary>
[Serializable]
public sealed class GUISettings
{
	[SerializeField]
	private bool m_DoubleClickSelectsWord = true;

	[SerializeField]
	private bool m_TripleClickSelectsLine = true;

	[SerializeField]
	private Color m_CursorColor = Color.white;

	[SerializeField]
	private float m_CursorFlashSpeed = -1f;

	[SerializeField]
	private Color m_SelectionColor = new Color(0.5f, 0.5f, 1f);

	/// <summary>
	///   <para>Should double-clicking select words in text fields.</para>
	/// </summary>
	public bool doubleClickSelectsWord
	{
		get
		{
			return m_DoubleClickSelectsWord;
		}
		set
		{
			m_DoubleClickSelectsWord = value;
		}
	}

	/// <summary>
	///   <para>Should triple-clicking select whole text in text fields.</para>
	/// </summary>
	public bool tripleClickSelectsLine
	{
		get
		{
			return m_TripleClickSelectsLine;
		}
		set
		{
			m_TripleClickSelectsLine = value;
		}
	}

	/// <summary>
	///   <para>The color of the cursor in text fields.</para>
	/// </summary>
	public Color cursorColor
	{
		get
		{
			return m_CursorColor;
		}
		set
		{
			m_CursorColor = value;
		}
	}

	/// <summary>
	///   <para>The speed of text field cursor flashes.</para>
	/// </summary>
	public float cursorFlashSpeed
	{
		get
		{
			if (m_CursorFlashSpeed >= 0f)
			{
				return m_CursorFlashSpeed;
			}
			return Internal_GetCursorFlashSpeed();
		}
		set
		{
			m_CursorFlashSpeed = value;
		}
	}

	/// <summary>
	///   <para>The color of the selection rect in text fields.</para>
	/// </summary>
	public Color selectionColor
	{
		get
		{
			return m_SelectionColor;
		}
		set
		{
			m_SelectionColor = value;
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern float Internal_GetCursorFlashSpeed();
}

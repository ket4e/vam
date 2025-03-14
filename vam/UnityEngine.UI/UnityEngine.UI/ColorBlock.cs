using System;
using UnityEngine.Serialization;

namespace UnityEngine.UI;

[Serializable]
public struct ColorBlock : IEquatable<ColorBlock>
{
	[FormerlySerializedAs("normalColor")]
	[SerializeField]
	private Color m_NormalColor;

	[FormerlySerializedAs("highlightedColor")]
	[FormerlySerializedAs("m_SelectedColor")]
	[SerializeField]
	private Color m_HighlightedColor;

	[FormerlySerializedAs("pressedColor")]
	[SerializeField]
	private Color m_PressedColor;

	[FormerlySerializedAs("disabledColor")]
	[SerializeField]
	private Color m_DisabledColor;

	[Range(1f, 5f)]
	[SerializeField]
	private float m_ColorMultiplier;

	[FormerlySerializedAs("fadeDuration")]
	[SerializeField]
	private float m_FadeDuration;

	public Color normalColor
	{
		get
		{
			return m_NormalColor;
		}
		set
		{
			m_NormalColor = value;
		}
	}

	public Color highlightedColor
	{
		get
		{
			return m_HighlightedColor;
		}
		set
		{
			m_HighlightedColor = value;
		}
	}

	public Color pressedColor
	{
		get
		{
			return m_PressedColor;
		}
		set
		{
			m_PressedColor = value;
		}
	}

	public Color disabledColor
	{
		get
		{
			return m_DisabledColor;
		}
		set
		{
			m_DisabledColor = value;
		}
	}

	public float colorMultiplier
	{
		get
		{
			return m_ColorMultiplier;
		}
		set
		{
			m_ColorMultiplier = value;
		}
	}

	public float fadeDuration
	{
		get
		{
			return m_FadeDuration;
		}
		set
		{
			m_FadeDuration = value;
		}
	}

	public static ColorBlock defaultColorBlock
	{
		get
		{
			ColorBlock result = default(ColorBlock);
			result.m_NormalColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			result.m_HighlightedColor = new Color32(245, 245, 245, byte.MaxValue);
			result.m_PressedColor = new Color32(200, 200, 200, byte.MaxValue);
			result.m_DisabledColor = new Color32(200, 200, 200, 128);
			result.colorMultiplier = 1f;
			result.fadeDuration = 0.1f;
			return result;
		}
	}

	public override bool Equals(object obj)
	{
		if (!(obj is ColorBlock))
		{
			return false;
		}
		return Equals((ColorBlock)obj);
	}

	public bool Equals(ColorBlock other)
	{
		return normalColor == other.normalColor && highlightedColor == other.highlightedColor && pressedColor == other.pressedColor && disabledColor == other.disabledColor && colorMultiplier == other.colorMultiplier && fadeDuration == other.fadeDuration;
	}

	public static bool operator ==(ColorBlock point1, ColorBlock point2)
	{
		return point1.Equals(point2);
	}

	public static bool operator !=(ColorBlock point1, ColorBlock point2)
	{
		return !point1.Equals(point2);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}

using UnityEngine.Experimental.UIElements.StyleSheets;

namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>A VisualElement representing a source texture.</para>
/// </summary>
public class Image : VisualElement
{
	private StyleValue<int> m_ScaleMode;

	private StyleValue<Texture> m_Image;

	private Rect m_UV;

	/// <summary>
	///   <para>The source texture of the Image element.</para>
	/// </summary>
	public StyleValue<Texture> image
	{
		get
		{
			return m_Image;
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref m_Image, value))
			{
				Dirty(ChangeType.Layout | ChangeType.Repaint);
				if (m_Image.value == null)
				{
					m_UV = new Rect(0f, 0f, 1f, 1f);
				}
			}
		}
	}

	/// <summary>
	///   <para>The source rectangle inside the texture relative to the top left corner.</para>
	/// </summary>
	public Rect sourceRect
	{
		get
		{
			return GetSourceRect();
		}
		set
		{
			CalculateUV(value);
		}
	}

	/// <summary>
	///   <para>The base texture coordinates of the Image relative to the bottom left corner.</para>
	/// </summary>
	public Rect uv
	{
		get
		{
			return m_UV;
		}
		set
		{
			m_UV = value;
		}
	}

	public StyleValue<ScaleMode> scaleMode
	{
		get
		{
			return new StyleValue<ScaleMode>((ScaleMode)m_ScaleMode.value, m_ScaleMode.specificity);
		}
		set
		{
			if (StyleValueUtils.ApplyAndCompare(ref m_ScaleMode, new StyleValue<int>((int)value.value, value.specificity)))
			{
				Dirty(ChangeType.Layout);
			}
		}
	}

	public Image()
	{
		scaleMode = ScaleMode.ScaleAndCrop;
		m_UV = new Rect(0f, 0f, 1f, 1f);
	}

	protected internal override Vector2 DoMeasure(float width, MeasureMode widthMode, float height, MeasureMode heightMode)
	{
		float x = float.NaN;
		float y = float.NaN;
		Texture specifiedValueOrDefault = image.GetSpecifiedValueOrDefault(null);
		if (specifiedValueOrDefault == null)
		{
			return new Vector2(x, y);
		}
		Rect rect = sourceRect;
		bool flag = rect != Rect.zero;
		x = ((!flag) ? ((float)specifiedValueOrDefault.width) : rect.width);
		y = ((!flag) ? ((float)specifiedValueOrDefault.height) : rect.height);
		if (widthMode == MeasureMode.AtMost)
		{
			x = Mathf.Min(x, width);
		}
		if (heightMode == MeasureMode.AtMost)
		{
			y = Mathf.Min(y, height);
		}
		return new Vector2(x, y);
	}

	internal override void DoRepaint(IStylePainter painter)
	{
		base.DoRepaint(painter);
		Texture specifiedValueOrDefault = image.GetSpecifiedValueOrDefault(null);
		if (specifiedValueOrDefault == null)
		{
			Debug.LogWarning("null texture passed to GUI.DrawTexture");
			return;
		}
		TextureStylePainterParameters textureStylePainterParameters = default(TextureStylePainterParameters);
		textureStylePainterParameters.rect = base.contentRect;
		textureStylePainterParameters.uv = uv;
		textureStylePainterParameters.texture = specifiedValueOrDefault;
		textureStylePainterParameters.color = GUI.color;
		textureStylePainterParameters.scaleMode = scaleMode;
		TextureStylePainterParameters painterParams = textureStylePainterParameters;
		painter.DrawTexture(painterParams);
	}

	protected override void OnStyleResolved(ICustomStyle elementStyle)
	{
		base.OnStyleResolved(elementStyle);
		elementStyle.ApplyCustomProperty("image", ref m_Image);
		elementStyle.ApplyCustomProperty("image-size", ref m_ScaleMode);
	}

	private void CalculateUV(Rect srcRect)
	{
		m_UV = new Rect(0f, 0f, 1f, 1f);
		Texture specifiedValueOrDefault = image.GetSpecifiedValueOrDefault(null);
		if (specifiedValueOrDefault != null)
		{
			int width = specifiedValueOrDefault.width;
			int height = specifiedValueOrDefault.height;
			m_UV.x = srcRect.x / (float)width;
			m_UV.width = srcRect.width / (float)width;
			m_UV.height = srcRect.height / (float)height;
			m_UV.y = 1f - m_UV.height - srcRect.y / (float)height;
		}
	}

	private Rect GetSourceRect()
	{
		Rect zero = Rect.zero;
		Texture specifiedValueOrDefault = image.GetSpecifiedValueOrDefault(null);
		if (specifiedValueOrDefault != null)
		{
			int width = specifiedValueOrDefault.width;
			int height = specifiedValueOrDefault.height;
			zero.x = uv.x * (float)width;
			zero.width = uv.width * (float)width;
			zero.y = (1f - uv.y - uv.height) * (float)height;
			zero.height = uv.height * (float)height;
		}
		return zero;
	}
}

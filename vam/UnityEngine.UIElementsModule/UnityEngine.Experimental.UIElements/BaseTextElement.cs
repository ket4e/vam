namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>Abstract base class for VisualElement containing text.</para>
/// </summary>
public abstract class BaseTextElement : VisualElement
{
	[SerializeField]
	private string m_Text;

	/// <summary>
	///   <para>The text associated with the element.</para>
	/// </summary>
	public virtual string text
	{
		get
		{
			return m_Text ?? string.Empty;
		}
		set
		{
			if (!(m_Text == value))
			{
				m_Text = value;
				Dirty(ChangeType.Layout);
				if (!string.IsNullOrEmpty(base.persistenceKey))
				{
					SavePersistentData();
				}
			}
		}
	}

	public override void DoRepaint()
	{
		IStylePainter stylePainter = base.elementPanel.stylePainter;
		stylePainter.DrawBackground(this);
		stylePainter.DrawBorder(this);
		stylePainter.DrawText(this);
	}

	protected internal override Vector2 DoMeasure(float width, MeasureMode widthMode, float height, MeasureMode heightMode)
	{
		float x = float.NaN;
		float y = float.NaN;
		Font font = base.style.font;
		if (text == null || font == null)
		{
			return new Vector2(x, y);
		}
		IStylePainter stylePainter = base.elementPanel.stylePainter;
		if (widthMode == MeasureMode.Exactly)
		{
			x = width;
		}
		else
		{
			TextStylePainterParameters defaultTextParameters = stylePainter.GetDefaultTextParameters(this);
			defaultTextParameters.text = text;
			defaultTextParameters.font = font;
			defaultTextParameters.wordWrapWidth = 0f;
			defaultTextParameters.wordWrap = false;
			defaultTextParameters.richText = true;
			x = stylePainter.ComputeTextWidth(defaultTextParameters);
			if (widthMode == MeasureMode.AtMost)
			{
				x = Mathf.Min(x, width);
			}
		}
		if (heightMode == MeasureMode.Exactly)
		{
			y = height;
		}
		else
		{
			TextStylePainterParameters defaultTextParameters2 = stylePainter.GetDefaultTextParameters(this);
			defaultTextParameters2.text = text;
			defaultTextParameters2.font = font;
			defaultTextParameters2.wordWrapWidth = x;
			defaultTextParameters2.richText = true;
			y = stylePainter.ComputeTextHeight(defaultTextParameters2);
			if (heightMode == MeasureMode.AtMost)
			{
				y = Mathf.Min(y, height);
			}
		}
		return new Vector2(x, y);
	}
}

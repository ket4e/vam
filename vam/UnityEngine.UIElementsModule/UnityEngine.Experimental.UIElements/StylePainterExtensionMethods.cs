namespace UnityEngine.Experimental.UIElements;

internal static class StylePainterExtensionMethods
{
	internal static TextureStylePainterParameters GetDefaultTextureParameters(this IStylePainter painter, VisualElement ve)
	{
		IStyle style = ve.style;
		TextureStylePainterParameters textureStylePainterParameters = default(TextureStylePainterParameters);
		textureStylePainterParameters.rect = ve.alignedRect;
		textureStylePainterParameters.uv = new Rect(0f, 0f, 1f, 1f);
		textureStylePainterParameters.color = Color.white;
		textureStylePainterParameters.texture = (Texture2D)style.backgroundImage;
		textureStylePainterParameters.scaleMode = style.backgroundSize;
		textureStylePainterParameters.sliceLeft = style.sliceLeft;
		textureStylePainterParameters.sliceTop = style.sliceTop;
		textureStylePainterParameters.sliceRight = style.sliceRight;
		textureStylePainterParameters.sliceBottom = style.sliceBottom;
		TextureStylePainterParameters result = textureStylePainterParameters;
		painter.SetBorderFromStyle(ref result.border, style);
		return result;
	}

	internal static RectStylePainterParameters GetDefaultRectParameters(this IStylePainter painter, VisualElement ve)
	{
		IStyle style = ve.style;
		RectStylePainterParameters rectStylePainterParameters = default(RectStylePainterParameters);
		rectStylePainterParameters.rect = ve.alignedRect;
		rectStylePainterParameters.color = style.backgroundColor;
		RectStylePainterParameters result = rectStylePainterParameters;
		painter.SetBorderFromStyle(ref result.border, style);
		return result;
	}

	internal static TextStylePainterParameters GetDefaultTextParameters(this IStylePainter painter, BaseTextElement te)
	{
		IStyle style = te.style;
		TextStylePainterParameters result = default(TextStylePainterParameters);
		result.rect = te.contentRect;
		result.text = te.text;
		result.font = style.font;
		result.fontSize = style.fontSize;
		result.fontStyle = style.fontStyle;
		result.fontColor = style.textColor.GetSpecifiedValueOrDefault(Color.black);
		result.anchor = style.textAlignment;
		result.wordWrap = style.wordWrap;
		result.wordWrapWidth = ((!style.wordWrap) ? 0f : te.contentRect.width);
		result.richText = false;
		result.clipping = style.textClipping;
		return result;
	}

	internal static CursorPositionStylePainterParameters GetDefaultCursorPositionParameters(this IStylePainter painter, BaseTextElement te)
	{
		IStyle style = te.style;
		CursorPositionStylePainterParameters result = default(CursorPositionStylePainterParameters);
		result.rect = te.contentRect;
		result.text = te.text;
		result.font = style.font;
		result.fontSize = style.fontSize;
		result.fontStyle = style.fontStyle;
		result.anchor = style.textAlignment;
		result.wordWrapWidth = ((!style.wordWrap) ? 0f : te.contentRect.width);
		result.richText = false;
		result.cursorIndex = 0;
		return result;
	}

	internal static void DrawBackground(this IStylePainter painter, VisualElement ve)
	{
		IStyle style = ve.style;
		if (style.backgroundColor != Color.clear)
		{
			RectStylePainterParameters defaultRectParameters = painter.GetDefaultRectParameters(ve);
			defaultRectParameters.border.SetWidth(0f);
			painter.DrawRect(defaultRectParameters);
		}
		if (style.backgroundImage.value != null)
		{
			TextureStylePainterParameters defaultTextureParameters = painter.GetDefaultTextureParameters(ve);
			defaultTextureParameters.border.SetWidth(0f);
			painter.DrawTexture(defaultTextureParameters);
		}
	}

	internal static void DrawBorder(this IStylePainter painter, VisualElement ve)
	{
		IStyle style = ve.style;
		if (style.borderColor != Color.clear && ((float)style.borderLeftWidth > 0f || (float)style.borderTopWidth > 0f || (float)style.borderRightWidth > 0f || (float)style.borderBottomWidth > 0f))
		{
			RectStylePainterParameters defaultRectParameters = painter.GetDefaultRectParameters(ve);
			defaultRectParameters.color = style.borderColor;
			painter.DrawRect(defaultRectParameters);
		}
	}

	internal static void DrawText(this IStylePainter painter, BaseTextElement te)
	{
		if (!string.IsNullOrEmpty(te.text) && te.contentRect.width > 0f && te.contentRect.height > 0f)
		{
			painter.DrawText(painter.GetDefaultTextParameters(te));
		}
	}

	internal static void SetBorderFromStyle(this IStylePainter painter, ref BorderParameters border, IStyle style)
	{
		border.SetWidth(style.borderTopWidth, style.borderRightWidth, style.borderBottomWidth, style.borderLeftWidth);
		border.SetRadius(style.borderTopLeftRadius, style.borderTopRightRadius, style.borderBottomRightRadius, style.borderBottomLeftRadius);
	}
}

using UnityEngine.Bindings;

namespace UnityEngine;

[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
internal interface IStylePainter
{
	Rect currentWorldClip { get; set; }

	Vector2 mousePosition { get; set; }

	Matrix4x4 currentTransform { get; set; }

	Event repaintEvent { get; set; }

	float opacity { get; set; }

	void DrawRect(RectStylePainterParameters painterParams);

	void DrawTexture(TextureStylePainterParameters painterParams);

	void DrawText(TextStylePainterParameters painterParams);

	Vector2 GetCursorPosition(CursorPositionStylePainterParameters painterParams);

	float ComputeTextWidth(TextStylePainterParameters painterParams);

	float ComputeTextHeight(TextStylePainterParameters painterParams);
}

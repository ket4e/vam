using UnityEngine.Bindings;

namespace UnityEngine;

[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
internal struct BorderParameters
{
	public float leftWidth;

	public float topWidth;

	public float rightWidth;

	public float bottomWidth;

	public float topLeftRadius;

	public float topRightRadius;

	public float bottomRightRadius;

	public float bottomLeftRadius;

	public void SetWidth(float top, float right, float bottom, float left)
	{
		topWidth = top;
		rightWidth = right;
		bottomWidth = bottom;
		leftWidth = left;
	}

	public void SetWidth(float allBorders)
	{
		SetWidth(allBorders, allBorders, allBorders, allBorders);
	}

	public void SetRadius(float topLeft, float topRight, float bottomRight, float bottomLeft)
	{
		topLeftRadius = topLeft;
		topRightRadius = topRight;
		bottomRightRadius = bottomRight;
		bottomLeftRadius = bottomLeft;
	}

	public void SetRadius(float radius)
	{
		SetRadius(radius, radius, radius, radius);
	}

	public Vector4 GetWidths()
	{
		return new Vector4(leftWidth, topWidth, rightWidth, bottomWidth);
	}

	public Vector4 GetRadiuses()
	{
		return new Vector4(topLeftRadius, topRightRadius, bottomRightRadius, bottomLeftRadius);
	}
}

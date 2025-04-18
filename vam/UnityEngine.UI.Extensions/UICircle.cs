using System;

namespace UnityEngine.UI.Extensions;

[AddComponentMenu("UI/Extensions/Primitives/UI Circle")]
public class UICircle : UIPrimitiveBase
{
	[Tooltip("The circular fill percentage of the primitive, affected by FixedToSegments")]
	[Range(0f, 100f)]
	[SerializeField]
	private int m_fillPercent = 100;

	[Tooltip("Should the primitive fill draw by segments or absolute percentage")]
	public bool FixedToSegments;

	[Tooltip("Draw the primitive filled or as a line")]
	[SerializeField]
	private bool m_fill = true;

	[Tooltip("If not filled, the thickness of the primitive line")]
	[SerializeField]
	private float m_thickness = 5f;

	[Tooltip("The number of segments to draw the primitive, more segments = smoother primitive")]
	[Range(0f, 360f)]
	[SerializeField]
	private int m_segments = 360;

	public int FillPercent
	{
		get
		{
			return m_fillPercent;
		}
		set
		{
			m_fillPercent = value;
			SetAllDirty();
		}
	}

	public bool Fill
	{
		get
		{
			return m_fill;
		}
		set
		{
			m_fill = value;
			SetAllDirty();
		}
	}

	public float Thickness
	{
		get
		{
			return m_thickness;
		}
		set
		{
			m_thickness = value;
			SetAllDirty();
		}
	}

	public int Segments
	{
		get
		{
			return m_segments;
		}
		set
		{
			m_segments = value;
			SetAllDirty();
		}
	}

	private void Update()
	{
		m_thickness = Mathf.Clamp(m_thickness, 0f, base.rectTransform.rect.width / 2f);
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		float outer = (0f - base.rectTransform.pivot.x) * base.rectTransform.rect.width;
		float inner = (0f - base.rectTransform.pivot.x) * base.rectTransform.rect.width + m_thickness;
		vh.Clear();
		Vector2 prevX = Vector2.zero;
		Vector2 prevY = Vector2.zero;
		Vector2 vector = new Vector2(0f, 0f);
		Vector2 vector2 = new Vector2(0f, 1f);
		Vector2 vector3 = new Vector2(1f, 1f);
		Vector2 vector4 = new Vector2(1f, 0f);
		Vector2 pos;
		Vector2 pos2;
		Vector2 pos3;
		Vector2 pos4;
		if (FixedToSegments)
		{
			float num = (float)m_fillPercent / 100f;
			float num2 = 360f / (float)m_segments;
			int num3 = (int)((float)(m_segments + 1) * num);
			for (int i = 0; i < num3; i++)
			{
				float f = (float)Math.PI / 180f * ((float)i * num2);
				float c = Mathf.Cos(f);
				float s = Mathf.Sin(f);
				vector = new Vector2(0f, 1f);
				vector2 = new Vector2(1f, 1f);
				vector3 = new Vector2(1f, 0f);
				vector4 = new Vector2(0f, 0f);
				StepThroughPointsAndFill(outer, inner, ref prevX, ref prevY, out pos, out pos2, out pos3, out pos4, c, s);
				vh.AddUIVertexQuad(SetVbo(new Vector2[4] { pos, pos2, pos3, pos4 }, new Vector2[4] { vector, vector2, vector3, vector4 }));
			}
			return;
		}
		float width = base.rectTransform.rect.width;
		float height = base.rectTransform.rect.height;
		float num4 = (float)m_fillPercent / 100f * ((float)Math.PI * 2f) / (float)m_segments;
		float num5 = 0f;
		for (int j = 0; j < m_segments + 1; j++)
		{
			float c2 = Mathf.Cos(num5);
			float s2 = Mathf.Sin(num5);
			StepThroughPointsAndFill(outer, inner, ref prevX, ref prevY, out pos, out pos2, out pos3, out pos4, c2, s2);
			vector = new Vector2(pos.x / width + 0.5f, pos.y / height + 0.5f);
			vector2 = new Vector2(pos2.x / width + 0.5f, pos2.y / height + 0.5f);
			vector3 = new Vector2(pos3.x / width + 0.5f, pos3.y / height + 0.5f);
			vector4 = new Vector2(pos4.x / width + 0.5f, pos4.y / height + 0.5f);
			vh.AddUIVertexQuad(SetVbo(new Vector2[4] { pos, pos2, pos3, pos4 }, new Vector2[4] { vector, vector2, vector3, vector4 }));
			num5 += num4;
		}
	}

	private void StepThroughPointsAndFill(float outer, float inner, ref Vector2 prevX, ref Vector2 prevY, out Vector2 pos0, out Vector2 pos1, out Vector2 pos2, out Vector2 pos3, float c, float s)
	{
		pos0 = prevX;
		pos1 = new Vector2(outer * c, outer * s);
		if (m_fill)
		{
			pos2 = Vector2.zero;
			pos3 = Vector2.zero;
		}
		else
		{
			pos2 = new Vector2(inner * c, inner * s);
			pos3 = prevY;
		}
		prevX = pos1;
		prevY = pos2;
	}
}

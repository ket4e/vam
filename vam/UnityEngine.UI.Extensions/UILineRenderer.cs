using System;
using System.Collections.Generic;
using UnityEngine.Sprites;

namespace UnityEngine.UI.Extensions;

[AddComponentMenu("UI/Extensions/Primitives/UILineRenderer")]
[RequireComponent(typeof(RectTransform))]
public class UILineRenderer : UIPrimitiveBase
{
	private enum SegmentType
	{
		Start,
		Middle,
		End,
		Full
	}

	public enum JoinType
	{
		Bevel,
		Miter
	}

	public enum BezierType
	{
		None,
		Quick,
		Basic,
		Improved,
		Catenary
	}

	private const float MIN_MITER_JOIN = (float)Math.PI / 12f;

	private const float MIN_BEVEL_NICE_JOIN = (float)Math.PI / 6f;

	private static Vector2 UV_TOP_LEFT;

	private static Vector2 UV_BOTTOM_LEFT;

	private static Vector2 UV_TOP_CENTER_LEFT;

	private static Vector2 UV_TOP_CENTER_RIGHT;

	private static Vector2 UV_BOTTOM_CENTER_LEFT;

	private static Vector2 UV_BOTTOM_CENTER_RIGHT;

	private static Vector2 UV_TOP_RIGHT;

	private static Vector2 UV_BOTTOM_RIGHT;

	private static Vector2[] startUvs;

	private static Vector2[] middleUvs;

	private static Vector2[] endUvs;

	private static Vector2[] fullUvs;

	[SerializeField]
	[Tooltip("Points to draw lines between\n Can be improved using the Resolution Option")]
	internal Vector2[] m_points;

	[SerializeField]
	[Tooltip("Thickness of the line")]
	internal float lineThickness = 2f;

	[SerializeField]
	[Tooltip("Use the relative bounds of the Rect Transform (0,0 -> 0,1) or screen space coordinates")]
	internal bool relativeSize;

	[SerializeField]
	[Tooltip("Do the points identify a single line or split pairs of lines")]
	internal bool lineList;

	[SerializeField]
	[Tooltip("Add end caps to each line\nMultiple caps when used with Line List")]
	internal bool lineCaps;

	[SerializeField]
	[Tooltip("Resolution of the Bezier curve, different to line Resolution")]
	internal int bezierSegmentsPerCurve = 10;

	[Tooltip("The type of Join used between lines, Square/Mitre or Curved/Bevel")]
	public JoinType LineJoins;

	[Tooltip("Bezier method to apply to line, see docs for options\nCan't be used in conjunction with Resolution as Bezier already changes the resolution")]
	public BezierType BezierMode;

	[HideInInspector]
	public bool drivenExternally;

	public float LineThickness
	{
		get
		{
			return lineThickness;
		}
		set
		{
			lineThickness = value;
			SetAllDirty();
		}
	}

	public bool RelativeSize
	{
		get
		{
			return relativeSize;
		}
		set
		{
			relativeSize = value;
			SetAllDirty();
		}
	}

	public bool LineList
	{
		get
		{
			return lineList;
		}
		set
		{
			lineList = value;
			SetAllDirty();
		}
	}

	public bool LineCaps
	{
		get
		{
			return lineCaps;
		}
		set
		{
			lineCaps = value;
			SetAllDirty();
		}
	}

	public int BezierSegmentsPerCurve
	{
		get
		{
			return bezierSegmentsPerCurve;
		}
		set
		{
			bezierSegmentsPerCurve = value;
		}
	}

	public Vector2[] Points
	{
		get
		{
			return m_points;
		}
		set
		{
			if (m_points != value)
			{
				m_points = value;
				SetAllDirty();
			}
		}
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		if (m_points == null)
		{
			return;
		}
		GeneratedUVs();
		Vector2[] array = m_points;
		if (BezierMode != 0 && BezierMode != BezierType.Catenary && m_points.Length > 3)
		{
			BezierPath bezierPath = new BezierPath();
			bezierPath.SetControlPoints(array);
			bezierPath.SegmentsPerCurve = bezierSegmentsPerCurve;
			array = (BezierMode switch
			{
				BezierType.Basic => bezierPath.GetDrawingPoints0(), 
				BezierType.Improved => bezierPath.GetDrawingPoints1(), 
				_ => bezierPath.GetDrawingPoints2(), 
			}).ToArray();
		}
		if (BezierMode == BezierType.Catenary && m_points.Length == 2)
		{
			CableCurve cableCurve = new CableCurve(array);
			cableCurve.slack = base.Resoloution;
			cableCurve.steps = BezierSegmentsPerCurve;
			array = cableCurve.Points();
		}
		if (base.ImproveResolution != 0)
		{
			array = IncreaseResolution(array);
		}
		float num = (relativeSize ? base.rectTransform.rect.width : 1f);
		float num2 = (relativeSize ? base.rectTransform.rect.height : 1f);
		float num3 = (0f - base.rectTransform.pivot.x) * num;
		float num4 = (0f - base.rectTransform.pivot.y) * num2;
		vh.Clear();
		List<UIVertex[]> list = new List<UIVertex[]>();
		if (lineList)
		{
			for (int i = 1; i < array.Length; i += 2)
			{
				Vector2 vector = array[i - 1];
				Vector2 vector2 = array[i];
				vector = new Vector2(vector.x * num + num3, vector.y * num2 + num4);
				vector2 = new Vector2(vector2.x * num + num3, vector2.y * num2 + num4);
				if (lineCaps)
				{
					list.Add(CreateLineCap(vector, vector2, SegmentType.Start));
				}
				list.Add(CreateLineSegment(vector, vector2, SegmentType.Middle));
				if (lineCaps)
				{
					list.Add(CreateLineCap(vector, vector2, SegmentType.End));
				}
			}
		}
		else
		{
			for (int j = 1; j < array.Length; j++)
			{
				Vector2 vector3 = array[j - 1];
				Vector2 vector4 = array[j];
				vector3 = new Vector2(vector3.x * num + num3, vector3.y * num2 + num4);
				vector4 = new Vector2(vector4.x * num + num3, vector4.y * num2 + num4);
				if (lineCaps && j == 1)
				{
					list.Add(CreateLineCap(vector3, vector4, SegmentType.Start));
				}
				list.Add(CreateLineSegment(vector3, vector4, SegmentType.Middle));
				if (lineCaps && j == array.Length - 1)
				{
					list.Add(CreateLineCap(vector3, vector4, SegmentType.End));
				}
			}
		}
		for (int k = 0; k < list.Count; k++)
		{
			if (!lineList && k < list.Count - 1)
			{
				Vector3 vector5 = list[k][1].position - list[k][2].position;
				Vector3 vector6 = list[k + 1][2].position - list[k + 1][1].position;
				float num5 = Vector2.Angle(vector5, vector6) * ((float)Math.PI / 180f);
				float num6 = Mathf.Sign(Vector3.Cross(vector5.normalized, vector6.normalized).z);
				float num7 = lineThickness / (2f * Mathf.Tan(num5 / 2f));
				Vector3 position = list[k][2].position - vector5.normalized * num7 * num6;
				Vector3 position2 = list[k][3].position + vector5.normalized * num7 * num6;
				JoinType joinType = LineJoins;
				if (joinType == JoinType.Miter)
				{
					if (num7 < vector5.magnitude / 2f && num7 < vector6.magnitude / 2f && num5 > (float)Math.PI / 12f)
					{
						list[k][2].position = position;
						list[k][3].position = position2;
						list[k + 1][0].position = position2;
						list[k + 1][1].position = position;
					}
					else
					{
						joinType = JoinType.Bevel;
					}
				}
				if (joinType == JoinType.Bevel)
				{
					if (num7 < vector5.magnitude / 2f && num7 < vector6.magnitude / 2f && num5 > (float)Math.PI / 6f)
					{
						if (num6 < 0f)
						{
							list[k][2].position = position;
							list[k + 1][1].position = position;
						}
						else
						{
							list[k][3].position = position2;
							list[k + 1][0].position = position2;
						}
					}
					UIVertex[] verts = new UIVertex[4]
					{
						list[k][2],
						list[k][3],
						list[k + 1][0],
						list[k + 1][1]
					};
					vh.AddUIVertexQuad(verts);
				}
			}
			vh.AddUIVertexQuad(list[k]);
		}
		if (vh.currentVertCount > 64000)
		{
			Debug.LogError("Max Verticies size is 64000, current mesh vertcies count is [" + vh.currentVertCount + "] - Cannot Draw");
			vh.Clear();
		}
	}

	private UIVertex[] CreateLineCap(Vector2 start, Vector2 end, SegmentType type)
	{
		switch (type)
		{
		case SegmentType.Start:
		{
			Vector2 start2 = start - (end - start).normalized * lineThickness / 2f;
			return CreateLineSegment(start2, start, SegmentType.Start);
		}
		case SegmentType.End:
		{
			Vector2 end2 = end + (end - start).normalized * lineThickness / 2f;
			return CreateLineSegment(end, end2, SegmentType.End);
		}
		default:
			Debug.LogError("Bad SegmentType passed in to CreateLineCap. Must be SegmentType.Start or SegmentType.End");
			return null;
		}
	}

	private UIVertex[] CreateLineSegment(Vector2 start, Vector2 end, SegmentType type)
	{
		Vector2 vector = new Vector2(start.y - end.y, end.x - start.x).normalized * lineThickness / 2f;
		Vector2 vector2 = start - vector;
		Vector2 vector3 = start + vector;
		Vector2 vector4 = end + vector;
		Vector2 vector5 = end - vector;
		return type switch
		{
			SegmentType.Start => SetVbo(new Vector2[4] { vector2, vector3, vector4, vector5 }, startUvs), 
			SegmentType.End => SetVbo(new Vector2[4] { vector2, vector3, vector4, vector5 }, endUvs), 
			SegmentType.Full => SetVbo(new Vector2[4] { vector2, vector3, vector4, vector5 }, fullUvs), 
			_ => SetVbo(new Vector2[4] { vector2, vector3, vector4, vector5 }, middleUvs), 
		};
	}

	protected override void GeneratedUVs()
	{
		if (base.activeSprite != null)
		{
			Vector4 outerUV = DataUtility.GetOuterUV(base.activeSprite);
			Vector4 innerUV = DataUtility.GetInnerUV(base.activeSprite);
			UV_TOP_LEFT = new Vector2(outerUV.x, outerUV.y);
			UV_BOTTOM_LEFT = new Vector2(outerUV.x, outerUV.w);
			UV_TOP_CENTER_LEFT = new Vector2(innerUV.x, innerUV.y);
			UV_TOP_CENTER_RIGHT = new Vector2(innerUV.z, innerUV.y);
			UV_BOTTOM_CENTER_LEFT = new Vector2(innerUV.x, innerUV.w);
			UV_BOTTOM_CENTER_RIGHT = new Vector2(innerUV.z, innerUV.w);
			UV_TOP_RIGHT = new Vector2(outerUV.z, outerUV.y);
			UV_BOTTOM_RIGHT = new Vector2(outerUV.z, outerUV.w);
		}
		else
		{
			UV_TOP_LEFT = Vector2.zero;
			UV_BOTTOM_LEFT = new Vector2(0f, 1f);
			UV_TOP_CENTER_LEFT = new Vector2(0.5f, 0f);
			UV_TOP_CENTER_RIGHT = new Vector2(0.5f, 0f);
			UV_BOTTOM_CENTER_LEFT = new Vector2(0.5f, 1f);
			UV_BOTTOM_CENTER_RIGHT = new Vector2(0.5f, 1f);
			UV_TOP_RIGHT = new Vector2(1f, 0f);
			UV_BOTTOM_RIGHT = Vector2.one;
		}
		startUvs = new Vector2[4] { UV_TOP_LEFT, UV_BOTTOM_LEFT, UV_BOTTOM_CENTER_LEFT, UV_TOP_CENTER_LEFT };
		middleUvs = new Vector2[4] { UV_TOP_CENTER_LEFT, UV_BOTTOM_CENTER_LEFT, UV_BOTTOM_CENTER_RIGHT, UV_TOP_CENTER_RIGHT };
		endUvs = new Vector2[4] { UV_TOP_CENTER_RIGHT, UV_BOTTOM_CENTER_RIGHT, UV_BOTTOM_RIGHT, UV_TOP_RIGHT };
		fullUvs = new Vector2[4] { UV_TOP_LEFT, UV_BOTTOM_LEFT, UV_BOTTOM_RIGHT, UV_TOP_RIGHT };
	}

	protected override void ResolutionToNativeSize(float distance)
	{
		if (base.UseNativeSize)
		{
			m_Resolution = distance / (base.activeSprite.rect.width / base.pixelsPerUnit);
			lineThickness = base.activeSprite.rect.height / base.pixelsPerUnit;
		}
	}
}

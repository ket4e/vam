using System;
using UnityEngine.Serialization;
using UnityEngine.Sprites;

namespace UnityEngine.UI;

[AddComponentMenu("UI/Image", 11)]
public class Image : MaskableGraphic, ISerializationCallbackReceiver, ILayoutElement, ICanvasRaycastFilter
{
	public enum Type
	{
		Simple,
		Sliced,
		Tiled,
		Filled
	}

	public enum FillMethod
	{
		Horizontal,
		Vertical,
		Radial90,
		Radial180,
		Radial360
	}

	public enum OriginHorizontal
	{
		Left,
		Right
	}

	public enum OriginVertical
	{
		Bottom,
		Top
	}

	public enum Origin90
	{
		BottomLeft,
		TopLeft,
		TopRight,
		BottomRight
	}

	public enum Origin180
	{
		Bottom,
		Left,
		Top,
		Right
	}

	public enum Origin360
	{
		Bottom,
		Right,
		Top,
		Left
	}

	protected static Material s_ETC1DefaultUI = null;

	[FormerlySerializedAs("m_Frame")]
	[SerializeField]
	private Sprite m_Sprite;

	[NonSerialized]
	private Sprite m_OverrideSprite;

	[SerializeField]
	private Type m_Type = Type.Simple;

	[SerializeField]
	private bool m_PreserveAspect = false;

	[SerializeField]
	private bool m_FillCenter = true;

	[SerializeField]
	private FillMethod m_FillMethod = FillMethod.Radial360;

	[Range(0f, 1f)]
	[SerializeField]
	private float m_FillAmount = 1f;

	[SerializeField]
	private bool m_FillClockwise = true;

	[SerializeField]
	private int m_FillOrigin;

	private float m_AlphaHitTestMinimumThreshold = 0f;

	private static readonly Vector2[] s_VertScratch = new Vector2[4];

	private static readonly Vector2[] s_UVScratch = new Vector2[4];

	private static readonly Vector3[] s_Xy = new Vector3[4];

	private static readonly Vector3[] s_Uv = new Vector3[4];

	public Sprite sprite
	{
		get
		{
			return m_Sprite;
		}
		set
		{
			if (SetPropertyUtility.SetClass(ref m_Sprite, value))
			{
				SetAllDirty();
			}
		}
	}

	public Sprite overrideSprite
	{
		get
		{
			return activeSprite;
		}
		set
		{
			if (SetPropertyUtility.SetClass(ref m_OverrideSprite, value))
			{
				SetAllDirty();
			}
		}
	}

	private Sprite activeSprite => (!(m_OverrideSprite != null)) ? sprite : m_OverrideSprite;

	public Type type
	{
		get
		{
			return m_Type;
		}
		set
		{
			if (SetPropertyUtility.SetStruct(ref m_Type, value))
			{
				SetVerticesDirty();
			}
		}
	}

	public bool preserveAspect
	{
		get
		{
			return m_PreserveAspect;
		}
		set
		{
			if (SetPropertyUtility.SetStruct(ref m_PreserveAspect, value))
			{
				SetVerticesDirty();
			}
		}
	}

	public bool fillCenter
	{
		get
		{
			return m_FillCenter;
		}
		set
		{
			if (SetPropertyUtility.SetStruct(ref m_FillCenter, value))
			{
				SetVerticesDirty();
			}
		}
	}

	public FillMethod fillMethod
	{
		get
		{
			return m_FillMethod;
		}
		set
		{
			if (SetPropertyUtility.SetStruct(ref m_FillMethod, value))
			{
				SetVerticesDirty();
				m_FillOrigin = 0;
			}
		}
	}

	public float fillAmount
	{
		get
		{
			return m_FillAmount;
		}
		set
		{
			if (SetPropertyUtility.SetStruct(ref m_FillAmount, Mathf.Clamp01(value)))
			{
				SetVerticesDirty();
			}
		}
	}

	public bool fillClockwise
	{
		get
		{
			return m_FillClockwise;
		}
		set
		{
			if (SetPropertyUtility.SetStruct(ref m_FillClockwise, value))
			{
				SetVerticesDirty();
			}
		}
	}

	public int fillOrigin
	{
		get
		{
			return m_FillOrigin;
		}
		set
		{
			if (SetPropertyUtility.SetStruct(ref m_FillOrigin, value))
			{
				SetVerticesDirty();
			}
		}
	}

	[Obsolete("eventAlphaThreshold has been deprecated. Use eventMinimumAlphaThreshold instead (UnityUpgradable) -> alphaHitTestMinimumThreshold")]
	public float eventAlphaThreshold
	{
		get
		{
			return 1f - alphaHitTestMinimumThreshold;
		}
		set
		{
			alphaHitTestMinimumThreshold = 1f - value;
		}
	}

	public float alphaHitTestMinimumThreshold
	{
		get
		{
			return m_AlphaHitTestMinimumThreshold;
		}
		set
		{
			m_AlphaHitTestMinimumThreshold = value;
		}
	}

	public static Material defaultETC1GraphicMaterial
	{
		get
		{
			if (s_ETC1DefaultUI == null)
			{
				s_ETC1DefaultUI = Canvas.GetETC1SupportedCanvasMaterial();
			}
			return s_ETC1DefaultUI;
		}
	}

	public override Texture mainTexture
	{
		get
		{
			if (activeSprite == null)
			{
				if (material != null && material.mainTexture != null)
				{
					return material.mainTexture;
				}
				return Graphic.s_WhiteTexture;
			}
			return activeSprite.texture;
		}
	}

	public bool hasBorder
	{
		get
		{
			if (activeSprite != null)
			{
				return activeSprite.border.sqrMagnitude > 0f;
			}
			return false;
		}
	}

	public float pixelsPerUnit
	{
		get
		{
			float num = 100f;
			if ((bool)activeSprite)
			{
				num = activeSprite.pixelsPerUnit;
			}
			float num2 = 100f;
			if ((bool)base.canvas)
			{
				num2 = base.canvas.referencePixelsPerUnit;
			}
			return num / num2;
		}
	}

	public override Material material
	{
		get
		{
			if (m_Material != null)
			{
				return m_Material;
			}
			if ((bool)activeSprite && activeSprite.associatedAlphaSplitTexture != null)
			{
				return defaultETC1GraphicMaterial;
			}
			return defaultMaterial;
		}
		set
		{
			base.material = value;
		}
	}

	public virtual float minWidth => 0f;

	public virtual float preferredWidth
	{
		get
		{
			if (activeSprite == null)
			{
				return 0f;
			}
			if (type == Type.Sliced || type == Type.Tiled)
			{
				return DataUtility.GetMinSize(activeSprite).x / pixelsPerUnit;
			}
			return activeSprite.rect.size.x / pixelsPerUnit;
		}
	}

	public virtual float flexibleWidth => -1f;

	public virtual float minHeight => 0f;

	public virtual float preferredHeight
	{
		get
		{
			if (activeSprite == null)
			{
				return 0f;
			}
			if (type == Type.Sliced || type == Type.Tiled)
			{
				return DataUtility.GetMinSize(activeSprite).y / pixelsPerUnit;
			}
			return activeSprite.rect.size.y / pixelsPerUnit;
		}
	}

	public virtual float flexibleHeight => -1f;

	public virtual int layoutPriority => 0;

	protected Image()
	{
		base.useLegacyMeshGeneration = false;
	}

	public virtual void OnBeforeSerialize()
	{
	}

	public virtual void OnAfterDeserialize()
	{
		if (m_FillOrigin < 0)
		{
			m_FillOrigin = 0;
		}
		else if (m_FillMethod == FillMethod.Horizontal && m_FillOrigin > 1)
		{
			m_FillOrigin = 0;
		}
		else if (m_FillMethod == FillMethod.Vertical && m_FillOrigin > 1)
		{
			m_FillOrigin = 0;
		}
		else if (m_FillOrigin > 3)
		{
			m_FillOrigin = 0;
		}
		m_FillAmount = Mathf.Clamp(m_FillAmount, 0f, 1f);
	}

	private Vector4 GetDrawingDimensions(bool shouldPreserveAspect)
	{
		Vector4 vector = ((!(activeSprite == null)) ? DataUtility.GetPadding(activeSprite) : Vector4.zero);
		Vector2 vector2 = ((!(activeSprite == null)) ? new Vector2(activeSprite.rect.width, activeSprite.rect.height) : Vector2.zero);
		Rect pixelAdjustedRect = GetPixelAdjustedRect();
		int num = Mathf.RoundToInt(vector2.x);
		int num2 = Mathf.RoundToInt(vector2.y);
		Vector4 vector3 = new Vector4(vector.x / (float)num, vector.y / (float)num2, ((float)num - vector.z) / (float)num, ((float)num2 - vector.w) / (float)num2);
		if (shouldPreserveAspect && vector2.sqrMagnitude > 0f)
		{
			float num3 = vector2.x / vector2.y;
			float num4 = pixelAdjustedRect.width / pixelAdjustedRect.height;
			if (num3 > num4)
			{
				float height = pixelAdjustedRect.height;
				pixelAdjustedRect.height = pixelAdjustedRect.width * (1f / num3);
				pixelAdjustedRect.y += (height - pixelAdjustedRect.height) * base.rectTransform.pivot.y;
			}
			else
			{
				float width = pixelAdjustedRect.width;
				pixelAdjustedRect.width = pixelAdjustedRect.height * num3;
				pixelAdjustedRect.x += (width - pixelAdjustedRect.width) * base.rectTransform.pivot.x;
			}
		}
		return new Vector4(pixelAdjustedRect.x + pixelAdjustedRect.width * vector3.x, pixelAdjustedRect.y + pixelAdjustedRect.height * vector3.y, pixelAdjustedRect.x + pixelAdjustedRect.width * vector3.z, pixelAdjustedRect.y + pixelAdjustedRect.height * vector3.w);
	}

	public override void SetNativeSize()
	{
		if (activeSprite != null)
		{
			float x = activeSprite.rect.width / pixelsPerUnit;
			float y = activeSprite.rect.height / pixelsPerUnit;
			base.rectTransform.anchorMax = base.rectTransform.anchorMin;
			base.rectTransform.sizeDelta = new Vector2(x, y);
			SetAllDirty();
		}
	}

	protected override void OnPopulateMesh(VertexHelper toFill)
	{
		if (activeSprite == null)
		{
			base.OnPopulateMesh(toFill);
			return;
		}
		switch (type)
		{
		case Type.Simple:
			GenerateSimpleSprite(toFill, m_PreserveAspect);
			break;
		case Type.Sliced:
			GenerateSlicedSprite(toFill);
			break;
		case Type.Tiled:
			GenerateTiledSprite(toFill);
			break;
		case Type.Filled:
			GenerateFilledSprite(toFill, m_PreserveAspect);
			break;
		}
	}

	protected override void UpdateMaterial()
	{
		base.UpdateMaterial();
		if (activeSprite == null)
		{
			base.canvasRenderer.SetAlphaTexture(null);
			return;
		}
		Texture2D associatedAlphaSplitTexture = activeSprite.associatedAlphaSplitTexture;
		if (associatedAlphaSplitTexture != null)
		{
			base.canvasRenderer.SetAlphaTexture(associatedAlphaSplitTexture);
		}
	}

	private void GenerateSimpleSprite(VertexHelper vh, bool lPreserveAspect)
	{
		Vector4 drawingDimensions = GetDrawingDimensions(lPreserveAspect);
		Vector4 vector = ((!(activeSprite != null)) ? Vector4.zero : DataUtility.GetOuterUV(activeSprite));
		Color color = this.color;
		vh.Clear();
		vh.AddVert(new Vector3(drawingDimensions.x, drawingDimensions.y), color, new Vector2(vector.x, vector.y));
		vh.AddVert(new Vector3(drawingDimensions.x, drawingDimensions.w), color, new Vector2(vector.x, vector.w));
		vh.AddVert(new Vector3(drawingDimensions.z, drawingDimensions.w), color, new Vector2(vector.z, vector.w));
		vh.AddVert(new Vector3(drawingDimensions.z, drawingDimensions.y), color, new Vector2(vector.z, vector.y));
		vh.AddTriangle(0, 1, 2);
		vh.AddTriangle(2, 3, 0);
	}

	private void GenerateSlicedSprite(VertexHelper toFill)
	{
		if (!hasBorder)
		{
			GenerateSimpleSprite(toFill, lPreserveAspect: false);
			return;
		}
		Vector4 vector;
		Vector4 vector2;
		Vector4 vector3;
		Vector4 vector4;
		if (activeSprite != null)
		{
			vector = DataUtility.GetOuterUV(activeSprite);
			vector2 = DataUtility.GetInnerUV(activeSprite);
			vector3 = DataUtility.GetPadding(activeSprite);
			vector4 = activeSprite.border;
		}
		else
		{
			vector = Vector4.zero;
			vector2 = Vector4.zero;
			vector3 = Vector4.zero;
			vector4 = Vector4.zero;
		}
		Rect pixelAdjustedRect = GetPixelAdjustedRect();
		Vector4 adjustedBorders = GetAdjustedBorders(vector4 / pixelsPerUnit, pixelAdjustedRect);
		vector3 /= pixelsPerUnit;
		ref Vector2 reference = ref s_VertScratch[0];
		reference = new Vector2(vector3.x, vector3.y);
		ref Vector2 reference2 = ref s_VertScratch[3];
		reference2 = new Vector2(pixelAdjustedRect.width - vector3.z, pixelAdjustedRect.height - vector3.w);
		s_VertScratch[1].x = adjustedBorders.x;
		s_VertScratch[1].y = adjustedBorders.y;
		s_VertScratch[2].x = pixelAdjustedRect.width - adjustedBorders.z;
		s_VertScratch[2].y = pixelAdjustedRect.height - adjustedBorders.w;
		for (int i = 0; i < 4; i++)
		{
			s_VertScratch[i].x += pixelAdjustedRect.x;
			s_VertScratch[i].y += pixelAdjustedRect.y;
		}
		ref Vector2 reference3 = ref s_UVScratch[0];
		reference3 = new Vector2(vector.x, vector.y);
		ref Vector2 reference4 = ref s_UVScratch[1];
		reference4 = new Vector2(vector2.x, vector2.y);
		ref Vector2 reference5 = ref s_UVScratch[2];
		reference5 = new Vector2(vector2.z, vector2.w);
		ref Vector2 reference6 = ref s_UVScratch[3];
		reference6 = new Vector2(vector.z, vector.w);
		toFill.Clear();
		for (int j = 0; j < 3; j++)
		{
			int num = j + 1;
			for (int k = 0; k < 3; k++)
			{
				if (m_FillCenter || j != 1 || k != 1)
				{
					int num2 = k + 1;
					AddQuad(toFill, new Vector2(s_VertScratch[j].x, s_VertScratch[k].y), new Vector2(s_VertScratch[num].x, s_VertScratch[num2].y), color, new Vector2(s_UVScratch[j].x, s_UVScratch[k].y), new Vector2(s_UVScratch[num].x, s_UVScratch[num2].y));
				}
			}
		}
	}

	private void GenerateTiledSprite(VertexHelper toFill)
	{
		Vector4 vector;
		Vector4 vector2;
		Vector2 vector4;
		Vector4 vector3;
		if (activeSprite != null)
		{
			vector = DataUtility.GetOuterUV(activeSprite);
			vector2 = DataUtility.GetInnerUV(activeSprite);
			vector3 = activeSprite.border;
			vector4 = activeSprite.rect.size;
		}
		else
		{
			vector = Vector4.zero;
			vector2 = Vector4.zero;
			vector3 = Vector4.zero;
			vector4 = Vector2.one * 100f;
		}
		Rect pixelAdjustedRect = GetPixelAdjustedRect();
		float num = (vector4.x - vector3.x - vector3.z) / pixelsPerUnit;
		float num2 = (vector4.y - vector3.y - vector3.w) / pixelsPerUnit;
		vector3 = GetAdjustedBorders(vector3 / pixelsPerUnit, pixelAdjustedRect);
		Vector2 vector5 = new Vector2(vector2.x, vector2.y);
		Vector2 vector6 = new Vector2(vector2.z, vector2.w);
		float x = vector3.x;
		float num3 = pixelAdjustedRect.width - vector3.z;
		float y = vector3.y;
		float num4 = pixelAdjustedRect.height - vector3.w;
		toFill.Clear();
		Vector2 uvMax = vector6;
		if (num <= 0f)
		{
			num = num3 - x;
		}
		if (num2 <= 0f)
		{
			num2 = num4 - y;
		}
		if (activeSprite != null && (hasBorder || activeSprite.packed || activeSprite.texture.wrapMode != 0))
		{
			long num5 = 0L;
			long num6 = 0L;
			if (m_FillCenter)
			{
				num5 = (long)Math.Ceiling((num3 - x) / num);
				num6 = (long)Math.Ceiling((num4 - y) / num2);
				double num7 = 0.0;
				num7 = ((!hasBorder) ? ((double)(num5 * num6) * 4.0) : (((double)num5 + 2.0) * ((double)num6 + 2.0) * 4.0));
				if (num7 > 65000.0)
				{
					Debug.LogError("Too many sprite tiles on Image \"" + base.name + "\". The tile size will be increased. To remove the limit on the number of tiles, convert the Sprite to an Advanced texture, remove the borders, clear the Packing tag and set the Wrap mode to Repeat.", this);
					double num8 = 16250.0;
					double num9 = ((!hasBorder) ? ((double)num5 / (double)num6) : (((double)num5 + 2.0) / ((double)num6 + 2.0)));
					double num10 = Math.Sqrt(num8 / num9);
					double num11 = num10 * num9;
					if (hasBorder)
					{
						num10 -= 2.0;
						num11 -= 2.0;
					}
					num5 = (long)Math.Floor(num10);
					num6 = (long)Math.Floor(num11);
					num = (num3 - x) / (float)num5;
					num2 = (num4 - y) / (float)num6;
				}
			}
			else if (hasBorder)
			{
				num5 = (long)Math.Ceiling((num3 - x) / num);
				num6 = (long)Math.Ceiling((num4 - y) / num2);
				double num12 = ((double)(num6 + num5) + 2.0) * 2.0 * 4.0;
				if (num12 > 65000.0)
				{
					Debug.LogError("Too many sprite tiles on Image \"" + base.name + "\". The tile size will be increased. To remove the limit on the number of tiles, convert the Sprite to an Advanced texture, remove the borders, clear the Packing tag and set the Wrap mode to Repeat.", this);
					double num13 = 16250.0;
					double num14 = (double)num5 / (double)num6;
					double num15 = (num13 - 4.0) / (2.0 * (1.0 + num14));
					double d = num15 * num14;
					num5 = (long)Math.Floor(num15);
					num6 = (long)Math.Floor(d);
					num = (num3 - x) / (float)num5;
					num2 = (num4 - y) / (float)num6;
				}
			}
			else
			{
				num6 = (num5 = 0L);
			}
			if (m_FillCenter)
			{
				for (long num16 = 0L; num16 < num6; num16++)
				{
					float num17 = y + (float)num16 * num2;
					float num18 = y + (float)(num16 + 1) * num2;
					if (num18 > num4)
					{
						uvMax.y = vector5.y + (vector6.y - vector5.y) * (num4 - num17) / (num18 - num17);
						num18 = num4;
					}
					uvMax.x = vector6.x;
					for (long num19 = 0L; num19 < num5; num19++)
					{
						float num20 = x + (float)num19 * num;
						float num21 = x + (float)(num19 + 1) * num;
						if (num21 > num3)
						{
							uvMax.x = vector5.x + (vector6.x - vector5.x) * (num3 - num20) / (num21 - num20);
							num21 = num3;
						}
						AddQuad(toFill, new Vector2(num20, num17) + pixelAdjustedRect.position, new Vector2(num21, num18) + pixelAdjustedRect.position, color, vector5, uvMax);
					}
				}
			}
			if (!hasBorder)
			{
				return;
			}
			uvMax = vector6;
			for (long num22 = 0L; num22 < num6; num22++)
			{
				float num23 = y + (float)num22 * num2;
				float num24 = y + (float)(num22 + 1) * num2;
				if (num24 > num4)
				{
					uvMax.y = vector5.y + (vector6.y - vector5.y) * (num4 - num23) / (num24 - num23);
					num24 = num4;
				}
				AddQuad(toFill, new Vector2(0f, num23) + pixelAdjustedRect.position, new Vector2(x, num24) + pixelAdjustedRect.position, color, new Vector2(vector.x, vector5.y), new Vector2(vector5.x, uvMax.y));
				AddQuad(toFill, new Vector2(num3, num23) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, num24) + pixelAdjustedRect.position, color, new Vector2(vector6.x, vector5.y), new Vector2(vector.z, uvMax.y));
			}
			uvMax = vector6;
			for (long num25 = 0L; num25 < num5; num25++)
			{
				float num26 = x + (float)num25 * num;
				float num27 = x + (float)(num25 + 1) * num;
				if (num27 > num3)
				{
					uvMax.x = vector5.x + (vector6.x - vector5.x) * (num3 - num26) / (num27 - num26);
					num27 = num3;
				}
				AddQuad(toFill, new Vector2(num26, 0f) + pixelAdjustedRect.position, new Vector2(num27, y) + pixelAdjustedRect.position, color, new Vector2(vector5.x, vector.y), new Vector2(uvMax.x, vector5.y));
				AddQuad(toFill, new Vector2(num26, num4) + pixelAdjustedRect.position, new Vector2(num27, pixelAdjustedRect.height) + pixelAdjustedRect.position, color, new Vector2(vector5.x, vector6.y), new Vector2(uvMax.x, vector.w));
			}
			AddQuad(toFill, new Vector2(0f, 0f) + pixelAdjustedRect.position, new Vector2(x, y) + pixelAdjustedRect.position, color, new Vector2(vector.x, vector.y), new Vector2(vector5.x, vector5.y));
			AddQuad(toFill, new Vector2(num3, 0f) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, y) + pixelAdjustedRect.position, color, new Vector2(vector6.x, vector.y), new Vector2(vector.z, vector5.y));
			AddQuad(toFill, new Vector2(0f, num4) + pixelAdjustedRect.position, new Vector2(x, pixelAdjustedRect.height) + pixelAdjustedRect.position, color, new Vector2(vector.x, vector6.y), new Vector2(vector5.x, vector.w));
			AddQuad(toFill, new Vector2(num3, num4) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, pixelAdjustedRect.height) + pixelAdjustedRect.position, color, new Vector2(vector6.x, vector6.y), new Vector2(vector.z, vector.w));
		}
		else
		{
			Vector2 b = new Vector2((num3 - x) / num, (num4 - y) / num2);
			if (m_FillCenter)
			{
				AddQuad(toFill, new Vector2(x, y) + pixelAdjustedRect.position, new Vector2(num3, num4) + pixelAdjustedRect.position, color, Vector2.Scale(vector5, b), Vector2.Scale(vector6, b));
			}
		}
	}

	private static void AddQuad(VertexHelper vertexHelper, Vector3[] quadPositions, Color32 color, Vector3[] quadUVs)
	{
		int currentVertCount = vertexHelper.currentVertCount;
		for (int i = 0; i < 4; i++)
		{
			vertexHelper.AddVert(quadPositions[i], color, quadUVs[i]);
		}
		vertexHelper.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
		vertexHelper.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
	}

	private static void AddQuad(VertexHelper vertexHelper, Vector2 posMin, Vector2 posMax, Color32 color, Vector2 uvMin, Vector2 uvMax)
	{
		int currentVertCount = vertexHelper.currentVertCount;
		vertexHelper.AddVert(new Vector3(posMin.x, posMin.y, 0f), color, new Vector2(uvMin.x, uvMin.y));
		vertexHelper.AddVert(new Vector3(posMin.x, posMax.y, 0f), color, new Vector2(uvMin.x, uvMax.y));
		vertexHelper.AddVert(new Vector3(posMax.x, posMax.y, 0f), color, new Vector2(uvMax.x, uvMax.y));
		vertexHelper.AddVert(new Vector3(posMax.x, posMin.y, 0f), color, new Vector2(uvMax.x, uvMin.y));
		vertexHelper.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
		vertexHelper.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
	}

	private Vector4 GetAdjustedBorders(Vector4 border, Rect adjustedRect)
	{
		Rect rect = base.rectTransform.rect;
		for (int i = 0; i <= 1; i++)
		{
			if (rect.size[i] != 0f)
			{
				float num = adjustedRect.size[i] / rect.size[i];
				border[i] *= num;
				border[i + 2] *= num;
			}
			float num2 = border[i] + border[i + 2];
			if (adjustedRect.size[i] < num2 && num2 != 0f)
			{
				float num = adjustedRect.size[i] / num2;
				border[i] *= num;
				border[i + 2] *= num;
			}
		}
		return border;
	}

	private void GenerateFilledSprite(VertexHelper toFill, bool preserveAspect)
	{
		toFill.Clear();
		if (m_FillAmount < 0.001f)
		{
			return;
		}
		Vector4 drawingDimensions = GetDrawingDimensions(preserveAspect);
		Vector4 vector = ((!(activeSprite != null)) ? Vector4.zero : DataUtility.GetOuterUV(activeSprite));
		UIVertex simpleVert = UIVertex.simpleVert;
		simpleVert.color = color;
		float num = vector.x;
		float num2 = vector.y;
		float num3 = vector.z;
		float num4 = vector.w;
		if (m_FillMethod == FillMethod.Horizontal || m_FillMethod == FillMethod.Vertical)
		{
			if (fillMethod == FillMethod.Horizontal)
			{
				float num5 = (num3 - num) * m_FillAmount;
				if (m_FillOrigin == 1)
				{
					drawingDimensions.x = drawingDimensions.z - (drawingDimensions.z - drawingDimensions.x) * m_FillAmount;
					num = num3 - num5;
				}
				else
				{
					drawingDimensions.z = drawingDimensions.x + (drawingDimensions.z - drawingDimensions.x) * m_FillAmount;
					num3 = num + num5;
				}
			}
			else if (fillMethod == FillMethod.Vertical)
			{
				float num6 = (num4 - num2) * m_FillAmount;
				if (m_FillOrigin == 1)
				{
					drawingDimensions.y = drawingDimensions.w - (drawingDimensions.w - drawingDimensions.y) * m_FillAmount;
					num2 = num4 - num6;
				}
				else
				{
					drawingDimensions.w = drawingDimensions.y + (drawingDimensions.w - drawingDimensions.y) * m_FillAmount;
					num4 = num2 + num6;
				}
			}
		}
		ref Vector3 reference = ref s_Xy[0];
		reference = new Vector2(drawingDimensions.x, drawingDimensions.y);
		ref Vector3 reference2 = ref s_Xy[1];
		reference2 = new Vector2(drawingDimensions.x, drawingDimensions.w);
		ref Vector3 reference3 = ref s_Xy[2];
		reference3 = new Vector2(drawingDimensions.z, drawingDimensions.w);
		ref Vector3 reference4 = ref s_Xy[3];
		reference4 = new Vector2(drawingDimensions.z, drawingDimensions.y);
		ref Vector3 reference5 = ref s_Uv[0];
		reference5 = new Vector2(num, num2);
		ref Vector3 reference6 = ref s_Uv[1];
		reference6 = new Vector2(num, num4);
		ref Vector3 reference7 = ref s_Uv[2];
		reference7 = new Vector2(num3, num4);
		ref Vector3 reference8 = ref s_Uv[3];
		reference8 = new Vector2(num3, num2);
		if (m_FillAmount < 1f && m_FillMethod != 0 && m_FillMethod != FillMethod.Vertical)
		{
			if (fillMethod == FillMethod.Radial90)
			{
				if (RadialCut(s_Xy, s_Uv, m_FillAmount, m_FillClockwise, m_FillOrigin))
				{
					AddQuad(toFill, s_Xy, color, s_Uv);
				}
			}
			else if (fillMethod == FillMethod.Radial180)
			{
				for (int i = 0; i < 2; i++)
				{
					int num7 = ((m_FillOrigin > 1) ? 1 : 0);
					float t;
					float t2;
					float t3;
					float t4;
					if (m_FillOrigin == 0 || m_FillOrigin == 2)
					{
						t = 0f;
						t2 = 1f;
						if (i == num7)
						{
							t3 = 0f;
							t4 = 0.5f;
						}
						else
						{
							t3 = 0.5f;
							t4 = 1f;
						}
					}
					else
					{
						t3 = 0f;
						t4 = 1f;
						if (i == num7)
						{
							t = 0.5f;
							t2 = 1f;
						}
						else
						{
							t = 0f;
							t2 = 0.5f;
						}
					}
					s_Xy[0].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, t3);
					s_Xy[1].x = s_Xy[0].x;
					s_Xy[2].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, t4);
					s_Xy[3].x = s_Xy[2].x;
					s_Xy[0].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, t);
					s_Xy[1].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, t2);
					s_Xy[2].y = s_Xy[1].y;
					s_Xy[3].y = s_Xy[0].y;
					s_Uv[0].x = Mathf.Lerp(num, num3, t3);
					s_Uv[1].x = s_Uv[0].x;
					s_Uv[2].x = Mathf.Lerp(num, num3, t4);
					s_Uv[3].x = s_Uv[2].x;
					s_Uv[0].y = Mathf.Lerp(num2, num4, t);
					s_Uv[1].y = Mathf.Lerp(num2, num4, t2);
					s_Uv[2].y = s_Uv[1].y;
					s_Uv[3].y = s_Uv[0].y;
					float value = ((!m_FillClockwise) ? (m_FillAmount * 2f - (float)(1 - i)) : (fillAmount * 2f - (float)i));
					if (RadialCut(s_Xy, s_Uv, Mathf.Clamp01(value), m_FillClockwise, (i + m_FillOrigin + 3) % 4))
					{
						AddQuad(toFill, s_Xy, color, s_Uv);
					}
				}
			}
			else
			{
				if (fillMethod != FillMethod.Radial360)
				{
					return;
				}
				for (int j = 0; j < 4; j++)
				{
					float t5;
					float t6;
					if (j < 2)
					{
						t5 = 0f;
						t6 = 0.5f;
					}
					else
					{
						t5 = 0.5f;
						t6 = 1f;
					}
					float t7;
					float t8;
					if (j == 0 || j == 3)
					{
						t7 = 0f;
						t8 = 0.5f;
					}
					else
					{
						t7 = 0.5f;
						t8 = 1f;
					}
					s_Xy[0].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, t5);
					s_Xy[1].x = s_Xy[0].x;
					s_Xy[2].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, t6);
					s_Xy[3].x = s_Xy[2].x;
					s_Xy[0].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, t7);
					s_Xy[1].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, t8);
					s_Xy[2].y = s_Xy[1].y;
					s_Xy[3].y = s_Xy[0].y;
					s_Uv[0].x = Mathf.Lerp(num, num3, t5);
					s_Uv[1].x = s_Uv[0].x;
					s_Uv[2].x = Mathf.Lerp(num, num3, t6);
					s_Uv[3].x = s_Uv[2].x;
					s_Uv[0].y = Mathf.Lerp(num2, num4, t7);
					s_Uv[1].y = Mathf.Lerp(num2, num4, t8);
					s_Uv[2].y = s_Uv[1].y;
					s_Uv[3].y = s_Uv[0].y;
					float value2 = ((!m_FillClockwise) ? (m_FillAmount * 4f - (float)(3 - (j + m_FillOrigin) % 4)) : (m_FillAmount * 4f - (float)((j + m_FillOrigin) % 4)));
					if (RadialCut(s_Xy, s_Uv, Mathf.Clamp01(value2), m_FillClockwise, (j + 2) % 4))
					{
						AddQuad(toFill, s_Xy, color, s_Uv);
					}
				}
			}
		}
		else
		{
			AddQuad(toFill, s_Xy, color, s_Uv);
		}
	}

	private static bool RadialCut(Vector3[] xy, Vector3[] uv, float fill, bool invert, int corner)
	{
		if (fill < 0.001f)
		{
			return false;
		}
		if ((corner & 1) == 1)
		{
			invert = !invert;
		}
		if (!invert && fill > 0.999f)
		{
			return true;
		}
		float num = Mathf.Clamp01(fill);
		if (invert)
		{
			num = 1f - num;
		}
		num *= (float)Math.PI / 2f;
		float cos = Mathf.Cos(num);
		float sin = Mathf.Sin(num);
		RadialCut(xy, cos, sin, invert, corner);
		RadialCut(uv, cos, sin, invert, corner);
		return true;
	}

	private static void RadialCut(Vector3[] xy, float cos, float sin, bool invert, int corner)
	{
		int num = (corner + 1) % 4;
		int num2 = (corner + 2) % 4;
		int num3 = (corner + 3) % 4;
		if ((corner & 1) == 1)
		{
			if (sin > cos)
			{
				cos /= sin;
				sin = 1f;
				if (invert)
				{
					xy[num].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
					xy[num2].x = xy[num].x;
				}
			}
			else if (cos > sin)
			{
				sin /= cos;
				cos = 1f;
				if (!invert)
				{
					xy[num2].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
					xy[num3].y = xy[num2].y;
				}
			}
			else
			{
				cos = 1f;
				sin = 1f;
			}
			if (!invert)
			{
				xy[num3].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
			}
			else
			{
				xy[num].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
			}
			return;
		}
		if (cos > sin)
		{
			sin /= cos;
			cos = 1f;
			if (!invert)
			{
				xy[num].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
				xy[num2].y = xy[num].y;
			}
		}
		else if (sin > cos)
		{
			cos /= sin;
			sin = 1f;
			if (invert)
			{
				xy[num2].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
				xy[num3].x = xy[num2].x;
			}
		}
		else
		{
			cos = 1f;
			sin = 1f;
		}
		if (invert)
		{
			xy[num3].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
		}
		else
		{
			xy[num].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
		}
	}

	public virtual void CalculateLayoutInputHorizontal()
	{
	}

	public virtual void CalculateLayoutInputVertical()
	{
	}

	public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
	{
		if (alphaHitTestMinimumThreshold <= 0f)
		{
			return true;
		}
		if (alphaHitTestMinimumThreshold > 1f)
		{
			return false;
		}
		if (activeSprite == null)
		{
			return true;
		}
		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(base.rectTransform, screenPoint, eventCamera, out var localPoint))
		{
			return false;
		}
		Rect pixelAdjustedRect = GetPixelAdjustedRect();
		localPoint.x += base.rectTransform.pivot.x * pixelAdjustedRect.width;
		localPoint.y += base.rectTransform.pivot.y * pixelAdjustedRect.height;
		localPoint = MapCoordinate(localPoint, pixelAdjustedRect);
		Rect textureRect = activeSprite.textureRect;
		Vector2 vector = new Vector2(localPoint.x / textureRect.width, localPoint.y / textureRect.height);
		float x = Mathf.Lerp(textureRect.x, textureRect.xMax, vector.x) / (float)activeSprite.texture.width;
		float y = Mathf.Lerp(textureRect.y, textureRect.yMax, vector.y) / (float)activeSprite.texture.height;
		try
		{
			return activeSprite.texture.GetPixelBilinear(x, y).a >= alphaHitTestMinimumThreshold;
		}
		catch (UnityException ex)
		{
			Debug.LogError("Using alphaHitTestMinimumThreshold greater than 0 on Image whose sprite texture cannot be read. " + ex.Message + " Also make sure to disable sprite packing for this sprite.", this);
			return true;
		}
	}

	private Vector2 MapCoordinate(Vector2 local, Rect rect)
	{
		Rect rect2 = activeSprite.rect;
		if (type == Type.Simple || type == Type.Filled)
		{
			return new Vector2(local.x * rect2.width / rect.width, local.y * rect2.height / rect.height);
		}
		Vector4 border = activeSprite.border;
		Vector4 adjustedBorders = GetAdjustedBorders(border / pixelsPerUnit, rect);
		for (int i = 0; i < 2; i++)
		{
			if (!(local[i] <= adjustedBorders[i]))
			{
				if (rect.size[i] - local[i] <= adjustedBorders[i + 2])
				{
					local[i] -= rect.size[i] - rect2.size[i];
				}
				else if (type == Type.Sliced)
				{
					float t = Mathf.InverseLerp(adjustedBorders[i], rect.size[i] - adjustedBorders[i + 2], local[i]);
					local[i] = Mathf.Lerp(border[i], rect2.size[i] - border[i + 2], t);
				}
				else
				{
					local[i] -= adjustedBorders[i];
					local[i] = Mathf.Repeat(local[i], rect2.size[i] - border[i] - border[i + 2]);
					local[i] += border[i];
				}
			}
		}
		return local;
	}
}

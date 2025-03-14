using System.ComponentModel;

namespace System.Drawing.Drawing2D;

[System.MonoTODO("libgdiplus/cairo doesn't support path gradients - unless it can be mapped to a radial gradient")]
public sealed class PathGradientBrush : Brush
{
	public Blend Blend
	{
		get
		{
			Status status = GDIPlus.GdipGetPathGradientBlendCount(nativeObject, out var count);
			GDIPlus.CheckStatus(status);
			float[] array = new float[count];
			float[] positions = new float[count];
			status = GDIPlus.GdipGetPathGradientBlend(nativeObject, array, positions, count);
			GDIPlus.CheckStatus(status);
			Blend blend = new Blend();
			blend.Factors = array;
			blend.Positions = positions;
			return blend;
		}
		set
		{
			float[] factors = value.Factors;
			float[] positions = value.Positions;
			int num = factors.Length;
			if (num == 0 || positions.Length == 0)
			{
				throw new ArgumentException("Invalid Blend object. It should have at least 2 elements in each of the factors and positions arrays.");
			}
			if (num != positions.Length)
			{
				throw new ArgumentException("Invalid Blend object. It should contain the same number of factors and positions values.");
			}
			if (positions[0] != 0f)
			{
				throw new ArgumentException("Invalid Blend object. The positions array must have 0.0 as its first element.");
			}
			if (positions[num - 1] != 1f)
			{
				throw new ArgumentException("Invalid Blend object. The positions array must have 1.0 as its last element.");
			}
			Status status = GDIPlus.GdipSetPathGradientBlend(nativeObject, factors, positions, num);
			GDIPlus.CheckStatus(status);
		}
	}

	public Color CenterColor
	{
		get
		{
			int color;
			Status status = GDIPlus.GdipGetPathGradientCenterColor(nativeObject, out color);
			GDIPlus.CheckStatus(status);
			return Color.FromArgb(color);
		}
		set
		{
			Status status = GDIPlus.GdipSetPathGradientCenterColor(nativeObject, value.ToArgb());
			GDIPlus.CheckStatus(status);
		}
	}

	public PointF CenterPoint
	{
		get
		{
			PointF point;
			Status status = GDIPlus.GdipGetPathGradientCenterPoint(nativeObject, out point);
			GDIPlus.CheckStatus(status);
			return point;
		}
		set
		{
			PointF point = value;
			Status status = GDIPlus.GdipSetPathGradientCenterPoint(nativeObject, ref point);
			GDIPlus.CheckStatus(status);
		}
	}

	public PointF FocusScales
	{
		get
		{
			float xScale;
			float yScale;
			Status status = GDIPlus.GdipGetPathGradientFocusScales(nativeObject, out xScale, out yScale);
			GDIPlus.CheckStatus(status);
			return new PointF(xScale, yScale);
		}
		set
		{
			Status status = GDIPlus.GdipSetPathGradientFocusScales(nativeObject, value.X, value.Y);
			GDIPlus.CheckStatus(status);
		}
	}

	public ColorBlend InterpolationColors
	{
		get
		{
			Status status = GDIPlus.GdipGetPathGradientPresetBlendCount(nativeObject, out var count);
			GDIPlus.CheckStatus(status);
			if (count < 1)
			{
				count = 1;
			}
			int[] array = new int[count];
			float[] positions = new float[count];
			if (count > 1)
			{
				status = GDIPlus.GdipGetPathGradientPresetBlend(nativeObject, array, positions, count);
				GDIPlus.CheckStatus(status);
			}
			ColorBlend colorBlend = new ColorBlend();
			Color[] array2 = new Color[count];
			for (int i = 0; i < count; i++)
			{
				ref Color reference = ref array2[i];
				reference = Color.FromArgb(array[i]);
			}
			colorBlend.Colors = array2;
			colorBlend.Positions = positions;
			return colorBlend;
		}
		set
		{
			Color[] colors = value.Colors;
			float[] positions = value.Positions;
			int num = colors.Length;
			if (num == 0 || positions.Length == 0)
			{
				throw new ArgumentException("Invalid ColorBlend object. It should have at least 2 elements in each of the colors and positions arrays.");
			}
			if (num != positions.Length)
			{
				throw new ArgumentException("Invalid ColorBlend object. It should contain the same number of positions and color values.");
			}
			if (positions[0] != 0f)
			{
				throw new ArgumentException("Invalid ColorBlend object. The positions array must have 0.0 as its first element.");
			}
			if (positions[num - 1] != 1f)
			{
				throw new ArgumentException("Invalid ColorBlend object. The positions array must have 1.0 as its last element.");
			}
			int[] array = new int[colors.Length];
			for (int i = 0; i < colors.Length; i++)
			{
				array[i] = colors[i].ToArgb();
			}
			Status status = GDIPlus.GdipSetPathGradientPresetBlend(nativeObject, array, positions, num);
			GDIPlus.CheckStatus(status);
		}
	}

	public RectangleF Rectangle
	{
		get
		{
			RectangleF rect;
			Status status = GDIPlus.GdipGetPathGradientRect(nativeObject, out rect);
			GDIPlus.CheckStatus(status);
			return rect;
		}
	}

	public Color[] SurroundColors
	{
		get
		{
			Status status = GDIPlus.GdipGetPathGradientSurroundColorCount(nativeObject, out var count);
			GDIPlus.CheckStatus(status);
			int[] array = new int[count];
			status = GDIPlus.GdipGetPathGradientSurroundColorsWithCount(nativeObject, array, ref count);
			GDIPlus.CheckStatus(status);
			Color[] array2 = new Color[count];
			for (int i = 0; i < count; i++)
			{
				ref Color reference = ref array2[i];
				reference = Color.FromArgb(array[i]);
			}
			return array2;
		}
		set
		{
			int count = value.Length;
			int[] array = new int[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = value[i].ToArgb();
			}
			Status status = GDIPlus.GdipSetPathGradientSurroundColorsWithCount(nativeObject, array, ref count);
			GDIPlus.CheckStatus(status);
		}
	}

	public Matrix Transform
	{
		get
		{
			Matrix matrix = new Matrix();
			Status status = GDIPlus.GdipGetPathGradientTransform(nativeObject, matrix.nativeMatrix);
			GDIPlus.CheckStatus(status);
			return matrix;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("Transform");
			}
			Status status = GDIPlus.GdipSetPathGradientTransform(nativeObject, value.nativeMatrix);
			GDIPlus.CheckStatus(status);
		}
	}

	public WrapMode WrapMode
	{
		get
		{
			WrapMode wrapMode;
			Status status = GDIPlus.GdipGetPathGradientWrapMode(nativeObject, out wrapMode);
			GDIPlus.CheckStatus(status);
			return wrapMode;
		}
		set
		{
			if (value < WrapMode.Tile || value > WrapMode.Clamp)
			{
				throw new InvalidEnumArgumentException("WrapMode");
			}
			Status status = GDIPlus.GdipSetPathGradientWrapMode(nativeObject, value);
			GDIPlus.CheckStatus(status);
		}
	}

	internal PathGradientBrush(IntPtr native)
		: base(native)
	{
	}

	public PathGradientBrush(GraphicsPath path)
	{
		if (path == null)
		{
			throw new ArgumentNullException("path");
		}
		Status status = GDIPlus.GdipCreatePathGradientFromPath(path.NativeObject, out nativeObject);
		GDIPlus.CheckStatus(status);
	}

	public PathGradientBrush(Point[] points)
		: this(points, WrapMode.Clamp)
	{
	}

	public PathGradientBrush(PointF[] points)
		: this(points, WrapMode.Clamp)
	{
	}

	public PathGradientBrush(Point[] points, WrapMode wrapMode)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		if (wrapMode < WrapMode.Tile || wrapMode > WrapMode.Clamp)
		{
			throw new InvalidEnumArgumentException("WrapMode");
		}
		Status status = GDIPlus.GdipCreatePathGradientI(points, points.Length, wrapMode, out nativeObject);
		GDIPlus.CheckStatus(status);
	}

	public PathGradientBrush(PointF[] points, WrapMode wrapMode)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		if (wrapMode < WrapMode.Tile || wrapMode > WrapMode.Clamp)
		{
			throw new InvalidEnumArgumentException("WrapMode");
		}
		Status status = GDIPlus.GdipCreatePathGradient(points, points.Length, wrapMode, out nativeObject);
		GDIPlus.CheckStatus(status);
	}

	public void MultiplyTransform(Matrix matrix)
	{
		MultiplyTransform(matrix, MatrixOrder.Prepend);
	}

	public void MultiplyTransform(Matrix matrix, MatrixOrder order)
	{
		if (matrix == null)
		{
			throw new ArgumentNullException("matrix");
		}
		Status status = GDIPlus.GdipMultiplyPathGradientTransform(nativeObject, matrix.nativeMatrix, order);
		GDIPlus.CheckStatus(status);
	}

	public void ResetTransform()
	{
		Status status = GDIPlus.GdipResetPathGradientTransform(nativeObject);
		GDIPlus.CheckStatus(status);
	}

	public void RotateTransform(float angle)
	{
		RotateTransform(angle, MatrixOrder.Prepend);
	}

	public void RotateTransform(float angle, MatrixOrder order)
	{
		Status status = GDIPlus.GdipRotatePathGradientTransform(nativeObject, angle, order);
		GDIPlus.CheckStatus(status);
	}

	public void ScaleTransform(float sx, float sy)
	{
		ScaleTransform(sx, sy, MatrixOrder.Prepend);
	}

	public void ScaleTransform(float sx, float sy, MatrixOrder order)
	{
		Status status = GDIPlus.GdipScalePathGradientTransform(nativeObject, sx, sy, order);
		GDIPlus.CheckStatus(status);
	}

	public void SetBlendTriangularShape(float focus)
	{
		SetBlendTriangularShape(focus, 1f);
	}

	public void SetBlendTriangularShape(float focus, float scale)
	{
		if (focus < 0f || focus > 1f || scale < 0f || scale > 1f)
		{
			throw new ArgumentException("Invalid parameter passed.");
		}
		Status status = GDIPlus.GdipSetPathGradientLinearBlend(nativeObject, focus, scale);
		GDIPlus.CheckStatus(status);
	}

	public void SetSigmaBellShape(float focus)
	{
		SetSigmaBellShape(focus, 1f);
	}

	public void SetSigmaBellShape(float focus, float scale)
	{
		if (focus < 0f || focus > 1f || scale < 0f || scale > 1f)
		{
			throw new ArgumentException("Invalid parameter passed.");
		}
		Status status = GDIPlus.GdipSetPathGradientSigmaBlend(nativeObject, focus, scale);
		GDIPlus.CheckStatus(status);
	}

	public void TranslateTransform(float dx, float dy)
	{
		TranslateTransform(dx, dy, MatrixOrder.Prepend);
	}

	public void TranslateTransform(float dx, float dy, MatrixOrder order)
	{
		Status status = GDIPlus.GdipTranslatePathGradientTransform(nativeObject, dx, dy, order);
		GDIPlus.CheckStatus(status);
	}

	public override object Clone()
	{
		IntPtr clonedBrush;
		Status status = GDIPlus.GdipCloneBrush(nativeObject, out clonedBrush);
		GDIPlus.CheckStatus(status);
		return new PathGradientBrush(clonedBrush);
	}
}

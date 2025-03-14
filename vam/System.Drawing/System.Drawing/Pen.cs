using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace System.Drawing;

public sealed class Pen : MarshalByRefObject, IDisposable, ICloneable
{
	internal IntPtr nativeObject;

	internal bool isModifiable = true;

	private Color color;

	private CustomLineCap startCap;

	private CustomLineCap endCap;

	[System.MonoLimitation("Libgdiplus doesn't use this property for rendering")]
	public PenAlignment Alignment
	{
		get
		{
			PenAlignment alignment;
			Status status = GDIPlus.GdipGetPenMode(nativeObject, out alignment);
			GDIPlus.CheckStatus(status);
			return alignment;
		}
		set
		{
			if (value < PenAlignment.Center || value > PenAlignment.Right)
			{
				throw new InvalidEnumArgumentException("Alignment", (int)value, typeof(PenAlignment));
			}
			if (isModifiable)
			{
				Status status = GDIPlus.GdipSetPenMode(nativeObject, value);
				GDIPlus.CheckStatus(status);
				return;
			}
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
	}

	public Brush Brush
	{
		get
		{
			IntPtr brush;
			Status status = GDIPlus.GdipGetPenBrushFill(nativeObject, out brush);
			GDIPlus.CheckStatus(status);
			return new SolidBrush(brush);
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("Brush");
			}
			if (!isModifiable)
			{
				throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
			}
			Status status = GDIPlus.GdipSetPenBrushFill(nativeObject, value.nativeObject);
			GDIPlus.CheckStatus(status);
			color = Color.Empty;
		}
	}

	public Color Color
	{
		get
		{
			if (color.Equals(Color.Empty))
			{
				int argb;
				Status status = GDIPlus.GdipGetPenColor(nativeObject, out argb);
				GDIPlus.CheckStatus(status);
				color = Color.FromArgb(argb);
			}
			return color;
		}
		set
		{
			if (!isModifiable)
			{
				throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
			}
			Status status = GDIPlus.GdipSetPenColor(nativeObject, value.ToArgb());
			GDIPlus.CheckStatus(status);
			color = value;
		}
	}

	public float[] CompoundArray
	{
		get
		{
			Status status = GDIPlus.GdipGetPenCompoundCount(nativeObject, out var count);
			GDIPlus.CheckStatus(status);
			float[] array = new float[count];
			status = GDIPlus.GdipGetPenCompoundArray(nativeObject, array, count);
			GDIPlus.CheckStatus(status);
			return array;
		}
		set
		{
			if (isModifiable)
			{
				int num = value.Length;
				if (num < 2)
				{
					throw new ArgumentException("Invalid parameter.");
				}
				foreach (float num2 in value)
				{
					if (num2 < 0f || num2 > 1f)
					{
						throw new ArgumentException("Invalid parameter.");
					}
				}
				Status status = GDIPlus.GdipSetPenCompoundArray(nativeObject, value, value.Length);
				GDIPlus.CheckStatus(status);
				return;
			}
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
	}

	public CustomLineCap CustomEndCap
	{
		get
		{
			return endCap;
		}
		set
		{
			if (isModifiable)
			{
				Status status = GDIPlus.GdipSetPenCustomEndCap(nativeObject, value.nativeObject);
				GDIPlus.CheckStatus(status);
				endCap = value;
				return;
			}
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
	}

	public CustomLineCap CustomStartCap
	{
		get
		{
			return startCap;
		}
		set
		{
			if (isModifiable)
			{
				Status status = GDIPlus.GdipSetPenCustomStartCap(nativeObject, value.nativeObject);
				GDIPlus.CheckStatus(status);
				startCap = value;
				return;
			}
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
	}

	public DashCap DashCap
	{
		get
		{
			DashCap dashCap;
			Status status = GDIPlus.GdipGetPenDashCap197819(nativeObject, out dashCap);
			GDIPlus.CheckStatus(status);
			return dashCap;
		}
		set
		{
			if (value < DashCap.Flat || value > DashCap.Triangle)
			{
				throw new InvalidEnumArgumentException("DashCap", (int)value, typeof(DashCap));
			}
			if (isModifiable)
			{
				Status status = GDIPlus.GdipSetPenDashCap197819(nativeObject, value);
				GDIPlus.CheckStatus(status);
				return;
			}
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
	}

	public float DashOffset
	{
		get
		{
			float offset;
			Status status = GDIPlus.GdipGetPenDashOffset(nativeObject, out offset);
			GDIPlus.CheckStatus(status);
			return offset;
		}
		set
		{
			if (isModifiable)
			{
				Status status = GDIPlus.GdipSetPenDashOffset(nativeObject, value);
				GDIPlus.CheckStatus(status);
				return;
			}
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
	}

	public float[] DashPattern
	{
		get
		{
			Status status = GDIPlus.GdipGetPenDashCount(nativeObject, out var count);
			GDIPlus.CheckStatus(status);
			float[] array;
			if (count <= 0)
			{
				array = ((DashStyle != DashStyle.Custom) ? new float[0] : new float[1] { 1f });
			}
			else
			{
				array = new float[count];
				status = GDIPlus.GdipGetPenDashArray(nativeObject, array, count);
				GDIPlus.CheckStatus(status);
			}
			return array;
		}
		set
		{
			if (isModifiable)
			{
				if (value.Length == 0)
				{
					throw new ArgumentException("Invalid parameter.");
				}
				foreach (float num in value)
				{
					if (num <= 0f)
					{
						throw new ArgumentException("Invalid parameter.");
					}
				}
				Status status = GDIPlus.GdipSetPenDashArray(nativeObject, value, value.Length);
				GDIPlus.CheckStatus(status);
				return;
			}
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
	}

	public DashStyle DashStyle
	{
		get
		{
			DashStyle dashStyle;
			Status status = GDIPlus.GdipGetPenDashStyle(nativeObject, out dashStyle);
			GDIPlus.CheckStatus(status);
			return dashStyle;
		}
		set
		{
			if (value < DashStyle.Solid || value > DashStyle.Custom)
			{
				throw new InvalidEnumArgumentException("DashStyle", (int)value, typeof(DashStyle));
			}
			if (isModifiable)
			{
				Status status = GDIPlus.GdipSetPenDashStyle(nativeObject, value);
				GDIPlus.CheckStatus(status);
				return;
			}
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
	}

	public LineCap StartCap
	{
		get
		{
			LineCap result;
			Status status = GDIPlus.GdipGetPenStartCap(nativeObject, out result);
			GDIPlus.CheckStatus(status);
			return result;
		}
		set
		{
			if (value < LineCap.Flat || value > LineCap.Custom)
			{
				throw new InvalidEnumArgumentException("StartCap", (int)value, typeof(LineCap));
			}
			if (isModifiable)
			{
				Status status = GDIPlus.GdipSetPenStartCap(nativeObject, value);
				GDIPlus.CheckStatus(status);
				return;
			}
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
	}

	public LineCap EndCap
	{
		get
		{
			LineCap result;
			Status status = GDIPlus.GdipGetPenEndCap(nativeObject, out result);
			GDIPlus.CheckStatus(status);
			return result;
		}
		set
		{
			if (value < LineCap.Flat || value > LineCap.Custom)
			{
				throw new InvalidEnumArgumentException("EndCap", (int)value, typeof(LineCap));
			}
			if (isModifiable)
			{
				Status status = GDIPlus.GdipSetPenEndCap(nativeObject, value);
				GDIPlus.CheckStatus(status);
				return;
			}
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
	}

	public LineJoin LineJoin
	{
		get
		{
			LineJoin lineJoin;
			Status status = GDIPlus.GdipGetPenLineJoin(nativeObject, out lineJoin);
			GDIPlus.CheckStatus(status);
			return lineJoin;
		}
		set
		{
			if (value < LineJoin.Miter || value > LineJoin.MiterClipped)
			{
				throw new InvalidEnumArgumentException("LineJoin", (int)value, typeof(LineJoin));
			}
			if (isModifiable)
			{
				Status status = GDIPlus.GdipSetPenLineJoin(nativeObject, value);
				GDIPlus.CheckStatus(status);
				return;
			}
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
	}

	public float MiterLimit
	{
		get
		{
			float miterLimit;
			Status status = GDIPlus.GdipGetPenMiterLimit(nativeObject, out miterLimit);
			GDIPlus.CheckStatus(status);
			return miterLimit;
		}
		set
		{
			if (isModifiable)
			{
				Status status = GDIPlus.GdipSetPenMiterLimit(nativeObject, value);
				GDIPlus.CheckStatus(status);
				return;
			}
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
	}

	public PenType PenType
	{
		get
		{
			PenType type;
			Status status = GDIPlus.GdipGetPenFillType(nativeObject, out type);
			GDIPlus.CheckStatus(status);
			return type;
		}
	}

	public Matrix Transform
	{
		get
		{
			Matrix matrix = new Matrix();
			Status status = GDIPlus.GdipGetPenTransform(nativeObject, matrix.nativeMatrix);
			GDIPlus.CheckStatus(status);
			return matrix;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("Transform");
			}
			if (isModifiable)
			{
				Status status = GDIPlus.GdipSetPenTransform(nativeObject, value.nativeMatrix);
				GDIPlus.CheckStatus(status);
				return;
			}
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
	}

	public float Width
	{
		get
		{
			float width;
			Status status = GDIPlus.GdipGetPenWidth(nativeObject, out width);
			GDIPlus.CheckStatus(status);
			return width;
		}
		set
		{
			if (isModifiable)
			{
				Status status = GDIPlus.GdipSetPenWidth(nativeObject, value);
				GDIPlus.CheckStatus(status);
				return;
			}
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
	}

	internal Pen(IntPtr p)
	{
		nativeObject = p;
	}

	public Pen(Brush brush)
		: this(brush, 1f)
	{
	}

	public Pen(Color color)
		: this(color, 1f)
	{
	}

	public Pen(Brush brush, float width)
	{
		if (brush == null)
		{
			throw new ArgumentNullException("brush");
		}
		Status status = GDIPlus.GdipCreatePen2(brush.nativeObject, width, GraphicsUnit.World, out nativeObject);
		GDIPlus.CheckStatus(status);
		color = Color.Empty;
	}

	public Pen(Color color, float width)
	{
		Status status = GDIPlus.GdipCreatePen1(color.ToArgb(), width, GraphicsUnit.World, out nativeObject);
		GDIPlus.CheckStatus(status);
		this.color = color;
	}

	public object Clone()
	{
		IntPtr clonepen;
		Status status = GDIPlus.GdipClonePen(nativeObject, out clonepen);
		GDIPlus.CheckStatus(status);
		Pen pen = new Pen(clonepen);
		pen.startCap = startCap;
		pen.endCap = endCap;
		return pen;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (disposing && !isModifiable)
		{
			throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
		}
		if (nativeObject != IntPtr.Zero)
		{
			Status status = GDIPlus.GdipDeletePen(nativeObject);
			nativeObject = IntPtr.Zero;
			GDIPlus.CheckStatus(status);
		}
	}

	~Pen()
	{
		Dispose(disposing: false);
	}

	public void MultiplyTransform(Matrix matrix)
	{
		MultiplyTransform(matrix, MatrixOrder.Prepend);
	}

	public void MultiplyTransform(Matrix matrix, MatrixOrder order)
	{
		Status status = GDIPlus.GdipMultiplyPenTransform(nativeObject, matrix.nativeMatrix, order);
		GDIPlus.CheckStatus(status);
	}

	public void ResetTransform()
	{
		Status status = GDIPlus.GdipResetPenTransform(nativeObject);
		GDIPlus.CheckStatus(status);
	}

	public void RotateTransform(float angle)
	{
		RotateTransform(angle, MatrixOrder.Prepend);
	}

	public void RotateTransform(float angle, MatrixOrder order)
	{
		Status status = GDIPlus.GdipRotatePenTransform(nativeObject, angle, order);
		GDIPlus.CheckStatus(status);
	}

	public void ScaleTransform(float sx, float sy)
	{
		ScaleTransform(sx, sy, MatrixOrder.Prepend);
	}

	public void ScaleTransform(float sx, float sy, MatrixOrder order)
	{
		Status status = GDIPlus.GdipScalePenTransform(nativeObject, sx, sy, order);
		GDIPlus.CheckStatus(status);
	}

	public void SetLineCap(LineCap startCap, LineCap endCap, DashCap dashCap)
	{
		if (isModifiable)
		{
			Status status = GDIPlus.GdipSetPenLineCap197819(nativeObject, startCap, endCap, dashCap);
			GDIPlus.CheckStatus(status);
			return;
		}
		throw new ArgumentException(global::Locale.GetText("This Pen object can't be modified."));
	}

	public void TranslateTransform(float dx, float dy)
	{
		TranslateTransform(dx, dy, MatrixOrder.Prepend);
	}

	public void TranslateTransform(float dx, float dy, MatrixOrder order)
	{
		Status status = GDIPlus.GdipTranslatePenTransform(nativeObject, dx, dy, order);
		GDIPlus.CheckStatus(status);
	}
}

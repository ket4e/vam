using System.Runtime.InteropServices;

namespace System.Drawing.Drawing2D;

public sealed class Matrix : MarshalByRefObject, IDisposable
{
	internal IntPtr nativeMatrix;

	public float[] Elements
	{
		get
		{
			float[] array = new float[6];
			IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(float)) * 6);
			try
			{
				Status status = GDIPlus.GdipGetMatrixElements(nativeMatrix, intPtr);
				GDIPlus.CheckStatus(status);
				Marshal.Copy(intPtr, array, 0, 6);
				return array;
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}
	}

	public bool IsIdentity
	{
		get
		{
			bool result;
			Status status = GDIPlus.GdipIsMatrixIdentity(nativeMatrix, out result);
			GDIPlus.CheckStatus(status);
			return result;
		}
	}

	public bool IsInvertible
	{
		get
		{
			bool result;
			Status status = GDIPlus.GdipIsMatrixInvertible(nativeMatrix, out result);
			GDIPlus.CheckStatus(status);
			return result;
		}
	}

	public float OffsetX => Elements[4];

	public float OffsetY => Elements[5];

	internal IntPtr NativeObject
	{
		get
		{
			return nativeMatrix;
		}
		set
		{
			nativeMatrix = value;
		}
	}

	internal Matrix(IntPtr ptr)
	{
		nativeMatrix = ptr;
	}

	public Matrix()
	{
		Status status = GDIPlus.GdipCreateMatrix(out nativeMatrix);
		GDIPlus.CheckStatus(status);
	}

	public Matrix(Rectangle rect, Point[] plgpts)
	{
		if (plgpts == null)
		{
			throw new ArgumentNullException("plgpts");
		}
		if (plgpts.Length != 3)
		{
			throw new ArgumentException("plgpts");
		}
		Status status = GDIPlus.GdipCreateMatrix3I(ref rect, plgpts, out nativeMatrix);
		GDIPlus.CheckStatus(status);
	}

	public Matrix(RectangleF rect, PointF[] plgpts)
	{
		if (plgpts == null)
		{
			throw new ArgumentNullException("plgpts");
		}
		if (plgpts.Length != 3)
		{
			throw new ArgumentException("plgpts");
		}
		Status status = GDIPlus.GdipCreateMatrix3(ref rect, plgpts, out nativeMatrix);
		GDIPlus.CheckStatus(status);
	}

	public Matrix(float m11, float m12, float m21, float m22, float dx, float dy)
	{
		Status status = GDIPlus.GdipCreateMatrix2(m11, m12, m21, m22, dx, dy, out nativeMatrix);
		GDIPlus.CheckStatus(status);
	}

	public Matrix Clone()
	{
		IntPtr cloneMatrix;
		Status status = GDIPlus.GdipCloneMatrix(nativeMatrix, out cloneMatrix);
		GDIPlus.CheckStatus(status);
		return new Matrix(cloneMatrix);
	}

	public void Dispose()
	{
		if (nativeMatrix != IntPtr.Zero)
		{
			Status status = GDIPlus.GdipDeleteMatrix(nativeMatrix);
			GDIPlus.CheckStatus(status);
			nativeMatrix = IntPtr.Zero;
		}
		GC.SuppressFinalize(this);
	}

	public override bool Equals(object obj)
	{
		if (obj is Matrix matrix)
		{
			bool result;
			Status status = GDIPlus.GdipIsMatrixEqual(nativeMatrix, matrix.nativeMatrix, out result);
			GDIPlus.CheckStatus(status);
			return result;
		}
		return false;
	}

	~Matrix()
	{
		Dispose();
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public void Invert()
	{
		Status status = GDIPlus.GdipInvertMatrix(nativeMatrix);
		GDIPlus.CheckStatus(status);
	}

	public void Multiply(Matrix matrix)
	{
		Multiply(matrix, MatrixOrder.Prepend);
	}

	public void Multiply(Matrix matrix, MatrixOrder order)
	{
		if (matrix == null)
		{
			throw new ArgumentNullException("matrix");
		}
		Status status = GDIPlus.GdipMultiplyMatrix(nativeMatrix, matrix.nativeMatrix, order);
		GDIPlus.CheckStatus(status);
	}

	public void Reset()
	{
		Status status = GDIPlus.GdipSetMatrixElements(nativeMatrix, 1f, 0f, 0f, 1f, 0f, 0f);
		GDIPlus.CheckStatus(status);
	}

	public void Rotate(float angle)
	{
		Rotate(angle, MatrixOrder.Prepend);
	}

	public void Rotate(float angle, MatrixOrder order)
	{
		Status status = GDIPlus.GdipRotateMatrix(nativeMatrix, angle, order);
		GDIPlus.CheckStatus(status);
	}

	public void RotateAt(float angle, PointF point)
	{
		RotateAt(angle, point, MatrixOrder.Prepend);
	}

	public void RotateAt(float angle, PointF point, MatrixOrder order)
	{
		if (order < MatrixOrder.Prepend || order > MatrixOrder.Append)
		{
			throw new ArgumentException("order");
		}
		angle *= (float)Math.PI / 180f;
		float num = (float)Math.Cos(angle);
		float num2 = (float)Math.Sin(angle);
		float num3 = (0f - point.X) * num + point.Y * num2 + point.X;
		float num4 = (0f - point.X) * num2 - point.Y * num + point.Y;
		float[] elements = Elements;
		Status status = ((order != 0) ? GDIPlus.GdipSetMatrixElements(nativeMatrix, elements[0] * num + elements[1] * (0f - num2), elements[0] * num2 + elements[1] * num, elements[2] * num + elements[3] * (0f - num2), elements[2] * num2 + elements[3] * num, elements[4] * num + elements[5] * (0f - num2) + num3, elements[4] * num2 + elements[5] * num + num4) : GDIPlus.GdipSetMatrixElements(nativeMatrix, num * elements[0] + num2 * elements[2], num * elements[1] + num2 * elements[3], (0f - num2) * elements[0] + num * elements[2], (0f - num2) * elements[1] + num * elements[3], num3 * elements[0] + num4 * elements[2] + elements[4], num3 * elements[1] + num4 * elements[3] + elements[5]));
		GDIPlus.CheckStatus(status);
	}

	public void Scale(float scaleX, float scaleY)
	{
		Scale(scaleX, scaleY, MatrixOrder.Prepend);
	}

	public void Scale(float scaleX, float scaleY, MatrixOrder order)
	{
		Status status = GDIPlus.GdipScaleMatrix(nativeMatrix, scaleX, scaleY, order);
		GDIPlus.CheckStatus(status);
	}

	public void Shear(float shearX, float shearY)
	{
		Shear(shearX, shearY, MatrixOrder.Prepend);
	}

	public void Shear(float shearX, float shearY, MatrixOrder order)
	{
		Status status = GDIPlus.GdipShearMatrix(nativeMatrix, shearX, shearY, order);
		GDIPlus.CheckStatus(status);
	}

	public void TransformPoints(Point[] pts)
	{
		if (pts == null)
		{
			throw new ArgumentNullException("pts");
		}
		Status status = GDIPlus.GdipTransformMatrixPointsI(nativeMatrix, pts, pts.Length);
		GDIPlus.CheckStatus(status);
	}

	public void TransformPoints(PointF[] pts)
	{
		if (pts == null)
		{
			throw new ArgumentNullException("pts");
		}
		Status status = GDIPlus.GdipTransformMatrixPoints(nativeMatrix, pts, pts.Length);
		GDIPlus.CheckStatus(status);
	}

	public void TransformVectors(Point[] pts)
	{
		if (pts == null)
		{
			throw new ArgumentNullException("pts");
		}
		Status status = GDIPlus.GdipVectorTransformMatrixPointsI(nativeMatrix, pts, pts.Length);
		GDIPlus.CheckStatus(status);
	}

	public void TransformVectors(PointF[] pts)
	{
		if (pts == null)
		{
			throw new ArgumentNullException("pts");
		}
		Status status = GDIPlus.GdipVectorTransformMatrixPoints(nativeMatrix, pts, pts.Length);
		GDIPlus.CheckStatus(status);
	}

	public void Translate(float offsetX, float offsetY)
	{
		Translate(offsetX, offsetY, MatrixOrder.Prepend);
	}

	public void Translate(float offsetX, float offsetY, MatrixOrder order)
	{
		Status status = GDIPlus.GdipTranslateMatrix(nativeMatrix, offsetX, offsetY, order);
		GDIPlus.CheckStatus(status);
	}

	public void VectorTransformPoints(Point[] pts)
	{
		TransformVectors(pts);
	}
}

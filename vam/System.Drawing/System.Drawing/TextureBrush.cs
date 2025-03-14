using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace System.Drawing;

public sealed class TextureBrush : Brush
{
	public Image Image
	{
		get
		{
			if (nativeObject == IntPtr.Zero)
			{
				throw new ArgumentException("Object was disposed");
			}
			IntPtr image;
			Status status = GDIPlus.GdipGetTextureImage(nativeObject, out image);
			GDIPlus.CheckStatus(status);
			return new Bitmap(image);
		}
	}

	public Matrix Transform
	{
		get
		{
			Matrix matrix = new Matrix();
			Status status = GDIPlus.GdipGetTextureTransform(nativeObject, matrix.nativeMatrix);
			GDIPlus.CheckStatus(status);
			return matrix;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("Transform");
			}
			Status status = GDIPlus.GdipSetTextureTransform(nativeObject, value.nativeMatrix);
			GDIPlus.CheckStatus(status);
		}
	}

	public WrapMode WrapMode
	{
		get
		{
			WrapMode wrapMode;
			Status status = GDIPlus.GdipGetTextureWrapMode(nativeObject, out wrapMode);
			GDIPlus.CheckStatus(status);
			return wrapMode;
		}
		set
		{
			if (value < WrapMode.Tile || value > WrapMode.Clamp)
			{
				throw new InvalidEnumArgumentException("WrapMode");
			}
			Status status = GDIPlus.GdipSetTextureWrapMode(nativeObject, value);
			GDIPlus.CheckStatus(status);
		}
	}

	internal TextureBrush(IntPtr ptr)
		: base(ptr)
	{
	}

	public TextureBrush(Image bitmap)
		: this(bitmap, WrapMode.Tile)
	{
	}

	public TextureBrush(Image image, Rectangle dstRect)
		: this(image, WrapMode.Tile, dstRect)
	{
	}

	public TextureBrush(Image image, RectangleF dstRect)
		: this(image, WrapMode.Tile, dstRect)
	{
	}

	public TextureBrush(Image image, WrapMode wrapMode)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (wrapMode < WrapMode.Tile || wrapMode > WrapMode.Clamp)
		{
			throw new InvalidEnumArgumentException("WrapMode");
		}
		Status status = GDIPlus.GdipCreateTexture(image.nativeObject, wrapMode, out nativeObject);
		GDIPlus.CheckStatus(status);
	}

	[System.MonoLimitation("ImageAttributes are ignored when using libgdiplus")]
	public TextureBrush(Image image, Rectangle dstRect, ImageAttributes imageAttr)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		IntPtr imageAttributes = imageAttr?.NativeObject ?? IntPtr.Zero;
		Status status = GDIPlus.GdipCreateTextureIAI(image.nativeObject, imageAttributes, dstRect.X, dstRect.Y, dstRect.Width, dstRect.Height, out nativeObject);
		GDIPlus.CheckStatus(status);
	}

	[System.MonoLimitation("ImageAttributes are ignored when using libgdiplus")]
	public TextureBrush(Image image, RectangleF dstRect, ImageAttributes imageAttr)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		IntPtr imageAttributes = imageAttr?.NativeObject ?? IntPtr.Zero;
		Status status = GDIPlus.GdipCreateTextureIA(image.nativeObject, imageAttributes, dstRect.X, dstRect.Y, dstRect.Width, dstRect.Height, out nativeObject);
		GDIPlus.CheckStatus(status);
	}

	public TextureBrush(Image image, WrapMode wrapMode, Rectangle dstRect)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (wrapMode < WrapMode.Tile || wrapMode > WrapMode.Clamp)
		{
			throw new InvalidEnumArgumentException("WrapMode");
		}
		Status status = GDIPlus.GdipCreateTexture2I(image.nativeObject, wrapMode, dstRect.X, dstRect.Y, dstRect.Width, dstRect.Height, out nativeObject);
		GDIPlus.CheckStatus(status);
	}

	public TextureBrush(Image image, WrapMode wrapMode, RectangleF dstRect)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (wrapMode < WrapMode.Tile || wrapMode > WrapMode.Clamp)
		{
			throw new InvalidEnumArgumentException("WrapMode");
		}
		Status status = GDIPlus.GdipCreateTexture2(image.nativeObject, wrapMode, dstRect.X, dstRect.Y, dstRect.Width, dstRect.Height, out nativeObject);
		GDIPlus.CheckStatus(status);
	}

	public override object Clone()
	{
		IntPtr clonedBrush;
		Status status = GDIPlus.GdipCloneBrush(nativeObject, out clonedBrush);
		GDIPlus.CheckStatus(status);
		return new TextureBrush(clonedBrush);
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
		Status status = GDIPlus.GdipMultiplyTextureTransform(nativeObject, matrix.nativeMatrix, order);
		GDIPlus.CheckStatus(status);
	}

	public void ResetTransform()
	{
		Status status = GDIPlus.GdipResetTextureTransform(nativeObject);
		GDIPlus.CheckStatus(status);
	}

	public void RotateTransform(float angle)
	{
		RotateTransform(angle, MatrixOrder.Prepend);
	}

	public void RotateTransform(float angle, MatrixOrder order)
	{
		Status status = GDIPlus.GdipRotateTextureTransform(nativeObject, angle, order);
		GDIPlus.CheckStatus(status);
	}

	public void ScaleTransform(float sx, float sy)
	{
		ScaleTransform(sx, sy, MatrixOrder.Prepend);
	}

	public void ScaleTransform(float sx, float sy, MatrixOrder order)
	{
		Status status = GDIPlus.GdipScaleTextureTransform(nativeObject, sx, sy, order);
		GDIPlus.CheckStatus(status);
	}

	public void TranslateTransform(float dx, float dy)
	{
		TranslateTransform(dx, dy, MatrixOrder.Prepend);
	}

	public void TranslateTransform(float dx, float dy, MatrixOrder order)
	{
		Status status = GDIPlus.GdipTranslateTextureTransform(nativeObject, dx, dy, order);
		GDIPlus.CheckStatus(status);
	}
}

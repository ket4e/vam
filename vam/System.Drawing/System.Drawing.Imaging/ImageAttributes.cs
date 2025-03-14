using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace System.Drawing.Imaging;

[StructLayout(LayoutKind.Sequential)]
public sealed class ImageAttributes : IDisposable, ICloneable
{
	private IntPtr nativeImageAttr;

	internal IntPtr NativeObject => nativeImageAttr;

	internal ImageAttributes(IntPtr native)
	{
		nativeImageAttr = native;
	}

	public ImageAttributes()
	{
		Status status = GDIPlus.GdipCreateImageAttributes(out nativeImageAttr);
		GDIPlus.CheckStatus(status);
	}

	public void ClearBrushRemapTable()
	{
		ClearRemapTable(ColorAdjustType.Brush);
	}

	public void ClearColorKey()
	{
		ClearColorKey(ColorAdjustType.Default);
	}

	public void ClearColorKey(ColorAdjustType type)
	{
		Status status = GDIPlus.GdipSetImageAttributesColorKeys(nativeImageAttr, type, enableFlag: false, 0, 0);
		GDIPlus.CheckStatus(status);
	}

	public void ClearColorMatrix()
	{
		ClearColorMatrix(ColorAdjustType.Default);
	}

	public void ClearColorMatrix(ColorAdjustType type)
	{
		Status status = GDIPlus.GdipSetImageAttributesColorMatrix(nativeImageAttr, type, enableFlag: false, IntPtr.Zero, IntPtr.Zero, ColorMatrixFlag.Default);
		GDIPlus.CheckStatus(status);
	}

	public void ClearGamma()
	{
		ClearGamma(ColorAdjustType.Default);
	}

	public void ClearGamma(ColorAdjustType type)
	{
		Status status = GDIPlus.GdipSetImageAttributesGamma(nativeImageAttr, type, enableFlag: false, 0f);
		GDIPlus.CheckStatus(status);
	}

	public void ClearNoOp()
	{
		ClearNoOp(ColorAdjustType.Default);
	}

	public void ClearNoOp(ColorAdjustType type)
	{
		Status status = GDIPlus.GdipSetImageAttributesNoOp(nativeImageAttr, type, enableFlag: false);
		GDIPlus.CheckStatus(status);
	}

	public void ClearOutputChannel()
	{
		ClearOutputChannel(ColorAdjustType.Default);
	}

	public void ClearOutputChannel(ColorAdjustType type)
	{
		Status status = GDIPlus.GdipSetImageAttributesOutputChannel(nativeImageAttr, type, enableFlag: false, ColorChannelFlag.ColorChannelLast);
		GDIPlus.CheckStatus(status);
	}

	public void ClearOutputChannelColorProfile()
	{
		ClearOutputChannelColorProfile(ColorAdjustType.Default);
	}

	public void ClearOutputChannelColorProfile(ColorAdjustType type)
	{
		Status status = GDIPlus.GdipSetImageAttributesOutputChannelColorProfile(nativeImageAttr, type, enableFlag: false, null);
		GDIPlus.CheckStatus(status);
	}

	public void ClearRemapTable()
	{
		ClearRemapTable(ColorAdjustType.Default);
	}

	public void ClearRemapTable(ColorAdjustType type)
	{
		Status status = GDIPlus.GdipSetImageAttributesRemapTable(nativeImageAttr, type, enableFlag: false, 0u, IntPtr.Zero);
		GDIPlus.CheckStatus(status);
	}

	public void ClearThreshold()
	{
		ClearThreshold(ColorAdjustType.Default);
	}

	public void ClearThreshold(ColorAdjustType type)
	{
		Status status = GDIPlus.GdipSetImageAttributesThreshold(nativeImageAttr, type, enableFlag: false, 0f);
		GDIPlus.CheckStatus(status);
	}

	public void SetColorKey(Color colorLow, Color colorHigh)
	{
		SetColorKey(colorLow, colorHigh, ColorAdjustType.Default);
	}

	public void SetColorMatrix(ColorMatrix colorMatrix)
	{
		SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Default);
	}

	public void SetColorMatrix(ColorMatrix colorMatrix, ColorMatrixFlag colorMatrixFlag)
	{
		SetColorMatrix(colorMatrix, colorMatrixFlag, ColorAdjustType.Default);
	}

	public void SetColorMatrix(ColorMatrix colorMatrix, ColorMatrixFlag colorMatrixFlag, ColorAdjustType colorAdjustType)
	{
		IntPtr intPtr = ColorMatrix.Alloc(colorMatrix);
		try
		{
			Status status = GDIPlus.GdipSetImageAttributesColorMatrix(nativeImageAttr, colorAdjustType, enableFlag: true, intPtr, IntPtr.Zero, colorMatrixFlag);
			GDIPlus.CheckStatus(status);
		}
		finally
		{
			ColorMatrix.Free(intPtr);
		}
	}

	public void Dispose()
	{
		if (nativeImageAttr != IntPtr.Zero)
		{
			Status status = GDIPlus.GdipDisposeImageAttributes(nativeImageAttr);
			nativeImageAttr = IntPtr.Zero;
			GDIPlus.CheckStatus(status);
		}
		GC.SuppressFinalize(this);
	}

	~ImageAttributes()
	{
		Dispose();
	}

	public object Clone()
	{
		IntPtr cloneImageattr;
		Status status = GDIPlus.GdipCloneImageAttributes(nativeImageAttr, out cloneImageattr);
		GDIPlus.CheckStatus(status);
		return new ImageAttributes(cloneImageattr);
	}

	[System.MonoTODO("Not supported by libgdiplus")]
	public void GetAdjustedPalette(ColorPalette palette, ColorAdjustType type)
	{
		IntPtr gDIPalette = palette.getGDIPalette();
		try
		{
			Status status = GDIPlus.GdipGetImageAttributesAdjustedPalette(nativeImageAttr, gDIPalette, type);
			GDIPlus.CheckStatus(status);
			palette.setFromGDIPalette(gDIPalette);
		}
		finally
		{
			if (gDIPalette != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(gDIPalette);
			}
		}
	}

	public void SetBrushRemapTable(ColorMap[] map)
	{
		GdiColorMap gdiColorMap = default(GdiColorMap);
		int num = Marshal.SizeOf(gdiColorMap);
		int cb = num * map.Length;
		IntPtr ptr;
		IntPtr intPtr = (ptr = Marshal.AllocHGlobal(cb));
		try
		{
			for (int i = 0; i < map.Length; i++)
			{
				gdiColorMap.from = map[i].OldColor.ToArgb();
				gdiColorMap.to = map[i].NewColor.ToArgb();
				Marshal.StructureToPtr(gdiColorMap, ptr, fDeleteOld: false);
				ptr = (IntPtr)(ptr.ToInt64() + num);
			}
			Status status = GDIPlus.GdipSetImageAttributesRemapTable(nativeImageAttr, ColorAdjustType.Brush, enableFlag: true, (uint)map.Length, intPtr);
			GDIPlus.CheckStatus(status);
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	public void SetColorKey(Color colorLow, Color colorHigh, ColorAdjustType type)
	{
		Status status = GDIPlus.GdipSetImageAttributesColorKeys(nativeImageAttr, type, enableFlag: true, colorLow.ToArgb(), colorHigh.ToArgb());
		GDIPlus.CheckStatus(status);
	}

	public void SetColorMatrices(ColorMatrix newColorMatrix, ColorMatrix grayMatrix)
	{
		SetColorMatrices(newColorMatrix, grayMatrix, ColorMatrixFlag.Default, ColorAdjustType.Default);
	}

	public void SetColorMatrices(ColorMatrix newColorMatrix, ColorMatrix grayMatrix, ColorMatrixFlag flags)
	{
		SetColorMatrices(newColorMatrix, grayMatrix, flags, ColorAdjustType.Default);
	}

	public void SetColorMatrices(ColorMatrix newColorMatrix, ColorMatrix grayMatrix, ColorMatrixFlag mode, ColorAdjustType type)
	{
		IntPtr intPtr = ColorMatrix.Alloc(newColorMatrix);
		Status status;
		try
		{
			if (grayMatrix == null)
			{
				status = GDIPlus.GdipSetImageAttributesColorMatrix(nativeImageAttr, type, enableFlag: true, intPtr, IntPtr.Zero, mode);
			}
			else
			{
				IntPtr intPtr2 = ColorMatrix.Alloc(grayMatrix);
				try
				{
					status = GDIPlus.GdipSetImageAttributesColorMatrix(nativeImageAttr, type, enableFlag: true, intPtr, intPtr2, mode);
				}
				finally
				{
					ColorMatrix.Free(intPtr2);
				}
			}
		}
		finally
		{
			ColorMatrix.Free(intPtr);
		}
		GDIPlus.CheckStatus(status);
	}

	public void SetGamma(float gamma)
	{
		SetGamma(gamma, ColorAdjustType.Default);
	}

	public void SetGamma(float gamma, ColorAdjustType coloradjust)
	{
		Status status = GDIPlus.GdipSetImageAttributesGamma(nativeImageAttr, coloradjust, enableFlag: true, gamma);
		GDIPlus.CheckStatus(status);
	}

	public void SetNoOp()
	{
		SetNoOp(ColorAdjustType.Default);
	}

	public void SetNoOp(ColorAdjustType type)
	{
		Status status = GDIPlus.GdipSetImageAttributesNoOp(nativeImageAttr, type, enableFlag: true);
		GDIPlus.CheckStatus(status);
	}

	[System.MonoTODO("Not supported by libgdiplus")]
	public void SetOutputChannel(ColorChannelFlag flags)
	{
		SetOutputChannel(flags, ColorAdjustType.Default);
	}

	[System.MonoTODO("Not supported by libgdiplus")]
	public void SetOutputChannel(ColorChannelFlag flags, ColorAdjustType type)
	{
		Status status = GDIPlus.GdipSetImageAttributesOutputChannel(nativeImageAttr, type, enableFlag: true, flags);
		GDIPlus.CheckStatus(status);
	}

	[System.MonoTODO("Not supported by libgdiplus")]
	public void SetOutputChannelColorProfile(string colorProfileFilename)
	{
		SetOutputChannelColorProfile(colorProfileFilename, ColorAdjustType.Default);
	}

	[System.MonoTODO("Not supported by libgdiplus")]
	public void SetOutputChannelColorProfile(string colorProfileFilename, ColorAdjustType type)
	{
		Status status = GDIPlus.GdipSetImageAttributesOutputChannelColorProfile(nativeImageAttr, type, enableFlag: true, colorProfileFilename);
		GDIPlus.CheckStatus(status);
	}

	public void SetRemapTable(ColorMap[] map)
	{
		SetRemapTable(map, ColorAdjustType.Default);
	}

	public void SetRemapTable(ColorMap[] map, ColorAdjustType type)
	{
		GdiColorMap gdiColorMap = default(GdiColorMap);
		int num = Marshal.SizeOf(gdiColorMap);
		int cb = num * map.Length;
		IntPtr ptr;
		IntPtr intPtr = (ptr = Marshal.AllocHGlobal(cb));
		try
		{
			for (int i = 0; i < map.Length; i++)
			{
				gdiColorMap.from = map[i].OldColor.ToArgb();
				gdiColorMap.to = map[i].NewColor.ToArgb();
				Marshal.StructureToPtr(gdiColorMap, ptr, fDeleteOld: false);
				ptr = (IntPtr)(ptr.ToInt64() + num);
			}
			Status status = GDIPlus.GdipSetImageAttributesRemapTable(nativeImageAttr, type, enableFlag: true, (uint)map.Length, intPtr);
			GDIPlus.CheckStatus(status);
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}

	[System.MonoTODO("Not supported by libgdiplus")]
	public void SetThreshold(float threshold)
	{
		SetThreshold(threshold, ColorAdjustType.Default);
	}

	[System.MonoTODO("Not supported by libgdiplus")]
	public void SetThreshold(float threshold, ColorAdjustType type)
	{
		Status status = GDIPlus.GdipSetImageAttributesThreshold(nativeImageAttr, type, enableFlag: true, 0f);
		GDIPlus.CheckStatus(status);
	}

	public void SetWrapMode(WrapMode mode)
	{
		SetWrapMode(mode, Color.Black);
	}

	public void SetWrapMode(WrapMode mode, Color color)
	{
		SetWrapMode(mode, color, clamp: false);
	}

	public void SetWrapMode(WrapMode mode, Color color, bool clamp)
	{
		Status status = GDIPlus.GdipSetImageAttributesWrapMode(nativeImageAttr, mode, color.ToArgb(), clamp);
		GDIPlus.CheckStatus(status);
	}
}

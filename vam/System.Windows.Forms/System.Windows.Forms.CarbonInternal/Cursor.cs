using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.CarbonInternal;

internal class Cursor
{
	internal static CarbonCursor defcur = new CarbonCursor(StdCursor.Default);

	internal static Bitmap DefineStdCursorBitmap(StdCursor id)
	{
		return new Bitmap(16, 16);
	}

	internal static IntPtr DefineCursor(Bitmap bitmap, Bitmap mask, Color cursor_pixel, Color mask_pixel, int xHotSpot, int yHotSpot)
	{
		CarbonCursor carbonCursor = new CarbonCursor(bitmap, mask, cursor_pixel, mask_pixel, xHotSpot, yHotSpot);
		return (IntPtr)GCHandle.Alloc(carbonCursor);
	}

	internal static IntPtr DefineStdCursor(StdCursor id)
	{
		CarbonCursor carbonCursor = new CarbonCursor(id);
		return (IntPtr)GCHandle.Alloc(carbonCursor);
	}

	internal static void SetCursor(IntPtr cursor)
	{
		if (cursor == IntPtr.Zero)
		{
			defcur.SetCursor();
		}
		else
		{
			((CarbonCursor)((GCHandle)cursor).Target).SetCursor();
		}
	}
}

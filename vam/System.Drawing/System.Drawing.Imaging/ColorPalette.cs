using System.Runtime.InteropServices;

namespace System.Drawing.Imaging;

public sealed class ColorPalette
{
	private int flags;

	private Color[] entries;

	public Color[] Entries => entries;

	public int Flags => flags;

	internal ColorPalette()
	{
		entries = new Color[0];
	}

	internal ColorPalette(int flags, Color[] colors)
	{
		this.flags = flags;
		entries = colors;
	}

	internal IntPtr getGDIPalette()
	{
		GdiColorPalette gdiColorPalette = default(GdiColorPalette);
		Color[] array = Entries;
		int num = 0;
		int cb = Marshal.SizeOf(gdiColorPalette) + Marshal.SizeOf(num) * array.Length;
		IntPtr intPtr = Marshal.AllocHGlobal(cb);
		gdiColorPalette.Flags = Flags;
		gdiColorPalette.Count = array.Length;
		int[] array2 = new int[gdiColorPalette.Count];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = array[i].ToArgb();
		}
		Marshal.StructureToPtr(gdiColorPalette, intPtr, fDeleteOld: false);
		Marshal.Copy(array2, 0, (IntPtr)(intPtr.ToInt64() + Marshal.SizeOf(gdiColorPalette)), array2.Length);
		return intPtr;
	}

	internal void setFromGDIPalette(IntPtr palette)
	{
		IntPtr ptr = palette;
		flags = Marshal.ReadInt32(ptr);
		ptr = (IntPtr)(ptr.ToInt64() + 4);
		int num = Marshal.ReadInt32(ptr);
		ptr = (IntPtr)(ptr.ToInt64() + 4);
		entries = new Color[num];
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			int argb = Marshal.ReadInt32(ptr, num2);
			ref Color reference = ref entries[i];
			reference = Color.FromArgb(argb);
			num2 += 4;
		}
	}
}

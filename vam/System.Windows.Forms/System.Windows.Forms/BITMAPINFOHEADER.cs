namespace System.Windows.Forms;

internal struct BITMAPINFOHEADER
{
	internal uint biSize;

	internal int biWidth;

	internal int biHeight;

	internal ushort biPlanes;

	internal ushort biBitCount;

	internal uint biCompression;

	internal uint biSizeImage;

	internal int biXPelsPerMeter;

	internal int biYPelsPerMeter;

	internal uint biClrUsed;

	internal uint biClrImportant;
}

using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct MINMAXINFO
{
	internal POINT ptReserved;

	internal POINT ptMaxSize;

	internal POINT ptMaxPosition;

	internal POINT ptMinTrackSize;

	internal POINT ptMaxTrackSize;
}

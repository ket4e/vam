namespace System.Windows.Forms;

internal struct HELPINFO
{
	internal uint cbSize;

	internal int iContextType;

	internal int iCtrlId;

	internal IntPtr hItemHandle;

	internal uint dwContextId;

	internal POINT MousePos;
}

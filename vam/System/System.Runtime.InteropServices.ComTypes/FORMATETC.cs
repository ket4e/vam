namespace System.Runtime.InteropServices.ComTypes;

public struct FORMATETC
{
	[MarshalAs(UnmanagedType.U2)]
	public short cfFormat;

	[MarshalAs(UnmanagedType.U4)]
	public DVASPECT dwAspect;

	public int lindex;

	public IntPtr ptd;

	[MarshalAs(UnmanagedType.U4)]
	public TYMED tymed;
}

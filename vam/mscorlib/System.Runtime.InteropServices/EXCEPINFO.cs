namespace System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
[Obsolete]
public struct EXCEPINFO
{
	public short wCode;

	public short wReserved;

	[MarshalAs(UnmanagedType.BStr)]
	public string bstrSource;

	[MarshalAs(UnmanagedType.BStr)]
	public string bstrDescription;

	[MarshalAs(UnmanagedType.BStr)]
	public string bstrHelpFile;

	public int dwHelpContext;

	public IntPtr pvReserved;

	public IntPtr pfnDeferredFillIn;
}

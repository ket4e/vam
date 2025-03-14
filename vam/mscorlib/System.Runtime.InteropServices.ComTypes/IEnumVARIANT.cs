namespace System.Runtime.InteropServices.ComTypes;

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("00020404-0000-0000-c000-000000000046")]
public interface IEnumVARIANT
{
	[PreserveSig]
	int Next(int celt, [Out][MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 0)] object[] rgVar, IntPtr pceltFetched);

	[PreserveSig]
	int Skip(int celt);

	[PreserveSig]
	int Reset();

	IEnumVARIANT Clone();
}

using System.Reflection;

namespace System.Runtime.InteropServices;

[CLSCompliant(false)]
[ComVisible(true)]
[Guid("D002E9BA-D9E3-3749-B1D3-D565A08B13E7")]
[TypeLibImportClass(typeof(Module))]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface _Module
{
	void GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId);

	void GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo);

	void GetTypeInfoCount(out uint pcTInfo);

	void Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr);
}

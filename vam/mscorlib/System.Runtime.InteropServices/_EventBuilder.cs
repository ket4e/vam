using System.Reflection.Emit;

namespace System.Runtime.InteropServices;

[CLSCompliant(false)]
[ComVisible(true)]
[TypeLibImportClass(typeof(EventBuilder))]
[Guid("AADABA99-895D-3D65-9760-B1F12621FAE8")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface _EventBuilder
{
	void GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId);

	void GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo);

	void GetTypeInfoCount(out uint pcTInfo);

	void Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr);
}

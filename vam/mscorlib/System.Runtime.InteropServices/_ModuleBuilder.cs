using System.Reflection.Emit;

namespace System.Runtime.InteropServices;

[CLSCompliant(false)]
[ComVisible(true)]
[TypeLibImportClass(typeof(ModuleBuilder))]
[Guid("D05FFA9A-04AF-3519-8EE1-8D93AD73430B")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface _ModuleBuilder
{
	void GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId);

	void GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo);

	void GetTypeInfoCount(out uint pcTInfo);

	void Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr);
}

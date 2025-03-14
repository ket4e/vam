using System.Reflection;
using System.Reflection.Emit;

namespace System.Runtime.InteropServices;

[ComVisible(true)]
[ClassInterface(ClassInterfaceType.None)]
[Guid("f1c3bf79-c3e4-11d3-88e7-00902754c43a")]
public sealed class TypeLibConverter : ITypeLibConverter
{
	[MonoTODO("implement")]
	[return: MarshalAs(UnmanagedType.Interface)]
	public object ConvertAssemblyToTypeLib(Assembly assembly, string strTypeLibName, TypeLibExporterFlags flags, ITypeLibExporterNotifySink notifySink)
	{
		throw new NotImplementedException();
	}

	[MonoTODO("implement")]
	public AssemblyBuilder ConvertTypeLibToAssembly([MarshalAs(UnmanagedType.Interface)] object typeLib, string asmFileName, int flags, ITypeLibImporterNotifySink notifySink, byte[] publicKey, StrongNameKeyPair keyPair, bool unsafeInterfaces)
	{
		throw new NotImplementedException();
	}

	[MonoTODO("implement")]
	public AssemblyBuilder ConvertTypeLibToAssembly([MarshalAs(UnmanagedType.Interface)] object typeLib, string asmFileName, TypeLibImporterFlags flags, ITypeLibImporterNotifySink notifySink, byte[] publicKey, StrongNameKeyPair keyPair, string asmNamespace, Version asmVersion)
	{
		throw new NotImplementedException();
	}

	[MonoTODO("implement")]
	public bool GetPrimaryInteropAssembly(Guid g, int major, int minor, int lcid, out string asmName, out string asmCodeBase)
	{
		throw new NotImplementedException();
	}
}

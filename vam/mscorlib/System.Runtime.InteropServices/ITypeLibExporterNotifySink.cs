using System.Reflection;

namespace System.Runtime.InteropServices;

[Guid("f1c3bf77-c3e4-11d3-88e7-00902754c43a")]
[ComVisible(true)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ITypeLibExporterNotifySink
{
	void ReportEvent(ExporterEventKind eventKind, int eventCode, string eventMsg);

	[return: MarshalAs(UnmanagedType.Interface)]
	object ResolveRef(Assembly assembly);
}

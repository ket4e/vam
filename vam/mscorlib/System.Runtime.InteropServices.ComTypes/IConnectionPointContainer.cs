namespace System.Runtime.InteropServices.ComTypes;

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("b196b284-bab4-101a-b69c-00aa00341d07")]
public interface IConnectionPointContainer
{
	void EnumConnectionPoints(out IEnumConnectionPoints ppEnum);

	void FindConnectionPoint([In] ref Guid riid, out IConnectionPoint ppCP);
}

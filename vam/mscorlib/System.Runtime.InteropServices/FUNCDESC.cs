namespace System.Runtime.InteropServices;

[Obsolete]
public struct FUNCDESC
{
	public int memid;

	public IntPtr lprgscode;

	public IntPtr lprgelemdescParam;

	public FUNCKIND funckind;

	public INVOKEKIND invkind;

	public CALLCONV callconv;

	public short cParams;

	public short cParamsOpt;

	public short oVft;

	public short cScodes;

	public ELEMDESC elemdescFunc;

	public short wFuncFlags;
}

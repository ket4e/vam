namespace System.Runtime.InteropServices;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class DllImportAttribute : Attribute
{
	public CallingConvention CallingConvention;

	public CharSet CharSet;

	private string Dll;

	public string EntryPoint;

	public bool ExactSpelling;

	public bool PreserveSig;

	public bool SetLastError;

	public bool BestFitMapping;

	public bool ThrowOnUnmappableChar;

	public string Value => Dll;

	public DllImportAttribute(string dllName)
	{
		Dll = dllName;
	}
}

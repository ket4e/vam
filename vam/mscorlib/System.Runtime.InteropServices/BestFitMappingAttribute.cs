namespace System.Runtime.InteropServices;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false)]
public sealed class BestFitMappingAttribute : Attribute
{
	private bool bfm;

	public bool ThrowOnUnmappableChar;

	public bool BestFitMapping => bfm;

	public BestFitMappingAttribute(bool BestFitMapping)
	{
		bfm = BestFitMapping;
	}
}

namespace System.Runtime.Serialization;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module, Inherited = false, AllowMultiple = true)]
public sealed class ContractNamespaceAttribute : Attribute
{
	private string clr_ns;

	private string contract_ns;

	public string ClrNamespace
	{
		get
		{
			return clr_ns;
		}
		set
		{
			clr_ns = value;
		}
	}

	public string ContractNamespace => contract_ns;

	public ContractNamespaceAttribute(string ns)
	{
		contract_ns = ns;
	}
}

using System.ComponentModel;

namespace System.Data.Odbc;

[AttributeUsage(AttributeTargets.All)]
internal sealed class OdbcDescriptionAttribute : DescriptionAttribute
{
	private string description;

	public override string Description => description;

	public OdbcDescriptionAttribute(string description)
		: base(description)
	{
		this.description = description;
	}
}

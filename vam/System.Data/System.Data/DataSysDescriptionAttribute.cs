using System.ComponentModel;

namespace System.Data;

[Obsolete("DataSysDescriptionAttribute has been deprecated")]
[AttributeUsage(AttributeTargets.All)]
public class DataSysDescriptionAttribute : DescriptionAttribute
{
	private string description;

	public override string Description => description;

	[Obsolete("DataSysDescriptionAttribute has been deprecated")]
	public DataSysDescriptionAttribute(string description)
		: base(description)
	{
		this.description = description;
	}
}

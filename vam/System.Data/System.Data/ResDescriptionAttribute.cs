using System.ComponentModel;

namespace System.Data;

[AttributeUsage(AttributeTargets.All)]
internal sealed class ResDescriptionAttribute : DescriptionAttribute
{
	private string description;

	public override string Description => description;

	public ResDescriptionAttribute(string description)
		: base(description)
	{
		this.description = description;
	}
}

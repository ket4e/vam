using System.ComponentModel;

namespace System.Data.Odbc;

[AttributeUsage(AttributeTargets.All)]
internal sealed class OdbcCategoryAttribute : CategoryAttribute
{
	private string category;

	public new string Category => category;

	public OdbcCategoryAttribute(string category)
	{
		this.category = category;
	}

	[System.MonoTODO]
	protected override string GetLocalizedString(string value)
	{
		throw new NotImplementedException();
	}
}

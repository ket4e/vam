using System.ComponentModel;
using System.Windows.Forms;

namespace System;

[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
internal sealed class MWFCategoryAttribute : CategoryAttribute
{
	public MWFCategoryAttribute()
	{
	}

	public MWFCategoryAttribute(string category)
		: base(category)
	{
	}

	protected override string GetLocalizedString(string value)
	{
		return Locale.GetText(value);
	}
}

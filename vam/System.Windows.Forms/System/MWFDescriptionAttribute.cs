using System.ComponentModel;
using System.Windows.Forms;

namespace System;

[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
internal sealed class MWFDescriptionAttribute : DescriptionAttribute
{
	public override string Description => Locale.GetText(base.Description);

	public MWFDescriptionAttribute()
	{
	}

	public MWFDescriptionAttribute(string category)
		: base(category)
	{
	}
}

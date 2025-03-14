namespace System.Configuration;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public sealed class SpecialSettingAttribute : Attribute
{
	private SpecialSetting setting;

	public SpecialSetting SpecialSetting => setting;

	public SpecialSettingAttribute(SpecialSetting setting)
	{
		this.setting = setting;
	}
}

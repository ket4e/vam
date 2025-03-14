namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SettingsBindableAttribute : Attribute
{
	public static readonly SettingsBindableAttribute Yes = new SettingsBindableAttribute(bindable: true);

	public static readonly SettingsBindableAttribute No = new SettingsBindableAttribute(bindable: false);

	private bool bindable;

	public bool Bindable => bindable;

	public SettingsBindableAttribute(bool bindable)
	{
		this.bindable = bindable;
	}

	public override int GetHashCode()
	{
		return bindable ? 1 : (-1);
	}

	public override bool Equals(object obj)
	{
		return obj is SettingsBindableAttribute settingsBindableAttribute && bindable == settingsBindableAttribute.bindable;
	}
}

namespace System.Diagnostics;

[SwitchLevel(typeof(bool))]
public class BooleanSwitch : Switch
{
	public bool Enabled
	{
		get
		{
			return base.SwitchSetting != 0;
		}
		set
		{
			base.SwitchSetting = Convert.ToInt32(value);
		}
	}

	public BooleanSwitch(string displayName, string description)
		: base(displayName, description)
	{
	}

	public BooleanSwitch(string displayName, string description, string defaultSwitchValue)
		: base(displayName, description, defaultSwitchValue)
	{
	}

	protected override void OnValueChanged()
	{
		if (int.TryParse(base.Value, out var result))
		{
			Enabled = result != 0;
		}
		else
		{
			Enabled = Convert.ToBoolean(base.Value);
		}
	}
}

namespace System.Diagnostics;

public class SourceSwitch : Switch
{
	private const string description = "Source switch.";

	public SourceLevels Level
	{
		get
		{
			return (SourceLevels)base.SwitchSetting;
		}
		set
		{
			base.SwitchSetting = (int)value;
		}
	}

	public SourceSwitch(string displayName)
		: this(displayName, null)
	{
	}

	public SourceSwitch(string displayName, string defaultSwitchValue)
		: base(displayName, "Source switch.", defaultSwitchValue)
	{
	}

	public bool ShouldTrace(TraceEventType eventType)
	{
		return eventType switch
		{
			TraceEventType.Critical => (Level & SourceLevels.Critical) != 0, 
			TraceEventType.Error => (Level & SourceLevels.Error) != 0, 
			TraceEventType.Warning => (Level & SourceLevels.Warning) != 0, 
			TraceEventType.Information => (Level & SourceLevels.Information) != 0, 
			TraceEventType.Verbose => (Level & SourceLevels.Verbose) != 0, 
			_ => (Level & SourceLevels.ActivityTracing) != 0, 
		};
	}

	protected override void OnValueChanged()
	{
		base.SwitchSetting = (int)Enum.Parse(typeof(SourceLevels), base.Value, ignoreCase: true);
	}
}

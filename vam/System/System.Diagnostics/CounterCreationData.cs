using System.ComponentModel;

namespace System.Diagnostics;

[Serializable]
[TypeConverter("System.Diagnostics.Design.CounterCreationDataConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
public class CounterCreationData
{
	private string help = string.Empty;

	private string name;

	private PerformanceCounterType type;

	[MonitoringDescription("Description of this counter.")]
	[DefaultValue("")]
	public string CounterHelp
	{
		get
		{
			return help;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			help = value;
		}
	}

	[MonitoringDescription("Name of this counter.")]
	[DefaultValue("")]
	[TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public string CounterName
	{
		get
		{
			return name;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value == string.Empty)
			{
				throw new ArgumentException("value");
			}
			name = value;
		}
	}

	[MonitoringDescription("Type of this counter.")]
	[DefaultValue(typeof(PerformanceCounterType), "NumberOfItems32")]
	public PerformanceCounterType CounterType
	{
		get
		{
			return type;
		}
		set
		{
			if (!Enum.IsDefined(typeof(PerformanceCounterType), value))
			{
				throw new InvalidEnumArgumentException();
			}
			type = value;
		}
	}

	public CounterCreationData()
	{
	}

	public CounterCreationData(string counterName, string counterHelp, PerformanceCounterType counterType)
	{
		CounterName = counterName;
		CounterHelp = counterHelp;
		CounterType = counterType;
	}
}

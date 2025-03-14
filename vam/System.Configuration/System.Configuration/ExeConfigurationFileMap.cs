namespace System.Configuration;

public sealed class ExeConfigurationFileMap : ConfigurationFileMap
{
	private string exeConfigFilename;

	private string localUserConfigFilename;

	private string roamingUserConfigFilename;

	public string ExeConfigFilename
	{
		get
		{
			return exeConfigFilename;
		}
		set
		{
			exeConfigFilename = value;
		}
	}

	public string LocalUserConfigFilename
	{
		get
		{
			return localUserConfigFilename;
		}
		set
		{
			localUserConfigFilename = value;
		}
	}

	public string RoamingUserConfigFilename
	{
		get
		{
			return roamingUserConfigFilename;
		}
		set
		{
			roamingUserConfigFilename = value;
		}
	}

	public ExeConfigurationFileMap()
	{
		exeConfigFilename = string.Empty;
		localUserConfigFilename = string.Empty;
		roamingUserConfigFilename = string.Empty;
	}

	public override object Clone()
	{
		ExeConfigurationFileMap exeConfigurationFileMap = new ExeConfigurationFileMap();
		exeConfigurationFileMap.exeConfigFilename = exeConfigFilename;
		exeConfigurationFileMap.localUserConfigFilename = localUserConfigFilename;
		exeConfigurationFileMap.roamingUserConfigFilename = roamingUserConfigFilename;
		exeConfigurationFileMap.MachineConfigFilename = base.MachineConfigFilename;
		return exeConfigurationFileMap;
	}
}

using System.Configuration.Internal;

namespace System.Configuration;

internal class ExeConfigurationHost : InternalConfigurationHost
{
	private ExeConfigurationFileMap map;

	private ConfigurationUserLevel level;

	public override void Init(IInternalConfigRoot root, params object[] hostInitParams)
	{
		map = (ExeConfigurationFileMap)hostInitParams[0];
		level = (ConfigurationUserLevel)(int)hostInitParams[1];
	}

	public override string GetStreamName(string configPath)
	{
		return configPath switch
		{
			"exe" => map.ExeConfigFilename, 
			"local" => map.LocalUserConfigFilename, 
			"roaming" => map.RoamingUserConfigFilename, 
			"machine" => map.MachineConfigFilename, 
			_ => level switch
			{
				ConfigurationUserLevel.None => map.ExeConfigFilename, 
				ConfigurationUserLevel.PerUserRoaming => map.RoamingUserConfigFilename, 
				ConfigurationUserLevel.PerUserRoamingAndLocal => map.LocalUserConfigFilename, 
				_ => map.MachineConfigFilename, 
			}, 
		};
	}

	public override void InitForConfiguration(ref string locationSubPath, out string configPath, out string locationConfigPath, IInternalConfigRoot root, params object[] hostInitConfigurationParams)
	{
		map = (ExeConfigurationFileMap)hostInitConfigurationParams[0];
		if (hostInitConfigurationParams.Length > 1 && hostInitConfigurationParams[1] is ConfigurationUserLevel)
		{
			level = (ConfigurationUserLevel)(int)hostInitConfigurationParams[1];
		}
		if (locationSubPath == null)
		{
			switch (level)
			{
			case ConfigurationUserLevel.PerUserRoaming:
				if (map.RoamingUserConfigFilename == null)
				{
					throw new ArgumentException("RoamingUserConfigFilename must be set correctly");
				}
				locationSubPath = "roaming";
				break;
			case ConfigurationUserLevel.PerUserRoamingAndLocal:
				if (map.LocalUserConfigFilename == null)
				{
					throw new ArgumentException("LocalUserConfigFilename must be set correctly");
				}
				locationSubPath = "local";
				break;
			}
		}
		configPath = null;
		string text = null;
		locationConfigPath = null;
		if (locationSubPath == "exe" || (locationSubPath == null && map.ExeConfigFilename != null))
		{
			configPath = "exe";
			text = "local";
			locationConfigPath = map.ExeConfigFilename;
		}
		if (locationSubPath == "local" && map.LocalUserConfigFilename != null)
		{
			configPath = "local";
			text = "roaming";
			locationConfigPath = map.LocalUserConfigFilename;
		}
		if (locationSubPath == "roaming" && map.RoamingUserConfigFilename != null)
		{
			configPath = "roaming";
			text = "machine";
			locationConfigPath = map.RoamingUserConfigFilename;
		}
		if ((locationSubPath == "machine" || configPath == null) && map.MachineConfigFilename != null)
		{
			configPath = "machine";
			text = null;
		}
		locationSubPath = text;
	}
}

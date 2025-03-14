using System.Runtime.CompilerServices;

namespace System.Configuration;

internal class DefaultConfig : IConfigurationSystem
{
	private static readonly System.Configuration.DefaultConfig instance = new System.Configuration.DefaultConfig();

	private System.Configuration.ConfigurationData config;

	private DefaultConfig()
	{
	}

	public static System.Configuration.DefaultConfig GetInstance()
	{
		return instance;
	}

	[Obsolete("This method is obsolete.  Please use System.Configuration.ConfigurationManager.GetConfig")]
	public object GetConfig(string sectionName)
	{
		Init();
		return config.GetConfig(sectionName);
	}

	public void Init()
	{
		lock (this)
		{
			if (config != null)
			{
				return;
			}
			System.Configuration.ConfigurationData configurationData = new System.Configuration.ConfigurationData();
			if (!configurationData.LoadString(GetBundledMachineConfig()) && !configurationData.Load(GetMachineConfigPath()))
			{
				throw new ConfigurationException("Cannot find " + GetMachineConfigPath());
			}
			string appConfigPath = GetAppConfigPath();
			if (appConfigPath == null)
			{
				config = configurationData;
				return;
			}
			System.Configuration.ConfigurationData configurationData2 = new System.Configuration.ConfigurationData(configurationData);
			if (configurationData2.Load(appConfigPath))
			{
				config = configurationData2;
			}
			else
			{
				config = configurationData;
			}
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern string get_bundled_machine_config();

	internal static string GetBundledMachineConfig()
	{
		return get_bundled_machine_config();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern string get_machine_config_path();

	internal static string GetMachineConfigPath()
	{
		return get_machine_config_path();
	}

	private static string GetAppConfigPath()
	{
		AppDomainSetup setupInformation = AppDomain.CurrentDomain.SetupInformation;
		string configurationFile = setupInformation.ConfigurationFile;
		if (configurationFile == null || configurationFile.Length == 0)
		{
			return null;
		}
		return configurationFile;
	}
}

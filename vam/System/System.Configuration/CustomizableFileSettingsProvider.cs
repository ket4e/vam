using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace System.Configuration;

internal class CustomizableFileSettingsProvider : SettingsProvider, IApplicationSettingsProvider
{
	private static Type webConfigurationFileMapType;

	private static string userRoamingPath = string.Empty;

	private static string userLocalPath = string.Empty;

	private static string userRoamingPathPrevVersion = string.Empty;

	private static string userLocalPathPrevVersion = string.Empty;

	private static string userRoamingName = "user.config";

	private static string userLocalName = "user.config";

	private static string userRoamingBasePath = string.Empty;

	private static string userLocalBasePath = string.Empty;

	private static string CompanyName = string.Empty;

	private static string ProductName = string.Empty;

	private static string ForceVersion = string.Empty;

	private static string[] ProductVersion;

	private static bool isVersionMajor;

	private static bool isVersionMinor;

	private static bool isVersionBuild;

	private static bool isVersionRevision;

	private static bool isCompany = true;

	private static bool isProduct = true;

	private static bool isEvidence;

	private static bool userDefine;

	private static System.Configuration.UserConfigLocationOption userConfig = System.Configuration.UserConfigLocationOption.Company_Product;

	private string app_name = string.Empty;

	private ExeConfigurationFileMap exeMapCurrent;

	private ExeConfigurationFileMap exeMapPrev;

	private SettingsPropertyValueCollection values;

	internal static string UserRoamingFullPath => Path.Combine(userRoamingPath, userRoamingName);

	internal static string UserLocalFullPath => Path.Combine(userLocalPath, userLocalName);

	public static string PrevUserRoamingFullPath => Path.Combine(userRoamingPathPrevVersion, userRoamingName);

	public static string PrevUserLocalFullPath => Path.Combine(userLocalPathPrevVersion, userLocalName);

	public static string UserRoamingPath => userRoamingPath;

	public static string UserLocalPath => userLocalPath;

	public static string UserRoamingName => userRoamingName;

	public static string UserLocalName => userLocalName;

	public static System.Configuration.UserConfigLocationOption UserConfigSelector
	{
		get
		{
			return userConfig;
		}
		set
		{
			userConfig = value;
			if ((userConfig & System.Configuration.UserConfigLocationOption.Other) != 0)
			{
				isVersionMajor = false;
				isVersionMinor = false;
				isVersionBuild = false;
				isVersionRevision = false;
				isCompany = false;
			}
			else
			{
				isVersionRevision = (userConfig & (System.Configuration.UserConfigLocationOption)8u) != 0;
				isVersionBuild = isVersionRevision | ((userConfig & (System.Configuration.UserConfigLocationOption)4u) != 0);
				isVersionMinor = isVersionBuild | ((userConfig & (System.Configuration.UserConfigLocationOption)2u) != 0);
				isVersionMajor = IsVersionMinor | ((userConfig & (System.Configuration.UserConfigLocationOption)1u) != 0);
				isCompany = (userConfig & (System.Configuration.UserConfigLocationOption)16u) != 0;
				isProduct = (userConfig & System.Configuration.UserConfigLocationOption.Product) != 0;
			}
		}
	}

	public static bool IsVersionMajor
	{
		get
		{
			return isVersionMajor;
		}
		set
		{
			isVersionMajor = value;
			isVersionMinor = false;
			isVersionBuild = false;
			isVersionRevision = false;
		}
	}

	public static bool IsVersionMinor
	{
		get
		{
			return isVersionMinor;
		}
		set
		{
			isVersionMinor = value;
			if (isVersionMinor)
			{
				isVersionMajor = true;
			}
			isVersionBuild = false;
			isVersionRevision = false;
		}
	}

	public static bool IsVersionBuild
	{
		get
		{
			return isVersionBuild;
		}
		set
		{
			isVersionBuild = value;
			if (isVersionBuild)
			{
				isVersionMajor = true;
				isVersionMinor = true;
			}
			isVersionRevision = false;
		}
	}

	public static bool IsVersionRevision
	{
		get
		{
			return isVersionRevision;
		}
		set
		{
			isVersionRevision = value;
			if (isVersionRevision)
			{
				isVersionMajor = true;
				isVersionMinor = true;
				isVersionBuild = true;
			}
		}
	}

	public static bool IsCompany
	{
		get
		{
			return isCompany;
		}
		set
		{
			isCompany = value;
		}
	}

	public static bool IsEvidence
	{
		get
		{
			return isEvidence;
		}
		set
		{
			isEvidence = value;
		}
	}

	public override string Name => base.Name;

	public override string ApplicationName
	{
		get
		{
			return app_name;
		}
		set
		{
			app_name = value;
		}
	}

	public override void Initialize(string name, NameValueCollection config)
	{
		base.Initialize(name, config);
	}

	private static string GetCompanyName()
	{
		Assembly assembly = Assembly.GetEntryAssembly();
		if (assembly == null)
		{
			assembly = Assembly.GetCallingAssembly();
		}
		AssemblyCompanyAttribute[] array = (AssemblyCompanyAttribute[])assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), inherit: true);
		if (array != null && array.Length > 0)
		{
			return array[0].Company;
		}
		Type type = assembly.EntryPoint?.DeclaringType;
		if (type != null && !string.IsNullOrEmpty(type.Namespace))
		{
			int num = type.Namespace.IndexOf('.');
			return (num >= 0) ? type.Namespace.Substring(0, num) : type.Namespace;
		}
		return "Program";
	}

	private static string GetProductName()
	{
		Assembly assembly = Assembly.GetEntryAssembly();
		if (assembly == null)
		{
			assembly = Assembly.GetCallingAssembly();
		}
		byte[] publicKeyToken = assembly.GetName().GetPublicKeyToken();
		return string.Format("{0}_{1}_{2}", AppDomain.CurrentDomain.FriendlyName, (publicKeyToken == null) ? "Url" : "StrongName", GetEvidenceHash());
	}

	private static string GetEvidenceHash()
	{
		Assembly assembly = Assembly.GetEntryAssembly();
		if (assembly == null)
		{
			assembly = Assembly.GetCallingAssembly();
		}
		byte[] publicKeyToken = assembly.GetName().GetPublicKeyToken();
		byte[] inArray = SHA1.Create().ComputeHash((publicKeyToken == null) ? Encoding.UTF8.GetBytes(assembly.EscapedCodeBase) : publicKeyToken);
		return Convert.ToBase64String(inArray);
	}

	private static string GetProductVersion()
	{
		Assembly assembly = Assembly.GetEntryAssembly();
		if (assembly == null)
		{
			assembly = Assembly.GetCallingAssembly();
		}
		if (assembly == null)
		{
			return string.Empty;
		}
		return assembly.GetName().Version.ToString();
	}

	private static void CreateUserConfigPath()
	{
		if (userDefine)
		{
			return;
		}
		if (ProductName == string.Empty)
		{
			ProductName = GetProductName();
		}
		if (CompanyName == string.Empty)
		{
			CompanyName = GetCompanyName();
		}
		if (ForceVersion == string.Empty)
		{
			ProductVersion = GetProductVersion().Split('.');
		}
		if (userRoamingBasePath == string.Empty)
		{
			userRoamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		}
		else
		{
			userRoamingPath = userRoamingBasePath;
		}
		if (userLocalBasePath == string.Empty)
		{
			userLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
		}
		else
		{
			userLocalPath = userLocalBasePath;
		}
		if (isCompany)
		{
			userRoamingPath = Path.Combine(userRoamingPath, CompanyName);
			userLocalPath = Path.Combine(userLocalPath, CompanyName);
		}
		if (isProduct)
		{
			if (isEvidence)
			{
				Assembly assembly = Assembly.GetEntryAssembly();
				if (assembly == null)
				{
					assembly = Assembly.GetCallingAssembly();
				}
				byte[] publicKeyToken = assembly.GetName().GetPublicKeyToken();
				ProductName = string.Format("{0}_{1}_{2}", ProductName, (publicKeyToken == null) ? "Url" : "StrongName", GetEvidenceHash());
			}
			userRoamingPath = Path.Combine(userRoamingPath, ProductName);
			userLocalPath = Path.Combine(userLocalPath, ProductName);
		}
		string text = ((!(ForceVersion == string.Empty)) ? ForceVersion : (isVersionRevision ? $"{ProductVersion[0]}.{ProductVersion[1]}.{ProductVersion[2]}.{ProductVersion[3]}" : (isVersionBuild ? $"{ProductVersion[0]}.{ProductVersion[1]}.{ProductVersion[2]}" : (isVersionMinor ? $"{ProductVersion[0]}.{ProductVersion[1]}" : ((!isVersionMajor) ? string.Empty : ProductVersion[0])))));
		string text2 = PrevVersionPath(userRoamingPath, text);
		string text3 = PrevVersionPath(userLocalPath, text);
		userRoamingPath = Path.Combine(userRoamingPath, text);
		userLocalPath = Path.Combine(userLocalPath, text);
		if (text2 != string.Empty)
		{
			userRoamingPathPrevVersion = Path.Combine(userRoamingPath, text2);
		}
		if (text3 != string.Empty)
		{
			userLocalPathPrevVersion = Path.Combine(userLocalPath, text3);
		}
	}

	private static string PrevVersionPath(string dirName, string currentVersion)
	{
		string text = string.Empty;
		if (!Directory.Exists(dirName))
		{
			return text;
		}
		DirectoryInfo directoryInfo = new DirectoryInfo(dirName);
		DirectoryInfo[] directories = directoryInfo.GetDirectories();
		foreach (DirectoryInfo directoryInfo2 in directories)
		{
			if (string.Compare(currentVersion, directoryInfo2.Name, StringComparison.Ordinal) > 0 && string.Compare(text, directoryInfo2.Name, StringComparison.Ordinal) < 0)
			{
				text = directoryInfo2.Name;
			}
		}
		return text;
	}

	public static bool SetUserRoamingPath(string configPath)
	{
		if (CheckPath(configPath))
		{
			userRoamingBasePath = configPath;
			return true;
		}
		return false;
	}

	public static bool SetUserLocalPath(string configPath)
	{
		if (CheckPath(configPath))
		{
			userLocalBasePath = configPath;
			return true;
		}
		return false;
	}

	private static bool CheckFileName(string configFile)
	{
		return configFile.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
	}

	public static bool SetUserRoamingFileName(string configFile)
	{
		if (CheckFileName(configFile))
		{
			userRoamingName = configFile;
			return true;
		}
		return false;
	}

	public static bool SetUserLocalFileName(string configFile)
	{
		if (CheckFileName(configFile))
		{
			userLocalName = configFile;
			return true;
		}
		return false;
	}

	public static bool SetCompanyName(string companyName)
	{
		if (CheckFileName(companyName))
		{
			CompanyName = companyName;
			return true;
		}
		return false;
	}

	public static bool SetProductName(string productName)
	{
		if (CheckFileName(productName))
		{
			ProductName = productName;
			return true;
		}
		return false;
	}

	public static bool SetVersion(int major)
	{
		ForceVersion = $"{major}";
		return true;
	}

	public static bool SetVersion(int major, int minor)
	{
		ForceVersion = $"{major}.{minor}";
		return true;
	}

	public static bool SetVersion(int major, int minor, int build)
	{
		ForceVersion = $"{major}.{minor}.{build}";
		return true;
	}

	public static bool SetVersion(int major, int minor, int build, int revision)
	{
		ForceVersion = $"{major}.{minor}.{build}.{revision}";
		return true;
	}

	public static bool SetVersion(string forceVersion)
	{
		if (CheckFileName(forceVersion))
		{
			ForceVersion = forceVersion;
			return true;
		}
		return false;
	}

	private static bool CheckPath(string configPath)
	{
		char[] invalidPathChars = Path.GetInvalidPathChars();
		if (configPath.IndexOfAny(invalidPathChars) >= 0)
		{
			return false;
		}
		string path = configPath;
		string fileName;
		while ((fileName = Path.GetFileName(path)) != string.Empty)
		{
			if (!CheckFileName(fileName))
			{
				return false;
			}
			path = Path.GetDirectoryName(path);
		}
		return true;
	}

	private void SaveProperties(ExeConfigurationFileMap exeMap, SettingsPropertyValueCollection collection, ConfigurationUserLevel level, SettingsContext context, bool checkUserLevel)
	{
		Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(exeMap, level);
		UserSettingsGroup userSettingsGroup = configuration.GetSectionGroup("userSettings") as UserSettingsGroup;
		bool flag = level == ConfigurationUserLevel.PerUserRoaming;
		if (userSettingsGroup == null)
		{
			userSettingsGroup = new UserSettingsGroup();
			configuration.SectionGroups.Add("userSettings", userSettingsGroup);
			ApplicationSettingsBase currentSettings = context.CurrentSettings;
			ClientSettingsSection section = new ClientSettingsSection();
			userSettingsGroup.Sections.Add(((currentSettings == null) ? typeof(ApplicationSettingsBase) : currentSettings.GetType()).FullName, section);
		}
		bool flag2 = false;
		foreach (ConfigurationSection section2 in userSettingsGroup.Sections)
		{
			if (!(section2 is ClientSettingsSection clientSettingsSection))
			{
				continue;
			}
			foreach (SettingsPropertyValue item in collection)
			{
				if (!checkUserLevel || item.Property.Attributes.Contains(typeof(SettingsManageabilityAttribute)) == flag)
				{
					flag2 = true;
					SettingElement settingElement = clientSettingsSection.Settings.Get(item.Name);
					if (settingElement == null)
					{
						settingElement = new SettingElement(item.Name, item.Property.SerializeAs);
						clientSettingsSection.Settings.Add(settingElement);
					}
					if (settingElement.Value.ValueXml == null)
					{
						settingElement.Value.ValueXml = new XmlDocument().CreateElement("value");
					}
					switch (item.Property.SerializeAs)
					{
					case SettingsSerializeAs.Xml:
						settingElement.Value.ValueXml.InnerXml = (item.SerializedValue as string) ?? string.Empty;
						break;
					case SettingsSerializeAs.String:
						settingElement.Value.ValueXml.InnerText = item.SerializedValue as string;
						break;
					case SettingsSerializeAs.Binary:
						settingElement.Value.ValueXml.InnerText = ((item.SerializedValue == null) ? string.Empty : Convert.ToBase64String(item.SerializedValue as byte[]));
						break;
					default:
						throw new NotImplementedException();
					}
				}
			}
		}
		if (flag2)
		{
			configuration.Save(ConfigurationSaveMode.Minimal, forceUpdateAll: true);
		}
	}

	private void LoadPropertyValue(SettingsPropertyCollection collection, SettingElement element, bool allowOverwrite)
	{
		SettingsProperty settingsProperty = collection[element.Name];
		if (settingsProperty == null)
		{
			settingsProperty = new SettingsProperty(element.Name);
			collection.Add(settingsProperty);
		}
		SettingsPropertyValue settingsPropertyValue = new SettingsPropertyValue(settingsProperty);
		settingsPropertyValue.IsDirty = false;
		if (element.Value.ValueXml != null)
		{
			switch (settingsPropertyValue.Property.SerializeAs)
			{
			case SettingsSerializeAs.Xml:
				settingsPropertyValue.SerializedValue = element.Value.ValueXml.InnerXml;
				break;
			case SettingsSerializeAs.String:
				settingsPropertyValue.SerializedValue = element.Value.ValueXml.InnerText;
				break;
			case SettingsSerializeAs.Binary:
				settingsPropertyValue.SerializedValue = Convert.FromBase64String(element.Value.ValueXml.InnerText);
				break;
			}
		}
		else
		{
			settingsPropertyValue.SerializedValue = settingsProperty.DefaultValue;
		}
		try
		{
			if (allowOverwrite)
			{
				values.Remove(element.Name);
			}
			values.Add(settingsPropertyValue);
		}
		catch (ArgumentException inner)
		{
			throw new ConfigurationErrorsException(string.Format(CultureInfo.InvariantCulture, "Failed to load value for '{0}'.", element.Name), inner);
		}
	}

	private void LoadProperties(ExeConfigurationFileMap exeMap, SettingsPropertyCollection collection, ConfigurationUserLevel level, string sectionGroupName, bool allowOverwrite, string groupName)
	{
		Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(exeMap, level);
		ConfigurationSectionGroup sectionGroup = configuration.GetSectionGroup(sectionGroupName);
		if (sectionGroup == null)
		{
			return;
		}
		foreach (ConfigurationSection section in sectionGroup.Sections)
		{
			if (section.SectionInformation.Name != groupName || !(section is ClientSettingsSection clientSettingsSection))
			{
				continue;
			}
			{
				foreach (SettingElement setting in clientSettingsSection.Settings)
				{
					LoadPropertyValue(collection, setting, allowOverwrite);
				}
				break;
			}
		}
	}

	public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
	{
		CreateExeMap();
		if (UserLocalFullPath == UserRoamingFullPath)
		{
			SaveProperties(exeMapCurrent, collection, ConfigurationUserLevel.PerUserRoaming, context, checkUserLevel: false);
			return;
		}
		SaveProperties(exeMapCurrent, collection, ConfigurationUserLevel.PerUserRoaming, context, checkUserLevel: true);
		SaveProperties(exeMapCurrent, collection, ConfigurationUserLevel.PerUserRoamingAndLocal, context, checkUserLevel: true);
	}

	public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
	{
		CreateExeMap();
		if (values == null)
		{
			values = new SettingsPropertyValueCollection();
			string groupName = context["GroupName"] as string;
			LoadProperties(exeMapCurrent, collection, ConfigurationUserLevel.None, "applicationSettings", allowOverwrite: false, groupName);
			LoadProperties(exeMapCurrent, collection, ConfigurationUserLevel.None, "userSettings", allowOverwrite: false, groupName);
			LoadProperties(exeMapCurrent, collection, ConfigurationUserLevel.PerUserRoaming, "userSettings", allowOverwrite: true, groupName);
			LoadProperties(exeMapCurrent, collection, ConfigurationUserLevel.PerUserRoamingAndLocal, "userSettings", allowOverwrite: true, groupName);
			foreach (SettingsProperty item in collection)
			{
				if (values[item.Name] == null)
				{
					values.Add(new SettingsPropertyValue(item));
				}
			}
		}
		return values;
	}

	private void CreateExeMap()
	{
		if (exeMapCurrent != null)
		{
			return;
		}
		CreateUserConfigPath();
		exeMapCurrent = new ExeConfigurationFileMap();
		Assembly assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
		exeMapCurrent.ExeConfigFilename = assembly.Location + ".config";
		exeMapCurrent.LocalUserConfigFilename = UserLocalFullPath;
		exeMapCurrent.RoamingUserConfigFilename = UserRoamingFullPath;
		if (webConfigurationFileMapType != null && typeof(ConfigurationFileMap).IsAssignableFrom(webConfigurationFileMapType))
		{
			try
			{
				if (Activator.CreateInstance(webConfigurationFileMapType) is ConfigurationFileMap configurationFileMap)
				{
					string machineConfigFilename = configurationFileMap.MachineConfigFilename;
					if (!string.IsNullOrEmpty(machineConfigFilename))
					{
						exeMapCurrent.ExeConfigFilename = machineConfigFilename;
					}
				}
			}
			catch
			{
			}
		}
		if (PrevUserLocalFullPath != string.Empty && PrevUserRoamingFullPath != string.Empty)
		{
			exeMapPrev = new ExeConfigurationFileMap();
			exeMapPrev.ExeConfigFilename = assembly.Location + ".config";
			exeMapPrev.LocalUserConfigFilename = PrevUserLocalFullPath;
			exeMapPrev.RoamingUserConfigFilename = PrevUserRoamingFullPath;
		}
	}

	public SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property)
	{
		return null;
	}

	public void Reset(SettingsContext context)
	{
		SettingsPropertyCollection collection = new SettingsPropertyCollection();
		GetPropertyValues(context, collection);
		foreach (SettingsPropertyValue value in values)
		{
			value.PropertyValue = value.Reset();
		}
		SetPropertyValues(context, values);
	}

	public void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
	{
	}

	public static void setCreate()
	{
		CreateUserConfigPath();
	}
}

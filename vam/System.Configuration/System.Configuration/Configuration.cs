using System.Collections;
using System.Configuration.Internal;
using System.IO;
using System.Xml;

namespace System.Configuration;

public sealed class Configuration
{
	private Configuration parent;

	private Hashtable elementData = new Hashtable();

	private string streamName;

	private ConfigurationSectionGroup rootSectionGroup;

	private ConfigurationLocationCollection locations;

	private SectionGroupInfo rootGroup;

	private IConfigSystem system;

	private bool hasFile;

	private string rootNamespace;

	private string configPath;

	private string locationConfigPath;

	private string locationSubPath;

	private ContextInformation evaluationContext;

	internal Configuration Parent
	{
		get
		{
			return parent;
		}
		set
		{
			parent = value;
		}
	}

	internal string FileName => streamName;

	internal IInternalConfigHost ConfigHost => system.Host;

	internal string LocationConfigPath => locationConfigPath;

	internal string ConfigPath => configPath;

	public AppSettingsSection AppSettings => (AppSettingsSection)GetSection("appSettings");

	public ConnectionStringsSection ConnectionStrings => (ConnectionStringsSection)GetSection("connectionStrings");

	public string FilePath
	{
		get
		{
			if (streamName == null && parent != null)
			{
				return parent.FilePath;
			}
			return streamName;
		}
	}

	public bool HasFile => hasFile;

	public ContextInformation EvaluationContext
	{
		get
		{
			if (evaluationContext == null)
			{
				object ctx = system.Host.CreateConfigurationContext(configPath, GetLocationSubPath());
				evaluationContext = new ContextInformation(this, ctx);
			}
			return evaluationContext;
		}
	}

	public ConfigurationLocationCollection Locations
	{
		get
		{
			if (locations == null)
			{
				locations = new ConfigurationLocationCollection();
			}
			return locations;
		}
	}

	public bool NamespaceDeclared
	{
		get
		{
			return rootNamespace != null;
		}
		set
		{
			rootNamespace = ((!value) ? null : "http://schemas.microsoft.com/.NetConfiguration/v2.0");
		}
	}

	public ConfigurationSectionGroup RootSectionGroup
	{
		get
		{
			if (rootSectionGroup == null)
			{
				rootSectionGroup = new ConfigurationSectionGroup();
				rootSectionGroup.Initialize(this, rootGroup);
			}
			return rootSectionGroup;
		}
	}

	public ConfigurationSectionGroupCollection SectionGroups => RootSectionGroup.SectionGroups;

	public ConfigurationSectionCollection Sections => RootSectionGroup.Sections;

	internal static event ConfigurationSaveEventHandler SaveStart;

	internal static event ConfigurationSaveEventHandler SaveEnd;

	internal Configuration(Configuration parent, string locationSubPath)
	{
		this.parent = parent;
		system = parent.system;
		rootGroup = parent.rootGroup;
		this.locationSubPath = locationSubPath;
		configPath = parent.ConfigPath;
	}

	internal Configuration(InternalConfigurationSystem system, string locationSubPath)
	{
		hasFile = true;
		this.system = system;
		system.InitForConfiguration(ref locationSubPath, out configPath, out locationConfigPath);
		Configuration configuration = null;
		if (locationSubPath != null)
		{
			configuration = new Configuration(system, locationSubPath);
			if (locationConfigPath != null)
			{
				configuration = configuration.FindLocationConfiguration(locationConfigPath, configuration);
			}
		}
		Init(system, configPath, configuration);
	}

	internal Configuration FindLocationConfiguration(string relativePath, Configuration defaultConfiguration)
	{
		Configuration configuration = defaultConfiguration;
		if (!string.IsNullOrEmpty(LocationConfigPath))
		{
			Configuration parentWithFile = GetParentWithFile();
			if (parentWithFile != null)
			{
				string configPathFromLocationSubPath = system.Host.GetConfigPathFromLocationSubPath(configPath, relativePath);
				configuration = parentWithFile.FindLocationConfiguration(configPathFromLocationSubPath, defaultConfiguration);
			}
		}
		string text = configPath.Substring(1) + "/";
		if (relativePath.StartsWith(text, StringComparison.Ordinal))
		{
			relativePath = relativePath.Substring(text.Length);
		}
		ConfigurationLocation configurationLocation = Locations.Find(relativePath);
		if (configurationLocation == null)
		{
			return configuration;
		}
		configurationLocation.SetParentConfiguration(configuration);
		return configurationLocation.OpenConfiguration();
	}

	internal void Init(IConfigSystem system, string configPath, Configuration parent)
	{
		this.system = system;
		this.configPath = configPath;
		streamName = system.Host.GetStreamName(configPath);
		this.parent = parent;
		if (parent != null)
		{
			rootGroup = parent.rootGroup;
		}
		else
		{
			rootGroup = new SectionGroupInfo();
			rootGroup.StreamName = streamName;
		}
		if (streamName != null)
		{
			Load();
		}
	}

	internal Configuration GetParentWithFile()
	{
		Configuration configuration = Parent;
		while (configuration != null && !configuration.HasFile)
		{
			configuration = configuration.Parent;
		}
		return configuration;
	}

	internal string GetLocationSubPath()
	{
		Configuration configuration = parent;
		string text = null;
		while (configuration != null)
		{
			text = configuration.locationSubPath;
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			configuration = configuration.parent;
		}
		return text;
	}

	public ConfigurationSection GetSection(string path)
	{
		string[] array = path.Split('/');
		if (array.Length == 1)
		{
			return Sections[array[0]];
		}
		ConfigurationSectionGroup configurationSectionGroup = SectionGroups[array[0]];
		int num = 1;
		while (configurationSectionGroup != null && num < array.Length - 1)
		{
			configurationSectionGroup = configurationSectionGroup.SectionGroups[array[num]];
			num++;
		}
		return configurationSectionGroup?.Sections[array[array.Length - 1]];
	}

	public ConfigurationSectionGroup GetSectionGroup(string path)
	{
		string[] array = path.Split('/');
		ConfigurationSectionGroup configurationSectionGroup = SectionGroups[array[0]];
		int num = 1;
		while (configurationSectionGroup != null && num < array.Length)
		{
			configurationSectionGroup = configurationSectionGroup.SectionGroups[array[num]];
			num++;
		}
		return configurationSectionGroup;
	}

	internal ConfigurationSection GetSectionInstance(SectionInfo config, bool createDefaultInstance)
	{
		object obj = elementData[config];
		ConfigurationSection configurationSection = obj as ConfigurationSection;
		if (configurationSection != null || !createDefaultInstance)
		{
			return configurationSection;
		}
		object obj2 = config.CreateInstance();
		configurationSection = obj2 as ConfigurationSection;
		if (configurationSection == null)
		{
			DefaultSection defaultSection = new DefaultSection();
			defaultSection.SectionHandler = obj2 as IConfigurationSectionHandler;
			configurationSection = defaultSection;
		}
		configurationSection.Configuration = this;
		ConfigurationSection configurationSection2 = null;
		if (parent != null)
		{
			configurationSection2 = parent.GetSectionInstance(config, createDefaultInstance: true);
			configurationSection.SectionInformation.SetParentSection(configurationSection2);
		}
		configurationSection.SectionInformation.ConfigFilePath = FilePath;
		configurationSection.ConfigContext = system.Host.CreateDeprecatedConfigContext(configPath);
		string text2 = (configurationSection.RawXml = obj as string);
		configurationSection.Reset(configurationSection2);
		if (text2 != null && text2 == obj)
		{
			XmlTextReader xmlTextReader = new ConfigXmlTextReader(new StringReader(text2), FilePath);
			configurationSection.DeserializeSection(xmlTextReader);
			xmlTextReader.Close();
			if (!string.IsNullOrEmpty(configurationSection.SectionInformation.ConfigSource) && !string.IsNullOrEmpty(FilePath))
			{
				configurationSection.DeserializeConfigSource(Path.GetDirectoryName(FilePath));
			}
		}
		elementData[config] = configurationSection;
		return configurationSection;
	}

	internal ConfigurationSectionGroup GetSectionGroupInstance(SectionGroupInfo group)
	{
		ConfigurationSectionGroup configurationSectionGroup = group.CreateInstance() as ConfigurationSectionGroup;
		configurationSectionGroup?.Initialize(this, group);
		return configurationSectionGroup;
	}

	internal void SetConfigurationSection(SectionInfo config, ConfigurationSection sec)
	{
		elementData[config] = sec;
	}

	internal void SetSectionXml(SectionInfo config, string data)
	{
		elementData[config] = data;
	}

	internal string GetSectionXml(SectionInfo config)
	{
		return elementData[config] as string;
	}

	internal void CreateSection(SectionGroupInfo group, string name, ConfigurationSection sec)
	{
		if (group.HasChild(name))
		{
			throw new ConfigurationException("Cannot add a ConfigurationSection. A section or section group already exists with the name '" + name + "'");
		}
		if (!HasFile && !sec.SectionInformation.AllowLocation)
		{
			throw new ConfigurationErrorsException("The configuration section <" + name + "> cannot be defined inside a <location> element.");
		}
		if (!system.Host.IsDefinitionAllowed(configPath, sec.SectionInformation.AllowDefinition, sec.SectionInformation.AllowExeDefinition))
		{
			object obj = ((sec.SectionInformation.AllowExeDefinition == ConfigurationAllowExeDefinition.MachineToApplication) ? ((object)sec.SectionInformation.AllowDefinition) : ((object)sec.SectionInformation.AllowExeDefinition));
			throw new ConfigurationErrorsException(string.Concat("The section <", name, "> can't be defined in this configuration file (the allowed definition context is '", obj, "')."));
		}
		if (sec.SectionInformation.Type == null)
		{
			sec.SectionInformation.Type = system.Host.GetConfigTypeName(sec.GetType());
		}
		SectionInfo sectionInfo = new SectionInfo(name, sec.SectionInformation);
		sectionInfo.StreamName = streamName;
		sectionInfo.ConfigHost = system.Host;
		group.AddChild(sectionInfo);
		elementData[sectionInfo] = sec;
	}

	internal void CreateSectionGroup(SectionGroupInfo parentGroup, string name, ConfigurationSectionGroup sec)
	{
		if (parentGroup.HasChild(name))
		{
			throw new ConfigurationException("Cannot add a ConfigurationSectionGroup. A section or section group already exists with the name '" + name + "'");
		}
		if (sec.Type == null)
		{
			sec.Type = system.Host.GetConfigTypeName(sec.GetType());
		}
		sec.SetName(name);
		SectionGroupInfo sectionGroupInfo = new SectionGroupInfo(name, sec.Type);
		sectionGroupInfo.StreamName = streamName;
		sectionGroupInfo.ConfigHost = system.Host;
		parentGroup.AddChild(sectionGroupInfo);
		elementData[sectionGroupInfo] = sec;
		sec.Initialize(this, sectionGroupInfo);
	}

	internal void RemoveConfigInfo(ConfigInfo config)
	{
		elementData.Remove(config);
	}

	public void Save()
	{
		Save(ConfigurationSaveMode.Modified, forceUpdateAll: false);
	}

	public void Save(ConfigurationSaveMode mode)
	{
		Save(mode, forceUpdateAll: false);
	}

	public void Save(ConfigurationSaveMode mode, bool forceUpdateAll)
	{
		ConfigurationSaveEventHandler saveStart = Configuration.SaveStart;
		ConfigurationSaveEventHandler saveEnd = Configuration.SaveEnd;
		object writeContext = null;
		Exception ex = null;
		Stream stream = system.Host.OpenStreamForWrite(streamName, null, ref writeContext);
		try
		{
			saveStart?.Invoke(this, new ConfigurationSaveEventArgs(streamName, start: true, null, writeContext));
			Save(stream, mode, forceUpdateAll);
			system.Host.WriteCompleted(streamName, success: true, writeContext);
		}
		catch (Exception ex2)
		{
			ex = ex2;
			system.Host.WriteCompleted(streamName, success: false, writeContext);
			throw;
		}
		finally
		{
			stream.Close();
			saveEnd?.Invoke(this, new ConfigurationSaveEventArgs(streamName, start: false, ex, writeContext));
		}
	}

	public void SaveAs(string filename)
	{
		SaveAs(filename, ConfigurationSaveMode.Modified, forceUpdateAll: false);
	}

	public void SaveAs(string filename, ConfigurationSaveMode mode)
	{
		SaveAs(filename, mode, forceUpdateAll: false);
	}

	[System.MonoInternalNote("Detect if file has changed")]
	public void SaveAs(string filename, ConfigurationSaveMode mode, bool forceUpdateAll)
	{
		string directoryName = Path.GetDirectoryName(Path.GetFullPath(filename));
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		Save(new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write), mode, forceUpdateAll);
	}

	private void Save(Stream stream, ConfigurationSaveMode mode, bool forceUpdateAll)
	{
		XmlTextWriter xmlTextWriter = new XmlTextWriter(new StreamWriter(stream));
		xmlTextWriter.Formatting = Formatting.Indented;
		try
		{
			xmlTextWriter.WriteStartDocument();
			if (rootNamespace != null)
			{
				xmlTextWriter.WriteStartElement("configuration", rootNamespace);
			}
			else
			{
				xmlTextWriter.WriteStartElement("configuration");
			}
			if (rootGroup.HasConfigContent(this))
			{
				rootGroup.WriteConfig(this, xmlTextWriter, mode);
			}
			foreach (ConfigurationLocation location in Locations)
			{
				if (location.OpenedConfiguration == null)
				{
					xmlTextWriter.WriteRaw("\n");
					xmlTextWriter.WriteRaw(location.XmlContent);
					continue;
				}
				xmlTextWriter.WriteStartElement("location");
				xmlTextWriter.WriteAttributeString("path", location.Path);
				if (!location.AllowOverride)
				{
					xmlTextWriter.WriteAttributeString("allowOverride", "false");
				}
				location.OpenedConfiguration.SaveData(xmlTextWriter, mode, forceUpdateAll);
				xmlTextWriter.WriteEndElement();
			}
			SaveData(xmlTextWriter, mode, forceUpdateAll);
			xmlTextWriter.WriteEndElement();
		}
		finally
		{
			xmlTextWriter.Flush();
			xmlTextWriter.Close();
		}
	}

	private void SaveData(XmlTextWriter tw, ConfigurationSaveMode mode, bool forceUpdateAll)
	{
		rootGroup.WriteRootData(tw, this, mode);
	}

	private bool Load()
	{
		if (string.IsNullOrEmpty(streamName))
		{
			return true;
		}
		Stream stream = null;
		try
		{
			stream = (stream = system.Host.OpenStreamForRead(streamName));
		}
		catch
		{
			return false;
		}
		using (XmlTextReader reader = new ConfigXmlTextReader(stream, streamName))
		{
			ReadConfigFile(reader, streamName);
		}
		return true;
	}

	private void ReadConfigFile(XmlReader reader, string fileName)
	{
		reader.MoveToContent();
		if (reader.NodeType != XmlNodeType.Element || reader.Name != "configuration")
		{
			ThrowException("Configuration file does not have a valid root element", reader);
		}
		if (reader.HasAttributes)
		{
			while (reader.MoveToNextAttribute())
			{
				if (reader.LocalName == "xmlns")
				{
					rootNamespace = reader.Value;
				}
				else
				{
					ThrowException($"Unrecognized attribute '{reader.LocalName}' in root element", reader);
				}
			}
		}
		reader.MoveToElement();
		if (reader.IsEmptyElement)
		{
			reader.Skip();
			return;
		}
		reader.ReadStartElement();
		reader.MoveToContent();
		if (reader.LocalName == "configSections")
		{
			if (reader.HasAttributes)
			{
				ThrowException("Unrecognized attribute in <configSections>.", reader);
			}
			rootGroup.ReadConfig(this, fileName, reader);
		}
		rootGroup.ReadRootData(reader, this, overrideAllowed: true);
	}

	internal void ReadData(XmlReader reader, bool allowOverride)
	{
		rootGroup.ReadData(this, reader, allowOverride);
	}

	private void ThrowException(string text, XmlReader reader)
	{
		throw new ConfigurationException(text, streamName, (reader as IXmlLineInfo)?.LineNumber ?? 0);
	}
}

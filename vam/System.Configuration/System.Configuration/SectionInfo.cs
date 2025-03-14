using System.IO;
using System.Text;
using System.Xml;

namespace System.Configuration;

internal class SectionInfo : ConfigInfo
{
	private bool allowLocation = true;

	private bool requirePermission = true;

	private bool restartOnExternalChanges;

	private ConfigurationAllowDefinition allowDefinition = ConfigurationAllowDefinition.Everywhere;

	private ConfigurationAllowExeDefinition allowExeDefinition = ConfigurationAllowExeDefinition.MachineToApplication;

	public SectionInfo()
	{
	}

	public SectionInfo(string sectionName, SectionInformation info)
	{
		Name = sectionName;
		TypeName = info.Type;
		allowLocation = info.AllowLocation;
		allowDefinition = info.AllowDefinition;
		allowExeDefinition = info.AllowExeDefinition;
		requirePermission = info.RequirePermission;
		restartOnExternalChanges = info.RestartOnExternalChanges;
	}

	public override object CreateInstance()
	{
		object obj = base.CreateInstance();
		if (obj is ConfigurationSection configurationSection)
		{
			configurationSection.SectionInformation.AllowLocation = allowLocation;
			configurationSection.SectionInformation.AllowDefinition = allowDefinition;
			configurationSection.SectionInformation.AllowExeDefinition = allowExeDefinition;
			configurationSection.SectionInformation.RequirePermission = requirePermission;
			configurationSection.SectionInformation.RestartOnExternalChanges = restartOnExternalChanges;
			configurationSection.SectionInformation.SetName(Name);
		}
		return obj;
	}

	public override bool HasDataContent(Configuration config)
	{
		return config.GetSectionInstance(this, createDefaultInstance: false) != null || config.GetSectionXml(this) != null;
	}

	public override bool HasConfigContent(Configuration cfg)
	{
		return base.StreamName == cfg.FileName;
	}

	public override void ReadConfig(Configuration cfg, string streamName, XmlReader reader)
	{
		base.StreamName = streamName;
		ConfigHost = cfg.ConfigHost;
		while (reader.MoveToNextAttribute())
		{
			switch (reader.Name)
			{
			case "allowLocation":
			{
				string value2 = reader.Value;
				allowLocation = value2 == "true";
				if (!allowLocation && value2 != "false")
				{
					ThrowException("Invalid attribute value", reader);
				}
				break;
			}
			case "allowDefinition":
			{
				string value5 = reader.Value;
				try
				{
					allowDefinition = (ConfigurationAllowDefinition)(int)Enum.Parse(typeof(ConfigurationAllowDefinition), value5);
				}
				catch
				{
					ThrowException("Invalid attribute value", reader);
				}
				break;
			}
			case "allowExeDefinition":
			{
				string value4 = reader.Value;
				try
				{
					allowExeDefinition = (ConfigurationAllowExeDefinition)(int)Enum.Parse(typeof(ConfigurationAllowExeDefinition), value4);
				}
				catch
				{
					ThrowException("Invalid attribute value", reader);
				}
				break;
			}
			case "type":
				TypeName = reader.Value;
				break;
			case "name":
				Name = reader.Value;
				if (Name == "location")
				{
					ThrowException("location is a reserved section name", reader);
				}
				break;
			case "requirePermission":
			{
				string value3 = reader.Value;
				bool flag2 = value3 == "true";
				if (!flag2 && value3 != "false")
				{
					ThrowException("Invalid attribute value", reader);
				}
				requirePermission = flag2;
				break;
			}
			case "restartOnExternalChanges":
			{
				string value = reader.Value;
				bool flag = value == "true";
				if (!flag && value != "false")
				{
					ThrowException("Invalid attribute value", reader);
				}
				restartOnExternalChanges = flag;
				break;
			}
			default:
				ThrowException($"Unrecognized attribute: {reader.Name}", reader);
				break;
			}
		}
		if (Name == null || TypeName == null)
		{
			ThrowException("Required attribute missing", reader);
		}
		reader.MoveToElement();
		reader.Skip();
	}

	public override void WriteConfig(Configuration cfg, XmlWriter writer, ConfigurationSaveMode mode)
	{
		writer.WriteStartElement("section");
		writer.WriteAttributeString("name", Name);
		writer.WriteAttributeString("type", TypeName);
		if (!allowLocation)
		{
			writer.WriteAttributeString("allowLocation", "false");
		}
		if (allowDefinition != ConfigurationAllowDefinition.Everywhere)
		{
			writer.WriteAttributeString("allowDefinition", allowDefinition.ToString());
		}
		if (allowExeDefinition != ConfigurationAllowExeDefinition.MachineToApplication)
		{
			writer.WriteAttributeString("allowExeDefinition", allowExeDefinition.ToString());
		}
		if (!requirePermission)
		{
			writer.WriteAttributeString("requirePermission", "false");
		}
		writer.WriteEndElement();
	}

	public override void ReadData(Configuration config, XmlReader reader, bool overrideAllowed)
	{
		if (!config.HasFile && !allowLocation)
		{
			throw new ConfigurationErrorsException("The configuration section <" + Name + "> cannot be defined inside a <location> element.", reader);
		}
		if (!config.ConfigHost.IsDefinitionAllowed(config.ConfigPath, allowDefinition, allowExeDefinition))
		{
			object obj = ((allowExeDefinition == ConfigurationAllowExeDefinition.MachineToApplication) ? ((object)allowDefinition) : ((object)allowExeDefinition));
			throw new ConfigurationErrorsException(string.Concat("The section <", Name, "> can't be defined in this configuration file (the allowed definition context is '", obj, "')."), reader);
		}
		if (config.GetSectionXml(this) != null)
		{
			ThrowException("The section <" + Name + "> is defined more than once in the same configuration file.", reader);
		}
		config.SetSectionXml(this, reader.ReadOuterXml());
	}

	public override void WriteData(Configuration config, XmlWriter writer, ConfigurationSaveMode mode)
	{
		ConfigurationSection sectionInstance = config.GetSectionInstance(this, createDefaultInstance: false);
		string text;
		if (sectionInstance != null)
		{
			ConfigurationSection parentElement = ((config.Parent == null) ? null : config.Parent.GetSectionInstance(this, createDefaultInstance: false));
			text = sectionInstance.SerializeSection(parentElement, Name, mode);
			string externalDataXml = sectionInstance.ExternalDataXml;
			string filePath = config.FilePath;
			if (!string.IsNullOrEmpty(filePath) && !string.IsNullOrEmpty(externalDataXml))
			{
				string path = Path.Combine(Path.GetDirectoryName(filePath), sectionInstance.SectionInformation.ConfigSource);
				using StreamWriter streamWriter = new StreamWriter(path);
				streamWriter.Write(externalDataXml);
			}
			if (sectionInstance.SectionInformation.IsProtected)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("<{0} configProtectionProvider=\"{1}\">\n", Name, sectionInstance.SectionInformation.ProtectionProvider.Name);
				stringBuilder.Append(config.ConfigHost.EncryptSection(text, sectionInstance.SectionInformation.ProtectionProvider, ProtectedConfiguration.Section));
				stringBuilder.AppendFormat("</{0}>", Name);
				text = stringBuilder.ToString();
			}
		}
		else
		{
			text = config.GetSectionXml(this);
		}
		if (text != null)
		{
			writer.WriteRaw(text);
		}
	}

	internal override void Merge(ConfigInfo data)
	{
	}
}

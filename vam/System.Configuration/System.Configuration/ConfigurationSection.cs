using System.IO;
using System.Xml;

namespace System.Configuration;

public abstract class ConfigurationSection : ConfigurationElement
{
	private SectionInformation sectionInformation;

	private IConfigurationSectionHandler section_handler;

	private string externalDataXml;

	private object _configContext;

	internal string ExternalDataXml => externalDataXml;

	internal IConfigurationSectionHandler SectionHandler
	{
		get
		{
			return section_handler;
		}
		set
		{
			section_handler = value;
		}
	}

	[System.MonoTODO]
	public SectionInformation SectionInformation
	{
		get
		{
			if (sectionInformation == null)
			{
				sectionInformation = new SectionInformation();
			}
			return sectionInformation;
		}
	}

	internal object ConfigContext
	{
		get
		{
			return _configContext;
		}
		set
		{
			_configContext = value;
		}
	}

	[System.MonoTODO("Provide ConfigContext. Likely the culprit of bug #322493")]
	protected internal virtual object GetRuntimeObject()
	{
		if (SectionHandler != null)
		{
			object obj = ((sectionInformation == null) ? null : sectionInformation.GetParentSection())?.GetRuntimeObject();
			if (base.RawXml == null)
			{
				return obj;
			}
			try
			{
				XmlReader reader = new ConfigXmlTextReader(new StringReader(base.RawXml), base.Configuration.FilePath);
				DoDeserializeSection(reader);
				if (!string.IsNullOrEmpty(SectionInformation.ConfigSource))
				{
					string configFilePath = SectionInformation.ConfigFilePath;
					configFilePath = (string.IsNullOrEmpty(configFilePath) ? string.Empty : Path.GetDirectoryName(configFilePath));
					string path = Path.Combine(configFilePath, SectionInformation.ConfigSource);
					if (File.Exists(path))
					{
						base.RawXml = File.ReadAllText(path);
						SectionInformation.SetRawXml(base.RawXml);
					}
				}
			}
			catch
			{
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(base.RawXml);
			return SectionHandler.Create(obj, ConfigContext, xmlDocument.DocumentElement);
		}
		return this;
	}

	[System.MonoTODO]
	protected internal override bool IsModified()
	{
		return base.IsModified();
	}

	[System.MonoTODO]
	protected internal override void ResetModified()
	{
		base.ResetModified();
	}

	private ConfigurationElement CreateElement(Type t)
	{
		ConfigurationElement configurationElement = (ConfigurationElement)Activator.CreateInstance(t);
		configurationElement.Init();
		if (IsReadOnly())
		{
			configurationElement.SetReadOnly();
		}
		return configurationElement;
	}

	private void DoDeserializeSection(XmlReader reader)
	{
		reader.MoveToContent();
		string text = null;
		string text2 = null;
		while (reader.MoveToNextAttribute())
		{
			string localName = reader.LocalName;
			if (localName == "configProtectionProvider")
			{
				text = reader.Value;
			}
			else if (localName == "configSource")
			{
				text2 = reader.Value;
			}
		}
		if (text != null)
		{
			ProtectedConfigurationProvider provider = ProtectedConfiguration.GetProvider(text, throwOnError: true);
			XmlDocument xmlDocument = new XmlDocument();
			reader.MoveToElement();
			xmlDocument.Load(new StringReader(reader.ReadInnerXml()));
			XmlNode node = provider.Decrypt(xmlDocument);
			reader = new XmlNodeReader(node);
			SectionInformation.ProtectSection(text);
			reader.MoveToContent();
		}
		if (text2 != null)
		{
			SectionInformation.ConfigSource = text2;
		}
		SectionInformation.SetRawXml(base.RawXml);
		DeserializeElement(reader, serializeCollectionKey: false);
	}

	[System.MonoInternalNote("find the proper location for the decryption stuff")]
	protected internal virtual void DeserializeSection(XmlReader reader)
	{
		DoDeserializeSection(reader);
	}

	internal void DeserializeConfigSource(string basePath)
	{
		string configSource = SectionInformation.ConfigSource;
		if (!string.IsNullOrEmpty(configSource))
		{
			if (Path.IsPathRooted(configSource))
			{
				throw new ConfigurationException("The configSource attribute must be a relative physical path.");
			}
			if (HasLocalModifications())
			{
				throw new ConfigurationException("A section using 'configSource' may contain no other attributes or elements.");
			}
			string text = Path.Combine(basePath, configSource);
			if (!File.Exists(text))
			{
				base.RawXml = null;
				SectionInformation.SetRawXml(null);
			}
			else
			{
				base.RawXml = File.ReadAllText(text);
				SectionInformation.SetRawXml(base.RawXml);
				DeserializeElement(new ConfigXmlTextReader(new StringReader(base.RawXml), text), serializeCollectionKey: false);
			}
		}
	}

	protected internal virtual string SerializeSection(ConfigurationElement parentElement, string name, ConfigurationSaveMode saveMode)
	{
		externalDataXml = null;
		ConfigurationElement configurationElement;
		if (parentElement != null)
		{
			configurationElement = CreateElement(GetType());
			configurationElement.Unmerge(this, parentElement, saveMode);
		}
		else
		{
			configurationElement = this;
		}
		string result;
		using (StringWriter stringWriter = new StringWriter())
		{
			using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
			{
				xmlTextWriter.Formatting = Formatting.Indented;
				configurationElement.SerializeToXmlElement(xmlTextWriter, name);
				xmlTextWriter.Close();
			}
			result = stringWriter.ToString();
		}
		string configSource = SectionInformation.ConfigSource;
		if (string.IsNullOrEmpty(configSource))
		{
			return result;
		}
		externalDataXml = result;
		using StringWriter stringWriter2 = new StringWriter();
		bool flag = !string.IsNullOrEmpty(name);
		using (XmlTextWriter xmlTextWriter2 = new XmlTextWriter(stringWriter2))
		{
			if (flag)
			{
				xmlTextWriter2.WriteStartElement(name);
			}
			xmlTextWriter2.WriteAttributeString("configSource", configSource);
			if (flag)
			{
				xmlTextWriter2.WriteEndElement();
			}
		}
		return stringWriter2.ToString();
	}
}

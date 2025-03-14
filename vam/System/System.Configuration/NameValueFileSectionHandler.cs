using System.Collections.Specialized;
using System.IO;
using System.Xml;

namespace System.Configuration;

public class NameValueFileSectionHandler : IConfigurationSectionHandler
{
	public object Create(object parent, object configContext, XmlNode section)
	{
		XmlNode xmlNode = null;
		if (section.Attributes != null)
		{
			xmlNode = section.Attributes.RemoveNamedItem("file");
		}
		NameValueCollection nameValueCollection = System.Configuration.ConfigHelper.GetNameValueCollection(parent as NameValueCollection, section, "key", "value");
		if (xmlNode != null && xmlNode.Value != string.Empty)
		{
			string filename = ((System.Configuration.IConfigXmlNode)section).Filename;
			filename = Path.GetFullPath(filename);
			string text = Path.Combine(Path.GetDirectoryName(filename), xmlNode.Value);
			if (!File.Exists(text))
			{
				return nameValueCollection;
			}
			ConfigXmlDocument configXmlDocument = new ConfigXmlDocument();
			configXmlDocument.Load(text);
			if (configXmlDocument.DocumentElement.Name != section.Name)
			{
				throw new ConfigurationException("Invalid root element", configXmlDocument.DocumentElement);
			}
			nameValueCollection = System.Configuration.ConfigHelper.GetNameValueCollection(nameValueCollection, configXmlDocument.DocumentElement, "key", "value");
		}
		return nameValueCollection;
	}
}

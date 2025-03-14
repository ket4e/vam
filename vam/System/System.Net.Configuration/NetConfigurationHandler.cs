using System.Configuration;
using System.Xml;

namespace System.Net.Configuration;

internal class NetConfigurationHandler : IConfigurationSectionHandler
{
	public virtual object Create(object parent, object configContext, XmlNode section)
	{
		System.Net.NetConfig netConfig = new System.Net.NetConfig();
		if (section.Attributes != null && section.Attributes.Count != 0)
		{
			System.Net.Configuration.HandlersUtil.ThrowException("Unrecognized attribute", section);
		}
		XmlNodeList childNodes = section.ChildNodes;
		foreach (XmlNode item in childNodes)
		{
			XmlNodeType nodeType = item.NodeType;
			if (nodeType == XmlNodeType.Whitespace || nodeType == XmlNodeType.Comment)
			{
				continue;
			}
			if (nodeType != XmlNodeType.Element)
			{
				System.Net.Configuration.HandlersUtil.ThrowException("Only elements allowed", item);
			}
			string name = item.Name;
			if (name == "ipv6")
			{
				string text = System.Net.Configuration.HandlersUtil.ExtractAttributeValue("enabled", item, optional: false);
				if (item.Attributes != null && item.Attributes.Count != 0)
				{
					System.Net.Configuration.HandlersUtil.ThrowException("Unrecognized attribute", item);
				}
				if (text == "true")
				{
					netConfig.ipv6Enabled = true;
				}
				else if (text != "false")
				{
					System.Net.Configuration.HandlersUtil.ThrowException("Invalid boolean value", item);
				}
			}
			else if (name == "httpWebRequest")
			{
				string text2 = System.Net.Configuration.HandlersUtil.ExtractAttributeValue("maximumResponseHeadersLength", item, optional: true);
				System.Net.Configuration.HandlersUtil.ExtractAttributeValue("useUnsafeHeaderParsing", item, optional: true);
				if (item.Attributes != null && item.Attributes.Count != 0)
				{
					System.Net.Configuration.HandlersUtil.ThrowException("Unrecognized attribute", item);
				}
				try
				{
					if (text2 != null)
					{
						int num = int.Parse(text2.Trim());
						if (num < -1)
						{
							System.Net.Configuration.HandlersUtil.ThrowException("Must be -1 or >= 0", item);
						}
						netConfig.MaxResponseHeadersLength = num;
					}
				}
				catch
				{
					System.Net.Configuration.HandlersUtil.ThrowException("Invalid int value", item);
				}
			}
			else
			{
				System.Net.Configuration.HandlersUtil.ThrowException("Unexpected element", item);
			}
		}
		return netConfig;
	}
}

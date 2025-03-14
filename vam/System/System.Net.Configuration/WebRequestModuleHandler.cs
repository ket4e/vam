using System.Configuration;
using System.Xml;

namespace System.Net.Configuration;

internal class WebRequestModuleHandler : IConfigurationSectionHandler
{
	public virtual object Create(object parent, object configContext, XmlNode section)
	{
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
			if (name == "clear")
			{
				if (item.Attributes != null && item.Attributes.Count != 0)
				{
					System.Net.Configuration.HandlersUtil.ThrowException("Unrecognized attribute", item);
				}
				WebRequest.ClearPrefixes();
				continue;
			}
			string prefix = System.Net.Configuration.HandlersUtil.ExtractAttributeValue("prefix", item);
			if (name == "add")
			{
				string typeName = System.Net.Configuration.HandlersUtil.ExtractAttributeValue("type", item, optional: false);
				if (item.Attributes != null && item.Attributes.Count != 0)
				{
					System.Net.Configuration.HandlersUtil.ThrowException("Unrecognized attribute", item);
				}
				WebRequest.AddPrefix(prefix, typeName);
			}
			else if (name == "remove")
			{
				if (item.Attributes != null && item.Attributes.Count != 0)
				{
					System.Net.Configuration.HandlersUtil.ThrowException("Unrecognized attribute", item);
				}
				WebRequest.RemovePrefix(prefix);
			}
			else
			{
				System.Net.Configuration.HandlersUtil.ThrowException("Unexpected element", item);
			}
		}
		return null;
	}
}

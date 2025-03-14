using System.Configuration;
using System.Xml;

namespace System.Net.Configuration;

internal class ConnectionManagementHandler : IConfigurationSectionHandler
{
	public virtual object Create(object parent, object configContext, XmlNode section)
	{
		System.Net.Configuration.ConnectionManagementData connectionManagementData = new System.Net.Configuration.ConnectionManagementData(parent);
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
				connectionManagementData.Clear();
				continue;
			}
			string address = System.Net.Configuration.HandlersUtil.ExtractAttributeValue("address", item);
			if (name == "add")
			{
				string nconns = System.Net.Configuration.HandlersUtil.ExtractAttributeValue("maxconnection", item, optional: true);
				if (item.Attributes != null && item.Attributes.Count != 0)
				{
					System.Net.Configuration.HandlersUtil.ThrowException("Unrecognized attribute", item);
				}
				connectionManagementData.Add(address, nconns);
			}
			else if (name == "remove")
			{
				if (item.Attributes != null && item.Attributes.Count != 0)
				{
					System.Net.Configuration.HandlersUtil.ThrowException("Unrecognized attribute", item);
				}
				connectionManagementData.Remove(address);
			}
			else
			{
				System.Net.Configuration.HandlersUtil.ThrowException("Unexpected element", item);
			}
		}
		return connectionManagementData;
	}
}

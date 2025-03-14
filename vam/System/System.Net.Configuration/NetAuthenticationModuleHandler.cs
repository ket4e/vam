using System.Configuration;
using System.Xml;

namespace System.Net.Configuration;

internal class NetAuthenticationModuleHandler : IConfigurationSectionHandler
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
				AuthenticationManager.Clear();
				continue;
			}
			string typeName = System.Net.Configuration.HandlersUtil.ExtractAttributeValue("type", item);
			if (item.Attributes != null && item.Attributes.Count != 0)
			{
				System.Net.Configuration.HandlersUtil.ThrowException("Unrecognized attribute", item);
			}
			if (name == "add")
			{
				AuthenticationManager.Register(CreateInstance(typeName, item));
			}
			else if (name == "remove")
			{
				AuthenticationManager.Unregister(CreateInstance(typeName, item));
			}
			else
			{
				System.Net.Configuration.HandlersUtil.ThrowException("Unexpected element", item);
			}
		}
		return AuthenticationManager.RegisteredModules;
	}

	private static IAuthenticationModule CreateInstance(string typeName, XmlNode node)
	{
		IAuthenticationModule result = null;
		try
		{
			Type type = Type.GetType(typeName, throwOnError: true);
			result = (IAuthenticationModule)Activator.CreateInstance(type);
		}
		catch (Exception ex)
		{
			System.Net.Configuration.HandlersUtil.ThrowException(ex.Message, node);
		}
		return result;
	}
}

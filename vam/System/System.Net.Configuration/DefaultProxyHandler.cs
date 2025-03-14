using System.Collections;
using System.Configuration;
using System.Xml;

namespace System.Net.Configuration;

internal class DefaultProxyHandler : IConfigurationSectionHandler
{
	public virtual object Create(object parent, object configContext, XmlNode section)
	{
		IWebProxy webProxy = parent as IWebProxy;
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
			string text;
			string environmentVariable;
			switch (item.Name)
			{
			case "proxy":
			{
				text = System.Net.Configuration.HandlersUtil.ExtractAttributeValue("usesystemdefault", item, optional: true);
				string text2 = System.Net.Configuration.HandlersUtil.ExtractAttributeValue("bypassonlocal", item, optional: true);
				environmentVariable = System.Net.Configuration.HandlersUtil.ExtractAttributeValue("proxyaddress", item, optional: true);
				if (item.Attributes != null && item.Attributes.Count != 0)
				{
					System.Net.Configuration.HandlersUtil.ThrowException("Unrecognized attribute", item);
				}
				webProxy = new WebProxy();
				bool flag = text2 != null && string.Compare(text2, "true", ignoreCase: true) == 0;
				if (!flag && text2 != null && string.Compare(text2, "false", ignoreCase: true) != 0)
				{
					System.Net.Configuration.HandlersUtil.ThrowException("Invalid boolean value", item);
				}
				if (!(webProxy is WebProxy))
				{
					continue;
				}
				((WebProxy)webProxy).BypassProxyOnLocal = flag;
				if (environmentVariable != null)
				{
					try
					{
						((WebProxy)webProxy).Address = new Uri(environmentVariable);
					}
					catch (UriFormatException)
					{
						goto IL_0179;
					}
					continue;
				}
				goto IL_0179;
			}
			case "bypasslist":
				if (webProxy is WebProxy)
				{
					FillByPassList(item, (WebProxy)webProxy);
				}
				continue;
			case "module":
				{
					System.Net.Configuration.HandlersUtil.ThrowException("WARNING: module not implemented yet", item);
					break;
				}
				IL_0179:
				if (text == null || string.Compare(text, "true", ignoreCase: true) != 0)
				{
					continue;
				}
				environmentVariable = Environment.GetEnvironmentVariable("http_proxy");
				if (environmentVariable == null)
				{
					environmentVariable = Environment.GetEnvironmentVariable("HTTP_PROXY");
				}
				if (environmentVariable == null)
				{
					continue;
				}
				try
				{
					Uri uri = new Uri(environmentVariable);
					if (IPAddress.TryParse(uri.Host, out var address))
					{
						if (IPAddress.Any.Equals(address))
						{
							UriBuilder uriBuilder = new UriBuilder(uri);
							uriBuilder.Host = "127.0.0.1";
							uri = uriBuilder.Uri;
						}
						else if (IPAddress.IPv6Any.Equals(address))
						{
							UriBuilder uriBuilder2 = new UriBuilder(uri);
							uriBuilder2.Host = "[::1]";
							uri = uriBuilder2.Uri;
						}
					}
					((WebProxy)webProxy).Address = uri;
				}
				catch (UriFormatException)
				{
				}
				continue;
			}
			System.Net.Configuration.HandlersUtil.ThrowException("Unexpected element", item);
		}
		return webProxy;
	}

	private static void FillByPassList(XmlNode node, WebProxy proxy)
	{
		ArrayList arrayList = new ArrayList(proxy.BypassArrayList);
		if (node.Attributes != null && node.Attributes.Count != 0)
		{
			System.Net.Configuration.HandlersUtil.ThrowException("Unrecognized attribute", node);
		}
		XmlNodeList childNodes = node.ChildNodes;
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
			switch (item.Name)
			{
			case "add":
			{
				string text = System.Net.Configuration.HandlersUtil.ExtractAttributeValue("address", item);
				if (!arrayList.Contains(text))
				{
					arrayList.Add(text);
				}
				break;
			}
			case "remove":
			{
				string obj = System.Net.Configuration.HandlersUtil.ExtractAttributeValue("address", item);
				arrayList.Remove(obj);
				break;
			}
			case "clear":
				if (node.Attributes != null && node.Attributes.Count != 0)
				{
					System.Net.Configuration.HandlersUtil.ThrowException("Unrecognized attribute", node);
				}
				arrayList.Clear();
				break;
			default:
				System.Net.Configuration.HandlersUtil.ThrowException("Unexpected element", item);
				break;
			}
		}
		proxy.BypassList = (string[])arrayList.ToArray(typeof(string));
	}
}

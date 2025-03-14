using System.Configuration.Internal;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Xml;

namespace System.Configuration;

[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
public sealed class ConfigXmlDocument : XmlDocument, IConfigErrorInfo, System.Configuration.IConfigXmlNode
{
	private class ConfigXmlAttribute : XmlAttribute, IConfigErrorInfo, System.Configuration.IConfigXmlNode
	{
		private string fileName;

		private int lineNumber;

		public string Filename
		{
			get
			{
				if (fileName != null && fileName.Length > 0 && SecurityManager.SecurityEnabled)
				{
					new FileIOPermission(FileIOPermissionAccess.PathDiscovery, fileName).Demand();
				}
				return fileName;
			}
		}

		public int LineNumber => lineNumber;

		public ConfigXmlAttribute(ConfigXmlDocument document, string prefix, string localName, string namespaceUri)
			: base(prefix, localName, namespaceUri, document)
		{
			fileName = document.fileName;
			lineNumber = document.LineNumber;
		}
	}

	private class ConfigXmlCDataSection : XmlCDataSection, IConfigErrorInfo, System.Configuration.IConfigXmlNode
	{
		private string fileName;

		private int lineNumber;

		public string Filename
		{
			get
			{
				if (fileName != null && fileName.Length > 0 && SecurityManager.SecurityEnabled)
				{
					new FileIOPermission(FileIOPermissionAccess.PathDiscovery, fileName).Demand();
				}
				return fileName;
			}
		}

		public int LineNumber => lineNumber;

		public ConfigXmlCDataSection(ConfigXmlDocument document, string data)
			: base(data, document)
		{
			fileName = document.fileName;
			lineNumber = document.LineNumber;
		}
	}

	private class ConfigXmlComment : XmlComment, System.Configuration.IConfigXmlNode
	{
		private string fileName;

		private int lineNumber;

		public string Filename
		{
			get
			{
				if (fileName != null && fileName.Length > 0 && SecurityManager.SecurityEnabled)
				{
					new FileIOPermission(FileIOPermissionAccess.PathDiscovery, fileName).Demand();
				}
				return fileName;
			}
		}

		public int LineNumber => lineNumber;

		public ConfigXmlComment(ConfigXmlDocument document, string comment)
			: base(comment, document)
		{
			fileName = document.fileName;
			lineNumber = document.LineNumber;
		}
	}

	private class ConfigXmlElement : XmlElement, IConfigErrorInfo, System.Configuration.IConfigXmlNode
	{
		private string fileName;

		private int lineNumber;

		public string Filename
		{
			get
			{
				if (fileName != null && fileName.Length > 0 && SecurityManager.SecurityEnabled)
				{
					new FileIOPermission(FileIOPermissionAccess.PathDiscovery, fileName).Demand();
				}
				return fileName;
			}
		}

		public int LineNumber => lineNumber;

		public ConfigXmlElement(ConfigXmlDocument document, string prefix, string localName, string namespaceUri)
			: base(prefix, localName, namespaceUri, document)
		{
			fileName = document.fileName;
			lineNumber = document.LineNumber;
		}
	}

	private class ConfigXmlText : XmlText, IConfigErrorInfo, System.Configuration.IConfigXmlNode
	{
		private string fileName;

		private int lineNumber;

		public string Filename
		{
			get
			{
				if (fileName != null && fileName.Length > 0 && SecurityManager.SecurityEnabled)
				{
					new FileIOPermission(FileIOPermissionAccess.PathDiscovery, fileName).Demand();
				}
				return fileName;
			}
		}

		public int LineNumber => lineNumber;

		public ConfigXmlText(ConfigXmlDocument document, string data)
			: base(data, document)
		{
			fileName = document.fileName;
			lineNumber = document.LineNumber;
		}
	}

	private XmlTextReader reader;

	private string fileName;

	private int lineNumber;

	string IConfigErrorInfo.Filename => Filename;

	int IConfigErrorInfo.LineNumber => LineNumber;

	string System.Configuration.IConfigXmlNode.Filename => Filename;

	int System.Configuration.IConfigXmlNode.LineNumber => LineNumber;

	public string Filename
	{
		get
		{
			if (fileName != null && fileName.Length > 0 && SecurityManager.SecurityEnabled)
			{
				new FileIOPermission(FileIOPermissionAccess.PathDiscovery, fileName).Demand();
			}
			return fileName;
		}
	}

	public int LineNumber => lineNumber;

	public override XmlAttribute CreateAttribute(string prefix, string localName, string namespaceUri)
	{
		return new ConfigXmlAttribute(this, prefix, localName, namespaceUri);
	}

	public override XmlCDataSection CreateCDataSection(string data)
	{
		return new ConfigXmlCDataSection(this, data);
	}

	public override XmlComment CreateComment(string comment)
	{
		return new ConfigXmlComment(this, comment);
	}

	public override XmlElement CreateElement(string prefix, string localName, string namespaceUri)
	{
		return new ConfigXmlElement(this, prefix, localName, namespaceUri);
	}

	public override XmlSignificantWhitespace CreateSignificantWhitespace(string data)
	{
		return base.CreateSignificantWhitespace(data);
	}

	public override XmlText CreateTextNode(string text)
	{
		return new ConfigXmlText(this, text);
	}

	public override XmlWhitespace CreateWhitespace(string data)
	{
		return base.CreateWhitespace(data);
	}

	public override void Load(string filename)
	{
		XmlTextReader xmlTextReader = new XmlTextReader(filename);
		try
		{
			xmlTextReader.MoveToContent();
			LoadSingleElement(filename, xmlTextReader);
		}
		finally
		{
			xmlTextReader.Close();
		}
	}

	public void LoadSingleElement(string filename, XmlTextReader sourceReader)
	{
		fileName = filename;
		lineNumber = sourceReader.LineNumber;
		string s = sourceReader.ReadOuterXml();
		reader = new XmlTextReader(new StringReader(s), sourceReader.NameTable);
		Load(reader);
		reader.Close();
	}
}

namespace System.Xml.Xsl;

public sealed class XsltSettings
{
	private static readonly XsltSettings defaultSettings;

	private static readonly XsltSettings trustedXslt;

	private bool readOnly;

	private bool enableDocument;

	private bool enableScript;

	public static XsltSettings Default => defaultSettings;

	public static XsltSettings TrustedXslt => trustedXslt;

	public bool EnableDocumentFunction
	{
		get
		{
			return enableDocument;
		}
		set
		{
			if (!readOnly)
			{
				enableDocument = value;
			}
		}
	}

	public bool EnableScript
	{
		get
		{
			return enableScript;
		}
		set
		{
			if (!readOnly)
			{
				enableScript = value;
			}
		}
	}

	public XsltSettings()
	{
	}

	public XsltSettings(bool enableDocumentFunction, bool enableScript)
	{
		enableDocument = enableDocumentFunction;
		this.enableScript = enableScript;
	}

	private XsltSettings(bool readOnly)
	{
		this.readOnly = readOnly;
	}

	static XsltSettings()
	{
		defaultSettings = new XsltSettings(readOnly: true);
		trustedXslt = new XsltSettings(readOnly: true);
		trustedXslt.enableDocument = true;
		trustedXslt.enableScript = true;
	}
}

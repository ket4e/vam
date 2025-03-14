using System.Collections;
using System.Xml;
using Mono.Xml.Xsl.Operations;

namespace Mono.Xml.Xsl;

internal class CompiledStylesheet
{
	private XslStylesheet style;

	private Hashtable globalVariables;

	private Hashtable attrSets;

	private XmlNamespaceManager nsMgr;

	private Hashtable keys;

	private Hashtable outputs;

	private Hashtable decimalFormats;

	private MSXslScriptManager msScripts;

	public Hashtable Variables => globalVariables;

	public XslStylesheet Style => style;

	public XmlNamespaceManager NamespaceManager => nsMgr;

	public Hashtable Keys => keys;

	public Hashtable Outputs => outputs;

	public MSXslScriptManager ScriptManager => msScripts;

	public CompiledStylesheet(XslStylesheet style, Hashtable globalVariables, Hashtable attrSets, XmlNamespaceManager nsMgr, Hashtable keys, Hashtable outputs, Hashtable decimalFormats, MSXslScriptManager msScripts)
	{
		this.style = style;
		this.globalVariables = globalVariables;
		this.attrSets = attrSets;
		this.nsMgr = nsMgr;
		this.keys = keys;
		this.outputs = outputs;
		this.decimalFormats = decimalFormats;
		this.msScripts = msScripts;
	}

	public XslDecimalFormat LookupDecimalFormat(XmlQualifiedName name)
	{
		XslDecimalFormat xslDecimalFormat = decimalFormats[name] as XslDecimalFormat;
		if (xslDecimalFormat == null && name == XmlQualifiedName.Empty)
		{
			return XslDecimalFormat.Default;
		}
		return xslDecimalFormat;
	}

	public XslGeneralVariable ResolveVariable(XmlQualifiedName name)
	{
		return (XslGeneralVariable)globalVariables[name];
	}

	public ArrayList ResolveKey(XmlQualifiedName name)
	{
		return (ArrayList)keys[name];
	}

	public XslAttributeSet ResolveAttributeSet(XmlQualifiedName name)
	{
		return (XslAttributeSet)attrSets[name];
	}
}

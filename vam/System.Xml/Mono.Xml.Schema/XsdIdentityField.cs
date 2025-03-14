using System.Xml.Schema;

namespace Mono.Xml.Schema;

internal class XsdIdentityField
{
	private XsdIdentityPath[] fieldPaths;

	private int index;

	public XsdIdentityPath[] Paths => fieldPaths;

	public int Index => index;

	public XsdIdentityField(XmlSchemaXPath field, int index)
	{
		this.index = index;
		fieldPaths = field.CompiledExpression;
	}
}

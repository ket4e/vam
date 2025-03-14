using System.Xml.XPath;
using Mono.Xml.Xsl.Operations;

namespace Mono.Xml.Xsl;

internal class Sort
{
	private string lang;

	private XmlDataType dataType;

	private XmlSortOrder order;

	private XmlCaseOrder caseOrder;

	private XslAvt langAvt;

	private XslAvt dataTypeAvt;

	private XslAvt orderAvt;

	private XslAvt caseOrderAvt;

	private XPathExpression expr;

	public bool IsContextDependent => orderAvt != null || caseOrderAvt != null || langAvt != null || dataTypeAvt != null;

	public Sort(Compiler c)
	{
		c.CheckExtraAttributes("sort", "select", "lang", "data-type", "order", "case-order");
		expr = c.CompileExpression(c.GetAttribute("select"));
		if (expr == null)
		{
			expr = c.CompileExpression("string(.)");
		}
		langAvt = c.ParseAvtAttribute("lang");
		dataTypeAvt = c.ParseAvtAttribute("data-type");
		orderAvt = c.ParseAvtAttribute("order");
		caseOrderAvt = c.ParseAvtAttribute("case-order");
		lang = ParseLang(XslAvt.AttemptPreCalc(ref langAvt));
		dataType = ParseDataType(XslAvt.AttemptPreCalc(ref dataTypeAvt));
		order = ParseOrder(XslAvt.AttemptPreCalc(ref orderAvt));
		caseOrder = ParseCaseOrder(XslAvt.AttemptPreCalc(ref caseOrderAvt));
	}

	private string ParseLang(string value)
	{
		return value;
	}

	private XmlDataType ParseDataType(string value)
	{
		return value switch
		{
			"number" => XmlDataType.Number, 
			_ => XmlDataType.Text, 
		};
	}

	private XmlSortOrder ParseOrder(string value)
	{
		return value switch
		{
			"descending" => XmlSortOrder.Descending, 
			_ => XmlSortOrder.Ascending, 
		};
	}

	private XmlCaseOrder ParseCaseOrder(string value)
	{
		return value switch
		{
			"upper-first" => XmlCaseOrder.UpperFirst, 
			"lower-first" => XmlCaseOrder.LowerFirst, 
			_ => XmlCaseOrder.None, 
		};
	}

	public void AddToExpr(XPathExpression e, XslTransformProcessor p)
	{
		e.AddSort(expr, (orderAvt != null) ? ParseOrder(orderAvt.Evaluate(p)) : order, (caseOrderAvt != null) ? ParseCaseOrder(caseOrderAvt.Evaluate(p)) : caseOrder, (langAvt != null) ? ParseLang(langAvt.Evaluate(p)) : lang, (dataTypeAvt != null) ? ParseDataType(dataTypeAvt.Evaluate(p)) : dataType);
	}

	public XPathSorter ToXPathSorter(XslTransformProcessor p)
	{
		return new XPathSorter(expr, (orderAvt != null) ? ParseOrder(orderAvt.Evaluate(p)) : order, (caseOrderAvt != null) ? ParseCaseOrder(caseOrderAvt.Evaluate(p)) : caseOrder, (langAvt != null) ? ParseLang(langAvt.Evaluate(p)) : lang, (dataTypeAvt != null) ? ParseDataType(dataTypeAvt.Evaluate(p)) : dataType);
	}
}

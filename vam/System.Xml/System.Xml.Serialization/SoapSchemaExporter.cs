namespace System.Xml.Serialization;

public class SoapSchemaExporter
{
	private XmlSchemaExporter _exporter;

	public SoapSchemaExporter(XmlSchemas schemas)
	{
		_exporter = new XmlSchemaExporter(schemas, encodedFormat: true);
	}

	public void ExportMembersMapping(XmlMembersMapping xmlMembersMapping)
	{
		_exporter.ExportMembersMapping(xmlMembersMapping, exportEnclosingType: false);
	}

	public void ExportMembersMapping(XmlMembersMapping xmlMembersMapping, bool exportEnclosingType)
	{
		_exporter.ExportMembersMapping(xmlMembersMapping, exportEnclosingType);
	}

	public void ExportTypeMapping(XmlTypeMapping xmlTypeMapping)
	{
		_exporter.ExportTypeMapping(xmlTypeMapping);
	}
}

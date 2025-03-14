using System.CodeDom.Compiler;

namespace System.Xml.Serialization;

public class SoapSchemaImporter : SchemaImporter
{
	private XmlSchemaImporter _importer;

	public SoapSchemaImporter(XmlSchemas schemas)
	{
		_importer = new XmlSchemaImporter(schemas);
		_importer.UseEncodedFormat = true;
	}

	public SoapSchemaImporter(XmlSchemas schemas, CodeIdentifiers typeIdentifiers)
	{
		_importer = new XmlSchemaImporter(schemas, typeIdentifiers);
		_importer.UseEncodedFormat = true;
	}

	public SoapSchemaImporter(XmlSchemas schemas, CodeGenerationOptions options, ImportContext context)
	{
		_importer = new XmlSchemaImporter(schemas, options, context);
		_importer.UseEncodedFormat = true;
	}

	public SoapSchemaImporter(XmlSchemas schemas, CodeIdentifiers typeIdentifiers, CodeGenerationOptions options)
	{
		_importer = new XmlSchemaImporter(schemas, typeIdentifiers, options);
		_importer.UseEncodedFormat = true;
	}

	public SoapSchemaImporter(XmlSchemas schemas, CodeGenerationOptions options, CodeDomProvider codeProvider, ImportContext context)
	{
		_importer = new XmlSchemaImporter(schemas, options, codeProvider, context);
		_importer.UseEncodedFormat = true;
	}

	public XmlTypeMapping ImportDerivedTypeMapping(XmlQualifiedName name, Type baseType, bool baseTypeCanBeIndirect)
	{
		return _importer.ImportDerivedTypeMapping(name, baseType, baseTypeCanBeIndirect);
	}

	public XmlMembersMapping ImportMembersMapping(string name, string ns, SoapSchemaMember member)
	{
		return _importer.ImportEncodedMembersMapping(name, ns, member);
	}

	public XmlMembersMapping ImportMembersMapping(string name, string ns, SoapSchemaMember[] members)
	{
		return _importer.ImportEncodedMembersMapping(name, ns, members, hasWrapperElement: false);
	}

	public XmlMembersMapping ImportMembersMapping(string name, string ns, SoapSchemaMember[] members, bool hasWrapperElement)
	{
		return _importer.ImportEncodedMembersMapping(name, ns, members, hasWrapperElement);
	}

	[System.MonoTODO]
	public XmlMembersMapping ImportMembersMapping(string name, string ns, SoapSchemaMember[] members, bool hasWrapperElement, Type baseType, bool baseTypeCanBeIndirect)
	{
		throw new NotImplementedException();
	}
}

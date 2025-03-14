using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;

namespace System.Xml.Serialization;

public class SoapCodeExporter : CodeExporter
{
	public SoapCodeExporter(CodeNamespace codeNamespace)
		: this(codeNamespace, null)
	{
	}

	public SoapCodeExporter(CodeNamespace codeNamespace, CodeCompileUnit codeCompileUnit)
	{
		codeGenerator = new SoapMapCodeGenerator(codeNamespace, codeCompileUnit);
	}

	public SoapCodeExporter(CodeNamespace codeNamespace, CodeCompileUnit codeCompileUnit, CodeGenerationOptions options)
		: this(codeNamespace, codeCompileUnit, null, options, null)
	{
	}

	public SoapCodeExporter(CodeNamespace codeNamespace, CodeCompileUnit codeCompileUnit, CodeGenerationOptions options, Hashtable mappings)
		: this(codeNamespace, codeCompileUnit, null, options, mappings)
	{
	}

	[System.MonoTODO]
	public SoapCodeExporter(CodeNamespace codeNamespace, CodeCompileUnit codeCompileUnit, CodeDomProvider codeProvider, CodeGenerationOptions options, Hashtable mappings)
	{
		codeGenerator = new SoapMapCodeGenerator(codeNamespace, codeCompileUnit, codeProvider, options, mappings);
	}

	public void AddMappingMetadata(CodeAttributeDeclarationCollection metadata, XmlMemberMapping member)
	{
		AddMappingMetadata(metadata, member, forceUseMemberName: false);
	}

	public void AddMappingMetadata(CodeAttributeDeclarationCollection metadata, XmlMemberMapping member, bool forceUseMemberName)
	{
		TypeData typeData = member.TypeMapMember.TypeData;
		CodeAttributeDeclaration codeAttributeDeclaration = new CodeAttributeDeclaration("System.Xml.Serialization.SoapElement");
		if (forceUseMemberName || member.ElementName != member.MemberName)
		{
			codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(member.ElementName)));
		}
		if (!TypeTranslator.IsDefaultPrimitiveTpeData(typeData))
		{
			codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument("DataType", new CodePrimitiveExpression(member.TypeName)));
		}
		if (codeAttributeDeclaration.Arguments.Count > 0)
		{
			metadata.Add(codeAttributeDeclaration);
		}
	}

	public void ExportMembersMapping(XmlMembersMapping xmlMembersMapping)
	{
		codeGenerator.ExportMembersMapping(xmlMembersMapping);
	}

	public void ExportTypeMapping(XmlTypeMapping xmlTypeMapping)
	{
		codeGenerator.ExportTypeMapping(xmlTypeMapping, isTopLevel: true);
	}
}

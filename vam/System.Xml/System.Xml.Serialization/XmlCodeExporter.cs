using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;

namespace System.Xml.Serialization;

public class XmlCodeExporter : CodeExporter
{
	public XmlCodeExporter(CodeNamespace codeNamespace)
		: this(codeNamespace, null)
	{
	}

	public XmlCodeExporter(CodeNamespace codeNamespace, CodeCompileUnit codeCompileUnit)
	{
		codeGenerator = new XmlMapCodeGenerator(codeNamespace, codeCompileUnit, CodeGenerationOptions.GenerateProperties);
	}

	public XmlCodeExporter(CodeNamespace codeNamespace, CodeCompileUnit codeCompileUnit, CodeGenerationOptions options)
		: this(codeNamespace, codeCompileUnit, null, options, null)
	{
	}

	public XmlCodeExporter(CodeNamespace codeNamespace, CodeCompileUnit codeCompileUnit, CodeGenerationOptions options, Hashtable mappings)
		: this(codeNamespace, codeCompileUnit, null, options, mappings)
	{
	}

	[System.MonoTODO]
	public XmlCodeExporter(CodeNamespace codeNamespace, CodeCompileUnit codeCompileUnit, CodeDomProvider codeProvider, CodeGenerationOptions options, Hashtable mappings)
	{
		codeGenerator = new XmlMapCodeGenerator(codeNamespace, codeCompileUnit, codeProvider, options, mappings);
	}

	public void AddMappingMetadata(CodeAttributeDeclarationCollection metadata, XmlMemberMapping member, string ns)
	{
		AddMappingMetadata(metadata, member, ns, forceUseMemberName: false);
	}

	public void AddMappingMetadata(CodeAttributeDeclarationCollection metadata, XmlTypeMapping member, string ns)
	{
		if ((member.TypeData.SchemaType == SchemaTypes.Primitive || member.TypeData.SchemaType == SchemaTypes.Array) && member.Namespace != "http://www.w3.org/2001/XMLSchema")
		{
			CodeAttributeDeclaration codeAttributeDeclaration = new CodeAttributeDeclaration("System.Xml.Serialization.XmlRoot");
			codeAttributeDeclaration.Arguments.Add(MapCodeGenerator.GetArg(member.ElementName));
			codeAttributeDeclaration.Arguments.Add(MapCodeGenerator.GetArg("Namespace", member.Namespace));
			codeAttributeDeclaration.Arguments.Add(MapCodeGenerator.GetArg("IsNullable", member.IsNullable));
			metadata.Add(codeAttributeDeclaration);
		}
	}

	public void AddMappingMetadata(CodeAttributeDeclarationCollection metadata, XmlMemberMapping member, string ns, bool forceUseMemberName)
	{
		TypeData typeData = member.TypeMapMember.TypeData;
		if (member.Any)
		{
			XmlTypeMapElementInfoList elementInfo = ((XmlTypeMapMemberElement)member.TypeMapMember).ElementInfo;
			{
				foreach (XmlTypeMapElementInfo item in elementInfo)
				{
					if (item.IsTextElement)
					{
						metadata.Add(new CodeAttributeDeclaration("System.Xml.Serialization.XmlText"));
						continue;
					}
					CodeAttributeDeclaration codeAttributeDeclaration = new CodeAttributeDeclaration("System.Xml.Serialization.XmlAnyElement");
					if (!item.IsUnnamedAnyElement)
					{
						codeAttributeDeclaration.Arguments.Add(MapCodeGenerator.GetArg("Name", item.ElementName));
						if (item.Namespace != ns)
						{
							codeAttributeDeclaration.Arguments.Add(MapCodeGenerator.GetArg("Namespace", member.Namespace));
						}
					}
					metadata.Add(codeAttributeDeclaration);
				}
				return;
			}
		}
		if (member.TypeMapMember is XmlTypeMapMemberList)
		{
			XmlTypeMapMemberList xmlTypeMapMemberList = member.TypeMapMember as XmlTypeMapMemberList;
			ListMap listMap = (ListMap)xmlTypeMapMemberList.ListTypeMapping.ObjectMap;
			codeGenerator.AddArrayAttributes(metadata, xmlTypeMapMemberList, ns, forceUseMemberName);
			codeGenerator.AddArrayItemAttributes(metadata, listMap, typeData.ListItemTypeData, xmlTypeMapMemberList.Namespace, 0);
		}
		else if (member.TypeMapMember is XmlTypeMapMemberElement)
		{
			codeGenerator.AddElementMemberAttributes((XmlTypeMapMemberElement)member.TypeMapMember, ns, metadata, forceUseMemberName);
		}
		else
		{
			if (!(member.TypeMapMember is XmlTypeMapMemberAttribute))
			{
				throw new NotSupportedException("Schema type not supported");
			}
			codeGenerator.AddAttributeMemberAttributes((XmlTypeMapMemberAttribute)member.TypeMapMember, ns, metadata, forceUseMemberName);
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

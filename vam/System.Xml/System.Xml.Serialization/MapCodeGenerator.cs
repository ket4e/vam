using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using Microsoft.CSharp;

namespace System.Xml.Serialization;

internal class MapCodeGenerator
{
	private CodeNamespace codeNamespace;

	private CodeCompileUnit codeCompileUnit;

	private CodeAttributeDeclarationCollection includeMetadata;

	private XmlTypeMapping exportedAnyType;

	protected bool includeArrayTypes;

	private CodeDomProvider codeProvider;

	private CodeGenerationOptions options;

	private CodeIdentifiers identifiers;

	private Hashtable exportedMaps = new Hashtable();

	private Hashtable includeMaps = new Hashtable();

	public CodeAttributeDeclarationCollection IncludeMetadata
	{
		get
		{
			if (includeMetadata != null)
			{
				return includeMetadata;
			}
			includeMetadata = new CodeAttributeDeclarationCollection();
			foreach (XmlTypeMapping value in includeMaps.Values)
			{
				GenerateClassInclude(includeMetadata, value);
			}
			return includeMetadata;
		}
	}

	private CodeDomProvider CodeProvider
	{
		get
		{
			if (codeProvider == null)
			{
				codeProvider = new CSharpCodeProvider();
			}
			return codeProvider;
		}
	}

	public MapCodeGenerator(CodeNamespace codeNamespace, CodeCompileUnit codeCompileUnit, CodeGenerationOptions options)
	{
		this.codeCompileUnit = codeCompileUnit;
		this.codeNamespace = codeNamespace;
		this.options = options;
		identifiers = new CodeIdentifiers();
	}

	public MapCodeGenerator(CodeNamespace codeNamespace, CodeCompileUnit codeCompileUnit, CodeDomProvider codeProvider, CodeGenerationOptions options, Hashtable mappings)
	{
		this.codeCompileUnit = codeCompileUnit;
		this.codeNamespace = codeNamespace;
		this.options = options;
		this.codeProvider = codeProvider;
		identifiers = new CodeIdentifiers((codeProvider.LanguageOptions & LanguageOptions.CaseInsensitive) == 0);
	}

	public void ExportMembersMapping(XmlMembersMapping xmlMembersMapping)
	{
		CodeTypeDeclaration codeClass = new CodeTypeDeclaration();
		ExportMembersMapCode(codeClass, (ClassMap)xmlMembersMapping.ObjectMap, xmlMembersMapping.Namespace, null);
	}

	public void ExportTypeMapping(XmlTypeMapping xmlTypeMapping, bool isTopLevel)
	{
		ExportMapCode(xmlTypeMapping, isTopLevel);
		RemoveInclude(xmlTypeMapping);
	}

	private void ExportMapCode(XmlTypeMapping map, bool isTopLevel)
	{
		switch (map.TypeData.SchemaType)
		{
		case SchemaTypes.Enum:
			ExportEnumCode(map, isTopLevel);
			break;
		case SchemaTypes.Array:
			ExportArrayCode(map);
			break;
		case SchemaTypes.Class:
			ExportClassCode(map, isTopLevel);
			break;
		case SchemaTypes.Primitive:
		case SchemaTypes.XmlSerializable:
		case SchemaTypes.XmlNode:
			break;
		}
	}

	private void ExportClassCode(XmlTypeMapping map, bool isTopLevel)
	{
		CodeTypeDeclaration mapDeclaration;
		if (IsMapExported(map))
		{
			mapDeclaration = GetMapDeclaration(map);
			if (mapDeclaration != null)
			{
				mapDeclaration.CustomAttributes.Clear();
				AddClassAttributes(mapDeclaration);
				GenerateClass(map, mapDeclaration, isTopLevel);
				ExportDerivedTypeAttributes(map, mapDeclaration);
			}
			return;
		}
		if (map.TypeData.Type == typeof(object))
		{
			exportedAnyType = map;
			SetMapExported(map, null);
			{
				foreach (XmlTypeMapping derivedType in exportedAnyType.DerivedTypes)
				{
					if (!IsMapExported(derivedType) && derivedType.IncludeInSchema)
					{
						ExportTypeMapping(derivedType, isTopLevel: false);
						AddInclude(derivedType);
					}
				}
				return;
			}
		}
		mapDeclaration = new CodeTypeDeclaration(map.TypeData.TypeName);
		SetMapExported(map, mapDeclaration);
		AddCodeType(mapDeclaration, map.Documentation);
		mapDeclaration.Attributes = MemberAttributes.Public;
		mapDeclaration.IsPartial = CodeProvider.Supports(GeneratorSupport.PartialTypes);
		AddClassAttributes(mapDeclaration);
		GenerateClass(map, mapDeclaration, isTopLevel);
		ExportDerivedTypeAttributes(map, mapDeclaration);
		ExportMembersMapCode(mapDeclaration, (ClassMap)map.ObjectMap, map.XmlTypeNamespace, map.BaseMap);
		if (map.BaseMap != null && map.BaseMap.TypeData.SchemaType != SchemaTypes.XmlNode)
		{
			CodeTypeReference domType = GetDomType(map.BaseMap.TypeData, requiresNullable: false);
			mapDeclaration.BaseTypes.Add(domType);
			if (map.BaseMap.IncludeInSchema)
			{
				ExportMapCode(map.BaseMap, isTopLevel: false);
				AddInclude(map.BaseMap);
			}
		}
		ExportDerivedTypes(map, mapDeclaration);
	}

	private void ExportDerivedTypeAttributes(XmlTypeMapping map, CodeTypeDeclaration codeClass)
	{
		foreach (XmlTypeMapping derivedType in map.DerivedTypes)
		{
			GenerateClassInclude(codeClass.CustomAttributes, derivedType);
			ExportDerivedTypeAttributes(derivedType, codeClass);
		}
	}

	private void ExportDerivedTypes(XmlTypeMapping map, CodeTypeDeclaration codeClass)
	{
		foreach (XmlTypeMapping derivedType in map.DerivedTypes)
		{
			if (codeClass.CustomAttributes == null)
			{
				codeClass.CustomAttributes = new CodeAttributeDeclarationCollection();
			}
			ExportMapCode(derivedType, isTopLevel: false);
			ExportDerivedTypes(derivedType, codeClass);
		}
	}

	private void ExportMembersMapCode(CodeTypeDeclaration codeClass, ClassMap map, string defaultNamespace, XmlTypeMapping baseMap)
	{
		ICollection attributeMembers = map.AttributeMembers;
		ICollection elementMembers = map.ElementMembers;
		if (attributeMembers != null)
		{
			foreach (XmlTypeMapMemberAttribute item in attributeMembers)
			{
				identifiers.AddUnique(item.Name, item);
			}
		}
		if (elementMembers != null)
		{
			foreach (XmlTypeMapMemberElement item2 in elementMembers)
			{
				identifiers.AddUnique(item2.Name, item2);
			}
		}
		if (attributeMembers != null)
		{
			foreach (XmlTypeMapMemberAttribute item3 in attributeMembers)
			{
				if (baseMap == null || !DefinedInBaseMap(baseMap, item3))
				{
					AddAttributeFieldMember(codeClass, item3, defaultNamespace);
				}
			}
		}
		elementMembers = map.ElementMembers;
		if (elementMembers != null)
		{
			foreach (XmlTypeMapMemberElement item4 in elementMembers)
			{
				if (baseMap != null && DefinedInBaseMap(baseMap, item4))
				{
					continue;
				}
				Type type = item4.GetType();
				if (type == typeof(XmlTypeMapMemberList))
				{
					AddArrayElementFieldMember(codeClass, (XmlTypeMapMemberList)item4, defaultNamespace);
					continue;
				}
				if (type == typeof(XmlTypeMapMemberFlatList))
				{
					AddElementFieldMember(codeClass, item4, defaultNamespace);
					continue;
				}
				if (type == typeof(XmlTypeMapMemberAnyElement))
				{
					AddAnyElementFieldMember(codeClass, item4, defaultNamespace);
					continue;
				}
				if (type == typeof(XmlTypeMapMemberElement))
				{
					AddElementFieldMember(codeClass, item4, defaultNamespace);
					continue;
				}
				throw new InvalidOperationException(string.Concat("Member type ", type, " not supported"));
			}
		}
		XmlTypeMapMember defaultAnyAttributeMember = map.DefaultAnyAttributeMember;
		if (defaultAnyAttributeMember != null)
		{
			CodeTypeMember codeTypeMember = CreateFieldMember(codeClass, defaultAnyAttributeMember.TypeData, defaultAnyAttributeMember.Name);
			AddComments(codeTypeMember, defaultAnyAttributeMember.Documentation);
			codeTypeMember.Attributes = MemberAttributes.Public;
			GenerateAnyAttribute(codeTypeMember);
		}
	}

	private CodeTypeMember CreateFieldMember(CodeTypeDeclaration codeClass, Type type, string name)
	{
		return CreateFieldMember(codeClass, new CodeTypeReference(type), name, DBNull.Value, null, null);
	}

	private CodeTypeMember CreateFieldMember(CodeTypeDeclaration codeClass, TypeData type, string name)
	{
		return CreateFieldMember(codeClass, GetDomType(type, requiresNullable: false), name, DBNull.Value, null, null);
	}

	private CodeTypeMember CreateFieldMember(CodeTypeDeclaration codeClass, XmlTypeMapMember member)
	{
		return CreateFieldMember(codeClass, GetDomType(member.TypeData, member.RequiresNullable), member.Name, member.DefaultValue, member.TypeData, member.Documentation);
	}

	private CodeTypeMember CreateFieldMember(CodeTypeDeclaration codeClass, CodeTypeReference type, string name, object defaultValue, TypeData defaultType, string documentation)
	{
		CodeMemberField codeMemberField = null;
		CodeTypeMember codeTypeMember = null;
		if ((options & CodeGenerationOptions.GenerateProperties) > CodeGenerationOptions.None)
		{
			string text = identifiers.AddUnique(CodeIdentifier.MakeCamel(name + "Field"), name);
			codeMemberField = new CodeMemberField(type, text);
			codeMemberField.Attributes = MemberAttributes.Private;
			codeClass.Members.Add(codeMemberField);
			CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
			codeMemberProperty.Name = name;
			codeMemberProperty.Type = type;
			codeMemberProperty.Attributes = (MemberAttributes)24578;
			codeTypeMember = codeMemberProperty;
			bool hasGet = (codeMemberProperty.HasSet = true);
			codeMemberProperty.HasGet = hasGet;
			CodeExpression codeExpression = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), text);
			codeMemberProperty.SetStatements.Add(new CodeAssignStatement(codeExpression, new CodePropertySetValueReferenceExpression()));
			codeMemberProperty.GetStatements.Add(new CodeMethodReturnStatement(codeExpression));
		}
		else
		{
			codeMemberField = new CodeMemberField(type, name);
			codeMemberField.Attributes = MemberAttributes.Public;
			codeTypeMember = codeMemberField;
		}
		if (defaultValue != DBNull.Value)
		{
			GenerateDefaultAttribute(codeMemberField, codeTypeMember, defaultType, defaultValue);
		}
		AddComments(codeTypeMember, documentation);
		codeClass.Members.Add(codeTypeMember);
		return codeTypeMember;
	}

	private void AddAttributeFieldMember(CodeTypeDeclaration codeClass, XmlTypeMapMemberAttribute attinfo, string defaultNamespace)
	{
		CodeTypeMember codeTypeMember = CreateFieldMember(codeClass, attinfo);
		CodeAttributeDeclarationCollection codeAttributeDeclarationCollection = codeTypeMember.CustomAttributes;
		if (codeAttributeDeclarationCollection == null)
		{
			codeAttributeDeclarationCollection = new CodeAttributeDeclarationCollection();
		}
		GenerateAttributeMember(codeAttributeDeclarationCollection, attinfo, defaultNamespace, forceUseMemberName: false);
		if (codeAttributeDeclarationCollection.Count > 0)
		{
			codeTypeMember.CustomAttributes = codeAttributeDeclarationCollection;
		}
		if (attinfo.MappedType != null)
		{
			ExportMapCode(attinfo.MappedType, isTopLevel: false);
			RemoveInclude(attinfo.MappedType);
		}
		if (attinfo.TypeData.IsValueType && attinfo.IsOptionalValueType)
		{
			codeTypeMember = CreateFieldMember(codeClass, typeof(bool), identifiers.MakeUnique(attinfo.Name + "Specified"));
			codeTypeMember.Attributes = MemberAttributes.Public;
			GenerateSpecifierMember(codeTypeMember);
		}
	}

	public void AddAttributeMemberAttributes(XmlTypeMapMemberAttribute attinfo, string defaultNamespace, CodeAttributeDeclarationCollection attributes, bool forceUseMemberName)
	{
		GenerateAttributeMember(attributes, attinfo, defaultNamespace, forceUseMemberName);
	}

	private void AddElementFieldMember(CodeTypeDeclaration codeClass, XmlTypeMapMemberElement member, string defaultNamespace)
	{
		CodeTypeMember codeTypeMember = CreateFieldMember(codeClass, member);
		CodeAttributeDeclarationCollection codeAttributeDeclarationCollection = codeTypeMember.CustomAttributes;
		if (codeAttributeDeclarationCollection == null)
		{
			codeAttributeDeclarationCollection = new CodeAttributeDeclarationCollection();
		}
		AddElementMemberAttributes(member, defaultNamespace, codeAttributeDeclarationCollection, forceUseMemberName: false);
		if (codeAttributeDeclarationCollection.Count > 0)
		{
			codeTypeMember.CustomAttributes = codeAttributeDeclarationCollection;
		}
		if (member.TypeData.IsValueType && member.IsOptionalValueType)
		{
			codeTypeMember = CreateFieldMember(codeClass, typeof(bool), identifiers.MakeUnique(member.Name + "Specified"));
			codeTypeMember.Attributes = MemberAttributes.Public;
			GenerateSpecifierMember(codeTypeMember);
		}
	}

	public void AddElementMemberAttributes(XmlTypeMapMemberElement member, string defaultNamespace, CodeAttributeDeclarationCollection attributes, bool forceUseMemberName)
	{
		TypeData typeData = member.TypeData;
		bool flag = false;
		if (member is XmlTypeMapMemberFlatList)
		{
			typeData = typeData.ListItemTypeData;
			flag = true;
		}
		foreach (XmlTypeMapElementInfo item in member.ElementInfo)
		{
			if (item.MappedType != null)
			{
				ExportMapCode(item.MappedType, isTopLevel: false);
				RemoveInclude(item.MappedType);
			}
			if (!ExportExtraElementAttributes(attributes, item, defaultNamespace, typeData))
			{
				GenerateElementInfoMember(attributes, member, item, typeData, defaultNamespace, flag, forceUseMemberName || flag);
			}
		}
		GenerateElementMember(attributes, member);
	}

	private void AddAnyElementFieldMember(CodeTypeDeclaration codeClass, XmlTypeMapMemberElement member, string defaultNamespace)
	{
		CodeTypeMember codeTypeMember = CreateFieldMember(codeClass, member);
		CodeAttributeDeclarationCollection codeAttributeDeclarationCollection = new CodeAttributeDeclarationCollection();
		foreach (XmlTypeMapElementInfo item in member.ElementInfo)
		{
			ExportExtraElementAttributes(codeAttributeDeclarationCollection, item, defaultNamespace, item.TypeData);
		}
		if (codeAttributeDeclarationCollection.Count > 0)
		{
			codeTypeMember.CustomAttributes = codeAttributeDeclarationCollection;
		}
	}

	private bool DefinedInBaseMap(XmlTypeMapping map, XmlTypeMapMember member)
	{
		if (((ClassMap)map.ObjectMap).FindMember(member.Name) != null)
		{
			return true;
		}
		if (map.BaseMap != null)
		{
			return DefinedInBaseMap(map.BaseMap, member);
		}
		return false;
	}

	private void AddArrayElementFieldMember(CodeTypeDeclaration codeClass, XmlTypeMapMemberList member, string defaultNamespace)
	{
		CodeTypeMember codeTypeMember = CreateFieldMember(codeClass, member.TypeData, member.Name);
		CodeAttributeDeclarationCollection codeAttributeDeclarationCollection = new CodeAttributeDeclarationCollection();
		AddArrayAttributes(codeAttributeDeclarationCollection, member, defaultNamespace, forceUseMemberName: false);
		ListMap listMap = (ListMap)member.ListTypeMapping.ObjectMap;
		AddArrayItemAttributes(codeAttributeDeclarationCollection, listMap, member.TypeData.ListItemTypeData, defaultNamespace, 0);
		if (codeAttributeDeclarationCollection.Count > 0)
		{
			codeTypeMember.CustomAttributes = codeAttributeDeclarationCollection;
		}
	}

	public void AddArrayAttributes(CodeAttributeDeclarationCollection attributes, XmlTypeMapMemberElement member, string defaultNamespace, bool forceUseMemberName)
	{
		GenerateArrayElement(attributes, member, defaultNamespace, forceUseMemberName);
	}

	public void AddArrayItemAttributes(CodeAttributeDeclarationCollection attributes, ListMap listMap, TypeData type, string defaultNamespace, int nestingLevel)
	{
		foreach (XmlTypeMapElementInfo item in listMap.ItemInfo)
		{
			string defaultName = ((item.MappedType == null) ? item.TypeData.XmlType : item.MappedType.ElementName);
			GenerateArrayItemAttributes(attributes, listMap, type, item, defaultName, defaultNamespace, nestingLevel);
			if (item.MappedType != null)
			{
				if (!IsMapExported(item.MappedType) && includeArrayTypes)
				{
					AddInclude(item.MappedType);
				}
				ExportMapCode(item.MappedType, isTopLevel: false);
			}
		}
		if (listMap.IsMultiArray)
		{
			XmlTypeMapping nestedArrayMapping = listMap.NestedArrayMapping;
			AddArrayItemAttributes(attributes, (ListMap)nestedArrayMapping.ObjectMap, nestedArrayMapping.TypeData.ListItemTypeData, defaultNamespace, nestingLevel + 1);
		}
	}

	private void ExportArrayCode(XmlTypeMapping map)
	{
		ListMap listMap = (ListMap)map.ObjectMap;
		foreach (XmlTypeMapElementInfo item in listMap.ItemInfo)
		{
			if (item.MappedType != null)
			{
				if (!IsMapExported(item.MappedType) && includeArrayTypes)
				{
					AddInclude(item.MappedType);
				}
				ExportMapCode(item.MappedType, isTopLevel: false);
			}
		}
	}

	private bool ExportExtraElementAttributes(CodeAttributeDeclarationCollection attributes, XmlTypeMapElementInfo einfo, string defaultNamespace, TypeData defaultType)
	{
		if (einfo.IsTextElement)
		{
			GenerateTextElementAttribute(attributes, einfo, defaultType);
			return true;
		}
		if (einfo.IsUnnamedAnyElement)
		{
			GenerateUnnamedAnyElementAttribute(attributes, einfo, defaultNamespace);
			return true;
		}
		return false;
	}

	private void ExportEnumCode(XmlTypeMapping map, bool isTopLevel)
	{
		if (IsMapExported(map))
		{
			return;
		}
		CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration(map.TypeData.TypeName);
		SetMapExported(map, codeTypeDeclaration);
		codeTypeDeclaration.Attributes = MemberAttributes.Public;
		codeTypeDeclaration.IsEnum = true;
		AddCodeType(codeTypeDeclaration, map.Documentation);
		EnumMap enumMap = (EnumMap)map.ObjectMap;
		if (enumMap.IsFlags)
		{
			codeTypeDeclaration.CustomAttributes.Add(new CodeAttributeDeclaration("System.FlagsAttribute"));
		}
		CodeAttributeDeclaration codeAttributeDeclaration = new CodeAttributeDeclaration(new CodeTypeReference(typeof(GeneratedCodeAttribute)));
		codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression("System.Xml")));
		codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression("2.0.50727.1433")));
		codeTypeDeclaration.CustomAttributes.Add(codeAttributeDeclaration);
		codeTypeDeclaration.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(SerializableAttribute))));
		GenerateEnum(map, codeTypeDeclaration, isTopLevel);
		int num = 1;
		EnumMap.EnumMapMember[] members = enumMap.Members;
		foreach (EnumMap.EnumMapMember enumMapMember in members)
		{
			CodeMemberField codeMemberField = new CodeMemberField(string.Empty, enumMapMember.EnumName);
			if (enumMap.IsFlags)
			{
				codeMemberField.InitExpression = new CodePrimitiveExpression(num);
				num *= 2;
			}
			AddComments(codeMemberField, enumMapMember.Documentation);
			GenerateEnumItem(codeMemberField, enumMapMember);
			codeTypeDeclaration.Members.Add(codeMemberField);
		}
	}

	private void AddInclude(XmlTypeMapping map)
	{
		if (!includeMaps.ContainsKey(map.TypeData.FullTypeName))
		{
			includeMaps[map.TypeData.FullTypeName] = map;
		}
	}

	private void RemoveInclude(XmlTypeMapping map)
	{
		includeMaps.Remove(map.TypeData.FullTypeName);
	}

	private bool IsMapExported(XmlTypeMapping map)
	{
		if (exportedMaps.Contains(map.TypeData.FullTypeName))
		{
			return true;
		}
		return false;
	}

	private void SetMapExported(XmlTypeMapping map, CodeTypeDeclaration declaration)
	{
		exportedMaps.Add(map.TypeData.FullTypeName, declaration);
	}

	private CodeTypeDeclaration GetMapDeclaration(XmlTypeMapping map)
	{
		return exportedMaps[map.TypeData.FullTypeName] as CodeTypeDeclaration;
	}

	public static void AddCustomAttribute(CodeTypeMember ctm, CodeAttributeDeclaration att, bool addIfNoParams)
	{
		if (att.Arguments.Count != 0 || addIfNoParams)
		{
			if (ctm.CustomAttributes == null)
			{
				ctm.CustomAttributes = new CodeAttributeDeclarationCollection();
			}
			ctm.CustomAttributes.Add(att);
		}
	}

	public static void AddCustomAttribute(CodeTypeMember ctm, string name, params CodeAttributeArgument[] args)
	{
		if (ctm.CustomAttributes == null)
		{
			ctm.CustomAttributes = new CodeAttributeDeclarationCollection();
		}
		ctm.CustomAttributes.Add(new CodeAttributeDeclaration(name, args));
	}

	public static CodeAttributeArgument GetArg(string name, object value)
	{
		return new CodeAttributeArgument(name, new CodePrimitiveExpression(value));
	}

	public static CodeAttributeArgument GetArg(object value)
	{
		return new CodeAttributeArgument(new CodePrimitiveExpression(value));
	}

	public static CodeAttributeArgument GetTypeArg(string name, string typeName)
	{
		return new CodeAttributeArgument(name, new CodeTypeOfExpression(typeName));
	}

	public static CodeAttributeArgument GetEnumArg(string name, string enumType, string enumValue)
	{
		return new CodeAttributeArgument(name, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(enumType), enumValue));
	}

	public static void AddComments(CodeTypeMember member, string comments)
	{
		if (comments == null || comments == string.Empty)
		{
			member.Comments.Add(new CodeCommentStatement("<remarks/>", docComment: true));
		}
		else
		{
			member.Comments.Add(new CodeCommentStatement("<remarks>\n" + comments + "\n</remarks>", docComment: true));
		}
	}

	private void AddCodeType(CodeTypeDeclaration type, string comments)
	{
		AddComments(type, comments);
		codeNamespace.Types.Add(type);
	}

	private void AddClassAttributes(CodeTypeDeclaration codeClass)
	{
		CodeAttributeDeclaration codeAttributeDeclaration = new CodeAttributeDeclaration(new CodeTypeReference(typeof(GeneratedCodeAttribute)));
		codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression("System.Xml")));
		codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression("2.0.50727.1433")));
		codeClass.CustomAttributes.Add(codeAttributeDeclaration);
		codeClass.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(SerializableAttribute))));
		codeClass.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(DebuggerStepThroughAttribute))));
		CodeAttributeDeclaration codeAttributeDeclaration2 = new CodeAttributeDeclaration(new CodeTypeReference(typeof(DesignerCategoryAttribute)));
		codeAttributeDeclaration2.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression("code")));
		codeClass.CustomAttributes.Add(codeAttributeDeclaration2);
	}

	private CodeTypeReference GetDomType(TypeData data, bool requiresNullable)
	{
		if (data.IsValueType && (data.IsNullable || requiresNullable))
		{
			return new CodeTypeReference("System.Nullable", new CodeTypeReference(data.FullTypeName));
		}
		if (data.SchemaType == SchemaTypes.Array)
		{
			return new CodeTypeReference(GetDomType(data.ListItemTypeData, requiresNullable: false), 1);
		}
		return new CodeTypeReference(data.FullTypeName);
	}

	protected virtual void GenerateClass(XmlTypeMapping map, CodeTypeDeclaration codeClass, bool isTopLevel)
	{
	}

	protected virtual void GenerateClassInclude(CodeAttributeDeclarationCollection attributes, XmlTypeMapping map)
	{
	}

	protected virtual void GenerateAnyAttribute(CodeTypeMember codeField)
	{
	}

	protected virtual void GenerateDefaultAttribute(CodeMemberField internalField, CodeTypeMember externalField, TypeData typeData, object defaultValue)
	{
		if (typeData.Type == null)
		{
			if (typeData.SchemaType != SchemaTypes.Enum)
			{
				throw new InvalidOperationException("Type " + typeData.TypeName + " not supported");
			}
			IFormattable formattable = defaultValue as IFormattable;
			CodeFieldReferenceExpression value = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(GetDomType(typeData, requiresNullable: false)), (formattable == null) ? defaultValue.ToString() : formattable.ToString(null, CultureInfo.InvariantCulture));
			CodeAttributeArgument codeAttributeArgument = new CodeAttributeArgument(value);
			AddCustomAttribute(externalField, "System.ComponentModel.DefaultValue", codeAttributeArgument);
		}
		else
		{
			AddCustomAttribute(externalField, "System.ComponentModel.DefaultValue", GetArg(defaultValue));
		}
	}

	protected virtual void GenerateAttributeMember(CodeAttributeDeclarationCollection attributes, XmlTypeMapMemberAttribute attinfo, string defaultNamespace, bool forceUseMemberName)
	{
	}

	protected virtual void GenerateElementInfoMember(CodeAttributeDeclarationCollection attributes, XmlTypeMapMemberElement member, XmlTypeMapElementInfo einfo, TypeData defaultType, string defaultNamespace, bool addAlwaysAttr, bool forceUseMemberName)
	{
	}

	protected virtual void GenerateElementMember(CodeAttributeDeclarationCollection attributes, XmlTypeMapMemberElement member)
	{
	}

	protected virtual void GenerateArrayElement(CodeAttributeDeclarationCollection attributes, XmlTypeMapMemberElement member, string defaultNamespace, bool forceUseMemberName)
	{
	}

	protected virtual void GenerateArrayItemAttributes(CodeAttributeDeclarationCollection attributes, ListMap listMap, TypeData type, XmlTypeMapElementInfo ainfo, string defaultName, string defaultNamespace, int nestingLevel)
	{
	}

	protected virtual void GenerateTextElementAttribute(CodeAttributeDeclarationCollection attributes, XmlTypeMapElementInfo einfo, TypeData defaultType)
	{
	}

	protected virtual void GenerateUnnamedAnyElementAttribute(CodeAttributeDeclarationCollection attributes, XmlTypeMapElementInfo einfo, string defaultNamespace)
	{
	}

	protected virtual void GenerateEnum(XmlTypeMapping map, CodeTypeDeclaration codeEnum, bool isTopLevel)
	{
	}

	protected virtual void GenerateEnumItem(CodeMemberField codeField, EnumMap.EnumMapMember emem)
	{
	}

	protected virtual void GenerateSpecifierMember(CodeTypeMember codeField)
	{
	}
}

using System.CodeDom;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Runtime.Serialization;

public class XsdDataContractImporter
{
	private const string default_ns_prefix = "http://schemas.datacontract.org/2004/07/";

	private ImportOptions options;

	private CodeCompileUnit ccu;

	private Dictionary<XmlQualifiedName, XmlQualifiedName> imported_names = new Dictionary<XmlQualifiedName, XmlQualifiedName>();

	private static readonly char[] split_tokens = new char[2] { '/', '.' };

	public CodeCompileUnit CodeCompileUnit
	{
		get
		{
			if (ccu == null)
			{
				ccu = new CodeCompileUnit();
			}
			return ccu;
		}
	}

	public ImportOptions Options
	{
		get
		{
			return options;
		}
		set
		{
			options = value;
		}
	}

	public XsdDataContractImporter()
		: this(null)
	{
	}

	public XsdDataContractImporter(CodeCompileUnit ccu)
	{
		this.ccu = ccu;
		imported_names = new Dictionary<XmlQualifiedName, XmlQualifiedName>();
	}

	[System.MonoTODO]
	public ICollection<CodeTypeReference> GetKnownTypeReferences(XmlQualifiedName typeName)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public CodeTypeReference GetCodeTypeReference(XmlQualifiedName typeName)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public CodeTypeReference GetCodeTypeReference(XmlQualifiedName typeName, XmlSchemaElement element)
	{
		throw new NotImplementedException();
	}

	public bool CanImport(XmlSchemaSet schemas)
	{
		foreach (XmlSchemaElement globalElement in schemas.GlobalElements)
		{
			if (!CanImport(schemas, globalElement))
			{
				return false;
			}
		}
		return true;
	}

	public bool CanImport(XmlSchemaSet schemas, ICollection<XmlQualifiedName> typeNames)
	{
		foreach (XmlQualifiedName typeName in typeNames)
		{
			if (!CanImport(schemas, typeName))
			{
				return false;
			}
		}
		return true;
	}

	public bool CanImport(XmlSchemaSet schemas, XmlQualifiedName name)
	{
		return CanImport(schemas, (XmlSchemaElement)schemas.GlobalElements[name]);
	}

	[System.MonoTODO]
	public bool CanImport(XmlSchemaSet schemas, XmlSchemaElement element)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public void Import(XmlSchemaSet schemas)
	{
		if (schemas == null)
		{
			throw new ArgumentNullException("schemas");
		}
		schemas.Compile();
		foreach (XmlSchemaElement value in schemas.GlobalElements.Values)
		{
			ImportInternal(schemas, value.QualifiedName);
		}
	}

	public void Import(XmlSchemaSet schemas, ICollection<XmlQualifiedName> typeNames)
	{
		if (schemas == null)
		{
			throw new ArgumentNullException("schemas");
		}
		if (typeNames == null)
		{
			throw new ArgumentNullException("typeNames");
		}
		schemas.Compile();
		foreach (XmlQualifiedName typeName in typeNames)
		{
			ImportInternal(schemas, typeName);
		}
	}

	public void Import(XmlSchemaSet schemas, XmlQualifiedName name)
	{
		if (schemas == null)
		{
			throw new ArgumentNullException("schemas");
		}
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		schemas.Compile();
		if (schemas.GlobalTypes[name] == null)
		{
			throw new InvalidDataContractException($"Type with name '{name.Name}' not found in schema with namespace '{name.Namespace}'");
		}
		ImportInternal(schemas, name);
	}

	[System.MonoTODO]
	public XmlQualifiedName Import(XmlSchemaSet schemas, XmlSchemaElement element)
	{
		if (schemas == null)
		{
			throw new ArgumentNullException("schemas");
		}
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		schemas.Compile();
		XmlQualifiedName result = ImportInternal(schemas, element.QualifiedName);
		foreach (XmlQualifiedName name in schemas.GlobalTypes.Names)
		{
			ImportInternal(schemas, name);
		}
		return result;
	}

	private XmlQualifiedName ImportInternal(XmlSchemaSet schemas, XmlQualifiedName qname)
	{
		if (qname.Namespace == "http://schemas.microsoft.com/2003/10/Serialization/")
		{
			return qname;
		}
		if (imported_names.ContainsKey(qname))
		{
			return imported_names[qname];
		}
		XmlSchemas xmlSchemas = new XmlSchemas();
		foreach (XmlSchema item in schemas.Schemas())
		{
			xmlSchemas.Add(item);
		}
		XmlSchemaImporter xmlSchemaImporter = new XmlSchemaImporter(xmlSchemas);
		XmlTypeMapping mapping = xmlSchemaImporter.ImportTypeMapping(qname);
		ImportFromTypeMapping(mapping);
		return qname;
	}

	private void ImportFromTypeMapping(XmlTypeMapping mapping)
	{
		if (mapping == null)
		{
			return;
		}
		XmlQualifiedName key = new XmlQualifiedName(mapping.TypeName, mapping.Namespace);
		if (imported_names.ContainsKey(key))
		{
			return;
		}
		CodeNamespace codeNamespace = new CodeNamespace();
		codeNamespace.Name = FromXmlnsToClrName(mapping.Namespace);
		XmlCodeExporter xmlCodeExporter = new XmlCodeExporter(codeNamespace);
		xmlCodeExporter.ExportTypeMapping(mapping);
		List<CodeTypeDeclaration> list = new List<CodeTypeDeclaration>();
		foreach (CodeTypeDeclaration type in codeNamespace.Types)
		{
			string @namespace = GetNamespace(type);
			if (@namespace == null)
			{
				continue;
			}
			XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(type.Name, @namespace);
			if (imported_names.ContainsKey(xmlQualifiedName))
			{
				list.Add(type);
				continue;
			}
			if (xmlQualifiedName.Namespace == "http://schemas.microsoft.com/2003/10/Serialization/Arrays")
			{
				list.Add(type);
				continue;
			}
			imported_names[xmlQualifiedName] = xmlQualifiedName;
			type.Comments.Clear();
			type.CustomAttributes.Clear();
			type.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference("System.CodeDom.Compiler.GeneratedCodeAttribute"), new CodeAttributeArgument(new CodePrimitiveExpression("System.Runtime.Serialization")), new CodeAttributeArgument(new CodePrimitiveExpression("3.0.0.0"))));
			type.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference("System.Runtime.Serialization.DataContractAttribute")));
			if (type.IsEnum)
			{
				continue;
			}
			type.BaseTypes.Add(new CodeTypeReference(typeof(object)));
			type.BaseTypes.Add(new CodeTypeReference("System.Runtime.Serialization.IExtensibleDataObject"));
			foreach (CodeTypeMember member in type.Members)
			{
				if (member is CodeMemberProperty codeMemberProperty && (codeMemberProperty.Attributes & MemberAttributes.Public) == MemberAttributes.Public)
				{
					codeMemberProperty.CustomAttributes.Clear();
					codeMemberProperty.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference("System.Runtime.Serialization.DataMemberAttribute")));
					codeMemberProperty.Comments.Clear();
				}
			}
			CodeMemberField codeMemberField = new CodeMemberField(new CodeTypeReference("System.Runtime.Serialization.ExtensionDataObject"), "extensionDataField");
			codeMemberField.Attributes = (MemberAttributes)20482;
			type.Members.Add(codeMemberField);
			CodeMemberProperty codeMemberProperty2 = new CodeMemberProperty();
			codeMemberProperty2.Type = new CodeTypeReference("System.Runtime.Serialization.ExtensionDataObject");
			codeMemberProperty2.Name = "ExtensionData";
			codeMemberProperty2.Attributes = (MemberAttributes)24578;
			codeMemberProperty2.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "extensionDataField")));
			codeMemberProperty2.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "extensionDataField"), new CodePropertySetValueReferenceExpression()));
			type.Members.Add(codeMemberProperty2);
		}
		foreach (CodeTypeDeclaration item in list)
		{
			codeNamespace.Types.Remove(item);
		}
		if (codeNamespace.Types.Count > 0)
		{
			CodeCompileUnit.Namespaces.Add(codeNamespace);
		}
	}

	private string FromXmlnsToClrName(string xns)
	{
		Uri result;
		string text;
		if (xns.StartsWith("http://schemas.datacontract.org/2004/07/", StringComparison.Ordinal))
		{
			xns = xns.Substring("http://schemas.datacontract.org/2004/07/".Length);
		}
		else if (Uri.TryCreate(xns, UriKind.Absolute, out result) && (text = MakeStringNamespaceComponentsValid(result.GetComponents(UriComponents.Host | UriComponents.Path, UriFormat.Unescaped))).Length > 0)
		{
			xns = text;
		}
		return MakeStringNamespaceComponentsValid(xns);
	}

	private string MakeStringNamespaceComponentsValid(string ns)
	{
		string[] array = ns.Split(split_tokens, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = CodeIdentifier.MakeValid(array[i]);
		}
		return string.Join(".", array);
	}

	private string GetNamespace(CodeTypeDeclaration type)
	{
		foreach (CodeAttributeDeclaration customAttribute in type.CustomAttributes)
		{
			if (!(customAttribute.Name == "System.Xml.Serialization.XmlTypeAttribute") && !(customAttribute.Name == "System.Xml.Serialization.XmlRootAttribute"))
			{
				continue;
			}
			foreach (CodeAttributeArgument argument in customAttribute.Arguments)
			{
				if (argument.Name == "Namespace")
				{
					return ((CodePrimitiveExpression)argument.Value).Value as string;
				}
			}
			return null;
		}
		return null;
	}
}

using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeAttributeDeclaration
{
	private string name;

	private CodeAttributeArgumentCollection arguments;

	private CodeTypeReference attribute;

	public CodeAttributeArgumentCollection Arguments
	{
		get
		{
			if (arguments == null)
			{
				arguments = new CodeAttributeArgumentCollection();
			}
			return arguments;
		}
	}

	public string Name
	{
		get
		{
			if (name == null)
			{
				return string.Empty;
			}
			return name;
		}
		set
		{
			name = value;
			attribute = new CodeTypeReference(name);
		}
	}

	public CodeTypeReference AttributeType => attribute;

	public CodeAttributeDeclaration()
	{
	}

	public CodeAttributeDeclaration(string name)
	{
		Name = name;
	}

	public CodeAttributeDeclaration(string name, params CodeAttributeArgument[] arguments)
	{
		Name = name;
		Arguments.AddRange(arguments);
	}

	public CodeAttributeDeclaration(CodeTypeReference attributeType)
	{
		attribute = attributeType;
		if (attributeType != null)
		{
			name = attributeType.BaseType;
		}
	}

	public CodeAttributeDeclaration(CodeTypeReference attributeType, params CodeAttributeArgument[] arguments)
	{
		attribute = attributeType;
		if (attributeType != null)
		{
			name = attributeType.BaseType;
		}
		Arguments.AddRange(arguments);
	}
}

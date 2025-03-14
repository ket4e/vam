using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeParameterDeclarationExpression : CodeExpression
{
	private CodeAttributeDeclarationCollection customAttributes;

	private FieldDirection direction;

	private string name;

	private CodeTypeReference type;

	public CodeAttributeDeclarationCollection CustomAttributes
	{
		get
		{
			if (customAttributes == null)
			{
				customAttributes = new CodeAttributeDeclarationCollection();
			}
			return customAttributes;
		}
		set
		{
			customAttributes = value;
		}
	}

	public FieldDirection Direction
	{
		get
		{
			return direction;
		}
		set
		{
			direction = value;
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
		}
	}

	public CodeTypeReference Type
	{
		get
		{
			if (type == null)
			{
				type = new CodeTypeReference(string.Empty);
			}
			return type;
		}
		set
		{
			type = value;
		}
	}

	public CodeParameterDeclarationExpression()
	{
	}

	public CodeParameterDeclarationExpression(CodeTypeReference type, string name)
	{
		this.type = type;
		this.name = name;
	}

	public CodeParameterDeclarationExpression(string type, string name)
	{
		this.type = new CodeTypeReference(type);
		this.name = name;
	}

	public CodeParameterDeclarationExpression(Type type, string name)
	{
		this.type = new CodeTypeReference(type);
		this.name = name;
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}

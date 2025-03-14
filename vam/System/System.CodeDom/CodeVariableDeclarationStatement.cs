using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeVariableDeclarationStatement : CodeStatement
{
	private CodeExpression initExpression;

	private CodeTypeReference type;

	private string name;

	public CodeExpression InitExpression
	{
		get
		{
			return initExpression;
		}
		set
		{
			initExpression = value;
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

	public CodeVariableDeclarationStatement()
	{
	}

	public CodeVariableDeclarationStatement(CodeTypeReference type, string name)
	{
		this.type = type;
		this.name = name;
	}

	public CodeVariableDeclarationStatement(string type, string name)
	{
		this.type = new CodeTypeReference(type);
		this.name = name;
	}

	public CodeVariableDeclarationStatement(Type type, string name)
	{
		this.type = new CodeTypeReference(type);
		this.name = name;
	}

	public CodeVariableDeclarationStatement(CodeTypeReference type, string name, CodeExpression initExpression)
	{
		this.type = type;
		this.name = name;
		this.initExpression = initExpression;
	}

	public CodeVariableDeclarationStatement(string type, string name, CodeExpression initExpression)
	{
		this.type = new CodeTypeReference(type);
		this.name = name;
		this.initExpression = initExpression;
	}

	public CodeVariableDeclarationStatement(Type type, string name, CodeExpression initExpression)
	{
		this.type = new CodeTypeReference(type);
		this.name = name;
		this.initExpression = initExpression;
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}

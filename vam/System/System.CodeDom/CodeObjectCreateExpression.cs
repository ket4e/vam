using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeObjectCreateExpression : CodeExpression
{
	private CodeTypeReference createType;

	private CodeExpressionCollection parameters;

	public CodeTypeReference CreateType
	{
		get
		{
			if (createType == null)
			{
				createType = new CodeTypeReference(string.Empty);
			}
			return createType;
		}
		set
		{
			createType = value;
		}
	}

	public CodeExpressionCollection Parameters
	{
		get
		{
			if (parameters == null)
			{
				parameters = new CodeExpressionCollection();
			}
			return parameters;
		}
	}

	public CodeObjectCreateExpression()
	{
	}

	public CodeObjectCreateExpression(CodeTypeReference createType, params CodeExpression[] parameters)
	{
		this.createType = createType;
		Parameters.AddRange(parameters);
	}

	public CodeObjectCreateExpression(string createType, params CodeExpression[] parameters)
	{
		this.createType = new CodeTypeReference(createType);
		Parameters.AddRange(parameters);
	}

	public CodeObjectCreateExpression(Type createType, params CodeExpression[] parameters)
	{
		this.createType = new CodeTypeReference(createType);
		Parameters.AddRange(parameters);
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}

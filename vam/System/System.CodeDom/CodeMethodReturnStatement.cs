using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeMethodReturnStatement : CodeStatement
{
	private CodeExpression expression;

	public CodeExpression Expression
	{
		get
		{
			return expression;
		}
		set
		{
			expression = value;
		}
	}

	public CodeMethodReturnStatement()
	{
	}

	public CodeMethodReturnStatement(CodeExpression expression)
	{
		this.expression = expression;
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}

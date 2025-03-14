using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
public class CodeThrowExceptionStatement : CodeStatement
{
	private CodeExpression toThrow;

	public CodeExpression ToThrow
	{
		get
		{
			return toThrow;
		}
		set
		{
			toThrow = value;
		}
	}

	public CodeThrowExceptionStatement()
	{
	}

	public CodeThrowExceptionStatement(CodeExpression toThrow)
	{
		this.toThrow = toThrow;
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}

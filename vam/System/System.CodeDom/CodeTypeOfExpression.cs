using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeTypeOfExpression : CodeExpression
{
	private CodeTypeReference type;

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

	public CodeTypeOfExpression()
	{
	}

	public CodeTypeOfExpression(CodeTypeReference type)
	{
		this.type = type;
	}

	public CodeTypeOfExpression(string type)
	{
		this.type = new CodeTypeReference(type);
	}

	public CodeTypeOfExpression(Type type)
	{
		this.type = new CodeTypeReference(type);
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}

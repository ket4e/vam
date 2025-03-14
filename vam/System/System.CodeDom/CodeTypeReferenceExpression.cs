using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeTypeReferenceExpression : CodeExpression
{
	private CodeTypeReference type;

	public CodeTypeReference Type
	{
		get
		{
			if (type == null)
			{
				return new CodeTypeReference(string.Empty);
			}
			return type;
		}
		set
		{
			type = value;
		}
	}

	public CodeTypeReferenceExpression()
	{
	}

	public CodeTypeReferenceExpression(CodeTypeReference type)
	{
		this.type = type;
	}

	public CodeTypeReferenceExpression(string type)
	{
		this.type = new CodeTypeReference(type);
	}

	public CodeTypeReferenceExpression(Type type)
	{
		this.type = new CodeTypeReference(type);
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}

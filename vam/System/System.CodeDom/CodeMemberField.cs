using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
public class CodeMemberField : CodeTypeMember
{
	private CodeExpression initExpression;

	private CodeTypeReference type;

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

	public CodeMemberField()
	{
	}

	public CodeMemberField(CodeTypeReference type, string name)
	{
		this.type = type;
		base.Name = name;
	}

	public CodeMemberField(string type, string name)
	{
		this.type = new CodeTypeReference(type);
		base.Name = name;
	}

	public CodeMemberField(Type type, string name)
	{
		this.type = new CodeTypeReference(type);
		base.Name = name;
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}

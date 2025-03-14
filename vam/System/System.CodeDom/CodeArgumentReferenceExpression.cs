using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
public class CodeArgumentReferenceExpression : CodeExpression
{
	private string parameterName;

	public string ParameterName
	{
		get
		{
			if (parameterName == null)
			{
				return string.Empty;
			}
			return parameterName;
		}
		set
		{
			parameterName = value;
		}
	}

	public CodeArgumentReferenceExpression()
	{
	}

	public CodeArgumentReferenceExpression(string name)
	{
		parameterName = name;
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}

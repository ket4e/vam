using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
public class CodePropertyReferenceExpression : CodeExpression
{
	private CodeExpression targetObject;

	private string propertyName;

	public string PropertyName
	{
		get
		{
			if (propertyName == null)
			{
				return string.Empty;
			}
			return propertyName;
		}
		set
		{
			propertyName = value;
		}
	}

	public CodeExpression TargetObject
	{
		get
		{
			return targetObject;
		}
		set
		{
			targetObject = value;
		}
	}

	public CodePropertyReferenceExpression()
	{
	}

	public CodePropertyReferenceExpression(CodeExpression targetObject, string propertyName)
	{
		this.targetObject = targetObject;
		this.propertyName = propertyName;
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}

using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeFieldReferenceExpression : CodeExpression
{
	private CodeExpression targetObject;

	private string fieldName;

	public string FieldName
	{
		get
		{
			return fieldName;
		}
		set
		{
			fieldName = value;
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

	public CodeFieldReferenceExpression()
	{
		fieldName = string.Empty;
	}

	public CodeFieldReferenceExpression(CodeExpression targetObject, string fieldName)
	{
		this.targetObject = targetObject;
		this.fieldName = fieldName;
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}

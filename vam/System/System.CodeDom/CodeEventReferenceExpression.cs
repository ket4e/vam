using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeEventReferenceExpression : CodeExpression
{
	private string eventName;

	private CodeExpression targetObject;

	public string EventName
	{
		get
		{
			if (eventName == null)
			{
				return string.Empty;
			}
			return eventName;
		}
		set
		{
			eventName = value;
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

	public CodeEventReferenceExpression()
	{
	}

	public CodeEventReferenceExpression(CodeExpression targetObject, string eventName)
	{
		this.targetObject = targetObject;
		this.eventName = eventName;
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}

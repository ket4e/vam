using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeAttachEventStatement : CodeStatement
{
	private CodeEventReferenceExpression eventRef;

	private CodeExpression listener;

	public CodeEventReferenceExpression Event
	{
		get
		{
			if (eventRef == null)
			{
				eventRef = new CodeEventReferenceExpression();
			}
			return eventRef;
		}
		set
		{
			eventRef = value;
		}
	}

	public CodeExpression Listener
	{
		get
		{
			return listener;
		}
		set
		{
			listener = value;
		}
	}

	public CodeAttachEventStatement()
	{
	}

	public CodeAttachEventStatement(CodeEventReferenceExpression eventRef, CodeExpression listener)
	{
		this.eventRef = eventRef;
		this.listener = listener;
	}

	public CodeAttachEventStatement(CodeExpression targetObject, string eventName, CodeExpression listener)
	{
		eventRef = new CodeEventReferenceExpression(targetObject, eventName);
		this.listener = listener;
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}

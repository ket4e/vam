using System;

namespace Mono.WebBrowser.DOM;

public interface INode
{
	IAttributeCollection Attributes { get; }

	INodeList ChildNodes { get; }

	INode FirstChild { get; }

	INode LastChild { get; }

	string LocalName { get; }

	INode Next { get; }

	IDocument Owner { get; }

	INode Parent { get; }

	INode Previous { get; }

	NodeType Type { get; }

	string Value { get; set; }

	IntPtr AccessibleObject { get; }

	event NodeEventHandler Click;

	event NodeEventHandler DoubleClick;

	event NodeEventHandler KeyDown;

	event NodeEventHandler KeyPress;

	event NodeEventHandler KeyUp;

	event NodeEventHandler MouseDown;

	event NodeEventHandler MouseEnter;

	event NodeEventHandler MouseLeave;

	event NodeEventHandler MouseMove;

	event NodeEventHandler MouseOver;

	event NodeEventHandler MouseUp;

	event NodeEventHandler OnFocus;

	event NodeEventHandler OnBlur;

	INode InsertBefore(INode newChild, INode refChild);

	INode ReplaceChild(INode newChild, INode oldChild);

	INode RemoveChild(INode child);

	INode AppendChild(INode child);

	void FireEvent(string eventName);

	new int GetHashCode();

	new bool Equals(object obj);

	void AttachEventHandler(string eventName, EventHandler handler);

	void DetachEventHandler(string eventName, EventHandler handler);

	void AttachEventHandler(string eventName, Delegate handler);

	void DetachEventHandler(string eventName, Delegate handler);
}

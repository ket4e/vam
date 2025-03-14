using System.Runtime.InteropServices;

namespace System.ComponentModel.Design;

[ComVisible(true)]
public sealed class ComponentChangedEventArgs : EventArgs
{
	private object component;

	private MemberDescriptor member;

	private object oldValue;

	private object newValue;

	public object Component => component;

	public MemberDescriptor Member => member;

	public object NewValue => oldValue;

	public object OldValue => newValue;

	public ComponentChangedEventArgs(object component, MemberDescriptor member, object oldValue, object newValue)
	{
		this.component = component;
		this.member = member;
		this.oldValue = oldValue;
		this.newValue = newValue;
	}
}

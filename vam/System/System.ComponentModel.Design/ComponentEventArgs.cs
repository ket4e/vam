using System.Runtime.InteropServices;

namespace System.ComponentModel.Design;

[ComVisible(true)]
public class ComponentEventArgs : EventArgs
{
	private IComponent icomp;

	public virtual IComponent Component => icomp;

	public ComponentEventArgs(IComponent component)
	{
		icomp = component;
	}
}

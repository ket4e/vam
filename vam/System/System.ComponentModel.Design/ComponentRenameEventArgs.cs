using System.Runtime.InteropServices;

namespace System.ComponentModel.Design;

[ComVisible(true)]
public class ComponentRenameEventArgs : EventArgs
{
	private object component;

	private string oldName;

	private string newName;

	public object Component => component;

	public virtual string NewName => newName;

	public virtual string OldName => oldName;

	public ComponentRenameEventArgs(object component, string oldName, string newName)
	{
		this.component = component;
		this.oldName = oldName;
		this.newName = newName;
	}
}

using System.ComponentModel;

namespace System.Windows.Forms;

public sealed class LayoutEventArgs : EventArgs
{
	private Control affected_control;

	private string affected_property;

	private IComponent affected_component;

	public IComponent AffectedComponent => affected_component;

	public Control AffectedControl => affected_control;

	public string AffectedProperty => affected_property;

	public LayoutEventArgs(Control affectedControl, string affectedProperty)
	{
		affected_control = affectedControl;
		affected_property = affectedProperty;
	}

	public LayoutEventArgs(IComponent affectedComponent, string affectedProperty)
	{
		affected_component = affectedComponent;
		affected_property = affectedProperty;
	}
}

namespace System.Windows.Forms.Layout;

public abstract class LayoutEngine
{
	public virtual void InitLayout(object child, BoundsSpecified specified)
	{
	}

	public virtual bool Layout(object container, LayoutEventArgs layoutEventArgs)
	{
		return false;
	}
}

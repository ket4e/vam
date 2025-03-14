using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
[DefaultEvent("Opening")]
public class ContextMenuStrip : ToolStripDropDownMenu
{
	private Control source_control;

	internal Control container;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public Control SourceControl => source_control;

	public ContextMenuStrip()
	{
		source_control = null;
	}

	public ContextMenuStrip(IContainer container)
	{
		source_control = null;
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	protected override void SetVisibleCore(bool visible)
	{
		base.SetVisibleCore(visible);
		if (visible)
		{
			XplatUI.SetTopmost(Handle, Enabled: true);
		}
	}

	internal void SetSourceControl(Control source_control)
	{
		container = (this.source_control = source_control);
	}
}

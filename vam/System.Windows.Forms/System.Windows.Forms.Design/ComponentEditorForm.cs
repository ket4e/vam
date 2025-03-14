using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.Design;

[ToolboxItem(false)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
public class ComponentEditorForm : Form
{
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new virtual bool AutoSize
	{
		get
		{
			return base.AutoSize;
		}
		set
		{
			base.AutoSize = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler AutoSizeChanged
	{
		add
		{
			base.AutoSizeChanged += value;
		}
		remove
		{
			base.AutoSizeChanged -= value;
		}
	}

	[System.MonoTODO]
	public ComponentEditorForm(object component, Type[] pageTypes)
	{
	}

	[System.MonoTODO]
	protected override void OnActivated(EventArgs e)
	{
	}

	[System.MonoTODO]
	protected virtual void OnSelChangeSelector(object source, TreeViewEventArgs e)
	{
	}

	[System.MonoTODO]
	public override bool PreProcessMessage(ref Message msg)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public virtual DialogResult ShowForm()
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public virtual DialogResult ShowForm(int page)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public virtual DialogResult ShowForm(IWin32Window owner)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public virtual DialogResult ShowForm(IWin32Window owner, int page)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	protected override void OnHelpRequested(HelpEventArgs e)
	{
	}
}

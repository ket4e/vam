using System.ComponentModel;

namespace System.Windows.Forms.Design;

public abstract class WindowsFormsComponentEditor : ComponentEditor
{
	public override bool EditComponent(ITypeDescriptorContext context, object component)
	{
		return EditComponent(context, component, null);
	}

	public virtual bool EditComponent(ITypeDescriptorContext context, object component, IWin32Window owner)
	{
		ComponentEditorForm componentEditorForm = new ComponentEditorForm(component, GetComponentEditorPages());
		if (componentEditorForm.ShowForm(owner, GetInitialComponentEditorPageIndex()) == DialogResult.OK)
		{
			return true;
		}
		return false;
	}

	public bool EditComponent(object component, IWin32Window owner)
	{
		return EditComponent(null, component, owner);
	}

	protected virtual Type[] GetComponentEditorPages()
	{
		return null;
	}

	protected virtual int GetInitialComponentEditorPageIndex()
	{
		return 0;
	}
}

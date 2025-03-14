using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.Design;

[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public abstract class ComponentEditorPage : Panel
{
	private bool commitOnDeactivate;

	private IComponent component;

	private bool firstActivate = true;

	private Icon icon;

	private int loading;

	private bool loadRequired;

	private IComponentEditorPageSite pageSite;

	[EditorBrowsable(EditorBrowsableState.Never)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
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

	public bool CommitOnDeactivate
	{
		get
		{
			return commitOnDeactivate;
		}
		set
		{
			commitOnDeactivate = value;
		}
	}

	protected IComponent Component
	{
		get
		{
			return component;
		}
		set
		{
			component = value;
		}
	}

	[System.MonoTODO("Find out what this does.")]
	protected override CreateParams CreateParams
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	protected bool FirstActivate
	{
		get
		{
			return firstActivate;
		}
		set
		{
			firstActivate = value;
		}
	}

	public Icon Icon
	{
		get
		{
			return icon;
		}
		set
		{
			icon = value;
		}
	}

	protected int Loading
	{
		get
		{
			return loading;
		}
		set
		{
			loading = value;
		}
	}

	protected bool LoadRequired
	{
		get
		{
			return loadRequired;
		}
		set
		{
			loadRequired = value;
		}
	}

	protected IComponentEditorPageSite PageSite
	{
		get
		{
			return pageSite;
		}
		set
		{
			pageSite = value;
		}
	}

	public virtual string Title => base.Text;

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

	public ComponentEditorPage()
	{
	}

	public virtual void Activate()
	{
		base.Visible = true;
		firstActivate = false;
		if (loadRequired)
		{
			EnterLoadingMode();
			LoadComponent();
			ExitLoadingMode();
		}
	}

	public virtual void ApplyChanges()
	{
		SaveComponent();
	}

	public virtual void Deactivate()
	{
		base.Visible = false;
	}

	protected void EnterLoadingMode()
	{
		loading++;
	}

	protected void ExitLoadingMode()
	{
		loading--;
	}

	public virtual Control GetControl()
	{
		return this;
	}

	protected IComponent GetSelectedComponent()
	{
		return component;
	}

	protected bool IsFirstActivate()
	{
		return firstActivate;
	}

	protected bool IsLoading()
	{
		return loading != 0;
	}

	public virtual bool IsPageMessage(ref Message msg)
	{
		return PreProcessMessage(ref msg);
	}

	protected abstract void LoadComponent();

	[System.MonoTODO("Find out what this does.")]
	public virtual void OnApplyComplete()
	{
	}

	protected virtual void ReloadComponent()
	{
		loadRequired = true;
	}

	protected abstract void SaveComponent();

	public virtual void SetComponent(IComponent component)
	{
		this.component = component;
		ReloadComponent();
	}

	[System.MonoTODO("Find out what this does.")]
	protected virtual void SetDirty()
	{
	}

	public virtual void SetSite(IComponentEditorPageSite site)
	{
		pageSite = site;
		pageSite.GetControl().Controls.Add(this);
	}

	public virtual void ShowHelp()
	{
	}

	public virtual bool SupportsHelp()
	{
		return false;
	}
}

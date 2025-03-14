using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ClassInterface(ClassInterfaceType.AutoDispatch)]
[DesignTimeVisible(false)]
[ToolboxItem(false)]
[ComVisible(true)]
public sealed class MdiClient : Control
{
	[ComVisible(false)]
	public new class ControlCollection : Control.ControlCollection
	{
		private MdiClient owner;

		public ControlCollection(MdiClient owner)
			: base(owner)
		{
			this.owner = owner;
		}

		public override void Add(Control value)
		{
			if (!(value is Form) || !((Form)value).IsMdiChild)
			{
				throw new ArgumentException("Form must be MdiChild");
			}
			owner.mdi_child_list.Add(value);
			base.Add(value);
			Form activeMdiChild = (Form)value;
			owner.ActiveMdiChild = activeMdiChild;
		}

		public override void Remove(Control value)
		{
			if (value is Form form && form.WindowManager is MdiWindowManager mdiWindowManager)
			{
				form.Closed -= mdiWindowManager.form_closed_handler;
			}
			owner.mdi_child_list.Remove(value);
			base.Remove(value);
		}
	}

	private int mdi_created;

	private ImplicitHScrollBar hbar;

	private ImplicitVScrollBar vbar;

	private SizeGrip sizegrip;

	private int hbar_value;

	private int vbar_value;

	private bool lock_sizing;

	private bool initializing_scrollbars;

	private int prev_bottom;

	private bool setting_windowstates;

	internal ArrayList mdi_child_list;

	private string form_text;

	private bool setting_form_text;

	private Form active_child;

	internal bool HorizontalScrollbarVisible => hbar != null && hbar.Visible;

	internal bool VerticalScrollbarVisible => vbar != null && vbar.Visible;

	internal Form ParentForm => (Form)base.Parent;

	[Localizable(true)]
	public override Image BackgroundImage
	{
		get
		{
			return base.BackgroundImage;
		}
		set
		{
			base.BackgroundImage = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public override ImageLayout BackgroundImageLayout
	{
		get
		{
			return base.BackgroundImageLayout;
		}
		set
		{
			base.BackgroundImageLayout = value;
		}
	}

	public Form[] MdiChildren
	{
		get
		{
			if (mdi_child_list == null)
			{
				return new Form[0];
			}
			return (Form[])mdi_child_list.ToArray(typeof(Form));
		}
	}

	protected override CreateParams CreateParams
	{
		get
		{
			CreateParams createParams = base.CreateParams;
			createParams.ExStyle |= 512;
			return createParams;
		}
	}

	internal int ChildrenCreated
	{
		get
		{
			return mdi_created;
		}
		set
		{
			mdi_created = value;
		}
	}

	internal Form ActiveMdiChild
	{
		get
		{
			if (ParentForm != null && !ParentForm.Visible)
			{
				return null;
			}
			if (base.Controls.Count < 1)
			{
				return null;
			}
			if (!ParentForm.IsHandleCreated)
			{
				return null;
			}
			if (!ParentForm.has_been_visible)
			{
				return null;
			}
			if (!ParentForm.Visible)
			{
				return active_child;
			}
			active_child = null;
			for (int i = 0; i < base.Controls.Count; i++)
			{
				if (base.Controls[i].Visible)
				{
					active_child = (Form)base.Controls[i];
					break;
				}
			}
			return active_child;
		}
		set
		{
			ActivateChild(value);
		}
	}

	public MdiClient()
	{
		mdi_child_list = new ArrayList();
		BackColor = SystemColors.AppWorkspace;
		Dock = DockStyle.Fill;
		SetStyle(ControlStyles.Selectable, value: false);
	}

	internal void SendFocusToActiveChild()
	{
		Form activeMdiChild = ActiveMdiChild;
		if (activeMdiChild == null)
		{
			ParentForm.SendControlFocus(this);
			return;
		}
		activeMdiChild.SendControlFocus(activeMdiChild);
		ParentForm.ActiveControl = activeMdiChild;
	}

	internal void SetParentText(bool text_changed)
	{
		if (setting_form_text)
		{
			return;
		}
		setting_form_text = true;
		if (text_changed)
		{
			form_text = ParentForm.Text;
		}
		if (ParentForm.ActiveMaximizedMdiChild == null)
		{
			ParentForm.Text = form_text;
		}
		else
		{
			string text = ParentForm.ActiveMaximizedMdiChild.form.Text;
			if (text.Length > 0)
			{
				ParentForm.Text = form_text + " - [" + ParentForm.ActiveMaximizedMdiChild.form.Text + "]";
			}
			else
			{
				ParentForm.Text = form_text;
			}
		}
		setting_form_text = false;
	}

	internal override void OnPaintBackgroundInternal(PaintEventArgs pe)
	{
		if (BackgroundImage == null && base.Parent != null && base.Parent.BackgroundImage != null)
		{
			base.Parent.PaintControlBackground(pe);
		}
	}

	protected override Control.ControlCollection CreateControlsInstance()
	{
		return new ControlCollection(this);
	}

	protected override void WndProc(ref Message m)
	{
		Msg msg = (Msg)m.Msg;
		if (msg == Msg.WM_NCPAINT)
		{
			PaintEventArgs paintEventArgs = XplatUI.PaintEventStart(ref m, Handle, client: false);
			ControlPaint.DrawBorder3D(rectangle: new Rectangle(0, 0, base.Width, base.Height), graphics: paintEventArgs.Graphics, style: Border3DStyle.Sunken);
			XplatUI.PaintEventEnd(ref m, Handle, client: false);
			m.Result = IntPtr.Zero;
		}
		else
		{
			base.WndProc(ref m);
		}
	}

	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);
		if (base.Parent != null && base.Parent.IsHandleCreated)
		{
			XplatUI.InvalidateNC(base.Parent.Handle);
		}
		SizeScrollBars();
		ArrangeWindows();
	}

	protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
	{
		specified &= ~BoundsSpecified.Location;
		base.ScaleControl(factor, specified);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void ScaleCore(float dx, float dy)
	{
		base.ScaleCore(dx, dy);
	}

	protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
	{
		base.SetBoundsCore(x, y, width, height, specified);
	}

	public void LayoutMdi(MdiLayout value)
	{
		ArrangeIconicWindows(rearrange_all: true);
		switch (value)
		{
		case MdiLayout.Cascade:
		{
			int num3 = 0;
			for (int num4 = base.Controls.Count - 1; num4 >= 0; num4--)
			{
				Form form3 = (Form)base.Controls[num4];
				if (form3.WindowState != FormWindowState.Minimized)
				{
					if (form3.WindowState == FormWindowState.Maximized)
					{
						form3.WindowState = FormWindowState.Normal;
					}
					form3.Width = Convert.ToInt32((double)base.ClientSize.Width * 0.8);
					form3.Height = Math.Max(Convert.ToInt32((double)base.ClientSize.Height * 0.8), SystemInformation.MinimumWindowSize.Height + 2);
					int num5 = 22 * num3;
					int num6 = 22 * num3;
					if (num3 != 0 && (num5 + form3.Width > base.ClientSize.Width || num6 + form3.Height > base.ClientSize.Height))
					{
						num3 = 0;
						num5 = 22 * num3;
						num6 = 22 * num3;
					}
					form3.Left = num5;
					form3.Top = num6;
					num3++;
				}
			}
			break;
		}
		case MdiLayout.TileHorizontal:
		case MdiLayout.TileVertical:
		{
			int num = 0;
			int num2 = base.ClientSize.Height;
			for (int i = 0; i < base.Controls.Count; i++)
			{
				if (!(base.Controls[i] is Form form) || !form.Visible)
				{
					continue;
				}
				if (form.WindowState == FormWindowState.Maximized)
				{
					form.WindowState = FormWindowState.Normal;
				}
				else if (form.WindowState == FormWindowState.Minimized)
				{
					if (form.Bounds.Top < num2)
					{
						num2 = form.Bounds.Top;
					}
					continue;
				}
				num++;
			}
			if (num <= 0)
			{
				break;
			}
			Size size;
			Size size2;
			if (value == MdiLayout.TileHorizontal)
			{
				size = new Size(base.ClientSize.Width, num2 / num);
				size2 = new Size(0, size.Height);
			}
			else
			{
				size = new Size(base.ClientSize.Width / num, num2);
				size2 = new Size(size.Width, 0);
			}
			Point empty = Point.Empty;
			for (int j = 0; j < base.Controls.Count; j++)
			{
				if (base.Controls[j] is Form form2 && form2.Visible && form2.WindowState != FormWindowState.Minimized)
				{
					form2.Size = size;
					form2.Location = empty;
					empty += size2;
				}
			}
			break;
		}
		}
	}

	internal void SizeScrollBars()
	{
		if (lock_sizing || !base.IsHandleCreated)
		{
			return;
		}
		if (base.Controls.Count == 0 || ((Form)base.Controls[0]).WindowState == FormWindowState.Maximized)
		{
			if (hbar != null)
			{
				hbar.Visible = false;
			}
			if (vbar != null)
			{
				vbar.Visible = false;
			}
			if (sizegrip != null)
			{
				sizegrip.Visible = false;
			}
			return;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		foreach (Form control in base.Controls)
		{
			if (control.Visible)
			{
				if (control.Right > num)
				{
					num = control.Right;
				}
				if (control.Left < num2)
				{
					num2 = control.Left;
				}
				if (control.Bottom > num4)
				{
					num4 = control.Bottom;
				}
				if (control.Top < 0)
				{
					num3 = control.Top;
				}
			}
		}
		int width = base.ClientSize.Width;
		int num5 = base.ClientSize.Height;
		bool flag = false;
		bool flag2 = false;
		if (num - num2 > width || num2 < 0)
		{
			flag = true;
			num5 -= SystemInformation.HorizontalScrollBarHeight;
		}
		if (num4 - num3 > num5 || num3 < 0)
		{
			flag2 = true;
			width -= SystemInformation.VerticalScrollBarWidth;
			if (!flag && (num - num2 > width || num2 < 0))
			{
				flag = true;
				num5 -= SystemInformation.HorizontalScrollBarHeight;
			}
		}
		if (flag)
		{
			if (hbar == null)
			{
				hbar = new ImplicitHScrollBar();
				base.Controls.AddImplicit(hbar);
			}
			hbar.Visible = true;
			CalcHBar(num2, num, flag2);
		}
		else if (hbar != null)
		{
			hbar.Visible = false;
		}
		if (flag2)
		{
			if (vbar == null)
			{
				vbar = new ImplicitVScrollBar();
				base.Controls.AddImplicit(vbar);
			}
			vbar.Visible = true;
			CalcVBar(num3, num4, flag);
		}
		else if (vbar != null)
		{
			vbar.Visible = false;
		}
		if (flag && flag2)
		{
			if (sizegrip == null)
			{
				sizegrip = new SizeGrip(ParentForm);
				base.Controls.AddImplicit(sizegrip);
			}
			sizegrip.Location = new Point(hbar.Right, vbar.Bottom);
			sizegrip.Visible = true;
			XplatUI.SetZOrder(sizegrip.Handle, vbar.Handle, Top: false, Bottom: false);
		}
		else if (sizegrip != null)
		{
			sizegrip.Visible = false;
		}
		XplatUI.InvalidateNC(Handle);
	}

	private void CalcHBar(int left, int right, bool vert_vis)
	{
		initializing_scrollbars = true;
		hbar.Left = 0;
		hbar.Top = base.ClientRectangle.Bottom - hbar.Height;
		hbar.Width = base.ClientRectangle.Width - (vert_vis ? SystemInformation.VerticalScrollBarWidth : 0);
		hbar.LargeChange = 50;
		hbar.Minimum = Math.Min(left, 0);
		hbar.Maximum = Math.Max(right - base.ClientSize.Width + 51 + (vert_vis ? SystemInformation.VerticalScrollBarWidth : 0), 0);
		hbar.Value = 0;
		hbar_value = 0;
		hbar.ValueChanged += HBarValueChanged;
		XplatUI.SetZOrder(hbar.Handle, IntPtr.Zero, Top: true, Bottom: false);
		initializing_scrollbars = false;
	}

	private void CalcVBar(int top, int bottom, bool horz_vis)
	{
		initializing_scrollbars = true;
		vbar.Top = 0;
		vbar.Left = base.ClientRectangle.Right - vbar.Width;
		vbar.Height = base.ClientRectangle.Height - (horz_vis ? SystemInformation.HorizontalScrollBarHeight : 0);
		vbar.LargeChange = 50;
		vbar.Minimum = Math.Min(top, 0);
		vbar.Maximum = Math.Max(bottom - base.ClientSize.Height + 51 + (horz_vis ? SystemInformation.HorizontalScrollBarHeight : 0), 0);
		vbar.Value = 0;
		vbar_value = 0;
		vbar.ValueChanged += VBarValueChanged;
		XplatUI.SetZOrder(vbar.Handle, IntPtr.Zero, Top: true, Bottom: false);
		initializing_scrollbars = false;
	}

	private void HBarValueChanged(object sender, EventArgs e)
	{
		if (initializing_scrollbars || hbar.Value == hbar_value)
		{
			return;
		}
		lock_sizing = true;
		try
		{
			int num = hbar_value - hbar.Value;
			foreach (Form control in base.Controls)
			{
				control.Left += num;
			}
		}
		finally
		{
			lock_sizing = false;
		}
		hbar_value = hbar.Value;
	}

	private void VBarValueChanged(object sender, EventArgs e)
	{
		if (initializing_scrollbars || vbar.Value == vbar_value)
		{
			return;
		}
		lock_sizing = true;
		try
		{
			int num = vbar_value - vbar.Value;
			foreach (Form control in base.Controls)
			{
				control.Top += num;
			}
		}
		finally
		{
			lock_sizing = false;
		}
		vbar_value = vbar.Value;
	}

	private void ArrangeWindows()
	{
		if (!base.IsHandleCreated)
		{
			return;
		}
		int num = 0;
		if (prev_bottom != -1)
		{
			num = base.Bottom - prev_bottom;
		}
		foreach (Control control in base.Controls)
		{
			Form form = control as Form;
			if (control != null && form.Visible)
			{
				MdiWindowManager mdiWindowManager = form.WindowManager as MdiWindowManager;
				if (mdiWindowManager.GetWindowState() == FormWindowState.Maximized)
				{
					form.Bounds = mdiWindowManager.MaximizedBounds;
				}
				if (mdiWindowManager.GetWindowState() == FormWindowState.Minimized)
				{
					form.Top += num;
				}
			}
		}
		prev_bottom = base.Bottom;
	}

	internal void ArrangeIconicWindows(bool rearrange_all)
	{
		Rectangle empty = Rectangle.Empty;
		lock_sizing = true;
		foreach (Form control in base.Controls)
		{
			if (control.WindowState != FormWindowState.Minimized)
			{
				continue;
			}
			MdiWindowManager mdiWindowManager = (MdiWindowManager)control.WindowManager;
			if (mdiWindowManager.IconicBounds != Rectangle.Empty && !rearrange_all)
			{
				if (control.Bounds != mdiWindowManager.IconicBounds)
				{
					control.Bounds = mdiWindowManager.IconicBounds;
				}
				continue;
			}
			bool flag = true;
			empty.Size = mdiWindowManager.IconicSize;
			int num = 0;
			int num2 = base.ClientSize.Height - empty.Height;
			int num3 = num;
			int num4 = num2;
			do
			{
				empty.X = num3;
				empty.Y = num4;
				flag = true;
				foreach (Form control2 in base.Controls)
				{
					if (control2 == control || control2.window_state != FormWindowState.Minimized || !control2.Bounds.IntersectsWith(empty))
					{
						continue;
					}
					flag = false;
					break;
				}
				if (!flag)
				{
					num3 += empty.Width;
					if (num3 + empty.Width > base.Right)
					{
						num3 = num;
						num4 -= empty.Height;
					}
				}
			}
			while (!flag);
			mdiWindowManager.IconicBounds = empty;
			control.Bounds = mdiWindowManager.IconicBounds;
		}
		lock_sizing = false;
	}

	internal void ChildFormClosed(Form form)
	{
		FormWindowState windowState = form.WindowState;
		form.Visible = false;
		base.Controls.Remove(form);
		if (base.Controls.Count == 0)
		{
			((MdiWindowManager)form.window_manager).RaiseDeactivate();
		}
		else if (windowState == FormWindowState.Maximized)
		{
			Form form2 = (Form)base.Controls[0];
			form2.WindowState = FormWindowState.Maximized;
			ActivateChild(form2);
		}
		if (base.Controls.Count == 0)
		{
			XplatUI.RequestNCRecalc(base.Parent.Handle);
			ParentForm.PerformLayout();
			MenuStrip mainMenuStrip = form.MdiParent.MainMenuStrip;
			if (mainMenuStrip != null && mainMenuStrip.IsCurrentlyMerged)
			{
				ToolStripManager.RevertMerge(mainMenuStrip);
			}
		}
		SizeScrollBars();
		SetParentText(text_changed: false);
		form.Dispose();
	}

	internal void ActivateNextChild()
	{
		if (base.Controls.Count >= 1 && (base.Controls.Count != 1 || base.Controls[0] != ActiveMdiChild))
		{
			Form form = (Form)base.Controls[0];
			Form form2 = (Form)base.Controls[1];
			ActivateChild(form2);
			form.SendToBack();
		}
	}

	internal void ActivatePreviousChild()
	{
		if (base.Controls.Count > 1)
		{
			Form form = (Form)base.Controls[base.Controls.Count - 1];
			ActivateChild(form);
		}
	}

	internal void ActivateChild(Form form)
	{
		if (base.Controls.Count < 1 || ParentForm.is_changing_visible_state > 0)
		{
			return;
		}
		Form form2 = (Form)base.Controls[0];
		bool flag = ParentForm.ActiveControl == form2;
		MdiWindowManager mdiWindowManager = (MdiWindowManager)form.WindowManager;
		if (form2.WindowState == FormWindowState.Maximized && form.WindowState != FormWindowState.Maximized && form.Visible)
		{
			FormWindowState window_state = form.window_state;
			SetWindowState(form, window_state, FormWindowState.Maximized, is_activating_child: true);
			mdiWindowManager.was_minimized = form.window_state == FormWindowState.Minimized;
			form.window_state = FormWindowState.Maximized;
			SetParentText(text_changed: false);
		}
		form.BringToFront();
		form.SendControlFocus(form);
		SetWindowStates(mdiWindowManager);
		if (form2 != form)
		{
			form.has_focus = false;
			if (form2.IsHandleCreated)
			{
				XplatUI.InvalidateNC(form2.Handle);
			}
			if (form.IsHandleCreated)
			{
				XplatUI.InvalidateNC(form.Handle);
			}
			if (flag)
			{
				MdiWindowManager mdiWindowManager2 = (MdiWindowManager)form2.window_manager;
				mdiWindowManager2.RaiseDeactivate();
			}
		}
		active_child = (Form)base.Controls[0];
		if (active_child.Visible)
		{
			bool flag2 = ParentForm.ActiveControl != active_child;
			ParentForm.ActiveControl = active_child;
			if (flag2)
			{
				MdiWindowManager mdiWindowManager3 = (MdiWindowManager)active_child.window_manager;
				mdiWindowManager3.RaiseActivated();
			}
		}
	}

	internal override IntPtr AfterTopMostControl()
	{
		if (hbar != null && hbar.Visible)
		{
			return hbar.Handle;
		}
		if (vbar != null && vbar.Visible)
		{
			return vbar.Handle;
		}
		return base.AfterTopMostControl();
	}

	internal bool SetWindowStates(MdiWindowManager wm)
	{
		Form form = wm.form;
		if (setting_windowstates)
		{
			return false;
		}
		if (!form.Visible)
		{
			return false;
		}
		bool isActive = wm.IsActive;
		bool flag = false;
		if (!isActive)
		{
			return false;
		}
		ArrayList arrayList = new ArrayList();
		ArrayList arrayList2 = new ArrayList();
		setting_windowstates = true;
		foreach (Form item in mdi_child_list)
		{
			if (item != form && item.Visible && item.WindowState == FormWindowState.Maximized && isActive)
			{
				flag = true;
				if (((MdiWindowManager)item.window_manager).was_minimized)
				{
					arrayList.Add(item);
				}
				else
				{
					arrayList2.Add(item);
				}
			}
		}
		if (flag && form.WindowState != FormWindowState.Maximized)
		{
			wm.was_minimized = form.window_state == FormWindowState.Minimized;
			form.WindowState = FormWindowState.Maximized;
		}
		foreach (Form item2 in arrayList)
		{
			item2.WindowState = FormWindowState.Minimized;
		}
		foreach (Form item3 in arrayList2)
		{
			item3.WindowState = FormWindowState.Normal;
		}
		SetParentText(text_changed: false);
		XplatUI.RequestNCRecalc(ParentForm.Handle);
		XplatUI.RequestNCRecalc(Handle);
		SizeScrollBars();
		setting_windowstates = false;
		if (form.MdiParent.MainMenuStrip != null)
		{
			form.MdiParent.MainMenuStrip.RefreshMdiItems();
		}
		MenuStrip mainMenuStrip = form.MdiParent.MainMenuStrip;
		if (mainMenuStrip != null)
		{
			if (mainMenuStrip.IsCurrentlyMerged)
			{
				ToolStripManager.RevertMerge(mainMenuStrip);
			}
			MenuStrip menuStrip = LookForChildMenu(form);
			if (form.WindowState != FormWindowState.Maximized)
			{
				RemoveControlMenuItems(wm);
			}
			if (form.WindowState == FormWindowState.Maximized)
			{
				bool flag2 = false;
				foreach (ToolStripItem item4 in mainMenuStrip.Items)
				{
					if (item4 is MdiControlStrip.SystemMenuItem)
					{
						(item4 as MdiControlStrip.SystemMenuItem).MdiForm = form;
						flag2 = true;
					}
					else if (item4 is MdiControlStrip.ControlBoxMenuItem)
					{
						(item4 as MdiControlStrip.ControlBoxMenuItem).MdiForm = form;
						flag2 = true;
					}
				}
				if (!flag2)
				{
					mainMenuStrip.SuspendLayout();
					mainMenuStrip.Items.Insert(0, new MdiControlStrip.SystemMenuItem(form));
					mainMenuStrip.Items.Add(new MdiControlStrip.ControlBoxMenuItem(form, MdiControlStrip.ControlBoxType.Close));
					mainMenuStrip.Items.Add(new MdiControlStrip.ControlBoxMenuItem(form, MdiControlStrip.ControlBoxType.Max));
					mainMenuStrip.Items.Add(new MdiControlStrip.ControlBoxMenuItem(form, MdiControlStrip.ControlBoxType.Min));
					mainMenuStrip.ResumeLayout();
				}
			}
			if (menuStrip != null)
			{
				ToolStripManager.Merge(menuStrip, mainMenuStrip);
			}
		}
		return flag;
	}

	private MenuStrip LookForChildMenu(Control parent)
	{
		foreach (Control control in parent.Controls)
		{
			if (control is MenuStrip)
			{
				return (MenuStrip)control;
			}
			if (control is ToolStripContainer || control is ToolStripPanel)
			{
				MenuStrip menuStrip = LookForChildMenu(control);
				if (menuStrip != null)
				{
					return menuStrip;
				}
			}
		}
		return null;
	}

	internal void RemoveControlMenuItems(MdiWindowManager wm)
	{
		Form form = wm.form;
		MenuStrip mainMenuStrip = form.MdiParent.MainMenuStrip;
		if (mainMenuStrip == null)
		{
			return;
		}
		mainMenuStrip.SuspendLayout();
		for (int num = mainMenuStrip.Items.Count - 1; num >= 0; num--)
		{
			if (mainMenuStrip.Items[num] is MdiControlStrip.SystemMenuItem)
			{
				if ((mainMenuStrip.Items[num] as MdiControlStrip.SystemMenuItem).MdiForm == form)
				{
					mainMenuStrip.Items.RemoveAt(num);
				}
			}
			else if (mainMenuStrip.Items[num] is MdiControlStrip.ControlBoxMenuItem && (mainMenuStrip.Items[num] as MdiControlStrip.ControlBoxMenuItem).MdiForm == form)
			{
				mainMenuStrip.Items.RemoveAt(num);
			}
		}
		mainMenuStrip.ResumeLayout();
	}

	internal void SetWindowState(Form form, FormWindowState old_window_state, FormWindowState new_window_state, bool is_activating_child)
	{
		MdiWindowManager mdiWindowManager = (MdiWindowManager)form.window_manager;
		if (!is_activating_child && new_window_state == FormWindowState.Maximized && !mdiWindowManager.IsActive)
		{
			ActivateChild(form);
			return;
		}
		if (old_window_state == FormWindowState.Normal)
		{
			mdiWindowManager.NormalBounds = form.Bounds;
		}
		if (!SetWindowStates(mdiWindowManager) && old_window_state != new_window_state)
		{
			bool flag = old_window_state == FormWindowState.Maximized || new_window_state == FormWindowState.Maximized;
			switch (new_window_state)
			{
			case FormWindowState.Minimized:
				ArrangeIconicWindows(rearrange_all: false);
				break;
			case FormWindowState.Maximized:
				form.Bounds = mdiWindowManager.MaximizedBounds;
				break;
			case FormWindowState.Normal:
				form.Bounds = mdiWindowManager.NormalBounds;
				break;
			}
			mdiWindowManager.UpdateWindowDecorations(new_window_state);
			form.ResetCursor();
			if (flag)
			{
				base.Parent.PerformLayout();
			}
			XplatUI.RequestNCRecalc(base.Parent.Handle);
			XplatUI.RequestNCRecalc(form.Handle);
			if (!setting_windowstates)
			{
				SizeScrollBars();
			}
		}
	}

	internal void ActivateActiveMdiChild()
	{
		if (ParentForm.is_changing_visible_state > 0)
		{
			return;
		}
		for (int i = 0; i < base.Controls.Count; i++)
		{
			if (base.Controls[i].Visible)
			{
				ActivateChild((Form)base.Controls[i]);
				break;
			}
		}
	}
}

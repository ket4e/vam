using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ComVisible(true)]
[ToolboxItemFilter("System.Windows.Forms.Control.TopLevel", ToolboxItemFilterType.Allow)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ToolboxItem(true)]
[DesignTimeVisible(true)]
[Designer("System.ComponentModel.Design.ComponentDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
[DefaultProperty("Document")]
public class PrintPreviewDialog : Form
{
	private class PrintToolBar : ToolBar
	{
		private bool left_pressed;

		private bool OnDropDownButton => base.CurrentItem != -1 && items[base.CurrentItem].Button.Style == ToolBarButtonStyle.DropDownButton;

		public int GetNext(int pos)
		{
			while (++pos < items.Length && items[pos].Button.Style == ToolBarButtonStyle.Separator)
			{
			}
			return pos;
		}

		public int GetPrev(int pos)
		{
			while (--pos > -1 && items[pos].Button.Style == ToolBarButtonStyle.Separator)
			{
			}
			return pos;
		}

		private void SelectNextOnParent(bool forward)
		{
			if (base.Parent is ContainerControl containerControl && containerControl.ActiveControl != null)
			{
				containerControl.SelectNextControl(containerControl.ActiveControl, forward, tabStopOnly: true, nested: true, wrap: true);
			}
		}

		protected override void OnGotFocus(EventArgs args)
		{
			base.OnGotFocus(args);
			base.CurrentItem = (((Control.ModifierKeys & Keys.Shift) != 0 || left_pressed) ? GetPrev(items.Length) : 0);
			left_pressed = false;
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			switch (keyData & Keys.KeyCode)
			{
			case Keys.Left:
				left_pressed = true;
				SelectNextOnParent(forward: false);
				return true;
			case Keys.Right:
				SelectNextOnParent(forward: true);
				return true;
			default:
				return base.ProcessDialogKey(keyData);
			}
		}

		private void NavigateItems(Keys key)
		{
			bool flag = true;
			switch (key & Keys.KeyCode)
			{
			case Keys.Left:
				flag = false;
				break;
			case Keys.Right:
				flag = true;
				break;
			case Keys.Tab:
				flag = (Control.ModifierKeys & Keys.Shift) == 0;
				break;
			}
			int num = ((!flag) ? GetPrev(base.CurrentItem) : GetNext(base.CurrentItem));
			if (num < 0 || num >= items.Length)
			{
				base.CurrentItem = -1;
				SelectNextOnParent(flag);
			}
			else
			{
				base.CurrentItem = num;
			}
		}

		internal override bool InternalPreProcessMessage(ref Message msg)
		{
			Keys keys = (Keys)msg.WParam.ToInt32();
			switch (keys)
			{
			case Keys.Up:
			case Keys.Down:
				if (OnDropDownButton)
				{
					break;
				}
				return true;
			case Keys.Tab:
			case Keys.Left:
			case Keys.Right:
				if (OnDropDownButton)
				{
					((ContextMenu)items[base.CurrentItem].Button.DropDownMenu).Hide();
				}
				NavigateItems(keys);
				return true;
			}
			return base.InternalPreProcessMessage(ref msg);
		}
	}

	private PrintPreviewControl print_preview;

	private MenuItem previous_checked_menu_item;

	private Menu mag_menu;

	private MenuItem auto_zoom_item;

	private NumericUpDown pageUpDown;

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new IButtonControl AcceptButton
	{
		get
		{
			return base.AcceptButton;
		}
		set
		{
			base.AcceptButton = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new string AccessibleDescription
	{
		get
		{
			return base.AccessibleDescription;
		}
		set
		{
			base.AccessibleDescription = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new string AccessibleName
	{
		get
		{
			return base.AccessibleName;
		}
		set
		{
			base.AccessibleName = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new AccessibleRole AccessibleRole
	{
		get
		{
			return base.AccessibleRole;
		}
		set
		{
			base.AccessibleRole = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override bool AllowDrop
	{
		get
		{
			return base.AllowDrop;
		}
		set
		{
			base.AllowDrop = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override AnchorStyles Anchor
	{
		get
		{
			return base.Anchor;
		}
		set
		{
			base.Anchor = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new bool AutoScale
	{
		get
		{
			return base.AutoScale;
		}
		set
		{
			base.AutoScale = value;
		}
	}

	[Browsable(false)]
	[Obsolete("This property has been deprecated.  Use AutoScaleDimensions instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override Size AutoScaleBaseSize
	{
		get
		{
			return base.AutoScaleBaseSize;
		}
		set
		{
			base.AutoScaleBaseSize = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public override bool AutoScroll
	{
		get
		{
			return base.AutoScroll;
		}
		set
		{
			base.AutoScroll = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new Size AutoScrollMargin
	{
		get
		{
			return base.AutoScrollMargin;
		}
		set
		{
			base.AutoScrollMargin = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new Size AutoScrollMinSize
	{
		get
		{
			return base.AutoScrollMinSize;
		}
		set
		{
			base.AutoScrollMinSize = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override bool AutoSize
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

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public override AutoValidate AutoValidate
	{
		get
		{
			return base.AutoValidate;
		}
		set
		{
			base.AutoValidate = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override Color BackColor
	{
		get
		{
			return base.BackColor;
		}
		set
		{
			base.BackColor = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new IButtonControl CancelButton
	{
		get
		{
			return base.CancelButton;
		}
		set
		{
			base.CancelButton = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new bool CausesValidation
	{
		get
		{
			return base.CausesValidation;
		}
		set
		{
			base.CausesValidation = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public override ContextMenu ContextMenu
	{
		get
		{
			return base.ContextMenu;
		}
		set
		{
			base.ContextMenu = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public override ContextMenuStrip ContextMenuStrip
	{
		get
		{
			return base.ContextMenuStrip;
		}
		set
		{
			base.ContextMenuStrip = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new bool ControlBox
	{
		get
		{
			return base.ControlBox;
		}
		set
		{
			base.ControlBox = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public override Cursor Cursor
	{
		get
		{
			return base.Cursor;
		}
		set
		{
			base.Cursor = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new ControlBindingsCollection DataBindings => base.DataBindings;

	protected override Size DefaultMinimumSize => new Size(370, 300);

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public override DockStyle Dock
	{
		get
		{
			return base.Dock;
		}
		set
		{
			base.Dock = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new DockPaddingEdges DockPadding => base.DockPadding;

	[DefaultValue(null)]
	public PrintDocument Document
	{
		get
		{
			return print_preview.Document;
		}
		set
		{
			print_preview.Document = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new bool Enabled
	{
		get
		{
			return base.Enabled;
		}
		set
		{
			base.Enabled = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override Font Font
	{
		get
		{
			return base.Font;
		}
		set
		{
			base.Font = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override Color ForeColor
	{
		get
		{
			return base.ForeColor;
		}
		set
		{
			base.ForeColor = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new FormBorderStyle FormBorderStyle
	{
		get
		{
			return base.FormBorderStyle;
		}
		set
		{
			base.FormBorderStyle = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new bool HelpButton
	{
		get
		{
			return base.HelpButton;
		}
		set
		{
			base.HelpButton = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new Icon Icon
	{
		get
		{
			return base.Icon;
		}
		set
		{
			base.Icon = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new ImeMode ImeMode
	{
		get
		{
			return base.ImeMode;
		}
		set
		{
			base.ImeMode = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new bool IsMdiContainer
	{
		get
		{
			return base.IsMdiContainer;
		}
		set
		{
			base.IsMdiContainer = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new bool KeyPreview
	{
		get
		{
			return base.KeyPreview;
		}
		set
		{
			base.KeyPreview = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new Point Location
	{
		get
		{
			return base.Location;
		}
		set
		{
			base.Location = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new Padding Margin
	{
		get
		{
			return base.Margin;
		}
		set
		{
			base.Margin = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new bool MaximizeBox
	{
		get
		{
			return base.MaximizeBox;
		}
		set
		{
			base.MaximizeBox = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new Size MaximumSize
	{
		get
		{
			return base.MaximumSize;
		}
		set
		{
			base.MaximumSize = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new MainMenu Menu
	{
		get
		{
			return base.Menu;
		}
		set
		{
			base.Menu = value;
		}
	}

	[DefaultValue(false)]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new bool MinimizeBox
	{
		get
		{
			return base.MinimizeBox;
		}
		set
		{
			base.MinimizeBox = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new Size MinimumSize
	{
		get
		{
			return base.MinimumSize;
		}
		set
		{
			base.MinimumSize = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public new double Opacity
	{
		get
		{
			return base.Opacity;
		}
		set
		{
			base.Opacity = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new Padding Padding
	{
		get
		{
			return base.Padding;
		}
		set
		{
			base.Padding = value;
		}
	}

	[Browsable(false)]
	public PrintPreviewControl PrintPreviewControl => print_preview;

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override RightToLeft RightToLeft
	{
		get
		{
			return base.RightToLeft;
		}
		set
		{
			base.RightToLeft = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override bool RightToLeftLayout
	{
		get
		{
			return base.RightToLeftLayout;
		}
		set
		{
			base.RightToLeftLayout = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[DefaultValue(false)]
	public new bool ShowInTaskbar
	{
		get
		{
			return base.ShowInTaskbar;
		}
		set
		{
			base.ShowInTaskbar = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new Size Size
	{
		get
		{
			return base.Size;
		}
		set
		{
			base.Size = value;
		}
	}

	[Browsable(false)]
	[DefaultValue(SizeGripStyle.Hide)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new SizeGripStyle SizeGripStyle
	{
		get
		{
			return base.SizeGripStyle;
		}
		set
		{
			base.SizeGripStyle = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new FormStartPosition StartPosition
	{
		get
		{
			return base.StartPosition;
		}
		set
		{
			base.StartPosition = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new bool TabStop
	{
		get
		{
			return base.TabStop;
		}
		set
		{
			base.TabStop = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new object Tag
	{
		get
		{
			return base.Tag;
		}
		set
		{
			base.Tag = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public override string Text
	{
		get
		{
			return base.Text;
		}
		set
		{
			base.Text = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new bool TopMost
	{
		get
		{
			return base.TopMost;
		}
		set
		{
			base.TopMost = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new Color TransparencyKey
	{
		get
		{
			return base.TransparencyKey;
		}
		set
		{
			base.TransparencyKey = value;
		}
	}

	[DefaultValue(false)]
	public bool UseAntiAlias
	{
		get
		{
			return print_preview.UseAntiAlias;
		}
		set
		{
			print_preview.UseAntiAlias = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new bool UseWaitCursor
	{
		get
		{
			return base.UseWaitCursor;
		}
		set
		{
			base.UseWaitCursor = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new bool Visible
	{
		get
		{
			return base.Visible;
		}
		set
		{
			base.Visible = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new FormWindowState WindowState
	{
		get
		{
			return base.WindowState;
		}
		set
		{
			base.WindowState = value;
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

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler AutoValidateChanged
	{
		add
		{
			base.AutoValidateChanged += value;
		}
		remove
		{
			base.AutoValidateChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler BackColorChanged
	{
		add
		{
			base.BackColorChanged += value;
		}
		remove
		{
			base.BackColorChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler BackgroundImageChanged
	{
		add
		{
			base.BackgroundImageChanged += value;
		}
		remove
		{
			base.BackgroundImageChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler BackgroundImageLayoutChanged
	{
		add
		{
			base.BackgroundImageLayoutChanged += value;
		}
		remove
		{
			base.BackgroundImageLayoutChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler CausesValidationChanged
	{
		add
		{
			base.CausesValidationChanged += value;
		}
		remove
		{
			base.CausesValidationChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler ContextMenuChanged
	{
		add
		{
			base.ContextMenuChanged += value;
		}
		remove
		{
			base.ContextMenuChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler ContextMenuStripChanged
	{
		add
		{
			base.ContextMenuStripChanged += value;
		}
		remove
		{
			base.ContextMenuStripChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler CursorChanged
	{
		add
		{
			base.CursorChanged += value;
		}
		remove
		{
			base.CursorChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler DockChanged
	{
		add
		{
			base.DockChanged += value;
		}
		remove
		{
			base.DockChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler EnabledChanged
	{
		add
		{
			base.EnabledChanged += value;
		}
		remove
		{
			base.EnabledChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler FontChanged
	{
		add
		{
			base.FontChanged += value;
		}
		remove
		{
			base.FontChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler ForeColorChanged
	{
		add
		{
			base.ForeColorChanged += value;
		}
		remove
		{
			base.ForeColorChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler ImeModeChanged
	{
		add
		{
			base.ImeModeChanged += value;
		}
		remove
		{
			base.ImeModeChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler LocationChanged
	{
		add
		{
			base.LocationChanged += value;
		}
		remove
		{
			base.LocationChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler MarginChanged
	{
		add
		{
			base.MarginChanged += value;
		}
		remove
		{
			base.MarginChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler MaximumSizeChanged
	{
		add
		{
			base.MaximumSizeChanged += value;
		}
		remove
		{
			base.MaximumSizeChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler MinimumSizeChanged
	{
		add
		{
			base.MinimumSizeChanged += value;
		}
		remove
		{
			base.MinimumSizeChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler PaddingChanged
	{
		add
		{
			base.PaddingChanged += value;
		}
		remove
		{
			base.PaddingChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler RightToLeftChanged
	{
		add
		{
			base.RightToLeftChanged += value;
		}
		remove
		{
			base.RightToLeftChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler RightToLeftLayoutChanged
	{
		add
		{
			base.RightToLeftLayoutChanged += value;
		}
		remove
		{
			base.RightToLeftLayoutChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler SizeChanged
	{
		add
		{
			base.SizeChanged += value;
		}
		remove
		{
			base.SizeChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler TabStopChanged
	{
		add
		{
			base.TabStopChanged += value;
		}
		remove
		{
			base.TabStopChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler TextChanged
	{
		add
		{
			base.TextChanged += value;
		}
		remove
		{
			base.TextChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler VisibleChanged
	{
		add
		{
			base.VisibleChanged += value;
		}
		remove
		{
			base.VisibleChanged -= value;
		}
	}

	public PrintPreviewDialog()
	{
		base.ClientSize = new Size(400, 300);
		ToolBar toolBar = CreateToolBar();
		toolBar.Location = new Point(0, 0);
		toolBar.Dock = DockStyle.Top;
		base.Controls.Add(toolBar);
		print_preview = new PrintPreviewControl();
		print_preview.Location = new Point(0, toolBar.Location.Y + toolBar.Size.Height);
		print_preview.Size = new Size(base.ClientSize.Width, base.ClientSize.Height - toolBar.Bottom);
		print_preview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		print_preview.TabStop = false;
		base.Controls.Add(print_preview);
		print_preview.Show();
	}

	private ToolBar CreateToolBar()
	{
		ImageList imageList = new ImageList();
		imageList.Images.Add(ResourceImageLoader.Get("32_printer.png"));
		imageList.Images.Add(ResourceImageLoader.Get("22_page-magnifier.png"));
		imageList.Images.Add(ResourceImageLoader.Get("1-up.png"));
		imageList.Images.Add(ResourceImageLoader.Get("2-up.png"));
		imageList.Images.Add(ResourceImageLoader.Get("3-up.png"));
		imageList.Images.Add(ResourceImageLoader.Get("4-up.png"));
		imageList.Images.Add(ResourceImageLoader.Get("6-up.png"));
		mag_menu = new ContextMenu();
		ToolBar toolBar = new PrintToolBar();
		ToolBarButton toolBarButton = new ToolBarButton();
		ToolBarButton toolBarButton2 = new ToolBarButton();
		ToolBarButton toolBarButton3 = new ToolBarButton();
		ToolBarButton toolBarButton4 = new ToolBarButton();
		ToolBarButton toolBarButton5 = new ToolBarButton();
		ToolBarButton toolBarButton6 = new ToolBarButton();
		ToolBarButton toolBarButton7 = new ToolBarButton();
		ToolBarButton toolBarButton8 = new ToolBarButton();
		ToolBarButton toolBarButton9 = new ToolBarButton();
		Button button = new Button();
		Label label = new Label();
		pageUpDown = new NumericUpDown();
		toolBar.ImageList = imageList;
		toolBar.Size = new Size(792, 26);
		toolBar.Dock = DockStyle.Top;
		toolBar.Appearance = ToolBarAppearance.Flat;
		toolBar.ShowToolTips = true;
		toolBar.DropDownArrows = true;
		toolBar.TabStop = true;
		toolBar.Buttons.AddRange(new ToolBarButton[9] { toolBarButton, toolBarButton2, toolBarButton3, toolBarButton4, toolBarButton5, toolBarButton6, toolBarButton7, toolBarButton8, toolBarButton9 });
		toolBar.ButtonClick += OnClickToolBarButton;
		toolBarButton.ImageIndex = 0;
		toolBarButton.Tag = 0;
		toolBarButton.ToolTipText = "Print";
		toolBarButton2.ImageIndex = 1;
		toolBarButton2.Tag = 1;
		toolBarButton2.ToolTipText = "Zoom";
		toolBarButton2.Style = ToolBarButtonStyle.DropDownButton;
		toolBarButton2.DropDownMenu = mag_menu;
		MenuItem menuItem = mag_menu.MenuItems.Add("Auto", OnClickPageMagnifierItem);
		menuItem.RadioCheck = true;
		menuItem.Checked = true;
		previous_checked_menu_item = menuItem;
		auto_zoom_item = menuItem;
		menuItem = mag_menu.MenuItems.Add("500%", OnClickPageMagnifierItem);
		menuItem.RadioCheck = true;
		menuItem = mag_menu.MenuItems.Add("200%", OnClickPageMagnifierItem);
		menuItem.RadioCheck = true;
		menuItem = mag_menu.MenuItems.Add("150%", OnClickPageMagnifierItem);
		menuItem.RadioCheck = true;
		menuItem = mag_menu.MenuItems.Add("100%", OnClickPageMagnifierItem);
		menuItem.RadioCheck = true;
		menuItem = mag_menu.MenuItems.Add("75%", OnClickPageMagnifierItem);
		menuItem.RadioCheck = true;
		menuItem = mag_menu.MenuItems.Add("50%", OnClickPageMagnifierItem);
		menuItem.RadioCheck = true;
		menuItem = mag_menu.MenuItems.Add("25%", OnClickPageMagnifierItem);
		menuItem.RadioCheck = true;
		menuItem = mag_menu.MenuItems.Add("10%", OnClickPageMagnifierItem);
		menuItem.RadioCheck = true;
		toolBarButton3.Style = ToolBarButtonStyle.Separator;
		toolBarButton4.ImageIndex = 2;
		toolBarButton4.Tag = 2;
		toolBarButton4.ToolTipText = "One page";
		toolBarButton5.ImageIndex = 3;
		toolBarButton5.Tag = 3;
		toolBarButton5.ToolTipText = "Two pages";
		toolBarButton6.ImageIndex = 4;
		toolBarButton6.Tag = 4;
		toolBarButton6.ToolTipText = "Three pages";
		toolBarButton7.ImageIndex = 5;
		toolBarButton7.Tag = 5;
		toolBarButton7.ToolTipText = "Four pages";
		toolBarButton8.ImageIndex = 6;
		toolBarButton8.Tag = 6;
		toolBarButton8.ToolTipText = "Six pages";
		toolBarButton9.Style = ToolBarButtonStyle.Separator;
		label.Text = "Page";
		label.TabStop = false;
		label.Size = new Size(50, 18);
		label.TextAlign = ContentAlignment.MiddleLeft;
		label.Dock = DockStyle.Right;
		pageUpDown.Dock = DockStyle.Right;
		pageUpDown.TextAlign = HorizontalAlignment.Right;
		pageUpDown.DecimalPlaces = 0;
		pageUpDown.TabIndex = 1;
		pageUpDown.Text = "1";
		pageUpDown.Minimum = 0m;
		pageUpDown.Maximum = 1000m;
		pageUpDown.Size = new Size(64, 14);
		pageUpDown.Dock = DockStyle.Right;
		pageUpDown.ValueChanged += OnPageUpDownValueChanged;
		button.Location = new Point(196, 2);
		button.Size = new Size(50, 20);
		button.TabIndex = 0;
		button.FlatStyle = FlatStyle.Popup;
		button.Text = "Close";
		button.Click += CloseButtonClicked;
		toolBar.Controls.Add(label);
		toolBar.Controls.Add(pageUpDown);
		toolBar.Controls.Add(button);
		return toolBar;
	}

	private void CloseButtonClicked(object sender, EventArgs e)
	{
		Close();
	}

	private void OnPageUpDownValueChanged(object sender, EventArgs e)
	{
		print_preview.StartPage = (int)pageUpDown.Value;
	}

	private void OnClickToolBarButton(object sender, ToolBarButtonClickEventArgs e)
	{
		if (e.Button.Tag != null && e.Button.Tag is int)
		{
			switch ((int)e.Button.Tag)
			{
			case 0:
				Console.WriteLine("do print here");
				break;
			case 1:
				OnClickPageMagnifierItem(auto_zoom_item, EventArgs.Empty);
				break;
			case 2:
				print_preview.Rows = 0;
				print_preview.Columns = 1;
				break;
			case 3:
				print_preview.Rows = 0;
				print_preview.Columns = 2;
				break;
			case 4:
				print_preview.Rows = 0;
				print_preview.Columns = 3;
				break;
			case 5:
				print_preview.Rows = 1;
				print_preview.Columns = 2;
				break;
			case 6:
				print_preview.Rows = 1;
				print_preview.Columns = 3;
				break;
			}
		}
	}

	private void OnClickPageMagnifierItem(object sender, EventArgs e)
	{
		MenuItem menuItem = (MenuItem)sender;
		previous_checked_menu_item.Checked = false;
		switch (menuItem.Index)
		{
		case 0:
			print_preview.AutoZoom = true;
			break;
		case 1:
			print_preview.AutoZoom = false;
			print_preview.Zoom = 5.0;
			break;
		case 2:
			print_preview.AutoZoom = false;
			print_preview.Zoom = 2.0;
			break;
		case 3:
			print_preview.AutoZoom = false;
			print_preview.Zoom = 1.5;
			break;
		case 4:
			print_preview.AutoZoom = false;
			print_preview.Zoom = 1.0;
			break;
		case 5:
			print_preview.AutoZoom = false;
			print_preview.Zoom = 0.75;
			break;
		case 6:
			print_preview.AutoZoom = false;
			print_preview.Zoom = 0.5;
			break;
		case 7:
			print_preview.AutoZoom = false;
			print_preview.Zoom = 0.25;
			break;
		case 8:
			print_preview.AutoZoom = false;
			print_preview.Zoom = 0.1;
			break;
		}
		menuItem.Checked = true;
		previous_checked_menu_item = menuItem;
	}

	[System.MonoInternalNote("Throw InvalidPrinterException")]
	protected override void CreateHandle()
	{
		base.CreateHandle();
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		print_preview.InvalidatePreview();
		base.OnClosing(e);
	}

	protected override bool ProcessDialogKey(Keys keyData)
	{
		switch (keyData)
		{
		case Keys.Left:
		case Keys.Up:
		case Keys.Right:
		case Keys.Down:
			return false;
		default:
			return base.ProcessDialogKey(keyData);
		}
	}

	protected override bool ProcessTabKey(bool forward)
	{
		return base.ProcessTabKey(forward);
	}
}

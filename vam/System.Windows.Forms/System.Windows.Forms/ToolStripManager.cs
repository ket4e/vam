using System.Collections.Generic;
using System.ComponentModel;

namespace System.Windows.Forms;

public sealed class ToolStripManager
{
	private static ToolStripRenderer renderer = new ToolStripProfessionalRenderer();

	private static ToolStripManagerRenderMode render_mode = ToolStripManagerRenderMode.Professional;

	private static bool visual_styles_enabled = Application.RenderWithVisualStyles;

	private static List<WeakReference> toolstrips = new List<WeakReference>();

	private static List<ToolStripMenuItem> menu_items = new List<ToolStripMenuItem>();

	private static bool activated_by_keyboard;

	public static ToolStripRenderer Renderer
	{
		get
		{
			return renderer;
		}
		set
		{
			if (Renderer != value)
			{
				renderer = value;
				OnRendererChanged(EventArgs.Empty);
			}
		}
	}

	public static ToolStripManagerRenderMode RenderMode
	{
		get
		{
			return render_mode;
		}
		set
		{
			if (!Enum.IsDefined(typeof(ToolStripManagerRenderMode), value))
			{
				throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for ToolStripManagerRenderMode");
			}
			if (render_mode != value)
			{
				render_mode = value;
				switch (value)
				{
				case ToolStripManagerRenderMode.Custom:
					throw new NotSupportedException();
				case ToolStripManagerRenderMode.System:
					Renderer = new ToolStripSystemRenderer();
					break;
				case ToolStripManagerRenderMode.Professional:
					Renderer = new ToolStripProfessionalRenderer();
					break;
				}
			}
		}
	}

	public static bool VisualStylesEnabled
	{
		get
		{
			return visual_styles_enabled;
		}
		set
		{
			if (visual_styles_enabled != value)
			{
				visual_styles_enabled = value;
				if (render_mode == ToolStripManagerRenderMode.Professional)
				{
					(renderer as ToolStripProfessionalRenderer).ColorTable.UseSystemColors = !value;
					OnRendererChanged(EventArgs.Empty);
				}
			}
		}
	}

	internal static bool ActivatedByKeyboard
	{
		get
		{
			return activated_by_keyboard;
		}
		set
		{
			activated_by_keyboard = value;
		}
	}

	public static event EventHandler RendererChanged;

	internal static event EventHandler AppClicked;

	internal static event EventHandler AppFocusChange;

	private ToolStripManager()
	{
	}

	public static ToolStrip FindToolStrip(string toolStripName)
	{
		lock (toolstrips)
		{
			foreach (WeakReference toolstrip in toolstrips)
			{
				ToolStrip toolStrip = (ToolStrip)toolstrip.Target;
				if (toolStrip == null || !(toolStrip.Name == toolStripName))
				{
					continue;
				}
				return toolStrip;
			}
		}
		return null;
	}

	public static bool IsShortcutDefined(Keys shortcut)
	{
		lock (menu_items)
		{
			foreach (ToolStripMenuItem menu_item in menu_items)
			{
				if (menu_item.ShortcutKeys == shortcut)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool IsValidShortcut(Keys shortcut)
	{
		if ((shortcut & Keys.F1) == Keys.F1)
		{
			return true;
		}
		if ((shortcut & Keys.F2) == Keys.F2)
		{
			return true;
		}
		if ((shortcut & Keys.F3) == Keys.F3)
		{
			return true;
		}
		if ((shortcut & Keys.F4) == Keys.F4)
		{
			return true;
		}
		if ((shortcut & Keys.F5) == Keys.F5)
		{
			return true;
		}
		if ((shortcut & Keys.F6) == Keys.F6)
		{
			return true;
		}
		if ((shortcut & Keys.F7) == Keys.F7)
		{
			return true;
		}
		if ((shortcut & Keys.F8) == Keys.F8)
		{
			return true;
		}
		if ((shortcut & Keys.F9) == Keys.F9)
		{
			return true;
		}
		if ((shortcut & Keys.F10) == Keys.F10)
		{
			return true;
		}
		if ((shortcut & Keys.F11) == Keys.F11)
		{
			return true;
		}
		if ((shortcut & Keys.F12) == Keys.F12)
		{
			return true;
		}
		if (shortcut == Keys.Shift || shortcut == Keys.Control || shortcut == (Keys.Shift | Keys.Control) || shortcut == Keys.Alt || shortcut == (Keys.Shift | Keys.Alt) || shortcut == (Keys.Control | Keys.Alt) || shortcut == (Keys.Shift | Keys.Control | Keys.Alt))
		{
			return false;
		}
		if ((shortcut & Keys.Alt) == Keys.Alt)
		{
			return true;
		}
		if ((shortcut & Keys.Control) == Keys.Control)
		{
			return true;
		}
		if ((shortcut & Keys.Shift) == Keys.Shift)
		{
			return true;
		}
		return false;
	}

	[System.MonoTODO("Stub, does nothing")]
	public static void LoadSettings(Form targetForm)
	{
		if (targetForm == null)
		{
			throw new ArgumentNullException("targetForm");
		}
	}

	[System.MonoTODO("Stub, does nothing")]
	public static void LoadSettings(Form targetForm, string key)
	{
		if (targetForm == null)
		{
			throw new ArgumentNullException("targetForm");
		}
		if (string.IsNullOrEmpty(key))
		{
			throw new ArgumentNullException("key");
		}
	}

	[System.MonoLimitation("Only supports one level of merging, cannot merge the same ToolStrip multiple times")]
	public static bool Merge(ToolStrip sourceToolStrip, string targetName)
	{
		if (string.IsNullOrEmpty(targetName))
		{
			throw new ArgumentNullException("targetName");
		}
		return Merge(sourceToolStrip, FindToolStrip(targetName));
	}

	[System.MonoLimitation("Only supports one level of merging, cannot merge the same ToolStrip multiple times")]
	public static bool Merge(ToolStrip sourceToolStrip, ToolStrip targetToolStrip)
	{
		if (sourceToolStrip == null)
		{
			throw new ArgumentNullException("sourceToolStrip");
		}
		if (targetToolStrip == null)
		{
			throw new ArgumentNullException("targetName");
		}
		if (targetToolStrip == sourceToolStrip)
		{
			throw new ArgumentException("Source and target ToolStrip must be different.");
		}
		if (!sourceToolStrip.AllowMerge || !targetToolStrip.AllowMerge)
		{
			return false;
		}
		if (sourceToolStrip.IsCurrentlyMerged || targetToolStrip.IsCurrentlyMerged)
		{
			return false;
		}
		List<ToolStripItem> list = new List<ToolStripItem>();
		foreach (ToolStripItem item in sourceToolStrip.Items)
		{
			switch (item.MergeAction)
			{
			default:
				list.Add(item);
				break;
			case MergeAction.Insert:
				if (item.MergeIndex >= 0)
				{
					list.Add(item);
				}
				break;
			case MergeAction.Replace:
			case MergeAction.Remove:
			case MergeAction.MatchOnly:
				foreach (ToolStripItem item2 in targetToolStrip.Items)
				{
					if (item.Text == item2.Text)
					{
						list.Add(item);
						break;
					}
				}
				break;
			}
		}
		if (list.Count == 0)
		{
			return false;
		}
		sourceToolStrip.BeginMerge();
		targetToolStrip.BeginMerge();
		sourceToolStrip.SuspendLayout();
		targetToolStrip.SuspendLayout();
		while (list.Count > 0)
		{
			ToolStripItem toolStripItem3 = list[0];
			list.Remove(toolStripItem3);
			switch (toolStripItem3.MergeAction)
			{
			default:
				ToolStrip.SetItemParent(toolStripItem3, targetToolStrip);
				break;
			case MergeAction.Insert:
				RemoveItemFromParentToolStrip(toolStripItem3);
				if (toolStripItem3.MergeIndex != -1)
				{
					if (toolStripItem3.MergeIndex >= CountRealToolStripItems(targetToolStrip))
					{
						targetToolStrip.Items.AddNoOwnerOrLayout(toolStripItem3);
					}
					else
					{
						targetToolStrip.Items.InsertNoOwnerOrLayout(AdjustItemMergeIndex(targetToolStrip, toolStripItem3), toolStripItem3);
					}
					toolStripItem3.Parent = targetToolStrip;
				}
				break;
			case MergeAction.Replace:
				foreach (ToolStripItem item3 in targetToolStrip.Items)
				{
					if (toolStripItem3.Text == item3.Text)
					{
						RemoveItemFromParentToolStrip(toolStripItem3);
						targetToolStrip.Items.InsertNoOwnerOrLayout(targetToolStrip.Items.IndexOf(item3), toolStripItem3);
						targetToolStrip.Items.RemoveNoOwnerOrLayout(item3);
						targetToolStrip.HiddenMergedItems.Add(item3);
						break;
					}
				}
				break;
			case MergeAction.Remove:
				foreach (ToolStripItem item4 in targetToolStrip.Items)
				{
					if (toolStripItem3.Text == item4.Text)
					{
						targetToolStrip.Items.RemoveNoOwnerOrLayout(item4);
						targetToolStrip.HiddenMergedItems.Add(item4);
						break;
					}
				}
				break;
			case MergeAction.MatchOnly:
				foreach (ToolStripItem item5 in targetToolStrip.Items)
				{
					if (toolStripItem3.Text == item5.Text)
					{
						if (item5 is ToolStripMenuItem && toolStripItem3 is ToolStripMenuItem)
						{
							ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)toolStripItem3;
							ToolStripMenuItem toolStripMenuItem2 = (ToolStripMenuItem)item5;
							Merge(toolStripMenuItem.DropDown, toolStripMenuItem2.DropDown);
						}
						break;
					}
				}
				break;
			}
		}
		sourceToolStrip.ResumeLayout();
		targetToolStrip.ResumeLayout();
		sourceToolStrip.CurrentlyMergedWith = targetToolStrip;
		targetToolStrip.CurrentlyMergedWith = sourceToolStrip;
		return true;
	}

	public static bool RevertMerge(string targetName)
	{
		return RevertMerge(FindToolStrip(targetName));
	}

	public static bool RevertMerge(ToolStrip targetToolStrip)
	{
		return RevertMerge(targetToolStrip, targetToolStrip.CurrentlyMergedWith);
	}

	public static bool RevertMerge(ToolStrip targetToolStrip, ToolStrip sourceToolStrip)
	{
		if (sourceToolStrip == null)
		{
			throw new ArgumentNullException("sourceToolStrip");
		}
		List<ToolStripItem> list = new List<ToolStripItem>();
		foreach (ToolStripItem item in targetToolStrip.Items)
		{
			if (item.Owner == sourceToolStrip)
			{
				list.Add(item);
			}
			else
			{
				if (!(item is ToolStripMenuItem))
				{
					continue;
				}
				foreach (ToolStripItem dropDownItem in (item as ToolStripMenuItem).DropDownItems)
				{
					foreach (ToolStripMenuItem item2 in sourceToolStrip.Items)
					{
						if (dropDownItem.Owner == item2.DropDown)
						{
							list.Add(dropDownItem);
						}
					}
				}
			}
		}
		if (list.Count == 0 && targetToolStrip.HiddenMergedItems.Count == 0)
		{
			return false;
		}
		while (targetToolStrip.HiddenMergedItems.Count > 0)
		{
			targetToolStrip.RevertMergeItem(targetToolStrip.HiddenMergedItems[0]);
			targetToolStrip.HiddenMergedItems.RemoveAt(0);
		}
		sourceToolStrip.SuspendLayout();
		targetToolStrip.SuspendLayout();
		while (list.Count > 0)
		{
			sourceToolStrip.RevertMergeItem(list[0]);
			list.Remove(list[0]);
		}
		sourceToolStrip.ResumeLayout();
		targetToolStrip.ResumeLayout();
		sourceToolStrip.IsCurrentlyMerged = false;
		targetToolStrip.IsCurrentlyMerged = false;
		sourceToolStrip.CurrentlyMergedWith = null;
		targetToolStrip.CurrentlyMergedWith = null;
		return true;
	}

	public static void SaveSettings(Form sourceForm)
	{
		if (sourceForm == null)
		{
			throw new ArgumentNullException("sourceForm");
		}
	}

	public static void SaveSettings(Form sourceForm, string key)
	{
		if (sourceForm == null)
		{
			throw new ArgumentNullException("sourceForm");
		}
		if (string.IsNullOrEmpty(key))
		{
			throw new ArgumentNullException("key");
		}
	}

	internal static void AddToolStrip(ToolStrip ts)
	{
		lock (toolstrips)
		{
			toolstrips.Add(new WeakReference(ts));
		}
	}

	private static int AdjustItemMergeIndex(ToolStrip ts, ToolStripItem tsi)
	{
		if (ts.Items[0] is MdiControlStrip.SystemMenuItem)
		{
			return tsi.MergeIndex + 1;
		}
		return tsi.MergeIndex;
	}

	private static int CountRealToolStripItems(ToolStrip ts)
	{
		int num = 0;
		foreach (ToolStripItem item in ts.Items)
		{
			if (!(item is MdiControlStrip.ControlBoxMenuItem) && !(item is MdiControlStrip.SystemMenuItem))
			{
				num++;
			}
		}
		return num;
	}

	internal static ToolStrip GetNextToolStrip(ToolStrip ts, bool forward)
	{
		lock (toolstrips)
		{
			List<ToolStrip> list = new List<ToolStrip>();
			foreach (WeakReference toolstrip in toolstrips)
			{
				ToolStrip toolStrip = (ToolStrip)toolstrip.Target;
				if (toolStrip != null)
				{
					list.Add(toolStrip);
				}
			}
			int num = list.IndexOf(ts);
			if (forward)
			{
				for (int i = num + 1; i < list.Count; i++)
				{
					if (list[i].TopLevelControl == ts.TopLevelControl && !(list[i] is StatusStrip))
					{
						return list[i];
					}
				}
				for (int j = 0; j < num; j++)
				{
					if (list[j].TopLevelControl == ts.TopLevelControl && !(list[j] is StatusStrip))
					{
						return list[j];
					}
				}
			}
			else
			{
				for (int num2 = num - 1; num2 >= 0; num2--)
				{
					if (list[num2].TopLevelControl == ts.TopLevelControl && !(list[num2] is StatusStrip))
					{
						return list[num2];
					}
				}
				for (int num3 = list.Count - 1; num3 > num; num3--)
				{
					if (list[num3].TopLevelControl == ts.TopLevelControl && !(list[num3] is StatusStrip))
					{
						return list[num3];
					}
				}
			}
		}
		return null;
	}

	internal static bool ProcessCmdKey(ref Message m, Keys keyData)
	{
		lock (menu_items)
		{
			foreach (ToolStripMenuItem menu_item in menu_items)
			{
				if (menu_item.ProcessCmdKey(ref m, keyData))
				{
					return true;
				}
			}
		}
		return false;
	}

	internal static bool ProcessMenuKey(ref Message m)
	{
		if (Application.KeyboardCapture != null && Application.KeyboardCapture.OnMenuKey())
		{
			return true;
		}
		Form form = (Form)Control.FromHandle(m.HWnd).TopLevelControl;
		if (form == null)
		{
			return false;
		}
		if (form.MainMenuStrip != null && form.MainMenuStrip.OnMenuKey())
		{
			return true;
		}
		lock (toolstrips)
		{
			foreach (WeakReference toolstrip in toolstrips)
			{
				ToolStrip toolStrip = (ToolStrip)toolstrip.Target;
				if (toolStrip == null || toolStrip.TopLevelControl != form || !toolStrip.OnMenuKey())
				{
					continue;
				}
				return true;
			}
		}
		return false;
	}

	internal static void SetActiveToolStrip(ToolStrip toolStrip, bool keyboard)
	{
		if (Application.KeyboardCapture != null)
		{
			Application.KeyboardCapture.KeyboardActive = false;
		}
		if (toolStrip == null)
		{
			activated_by_keyboard = false;
			return;
		}
		activated_by_keyboard = keyboard;
		toolStrip.KeyboardActive = true;
	}

	internal static void AddToolStripMenuItem(ToolStripMenuItem tsmi)
	{
		lock (menu_items)
		{
			menu_items.Add(tsmi);
		}
	}

	internal static void RemoveToolStrip(ToolStrip ts)
	{
		lock (toolstrips)
		{
			foreach (WeakReference toolstrip in toolstrips)
			{
				if (toolstrip.Target == ts)
				{
					toolstrips.Remove(toolstrip);
					break;
				}
			}
		}
	}

	internal static void RemoveToolStripMenuItem(ToolStripMenuItem tsmi)
	{
		lock (menu_items)
		{
			menu_items.Remove(tsmi);
		}
	}

	internal static void FireAppClicked()
	{
		if (ToolStripManager.AppClicked != null)
		{
			ToolStripManager.AppClicked(null, EventArgs.Empty);
		}
		if (Application.KeyboardCapture != null)
		{
			Application.KeyboardCapture.Dismiss(ToolStripDropDownCloseReason.AppClicked);
		}
	}

	internal static void FireAppFocusChanged(Form form)
	{
		if (ToolStripManager.AppFocusChange != null)
		{
			ToolStripManager.AppFocusChange(form, EventArgs.Empty);
		}
		if (Application.KeyboardCapture != null)
		{
			Application.KeyboardCapture.Dismiss(ToolStripDropDownCloseReason.AppFocusChange);
		}
	}

	internal static void FireAppFocusChanged(object sender)
	{
		if (ToolStripManager.AppFocusChange != null)
		{
			ToolStripManager.AppFocusChange(sender, EventArgs.Empty);
		}
		if (Application.KeyboardCapture != null)
		{
			Application.KeyboardCapture.Dismiss(ToolStripDropDownCloseReason.AppFocusChange);
		}
	}

	private static void OnRendererChanged(EventArgs e)
	{
		if (ToolStripManager.RendererChanged != null)
		{
			ToolStripManager.RendererChanged(null, e);
		}
	}

	private static void RemoveItemFromParentToolStrip(ToolStripItem tsi)
	{
		if (tsi.Owner != null)
		{
			tsi.Owner.Items.RemoveNoOwnerOrLayout(tsi);
			if (tsi.Owner is ToolStripOverflow)
			{
				(tsi.Owner as ToolStripOverflow).ParentToolStrip.Items.RemoveNoOwnerOrLayout(tsi);
			}
		}
	}
}

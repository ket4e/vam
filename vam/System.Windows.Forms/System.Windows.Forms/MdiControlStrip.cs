using System.Drawing;

namespace System.Windows.Forms;

internal class MdiControlStrip
{
	public class SystemMenuItem : ToolStripMenuItem
	{
		private Form form;

		public Form MdiForm
		{
			get
			{
				return form;
			}
			set
			{
				form = value;
			}
		}

		public SystemMenuItem(Form ownerForm)
		{
			form = ownerForm;
			base.AutoSize = false;
			base.Size = new Size(20, 20);
			base.Image = ownerForm.Icon.ToBitmap();
			base.MergeIndex = int.MinValue;
			base.DisplayStyle = ToolStripItemDisplayStyle.Image;
			base.DropDownItems.Add("&Restore", null, RestoreItemHandler);
			ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)base.DropDownItems.Add("&Move");
			toolStripMenuItem.Enabled = false;
			ToolStripMenuItem toolStripMenuItem2 = (ToolStripMenuItem)base.DropDownItems.Add("&Size");
			toolStripMenuItem2.Enabled = false;
			base.DropDownItems.Add("Mi&nimize", null, MinimizeItemHandler);
			ToolStripMenuItem toolStripMenuItem3 = (ToolStripMenuItem)base.DropDownItems.Add("Ma&ximize");
			toolStripMenuItem3.Enabled = false;
			base.DropDownItems.Add("-");
			ToolStripMenuItem toolStripMenuItem4 = (ToolStripMenuItem)base.DropDownItems.Add("&Close", null, CloseItemHandler);
			toolStripMenuItem4.ShortcutKeys = Keys.F4 | Keys.Control;
			base.DropDownItems.Add("-");
			ToolStripMenuItem toolStripMenuItem5 = (ToolStripMenuItem)base.DropDownItems.Add("Nex&t", null, NextItemHandler);
			toolStripMenuItem5.ShortcutKeys = Keys.F6 | Keys.Control;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (base.Owner != null)
			{
				Image image = Image;
				CalculateTextAndImageRectangles(out var _, out var image_rect);
				if (image_rect != Rectangle.Empty)
				{
					base.Owner.Renderer.DrawItemImage(new ToolStripItemImageRenderEventArgs(e.Graphics, this, image, image_rect));
				}
			}
		}

		private void RestoreItemHandler(object sender, EventArgs e)
		{
			form.WindowState = FormWindowState.Normal;
		}

		private void MinimizeItemHandler(object sender, EventArgs e)
		{
			form.WindowState = FormWindowState.Minimized;
		}

		private void CloseItemHandler(object sender, EventArgs e)
		{
			form.Close();
		}

		private void NextItemHandler(object sender, EventArgs e)
		{
			form.MdiParent.MdiContainer.ActivateNextChild();
		}
	}

	public class ControlBoxMenuItem : ToolStripMenuItem
	{
		private Form form;

		private ControlBoxType type;

		public Form MdiForm
		{
			get
			{
				return form;
			}
			set
			{
				form = value;
			}
		}

		public ControlBoxMenuItem(Form ownerForm, ControlBoxType type)
		{
			form = ownerForm;
			this.type = type;
			base.AutoSize = false;
			base.Alignment = ToolStripItemAlignment.Right;
			base.Size = new Size(20, 20);
			base.MergeIndex = int.MaxValue;
			base.DisplayStyle = ToolStripItemDisplayStyle.None;
			switch (type)
			{
			case ControlBoxType.Close:
				base.Click += CloseItemHandler;
				break;
			case ControlBoxType.Min:
				base.Click += MinimizeItemHandler;
				break;
			case ControlBoxType.Max:
				base.Click += RestoreItemHandler;
				break;
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			Graphics graphics = e.Graphics;
			switch (type)
			{
			case ControlBoxType.Close:
				graphics.FillRectangle(Brushes.Black, 8, 8, 4, 4);
				graphics.FillRectangle(Brushes.Black, 6, 6, 2, 2);
				graphics.FillRectangle(Brushes.Black, 6, 12, 2, 2);
				graphics.FillRectangle(Brushes.Black, 12, 6, 2, 2);
				graphics.FillRectangle(Brushes.Black, 12, 12, 2, 2);
				graphics.DrawLine(Pens.Black, 8, 7, 8, 12);
				graphics.DrawLine(Pens.Black, 7, 8, 12, 8);
				graphics.DrawLine(Pens.Black, 11, 7, 11, 12);
				graphics.DrawLine(Pens.Black, 7, 11, 12, 11);
				break;
			case ControlBoxType.Min:
				graphics.DrawLine(Pens.Black, 6, 12, 11, 12);
				graphics.DrawLine(Pens.Black, 6, 13, 11, 13);
				break;
			case ControlBoxType.Max:
				graphics.DrawLines(Pens.Black, new Point[5]
				{
					new Point(7, 8),
					new Point(7, 5),
					new Point(13, 5),
					new Point(13, 10),
					new Point(11, 10)
				});
				graphics.DrawLine(Pens.Black, 7, 6, 12, 6);
				graphics.DrawRectangle(Pens.Black, new Rectangle(5, 8, 6, 5));
				graphics.DrawLine(Pens.Black, 5, 9, 11, 9);
				break;
			}
		}

		private void RestoreItemHandler(object sender, EventArgs e)
		{
			form.WindowState = FormWindowState.Normal;
		}

		private void MinimizeItemHandler(object sender, EventArgs e)
		{
			form.WindowState = FormWindowState.Minimized;
		}

		private void CloseItemHandler(object sender, EventArgs e)
		{
			form.Close();
		}
	}

	public enum ControlBoxType
	{
		Close,
		Min,
		Max
	}
}

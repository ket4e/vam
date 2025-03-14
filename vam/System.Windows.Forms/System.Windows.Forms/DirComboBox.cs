using System.Collections;
using System.Drawing;
using System.IO;

namespace System.Windows.Forms;

internal class DirComboBox : ComboBox, IUpdateFolder
{
	internal class DirComboBoxItem
	{
		private int imageIndex;

		private string name;

		private string path;

		private int xPos;

		private ImageList imageList;

		public int ImageIndex => imageIndex;

		public string Name => name;

		public string Path => path;

		public int XPos => xPos;

		public ImageList ImageList
		{
			get
			{
				return imageList;
			}
			set
			{
				imageList = value;
			}
		}

		public DirComboBoxItem(ImageList imageList, int imageIndex, string name, string path, int xPos)
		{
			this.imageList = imageList;
			this.imageIndex = imageIndex;
			this.name = name;
			this.path = path;
			this.xPos = xPos;
		}

		public override string ToString()
		{
			return name;
		}
	}

	private ImageList imageList = new ImageList();

	private string currentPath;

	private Stack folderStack = new Stack();

	private static readonly int indent = 6;

	private DirComboBoxItem recentlyUsedDirComboboxItem;

	private DirComboBoxItem desktopDirComboboxItem;

	private DirComboBoxItem personalDirComboboxItem;

	private DirComboBoxItem myComputerDirComboboxItem;

	private DirComboBoxItem networkDirComboboxItem;

	private ArrayList myComputerItems = new ArrayList();

	private DirComboBoxItem mainParentDirComboBoxItem;

	private DirComboBoxItem real_parent;

	private MWFVFS vfs;

	private static object CDirectoryChangedEvent = new object();

	public string CurrentFolder
	{
		get
		{
			return currentPath;
		}
		set
		{
			currentPath = value;
			CreateComboList();
		}
	}

	public event EventHandler DirectoryChanged
	{
		add
		{
			base.Events.AddHandler(CDirectoryChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(CDirectoryChangedEvent, value);
		}
	}

	public DirComboBox(MWFVFS vfs)
	{
		this.vfs = vfs;
		SuspendLayout();
		base.DrawMode = DrawMode.OwnerDrawFixed;
		imageList.ColorDepth = ColorDepth.Depth32Bit;
		imageList.ImageSize = new Size(16, 16);
		imageList.Images.Add(ThemeEngine.Current.Images(UIIcon.PlacesRecentDocuments, 16));
		imageList.Images.Add(ThemeEngine.Current.Images(UIIcon.PlacesDesktop, 16));
		imageList.Images.Add(ThemeEngine.Current.Images(UIIcon.PlacesPersonal, 16));
		imageList.Images.Add(ThemeEngine.Current.Images(UIIcon.PlacesMyComputer, 16));
		imageList.Images.Add(ThemeEngine.Current.Images(UIIcon.PlacesMyNetwork, 16));
		imageList.Images.Add(ThemeEngine.Current.Images(UIIcon.NormalFolder, 16));
		imageList.TransparentColor = Color.Transparent;
		recentlyUsedDirComboboxItem = new DirComboBoxItem(imageList, 0, "Recently used", MWFVFS.RecentlyUsedPrefix, 0);
		desktopDirComboboxItem = new DirComboBoxItem(imageList, 1, "Desktop", MWFVFS.DesktopPrefix, 0);
		personalDirComboboxItem = new DirComboBoxItem(imageList, 2, "Personal folder", MWFVFS.PersonalPrefix, indent);
		myComputerDirComboboxItem = new DirComboBoxItem(imageList, 3, "My Computer", MWFVFS.MyComputerPrefix, indent);
		networkDirComboboxItem = new DirComboBoxItem(imageList, 4, "My Network", MWFVFS.MyNetworkPrefix, indent);
		ArrayList myComputerContent = this.vfs.GetMyComputerContent();
		foreach (FSEntry item in myComputerContent)
		{
			myComputerItems.Add(new DirComboBoxItem(MimeIconEngine.LargeIcons, item.IconIndex, item.Name, item.FullName, indent * 2));
		}
		myComputerContent.Clear();
		myComputerContent = null;
		mainParentDirComboBoxItem = myComputerDirComboboxItem;
		ResumeLayout(performLayout: false);
	}

	private void CreateComboList()
	{
		real_parent = null;
		DirComboBoxItem dirComboBoxItem = null;
		if (currentPath == MWFVFS.RecentlyUsedPrefix)
		{
			mainParentDirComboBoxItem = recentlyUsedDirComboboxItem;
			dirComboBoxItem = recentlyUsedDirComboboxItem;
		}
		else if (currentPath == MWFVFS.DesktopPrefix)
		{
			dirComboBoxItem = desktopDirComboboxItem;
			mainParentDirComboBoxItem = desktopDirComboboxItem;
		}
		else if (currentPath == MWFVFS.PersonalPrefix)
		{
			dirComboBoxItem = personalDirComboboxItem;
			mainParentDirComboBoxItem = personalDirComboboxItem;
		}
		else if (currentPath == MWFVFS.MyComputerPrefix)
		{
			dirComboBoxItem = myComputerDirComboboxItem;
			mainParentDirComboBoxItem = myComputerDirComboboxItem;
		}
		else if (currentPath == MWFVFS.MyNetworkPrefix)
		{
			dirComboBoxItem = networkDirComboboxItem;
			mainParentDirComboBoxItem = networkDirComboboxItem;
		}
		else
		{
			foreach (DirComboBoxItem myComputerItem in myComputerItems)
			{
				if (myComputerItem.Path == currentPath)
				{
					dirComboBoxItem = (mainParentDirComboBoxItem = myComputerItem);
					break;
				}
			}
		}
		BeginUpdate();
		base.Items.Clear();
		base.Items.Add(recentlyUsedDirComboboxItem);
		base.Items.Add(desktopDirComboboxItem);
		base.Items.Add(personalDirComboboxItem);
		base.Items.Add(myComputerDirComboboxItem);
		base.Items.AddRange(myComputerItems);
		base.Items.Add(networkDirComboboxItem);
		if (dirComboBoxItem == null)
		{
			real_parent = CreateFolderStack();
		}
		if (real_parent != null)
		{
			int num = 0;
			num = ((real_parent == desktopDirComboboxItem) ? 1 : ((real_parent != personalDirComboboxItem && real_parent != networkDirComboboxItem) ? 3 : 2));
			dirComboBoxItem = AppendToParent(num, real_parent);
		}
		EndUpdate();
		if (dirComboBoxItem != null)
		{
			base.SelectedItem = dirComboBoxItem;
		}
	}

	private DirComboBoxItem CreateFolderStack()
	{
		folderStack.Clear();
		DirectoryInfo directoryInfo = new DirectoryInfo(currentPath);
		folderStack.Push(directoryInfo);
		bool ignoreCase = !XplatUI.RunningOnUnix;
		while (directoryInfo.Parent != null)
		{
			directoryInfo = directoryInfo.Parent;
			if (mainParentDirComboBoxItem != personalDirComboboxItem && string.Compare(directoryInfo.FullName, ThemeEngine.Current.Places(UIIcon.PlacesDesktop), ignoreCase) == 0)
			{
				return desktopDirComboboxItem;
			}
			if (mainParentDirComboBoxItem == personalDirComboboxItem)
			{
				if (string.Compare(directoryInfo.FullName, ThemeEngine.Current.Places(UIIcon.PlacesPersonal), ignoreCase) == 0)
				{
					return personalDirComboboxItem;
				}
			}
			else
			{
				foreach (DirComboBoxItem myComputerItem in myComputerItems)
				{
					if (string.Compare(myComputerItem.Path, directoryInfo.FullName, ignoreCase) == 0)
					{
						return myComputerItem;
					}
				}
			}
			folderStack.Push(directoryInfo);
		}
		return null;
	}

	private DirComboBoxItem AppendToParent(int nr_indents, DirComboBoxItem parentDirComboBoxItem)
	{
		DirComboBoxItem result = null;
		int num = base.Items.IndexOf(parentDirComboBoxItem) + 1;
		int num2 = indent * nr_indents;
		while (folderStack.Count != 0)
		{
			DirectoryInfo directoryInfo = folderStack.Pop() as DirectoryInfo;
			DirComboBoxItem dirComboBoxItem = new DirComboBoxItem(imageList, 5, directoryInfo.Name, directoryInfo.FullName, num2);
			base.Items.Insert(num, dirComboBoxItem);
			num++;
			result = dirComboBoxItem;
			num2 += indent;
		}
		return result;
	}

	protected override void OnDrawItem(DrawItemEventArgs e)
	{
		if (e.Index == -1)
		{
			return;
		}
		DirComboBoxItem dirComboBoxItem = base.Items[e.Index] as DirComboBoxItem;
		Bitmap bitmap = new Bitmap(e.Bounds.Width, e.Bounds.Height, e.Graphics);
		Graphics graphics = Graphics.FromImage(bitmap);
		Color backColor = e.BackColor;
		Color color = e.ForeColor;
		int num = dirComboBoxItem.XPos;
		if ((e.State & DrawItemState.ComboBoxEdit) != 0)
		{
			num = 0;
		}
		graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(backColor), new Rectangle(0, 0, bitmap.Width, bitmap.Height));
		if ((e.State & DrawItemState.Selected) == DrawItemState.Selected && (!base.DroppedDown || (e.State & DrawItemState.ComboBoxEdit) != DrawItemState.ComboBoxEdit))
		{
			color = ThemeEngine.Current.ColorHighlightText;
			int num2 = (int)graphics.MeasureString(dirComboBoxItem.Name, e.Font).Width;
			graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(ThemeEngine.Current.ColorHighlight), new Rectangle(num + 23, 1, num2 + 3, e.Bounds.Height - 2));
			if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
			{
				ControlPaint.DrawFocusRectangle(graphics, new Rectangle(num + 22, 0, num2 + 5, e.Bounds.Height), color, ThemeEngine.Current.ColorHighlight);
			}
		}
		graphics.DrawString(dirComboBoxItem.Name, e.Font, ThemeEngine.Current.ResPool.GetSolidBrush(color), new Point(24 + num, (bitmap.Height - e.Font.Height) / 2));
		graphics.DrawImage(dirComboBoxItem.ImageList.Images[dirComboBoxItem.ImageIndex], new Rectangle(new Point(num + 2, 0), new Size(16, 16)));
		e.Graphics.DrawImage(bitmap, e.Bounds.X, e.Bounds.Y);
		graphics.Dispose();
		bitmap.Dispose();
	}

	protected override void OnSelectedIndexChanged(EventArgs e)
	{
		if (base.Items.Count > 0)
		{
			DirComboBoxItem dirComboBoxItem = base.Items[SelectedIndex] as DirComboBoxItem;
			currentPath = dirComboBoxItem.Path;
		}
	}

	protected override void OnSelectionChangeCommitted(EventArgs e)
	{
		((EventHandler)base.Events[CDirectoryChangedEvent])?.Invoke(this, EventArgs.Empty);
	}
}

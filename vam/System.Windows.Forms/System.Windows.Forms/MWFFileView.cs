using System.Collections;
using System.Collections.Specialized;
using System.IO;

namespace System.Windows.Forms;

internal class MWFFileView : ListView
{
	private ArrayList filterArrayList;

	private bool showHiddenFiles;

	private string selectedFilesString;

	private int filterIndex = 1;

	private ToolTip toolTip;

	private int oldItemIndexForToolTip = -1;

	private ContextMenu contextMenu;

	private MenuItem menuItemView;

	private MenuItem menuItemNew;

	private MenuItem smallIconMenutItem;

	private MenuItem tilesMenutItem;

	private MenuItem largeIconMenutItem;

	private MenuItem listMenutItem;

	private MenuItem detailsMenutItem;

	private MenuItem newFolderMenuItem;

	private MenuItem showHiddenFilesMenuItem;

	private int previousCheckedMenuItemIndex;

	private ArrayList viewMenuItemClones = new ArrayList();

	private FSEntry currentFSEntry;

	private string currentFolder;

	private string currentRealFolder;

	private FSEntry currentFolderFSEntry;

	private Stack directoryStack = new Stack();

	private ArrayList dirStackControlsOrComponents = new ArrayList();

	private ToolBarButton folderUpToolBarButton;

	private ArrayList registered_senders = new ArrayList();

	private bool should_push = true;

	private MWFVFS vfs;

	private View old_view;

	private int old_menuitem_index;

	private bool do_update_view;

	private ColumnHeader[] columns;

	private static object MSelectedFileChangedEvent = new object();

	private static object MSelectedFilesChangedEvent = new object();

	private static object MDirectoryChangedEvent = new object();

	private static object MForceDialogEndEvent = new object();

	public string CurrentFolder
	{
		get
		{
			return currentFolder;
		}
		set
		{
			currentFolder = value;
		}
	}

	public string CurrentRealFolder => currentRealFolder;

	public FSEntry CurrentFSEntry => currentFSEntry;

	public MenuItem[] ViewMenuItems
	{
		get
		{
			MenuItem[] array = new MenuItem[5]
			{
				smallIconMenutItem.CloneMenu(),
				tilesMenutItem.CloneMenu(),
				largeIconMenutItem.CloneMenu(),
				listMenutItem.CloneMenu(),
				detailsMenutItem.CloneMenu()
			};
			viewMenuItemClones.Add(array);
			return array;
		}
	}

	public ArrayList FilterArrayList
	{
		get
		{
			return filterArrayList;
		}
		set
		{
			filterArrayList = value;
		}
	}

	public bool ShowHiddenFiles
	{
		get
		{
			return showHiddenFiles;
		}
		set
		{
			showHiddenFiles = value;
		}
	}

	public int FilterIndex
	{
		get
		{
			return filterIndex;
		}
		set
		{
			filterIndex = value;
			if (base.Visible)
			{
				UpdateFileView();
			}
		}
	}

	public string SelectedFilesString
	{
		get
		{
			return selectedFilesString;
		}
		set
		{
			selectedFilesString = value;
		}
	}

	public event EventHandler SelectedFileChanged
	{
		add
		{
			base.Events.AddHandler(MSelectedFileChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(MSelectedFileChangedEvent, value);
		}
	}

	public event EventHandler SelectedFilesChanged
	{
		add
		{
			base.Events.AddHandler(MSelectedFilesChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(MSelectedFilesChangedEvent, value);
		}
	}

	public event EventHandler DirectoryChanged
	{
		add
		{
			base.Events.AddHandler(MDirectoryChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(MDirectoryChangedEvent, value);
		}
	}

	public event EventHandler ForceDialogEnd
	{
		add
		{
			base.Events.AddHandler(MForceDialogEndEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(MForceDialogEndEvent, value);
		}
	}

	public MWFFileView(MWFVFS vfs)
	{
		this.vfs = vfs;
		this.vfs.RegisterUpdateDelegate(RealFileViewUpdate, this);
		SuspendLayout();
		contextMenu = new ContextMenu();
		toolTip = new ToolTip();
		toolTip.InitialDelay = 300;
		toolTip.ReshowDelay = 0;
		menuItemView = new MenuItem("View");
		smallIconMenutItem = new MenuItem("Small Icon", OnClickViewMenuSubItem);
		smallIconMenutItem.RadioCheck = true;
		menuItemView.MenuItems.Add(smallIconMenutItem);
		tilesMenutItem = new MenuItem("Tiles", OnClickViewMenuSubItem);
		tilesMenutItem.RadioCheck = true;
		menuItemView.MenuItems.Add(tilesMenutItem);
		largeIconMenutItem = new MenuItem("Large Icon", OnClickViewMenuSubItem);
		largeIconMenutItem.RadioCheck = true;
		menuItemView.MenuItems.Add(largeIconMenutItem);
		listMenutItem = new MenuItem("List", OnClickViewMenuSubItem);
		listMenutItem.RadioCheck = true;
		listMenutItem.Checked = true;
		menuItemView.MenuItems.Add(listMenutItem);
		previousCheckedMenuItemIndex = listMenutItem.Index;
		detailsMenutItem = new MenuItem("Details", OnClickViewMenuSubItem);
		detailsMenutItem.RadioCheck = true;
		menuItemView.MenuItems.Add(detailsMenutItem);
		contextMenu.MenuItems.Add(menuItemView);
		contextMenu.MenuItems.Add(new MenuItem("-"));
		menuItemNew = new MenuItem("New");
		newFolderMenuItem = new MenuItem("New Folder", OnClickNewFolderMenuItem);
		menuItemNew.MenuItems.Add(newFolderMenuItem);
		contextMenu.MenuItems.Add(menuItemNew);
		contextMenu.MenuItems.Add(new MenuItem("-"));
		showHiddenFilesMenuItem = new MenuItem("Show hidden files", OnClickContextMenu);
		showHiddenFilesMenuItem.Checked = showHiddenFiles;
		contextMenu.MenuItems.Add(showHiddenFilesMenuItem);
		base.LabelWrap = true;
		base.SmallImageList = MimeIconEngine.SmallIcons;
		base.LargeImageList = MimeIconEngine.LargeIcons;
		base.View = (old_view = View.List);
		base.LabelEdit = true;
		ContextMenu = contextMenu;
		columns = new ColumnHeader[4];
		columns[0] = CreateColumnHeader(" Name", 170, HorizontalAlignment.Left);
		columns[1] = CreateColumnHeader("Size ", 80, HorizontalAlignment.Right);
		columns[2] = CreateColumnHeader(" Type", 100, HorizontalAlignment.Left);
		columns[3] = CreateColumnHeader(" Last Access", 150, HorizontalAlignment.Left);
		base.AllowColumnReorder = true;
		ResumeLayout(performLayout: false);
		base.KeyDown += MWF_KeyDown;
	}

	private ColumnHeader CreateColumnHeader(string text, int width, HorizontalAlignment alignment)
	{
		ColumnHeader columnHeader = new ColumnHeader();
		columnHeader.Text = text;
		columnHeader.Width = width;
		columnHeader.TextAlign = alignment;
		return columnHeader;
	}

	public void PushDir()
	{
		if (currentFolder != null)
		{
			directoryStack.Push(currentFolder);
		}
		EnableOrDisableDirstackObjects();
	}

	public void PopDir()
	{
		PopDir(null);
	}

	public void PopDir(string filter)
	{
		if (directoryStack.Count != 0)
		{
			string folder = directoryStack.Pop() as string;
			EnableOrDisableDirstackObjects();
			should_push = false;
			ChangeDirectory(null, folder, filter);
		}
	}

	public void RegisterSender(IUpdateFolder iud)
	{
		registered_senders.Add(iud);
	}

	public void CreateNewFolder()
	{
		if (currentFolder == MWFVFS.MyComputerPrefix || currentFolder == MWFVFS.RecentlyUsedPrefix)
		{
			return;
		}
		FSEntry fSEntry = new FSEntry();
		fSEntry.Attributes = FileAttributes.Directory;
		fSEntry.FileType = FSEntry.FSEntryType.Directory;
		fSEntry.IconIndex = MimeIconEngine.GetIconIndexForMimeType("inode/directory");
		fSEntry.LastAccessTime = DateTime.Now;
		TextEntryDialog textEntryDialog = new TextEntryDialog();
		textEntryDialog.IconPictureBoxImage = MimeIconEngine.LargeIcons.Images.GetImage(fSEntry.IconIndex);
		string empty = string.Empty;
		empty = ((currentFolderFSEntry.RealName == null) ? currentFolder : currentFolderFSEntry.RealName);
		string text = "New Folder";
		if (Directory.Exists(Path.Combine(empty, text)))
		{
			int num = 1;
			text = ((!XplatUI.RunningOnUnix) ? (text + " (" + num + ")") : (text + "-" + num));
			while (Directory.Exists(Path.Combine(empty, text)))
			{
				num++;
				text = ((!XplatUI.RunningOnUnix) ? ("New Folder (" + num + ")") : ("New Folder-" + num));
			}
		}
		textEntryDialog.FileName = text;
		if (textEntryDialog.ShowDialog() == DialogResult.OK)
		{
			string text2 = Path.Combine(empty, textEntryDialog.FileName);
			if (vfs.CreateFolder(text2))
			{
				fSEntry.FullName = text2;
				fSEntry.Name = textEntryDialog.FileName;
				FileViewListViewItem fileViewListViewItem = new FileViewListViewItem(fSEntry);
				BeginUpdate();
				base.Items.Add(fileViewListViewItem);
				EndUpdate();
				fileViewListViewItem.EnsureVisible();
			}
		}
	}

	public void SetSelectedIndexTo(string fname)
	{
		foreach (FileViewListViewItem item in base.Items)
		{
			if (item.Text == fname)
			{
				BeginUpdate();
				base.SelectedItems.Clear();
				item.Selected = true;
				EndUpdate();
				break;
			}
		}
	}

	public void OneDirUp()
	{
		OneDirUp(null);
	}

	public void OneDirUp(string filter)
	{
		string text = vfs.GetParent();
		if (text != null)
		{
			ChangeDirectory(null, text, filter);
		}
	}

	public void ChangeDirectory(object sender, string folder)
	{
		ChangeDirectory(sender, folder, null);
	}

	public void ChangeDirectory(object sender, string folder, string filter)
	{
		if (folder == MWFVFS.DesktopPrefix || folder == MWFVFS.RecentlyUsedPrefix)
		{
			folderUpToolBarButton.Enabled = false;
		}
		else
		{
			folderUpToolBarButton.Enabled = true;
		}
		foreach (IUpdateFolder registered_sender in registered_senders)
		{
			registered_sender.CurrentFolder = folder;
		}
		if (should_push)
		{
			PushDir();
		}
		else
		{
			should_push = true;
		}
		currentFolderFSEntry = vfs.ChangeDirectory(folder);
		currentFolder = folder;
		if (currentFolder.IndexOf("://") != -1)
		{
			currentRealFolder = currentFolderFSEntry.RealName;
		}
		else
		{
			currentRealFolder = currentFolder;
		}
		BeginUpdate();
		base.Items.Clear();
		base.SelectedItems.Clear();
		if (folder == MWFVFS.RecentlyUsedPrefix)
		{
			old_view = base.View;
			base.View = View.Details;
			old_menuitem_index = previousCheckedMenuItemIndex;
			UpdateMenuItems(detailsMenutItem);
			do_update_view = true;
		}
		else if (base.View != old_view && do_update_view)
		{
			UpdateMenuItems(menuItemView.MenuItems[old_menuitem_index]);
			base.View = old_view;
			do_update_view = false;
		}
		EndUpdate();
		try
		{
			UpdateFileView(filter);
		}
		catch (Exception ex)
		{
			if (should_push)
			{
				PopDir();
			}
			MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}

	public void UpdateFileView()
	{
		UpdateFileView(null);
	}

	public void UpdateFileView(string custom_filter)
	{
		if (custom_filter != null)
		{
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(custom_filter);
			vfs.GetFolderContent(stringCollection);
		}
		else if (filterArrayList != null && filterArrayList.Count != 0)
		{
			FilterStruct filterStruct = (FilterStruct)filterArrayList[filterIndex - 1];
			vfs.GetFolderContent(filterStruct.filters);
		}
		else
		{
			vfs.GetFolderContent();
		}
	}

	public void RealFileViewUpdate(ArrayList directoriesArrayList, ArrayList fileArrayList)
	{
		BeginUpdate();
		base.Items.Clear();
		base.SelectedItems.Clear();
		foreach (FSEntry directoriesArray in directoriesArrayList)
		{
			if (ShowHiddenFiles || (!directoriesArray.Name.StartsWith(".") && (directoriesArray.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden))
			{
				FileViewListViewItem value = new FileViewListViewItem(directoriesArray);
				base.Items.Add(value);
			}
		}
		StringCollection stringCollection = new StringCollection();
		foreach (FSEntry fileArray in fileArrayList)
		{
			if (stringCollection.Contains(fileArray.Name))
			{
				string text = fileArray.Name;
				if (stringCollection.Contains(text))
				{
					int num = 1;
					while (stringCollection.Contains(text + "(" + num + ")"))
					{
						num++;
					}
					text = text + "(" + num + ")";
				}
				fileArray.Name = text;
			}
			stringCollection.Add(fileArray.Name);
			DoOneFSEntry(fileArray);
		}
		EndUpdate();
		stringCollection.Clear();
		stringCollection = null;
		directoriesArrayList.Clear();
		fileArrayList.Clear();
	}

	public void AddControlToEnableDisableByDirStack(object control)
	{
		dirStackControlsOrComponents.Add(control);
	}

	public void SetFolderUpToolBarButton(ToolBarButton tb)
	{
		folderUpToolBarButton = tb;
	}

	public void WriteRecentlyUsed(string fullfilename)
	{
		vfs.WriteRecentlyUsedFiles(fullfilename);
	}

	private void EnableOrDisableDirstackObjects()
	{
		foreach (object dirStackControlsOrComponent in dirStackControlsOrComponents)
		{
			if (dirStackControlsOrComponent is Control)
			{
				Control control = dirStackControlsOrComponent as Control;
				control.Enabled = directoryStack.Count > 1;
			}
			else if (dirStackControlsOrComponent is ToolBarButton)
			{
				ToolBarButton toolBarButton = dirStackControlsOrComponent as ToolBarButton;
				toolBarButton.Enabled = directoryStack.Count > 0;
			}
		}
	}

	private void DoOneFSEntry(FSEntry fsEntry)
	{
		if (ShowHiddenFiles || (!fsEntry.Name.StartsWith(".") && (fsEntry.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden))
		{
			FileViewListViewItem value = new FileViewListViewItem(fsEntry);
			base.Items.Add(value);
		}
	}

	private void MWF_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Back)
		{
			OneDirUp();
		}
		else
		{
			if (!e.Control || e.KeyCode != Keys.A || !base.MultiSelect)
			{
				return;
			}
			foreach (ListViewItem item in base.Items)
			{
				item.Selected = true;
			}
		}
	}

	internal void PerformClick()
	{
		OnClick(EventArgs.Empty);
	}

	internal void PerformDoubleClick()
	{
		OnDoubleClick(EventArgs.Empty);
	}

	protected override void OnClick(EventArgs e)
	{
		if (!base.MultiSelect && base.SelectedItems.Count > 0)
		{
			FileViewListViewItem fileViewListViewItem = base.SelectedItems[0] as FileViewListViewItem;
			FSEntry fSEntry = fileViewListViewItem.FSEntry;
			if (fSEntry.FileType == FSEntry.FSEntryType.File)
			{
				currentFSEntry = fSEntry;
				((EventHandler)base.Events[MSelectedFileChangedEvent])?.Invoke(this, EventArgs.Empty);
			}
		}
		base.OnClick(e);
	}

	protected override void OnDoubleClick(EventArgs e)
	{
		if (base.SelectedItems.Count > 0)
		{
			FileViewListViewItem fileViewListViewItem = base.SelectedItems[0] as FileViewListViewItem;
			FSEntry fSEntry = fileViewListViewItem.FSEntry;
			if ((fSEntry.Attributes & FileAttributes.Directory) != FileAttributes.Directory)
			{
				currentFSEntry = fSEntry;
				((EventHandler)base.Events[MSelectedFileChangedEvent])?.Invoke(this, EventArgs.Empty);
				((EventHandler)base.Events[MForceDialogEndEvent])?.Invoke(this, EventArgs.Empty);
				return;
			}
			ChangeDirectory(null, fSEntry.FullName);
			((EventHandler)base.Events[MDirectoryChangedEvent])?.Invoke(this, EventArgs.Empty);
		}
		base.OnDoubleClick(e);
	}

	protected override void OnSelectedIndexChanged(EventArgs e)
	{
		if (base.SelectedItems.Count > 0)
		{
			selectedFilesString = string.Empty;
			if (base.SelectedItems.Count == 1)
			{
				FileViewListViewItem fileViewListViewItem = base.SelectedItems[0] as FileViewListViewItem;
				FSEntry fSEntry = fileViewListViewItem.FSEntry;
				if ((fSEntry.Attributes & FileAttributes.Directory) != FileAttributes.Directory)
				{
					selectedFilesString = base.SelectedItems[0].Text;
				}
			}
			else
			{
				foreach (FileViewListViewItem selectedItem in base.SelectedItems)
				{
					FSEntry fSEntry2 = selectedItem.FSEntry;
					if ((fSEntry2.Attributes & FileAttributes.Directory) != FileAttributes.Directory)
					{
						selectedFilesString = selectedFilesString + "\"" + selectedItem.Text + "\" ";
					}
				}
			}
			((EventHandler)base.Events[MSelectedFilesChangedEvent])?.Invoke(this, EventArgs.Empty);
		}
		base.OnSelectedIndexChanged(e);
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		if (GetItemAt(e.X, e.Y) is FileViewListViewItem fileViewListViewItem)
		{
			int index = fileViewListViewItem.Index;
			if (index != oldItemIndexForToolTip)
			{
				oldItemIndexForToolTip = index;
				if (toolTip != null && toolTip.Active)
				{
					toolTip.Active = false;
				}
				FSEntry fSEntry = fileViewListViewItem.FSEntry;
				string empty = string.Empty;
				empty = ((fSEntry.FileType == FSEntry.FSEntryType.Directory) ? ("Directory: " + fSEntry.FullName) : ((fSEntry.FileType == FSEntry.FSEntryType.Device) ? ("Device: " + fSEntry.FullName) : ((fSEntry.FileType != FSEntry.FSEntryType.Network) ? ("File: " + fSEntry.FullName) : ("Network: " + fSEntry.FullName))));
				toolTip.SetToolTip(this, empty);
				toolTip.Active = true;
			}
		}
		else
		{
			toolTip.Active = false;
		}
		base.OnMouseMove(e);
	}

	private void OnClickContextMenu(object sender, EventArgs e)
	{
		MenuItem menuItem = sender as MenuItem;
		if (menuItem == showHiddenFilesMenuItem)
		{
			menuItem.Checked = !menuItem.Checked;
			showHiddenFiles = menuItem.Checked;
			UpdateFileView();
		}
	}

	private void OnClickViewMenuSubItem(object sender, EventArgs e)
	{
		MenuItem menuItem = (MenuItem)sender;
		UpdateMenuItems(menuItem);
		BeginUpdate();
		switch (menuItem.Index)
		{
		case 0:
			base.View = View.SmallIcon;
			break;
		case 1:
			base.View = View.Tile;
			break;
		case 2:
			base.View = View.LargeIcon;
			break;
		case 3:
			base.View = View.List;
			break;
		case 4:
			base.View = View.Details;
			break;
		}
		if (base.View == View.Details)
		{
			base.Columns.AddRange(columns);
		}
		else
		{
			base.ListViewItemSorter = null;
			base.Columns.Clear();
		}
		EndUpdate();
	}

	protected override void OnBeforeLabelEdit(LabelEditEventArgs e)
	{
		FileViewListViewItem fileViewListViewItem = base.SelectedItems[0] as FileViewListViewItem;
		FSEntry fSEntry = fileViewListViewItem.FSEntry;
		if (fSEntry.FileType != FSEntry.FSEntryType.Directory && fSEntry.FileType != FSEntry.FSEntryType.File)
		{
			e.CancelEdit = true;
		}
		base.OnBeforeLabelEdit(e);
	}

	protected override void OnAfterLabelEdit(LabelEditEventArgs e)
	{
		base.OnAfterLabelEdit(e);
		if (e.Label == null || base.Items[e.Item].Text == e.Label)
		{
			return;
		}
		FileViewListViewItem fileViewListViewItem = base.SelectedItems[0] as FileViewListViewItem;
		FSEntry fSEntry = fileViewListViewItem.FSEntry;
		string path = ((currentFolderFSEntry.RealName == null) ? currentFolder : currentFolderFSEntry.RealName);
		switch (fSEntry.FileType)
		{
		case FSEntry.FSEntryType.Directory:
		{
			string sourceDirName = ((fSEntry.RealName == null) ? fSEntry.FullName : fSEntry.RealName);
			string text2 = Path.Combine(path, e.Label);
			if (!vfs.MoveFolder(sourceDirName, text2))
			{
				e.CancelEdit = true;
			}
			else if (fSEntry.RealName != null)
			{
				fSEntry.RealName = text2;
			}
			else
			{
				fSEntry.FullName = text2;
			}
			break;
		}
		case FSEntry.FSEntryType.File:
		{
			string sourceFileName = ((fSEntry.RealName == null) ? fSEntry.FullName : fSEntry.RealName);
			string text = Path.Combine(path, e.Label);
			if (!vfs.MoveFile(sourceFileName, text))
			{
				e.CancelEdit = true;
			}
			else if (fSEntry.RealName != null)
			{
				fSEntry.RealName = text;
			}
			else
			{
				fSEntry.FullName = text;
			}
			break;
		}
		}
	}

	private void UpdateMenuItems(MenuItem senderMenuItem)
	{
		menuItemView.MenuItems[previousCheckedMenuItemIndex].Checked = false;
		menuItemView.MenuItems[senderMenuItem.Index].Checked = true;
		foreach (MenuItem[] viewMenuItemClone in viewMenuItemClones)
		{
			viewMenuItemClone[previousCheckedMenuItemIndex].Checked = false;
			viewMenuItemClone[senderMenuItem.Index].Checked = true;
		}
		previousCheckedMenuItemIndex = senderMenuItem.Index;
	}

	private void OnClickNewFolderMenuItem(object sender, EventArgs e)
	{
		CreateNewFolder();
	}
}

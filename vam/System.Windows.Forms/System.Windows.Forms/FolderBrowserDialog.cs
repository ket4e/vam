using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.IO;

namespace System.Windows.Forms;

[Designer("System.Windows.Forms.Design.FolderBrowserDialogDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[DefaultEvent("HelpRequest")]
[DefaultProperty("SelectedPath")]
public sealed class FolderBrowserDialog : CommonDialog
{
	internal class FolderBrowserTreeView : TreeView
	{
		private MWFVFS vfs = new MWFVFS();

		private new FBTreeNode root_node;

		private FolderBrowserDialog parentDialog;

		private ImageList imageList = new ImageList();

		private Environment.SpecialFolder rootFolder;

		private bool dont_enable;

		private TreeNode node_under_mouse;

		private string parent_real_path;

		private bool dont_do_onbeforeexpand;

		public Environment.SpecialFolder RootFolder
		{
			set
			{
				rootFolder = value;
				string empty = string.Empty;
				switch (rootFolder)
				{
				case Environment.SpecialFolder.Desktop:
					root_node = new FBTreeNode("Desktop");
					root_node.RealPath = ThemeEngine.Current.Places(UIIcon.PlacesDesktop);
					empty = MWFVFS.DesktopPrefix;
					break;
				case Environment.SpecialFolder.Recent:
					root_node = new FBTreeNode("My Recent Documents");
					root_node.RealPath = ThemeEngine.Current.Places(UIIcon.PlacesRecentDocuments);
					empty = MWFVFS.RecentlyUsedPrefix;
					break;
				case Environment.SpecialFolder.MyComputer:
					root_node = new FBTreeNode("My Computer");
					empty = MWFVFS.MyComputerPrefix;
					break;
				case Environment.SpecialFolder.MyDocuments:
					root_node = new FBTreeNode("Personal");
					empty = MWFVFS.PersonalPrefix;
					root_node.RealPath = ThemeEngine.Current.Places(UIIcon.PlacesPersonal);
					break;
				default:
					root_node = new FBTreeNode(rootFolder.ToString());
					root_node.RealPath = Environment.GetFolderPath(rootFolder);
					empty = root_node.RealPath;
					break;
				}
				root_node.Tag = empty;
				root_node.ImageIndex = NodeImageIndex(empty);
				BeginUpdate();
				base.Nodes.Clear();
				EndUpdate();
				FillNode(root_node);
				root_node.Expand();
				base.Nodes.Add(root_node);
			}
		}

		public string SelectedPath
		{
			set
			{
				if (value.Length == 0 || !Path.IsPathRooted(value))
				{
					return;
				}
				try
				{
					if (Check_if_path_is_child_of_RootFolder(value))
					{
						SetSelectedPath(Path.GetFullPath(value));
					}
				}
				catch (Exception)
				{
					EndUpdate();
					RootFolder = rootFolder;
				}
			}
		}

		public FolderBrowserTreeView(FolderBrowserDialog parent_dialog)
		{
			parentDialog = parent_dialog;
			base.HideSelection = false;
			base.ImageList = imageList;
			SetupImageList();
		}

		public void CreateNewFolder()
		{
			FBTreeNode fBTreeNode = ((node_under_mouse != null) ? (node_under_mouse as FBTreeNode) : (base.SelectedNode as FBTreeNode));
			if (fBTreeNode == null || fBTreeNode.RealPath == null)
			{
				return;
			}
			string text = "New Folder";
			if (Directory.Exists(Path.Combine(fBTreeNode.RealPath, text)))
			{
				int num = 1;
				text = ((!XplatUI.RunningOnUnix) ? (text + " (" + num + ")") : (text + "-" + num));
				while (Directory.Exists(Path.Combine(fBTreeNode.RealPath, text)))
				{
					num++;
					text = ((!XplatUI.RunningOnUnix) ? ("New Folder (" + num + ")") : ("New Folder-" + num));
				}
			}
			parent_real_path = fBTreeNode.RealPath;
			FillNode(fBTreeNode);
			dont_do_onbeforeexpand = true;
			fBTreeNode.Expand();
			dont_do_onbeforeexpand = false;
			string text2 = Path.Combine(fBTreeNode.RealPath, text);
			if (vfs.CreateFolder(text2))
			{
				FBTreeNode fBTreeNode2 = new FBTreeNode(text);
				fBTreeNode2.ImageIndex = NodeImageIndex(text);
				string tag = (fBTreeNode2.RealPath = text2);
				fBTreeNode2.Tag = tag;
				fBTreeNode.Nodes.Add(fBTreeNode2);
				base.LabelEdit = true;
				fBTreeNode2.BeginEdit();
			}
		}

		protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e)
		{
			if (e.Label != null)
			{
				if (e.Label.Length <= 0)
				{
					e.CancelEdit = true;
					e.Node.BeginEdit();
					return;
				}
				FBTreeNode fBTreeNode = e.Node as FBTreeNode;
				string realPath = fBTreeNode.RealPath;
				string text = Path.Combine(parent_real_path, e.Label);
				if (!vfs.MoveFolder(realPath, text))
				{
					e.CancelEdit = true;
					e.Node.BeginEdit();
					return;
				}
				string tag = (fBTreeNode.RealPath = text);
				fBTreeNode.Tag = tag;
			}
			if (node_under_mouse == base.SelectedNode)
			{
				base.SelectedNode = e.Node;
			}
			base.LabelEdit = false;
		}

		private void SetSelectedPath(string path)
		{
			BeginUpdate();
			FBTreeNode fBTreeNode = FindPathInNodes(path, base.Nodes);
			if (fBTreeNode == null)
			{
				Stack stack = new Stack();
				string text = path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar));
				if (!XplatUI.RunningOnUnix && text.Length == 2)
				{
					text += Path.DirectorySeparatorChar;
				}
				while (fBTreeNode == null && text.Length > 0)
				{
					fBTreeNode = FindPathInNodes(text, base.Nodes);
					if (fBTreeNode == null)
					{
						string text2 = text.Substring(0, text.LastIndexOf(Path.DirectorySeparatorChar));
						string obj = text.Replace(text2, string.Empty);
						stack.Push(obj);
						text = text2;
					}
				}
				if (fBTreeNode == null)
				{
					EndUpdate();
					RootFolder = rootFolder;
					return;
				}
				FillNode(fBTreeNode);
				fBTreeNode.Expand();
				while (stack.Count > 0)
				{
					string text3 = stack.Pop() as string;
					foreach (TreeNode node in fBTreeNode.Nodes)
					{
						FBTreeNode fBTreeNode2 = node as FBTreeNode;
						if (text + text3 == fBTreeNode2.RealPath)
						{
							fBTreeNode = fBTreeNode2;
							text += text3;
							FillNode(fBTreeNode);
							fBTreeNode.Expand();
							break;
						}
					}
				}
				foreach (TreeNode node2 in fBTreeNode.Nodes)
				{
					FBTreeNode fBTreeNode3 = node2 as FBTreeNode;
					if (path == fBTreeNode3.RealPath)
					{
						fBTreeNode = fBTreeNode3;
						break;
					}
				}
			}
			if (fBTreeNode != null)
			{
				base.SelectedNode = fBTreeNode;
				fBTreeNode.EnsureVisible();
			}
			EndUpdate();
		}

		private FBTreeNode FindPathInNodes(string path, TreeNodeCollection nodes)
		{
			if (!XplatUI.RunningOnUnix && path.Length == 2)
			{
				path += Path.DirectorySeparatorChar;
			}
			foreach (TreeNode node in nodes)
			{
				if (node is FBTreeNode fBTreeNode && fBTreeNode.RealPath != null && fBTreeNode.RealPath == path)
				{
					return fBTreeNode;
				}
				FBTreeNode fBTreeNode2 = FindPathInNodes(path, node.Nodes);
				if (fBTreeNode2 != null)
				{
					return fBTreeNode2;
				}
			}
			return null;
		}

		private bool Check_if_path_is_child_of_RootFolder(string path)
		{
			string realPath = root_node.RealPath;
			if (realPath != null || rootFolder == Environment.SpecialFolder.MyComputer)
			{
				try
				{
					if (!Directory.Exists(path))
					{
						return false;
					}
					switch (rootFolder)
					{
					case Environment.SpecialFolder.Desktop:
					case Environment.SpecialFolder.MyComputer:
						return true;
					case Environment.SpecialFolder.MyDocuments:
						if (!path.StartsWith(realPath))
						{
							return false;
						}
						return true;
					default:
						return false;
					}
				}
				catch
				{
				}
			}
			return false;
		}

		private void FillNode(TreeNode node)
		{
			BeginUpdate();
			node.Nodes.Clear();
			vfs.ChangeDirectory((string)node.Tag);
			ArrayList foldersOnly = vfs.GetFoldersOnly();
			foreach (FSEntry item in foldersOnly)
			{
				if (item.Name.StartsWith("."))
				{
					continue;
				}
				FBTreeNode fBTreeNode = new FBTreeNode(item.Name);
				fBTreeNode.Tag = item.FullName;
				fBTreeNode.RealPath = ((item.RealName != null) ? item.RealName : item.FullName);
				fBTreeNode.ImageIndex = NodeImageIndex(item.FullName);
				vfs.ChangeDirectory(item.FullName);
				ArrayList foldersOnly2 = vfs.GetFoldersOnly();
				foreach (FSEntry item2 in foldersOnly2)
				{
					if (!item2.Name.StartsWith("."))
					{
						fBTreeNode.Nodes.Add(new TreeNode(string.Empty));
						break;
					}
				}
				node.Nodes.Add(fBTreeNode);
			}
			EndUpdate();
		}

		private void SetupImageList()
		{
			imageList.ColorDepth = ColorDepth.Depth32Bit;
			imageList.ImageSize = new Size(16, 16);
			imageList.Images.Add(ThemeEngine.Current.Images(UIIcon.PlacesRecentDocuments, 16));
			imageList.Images.Add(ThemeEngine.Current.Images(UIIcon.PlacesDesktop, 16));
			imageList.Images.Add(ThemeEngine.Current.Images(UIIcon.PlacesPersonal, 16));
			imageList.Images.Add(ThemeEngine.Current.Images(UIIcon.PlacesMyComputer, 16));
			imageList.Images.Add(ThemeEngine.Current.Images(UIIcon.PlacesMyNetwork, 16));
			imageList.Images.Add(ThemeEngine.Current.Images(UIIcon.NormalFolder, 16));
			imageList.TransparentColor = Color.Transparent;
		}

		private int NodeImageIndex(string path)
		{
			int result = 5;
			if (path == MWFVFS.DesktopPrefix)
			{
				result = 1;
			}
			else if (path == MWFVFS.RecentlyUsedPrefix)
			{
				result = 0;
			}
			else if (path == MWFVFS.PersonalPrefix)
			{
				result = 2;
			}
			else if (path == MWFVFS.MyComputerPrefix)
			{
				result = 3;
			}
			else if (path == MWFVFS.MyNetworkPrefix)
			{
				result = 4;
			}
			return result;
		}

		protected override void OnAfterSelect(TreeViewEventArgs e)
		{
			if (e.Node != null)
			{
				FBTreeNode fBTreeNode = e.Node as FBTreeNode;
				if (fBTreeNode.RealPath == null || fBTreeNode.RealPath.IndexOf("://") != -1)
				{
					parentDialog.okButton.Enabled = false;
					parentDialog.newFolderButton.Enabled = false;
					parentDialog.newFolderMenuItem.Enabled = false;
					dont_enable = true;
				}
				else
				{
					parentDialog.okButton.Enabled = true;
					parentDialog.newFolderButton.Enabled = true;
					parentDialog.newFolderMenuItem.Enabled = true;
					parentDialog.selectedPath = fBTreeNode.RealPath;
					dont_enable = false;
				}
				base.OnAfterSelect(e);
			}
		}

		protected internal override void OnBeforeExpand(TreeViewCancelEventArgs e)
		{
			if (!dont_do_onbeforeexpand)
			{
				if (e.Node == root_node)
				{
					return;
				}
				FillNode(e.Node);
			}
			base.OnBeforeExpand(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			node_under_mouse = GetNodeAt(e.X, e.Y);
			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (base.SelectedNode == null)
			{
				parentDialog.okButton.Enabled = false;
				parentDialog.newFolderButton.Enabled = false;
				parentDialog.newFolderMenuItem.Enabled = false;
			}
			else if (!dont_enable)
			{
				parentDialog.okButton.Enabled = true;
				parentDialog.newFolderButton.Enabled = true;
				parentDialog.newFolderMenuItem.Enabled = true;
			}
			node_under_mouse = null;
			base.OnMouseUp(e);
		}
	}

	internal class FBTreeNode : TreeNode
	{
		private string realPath;

		public string RealPath
		{
			get
			{
				return realPath;
			}
			set
			{
				realPath = value;
			}
		}

		public FBTreeNode(string text)
		{
			base.Text = text;
		}
	}

	private Environment.SpecialFolder rootFolder;

	private string selectedPath = string.Empty;

	private bool showNewFolderButton = true;

	private Label descriptionLabel;

	private Button cancelButton;

	private Button okButton;

	private FolderBrowserTreeView folderBrowserTreeView;

	private Button newFolderButton;

	private ContextMenu folderBrowserTreeViewContextMenu;

	private MenuItem newFolderMenuItem;

	private string old_selectedPath = string.Empty;

	private readonly string folderbrowserdialog_string = "FolderBrowserDialog";

	private readonly string width_string = "Width";

	private readonly string height_string = "Height";

	private readonly string x_string = "X";

	private readonly string y_string = "Y";

	[Localizable(true)]
	[DefaultValue("")]
	[Browsable(true)]
	public string Description
	{
		get
		{
			return descriptionLabel.Text;
		}
		set
		{
			descriptionLabel.Text = value;
		}
	}

	[TypeConverter(typeof(SpecialFolderEnumConverter))]
	[Localizable(false)]
	[DefaultValue(Environment.SpecialFolder.Desktop)]
	[Browsable(true)]
	public Environment.SpecialFolder RootFolder
	{
		get
		{
			return rootFolder;
		}
		set
		{
			Type typeFromHandle = typeof(Environment.SpecialFolder);
			if (!Enum.IsDefined(typeFromHandle, (int)value))
			{
				throw new InvalidEnumArgumentException("value", (int)value, typeFromHandle);
			}
			rootFolder = value;
		}
	}

	[DefaultValue("")]
	[Localizable(true)]
	[Editor("System.Windows.Forms.Design.SelectedPathEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[Browsable(true)]
	public string SelectedPath
	{
		get
		{
			return selectedPath;
		}
		set
		{
			if (value == null)
			{
				value = string.Empty;
			}
			selectedPath = value;
			old_selectedPath = value;
		}
	}

	[DefaultValue(true)]
	[Localizable(false)]
	[Browsable(true)]
	public bool ShowNewFolderButton
	{
		get
		{
			return showNewFolderButton;
		}
		set
		{
			if (value != showNewFolderButton)
			{
				newFolderButton.Visible = value;
				showNewFolderButton = value;
			}
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler HelpRequest
	{
		add
		{
			base.HelpRequest += value;
		}
		remove
		{
			base.HelpRequest -= value;
		}
	}

	public FolderBrowserDialog()
	{
		form = new DialogForm(this);
		Size size = Size.Empty;
		Point point = Point.Empty;
		object value = MWFConfig.GetValue(folderbrowserdialog_string, width_string);
		object value2 = MWFConfig.GetValue(folderbrowserdialog_string, height_string);
		if (value2 != null && value != null)
		{
			size = new Size((int)value, (int)value2);
		}
		object value3 = MWFConfig.GetValue(folderbrowserdialog_string, x_string);
		object value4 = MWFConfig.GetValue(folderbrowserdialog_string, y_string);
		if (value3 != null && value4 != null)
		{
			point = new Point((int)value3, (int)value4);
		}
		newFolderButton = new Button();
		folderBrowserTreeView = new FolderBrowserTreeView(this);
		okButton = new Button();
		cancelButton = new Button();
		descriptionLabel = new Label();
		folderBrowserTreeViewContextMenu = new ContextMenu();
		form.AcceptButton = okButton;
		form.CancelButton = cancelButton;
		form.SuspendLayout();
		form.ClientSize = new Size(322, 324);
		form.MinimumSize = new Size(310, 254);
		form.Text = "Browse For Folder";
		form.SizeGripStyle = SizeGripStyle.Show;
		newFolderMenuItem = new MenuItem("New Folder", OnClickNewFolderButton);
		folderBrowserTreeViewContextMenu.MenuItems.Add(newFolderMenuItem);
		descriptionLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		descriptionLabel.Location = new Point(15, 14);
		descriptionLabel.Size = new Size(292, 40);
		descriptionLabel.TabIndex = 0;
		descriptionLabel.Text = string.Empty;
		folderBrowserTreeView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		folderBrowserTreeView.ImageIndex = -1;
		folderBrowserTreeView.Location = new Point(15, 60);
		folderBrowserTreeView.SelectedImageIndex = -1;
		folderBrowserTreeView.Size = new Size(292, 212);
		folderBrowserTreeView.TabIndex = 3;
		folderBrowserTreeView.ShowLines = false;
		folderBrowserTreeView.ShowPlusMinus = true;
		folderBrowserTreeView.HotTracking = true;
		folderBrowserTreeView.BorderStyle = BorderStyle.Fixed3D;
		folderBrowserTreeView.ContextMenu = folderBrowserTreeViewContextMenu;
		newFolderButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
		newFolderButton.FlatStyle = FlatStyle.System;
		newFolderButton.Location = new Point(15, 285);
		newFolderButton.Size = new Size(105, 23);
		newFolderButton.TabIndex = 4;
		newFolderButton.Text = "Make New Folder";
		newFolderButton.Enabled = true;
		okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		okButton.FlatStyle = FlatStyle.System;
		okButton.Location = new Point(135, 285);
		okButton.Size = new Size(80, 23);
		okButton.TabIndex = 1;
		okButton.Text = "OK";
		cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		cancelButton.DialogResult = DialogResult.Cancel;
		cancelButton.FlatStyle = FlatStyle.System;
		cancelButton.Location = new Point(227, 285);
		cancelButton.Size = new Size(80, 23);
		cancelButton.TabIndex = 2;
		cancelButton.Text = "Cancel";
		form.Controls.Add(cancelButton);
		form.Controls.Add(okButton);
		form.Controls.Add(newFolderButton);
		form.Controls.Add(folderBrowserTreeView);
		form.Controls.Add(descriptionLabel);
		form.ResumeLayout(performLayout: false);
		if (size != Size.Empty)
		{
			form.Size = size;
		}
		if (point != Point.Empty)
		{
			form.Location = point;
		}
		okButton.Click += OnClickOKButton;
		cancelButton.Click += OnClickCancelButton;
		newFolderButton.Click += OnClickNewFolderButton;
		form.VisibleChanged += OnFormVisibleChanged;
		RootFolder = rootFolder;
	}

	public override void Reset()
	{
		Description = string.Empty;
		RootFolder = Environment.SpecialFolder.Desktop;
		selectedPath = string.Empty;
		ShowNewFolderButton = true;
	}

	protected override bool RunDialog(IntPtr hWndOwner)
	{
		folderBrowserTreeView.RootFolder = RootFolder;
		folderBrowserTreeView.SelectedPath = SelectedPath;
		form.Refresh();
		return true;
	}

	private void OnClickOKButton(object sender, EventArgs e)
	{
		WriteConfigValues();
		form.DialogResult = DialogResult.OK;
	}

	private void OnClickCancelButton(object sender, EventArgs e)
	{
		WriteConfigValues();
		selectedPath = old_selectedPath;
		form.DialogResult = DialogResult.Cancel;
	}

	private void OnClickNewFolderButton(object sender, EventArgs e)
	{
		folderBrowserTreeView.CreateNewFolder();
	}

	private void OnFormVisibleChanged(object sender, EventArgs e)
	{
		if (form.Visible && okButton.Enabled)
		{
			okButton.Select();
		}
	}

	private void WriteConfigValues()
	{
		MWFConfig.SetValue(folderbrowserdialog_string, width_string, form.Width);
		MWFConfig.SetValue(folderbrowserdialog_string, height_string, form.Height);
		MWFConfig.SetValue(folderbrowserdialog_string, x_string, form.Location.X);
		MWFConfig.SetValue(folderbrowserdialog_string, y_string, form.Location.Y);
	}
}

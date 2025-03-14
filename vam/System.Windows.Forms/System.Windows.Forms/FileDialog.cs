using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;

namespace System.Windows.Forms;

[DefaultProperty("FileName")]
[DefaultEvent("FileOk")]
public abstract class FileDialog : CommonDialog
{
	internal enum FileDialogType
	{
		OpenFileDialog,
		SaveFileDialog
	}

	private class FileNamesTokenizer
	{
		private readonly bool _allowMultiple;

		private int _position;

		private readonly string _text;

		private TokenType _tokenType;

		private string _tokenText;

		public TokenType CurrentToken => _tokenType;

		public string TokenText => _tokenText;

		public bool AllowMultiple => _allowMultiple;

		public FileNamesTokenizer(string text, bool allowMultiple)
		{
			_text = text;
			_position = 0;
			_tokenType = TokenType.BOF;
			_allowMultiple = allowMultiple;
		}

		private int ReadChar()
		{
			if (_position < _text.Length)
			{
				return _text[_position++];
			}
			return -1;
		}

		private int PeekChar()
		{
			if (_position < _text.Length)
			{
				return _text[_position];
			}
			return -1;
		}

		private void SkipWhitespaceAndQuotes()
		{
			int num;
			while ((num = PeekChar()) != -1 && ((ushort)num == 34 || char.IsWhiteSpace((char)num)))
			{
				ReadChar();
			}
		}

		public void GetNextFile()
		{
			if (_tokenType == TokenType.EOF)
			{
				throw new Exception(string.Empty);
			}
			SkipWhitespaceAndQuotes();
			if (PeekChar() == -1)
			{
				_tokenType = TokenType.EOF;
				return;
			}
			_tokenType = TokenType.FileName;
			StringBuilder stringBuilder = new StringBuilder();
			int num;
			while ((num = PeekChar()) != -1)
			{
				if ((ushort)num == 34)
				{
					ReadChar();
					if (AllowMultiple)
					{
						break;
					}
					int position = _position;
					SkipWhitespaceAndQuotes();
					if (PeekChar() == -1)
					{
						break;
					}
					position = (_position = position + 1);
					stringBuilder.Append((char)num);
				}
				else
				{
					stringBuilder.Append((char)ReadChar());
				}
			}
			_tokenText = stringBuilder.ToString();
		}
	}

	internal enum TokenType
	{
		BOF,
		EOF,
		FileName
	}

	private const string filedialog_string = "FileDialog";

	private const string lastfolder_string = "LastFolder";

	private const string width_string = "Width";

	private const string height_string = "Height";

	private const string filenames_string = "FileNames";

	private const string x_string = "X";

	private const string y_string = "Y";

	protected static readonly object EventFileOk = new object();

	private static int MaxFileNameItems = 10;

	private bool addExtension = true;

	private bool checkFileExists;

	private bool checkPathExists = true;

	private string defaultExt;

	private bool dereferenceLinks = true;

	private string[] fileNames;

	private string filter = string.Empty;

	private int filterIndex = 1;

	private string initialDirectory;

	private bool restoreDirectory;

	private bool showHelp;

	private string title;

	private bool validateNames = true;

	private bool auto_upgrade_enable = true;

	private FileDialogCustomPlacesCollection custom_places;

	private bool supportMultiDottedExtensions;

	private bool checkForIllegalChars = true;

	private Button cancelButton;

	private ToolBarButton upToolBarButton;

	private PopupButtonPanel popupButtonPanel;

	private Button openSaveButton;

	private Button helpButton;

	private Label fileTypeLabel;

	private ToolBarButton menueToolBarButton;

	private ContextMenu menueToolBarButtonContextMenu;

	private ToolBar smallButtonToolBar;

	private DirComboBox dirComboBox;

	private ComboBox fileNameComboBox;

	private Label fileNameLabel;

	private MWFFileView mwfFileView;

	private MwfFileViewItemComparer file_view_comparer;

	private Label searchSaveLabel;

	private ToolBarButton newdirToolBarButton;

	private ToolBarButton backToolBarButton;

	private ComboBox fileTypeComboBox;

	private ImageList imageListTopToolbar;

	private CheckBox readonlyCheckBox;

	private bool multiSelect;

	private string restoreDirectoryString = string.Empty;

	internal FileDialogType fileDialogType;

	private bool do_not_call_OnSelectedIndexChangedFileTypeComboBox;

	private bool showReadOnly;

	private bool readOnlyChecked;

	internal bool createPrompt;

	internal bool overwritePrompt = true;

	private FileFilter fileFilter;

	private string[] configFileNames;

	private string lastFolder = string.Empty;

	private MWFVFS vfs;

	private readonly char[] wildcard_chars = new char[2] { '*', '?' };

	private bool disable_form_closed_event;

	[DefaultValue(true)]
	public bool AddExtension
	{
		get
		{
			return addExtension;
		}
		set
		{
			addExtension = value;
		}
	}

	[System.MonoTODO("Stub, value not respected")]
	[DefaultValue(true)]
	public bool AutoUpgradeEnabled
	{
		get
		{
			return auto_upgrade_enable;
		}
		set
		{
			auto_upgrade_enable = value;
		}
	}

	[DefaultValue(false)]
	public virtual bool CheckFileExists
	{
		get
		{
			return checkFileExists;
		}
		set
		{
			checkFileExists = value;
		}
	}

	[DefaultValue(true)]
	public bool CheckPathExists
	{
		get
		{
			return checkPathExists;
		}
		set
		{
			checkPathExists = value;
		}
	}

	[System.MonoTODO("Stub, collection not used")]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public FileDialogCustomPlacesCollection CustomPlaces => custom_places;

	[DefaultValue("")]
	public string DefaultExt
	{
		get
		{
			if (defaultExt == null)
			{
				return string.Empty;
			}
			return defaultExt;
		}
		set
		{
			if (value != null && value.Length > 0 && value[0] == '.')
			{
				value = value.Substring(1);
			}
			defaultExt = value;
		}
	}

	[DefaultValue(true)]
	public bool DereferenceLinks
	{
		get
		{
			return dereferenceLinks;
		}
		set
		{
			dereferenceLinks = value;
		}
	}

	[DefaultValue("")]
	public string FileName
	{
		get
		{
			if (fileNames == null || fileNames.Length == 0)
			{
				return string.Empty;
			}
			if (fileNames[0].Length == 0)
			{
				return string.Empty;
			}
			if (!checkForIllegalChars)
			{
				return fileNames[0];
			}
			Path.GetFullPath(fileNames[0]);
			return fileNames[0];
		}
		set
		{
			if (value != null)
			{
				fileNames = new string[1] { value };
			}
			else
			{
				fileNames = new string[0];
			}
			checkForIllegalChars = false;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string[] FileNames
	{
		get
		{
			if (fileNames == null || fileNames.Length == 0)
			{
				return new string[0];
			}
			string[] array = new string[fileNames.Length];
			fileNames.CopyTo(array, 0);
			if (!checkForIllegalChars)
			{
				return array;
			}
			string[] array2 = array;
			foreach (string path in array2)
			{
				Path.GetFullPath(path);
			}
			return array;
		}
	}

	[DefaultValue("")]
	[Localizable(true)]
	public string Filter
	{
		get
		{
			return filter;
		}
		set
		{
			if (value == null)
			{
				filter = string.Empty;
				if (fileFilter != null)
				{
					fileFilter.FilterArrayList.Clear();
				}
			}
			else
			{
				if (!FileFilter.CheckFilter(value))
				{
					throw new ArgumentException("The provided filter string is invalid. The filter string should contain a description of the filter, followed by the  vertical bar (|) and the filter pattern. The strings for different filtering options should also be separated by the vertical bar. Example: Text files (*.txt)|*.txt|All files (*.*)|*.*");
				}
				filter = value;
				fileFilter = new FileFilter(filter);
			}
			UpdateFilters();
		}
	}

	[DefaultValue(1)]
	public int FilterIndex
	{
		get
		{
			return filterIndex;
		}
		set
		{
			filterIndex = value;
		}
	}

	[DefaultValue("")]
	public string InitialDirectory
	{
		get
		{
			if (initialDirectory == null)
			{
				return string.Empty;
			}
			return initialDirectory;
		}
		set
		{
			initialDirectory = value;
		}
	}

	[DefaultValue(false)]
	public bool RestoreDirectory
	{
		get
		{
			return restoreDirectory;
		}
		set
		{
			restoreDirectory = value;
		}
	}

	[DefaultValue(false)]
	public bool ShowHelp
	{
		get
		{
			return showHelp;
		}
		set
		{
			showHelp = value;
			ResizeAndRelocateForHelpOrReadOnly();
		}
	}

	[DefaultValue(false)]
	public bool SupportMultiDottedExtensions
	{
		get
		{
			return supportMultiDottedExtensions;
		}
		set
		{
			supportMultiDottedExtensions = value;
		}
	}

	[DefaultValue("")]
	[Localizable(true)]
	public string Title
	{
		get
		{
			if (title == null)
			{
				return string.Empty;
			}
			return title;
		}
		set
		{
			title = value;
		}
	}

	[DefaultValue(true)]
	public bool ValidateNames
	{
		get
		{
			return validateNames;
		}
		set
		{
			validateNames = value;
		}
	}

	protected virtual IntPtr Instance
	{
		get
		{
			if (form == null)
			{
				return IntPtr.Zero;
			}
			return form.Handle;
		}
	}

	protected int Options => -1;

	internal virtual string DialogTitle => Title;

	internal virtual bool ShowReadOnly
	{
		get
		{
			return showReadOnly;
		}
		set
		{
			showReadOnly = value;
			ResizeAndRelocateForHelpOrReadOnly();
		}
	}

	internal virtual bool ReadOnlyChecked
	{
		get
		{
			return readOnlyChecked;
		}
		set
		{
			readOnlyChecked = value;
			readonlyCheckBox.Checked = value;
		}
	}

	internal bool BMultiSelect
	{
		get
		{
			return multiSelect;
		}
		set
		{
			multiSelect = value;
			mwfFileView.MultiSelect = value;
		}
	}

	internal string OpenSaveButtonText
	{
		set
		{
			openSaveButton.Text = value;
		}
	}

	internal string SearchSaveLabel
	{
		set
		{
			searchSaveLabel.Text = value;
		}
	}

	internal string FileTypeLabel
	{
		set
		{
			fileTypeLabel.Text = value;
		}
	}

	internal string CustomFilter
	{
		get
		{
			string text = fileNameComboBox.Text;
			if (text.IndexOfAny(wildcard_chars) == -1)
			{
				return null;
			}
			return text;
		}
	}

	public event CancelEventHandler FileOk
	{
		add
		{
			base.Events.AddHandler(EventFileOk, value);
		}
		remove
		{
			base.Events.RemoveHandler(EventFileOk, value);
		}
	}

	internal FileDialog()
	{
		form = new DialogForm(this);
		vfs = new MWFVFS();
		Size size = Size.Empty;
		Point point = Point.Empty;
		object value = MWFConfig.GetValue("FileDialog", "Width");
		object value2 = MWFConfig.GetValue("FileDialog", "Height");
		if (value2 != null && value != null)
		{
			size = new Size((int)value, (int)value2);
		}
		object value3 = MWFConfig.GetValue("FileDialog", "X");
		object value4 = MWFConfig.GetValue("FileDialog", "Y");
		if (value3 != null && value4 != null)
		{
			point = new Point((int)value3, (int)value4);
		}
		configFileNames = (string[])MWFConfig.GetValue("FileDialog", "FileNames");
		fileTypeComboBox = new ComboBox();
		backToolBarButton = new ToolBarButton();
		newdirToolBarButton = new ToolBarButton();
		searchSaveLabel = new Label();
		mwfFileView = new MWFFileView(vfs);
		fileNameLabel = new Label();
		fileNameComboBox = new ComboBox();
		dirComboBox = new DirComboBox(vfs);
		smallButtonToolBar = new ToolBar();
		menueToolBarButton = new ToolBarButton();
		fileTypeLabel = new Label();
		openSaveButton = new Button();
		helpButton = new Button();
		popupButtonPanel = new PopupButtonPanel();
		upToolBarButton = new ToolBarButton();
		cancelButton = new Button();
		form.CancelButton = cancelButton;
		imageListTopToolbar = new ImageList();
		menueToolBarButtonContextMenu = new ContextMenu();
		readonlyCheckBox = new CheckBox();
		form.SuspendLayout();
		imageListTopToolbar.ColorDepth = ColorDepth.Depth32Bit;
		imageListTopToolbar.ImageSize = new Size(16, 16);
		imageListTopToolbar.Images.Add(ResourceImageLoader.Get("go-previous.png"));
		imageListTopToolbar.Images.Add(ResourceImageLoader.Get("go-top.png"));
		imageListTopToolbar.Images.Add(ResourceImageLoader.Get("folder-new.png"));
		imageListTopToolbar.Images.Add(ResourceImageLoader.Get("preferences-system-windows.png"));
		imageListTopToolbar.TransparentColor = Color.Transparent;
		searchSaveLabel.FlatStyle = FlatStyle.System;
		searchSaveLabel.Location = new Point(6, 6);
		searchSaveLabel.Size = new Size(86, 22);
		searchSaveLabel.TextAlign = ContentAlignment.MiddleRight;
		dirComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		dirComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
		dirComboBox.Location = new Point(99, 6);
		dirComboBox.Size = new Size(261, 22);
		dirComboBox.TabIndex = 7;
		smallButtonToolBar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		smallButtonToolBar.Appearance = ToolBarAppearance.Flat;
		smallButtonToolBar.AutoSize = false;
		smallButtonToolBar.Buttons.AddRange(new ToolBarButton[4] { backToolBarButton, upToolBarButton, newdirToolBarButton, menueToolBarButton });
		smallButtonToolBar.ButtonSize = new Size(24, 24);
		smallButtonToolBar.Divider = false;
		smallButtonToolBar.Dock = DockStyle.None;
		smallButtonToolBar.DropDownArrows = true;
		smallButtonToolBar.ImageList = imageListTopToolbar;
		smallButtonToolBar.Location = new Point(372, 6);
		smallButtonToolBar.ShowToolTips = true;
		smallButtonToolBar.Size = new Size(140, 28);
		smallButtonToolBar.TabIndex = 8;
		smallButtonToolBar.TextAlign = ToolBarTextAlign.Right;
		popupButtonPanel.Dock = DockStyle.None;
		popupButtonPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
		popupButtonPanel.Location = new Point(6, 35);
		popupButtonPanel.Size = new Size(87, 338);
		popupButtonPanel.TabIndex = 9;
		mwfFileView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		mwfFileView.Location = new Point(99, 35);
		mwfFileView.Size = new Size(450, 283);
		mwfFileView.MultiSelect = false;
		mwfFileView.TabIndex = 10;
		mwfFileView.RegisterSender(dirComboBox);
		mwfFileView.RegisterSender(popupButtonPanel);
		fileNameLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
		fileNameLabel.FlatStyle = FlatStyle.System;
		fileNameLabel.Location = new Point(101, 326);
		fileNameLabel.Size = new Size(70, 21);
		fileNameLabel.Text = "File name:";
		fileNameLabel.TextAlign = ContentAlignment.MiddleLeft;
		fileNameComboBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		fileNameComboBox.Location = new Point(195, 326);
		fileNameComboBox.Size = new Size(246, 22);
		fileNameComboBox.TabIndex = 1;
		fileNameComboBox.MaxDropDownItems = MaxFileNameItems;
		fileNameComboBox.RestoreContextMenu();
		UpdateRecentFiles();
		fileTypeLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
		fileTypeLabel.FlatStyle = FlatStyle.System;
		fileTypeLabel.Location = new Point(101, 355);
		fileTypeLabel.Size = new Size(90, 21);
		fileTypeLabel.Text = "Files of type:";
		fileTypeLabel.TextAlign = ContentAlignment.MiddleLeft;
		fileTypeComboBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		fileTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
		fileTypeComboBox.Location = new Point(195, 355);
		fileTypeComboBox.Size = new Size(246, 22);
		fileTypeComboBox.TabIndex = 2;
		backToolBarButton.ImageIndex = 0;
		backToolBarButton.Enabled = false;
		backToolBarButton.Style = ToolBarButtonStyle.PushButton;
		mwfFileView.AddControlToEnableDisableByDirStack(backToolBarButton);
		upToolBarButton.ImageIndex = 1;
		upToolBarButton.Style = ToolBarButtonStyle.PushButton;
		mwfFileView.SetFolderUpToolBarButton(upToolBarButton);
		newdirToolBarButton.ImageIndex = 2;
		newdirToolBarButton.Style = ToolBarButtonStyle.PushButton;
		menueToolBarButton.ImageIndex = 3;
		menueToolBarButton.DropDownMenu = menueToolBarButtonContextMenu;
		menueToolBarButton.Style = ToolBarButtonStyle.DropDownButton;
		menueToolBarButtonContextMenu.MenuItems.AddRange(mwfFileView.ViewMenuItems);
		openSaveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		openSaveButton.FlatStyle = FlatStyle.System;
		openSaveButton.Location = new Point(474, 326);
		openSaveButton.Size = new Size(75, 23);
		openSaveButton.TabIndex = 4;
		openSaveButton.FlatStyle = FlatStyle.System;
		cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		cancelButton.FlatStyle = FlatStyle.System;
		cancelButton.Location = new Point(474, 353);
		cancelButton.Size = new Size(75, 23);
		cancelButton.TabIndex = 5;
		cancelButton.Text = "Cancel";
		cancelButton.FlatStyle = FlatStyle.System;
		helpButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		helpButton.FlatStyle = FlatStyle.System;
		helpButton.Location = new Point(474, 353);
		helpButton.Size = new Size(75, 23);
		helpButton.TabIndex = 6;
		helpButton.Text = "Help";
		helpButton.FlatStyle = FlatStyle.System;
		helpButton.Visible = false;
		readonlyCheckBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		readonlyCheckBox.Text = "Open Readonly";
		readonlyCheckBox.Location = new Point(195, 350);
		readonlyCheckBox.Size = new Size(245, 21);
		readonlyCheckBox.TabIndex = 3;
		readonlyCheckBox.FlatStyle = FlatStyle.System;
		readonlyCheckBox.Visible = false;
		form.SizeGripStyle = SizeGripStyle.Show;
		form.AcceptButton = openSaveButton;
		form.MaximizeBox = true;
		form.MinimizeBox = true;
		form.FormBorderStyle = FormBorderStyle.Sizable;
		form.ClientSize = new Size(555, 385);
		form.MinimumSize = form.Size;
		form.Controls.Add(smallButtonToolBar);
		form.Controls.Add(cancelButton);
		form.Controls.Add(openSaveButton);
		form.Controls.Add(mwfFileView);
		form.Controls.Add(fileTypeLabel);
		form.Controls.Add(fileNameLabel);
		form.Controls.Add(fileTypeComboBox);
		form.Controls.Add(fileNameComboBox);
		form.Controls.Add(dirComboBox);
		form.Controls.Add(searchSaveLabel);
		form.Controls.Add(popupButtonPanel);
		form.Controls.Add(helpButton);
		form.Controls.Add(readonlyCheckBox);
		form.ResumeLayout(performLayout: true);
		if (size != Size.Empty)
		{
			form.ClientSize = size;
		}
		if (point != Point.Empty)
		{
			form.Location = point;
		}
		openSaveButton.Click += OnClickOpenSaveButton;
		cancelButton.Click += OnClickCancelButton;
		helpButton.Click += OnClickHelpButton;
		smallButtonToolBar.ButtonClick += OnClickSmallButtonToolBar;
		fileTypeComboBox.SelectedIndexChanged += OnSelectedIndexChangedFileTypeComboBox;
		mwfFileView.SelectedFileChanged += OnSelectedFileChangedFileView;
		mwfFileView.ForceDialogEnd += OnForceDialogEndFileView;
		mwfFileView.SelectedFilesChanged += OnSelectedFilesChangedFileView;
		mwfFileView.ColumnClick += OnColumnClickFileView;
		dirComboBox.DirectoryChanged += OnDirectoryChangedDirComboBox;
		popupButtonPanel.DirectoryChanged += OnDirectoryChangedPopupButtonPanel;
		readonlyCheckBox.CheckedChanged += OnCheckCheckChanged;
		form.FormClosed += OnFileDialogFormClosed;
		custom_places = new FileDialogCustomPlacesCollection();
	}

	public override void Reset()
	{
		addExtension = true;
		checkFileExists = false;
		checkPathExists = true;
		DefaultExt = null;
		dereferenceLinks = true;
		FileName = null;
		Filter = string.Empty;
		FilterIndex = 1;
		InitialDirectory = null;
		restoreDirectory = false;
		SupportMultiDottedExtensions = false;
		ShowHelp = false;
		Title = null;
		validateNames = true;
		UpdateFilters();
	}

	public override string ToString()
	{
		return $"{base.ToString()}: Title: {Title}, FileName: {FileName}";
	}

	[System.MonoTODO("Not implemented, will throw NotImplementedException")]
	protected override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
	{
		throw new NotImplementedException();
	}

	protected void OnFileOk(CancelEventArgs e)
	{
		((CancelEventHandler)base.Events[EventFileOk])?.Invoke(this, e);
	}

	private void CleanupOnClose()
	{
		WriteConfigValues();
		Mime.CleanFileCache();
		disable_form_closed_event = true;
	}

	protected override bool RunDialog(IntPtr hWndOwner)
	{
		ReadConfigValues();
		form.Text = DialogTitle;
		string text = null;
		text = ((fileNames == null || fileNames.Length == 0) ? string.Empty : fileNames[0]);
		SelectFilter();
		form.Refresh();
		SetFileAndDirectory(text);
		fileNameComboBox.Select();
		return true;
	}

	private void SelectFilter()
	{
		int num = filterIndex - 1;
		if (mwfFileView.FilterArrayList == null || mwfFileView.FilterArrayList.Count == 0)
		{
			num = -1;
		}
		else if (num < 0 || num >= mwfFileView.FilterArrayList.Count)
		{
			num = 0;
		}
		do_not_call_OnSelectedIndexChangedFileTypeComboBox = true;
		fileTypeComboBox.BeginUpdate();
		fileTypeComboBox.SelectedIndex = num;
		fileTypeComboBox.EndUpdate();
		do_not_call_OnSelectedIndexChangedFileTypeComboBox = false;
		mwfFileView.FilterIndex = num + 1;
	}

	private void SetFileAndDirectory(string fname)
	{
		if (fname.Length != 0)
		{
			if (!Path.IsPathRooted(fname))
			{
				mwfFileView.ChangeDirectory(null, lastFolder);
				fileNameComboBox.Text = fname;
				return;
			}
			string directoryName = Path.GetDirectoryName(fname);
			if (directoryName != null && directoryName.Length > 0 && Directory.Exists(directoryName))
			{
				fileNameComboBox.Text = Path.GetFileName(fname);
				mwfFileView.ChangeDirectory(null, directoryName);
			}
			else
			{
				fileNameComboBox.Text = fname;
				mwfFileView.ChangeDirectory(null, lastFolder);
			}
		}
		else
		{
			mwfFileView.ChangeDirectory(null, lastFolder);
			fileNameComboBox.Text = null;
		}
	}

	private void OnClickOpenSaveButton(object sender, EventArgs e)
	{
		checkForIllegalChars = true;
		if (fileDialogType == FileDialogType.OpenFileDialog)
		{
			ListView.SelectedListViewItemCollection selectedItems = mwfFileView.SelectedItems;
			if (selectedItems.Count > 0 && selectedItems[0] != null)
			{
				if (selectedItems.Count == 1)
				{
					FileViewListViewItem fileViewListViewItem = selectedItems[0] as FileViewListViewItem;
					FSEntry fSEntry = fileViewListViewItem.FSEntry;
					if ((fSEntry.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
					{
						mwfFileView.ChangeDirectory(null, fSEntry.FullName, CustomFilter);
						return;
					}
				}
				else
				{
					foreach (FileViewListViewItem item in selectedItems)
					{
						FSEntry fSEntry2 = item.FSEntry;
						if ((fSEntry2.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
						{
							mwfFileView.ChangeDirectory(null, fSEntry2.FullName, CustomFilter);
							return;
						}
					}
				}
			}
		}
		if (fileNameComboBox.Text.IndexOfAny(wildcard_chars) != -1)
		{
			mwfFileView.UpdateFileView(fileNameComboBox.Text);
			return;
		}
		ArrayList arrayList = new ArrayList();
		FileNamesTokenizer fileNamesTokenizer = new FileNamesTokenizer(fileNameComboBox.Text, multiSelect);
		fileNamesTokenizer.GetNextFile();
		while (fileNamesTokenizer.CurrentToken != TokenType.EOF)
		{
			string text = fileNamesTokenizer.TokenText;
			if (!Path.IsPathRooted(text))
			{
				if (mwfFileView.CurrentRealFolder != null)
				{
					text = Path.Combine(mwfFileView.CurrentRealFolder, text);
				}
				else if (mwfFileView.CurrentFSEntry != null)
				{
					text = mwfFileView.CurrentFSEntry.FullName;
				}
			}
			FileInfo fileInfo = new FileInfo(text);
			string text2;
			if (fileInfo.Exists || fileDialogType == FileDialogType.SaveFileDialog)
			{
				text2 = text;
			}
			else
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(text);
				if (directoryInfo.Exists)
				{
					mwfFileView.ChangeDirectory(null, directoryInfo.FullName, CustomFilter);
					fileNameComboBox.Text = null;
					return;
				}
				text2 = text;
			}
			if (addExtension)
			{
				string extension = Path.GetExtension(text);
				if (extension.Length == 0)
				{
					string text3 = string.Empty;
					if (AddFilterExtension(text2))
					{
						text3 = GetExtension(text2);
					}
					if (text3.Length == 0 && DefaultExt.Length > 0)
					{
						text3 = "." + DefaultExt;
						if (checkFileExists && !File.Exists(text2 + text3))
						{
							text3 = string.Empty;
						}
					}
					text2 += text3;
				}
			}
			if (checkFileExists && !File.Exists(text2))
			{
				string text4 = "\"" + text2 + "\" does not exist. Please verify that you have entered the correct file name.";
				MessageBox.Show(text4, openSaveButton.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			if (fileDialogType == FileDialogType.SaveFileDialog)
			{
				if (overwritePrompt && File.Exists(text2))
				{
					string text5 = "\"" + text2 + "\" already exists. Do you want to overwrite it?";
					DialogResult dialogResult = MessageBox.Show(text5, openSaveButton.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
					if (dialogResult == DialogResult.Cancel)
					{
						return;
					}
				}
				if (createPrompt && !File.Exists(text2))
				{
					string text6 = "\"" + text2 + "\" does not exist. Do you want to create it?";
					DialogResult dialogResult2 = MessageBox.Show(text6, openSaveButton.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
					if (dialogResult2 == DialogResult.Cancel)
					{
						return;
					}
				}
			}
			arrayList.Add(text2);
			fileNamesTokenizer.GetNextFile();
		}
		if (arrayList.Count > 0)
		{
			fileNames = new string[arrayList.Count];
			for (int i = 0; i < arrayList.Count; i++)
			{
				string text7 = (string)arrayList[i];
				fileNames[i] = text7;
				mwfFileView.WriteRecentlyUsed(text7);
				if (File.Exists(text7) && fileNameComboBox.Items.IndexOf(text7) == -1)
				{
					fileNameComboBox.Items.Insert(0, text7);
				}
			}
			while (fileNameComboBox.Items.Count > MaxFileNameItems)
			{
				fileNameComboBox.Items.RemoveAt(MaxFileNameItems);
			}
			if (checkPathExists && mwfFileView.CurrentRealFolder != null && !Directory.Exists(mwfFileView.CurrentRealFolder))
			{
				string text8 = "\"" + mwfFileView.CurrentRealFolder + "\" does not exist. Please verify that you have entered the correct directory name.";
				MessageBox.Show(text8, openSaveButton.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				if (InitialDirectory.Length == 0 || !Directory.Exists(InitialDirectory))
				{
					mwfFileView.ChangeDirectory(null, lastFolder, CustomFilter);
				}
				else
				{
					mwfFileView.ChangeDirectory(null, InitialDirectory, CustomFilter);
				}
				return;
			}
			if (restoreDirectory)
			{
				lastFolder = restoreDirectoryString;
			}
			else
			{
				lastFolder = mwfFileView.CurrentFolder;
			}
			filterIndex = fileTypeComboBox.SelectedIndex + 1;
			CancelEventArgs cancelEventArgs = new CancelEventArgs();
			OnFileOk(cancelEventArgs);
			if (!cancelEventArgs.Cancel)
			{
				CleanupOnClose();
				form.DialogResult = DialogResult.OK;
			}
			return;
		}
		foreach (FileViewListViewItem selectedItem in mwfFileView.SelectedItems)
		{
			FSEntry fSEntry3 = selectedItem.FSEntry;
			if ((fSEntry3.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
			{
				mwfFileView.ChangeDirectory(null, fSEntry3.FullName, CustomFilter);
				break;
			}
		}
	}

	private bool AddFilterExtension(string fileName)
	{
		if (fileDialogType == FileDialogType.OpenFileDialog)
		{
			if (DefaultExt.Length == 0)
			{
				return true;
			}
			if (checkFileExists)
			{
				string path = fileName + "." + DefaultExt;
				return !File.Exists(path);
			}
			return !File.Exists(fileName);
		}
		return true;
	}

	private string GetExtension(string fileName)
	{
		string result = string.Empty;
		if (fileFilter == null || fileTypeComboBox.SelectedIndex == -1)
		{
			return result;
		}
		FilterStruct filterStruct = (FilterStruct)fileFilter.FilterArrayList[fileTypeComboBox.SelectedIndex];
		for (int i = 0; i < filterStruct.filters.Count; i++)
		{
			string text = filterStruct.filters[i];
			if (text.StartsWith("*"))
			{
				text = text.Remove(0, 1);
			}
			if (text.IndexOf('*') != -1)
			{
				continue;
			}
			if (!supportMultiDottedExtensions)
			{
				int num = text.LastIndexOf('.');
				if (num > 0 && text.LastIndexOf('.', num - 1) != -1)
				{
					text = text.Remove(0, num);
				}
			}
			if (!checkFileExists)
			{
				result = text;
				break;
			}
			if (fileDialogType == FileDialogType.SaveFileDialog && DefaultExt.Length > 0)
			{
				result = text;
				break;
			}
			string path = fileName + text;
			if (File.Exists(path))
			{
				result = text;
				break;
			}
			if (fileDialogType == FileDialogType.SaveFileDialog && DefaultExt.Length > 0)
			{
				result = text;
				break;
			}
		}
		return result;
	}

	private void OnClickCancelButton(object sender, EventArgs e)
	{
		if (restoreDirectory)
		{
			mwfFileView.CurrentFolder = restoreDirectoryString;
		}
		CleanupOnClose();
		form.DialogResult = DialogResult.Cancel;
	}

	private void OnClickHelpButton(object sender, EventArgs e)
	{
		OnHelpRequest(e);
	}

	private void OnClickSmallButtonToolBar(object sender, ToolBarButtonClickEventArgs e)
	{
		if (e.Button == upToolBarButton)
		{
			mwfFileView.OneDirUp(CustomFilter);
		}
		else if (e.Button == backToolBarButton)
		{
			mwfFileView.PopDir(CustomFilter);
		}
		else if (e.Button == newdirToolBarButton)
		{
			mwfFileView.CreateNewFolder();
		}
	}

	private void OnSelectedIndexChangedFileTypeComboBox(object sender, EventArgs e)
	{
		if (do_not_call_OnSelectedIndexChangedFileTypeComboBox)
		{
			do_not_call_OnSelectedIndexChangedFileTypeComboBox = false;
			return;
		}
		UpdateRecentFiles();
		mwfFileView.FilterIndex = fileTypeComboBox.SelectedIndex + 1;
	}

	private void OnSelectedFileChangedFileView(object sender, EventArgs e)
	{
		fileNameComboBox.Text = mwfFileView.CurrentFSEntry.Name;
	}

	private void OnSelectedFilesChangedFileView(object sender, EventArgs e)
	{
		string selectedFilesString = mwfFileView.SelectedFilesString;
		if (selectedFilesString != null && selectedFilesString.Length != 0)
		{
			fileNameComboBox.Text = selectedFilesString;
		}
	}

	private void OnForceDialogEndFileView(object sender, EventArgs e)
	{
		OnClickOpenSaveButton(this, EventArgs.Empty);
	}

	private void OnDirectoryChangedDirComboBox(object sender, EventArgs e)
	{
		mwfFileView.ChangeDirectory(sender, dirComboBox.CurrentFolder, CustomFilter);
	}

	private void OnDirectoryChangedPopupButtonPanel(object sender, EventArgs e)
	{
		mwfFileView.ChangeDirectory(sender, popupButtonPanel.CurrentFolder, CustomFilter);
	}

	private void OnCheckCheckChanged(object sender, EventArgs e)
	{
		ReadOnlyChecked = readonlyCheckBox.Checked;
	}

	private void OnFileDialogFormClosed(object sender, FormClosedEventArgs e)
	{
		HandleFormClosedEvent(sender);
	}

	private void OnColumnClickFileView(object sender, ColumnClickEventArgs e)
	{
		if (file_view_comparer == null)
		{
			file_view_comparer = new MwfFileViewItemComparer(asc: true);
		}
		file_view_comparer.ColumnIndex = e.Column;
		file_view_comparer.Ascendent = !file_view_comparer.Ascendent;
		if (mwfFileView.ListViewItemSorter == null)
		{
			mwfFileView.ListViewItemSorter = file_view_comparer;
		}
		else
		{
			mwfFileView.Sort();
		}
	}

	private void HandleFormClosedEvent(object sender)
	{
		if (!disable_form_closed_event)
		{
			OnClickCancelButton(sender, EventArgs.Empty);
		}
		disable_form_closed_event = false;
	}

	private void UpdateFilters()
	{
		if (fileFilter == null)
		{
			fileFilter = new FileFilter();
		}
		ArrayList filterArrayList = fileFilter.FilterArrayList;
		fileTypeComboBox.BeginUpdate();
		fileTypeComboBox.Items.Clear();
		foreach (FilterStruct item in filterArrayList)
		{
			fileTypeComboBox.Items.Add(item.filterName);
		}
		fileTypeComboBox.EndUpdate();
		mwfFileView.FilterArrayList = filterArrayList;
	}

	private void UpdateRecentFiles()
	{
		fileNameComboBox.Items.Clear();
		if (configFileNames == null)
		{
			return;
		}
		string[] array = configFileNames;
		foreach (string text in array)
		{
			if (text != null && text.Trim().Length != 0)
			{
				if (fileNameComboBox.Items.Count >= MaxFileNameItems)
				{
					break;
				}
				fileNameComboBox.Items.Add(text);
			}
		}
	}

	private void ResizeAndRelocateForHelpOrReadOnly()
	{
		form.SuspendLayout();
		int num = form.Size.Width - form.MinimumSize.Width;
		int num2 = form.Size.Height - form.MinimumSize.Height;
		if (!ShowHelp && !ShowReadOnly)
		{
			num2 += 29;
		}
		mwfFileView.Size = new Size(450 + num, 254 + num2);
		fileNameLabel.Location = new Point(101, 298 + num2);
		fileNameComboBox.Location = new Point(195, 298 + num2);
		fileTypeLabel.Location = new Point(101, 326 + num2);
		fileTypeComboBox.Location = new Point(195, 326 + num2);
		openSaveButton.Location = new Point(474 + num, 298 + num2);
		cancelButton.Location = new Point(474 + num, 324 + num2);
		helpButton.Location = new Point(474 + num, 353 + num2);
		readonlyCheckBox.Location = new Point(195, 350 + num2);
		helpButton.Visible = ShowHelp;
		readonlyCheckBox.Visible = ShowReadOnly;
		form.ResumeLayout();
	}

	private void WriteConfigValues()
	{
		MWFConfig.SetValue("FileDialog", "Width", form.ClientSize.Width);
		MWFConfig.SetValue("FileDialog", "Height", form.ClientSize.Height);
		MWFConfig.SetValue("FileDialog", "X", form.Location.X);
		MWFConfig.SetValue("FileDialog", "Y", form.Location.Y);
		MWFConfig.SetValue("FileDialog", "LastFolder", lastFolder);
		string[] array = new string[fileNameComboBox.Items.Count];
		fileNameComboBox.Items.CopyTo(array, 0);
		MWFConfig.SetValue("FileDialog", "FileNames", array);
	}

	private void ReadConfigValues()
	{
		lastFolder = (string)MWFConfig.GetValue("FileDialog", "LastFolder");
		if (lastFolder != null && lastFolder.IndexOf("://") == -1 && !Directory.Exists(lastFolder))
		{
			lastFolder = MWFVFS.DesktopPrefix;
		}
		if (InitialDirectory.Length > 0 && Directory.Exists(InitialDirectory))
		{
			lastFolder = InitialDirectory;
		}
		else if (lastFolder == null || lastFolder.Length == 0)
		{
			lastFolder = Environment.CurrentDirectory;
		}
		if (RestoreDirectory)
		{
			restoreDirectoryString = lastFolder;
		}
	}
}

namespace System.Windows.Forms;

internal class FileViewListViewItem : ListViewItem
{
	private FSEntry fsEntry;

	public FSEntry FSEntry
	{
		get
		{
			return fsEntry;
		}
		set
		{
			fsEntry = value;
		}
	}

	public FileViewListViewItem(FSEntry fsEntry)
	{
		this.fsEntry = fsEntry;
		base.ImageIndex = fsEntry.IconIndex;
		base.Text = fsEntry.Name;
		switch (fsEntry.FileType)
		{
		case FSEntry.FSEntryType.Directory:
			base.SubItems.Add(string.Empty);
			base.SubItems.Add("Directory");
			base.SubItems.Add(fsEntry.LastAccessTime.ToShortDateString() + " " + fsEntry.LastAccessTime.ToShortTimeString());
			break;
		case FSEntry.FSEntryType.File:
		{
			long num = 1L;
			try
			{
				if (fsEntry.FileSize > 1024)
				{
					num = fsEntry.FileSize / 1024;
				}
			}
			catch (Exception)
			{
				num = 1L;
			}
			base.SubItems.Add(num + " KB");
			base.SubItems.Add("File");
			base.SubItems.Add(fsEntry.LastAccessTime.ToShortDateString() + " " + fsEntry.LastAccessTime.ToShortTimeString());
			break;
		}
		case FSEntry.FSEntryType.Device:
			base.SubItems.Add(string.Empty);
			base.SubItems.Add("Device");
			base.SubItems.Add(fsEntry.LastAccessTime.ToShortDateString() + " " + fsEntry.LastAccessTime.ToShortTimeString());
			break;
		case FSEntry.FSEntryType.RemovableDevice:
			base.SubItems.Add(string.Empty);
			base.SubItems.Add("RemovableDevice");
			base.SubItems.Add(fsEntry.LastAccessTime.ToShortDateString() + " " + fsEntry.LastAccessTime.ToShortTimeString());
			break;
		}
	}
}

using System.Collections;
using System.IO;

namespace System.Windows.Forms;

internal class WinFileSystem : FileSystem
{
	private FSEntry desktopFSEntry;

	private FSEntry recentlyusedFSEntry;

	private FSEntry personalFSEntry;

	private FSEntry mycomputerpersonalFSEntry;

	private FSEntry mycomputerFSEntry;

	private FSEntry mynetworkFSEntry;

	public WinFileSystem()
	{
		desktopFSEntry = new FSEntry();
		desktopFSEntry.Attributes = FileAttributes.Directory;
		desktopFSEntry.FullName = MWFVFS.DesktopPrefix;
		desktopFSEntry.Name = "Desktop";
		desktopFSEntry.RealName = ThemeEngine.Current.Places(UIIcon.PlacesDesktop);
		desktopFSEntry.FileType = FSEntry.FSEntryType.Directory;
		desktopFSEntry.IconIndex = MimeIconEngine.GetIconIndexForMimeType("desktop/desktop");
		desktopFSEntry.LastAccessTime = DateTime.Now;
		recentlyusedFSEntry = new FSEntry();
		recentlyusedFSEntry.Attributes = FileAttributes.Directory;
		recentlyusedFSEntry.FullName = MWFVFS.RecentlyUsedPrefix;
		recentlyusedFSEntry.RealName = ThemeEngine.Current.Places(UIIcon.PlacesRecentDocuments);
		recentlyusedFSEntry.Name = "Recently Used";
		recentlyusedFSEntry.FileType = FSEntry.FSEntryType.Directory;
		recentlyusedFSEntry.IconIndex = MimeIconEngine.GetIconIndexForMimeType("recently/recently");
		recentlyusedFSEntry.LastAccessTime = DateTime.Now;
		personalFSEntry = new FSEntry();
		personalFSEntry.Attributes = FileAttributes.Directory;
		personalFSEntry.FullName = MWFVFS.PersonalPrefix;
		personalFSEntry.Name = "Personal";
		personalFSEntry.MainTopNode = GetDesktopFSEntry();
		personalFSEntry.RealName = ThemeEngine.Current.Places(UIIcon.PlacesPersonal);
		personalFSEntry.FileType = FSEntry.FSEntryType.Directory;
		personalFSEntry.IconIndex = MimeIconEngine.GetIconIndexForMimeType("directory/home");
		personalFSEntry.LastAccessTime = DateTime.Now;
		mycomputerpersonalFSEntry = new FSEntry();
		mycomputerpersonalFSEntry.Attributes = FileAttributes.Directory;
		mycomputerpersonalFSEntry.FullName = MWFVFS.MyComputerPersonalPrefix;
		mycomputerpersonalFSEntry.Name = "Personal";
		mycomputerpersonalFSEntry.MainTopNode = GetMyComputerFSEntry();
		mycomputerpersonalFSEntry.RealName = ThemeEngine.Current.Places(UIIcon.PlacesPersonal);
		mycomputerpersonalFSEntry.FileType = FSEntry.FSEntryType.Directory;
		mycomputerpersonalFSEntry.IconIndex = MimeIconEngine.GetIconIndexForMimeType("directory/home");
		mycomputerpersonalFSEntry.LastAccessTime = DateTime.Now;
		mycomputerFSEntry = new FSEntry();
		mycomputerFSEntry.Attributes = FileAttributes.Directory;
		mycomputerFSEntry.FullName = MWFVFS.MyComputerPrefix;
		mycomputerFSEntry.Name = "My Computer";
		mycomputerFSEntry.MainTopNode = GetDesktopFSEntry();
		mycomputerFSEntry.FileType = FSEntry.FSEntryType.Directory;
		mycomputerFSEntry.IconIndex = MimeIconEngine.GetIconIndexForMimeType("workplace/workplace");
		mycomputerFSEntry.LastAccessTime = DateTime.Now;
		mynetworkFSEntry = new FSEntry();
		mynetworkFSEntry.Attributes = FileAttributes.Directory;
		mynetworkFSEntry.FullName = MWFVFS.MyNetworkPrefix;
		mynetworkFSEntry.Name = "My Network";
		mynetworkFSEntry.MainTopNode = GetDesktopFSEntry();
		mynetworkFSEntry.FileType = FSEntry.FSEntryType.Directory;
		mynetworkFSEntry.IconIndex = MimeIconEngine.GetIconIndexForMimeType("network/network");
		mynetworkFSEntry.LastAccessTime = DateTime.Now;
	}

	public override void WriteRecentlyUsedFiles(string fileToAdd)
	{
	}

	public override ArrayList GetRecentlyUsedFiles()
	{
		ArrayList arrayList = new ArrayList();
		DirectoryInfo directoryInfo = new DirectoryInfo(recentlyusedFSEntry.RealName);
		FileInfo[] files = directoryInfo.GetFiles();
		FileInfo[] array = files;
		foreach (FileInfo fileinfo in array)
		{
			FSEntry fileFSEntry = GetFileFSEntry(fileinfo);
			if (fileFSEntry != null)
			{
				arrayList.Add(fileFSEntry);
			}
		}
		return arrayList;
	}

	public override ArrayList GetMyComputerContent()
	{
		string[] logicalDrives = Directory.GetLogicalDrives();
		ArrayList arrayList = new ArrayList();
		string[] array = logicalDrives;
		foreach (string text in array)
		{
			FSEntry fSEntry = new FSEntry();
			fSEntry.FileType = FSEntry.FSEntryType.Device;
			fSEntry.FullName = text;
			fSEntry.Name = text;
			fSEntry.IconIndex = MimeIconEngine.GetIconIndexForMimeType("harddisk/harddisk");
			fSEntry.Attributes = FileAttributes.Directory;
			fSEntry.MainTopNode = GetMyComputerFSEntry();
			arrayList.Add(fSEntry);
			string key = fSEntry.FullName + "://";
			if (!MWFVFS.MyComputerDevicesPrefix.Contains(key))
			{
				MWFVFS.MyComputerDevicesPrefix.Add(key, fSEntry);
			}
		}
		arrayList.Add(GetMyComputerPersonalFSEntry());
		return arrayList;
	}

	public override ArrayList GetMyNetworkContent()
	{
		return new ArrayList();
	}

	protected override FSEntry GetDesktopFSEntry()
	{
		return desktopFSEntry;
	}

	protected override FSEntry GetRecentlyUsedFSEntry()
	{
		return recentlyusedFSEntry;
	}

	protected override FSEntry GetPersonalFSEntry()
	{
		return personalFSEntry;
	}

	protected override FSEntry GetMyComputerPersonalFSEntry()
	{
		return mycomputerpersonalFSEntry;
	}

	protected override FSEntry GetMyComputerFSEntry()
	{
		return mycomputerFSEntry;
	}

	protected override FSEntry GetMyNetworkFSEntry()
	{
		return mynetworkFSEntry;
	}
}

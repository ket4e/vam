using System.Collections;
using System.Collections.Specialized;
using System.IO;

namespace System.Windows.Forms;

internal abstract class FileSystem
{
	internal class FileInfoComparer : IComparer
	{
		public int Compare(object fileInfo1, object fileInfo2)
		{
			return string.Compare(((FileInfo)fileInfo1).Name, ((FileInfo)fileInfo2).Name);
		}
	}

	internal class FSEntryComparer : IComparer
	{
		public int Compare(object fileInfo1, object fileInfo2)
		{
			return string.Compare(((FSEntry)fileInfo1).Name, ((FSEntry)fileInfo2).Name);
		}
	}

	protected string currentTopFolder = string.Empty;

	protected FSEntry currentFolderFSEntry;

	protected FSEntry currentTopFolderFSEntry;

	private FileInfoComparer fileInfoComparer = new FileInfoComparer();

	private FSEntryComparer fsEntryComparer = new FSEntryComparer();

	public FSEntry ChangeDirectory(string folder)
	{
		if (folder == MWFVFS.DesktopPrefix)
		{
			currentTopFolder = MWFVFS.DesktopPrefix;
			currentTopFolderFSEntry = (currentFolderFSEntry = GetDesktopFSEntry());
		}
		else if (folder == MWFVFS.PersonalPrefix)
		{
			currentTopFolder = MWFVFS.PersonalPrefix;
			currentTopFolderFSEntry = (currentFolderFSEntry = GetPersonalFSEntry());
		}
		else if (folder == MWFVFS.MyComputerPersonalPrefix)
		{
			currentTopFolder = MWFVFS.MyComputerPersonalPrefix;
			currentTopFolderFSEntry = (currentFolderFSEntry = GetMyComputerPersonalFSEntry());
		}
		else if (folder == MWFVFS.RecentlyUsedPrefix)
		{
			currentTopFolder = MWFVFS.RecentlyUsedPrefix;
			currentTopFolderFSEntry = (currentFolderFSEntry = GetRecentlyUsedFSEntry());
		}
		else if (folder == MWFVFS.MyComputerPrefix)
		{
			currentTopFolder = MWFVFS.MyComputerPrefix;
			currentTopFolderFSEntry = (currentFolderFSEntry = GetMyComputerFSEntry());
		}
		else if (folder == MWFVFS.MyNetworkPrefix)
		{
			currentTopFolder = MWFVFS.MyNetworkPrefix;
			currentTopFolderFSEntry = (currentFolderFSEntry = GetMyNetworkFSEntry());
		}
		else
		{
			bool flag = false;
			foreach (DictionaryEntry item in MWFVFS.MyComputerDevicesPrefix)
			{
				FSEntry fSEntry = item.Value as FSEntry;
				if (folder == fSEntry.FullName)
				{
					currentTopFolder = item.Key as string;
					currentTopFolderFSEntry = (currentFolderFSEntry = fSEntry);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				currentFolderFSEntry = GetDirectoryFSEntry(new DirectoryInfo(folder), currentTopFolderFSEntry);
			}
		}
		return currentFolderFSEntry;
	}

	public string GetParent()
	{
		return currentFolderFSEntry.Parent;
	}

	public void GetFolderContent(StringCollection filters, out ArrayList directories_out, out ArrayList files_out)
	{
		directories_out = new ArrayList();
		files_out = new ArrayList();
		if (currentFolderFSEntry.FullName == MWFVFS.DesktopPrefix)
		{
			FSEntry personalFSEntry = GetPersonalFSEntry();
			directories_out.Add(personalFSEntry);
			FSEntry myComputerFSEntry = GetMyComputerFSEntry();
			directories_out.Add(myComputerFSEntry);
			FSEntry myNetworkFSEntry = GetMyNetworkFSEntry();
			directories_out.Add(myNetworkFSEntry);
			ArrayList directories_out2 = null;
			ArrayList files_out2 = null;
			GetNormalFolderContent(ThemeEngine.Current.Places(UIIcon.PlacesDesktop), filters, out directories_out2, out files_out2);
			directories_out.AddRange(directories_out2);
			files_out.AddRange(files_out2);
		}
		else if (currentFolderFSEntry.FullName == MWFVFS.RecentlyUsedPrefix)
		{
			files_out = GetRecentlyUsedFiles();
		}
		else if (currentFolderFSEntry.FullName == MWFVFS.MyComputerPrefix)
		{
			directories_out.AddRange(GetMyComputerContent());
		}
		else if (currentFolderFSEntry.FullName == MWFVFS.PersonalPrefix || currentFolderFSEntry.FullName == MWFVFS.MyComputerPersonalPrefix)
		{
			ArrayList directories_out3 = null;
			ArrayList files_out3 = null;
			GetNormalFolderContent(ThemeEngine.Current.Places(UIIcon.PlacesPersonal), filters, out directories_out3, out files_out3);
			directories_out.AddRange(directories_out3);
			files_out.AddRange(files_out3);
		}
		else if (currentFolderFSEntry.FullName == MWFVFS.MyNetworkPrefix)
		{
			directories_out.AddRange(GetMyNetworkContent());
		}
		else
		{
			GetNormalFolderContent(currentFolderFSEntry.FullName, filters, out directories_out, out files_out);
		}
	}

	public ArrayList GetFoldersOnly()
	{
		ArrayList arrayList = new ArrayList();
		if (currentFolderFSEntry.FullName == MWFVFS.DesktopPrefix)
		{
			FSEntry personalFSEntry = GetPersonalFSEntry();
			arrayList.Add(personalFSEntry);
			FSEntry myComputerFSEntry = GetMyComputerFSEntry();
			arrayList.Add(myComputerFSEntry);
			FSEntry myNetworkFSEntry = GetMyNetworkFSEntry();
			arrayList.Add(myNetworkFSEntry);
			ArrayList normalFolders = GetNormalFolders(ThemeEngine.Current.Places(UIIcon.PlacesDesktop));
			arrayList.AddRange(normalFolders);
		}
		else if (!(currentFolderFSEntry.FullName == MWFVFS.RecentlyUsedPrefix))
		{
			if (currentFolderFSEntry.FullName == MWFVFS.MyComputerPrefix)
			{
				arrayList.AddRange(GetMyComputerContent());
			}
			else if (currentFolderFSEntry.FullName == MWFVFS.PersonalPrefix || currentFolderFSEntry.FullName == MWFVFS.MyComputerPersonalPrefix)
			{
				ArrayList normalFolders2 = GetNormalFolders(ThemeEngine.Current.Places(UIIcon.PlacesPersonal));
				arrayList.AddRange(normalFolders2);
			}
			else if (currentFolderFSEntry.FullName == MWFVFS.MyNetworkPrefix)
			{
				arrayList.AddRange(GetMyNetworkContent());
			}
			else
			{
				arrayList = GetNormalFolders(currentFolderFSEntry.FullName);
			}
		}
		return arrayList;
	}

	protected void GetNormalFolderContent(string from_folder, StringCollection filters, out ArrayList directories_out, out ArrayList files_out)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(from_folder);
		directories_out = new ArrayList();
		DirectoryInfo[] array = null;
		try
		{
			array = directoryInfo.GetDirectories();
		}
		catch (Exception)
		{
		}
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				directories_out.Add(GetDirectoryFSEntry(array[i], currentTopFolderFSEntry));
			}
		}
		directories_out.Sort(fsEntryComparer);
		files_out = new ArrayList();
		ArrayList arrayList = new ArrayList();
		try
		{
			if (filters == null)
			{
				arrayList.AddRange(directoryInfo.GetFiles());
			}
			else
			{
				StringEnumerator enumerator = filters.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						string current = enumerator.Current;
						arrayList.AddRange(directoryInfo.GetFiles(current));
					}
				}
				finally
				{
					if (enumerator is IDisposable disposable)
					{
						disposable.Dispose();
					}
				}
				arrayList.Sort(fileInfoComparer);
			}
		}
		catch (Exception)
		{
		}
		for (int j = 0; j < arrayList.Count; j++)
		{
			FSEntry fileFSEntry = GetFileFSEntry(arrayList[j] as FileInfo);
			if (fileFSEntry != null)
			{
				files_out.Add(fileFSEntry);
			}
		}
	}

	protected ArrayList GetNormalFolders(string from_folder)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(from_folder);
		ArrayList arrayList = new ArrayList();
		DirectoryInfo[] array = null;
		try
		{
			array = directoryInfo.GetDirectories();
		}
		catch (Exception)
		{
		}
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				arrayList.Add(GetDirectoryFSEntry(array[i], currentTopFolderFSEntry));
			}
		}
		return arrayList;
	}

	protected virtual FSEntry GetDirectoryFSEntry(DirectoryInfo dirinfo, FSEntry topFolderFSEntry)
	{
		FSEntry fSEntry = new FSEntry();
		fSEntry.Attributes = dirinfo.Attributes;
		fSEntry.FullName = dirinfo.FullName;
		fSEntry.Name = dirinfo.Name;
		fSEntry.MainTopNode = topFolderFSEntry;
		fSEntry.FileType = FSEntry.FSEntryType.Directory;
		fSEntry.IconIndex = MimeIconEngine.GetIconIndexForMimeType("inode/directory");
		fSEntry.LastAccessTime = dirinfo.LastAccessTime;
		return fSEntry;
	}

	protected virtual FSEntry GetFileFSEntry(FileInfo fileinfo)
	{
		if ((fileinfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
		{
			return null;
		}
		FSEntry fSEntry = new FSEntry();
		fSEntry.Attributes = fileinfo.Attributes;
		fSEntry.FullName = fileinfo.FullName;
		fSEntry.Name = fileinfo.Name;
		fSEntry.FileType = FSEntry.FSEntryType.File;
		fSEntry.IconIndex = MimeIconEngine.GetIconIndexForFile(fileinfo.FullName);
		fSEntry.FileSize = fileinfo.Length;
		fSEntry.LastAccessTime = fileinfo.LastAccessTime;
		return fSEntry;
	}

	protected abstract FSEntry GetDesktopFSEntry();

	protected abstract FSEntry GetRecentlyUsedFSEntry();

	protected abstract FSEntry GetPersonalFSEntry();

	protected abstract FSEntry GetMyComputerPersonalFSEntry();

	protected abstract FSEntry GetMyComputerFSEntry();

	protected abstract FSEntry GetMyNetworkFSEntry();

	public abstract void WriteRecentlyUsedFiles(string fileToAdd);

	public abstract ArrayList GetRecentlyUsedFiles();

	public abstract ArrayList GetMyComputerContent();

	public abstract ArrayList GetMyNetworkContent();
}

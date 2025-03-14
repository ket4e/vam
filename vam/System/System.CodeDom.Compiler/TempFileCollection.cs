using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace System.CodeDom.Compiler;

[Serializable]
[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
public class TempFileCollection : IDisposable, ICollection, IEnumerable
{
	private Hashtable filehash;

	private string tempdir;

	private bool keepfiles;

	private string basepath;

	private Random rnd;

	private string ownTempDir;

	int ICollection.Count => filehash.Count;

	object ICollection.SyncRoot => null;

	bool ICollection.IsSynchronized => false;

	public string BasePath
	{
		get
		{
			if (basepath == null)
			{
				if (rnd == null)
				{
					rnd = new Random();
				}
				string text = tempdir;
				if (text.Length == 0)
				{
					text = GetOwnTempDir();
				}
				FileStream fileStream = null;
				do
				{
					int num = rnd.Next();
					basepath = Path.Combine(text, (num + 1).ToString("x"));
					string path = basepath + ".tmp";
					try
					{
						fileStream = new FileStream(path, FileMode.CreateNew);
					}
					catch (IOException)
					{
						fileStream = null;
					}
					catch
					{
						throw;
					}
				}
				while (fileStream == null);
				fileStream.Close();
				if (SecurityManager.SecurityEnabled)
				{
					new FileIOPermission(FileIOPermissionAccess.PathDiscovery, basepath).Demand();
				}
			}
			return basepath;
		}
	}

	public int Count => filehash.Count;

	public bool KeepFiles
	{
		get
		{
			return keepfiles;
		}
		set
		{
			keepfiles = value;
		}
	}

	public string TempDir => tempdir;

	public TempFileCollection()
		: this(string.Empty, keepFiles: false)
	{
	}

	public TempFileCollection(string tempDir)
		: this(tempDir, keepFiles: false)
	{
	}

	public TempFileCollection(string tempDir, bool keepFiles)
	{
		filehash = new Hashtable();
		tempdir = ((tempDir != null) ? tempDir : string.Empty);
		keepfiles = keepFiles;
	}

	void ICollection.CopyTo(Array array, int start)
	{
		filehash.Keys.CopyTo(array, start);
	}

	void IDisposable.Dispose()
	{
		Dispose(disposing: true);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return filehash.Keys.GetEnumerator();
	}

	private string GetOwnTempDir()
	{
		if (ownTempDir != null)
		{
			return ownTempDir;
		}
		string tempPath = Path.GetTempPath();
		int num = -1;
		bool flag = false;
		switch (Environment.OSVersion.Platform)
		{
		case PlatformID.Win32S:
		case PlatformID.Win32Windows:
		case PlatformID.Win32NT:
		case PlatformID.WinCE:
			flag = true;
			num = 0;
			break;
		}
		do
		{
			int num2 = rnd.Next();
			ownTempDir = Path.Combine(tempPath, (num2 + 1).ToString("x"));
			if (!Directory.Exists(ownTempDir))
			{
				if (flag)
				{
					Directory.CreateDirectory(ownTempDir);
				}
				else
				{
					num = mkdir(ownTempDir, 448u);
				}
				if (num != 0 && !Directory.Exists(ownTempDir))
				{
					throw new IOException();
				}
			}
		}
		while (num != 0);
		return ownTempDir;
	}

	public string AddExtension(string fileExtension)
	{
		return AddExtension(fileExtension, keepfiles);
	}

	public string AddExtension(string fileExtension, bool keepFile)
	{
		string text = BasePath + "." + fileExtension;
		AddFile(text, keepFile);
		return text;
	}

	public void AddFile(string fileName, bool keepFile)
	{
		filehash.Add(fileName, keepFile);
	}

	public void CopyTo(string[] fileNames, int start)
	{
		filehash.Keys.CopyTo(fileNames, start);
	}

	public void Delete()
	{
		bool flag = true;
		string[] array = new string[filehash.Count];
		filehash.Keys.CopyTo(array, 0);
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (!(bool)filehash[text])
			{
				File.Delete(text);
				filehash.Remove(text);
			}
			else
			{
				flag = false;
			}
		}
		if (basepath != null)
		{
			string path = basepath + ".tmp";
			File.Delete(path);
			basepath = null;
		}
		if (flag && ownTempDir != null)
		{
			Directory.Delete(ownTempDir, recursive: true);
			ownTempDir = null;
		}
	}

	public IEnumerator GetEnumerator()
	{
		return filehash.Keys.GetEnumerator();
	}

	protected virtual void Dispose(bool disposing)
	{
		Delete();
		if (disposing)
		{
			GC.SuppressFinalize(true);
		}
	}

	~TempFileCollection()
	{
		Dispose(disposing: false);
	}

	[DllImport("libc")]
	private static extern int mkdir(string olpath, uint mode);
}

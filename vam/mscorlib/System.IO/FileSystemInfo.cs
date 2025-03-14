using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.IO;

[Serializable]
[ComVisible(true)]
[PermissionSet(SecurityAction.InheritanceDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Unrestricted=\"true\"/>\n</PermissionSet>\n")]
public abstract class FileSystemInfo : MarshalByRefObject, ISerializable
{
	protected string FullPath;

	protected string OriginalPath;

	internal MonoIOStat stat;

	internal bool valid;

	public abstract bool Exists { get; }

	public abstract string Name { get; }

	public virtual string FullName => FullPath;

	public string Extension => Path.GetExtension(Name);

	public FileAttributes Attributes
	{
		get
		{
			Refresh(force: false);
			return stat.Attributes;
		}
		set
		{
			if (!MonoIO.SetFileAttributes(FullName, value, out var error))
			{
				throw MonoIO.GetException(FullName, error);
			}
			Refresh(force: true);
		}
	}

	public DateTime CreationTime
	{
		get
		{
			Refresh(force: false);
			return DateTime.FromFileTime(stat.CreationTime);
		}
		set
		{
			long creation_time = value.ToFileTime();
			if (!MonoIO.SetFileTime(FullName, creation_time, -1L, -1L, out var error))
			{
				throw MonoIO.GetException(FullName, error);
			}
			Refresh(force: true);
		}
	}

	[ComVisible(false)]
	public DateTime CreationTimeUtc
	{
		get
		{
			return CreationTime.ToUniversalTime();
		}
		set
		{
			CreationTime = value.ToLocalTime();
		}
	}

	public DateTime LastAccessTime
	{
		get
		{
			Refresh(force: false);
			return DateTime.FromFileTime(stat.LastAccessTime);
		}
		set
		{
			long last_access_time = value.ToFileTime();
			if (!MonoIO.SetFileTime(FullName, -1L, last_access_time, -1L, out var error))
			{
				throw MonoIO.GetException(FullName, error);
			}
			Refresh(force: true);
		}
	}

	[ComVisible(false)]
	public DateTime LastAccessTimeUtc
	{
		get
		{
			Refresh(force: false);
			return LastAccessTime.ToUniversalTime();
		}
		set
		{
			LastAccessTime = value.ToLocalTime();
		}
	}

	public DateTime LastWriteTime
	{
		get
		{
			Refresh(force: false);
			return DateTime.FromFileTime(stat.LastWriteTime);
		}
		set
		{
			long last_write_time = value.ToFileTime();
			if (!MonoIO.SetFileTime(FullName, -1L, -1L, last_write_time, out var error))
			{
				throw MonoIO.GetException(FullName, error);
			}
			Refresh(force: true);
		}
	}

	[ComVisible(false)]
	public DateTime LastWriteTimeUtc
	{
		get
		{
			Refresh(force: false);
			return LastWriteTime.ToUniversalTime();
		}
		set
		{
			LastWriteTime = value.ToLocalTime();
		}
	}

	protected FileSystemInfo()
	{
		valid = false;
		FullPath = null;
	}

	protected FileSystemInfo(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		FullPath = info.GetString("FullPath");
		OriginalPath = info.GetString("OriginalPath");
	}

	[ComVisible(false)]
	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("OriginalPath", OriginalPath, typeof(string));
		info.AddValue("FullPath", FullPath, typeof(string));
	}

	public abstract void Delete();

	public void Refresh()
	{
		Refresh(force: true);
	}

	internal void Refresh(bool force)
	{
		if (!valid || force)
		{
			MonoIO.GetFileStat(FullName, out stat, out var _);
			valid = true;
			InternalRefresh();
		}
	}

	internal virtual void InternalRefresh()
	{
	}

	internal void CheckPath(string path)
	{
		if (path == null)
		{
			throw new ArgumentNullException("path");
		}
		if (path.Length == 0)
		{
			throw new ArgumentException("An empty file name is not valid.");
		}
		if (path.IndexOfAny(Path.InvalidPathChars) != -1)
		{
			throw new ArgumentException("Illegal characters in path.");
		}
	}
}

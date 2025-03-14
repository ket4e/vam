using System;
using Mono.Unix.Native;

namespace Mono.Unix;

public abstract class UnixFileSystemInfo
{
	internal const FileSpecialAttributes AllSpecialAttributes = FileSpecialAttributes.SetUserId | FileSpecialAttributes.SetGroupId | FileSpecialAttributes.Sticky;

	internal const FileTypes AllFileTypes = (FileTypes)61440;

	private Stat stat;

	private string fullPath;

	private string originalPath;

	private bool valid;

	protected string FullPath
	{
		get
		{
			return fullPath;
		}
		set
		{
			if (fullPath != value)
			{
				UnixPath.CheckPath(value);
				valid = false;
				fullPath = value;
			}
		}
	}

	protected string OriginalPath
	{
		get
		{
			return originalPath;
		}
		set
		{
			originalPath = value;
		}
	}

	public virtual string FullName => FullPath;

	public abstract string Name { get; }

	public bool Exists
	{
		get
		{
			Refresh(force: true);
			return valid;
		}
	}

	public long Device
	{
		get
		{
			AssertValid();
			return Convert.ToInt64(stat.st_dev);
		}
	}

	public long Inode
	{
		get
		{
			AssertValid();
			return Convert.ToInt64(stat.st_ino);
		}
	}

	[CLSCompliant(false)]
	public FilePermissions Protection
	{
		get
		{
			AssertValid();
			return stat.st_mode;
		}
		set
		{
			int retval = Syscall.chmod(FullPath, value);
			UnixMarshal.ThrowExceptionForLastErrorIf(retval);
		}
	}

	public FileTypes FileType
	{
		get
		{
			AssertValid();
			return (FileTypes)(stat.st_mode & FilePermissions.S_IFMT);
		}
	}

	public FileAccessPermissions FileAccessPermissions
	{
		get
		{
			AssertValid();
			int st_mode = (int)stat.st_mode;
			return (FileAccessPermissions)(st_mode & 0x1FF);
		}
		set
		{
			AssertValid();
			int st_mode = (int)stat.st_mode;
			st_mode &= -512;
			st_mode |= (int)value;
			Protection = (FilePermissions)st_mode;
		}
	}

	public FileSpecialAttributes FileSpecialAttributes
	{
		get
		{
			AssertValid();
			int st_mode = (int)stat.st_mode;
			return (FileSpecialAttributes)(st_mode & 0xE00);
		}
		set
		{
			AssertValid();
			int st_mode = (int)stat.st_mode;
			st_mode &= -3585;
			st_mode |= (int)value;
			Protection = (FilePermissions)st_mode;
		}
	}

	public long LinkCount
	{
		get
		{
			AssertValid();
			return Convert.ToInt64(stat.st_nlink);
		}
	}

	public UnixUserInfo OwnerUser
	{
		get
		{
			AssertValid();
			return new UnixUserInfo(stat.st_uid);
		}
	}

	public long OwnerUserId
	{
		get
		{
			AssertValid();
			return stat.st_uid;
		}
	}

	public UnixGroupInfo OwnerGroup
	{
		get
		{
			AssertValid();
			return new UnixGroupInfo(stat.st_gid);
		}
	}

	public long OwnerGroupId
	{
		get
		{
			AssertValid();
			return stat.st_gid;
		}
	}

	public long DeviceType
	{
		get
		{
			AssertValid();
			return Convert.ToInt64(stat.st_rdev);
		}
	}

	public long Length
	{
		get
		{
			AssertValid();
			return stat.st_size;
		}
	}

	public long BlockSize
	{
		get
		{
			AssertValid();
			return stat.st_blksize;
		}
	}

	public long BlocksAllocated
	{
		get
		{
			AssertValid();
			return stat.st_blocks;
		}
	}

	public DateTime LastAccessTime
	{
		get
		{
			AssertValid();
			return NativeConvert.ToDateTime(stat.st_atime);
		}
	}

	public DateTime LastAccessTimeUtc => LastAccessTime.ToUniversalTime();

	public DateTime LastWriteTime
	{
		get
		{
			AssertValid();
			return NativeConvert.ToDateTime(stat.st_mtime);
		}
	}

	public DateTime LastWriteTimeUtc => LastWriteTime.ToUniversalTime();

	public DateTime LastStatusChangeTime
	{
		get
		{
			AssertValid();
			return NativeConvert.ToDateTime(stat.st_ctime);
		}
	}

	public DateTime LastStatusChangeTimeUtc => LastStatusChangeTime.ToUniversalTime();

	public bool IsDirectory
	{
		get
		{
			AssertValid();
			return IsFileType(stat.st_mode, FilePermissions.S_IFDIR);
		}
	}

	public bool IsCharacterDevice
	{
		get
		{
			AssertValid();
			return IsFileType(stat.st_mode, FilePermissions.S_IFCHR);
		}
	}

	public bool IsBlockDevice
	{
		get
		{
			AssertValid();
			return IsFileType(stat.st_mode, FilePermissions.S_IFBLK);
		}
	}

	public bool IsRegularFile
	{
		get
		{
			AssertValid();
			return IsFileType(stat.st_mode, FilePermissions.S_IFREG);
		}
	}

	public bool IsFifo
	{
		get
		{
			AssertValid();
			return IsFileType(stat.st_mode, FilePermissions.S_IFIFO);
		}
	}

	public bool IsSymbolicLink
	{
		get
		{
			AssertValid();
			return IsFileType(stat.st_mode, FilePermissions.S_IFLNK);
		}
	}

	public bool IsSocket
	{
		get
		{
			AssertValid();
			return IsFileType(stat.st_mode, FilePermissions.S_IFSOCK);
		}
	}

	public bool IsSetUser
	{
		get
		{
			AssertValid();
			return IsSet(stat.st_mode, FilePermissions.S_ISUID);
		}
	}

	public bool IsSetGroup
	{
		get
		{
			AssertValid();
			return IsSet(stat.st_mode, FilePermissions.S_ISGID);
		}
	}

	public bool IsSticky
	{
		get
		{
			AssertValid();
			return IsSet(stat.st_mode, FilePermissions.S_ISVTX);
		}
	}

	protected UnixFileSystemInfo(string path)
	{
		UnixPath.CheckPath(path);
		originalPath = path;
		fullPath = UnixPath.GetFullPath(path);
		Refresh(force: true);
	}

	internal UnixFileSystemInfo(string path, Stat stat)
	{
		originalPath = path;
		fullPath = UnixPath.GetFullPath(path);
		this.stat = stat;
		valid = true;
	}

	private void AssertValid()
	{
		Refresh(force: false);
		if (!valid)
		{
			throw new InvalidOperationException("Path doesn't exist!");
		}
	}

	internal static bool IsFileType(FilePermissions mode, FilePermissions type)
	{
		return (mode & FilePermissions.S_IFMT) == type;
	}

	internal static bool IsSet(FilePermissions mode, FilePermissions type)
	{
		return (mode & type) == type;
	}

	[CLSCompliant(false)]
	public bool CanAccess(AccessModes mode)
	{
		int num = Syscall.access(FullPath, mode);
		return num == 0;
	}

	public UnixFileSystemInfo CreateLink(string path)
	{
		int retval = Syscall.link(FullName, path);
		UnixMarshal.ThrowExceptionForLastErrorIf(retval);
		return GetFileSystemEntry(path);
	}

	public UnixSymbolicLinkInfo CreateSymbolicLink(string path)
	{
		int retval = Syscall.symlink(FullName, path);
		UnixMarshal.ThrowExceptionForLastErrorIf(retval);
		return new UnixSymbolicLinkInfo(path);
	}

	public abstract void Delete();

	[CLSCompliant(false)]
	public long GetConfigurationValue(PathconfName name)
	{
		long num = Syscall.pathconf(FullPath, name);
		if (num == -1 && Stdlib.GetLastError() != 0)
		{
			UnixMarshal.ThrowExceptionForLastError();
		}
		return num;
	}

	public void Refresh()
	{
		Refresh(force: true);
	}

	internal void Refresh(bool force)
	{
		if (!valid || force)
		{
			valid = GetFileStatus(FullPath, out stat);
		}
	}

	protected virtual bool GetFileStatus(string path, out Stat stat)
	{
		return Syscall.stat(path, out stat) == 0;
	}

	public void SetLength(long length)
	{
		int num;
		do
		{
			num = Syscall.truncate(FullPath, length);
		}
		while (UnixMarshal.ShouldRetrySyscall(num));
		UnixMarshal.ThrowExceptionForLastErrorIf(num);
	}

	public virtual void SetOwner(long owner, long group)
	{
		uint owner2 = Convert.ToUInt32(owner);
		uint group2 = Convert.ToUInt32(group);
		int retval = Syscall.chown(FullPath, owner2, group2);
		UnixMarshal.ThrowExceptionForLastErrorIf(retval);
	}

	public void SetOwner(string owner)
	{
		Passwd passwd = Syscall.getpwnam(owner);
		if (passwd == null)
		{
			throw new ArgumentException(global::Locale.GetText("invalid username"), "owner");
		}
		uint pw_uid = passwd.pw_uid;
		uint pw_gid = passwd.pw_gid;
		SetOwner(pw_uid, pw_gid);
	}

	public void SetOwner(string owner, string group)
	{
		long owner2 = -1L;
		if (owner != null)
		{
			owner2 = new UnixUserInfo(owner).UserId;
		}
		long group2 = -1L;
		if (group != null)
		{
			group2 = new UnixGroupInfo(group).GroupId;
		}
		SetOwner(owner2, group2);
	}

	public void SetOwner(UnixUserInfo owner)
	{
		long group;
		long owner2 = (group = -1L);
		if (owner != null)
		{
			owner2 = owner.UserId;
			group = owner.GroupId;
		}
		SetOwner(owner2, group);
	}

	public void SetOwner(UnixUserInfo owner, UnixGroupInfo group)
	{
		long group2;
		long owner2 = (group2 = -1L);
		if (owner != null)
		{
			owner2 = owner.UserId;
		}
		if (group != null)
		{
			group2 = owner.GroupId;
		}
		SetOwner(owner2, group2);
	}

	public override string ToString()
	{
		return FullPath;
	}

	public Stat ToStat()
	{
		AssertValid();
		return stat;
	}

	public static UnixFileSystemInfo GetFileSystemEntry(string path)
	{
		Stat buf;
		int num = Syscall.lstat(path, out buf);
		if (num == -1 && Stdlib.GetLastError() == Errno.ENOENT)
		{
			return new UnixFileInfo(path);
		}
		UnixMarshal.ThrowExceptionForLastErrorIf(num);
		if (IsFileType(buf.st_mode, FilePermissions.S_IFDIR))
		{
			return new UnixDirectoryInfo(path, buf);
		}
		if (IsFileType(buf.st_mode, FilePermissions.S_IFLNK))
		{
			return new UnixSymbolicLinkInfo(path, buf);
		}
		return new UnixFileInfo(path, buf);
	}
}

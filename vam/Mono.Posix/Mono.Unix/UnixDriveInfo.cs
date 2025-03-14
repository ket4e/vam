using System;
using System.Collections;
using System.IO;
using Mono.Unix.Native;

namespace Mono.Unix;

public sealed class UnixDriveInfo
{
	private Statvfs stat;

	private string fstype;

	private string mount_point;

	private string block_device;

	public long AvailableFreeSpace
	{
		get
		{
			Refresh();
			return Convert.ToInt64(stat.f_bavail * stat.f_frsize);
		}
	}

	public string DriveFormat => fstype;

	public UnixDriveType DriveType => UnixDriveType.Unknown;

	public bool IsReady
	{
		get
		{
			bool flag = Refresh(throwException: false);
			if (mount_point == "/" || !flag)
			{
				return flag;
			}
			if (Syscall.statvfs(RootDirectory.Parent.FullName, out var buf) != 0)
			{
				return false;
			}
			return buf.f_fsid != stat.f_fsid;
		}
	}

	public string Name => mount_point;

	public UnixDirectoryInfo RootDirectory => new UnixDirectoryInfo(mount_point);

	public long TotalFreeSpace
	{
		get
		{
			Refresh();
			return (long)(stat.f_bfree * stat.f_frsize);
		}
	}

	public long TotalSize
	{
		get
		{
			Refresh();
			return (long)(stat.f_frsize * stat.f_blocks);
		}
	}

	public string VolumeLabel => block_device;

	public long MaximumFilenameLength
	{
		get
		{
			Refresh();
			return Convert.ToInt64(stat.f_namemax);
		}
	}

	public UnixDriveInfo(string mountPoint)
	{
		if (mountPoint == null)
		{
			throw new ArgumentNullException("mountPoint");
		}
		Fstab fstab = Syscall.getfsfile(mountPoint);
		if (fstab != null)
		{
			FromFstab(fstab);
			return;
		}
		mount_point = mountPoint;
		block_device = string.Empty;
		fstype = "Unknown";
	}

	private UnixDriveInfo(Fstab fstab)
	{
		FromFstab(fstab);
	}

	private void FromFstab(Fstab fstab)
	{
		fstype = fstab.fs_vfstype;
		mount_point = fstab.fs_file;
		block_device = fstab.fs_spec;
	}

	public static UnixDriveInfo GetForSpecialFile(string specialFile)
	{
		if (specialFile == null)
		{
			throw new ArgumentNullException("specialFile");
		}
		Fstab fstab = Syscall.getfsspec(specialFile);
		if (fstab == null)
		{
			throw new ArgumentException("specialFile isn't valid: " + specialFile);
		}
		return new UnixDriveInfo(fstab);
	}

	public static UnixDriveInfo[] GetDrives()
	{
		ArrayList arrayList = new ArrayList();
		lock (Syscall.fstab_lock)
		{
			int num = Syscall.setfsent();
			if (num != 1)
			{
				throw new IOException("Error calling setfsent(3)", new UnixIOException());
			}
			try
			{
				Fstab fstab;
				while ((fstab = Syscall.getfsent()) != null)
				{
					if (fstab.fs_file.StartsWith("/"))
					{
						arrayList.Add(new UnixDriveInfo(fstab));
					}
				}
			}
			finally
			{
				Syscall.endfsent();
			}
		}
		return (UnixDriveInfo[])arrayList.ToArray(typeof(UnixDriveInfo));
	}

	public override string ToString()
	{
		return VolumeLabel;
	}

	private void Refresh()
	{
		Refresh(throwException: true);
	}

	private bool Refresh(bool throwException)
	{
		int num = Syscall.statvfs(mount_point, out stat);
		if (num == -1 && throwException)
		{
			Errno lastError = Stdlib.GetLastError();
			throw new InvalidOperationException(UnixMarshal.GetErrorDescription(lastError), new UnixIOException(lastError));
		}
		if (num == -1)
		{
			return false;
		}
		return true;
	}
}

using System.Collections;
using System.IO;

namespace System.Windows.Forms;

internal class MasterMount
{
	internal enum FsTypes
	{
		none,
		ext2,
		ext3,
		hpfs,
		iso9660,
		jfs,
		minix,
		msdos,
		ntfs,
		reiserfs,
		ufs,
		umsdos,
		vfat,
		sysv,
		xfs,
		ncpfs,
		nfs,
		smbfs,
		usbfs,
		cifs
	}

	internal struct Mount
	{
		public string device_or_filesystem;

		public string device_short;

		public string mount_point;

		public FsTypes fsType;
	}

	public class MountComparer : IComparer
	{
		public int Compare(object mount1, object mount2)
		{
			return string.Compare(((Mount)mount1).device_short, ((Mount)mount2).device_short);
		}
	}

	private bool proc_mount_available;

	private ArrayList block_devices = new ArrayList();

	private ArrayList network_devices = new ArrayList();

	private ArrayList removable_devices = new ArrayList();

	private MountComparer mountComparer = new MountComparer();

	public ArrayList Block_devices => block_devices;

	public ArrayList Network_devices => network_devices;

	public ArrayList Removable_devices => removable_devices;

	public bool ProcMountAvailable => proc_mount_available;

	public MasterMount()
	{
		if (XplatUI.RunningOnUnix && File.Exists("/proc/mounts"))
		{
			proc_mount_available = true;
		}
	}

	public void GetMounts()
	{
		if (!proc_mount_available)
		{
			return;
		}
		block_devices.Clear();
		network_devices.Clear();
		removable_devices.Clear();
		try
		{
			StreamReader streamReader = new StreamReader("/proc/mounts");
			string text = streamReader.ReadLine();
			ArrayList arrayList = new ArrayList();
			while (text != null)
			{
				if (arrayList.IndexOf(text) == -1)
				{
					ProcessProcMountLine(text);
					arrayList.Add(text);
				}
				text = streamReader.ReadLine();
			}
			streamReader.Close();
			block_devices.Sort(mountComparer);
			network_devices.Sort(mountComparer);
			removable_devices.Sort(mountComparer);
		}
		catch
		{
		}
	}

	private void ProcessProcMountLine(string line)
	{
		string[] array = line.Split(' ');
		if (array == null || array.Length <= 0)
		{
			return;
		}
		Mount mount = default(Mount);
		if (array[0].StartsWith("/dev/"))
		{
			mount.device_short = array[0].Replace("/dev/", string.Empty);
		}
		else
		{
			mount.device_short = array[0];
		}
		mount.device_or_filesystem = array[0];
		mount.mount_point = array[1];
		if (array[2] == "nfs")
		{
			mount.fsType = FsTypes.nfs;
			network_devices.Add(mount);
		}
		else if (array[2] == "smbfs")
		{
			mount.fsType = FsTypes.smbfs;
			network_devices.Add(mount);
		}
		else if (array[2] == "cifs")
		{
			mount.fsType = FsTypes.cifs;
			network_devices.Add(mount);
		}
		else if (array[2] == "ncpfs")
		{
			mount.fsType = FsTypes.ncpfs;
			network_devices.Add(mount);
		}
		else if (array[2] == "iso9660")
		{
			mount.fsType = FsTypes.iso9660;
			removable_devices.Add(mount);
		}
		else if (array[2] == "usbfs")
		{
			mount.fsType = FsTypes.usbfs;
			removable_devices.Add(mount);
		}
		else if (array[0].StartsWith("/") && !array[1].StartsWith("/dev/"))
		{
			if (array[2] == "ext2")
			{
				mount.fsType = FsTypes.ext2;
			}
			else if (array[2] == "ext3")
			{
				mount.fsType = FsTypes.ext3;
			}
			else if (array[2] == "reiserfs")
			{
				mount.fsType = FsTypes.reiserfs;
			}
			else if (array[2] == "xfs")
			{
				mount.fsType = FsTypes.xfs;
			}
			else if (array[2] == "vfat")
			{
				mount.fsType = FsTypes.vfat;
			}
			else if (array[2] == "ntfs")
			{
				mount.fsType = FsTypes.ntfs;
			}
			else if (array[2] == "msdos")
			{
				mount.fsType = FsTypes.msdos;
			}
			else if (array[2] == "umsdos")
			{
				mount.fsType = FsTypes.umsdos;
			}
			else if (array[2] == "hpfs")
			{
				mount.fsType = FsTypes.hpfs;
			}
			else if (array[2] == "minix")
			{
				mount.fsType = FsTypes.minix;
			}
			else if (array[2] == "jfs")
			{
				mount.fsType = FsTypes.jfs;
			}
			block_devices.Add(mount);
		}
	}
}

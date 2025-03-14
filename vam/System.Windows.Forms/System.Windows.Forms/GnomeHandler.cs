using System.Drawing;

namespace System.Windows.Forms;

internal class GnomeHandler : PlatformMimeIconHandler
{
	public override MimeExtensionHandlerStatus Start()
	{
		CreateUIIcons();
		return MimeExtensionHandlerStatus.OK;
	}

	private void CreateUIIcons()
	{
		AddGnomeIcon("unknown/unknown", "gnome-fs-regular");
		AddGnomeIcon("inode/directory", "gnome-fs-directory");
		AddGnomeIcon("directory/home", "gnome-fs-home");
		AddGnomeIcon("desktop/desktop", "gnome-fs-desktop");
		AddGnomeIcon("recently/recently", "gnome-fs-directory-accept");
		AddGnomeIcon("workplace/workplace", "gnome-fs-client");
		AddGnomeIcon("network/network", "gnome-fs-network");
		AddGnomeIcon("nfs/nfs", "gnome-fs-nfs");
		AddGnomeIcon("smb/smb", "gnome-fs-smb");
		AddGnomeIcon("harddisk/harddisk", "gnome-dev-harddisk");
		AddGnomeIcon("cdrom/cdrom", "gnome-dev-cdrom");
		AddGnomeIcon("removable/removable", "gnome-dev-removable");
	}

	private void AddGnomeIcon(string internal_mime_type, string name)
	{
		int num = -1;
		if (MimeIconEngine.MimeIconIndex.ContainsKey(internal_mime_type))
		{
			return;
		}
		Image image = GnomeUtil.GetIcon(name, 48);
		if (image == null)
		{
			switch (internal_mime_type)
			{
			case "unknown/unknown":
				image = ResourceImageLoader.Get("text-x-generic.png");
				break;
			case "inode/directory":
				image = ResourceImageLoader.Get("folder.png");
				break;
			case "directory/home":
				image = ResourceImageLoader.Get("user-home.png");
				break;
			case "desktop/desktop":
				image = ResourceImageLoader.Get("user-desktop.png");
				break;
			case "recently/recently":
				image = ResourceImageLoader.Get("document-open.png");
				break;
			case "workplace/workplace":
				image = ResourceImageLoader.Get("computer.png");
				break;
			case "network/network":
			case "nfs/nfs":
			case "smb/smb":
				image = ResourceImageLoader.Get("folder-remote.png");
				break;
			case "harddisk/harddisk":
			case "cdrom/cdrom":
			case "removable/removable":
				image = ResourceImageLoader.Get("text-x-generic.png");
				break;
			}
		}
		if (image != null)
		{
			num = MimeIconEngine.SmallIcons.Images.Add(image, Color.Transparent);
			MimeIconEngine.LargeIcons.Images.Add(image, Color.Transparent);
			MimeIconEngine.MimeIconIndex.Add(internal_mime_type, num);
		}
	}

	public override object AddAndGetIconIndex(string filename, string mime_type)
	{
		int num = -1;
		Image icon = GnomeUtil.GetIcon(filename, mime_type, 48);
		if (icon != null)
		{
			num = MimeIconEngine.SmallIcons.Images.Add(icon, Color.Transparent);
			MimeIconEngine.LargeIcons.Images.Add(icon, Color.Transparent);
			MimeIconEngine.MimeIconIndex.Add(mime_type, num);
		}
		return num;
	}

	public override object AddAndGetIconIndex(string mime_type)
	{
		int num = -1;
		Image icon = GnomeUtil.GetIcon(mime_type, 48);
		if (icon != null)
		{
			num = MimeIconEngine.SmallIcons.Images.Add(icon, Color.Transparent);
			MimeIconEngine.LargeIcons.Images.Add(icon, Color.Transparent);
			MimeIconEngine.MimeIconIndex.Add(mime_type, num);
		}
		return num;
	}
}

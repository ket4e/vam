using System.Collections;
using System.Drawing;

namespace System.Windows.Forms;

internal class MimeIconEngine
{
	public static ImageList SmallIcons;

	public static ImageList LargeIcons;

	private static EPlatformHandler platform;

	internal static Hashtable MimeIconIndex;

	private static PlatformMimeIconHandler platformMimeHandler;

	private static object lock_object;

	static MimeIconEngine()
	{
		SmallIcons = new ImageList();
		LargeIcons = new ImageList();
		platform = EPlatformHandler.Default;
		MimeIconIndex = new Hashtable();
		platformMimeHandler = null;
		lock_object = new object();
		SmallIcons.ColorDepth = ColorDepth.Depth32Bit;
		SmallIcons.TransparentColor = Color.Transparent;
		LargeIcons.ColorDepth = ColorDepth.Depth32Bit;
		LargeIcons.TransparentColor = Color.Transparent;
		string environmentVariable = Environment.GetEnvironmentVariable("DESKTOP_SESSION");
		if (environmentVariable != null)
		{
			environmentVariable = environmentVariable.ToUpper();
			if (environmentVariable == "DEFAULT")
			{
				string environmentVariable2 = Environment.GetEnvironmentVariable("GNOME_DESKTOP_SESSION_ID");
				if (environmentVariable2 != null)
				{
					environmentVariable = "GNOME";
				}
			}
		}
		else
		{
			environmentVariable = string.Empty;
		}
		if (Mime.MimeAvailable && environmentVariable == "GNOME")
		{
			SmallIcons.ImageSize = new Size(24, 24);
			LargeIcons.ImageSize = new Size(48, 48);
			platformMimeHandler = new GnomeHandler();
			if (platformMimeHandler.Start() == MimeExtensionHandlerStatus.OK)
			{
				platform = EPlatformHandler.GNOME;
				return;
			}
			LargeIcons.Images.Clear();
			SmallIcons.Images.Clear();
			platformMimeHandler = new PlatformDefaultHandler();
			platformMimeHandler.Start();
		}
		else
		{
			SmallIcons.ImageSize = new Size(16, 16);
			LargeIcons.ImageSize = new Size(48, 48);
			platformMimeHandler = new PlatformDefaultHandler();
			platformMimeHandler.Start();
		}
	}

	public static int GetIconIndexForFile(string full_filename)
	{
		lock (lock_object)
		{
			if (platform == EPlatformHandler.Default)
			{
				return (int)MimeIconIndex["unknown/unknown"];
			}
			string mimeTypeForFile = Mime.GetMimeTypeForFile(full_filename);
			object obj = GetIconIndex(mimeTypeForFile);
			if (obj == null)
			{
				int num = full_filename.IndexOf(':');
				if (num > 1)
				{
					obj = MimeIconIndex["unknown/unknown"];
				}
				else
				{
					obj = platformMimeHandler.AddAndGetIconIndex(full_filename, mimeTypeForFile);
					if (obj == null)
					{
						obj = MimeIconIndex["unknown/unknown"];
					}
				}
			}
			return (int)obj;
		}
	}

	public static int GetIconIndexForMimeType(string mime_type)
	{
		lock (lock_object)
		{
			if (platform == EPlatformHandler.Default)
			{
				if (mime_type == "inode/directory")
				{
					return (int)MimeIconIndex["inode/directory"];
				}
				return (int)MimeIconIndex["unknown/unknown"];
			}
			object obj = GetIconIndex(mime_type);
			if (obj == null)
			{
				obj = platformMimeHandler.AddAndGetIconIndex(mime_type);
				if (obj == null)
				{
					obj = MimeIconIndex["unknown/unknown"];
				}
			}
			return (int)obj;
		}
	}

	public static Image GetIconForMimeTypeAndSize(string mime_type, Size size)
	{
		lock (lock_object)
		{
			object iconIndex = GetIconIndex(mime_type);
			return new Bitmap(LargeIcons.Images[(int)iconIndex], size);
		}
	}

	internal static void AddIconByImage(string mime_type, Image image)
	{
		int num = SmallIcons.Images.Add(image, Color.Transparent);
		LargeIcons.Images.Add(image, Color.Transparent);
		MimeIconIndex.Add(mime_type, num);
	}

	private static object GetIconIndex(string mime_type)
	{
		object obj = null;
		if (mime_type != null)
		{
			obj = MimeIconIndex[mime_type];
			if (obj == null)
			{
				string mimeAlias = Mime.GetMimeAlias(mime_type);
				if (mimeAlias != null)
				{
					string[] array = mimeAlias.Split(',');
					for (int i = 0; i < array.Length; i++)
					{
						obj = MimeIconIndex[array[i]];
						if (obj != null)
						{
							return obj;
						}
					}
				}
				string text = Mime.SubClasses[mime_type];
				if (text != null)
				{
					obj = MimeIconIndex[text];
					if (obj != null)
					{
						return obj;
					}
				}
				string key = mime_type.Substring(0, mime_type.IndexOf('/'));
				return MimeIconIndex[key];
			}
		}
		return obj;
	}
}

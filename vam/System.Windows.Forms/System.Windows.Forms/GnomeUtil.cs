using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

internal class GnomeUtil
{
	private enum GnomeIconLookupFlags
	{
		GNOME_ICON_LOOKUP_FLAGS_NONE = 0,
		GNOME_ICON_LOOKUP_FLAGS_EMBEDDING_TEXT = 1,
		GNOME_ICON_LOOKUP_FLAGS_SHOW_SMALL_IMAGES_AS_THEMSELVES = 2,
		GNOME_ICON_LOOKUP_FLAGS_ALLOW_SVG_AS_THEMSELVES = 4
	}

	private enum GtkIconLookupFlags
	{
		GTK_ICON_LOOKUP_NO_SVG = 1,
		GTK_ICON_LOOKUP_FORCE_SVG = 2,
		GTK_ICON_LOOKUP_USE_BUILTIN = 4
	}

	private const string libgdk = "libgdk-x11-2.0.so.0";

	private const string libgdk_pixbuf = "libgdk_pixbuf-2.0.so.0";

	private const string libgtk = "libgtk-x11-2.0.so.0";

	private const string libglib = "libglib-2.0.so.0";

	private const string libgobject = "libgobject-2.0.so.0";

	private const string libgnomeui = "libgnomeui-2.so.0";

	private const string librsvg = "librsvg-2.so.2";

	private static bool inited = false;

	private static IntPtr default_icon_theme = IntPtr.Zero;

	[DllImport("librsvg-2.so.2")]
	private static extern IntPtr rsvg_pixbuf_from_file_at_size(string file_name, int width, int height, out IntPtr error);

	[DllImport("libgdk_pixbuf-2.0.so.0")]
	private static extern bool gdk_pixbuf_save_to_buffer(IntPtr pixbuf, out IntPtr buffer, out UIntPtr buffer_size, string type, out IntPtr error, IntPtr option_dummy);

	[DllImport("libglib-2.0.so.0")]
	private static extern void g_free(IntPtr mem);

	[DllImport("libgdk-x11-2.0.so.0")]
	private static extern bool gdk_init_check(IntPtr argc, IntPtr argv);

	[DllImport("libgobject-2.0.so.0")]
	private static extern void g_object_unref(IntPtr nativeObject);

	[DllImport("libgnomeui-2.so.0")]
	private static extern string gnome_icon_lookup(IntPtr icon_theme, IntPtr thumbnail_factory, string file_uri, string custom_icon, IntPtr file_info, string mime_type, GnomeIconLookupFlags flags, IntPtr result);

	[DllImport("libgtk-x11-2.0.so.0")]
	private static extern IntPtr gtk_icon_theme_get_default();

	[DllImport("libgtk-x11-2.0.so.0")]
	private static extern IntPtr gtk_icon_theme_load_icon(IntPtr icon_theme, string icon_name, int size, GtkIconLookupFlags flags, out IntPtr error);

	[DllImport("libgtk-x11-2.0.so.0")]
	private static extern bool gtk_icon_theme_has_icon(IntPtr icon_theme, string icon_name);

	private static void Init()
	{
		gdk_init_check(IntPtr.Zero, IntPtr.Zero);
		inited = true;
		default_icon_theme = gtk_icon_theme_get_default();
	}

	public static Image GetIcon(string file_name, string mime_type, int size)
	{
		if (!inited)
		{
			Init();
		}
		Uri uri = new Uri(file_name);
		string icon_name = gnome_icon_lookup(default_icon_theme, IntPtr.Zero, uri.AbsoluteUri, null, IntPtr.Zero, mime_type, GnomeIconLookupFlags.GNOME_ICON_LOOKUP_FLAGS_NONE, IntPtr.Zero);
		IntPtr error = IntPtr.Zero;
		IntPtr pixbuf = gtk_icon_theme_load_icon(default_icon_theme, icon_name, size, GtkIconLookupFlags.GTK_ICON_LOOKUP_USE_BUILTIN, out error);
		if (error != IntPtr.Zero)
		{
			return null;
		}
		return GdkPixbufToImage(pixbuf);
	}

	public static Image GetIcon(string icon, int size)
	{
		if (!inited)
		{
			Init();
		}
		IntPtr error = IntPtr.Zero;
		IntPtr pixbuf = gtk_icon_theme_load_icon(default_icon_theme, icon, size, GtkIconLookupFlags.GTK_ICON_LOOKUP_USE_BUILTIN, out error);
		if (error != IntPtr.Zero)
		{
			return null;
		}
		return GdkPixbufToImage(pixbuf);
	}

	public static Image GdkPixbufToImage(IntPtr pixbuf)
	{
		IntPtr error = IntPtr.Zero;
		string type = "png";
		if (!gdk_pixbuf_save_to_buffer(pixbuf, out var buffer, out var buffer_size, type, out error, IntPtr.Zero))
		{
			return null;
		}
		int num = (int)(ulong)buffer_size;
		byte[] array = new byte[num];
		Marshal.Copy(buffer, array, 0, num);
		g_free(buffer);
		g_object_unref(pixbuf);
		Image image = null;
		MemoryStream stream = new MemoryStream(array);
		return Image.FromStream(stream);
	}

	public static Image GetSVGasImage(string filename, int width, int height)
	{
		if (!inited)
		{
			Init();
		}
		if (!File.Exists(filename))
		{
			return null;
		}
		IntPtr error = IntPtr.Zero;
		IntPtr pixbuf = rsvg_pixbuf_from_file_at_size(filename, width, height, out error);
		if (error != IntPtr.Zero)
		{
			return null;
		}
		return GdkPixbufToImage(pixbuf);
	}

	public static bool HasImage(string name)
	{
		if (!inited)
		{
			Init();
		}
		return gtk_icon_theme_has_icon(default_icon_theme, name);
	}
}

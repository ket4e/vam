using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

internal class X11DesktopColors
{
	internal struct GdkColorStruct
	{
		internal int pixel;

		internal short red;

		internal short green;

		internal short blue;
	}

	internal struct GObjectStruct
	{
		public IntPtr Instance;

		public IntPtr ref_count;

		public IntPtr data;
	}

	internal struct GtkStyleStruct
	{
		internal GObjectStruct obj;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		internal GdkColorStruct[] fg;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		internal GdkColorStruct[] bg;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		internal GdkColorStruct[] light;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		internal GdkColorStruct[] dark;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		internal GdkColorStruct[] mid;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		internal GdkColorStruct[] text;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		internal GdkColorStruct[] baseclr;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		internal GdkColorStruct[] text_aa;

		internal GdkColorStruct black;

		internal GdkColorStruct white;
	}

	private enum Desktop
	{
		Gtk,
		KDE,
		Unknown
	}

	private const string libgdk = "libgdk-x11-2.0.so.0";

	private const string libgtk = "libgtk-x11-2.0.so.0";

	private static Desktop desktop;

	static X11DesktopColors()
	{
		FindDesktopEnvironment();
		switch (desktop)
		{
		case Desktop.Gtk:
			try
			{
				GtkInit();
				IntPtr raw = gtk_invisible_new();
				gtk_widget_ensure_style(raw);
				IntPtr ptr = gtk_widget_get_style(raw);
				GtkStyleStruct gtkStyleStruct = (GtkStyleStruct)Marshal.PtrToStructure(ptr, typeof(GtkStyleStruct));
				ThemeEngine.Current.ColorControl = ColorFromGdkColor(gtkStyleStruct.bg[0]);
				ThemeEngine.Current.ColorControlText = ColorFromGdkColor(gtkStyleStruct.fg[0]);
				ThemeEngine.Current.ColorControlDark = ColorFromGdkColor(gtkStyleStruct.dark[0]);
				ThemeEngine.Current.ColorControlLight = ColorFromGdkColor(gtkStyleStruct.light[0]);
				ThemeEngine.Current.ColorControlLightLight = ControlPaint.Light(ColorFromGdkColor(gtkStyleStruct.light[0]));
				ThemeEngine.Current.ColorControlDarkDark = ControlPaint.Dark(ColorFromGdkColor(gtkStyleStruct.dark[0]));
				raw = gtk_menu_new();
				gtk_widget_ensure_style(raw);
				ptr = gtk_widget_get_style(raw);
				gtkStyleStruct = (GtkStyleStruct)Marshal.PtrToStructure(ptr, typeof(GtkStyleStruct));
				ThemeEngine.Current.ColorMenu = ColorFromGdkColor(gtkStyleStruct.bg[0]);
				ThemeEngine.Current.ColorMenuText = ColorFromGdkColor(gtkStyleStruct.text[0]);
				break;
			}
			catch (DllNotFoundException)
			{
				Console.Error.WriteLine("Gtk not found (missing LD_LIBRARY_PATH to libgtk-x11-2.0.so.0?), using built-in colorscheme");
				break;
			}
			catch
			{
				Console.Error.WriteLine("Gtk colorscheme read failure, using built-in colorscheme");
				break;
			}
		case Desktop.KDE:
			if (!ReadKDEColorsheme())
			{
				Console.Error.WriteLine("KDE colorscheme read failure, using built-in colorscheme");
			}
			break;
		}
	}

	private static void GtkInit()
	{
		gtk_init_check(IntPtr.Zero, IntPtr.Zero);
	}

	private static void FindDesktopEnvironment()
	{
		desktop = Desktop.Gtk;
		string environmentVariable = Environment.GetEnvironmentVariable("DESKTOP_SESSION");
		if (environmentVariable == null)
		{
			return;
		}
		environmentVariable = environmentVariable.ToUpper();
		if (environmentVariable == "DEFAULT")
		{
			string environmentVariable2 = Environment.GetEnvironmentVariable("KDE_FULL_SESSION");
			if (environmentVariable2 != null)
			{
				desktop = Desktop.KDE;
			}
		}
		else if (environmentVariable.StartsWith("KDE"))
		{
			desktop = Desktop.KDE;
		}
	}

	internal static void Initialize()
	{
	}

	private static Color ColorFromGdkColor(GdkColorStruct gtkcolor)
	{
		return Color.FromArgb(255, (gtkcolor.red >> 8) & 0xFF, (gtkcolor.green >> 8) & 0xFF, (gtkcolor.blue >> 8) & 0xFF);
	}

	private static bool ReadKDEColorsheme()
	{
		string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/.kde/share/config/kdeglobals";
		if (!File.Exists(path))
		{
			return false;
		}
		StreamReader streamReader = new StreamReader(path);
		for (string text = streamReader.ReadLine(); text != null; text = streamReader.ReadLine())
		{
			text = text.Trim();
			if (text.StartsWith("background="))
			{
				Color colorFromKDEString = GetColorFromKDEString(text);
				if (colorFromKDEString != Color.Empty)
				{
					ThemeEngine.Current.ColorControl = colorFromKDEString;
					ThemeEngine.Current.ColorMenu = colorFromKDEString;
				}
			}
			else if (text.StartsWith("foreground="))
			{
				Color colorFromKDEString = GetColorFromKDEString(text);
				if (colorFromKDEString != Color.Empty)
				{
					ThemeEngine.Current.ColorControlText = colorFromKDEString;
					ThemeEngine.Current.ColorMenuText = colorFromKDEString;
				}
			}
			else if (text.StartsWith("selectBackground"))
			{
				Color colorFromKDEString = GetColorFromKDEString(text);
				if (colorFromKDEString != Color.Empty)
				{
					ThemeEngine.Current.ColorHighlight = colorFromKDEString;
				}
			}
			else if (text.StartsWith("selectForeground"))
			{
				Color colorFromKDEString = GetColorFromKDEString(text);
				if (colorFromKDEString != Color.Empty)
				{
					ThemeEngine.Current.ColorHighlightText = colorFromKDEString;
				}
			}
		}
		streamReader.Close();
		return true;
	}

	private static Color GetColorFromKDEString(string line)
	{
		string[] array = line.Split('=');
		if (array.Length > 0)
		{
			line = array[1];
			array = line.Split(',');
			if (array.Length == 3)
			{
				int red = Convert.ToInt32(array[0]);
				int green = Convert.ToInt32(array[1]);
				int blue = Convert.ToInt32(array[2]);
				return Color.FromArgb(red, green, blue);
			}
		}
		return Color.Empty;
	}

	[DllImport("libgtk-x11-2.0.so.0")]
	private static extern bool gtk_init_check(IntPtr argc, IntPtr argv);

	[DllImport("libgdk-x11-2.0.so.0")]
	internal static extern IntPtr gdk_display_manager_get();

	[DllImport("libgdk-x11-2.0.so.0")]
	internal static extern IntPtr gdk_display_manager_get_default_display(IntPtr display_manager);

	[DllImport("libgtk-x11-2.0.so.0")]
	private static extern IntPtr gtk_invisible_new();

	[DllImport("libgtk-x11-2.0.so.0")]
	private static extern IntPtr gtk_menu_new();

	[DllImport("libgtk-x11-2.0.so.0")]
	private static extern void gtk_widget_ensure_style(IntPtr raw);

	[DllImport("libgtk-x11-2.0.so.0")]
	private static extern IntPtr gtk_widget_get_style(IntPtr raw);
}

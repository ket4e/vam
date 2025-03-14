namespace System.Windows.Forms;

public class DataFormats
{
	public class Format
	{
		private static readonly object lockobj = new object();

		private static Format formats;

		private string name;

		private int id;

		private Format next;

		internal bool is_serializable;

		public int Id => id;

		public string Name => name;

		internal Format Next => next;

		internal static Format List => formats;

		public Format(string name, int id)
		{
			this.name = name;
			this.id = id;
			lock (lockobj)
			{
				if (formats == null)
				{
					formats = this;
					return;
				}
				Format format = formats;
				while (format.next != null)
				{
					format = format.next;
				}
				format.next = this;
			}
		}

		internal static Format Add(string name)
		{
			Format format = Find(name);
			if (format == null)
			{
				IntPtr handle = XplatUI.ClipboardOpen(primary_selection: false);
				format = new Format(name, XplatUI.ClipboardGetID(handle, name));
				XplatUI.ClipboardClose(handle);
			}
			return format;
		}

		internal static Format Add(int id)
		{
			Format format = Find(id);
			if (format == null)
			{
				format = new Format("Format" + id, id);
			}
			return format;
		}

		internal static Format Find(int id)
		{
			Format format = formats;
			while (format != null && format.Id != id)
			{
				format = format.next;
			}
			return format;
		}

		internal static Format Find(string name)
		{
			Format format = formats;
			while (format != null && !format.Name.Equals(name))
			{
				format = format.next;
			}
			return format;
		}
	}

	public static readonly string Bitmap = "Bitmap";

	public static readonly string CommaSeparatedValue = "Csv";

	public static readonly string Dib = "DeviceIndependentBitmap";

	public static readonly string Dif = "DataInterchangeFormat";

	public static readonly string EnhancedMetafile = "EnhancedMetafile";

	public static readonly string FileDrop = "FileDrop";

	public static readonly string Html = "HTML Format";

	public static readonly string Locale = "Locale";

	public static readonly string MetafilePict = "MetaFilePict";

	public static readonly string OemText = "OEMText";

	public static readonly string Palette = "Palette";

	public static readonly string PenData = "PenData";

	public static readonly string Riff = "RiffAudio";

	public static readonly string Rtf = "Rich Text Format";

	public static readonly string Serializable = "WindowsForms10PersistentObject";

	public static readonly string StringFormat = "System.String";

	public static readonly string SymbolicLink = "SymbolicLink";

	public static readonly string Text = "Text";

	public static readonly string Tiff = "Tiff";

	public static readonly string UnicodeText = "UnicodeText";

	public static readonly string WaveAudio = "WaveAudio";

	private static object lock_object = new object();

	private static bool initialized;

	private DataFormats()
	{
	}

	internal static bool ContainsFormat(int id)
	{
		lock (lock_object)
		{
			if (!initialized)
			{
				Init();
			}
			return Format.Find(id) != null;
		}
	}

	public static Format GetFormat(int id)
	{
		lock (lock_object)
		{
			if (!initialized)
			{
				Init();
			}
			return Format.Find(id);
		}
	}

	public static Format GetFormat(string format)
	{
		lock (lock_object)
		{
			if (!initialized)
			{
				Init();
			}
			return Format.Add(format);
		}
	}

	private static void Init()
	{
		if (!initialized)
		{
			IntPtr handle = XplatUI.ClipboardOpen(primary_selection: false);
			new Format(Text, XplatUI.ClipboardGetID(handle, Text));
			new Format(Bitmap, XplatUI.ClipboardGetID(handle, Bitmap));
			new Format(MetafilePict, XplatUI.ClipboardGetID(handle, MetafilePict));
			new Format(SymbolicLink, XplatUI.ClipboardGetID(handle, SymbolicLink));
			new Format(Dif, XplatUI.ClipboardGetID(handle, Dif));
			new Format(Tiff, XplatUI.ClipboardGetID(handle, Tiff));
			new Format(OemText, XplatUI.ClipboardGetID(handle, OemText));
			new Format(Dib, XplatUI.ClipboardGetID(handle, Dib));
			new Format(Palette, XplatUI.ClipboardGetID(handle, Palette));
			new Format(PenData, XplatUI.ClipboardGetID(handle, PenData));
			new Format(Riff, XplatUI.ClipboardGetID(handle, Riff));
			new Format(WaveAudio, XplatUI.ClipboardGetID(handle, WaveAudio));
			new Format(UnicodeText, XplatUI.ClipboardGetID(handle, UnicodeText));
			new Format(EnhancedMetafile, XplatUI.ClipboardGetID(handle, EnhancedMetafile));
			new Format(FileDrop, XplatUI.ClipboardGetID(handle, FileDrop));
			new Format(Locale, XplatUI.ClipboardGetID(handle, Locale));
			new Format(CommaSeparatedValue, XplatUI.ClipboardGetID(handle, CommaSeparatedValue));
			new Format(Html, XplatUI.ClipboardGetID(handle, Html));
			new Format(Rtf, XplatUI.ClipboardGetID(handle, Rtf));
			new Format(Serializable, XplatUI.ClipboardGetID(handle, Serializable));
			new Format(StringFormat, XplatUI.ClipboardGetID(handle, StringFormat));
			XplatUI.ClipboardClose(handle);
			initialized = true;
		}
	}
}

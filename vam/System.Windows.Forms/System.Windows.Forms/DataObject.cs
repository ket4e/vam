using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace System.Windows.Forms;

[ClassInterface(ClassInterfaceType.None)]
public class DataObject : System.Runtime.InteropServices.ComTypes.IDataObject, IDataObject
{
	private class Entry
	{
		private string type;

		private object data;

		private bool autoconvert;

		internal Entry next;

		public object Data
		{
			get
			{
				return data;
			}
			set
			{
				data = value;
			}
		}

		public bool AutoConvert
		{
			get
			{
				return autoconvert;
			}
			set
			{
				autoconvert = value;
			}
		}

		internal Entry(string type, object data, bool autoconvert)
		{
			this.type = type;
			this.data = data;
			this.autoconvert = autoconvert;
		}

		public static int Count(Entry entries)
		{
			int num = 0;
			while (entries != null)
			{
				num++;
				entries = entries.next;
			}
			return num;
		}

		public static Entry Find(Entry entries, string type)
		{
			return Find(entries, type, only_convertible: false);
		}

		public static Entry Find(Entry entries, string type, bool only_convertible)
		{
			while (entries != null)
			{
				bool flag = true;
				if (only_convertible && !entries.autoconvert)
				{
					flag = false;
				}
				if (flag && string.Compare(entries.type, type, ignoreCase: true) == 0)
				{
					return entries;
				}
				entries = entries.next;
			}
			return null;
		}

		public static Entry FindConvertible(Entry entries, string type)
		{
			Entry entry = Find(entries, type);
			if (entry != null)
			{
				return entry;
			}
			if (type == DataFormats.StringFormat || type == DataFormats.Text || type == DataFormats.UnicodeText)
			{
				for (entry = entries; entry != null; entry = entry.next)
				{
					if (entry.type == DataFormats.StringFormat || entry.type == DataFormats.Text || entry.type == DataFormats.UnicodeText)
					{
						return entry;
					}
				}
			}
			return null;
		}

		public static string[] Entries(Entry entries, bool convertible)
		{
			ArrayList arrayList = new ArrayList(Count(entries));
			Entry entry = entries;
			if (convertible)
			{
				Entry entry2 = Find(entries, DataFormats.Text);
				Entry entry3 = Find(entries, DataFormats.UnicodeText);
				Entry entry4 = Find(entries, DataFormats.StringFormat);
				bool flag = entry2?.AutoConvert ?? false;
				bool flag2 = entry3?.AutoConvert ?? false;
				bool flag3 = entry4?.AutoConvert ?? false;
				if (flag || flag2 || flag3)
				{
					arrayList.Add(DataFormats.StringFormat);
					arrayList.Add(DataFormats.UnicodeText);
					arrayList.Add(DataFormats.Text);
				}
			}
			while (entry != null)
			{
				if (!arrayList.Contains(entry.type))
				{
					arrayList.Add(entry.type);
				}
				entry = entry.next;
			}
			string[] array = new string[arrayList.Count];
			for (int i = 0; i < arrayList.Count; i++)
			{
				array[i] = (string)arrayList[i];
			}
			return array;
		}
	}

	private Entry entries;

	public DataObject()
	{
		entries = null;
	}

	public DataObject(object data)
	{
		SetData(data);
	}

	public DataObject(string format, object data)
	{
		SetData(format, data);
	}

	int System.Runtime.InteropServices.ComTypes.IDataObject.DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection)
	{
		throw new NotImplementedException();
	}

	void System.Runtime.InteropServices.ComTypes.IDataObject.DUnadvise(int connection)
	{
		throw new NotImplementedException();
	}

	int System.Runtime.InteropServices.ComTypes.IDataObject.EnumDAdvise(out IEnumSTATDATA enumAdvise)
	{
		throw new NotImplementedException();
	}

	IEnumFORMATETC System.Runtime.InteropServices.ComTypes.IDataObject.EnumFormatEtc(DATADIR direction)
	{
		throw new NotImplementedException();
	}

	int System.Runtime.InteropServices.ComTypes.IDataObject.GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut)
	{
		throw new NotImplementedException();
	}

	void System.Runtime.InteropServices.ComTypes.IDataObject.GetData(ref FORMATETC format, out STGMEDIUM medium)
	{
		throw new NotImplementedException();
	}

	void System.Runtime.InteropServices.ComTypes.IDataObject.GetDataHere(ref FORMATETC format, ref STGMEDIUM medium)
	{
		throw new NotImplementedException();
	}

	int System.Runtime.InteropServices.ComTypes.IDataObject.QueryGetData(ref FORMATETC format)
	{
		throw new NotImplementedException();
	}

	void System.Runtime.InteropServices.ComTypes.IDataObject.SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release)
	{
		throw new NotImplementedException();
	}

	public virtual bool ContainsAudio()
	{
		return GetDataPresent(DataFormats.WaveAudio, autoConvert: true);
	}

	public virtual bool ContainsFileDropList()
	{
		return GetDataPresent(DataFormats.FileDrop, autoConvert: true);
	}

	public virtual bool ContainsImage()
	{
		return GetDataPresent(DataFormats.Bitmap, autoConvert: true);
	}

	public virtual bool ContainsText()
	{
		return GetDataPresent(DataFormats.UnicodeText, autoConvert: true);
	}

	public virtual bool ContainsText(TextDataFormat format)
	{
		if (!Enum.IsDefined(typeof(TextDataFormat), format))
		{
			throw new InvalidEnumArgumentException($"Enum argument value '{format}' is not valid for TextDataFormat");
		}
		return GetDataPresent(TextFormatToDataFormat(format), autoConvert: true);
	}

	public virtual Stream GetAudioStream()
	{
		return (Stream)GetData(DataFormats.WaveAudio, autoConvert: true);
	}

	public virtual object GetData(string format)
	{
		return GetData(format, autoConvert: true);
	}

	public virtual object GetData(string format, bool autoConvert)
	{
		return ((!autoConvert) ? Entry.Find(entries, format) : Entry.FindConvertible(entries, format))?.Data;
	}

	public virtual object GetData(Type format)
	{
		return GetData(format.FullName, autoConvert: true);
	}

	public virtual bool GetDataPresent(string format)
	{
		return GetDataPresent(format, autoConvert: true);
	}

	public virtual bool GetDataPresent(string format, bool autoConvert)
	{
		if (autoConvert)
		{
			return Entry.FindConvertible(entries, format) != null;
		}
		return Entry.Find(entries, format) != null;
	}

	public virtual bool GetDataPresent(Type format)
	{
		return GetDataPresent(format.FullName, autoConvert: true);
	}

	public virtual StringCollection GetFileDropList()
	{
		return (StringCollection)GetData(DataFormats.FileDrop, autoConvert: true);
	}

	public virtual string[] GetFormats()
	{
		return GetFormats(autoConvert: true);
	}

	public virtual string[] GetFormats(bool autoConvert)
	{
		return Entry.Entries(entries, autoConvert);
	}

	public virtual Image GetImage()
	{
		return (Image)GetData(DataFormats.Bitmap, autoConvert: true);
	}

	public virtual string GetText()
	{
		return (string)GetData(DataFormats.UnicodeText, autoConvert: true);
	}

	public virtual string GetText(TextDataFormat format)
	{
		if (!Enum.IsDefined(typeof(TextDataFormat), format))
		{
			throw new InvalidEnumArgumentException($"Enum argument value '{format}' is not valid for TextDataFormat");
		}
		return (string)GetData(TextFormatToDataFormat(format), autoConvert: false);
	}

	public virtual void SetAudio(byte[] audioBytes)
	{
		if (audioBytes == null)
		{
			throw new ArgumentNullException("audioBytes");
		}
		MemoryStream audio = new MemoryStream(audioBytes);
		SetAudio(audio);
	}

	public virtual void SetAudio(Stream audioStream)
	{
		if (audioStream == null)
		{
			throw new ArgumentNullException("audioStream");
		}
		SetData(DataFormats.WaveAudio, audioStream);
	}

	public virtual void SetData(object data)
	{
		SetData(data.GetType(), data);
	}

	public virtual void SetData(string format, bool autoConvert, object data)
	{
		Entry entry = Entry.Find(entries, format);
		if (entry == null)
		{
			entry = new Entry(format, data, autoConvert);
			lock (this)
			{
				if (entries == null)
				{
					entries = entry;
					return;
				}
				Entry next = entries;
				while (next.next != null)
				{
					next = next.next;
				}
				next.next = entry;
				return;
			}
		}
		entry.Data = data;
	}

	public virtual void SetData(string format, object data)
	{
		SetData(format, autoConvert: true, data);
	}

	public virtual void SetData(Type format, object data)
	{
		SetData(EnsureFormat(format), autoConvert: true, data);
	}

	[System.MonoInternalNote("Needs additional checks for valid paths, see MSDN")]
	public virtual void SetFileDropList(StringCollection filePaths)
	{
		if (filePaths == null)
		{
			throw new ArgumentNullException("filePaths");
		}
		SetData(DataFormats.FileDrop, filePaths);
	}

	public virtual void SetImage(Image image)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		SetData(DataFormats.Bitmap, image);
	}

	public virtual void SetText(string textData)
	{
		if (string.IsNullOrEmpty(textData))
		{
			throw new ArgumentNullException("text");
		}
		SetData(DataFormats.UnicodeText, textData);
	}

	public virtual void SetText(string textData, TextDataFormat format)
	{
		if (string.IsNullOrEmpty(textData))
		{
			throw new ArgumentNullException("text");
		}
		if (!Enum.IsDefined(typeof(TextDataFormat), format))
		{
			throw new InvalidEnumArgumentException($"Enum argument value '{format}' is not valid for TextDataFormat");
		}
		switch (format)
		{
		case TextDataFormat.Text:
			SetData(DataFormats.Text, textData);
			break;
		case TextDataFormat.UnicodeText:
			SetData(DataFormats.UnicodeText, textData);
			break;
		case TextDataFormat.Rtf:
			SetData(DataFormats.Rtf, textData);
			break;
		case TextDataFormat.Html:
			SetData(DataFormats.Html, textData);
			break;
		case TextDataFormat.CommaSeparatedValue:
			SetData(DataFormats.CommaSeparatedValue, textData);
			break;
		}
	}

	internal string EnsureFormat(string name)
	{
		DataFormats.Format format = DataFormats.Format.Find(name);
		if (format == null)
		{
			format = DataFormats.Format.Add(name);
		}
		return format.Name;
	}

	internal string EnsureFormat(Type type)
	{
		return EnsureFormat(type.FullName);
	}

	private string TextFormatToDataFormat(TextDataFormat format)
	{
		return format switch
		{
			TextDataFormat.UnicodeText => DataFormats.UnicodeText, 
			TextDataFormat.Rtf => DataFormats.Rtf, 
			TextDataFormat.Html => DataFormats.Html, 
			TextDataFormat.CommaSeparatedValue => DataFormats.CommaSeparatedValue, 
			_ => DataFormats.Text, 
		};
	}
}

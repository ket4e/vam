using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;

namespace System.Windows.Forms;

public sealed class Clipboard
{
	private Clipboard()
	{
	}

	private static bool ConvertToClipboardData(ref int type, object obj, out byte[] data)
	{
		data = null;
		return false;
	}

	private static bool ConvertFromClipboardData(int type, IntPtr data, out object obj)
	{
		obj = null;
		if (data == IntPtr.Zero)
		{
			return false;
		}
		return false;
	}

	public static void Clear()
	{
		IntPtr handle = XplatUI.ClipboardOpen(primary_selection: false);
		XplatUI.ClipboardStore(handle, null, 0, null);
	}

	public static bool ContainsAudio()
	{
		return ClipboardContainsFormat(DataFormats.WaveAudio);
	}

	public static bool ContainsData(string format)
	{
		return ClipboardContainsFormat(format);
	}

	public static bool ContainsFileDropList()
	{
		return ClipboardContainsFormat(DataFormats.FileDrop);
	}

	public static bool ContainsImage()
	{
		return ClipboardContainsFormat(DataFormats.Bitmap);
	}

	public static bool ContainsText()
	{
		return ClipboardContainsFormat(DataFormats.Text, DataFormats.UnicodeText);
	}

	public static bool ContainsText(TextDataFormat format)
	{
		return format switch
		{
			TextDataFormat.Text => ClipboardContainsFormat(DataFormats.Text), 
			TextDataFormat.UnicodeText => ClipboardContainsFormat(DataFormats.UnicodeText), 
			TextDataFormat.Rtf => ClipboardContainsFormat(DataFormats.Rtf), 
			TextDataFormat.Html => ClipboardContainsFormat(DataFormats.Html), 
			TextDataFormat.CommaSeparatedValue => ClipboardContainsFormat(DataFormats.CommaSeparatedValue), 
			_ => false, 
		};
	}

	public static Stream GetAudioStream()
	{
		IDataObject dataObject = GetDataObject();
		if (dataObject == null)
		{
			return null;
		}
		return (Stream)dataObject.GetData(DataFormats.WaveAudio, autoConvert: true);
	}

	public static object GetData(string format)
	{
		return GetDataObject()?.GetData(format, autoConvert: true);
	}

	public static IDataObject GetDataObject()
	{
		return GetDataObject(primary_selection: false);
	}

	public static StringCollection GetFileDropList()
	{
		IDataObject dataObject = GetDataObject();
		if (dataObject == null)
		{
			return null;
		}
		return (StringCollection)dataObject.GetData(DataFormats.FileDrop, autoConvert: true);
	}

	public static Image GetImage()
	{
		IDataObject dataObject = GetDataObject();
		if (dataObject == null)
		{
			return null;
		}
		return (Image)dataObject.GetData(DataFormats.Bitmap, autoConvert: true);
	}

	public static string GetText()
	{
		return GetText(TextDataFormat.UnicodeText);
	}

	public static string GetText(TextDataFormat format)
	{
		if (!Enum.IsDefined(typeof(TextDataFormat), format))
		{
			throw new InvalidEnumArgumentException($"Enum argument value '{format}' is not valid for TextDataFormat");
		}
		IDataObject dataObject = GetDataObject();
		if (dataObject == null)
		{
			return string.Empty;
		}
		string text = format switch
		{
			TextDataFormat.UnicodeText => (string)dataObject.GetData(DataFormats.UnicodeText, autoConvert: true), 
			TextDataFormat.Rtf => (string)dataObject.GetData(DataFormats.Rtf, autoConvert: true), 
			TextDataFormat.Html => (string)dataObject.GetData(DataFormats.Html, autoConvert: true), 
			TextDataFormat.CommaSeparatedValue => (string)dataObject.GetData(DataFormats.CommaSeparatedValue, autoConvert: true), 
			_ => (string)dataObject.GetData(DataFormats.Text, autoConvert: true), 
		};
		return (text != null) ? text : string.Empty;
	}

	public static void SetAudio(byte[] audioBytes)
	{
		if (audioBytes == null)
		{
			throw new ArgumentNullException("audioBytes");
		}
		MemoryStream audio = new MemoryStream(audioBytes);
		SetAudio(audio);
	}

	public static void SetAudio(Stream audioStream)
	{
		if (audioStream == null)
		{
			throw new ArgumentNullException("audioStream");
		}
		SetData(DataFormats.WaveAudio, audioStream);
	}

	public static void SetData(string format, object data)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		DataObject dataObject = new DataObject(format, data);
		SetDataObject(dataObject);
	}

	public static void SetDataObject(object data)
	{
		SetDataObject(data, copy: false);
	}

	public static void SetDataObject(object data, bool copy)
	{
		SetDataObject(data, copy, 10, 100);
	}

	internal static void SetDataObjectImpl(object data, bool copy)
	{
		XplatUI.ObjectToClipboard converter = ConvertToClipboardData;
		IntPtr handle = XplatUI.ClipboardOpen(primary_selection: false);
		XplatUI.ClipboardStore(handle, null, 0, null);
		int type = -1;
		if (data is IDataObject)
		{
			IDataObject dataObject = data as IDataObject;
			string[] formats = dataObject.GetFormats();
			for (int i = 0; i < formats.Length; i++)
			{
				DataFormats.Format format = DataFormats.GetFormat(formats[i]);
				if (format != null && format.Name != DataFormats.StringFormat)
				{
					type = format.Id;
				}
				object data2 = dataObject.GetData(formats[i]);
				if (IsDataSerializable(data2))
				{
					format.is_serializable = true;
				}
				XplatUI.ClipboardStore(handle, data2, type, converter);
			}
		}
		else
		{
			DataFormats.Format format = DataFormats.Format.Find(data.GetType().FullName);
			if (format != null && format.Name != DataFormats.StringFormat)
			{
				type = format.Id;
			}
			XplatUI.ClipboardStore(handle, data, type, converter);
		}
		XplatUI.ClipboardClose(handle);
	}

	private static bool IsDataSerializable(object obj)
	{
		if (obj is ISerializable)
		{
			return true;
		}
		AttributeCollection attributes = TypeDescriptor.GetAttributes(obj);
		return attributes[typeof(SerializableAttribute)] != null;
	}

	public static void SetDataObject(object data, bool copy, int retryTimes, int retryDelay)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (retryTimes < 0)
		{
			throw new ArgumentOutOfRangeException("retryTimes");
		}
		if (retryDelay < 0)
		{
			throw new ArgumentOutOfRangeException("retryDelay");
		}
		bool flag = true;
		do
		{
			flag = false;
			retryTimes--;
			try
			{
				SetDataObjectImpl(data, copy);
			}
			catch (ExternalException)
			{
				if (retryTimes <= 0)
				{
					throw;
				}
				flag = true;
				Thread.Sleep(retryDelay);
			}
		}
		while (flag && retryTimes > 0);
	}

	[System.MonoInternalNote("Needs additional checks for valid paths, see MSDN")]
	public static void SetFileDropList(StringCollection filePaths)
	{
		if (filePaths == null)
		{
			throw new ArgumentNullException("filePaths");
		}
		SetData(DataFormats.FileDrop, filePaths);
	}

	public static void SetImage(Image image)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		SetData(DataFormats.Bitmap, image);
	}

	public static void SetText(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			throw new ArgumentNullException("text");
		}
		SetData(DataFormats.UnicodeText, text);
	}

	public static void SetText(string text, TextDataFormat format)
	{
		if (string.IsNullOrEmpty(text))
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
			SetData(DataFormats.Text, text);
			break;
		case TextDataFormat.UnicodeText:
			SetData(DataFormats.UnicodeText, text);
			break;
		case TextDataFormat.Rtf:
			SetData(DataFormats.Rtf, text);
			break;
		case TextDataFormat.Html:
			SetData(DataFormats.Html, text);
			break;
		case TextDataFormat.CommaSeparatedValue:
			SetData(DataFormats.CommaSeparatedValue, text);
			break;
		}
	}

	internal static IDataObject GetDataObject(bool primary_selection)
	{
		XplatUI.ClipboardToObject converter = ConvertFromClipboardData;
		IntPtr handle = XplatUI.ClipboardOpen(primary_selection);
		int[] array = XplatUI.ClipboardAvailableFormats(handle);
		if (array == null)
		{
			return null;
		}
		DataObject dataObject = new DataObject();
		for (int i = 0; i < array.Length; i++)
		{
			DataFormats.Format format = DataFormats.GetFormat(array[i]);
			if (format == null)
			{
				continue;
			}
			object obj = XplatUI.ClipboardRetrieve(handle, array[i], converter);
			if (obj != null)
			{
				dataObject.SetData(format.Name, obj);
				if (format.Name == DataFormats.Dib)
				{
					dataObject.SetData(DataFormats.Bitmap, obj);
				}
			}
		}
		XplatUI.ClipboardClose(handle);
		return dataObject;
	}

	internal static bool ClipboardContainsFormat(params string[] formats)
	{
		IntPtr handle = XplatUI.ClipboardOpen(primary_selection: false);
		int[] array = XplatUI.ClipboardAvailableFormats(handle);
		if (array == null)
		{
			return false;
		}
		int[] array2 = array;
		foreach (int id in array2)
		{
			DataFormats.Format format = DataFormats.GetFormat(id);
			if (format != null && ((IList)formats).Contains((object)format.Name))
			{
				return true;
			}
		}
		return false;
	}
}

using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace System.Windows.Forms.CarbonInternal;

internal class FlavorHandler
{
	internal IntPtr flavorref;

	internal IntPtr dragref;

	internal IntPtr itemref;

	internal int size;

	internal uint flags;

	internal byte[] data;

	internal string fourcc;

	internal string DataString => Encoding.Default.GetString(data);

	internal byte[] DataArray => data;

	internal IntPtr DataPtr => (IntPtr)BitConverter.ToInt32(data, 0);

	internal bool Supported => fourcc switch
	{
		"furl" => true, 
		"mono" => true, 
		"mser" => true, 
		_ => false, 
	};

	internal FlavorHandler(IntPtr dragref, IntPtr itemref, uint counter)
	{
		GetFlavorType(dragref, itemref, counter, ref flavorref);
		GetFlavorFlags(dragref, itemref, flavorref, ref flags);
		byte[] bytes = BitConverter.GetBytes((int)flavorref);
		fourcc = $"{(char)bytes[3]}{(char)bytes[2]}{(char)bytes[1]}{(char)bytes[0]}";
		this.dragref = dragref;
		this.itemref = itemref;
		GetData();
	}

	internal void GetData()
	{
		GetFlavorDataSize(dragref, itemref, flavorref, ref size);
		data = new byte[size];
		GetFlavorData(dragref, itemref, flavorref, data, ref size, 0u);
	}

	internal DataObject Convert(ArrayList flavorlist)
	{
		return fourcc switch
		{
			"furl" => ConvertToFileDrop(flavorlist), 
			"mono" => ConvertToObject(flavorlist), 
			"mser" => DeserializeObject(flavorlist), 
			_ => new DataObject(), 
		};
	}

	internal DataObject DeserializeObject(ArrayList flavorlist)
	{
		DataObject dataObject = new DataObject();
		MemoryStream memoryStream = new MemoryStream(DataArray);
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		if (memoryStream.Length == 0L)
		{
			return dataObject;
		}
		memoryStream.Seek(0L, SeekOrigin.Begin);
		dataObject.SetData(binaryFormatter.Deserialize(memoryStream));
		return dataObject;
	}

	internal DataObject ConvertToObject(ArrayList flavorlist)
	{
		DataObject dataObject = new DataObject();
		foreach (FlavorHandler item in flavorlist)
		{
			dataObject.SetData(((GCHandle)item.DataPtr).Target);
		}
		return dataObject;
	}

	internal DataObject ConvertToFileDrop(ArrayList flavorlist)
	{
		DataObject dataObject = new DataObject();
		ArrayList arrayList = new ArrayList();
		foreach (FlavorHandler item in flavorlist)
		{
			try
			{
				arrayList.Add(new Uri(item.DataString).LocalPath);
			}
			catch
			{
			}
		}
		string[] array = (string[])arrayList.ToArray(typeof(string));
		if (array.Length < 1)
		{
			return dataObject;
		}
		dataObject.SetData(DataFormats.FileDrop, array);
		dataObject.SetData("FileName", array[0]);
		dataObject.SetData("FileNameW", array[0]);
		return dataObject;
	}

	public override string ToString()
	{
		return fourcc;
	}

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int GetFlavorDataSize(IntPtr dragref, IntPtr itemref, IntPtr flavorref, ref int size);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int GetFlavorData(IntPtr dragref, IntPtr itemref, IntPtr flavorref, [In][Out] byte[] data, ref int size, uint offset);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int GetFlavorFlags(IntPtr dragref, IntPtr itemref, IntPtr flavorref, ref uint flags);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int GetFlavorType(IntPtr dragref, IntPtr itemref, uint index, ref IntPtr flavor);
}

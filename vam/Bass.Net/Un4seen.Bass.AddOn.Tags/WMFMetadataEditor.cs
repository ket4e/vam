using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace Un4seen.Bass.AddOn.Tags;

[Serializable]
[SuppressUnmanagedCodeSecurity]
internal class WMFMetadataEditor
{
	private IWMHeaderInfo3 HeaderInfo3;

	[DllImport("WMVCore.dll", SetLastError = true)]
	internal static extern uint WMCreateEditor([MarshalAs(UnmanagedType.Interface)] out IWMMetadataEditor ppMetadataEditor);

	public WMFMetadataEditor(IWMHeaderInfo3 headerInfo3)
	{
		HeaderInfo3 = headerInfo3;
	}

	internal List<TagPicture> GetAllPictures()
	{
		if (HeaderInfo3 == null)
		{
			return null;
		}
		List<TagPicture> list = null;
		List<Tag> list2 = WMGetAllAttrib("WM/Picture");
		if (list2 != null && list2.Count > 0)
		{
			list = new List<TagPicture>(list2.Count);
			foreach (Tag item in list2)
			{
				TagPicture picture = GetPicture(item);
				if (picture != null)
				{
					list.Add(picture);
				}
			}
		}
		return list;
	}

	private TagPicture GetPicture(Tag pTag)
	{
		TagPicture result = null;
		string text = "Unknown";
		TagPicture.PICTURE_TYPE pICTURE_TYPE = TagPicture.PICTURE_TYPE.Unknown;
		string text2 = "";
		MemoryStream memoryStream = null;
		BinaryReader binaryReader = null;
		if (pTag.Name == "WM/Picture")
		{
			try
			{
				memoryStream = new MemoryStream((byte[])pTag);
				binaryReader = new BinaryReader(memoryStream);
				text = ((!Utils.Is64Bit) ? Marshal.PtrToStringUni(new IntPtr(binaryReader.ReadInt32())) : Marshal.PtrToStringUni(new IntPtr(binaryReader.ReadInt64())));
				byte b = binaryReader.ReadByte();
				try
				{
					pICTURE_TYPE = (TagPicture.PICTURE_TYPE)b;
				}
				catch
				{
					pICTURE_TYPE = TagPicture.PICTURE_TYPE.Unknown;
				}
				text2 = ((!Utils.Is64Bit) ? Marshal.PtrToStringUni(new IntPtr(binaryReader.ReadInt32())) : Marshal.PtrToStringUni(new IntPtr(binaryReader.ReadInt64())));
				int num = binaryReader.ReadInt32();
				byte[] array = new byte[num];
				if (Utils.Is64Bit)
				{
					Marshal.Copy(new IntPtr(binaryReader.ReadInt64()), array, 0, num);
				}
				else
				{
					Marshal.Copy(new IntPtr(binaryReader.ReadInt32()), array, 0, num);
				}
				result = new TagPicture(pTag.Index, text, pICTURE_TYPE, text2, array);
			}
			catch
			{
			}
			finally
			{
				binaryReader?.Close();
				if (memoryStream != null)
				{
					memoryStream.Close();
					memoryStream.Dispose();
				}
			}
		}
		return result;
	}

	internal List<Tag> ReadAllAttributes()
	{
		if (HeaderInfo3 == null)
		{
			return null;
		}
		List<Tag> list = new List<Tag>(42);
		try
		{
			ushort pwLangIndex = 0;
			ushort[] pwIndices = null;
			ushort pwCount = 0;
			HeaderInfo3.GetAttributeIndices(0, null, ref pwLangIndex, pwIndices, ref pwCount);
			ushort[] array = new ushort[pwCount];
			HeaderInfo3.GetAttributeIndices(0, null, ref pwLangIndex, array, ref pwCount);
			if (array != null && array.Length != 0)
			{
				ushort[] array2 = array;
				foreach (ushort num in array2)
				{
					string pwszName = null;
					object obj = null;
					ushort pwNameLen = 0;
					uint pdwDataLength = 0u;
					try
					{
						HeaderInfo3.GetAttributeByIndexEx(0, num, pwszName, ref pwNameLen, out var pType, out pwLangIndex, IntPtr.Zero, ref pdwDataLength);
						pwszName = new string('\0', pwNameLen);
						switch (pType)
						{
						case WMT_ATTR_DATATYPE.WMT_TYPE_DWORD:
						case WMT_ATTR_DATATYPE.WMT_TYPE_BOOL:
							obj = 0u;
							break;
						case WMT_ATTR_DATATYPE.WMT_TYPE_GUID:
							obj = Guid.NewGuid();
							break;
						case WMT_ATTR_DATATYPE.WMT_TYPE_QWORD:
							obj = 0uL;
							break;
						case WMT_ATTR_DATATYPE.WMT_TYPE_WORD:
							obj = (ushort)0;
							break;
						case WMT_ATTR_DATATYPE.WMT_TYPE_STRING:
						case WMT_ATTR_DATATYPE.WMT_TYPE_BINARY:
							obj = new byte[pdwDataLength];
							break;
						default:
							throw new InvalidOperationException($"Not supported data type: {pType.ToString()}");
						}
						GCHandle gCHandle = GCHandle.Alloc(obj, GCHandleType.Pinned);
						try
						{
							IntPtr intPtr = gCHandle.AddrOfPinnedObject();
							HeaderInfo3.GetAttributeByIndexEx(0, num, pwszName, ref pwNameLen, out pType, out pwLangIndex, intPtr, ref pdwDataLength);
							switch (pType)
							{
							case WMT_ATTR_DATATYPE.WMT_TYPE_STRING:
								obj = Marshal.PtrToStringUni(intPtr);
								break;
							case WMT_ATTR_DATATYPE.WMT_TYPE_BOOL:
								obj = (uint)obj != 0;
								break;
							}
							list.Add(new Tag(num, pwszName, pType, obj));
						}
						finally
						{
							gCHandle.Free();
						}
					}
					catch
					{
					}
				}
			}
		}
		catch
		{
		}
		return list;
	}

	private List<Tag> WMGetAllAttrib(string pAttribName)
	{
		List<Tag> list = new List<Tag>();
		try
		{
			if (!pAttribName.EndsWith("\0"))
			{
				pAttribName += "\0";
			}
			ushort pwLangIndex = 0;
			ushort[] pwIndices = null;
			ushort pwCount = 0;
			HeaderInfo3.GetAttributeIndices(0, pAttribName, ref pwLangIndex, pwIndices, ref pwCount);
			ushort[] array = new ushort[pwCount];
			HeaderInfo3.GetAttributeIndices(0, pAttribName, ref pwLangIndex, array, ref pwCount);
			if (array != null && array.Length != 0)
			{
				ushort[] array2 = array;
				foreach (ushort num in array2)
				{
					string pwszName = null;
					object obj = null;
					ushort pwNameLen = 0;
					uint pdwDataLength = 0u;
					try
					{
						HeaderInfo3.GetAttributeByIndexEx(0, num, pwszName, ref pwNameLen, out var pType, out pwLangIndex, IntPtr.Zero, ref pdwDataLength);
						pwszName = new string('\0', pwNameLen);
						switch (pType)
						{
						case WMT_ATTR_DATATYPE.WMT_TYPE_DWORD:
						case WMT_ATTR_DATATYPE.WMT_TYPE_BOOL:
							obj = 0u;
							break;
						case WMT_ATTR_DATATYPE.WMT_TYPE_GUID:
							obj = Guid.NewGuid();
							break;
						case WMT_ATTR_DATATYPE.WMT_TYPE_QWORD:
							obj = 0uL;
							break;
						case WMT_ATTR_DATATYPE.WMT_TYPE_WORD:
							obj = (ushort)0;
							break;
						case WMT_ATTR_DATATYPE.WMT_TYPE_STRING:
						case WMT_ATTR_DATATYPE.WMT_TYPE_BINARY:
							obj = new byte[pdwDataLength];
							break;
						default:
							throw new InvalidOperationException($"Not supported data type: {pType.ToString()}");
						}
						GCHandle gCHandle = GCHandle.Alloc(obj, GCHandleType.Pinned);
						try
						{
							IntPtr intPtr = gCHandle.AddrOfPinnedObject();
							HeaderInfo3.GetAttributeByIndexEx(0, num, pwszName, ref pwNameLen, out pType, out pwLangIndex, intPtr, ref pdwDataLength);
							switch (pType)
							{
							case WMT_ATTR_DATATYPE.WMT_TYPE_STRING:
								obj = Marshal.PtrToStringUni(intPtr);
								break;
							case WMT_ATTR_DATATYPE.WMT_TYPE_BOOL:
								obj = (uint)obj != 0;
								break;
							}
							list.Add(new Tag(num, pwszName, pType, obj));
						}
						finally
						{
							gCHandle.Free();
						}
					}
					catch
					{
					}
				}
			}
		}
		catch
		{
		}
		return list;
	}

	private ushort[] WMGetAttribIndices(string pAttribName)
	{
		ushort[] array = null;
		try
		{
			if (!pAttribName.EndsWith("\0"))
			{
				pAttribName += "\0";
			}
			ushort pwLangIndex = 0;
			ushort[] pwIndices = null;
			ushort pwCount = 0;
			HeaderInfo3.GetAttributeIndices(0, pAttribName, ref pwLangIndex, pwIndices, ref pwCount);
			array = new ushort[pwCount];
			HeaderInfo3.GetAttributeIndices(0, pAttribName, ref pwLangIndex, array, ref pwCount);
		}
		catch
		{
			array = null;
		}
		return array;
	}
}

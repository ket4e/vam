using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine.Internal;

namespace UnityEngine;

/// <summary>
///   <para>Helper class to generate form data to post to web servers using the UnityWebRequest or WWW classes.</para>
/// </summary>
public class WWWForm
{
	private List<byte[]> formData;

	private List<string> fieldNames;

	private List<string> fileNames;

	private List<string> types;

	private byte[] boundary;

	private bool containsFiles = false;

	internal static Encoding DefaultEncoding => Encoding.ASCII;

	/// <summary>
	///   <para>(Read Only) Returns the correct request headers for posting the form using the WWW class.</para>
	/// </summary>
	public Dictionary<string, string> headers
	{
		get
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (containsFiles)
			{
				dictionary["Content-Type"] = "multipart/form-data; boundary=\"" + Encoding.UTF8.GetString(boundary, 0, boundary.Length) + "\"";
			}
			else
			{
				dictionary["Content-Type"] = "application/x-www-form-urlencoded";
			}
			return dictionary;
		}
	}

	/// <summary>
	///   <para>(Read Only) The raw data to pass as the POST request body when sending the form.</para>
	/// </summary>
	public byte[] data
	{
		get
		{
			if (containsFiles)
			{
				byte[] bytes = DefaultEncoding.GetBytes("--");
				byte[] bytes2 = DefaultEncoding.GetBytes("\r\n");
				byte[] bytes3 = DefaultEncoding.GetBytes("Content-Type: ");
				byte[] bytes4 = DefaultEncoding.GetBytes("Content-disposition: form-data; name=\"");
				byte[] bytes5 = DefaultEncoding.GetBytes("\"");
				byte[] bytes6 = DefaultEncoding.GetBytes("; filename=\"");
				using MemoryStream memoryStream = new MemoryStream(1024);
				for (int i = 0; i < formData.Count; i++)
				{
					memoryStream.Write(bytes2, 0, bytes2.Length);
					memoryStream.Write(bytes, 0, bytes.Length);
					memoryStream.Write(boundary, 0, boundary.Length);
					memoryStream.Write(bytes2, 0, bytes2.Length);
					memoryStream.Write(bytes3, 0, bytes3.Length);
					byte[] bytes7 = Encoding.UTF8.GetBytes(types[i]);
					memoryStream.Write(bytes7, 0, bytes7.Length);
					memoryStream.Write(bytes2, 0, bytes2.Length);
					memoryStream.Write(bytes4, 0, bytes4.Length);
					string headerName = Encoding.UTF8.HeaderName;
					string text = fieldNames[i];
					if (!WWWTranscoder.SevenBitClean(text, Encoding.UTF8) || text.IndexOf("=?") > -1)
					{
						text = "=?" + headerName + "?Q?" + WWWTranscoder.QPEncode(text, Encoding.UTF8) + "?=";
					}
					byte[] bytes8 = Encoding.UTF8.GetBytes(text);
					memoryStream.Write(bytes8, 0, bytes8.Length);
					memoryStream.Write(bytes5, 0, bytes5.Length);
					if (fileNames[i] != null)
					{
						string text2 = fileNames[i];
						if (!WWWTranscoder.SevenBitClean(text2, Encoding.UTF8) || text2.IndexOf("=?") > -1)
						{
							text2 = "=?" + headerName + "?Q?" + WWWTranscoder.QPEncode(text2, Encoding.UTF8) + "?=";
						}
						byte[] bytes9 = Encoding.UTF8.GetBytes(text2);
						memoryStream.Write(bytes6, 0, bytes6.Length);
						memoryStream.Write(bytes9, 0, bytes9.Length);
						memoryStream.Write(bytes5, 0, bytes5.Length);
					}
					memoryStream.Write(bytes2, 0, bytes2.Length);
					memoryStream.Write(bytes2, 0, bytes2.Length);
					byte[] array = formData[i];
					memoryStream.Write(array, 0, array.Length);
				}
				memoryStream.Write(bytes2, 0, bytes2.Length);
				memoryStream.Write(bytes, 0, bytes.Length);
				memoryStream.Write(boundary, 0, boundary.Length);
				memoryStream.Write(bytes, 0, bytes.Length);
				memoryStream.Write(bytes2, 0, bytes2.Length);
				return memoryStream.ToArray();
			}
			byte[] bytes10 = DefaultEncoding.GetBytes("&");
			byte[] bytes11 = DefaultEncoding.GetBytes("=");
			using MemoryStream memoryStream2 = new MemoryStream(1024);
			for (int j = 0; j < formData.Count; j++)
			{
				byte[] array2 = WWWTranscoder.DataEncode(Encoding.UTF8.GetBytes(fieldNames[j]));
				byte[] toEncode = formData[j];
				byte[] array3 = WWWTranscoder.DataEncode(toEncode);
				if (j > 0)
				{
					memoryStream2.Write(bytes10, 0, bytes10.Length);
				}
				memoryStream2.Write(array2, 0, array2.Length);
				memoryStream2.Write(bytes11, 0, bytes11.Length);
				memoryStream2.Write(array3, 0, array3.Length);
			}
			return memoryStream2.ToArray();
		}
	}

	/// <summary>
	///   <para>Creates an empty WWWForm object.</para>
	/// </summary>
	public WWWForm()
	{
		formData = new List<byte[]>();
		fieldNames = new List<string>();
		fileNames = new List<string>();
		types = new List<string>();
		boundary = new byte[40];
		for (int i = 0; i < 40; i++)
		{
			int num = Random.Range(48, 110);
			if (num > 57)
			{
				num += 7;
			}
			if (num > 90)
			{
				num += 6;
			}
			boundary[i] = (byte)num;
		}
	}

	/// <summary>
	///   <para>Add a simple field to the form.</para>
	/// </summary>
	/// <param name="fieldName"></param>
	/// <param name="value"></param>
	/// <param name="e"></param>
	public void AddField(string fieldName, string value)
	{
		AddField(fieldName, value, Encoding.UTF8);
	}

	/// <summary>
	///   <para>Add a simple field to the form.</para>
	/// </summary>
	/// <param name="fieldName"></param>
	/// <param name="value"></param>
	/// <param name="e"></param>
	public void AddField(string fieldName, string value, Encoding e)
	{
		fieldNames.Add(fieldName);
		fileNames.Add(null);
		formData.Add(e.GetBytes(value));
		types.Add("text/plain; charset=\"" + e.WebName + "\"");
	}

	/// <summary>
	///   <para>Adds a simple field to the form.</para>
	/// </summary>
	/// <param name="fieldName"></param>
	/// <param name="i"></param>
	public void AddField(string fieldName, int i)
	{
		AddField(fieldName, i.ToString());
	}

	/// <summary>
	///   <para>Add binary data to the form.</para>
	/// </summary>
	/// <param name="fieldName"></param>
	/// <param name="contents"></param>
	/// <param name="fileName"></param>
	/// <param name="mimeType"></param>
	[ExcludeFromDocs]
	public void AddBinaryData(string fieldName, byte[] contents)
	{
		AddBinaryData(fieldName, contents, null, null);
	}

	/// <summary>
	///   <para>Add binary data to the form.</para>
	/// </summary>
	/// <param name="fieldName"></param>
	/// <param name="contents"></param>
	/// <param name="fileName"></param>
	/// <param name="mimeType"></param>
	[ExcludeFromDocs]
	public void AddBinaryData(string fieldName, byte[] contents, string fileName)
	{
		AddBinaryData(fieldName, contents, fileName, null);
	}

	/// <summary>
	///   <para>Add binary data to the form.</para>
	/// </summary>
	/// <param name="fieldName"></param>
	/// <param name="contents"></param>
	/// <param name="fileName"></param>
	/// <param name="mimeType"></param>
	public void AddBinaryData(string fieldName, byte[] contents, [DefaultValue("null")] string fileName, [DefaultValue("null")] string mimeType)
	{
		containsFiles = true;
		bool flag = contents.Length > 8 && contents[0] == 137 && contents[1] == 80 && contents[2] == 78 && contents[3] == 71 && contents[4] == 13 && contents[5] == 10 && contents[6] == 26 && contents[7] == 10;
		if (fileName == null)
		{
			fileName = fieldName + ((!flag) ? ".dat" : ".png");
		}
		if (mimeType == null)
		{
			mimeType = ((!flag) ? "application/octet-stream" : "image/png");
		}
		fieldNames.Add(fieldName);
		fileNames.Add(fileName);
		formData.Add(contents);
		types.Add(mimeType);
	}
}

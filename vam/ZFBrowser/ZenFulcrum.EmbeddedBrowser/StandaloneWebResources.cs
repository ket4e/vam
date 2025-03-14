using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace ZenFulcrum.EmbeddedBrowser;

public class StandaloneWebResources : WebResources
{
	public struct IndexEntry
	{
		public string name;

		public long offset;

		public int length;
	}

	private const string FileHeader = "zfbRes_v1";

	protected Dictionary<string, IndexEntry> toc = new Dictionary<string, IndexEntry>();

	protected string dataFile;

	public const string DefaultPath = "Resources/browser_assets";

	public StandaloneWebResources(string dataFile)
	{
		this.dataFile = dataFile;
	}

	public void LoadIndex()
	{
		using BinaryReader binaryReader = new BinaryReader(File.OpenRead(dataFile));
		string text = binaryReader.ReadString();
		if (text != "zfbRes_v1")
		{
			throw new Exception("Invalid web resource file");
		}
		int num = binaryReader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			IndexEntry indexEntry = default(IndexEntry);
			indexEntry.name = binaryReader.ReadString();
			indexEntry.offset = binaryReader.ReadInt64();
			indexEntry.length = binaryReader.ReadInt32();
			IndexEntry value = indexEntry;
			toc[value.name] = value;
		}
	}

	public override void HandleRequest(int id, string url)
	{
		Uri uri = new Uri(url);
		string key = UnityWebRequest.UnEscapeURL(uri.AbsolutePath);
		if (!toc.TryGetValue(key, out var entry))
		{
			SendError(id, "Not found", 404);
			return;
		}
		new Thread((ThreadStart)delegate
		{
			try
			{
				string text = Path.GetExtension(entry.name);
				if (text.Length > 0)
				{
					text = text.Substring(1);
				}
				if (!WebResources.extensionMimeTypes.TryGetValue(text, out var value))
				{
					value = WebResources.extensionMimeTypes["*"];
				}
				using FileStream fileStream = File.OpenRead(dataFile);
				ResponsePreamble pre = new ResponsePreamble
				{
					headers = null,
					length = entry.length,
					mimeType = value,
					statusCode = 200
				};
				SendPreamble(id, pre);
				fileStream.Seek(entry.offset, SeekOrigin.Begin);
				byte[] array = new byte[entry.length];
				int num = fileStream.Read(array, 0, entry.length);
				if (num != array.Length)
				{
					throw new Exception("Insufficient data for file");
				}
				SendData(id, array);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}).Start();
	}

	public void WriteData(Dictionary<string, byte[]> files)
	{
		Dictionary<string, IndexEntry> dictionary = new Dictionary<string, IndexEntry>();
		using FileStream fileStream = File.OpenWrite(dataFile);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream, Encoding.UTF8);
		binaryWriter.Write("zfbRes_v1");
		binaryWriter.Write(files.Count);
		long position = fileStream.Position;
		foreach (KeyValuePair<string, byte[]> file in files)
		{
			binaryWriter.Write(file.Key);
			binaryWriter.Write(0L);
			binaryWriter.Write(0);
		}
		foreach (KeyValuePair<string, byte[]> file2 in files)
		{
			byte[] value = file2.Value;
			IndexEntry indexEntry = default(IndexEntry);
			indexEntry.name = file2.Key;
			indexEntry.length = file2.Value.Length;
			indexEntry.offset = fileStream.Position;
			IndexEntry value2 = indexEntry;
			binaryWriter.Write(value);
			dictionary[file2.Key] = value2;
		}
		binaryWriter.Seek((int)position, SeekOrigin.Begin);
		foreach (KeyValuePair<string, byte[]> file3 in files)
		{
			IndexEntry indexEntry2 = dictionary[file3.Key];
			binaryWriter.Write(file3.Key);
			binaryWriter.Write(indexEntry2.offset);
			binaryWriter.Write(indexEntry2.length);
		}
	}
}

using System;
using MHLab.PATCH.Compression;
using MHLab.PATCH.Debugging;
using MHLab.PATCH.Settings;

namespace MHLab.PATCH;

internal class Patch
{
	public Version From;

	public Version To;

	public string Hash;

	public CompressionType Type;

	public string PatchName => string.Concat(From, "_", To);

	public string ArchiveName => string.Concat(From, "_", To, ".archive");

	public string IndexerName => string.Concat(From, "_", To, ".pix");

	public Patch(Version from, Version to)
	{
		From = from;
		To = to;
	}

	public Patch(string entry)
	{
		try
		{
			string[] array = entry.Split(SettingsManager.PATCHES_SYMBOL_SEPARATOR);
			From = new Version(array[0]);
			To = new Version(array[1]);
			Hash = array[2];
			Type = (CompressionType)Enum.Parse(typeof(CompressionType), array[3].Replace("\n", "").Replace("\r", ""));
		}
		catch (Exception ex)
		{
			Debugger.Log(ex.Message);
		}
	}
}

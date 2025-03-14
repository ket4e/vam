using System.IO;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

namespace MHLab.PATCH.Compression.TAR;

internal class TARGZCompressor
{
	public static void ArchiveFolder(string outPathname, string folderName, bool compress)
	{
		Stream stream = File.Create(outPathname);
		TarOutputStream tarOutputStream;
		if (compress)
		{
			GZipOutputStream gZipOutputStream = new GZipOutputStream(stream);
			gZipOutputStream.SetLevel(9);
			tarOutputStream = new TarOutputStream(gZipOutputStream);
		}
		else
		{
			tarOutputStream = new TarOutputStream(stream);
		}
		CreateTarManually(tarOutputStream, folderName, folderName);
		tarOutputStream.Close();
	}

	private static void CreateTarManually(TarOutputStream tarOutputStream, string sourceDirectory, string rootDirectory)
	{
		string[] files = Directory.GetFiles(sourceDirectory);
		foreach (string text in files)
		{
			using (Stream stream = File.OpenRead(text))
			{
				string name = text.Substring(rootDirectory.Length + ((!rootDirectory.EndsWith(Path.DirectorySeparatorChar.ToString())) ? 1 : 0));
				long length = stream.Length;
				TarEntry tarEntry = TarEntry.CreateTarEntry(name);
				tarEntry.Size = length;
				tarOutputStream.PutNextEntry(tarEntry);
				byte[] array = new byte[32768];
				while (true)
				{
					int num = stream.Read(array, 0, array.Length);
					if (num <= 0)
					{
						break;
					}
					tarOutputStream.Write(array, 0, num);
				}
			}
			tarOutputStream.CloseEntry();
		}
		files = Directory.GetDirectories(sourceDirectory);
		foreach (string sourceDirectory2 in files)
		{
			CreateTarManually(tarOutputStream, sourceDirectory2, rootDirectory);
		}
	}

	public static void ExtractTAR(string tarFileName, string destFolder)
	{
		FileStream fileStream = File.OpenRead(tarFileName);
		TarArchive tarArchive = TarArchive.CreateInputTarArchive(fileStream);
		tarArchive.ExtractContents(destFolder);
		tarArchive.Close();
		fileStream.Close();
	}

	public static void ExtractTGZ(string gzArchiveName, string destFolder)
	{
		FileStream fileStream = File.OpenRead(gzArchiveName);
		GZipInputStream gZipInputStream = new GZipInputStream(fileStream);
		TarArchive tarArchive = TarArchive.CreateInputTarArchive(gZipInputStream);
		tarArchive.ExtractContents(destFolder);
		tarArchive.Close();
		gZipInputStream.Close();
		fileStream.Close();
	}
}

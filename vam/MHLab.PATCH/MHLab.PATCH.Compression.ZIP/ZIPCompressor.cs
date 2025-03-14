using System;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace MHLab.PATCH.Compression.ZIP;

internal class ZIPCompressor
{
	public static void ZipFolder(string outPathname, string password, string folderName)
	{
		ZipConstants.DefaultCodePage = 0;
		try
		{
			using FileStream baseOutputStream = File.Create(outPathname);
			using ZipOutputStream zipOutputStream = new ZipOutputStream(baseOutputStream);
			zipOutputStream.SetLevel(9);
			zipOutputStream.Password = password;
			int folderOffset = folderName.Length + ((!folderName.EndsWith(Path.DirectorySeparatorChar.ToString())) ? 1 : 0);
			CompressFolder(folderName, zipOutputStream, folderOffset);
			zipOutputStream.IsStreamOwner = true;
			zipOutputStream.Close();
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	private static void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset)
	{
		string[] files = Directory.GetFiles(path);
		foreach (string obj in files)
		{
			FileInfo fileInfo = new FileInfo(obj);
			zipStream.PutNextEntry(new ZipEntry(ZipEntry.CleanName(obj.Substring(folderOffset)))
			{
				DateTime = fileInfo.LastWriteTime,
				Size = fileInfo.Length
			});
			byte[] buffer = new byte[4096];
			using (FileStream source = File.OpenRead(obj))
			{
				StreamUtils.Copy(source, zipStream, buffer);
			}
			zipStream.CloseEntry();
		}
		files = Directory.GetDirectories(path);
		for (int i = 0; i < files.Length; i++)
		{
			CompressFolder(files[i], zipStream, folderOffset);
		}
	}

	public static void ExtractZipFile(string archiveFilenameIn, string password, string outFolder)
	{
		ZipConstants.DefaultCodePage = 0;
		ZipFile zipFile = null;
		try
		{
			zipFile = new ZipFile(File.OpenRead(archiveFilenameIn));
			if (!string.IsNullOrEmpty(password))
			{
				zipFile.Password = password;
			}
			foreach (ZipEntry item in zipFile)
			{
				if (item.IsFile)
				{
					string name = item.Name;
					byte[] buffer = new byte[4096];
					Stream inputStream = zipFile.GetInputStream(item);
					string path = Path.Combine(outFolder, name);
					string directoryName = Path.GetDirectoryName(path);
					if (directoryName.Length > 0)
					{
						Directory.CreateDirectory(directoryName);
					}
					using FileStream destination = File.Create(path);
					StreamUtils.Copy(inputStream, destination, buffer);
				}
			}
		}
		catch (Exception ex)
		{
			throw ex;
		}
		finally
		{
			if (zipFile != null)
			{
				zipFile.IsStreamOwner = true;
				zipFile.Close();
			}
		}
	}
}

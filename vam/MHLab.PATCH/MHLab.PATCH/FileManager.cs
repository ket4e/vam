using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using MHLab.PATCH.Debugging;
using MHLab.PATCH.Settings;

namespace MHLab.PATCH;

public class FileManager
{
	public static IEnumerable<string> GetFiles()
	{
		return Directory.GetFiles(SettingsManager.APP_PATH, "*", SearchOption.AllDirectories);
	}

	public static IEnumerable<string> GetFiles(string path)
	{
		return Directory.GetFiles(path, "*", SearchOption.AllDirectories);
	}

	public static IEnumerable<string> GetFiles(string path, string pattern)
	{
		return Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
	}

	public static IEnumerable<string> GetFiles(string path, string pattern, SearchOption option)
	{
		return Directory.GetFiles(path, pattern, option);
	}

	public static IEnumerable<string> GetCurrentBuildFiles()
	{
		return GetFiles(SettingsManager.CURRENT_BUILD_PATH);
	}

	public static IEnumerable<string> GetAllBuildsDirectories()
	{
		DirectoryInfo[] directories = new DirectoryInfo(SettingsManager.BUILDS_PATH).GetDirectories("*", SearchOption.TopDirectoryOnly);
		List<string> list = new List<string>();
		DirectoryInfo[] array = directories;
		foreach (DirectoryInfo directoryInfo in array)
		{
			list.Add(directoryInfo.FullName);
		}
		return list;
	}

	public static void CleanDirectory(string directory)
	{
		try
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			DirectoryInfo directoryInfo = new DirectoryInfo(directory);
			FileInfo[] files = directoryInfo.GetFiles();
			foreach (FileInfo obj in files)
			{
				obj.Attributes &= ~FileAttributes.ReadOnly;
				obj.Delete();
			}
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			for (int i = 0; i < directories.Length; i++)
			{
				DeleteRecursiveFolder(directories[i].FullName);
			}
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	private static void DeleteRecursiveFolder(string pFolderPath)
	{
		string[] directories = Directory.GetDirectories(pFolderPath);
		for (int i = 0; i < directories.Length; i++)
		{
			DeleteRecursiveFolder(directories[i]);
		}
		directories = Directory.GetFiles(pFolderPath);
		foreach (string path in directories)
		{
			FileInfo fileInfo = new FileInfo(Path.Combine(pFolderPath, path));
			File.SetAttributes(fileInfo.FullName, FileAttributes.Normal);
			File.Delete(fileInfo.FullName);
		}
		Directory.Delete(pFolderPath);
	}

	public static void DeleteFiles(string directory, string pattern)
	{
		foreach (string file in GetFiles(directory, pattern))
		{
			DeleteFile(file);
		}
	}

	public static bool DeleteFile(string file)
	{
		try
		{
			if (!_deleteFile(file))
			{
				GC.Collect();
				GC.WaitForPendingFinalizers();
				return _deleteFile(file);
			}
			return true;
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	private static bool _deleteFile(string file)
	{
		try
		{
			File.Delete(file);
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static bool DeleteDirectory(string directory)
	{
		try
		{
			CleanDirectory(directory);
			Directory.Delete(directory);
			return true;
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static bool IsDirectoryEmpty(string path)
	{
		if (!Directory.Exists(path))
		{
			throw new DirectoryNotFoundException();
		}
		string[] directories = Directory.GetDirectories(path);
		string[] files = Directory.GetFiles(path);
		if (directories.Length == 0 && files.Length == 0)
		{
			return true;
		}
		return false;
	}

	public static bool DirectoryExists(string path)
	{
		return Directory.Exists(path);
	}

	public static bool FileExists(string path)
	{
		return File.Exists(path);
	}

	public static bool CreateDirectory(string path)
	{
		try
		{
			if (!DirectoryExists(path))
			{
				Directory.CreateDirectory(path);
			}
			return true;
		}
		catch (Exception ex)
		{
			Debugger.Log(ex.Message);
			return false;
		}
	}

	public static void CopyDirectory(string sourceFolder, string destFolder)
	{
		if (!DirectoryExists(destFolder))
		{
			CreateDirectory(destFolder);
		}
		foreach (string file in GetFiles(sourceFolder))
		{
			string text = file.Replace(sourceFolder, destFolder);
			if (!DirectoryExists(Path.GetDirectoryName(text)))
			{
				CreateDirectory(Path.GetDirectoryName(text));
			}
			File.Copy(file, text);
		}
	}

	public static string PathCombine(string dir, string fileName)
	{
		return Path.Combine(dir, Path.GetFileName(fileName));
	}

	public static void CreateFile(string filePath)
	{
		if (!File.Exists(filePath))
		{
			using (File.Create(filePath))
			{
			}
		}
	}

	public static bool IsFileLocked(string file)
	{
		FileStream fileStream = null;
		try
		{
			fileStream = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete);
		}
		catch (IOException)
		{
			return true;
		}
		finally
		{
			if (fileStream != null)
			{
				fileStream.Dispose();
				fileStream.Close();
			}
		}
		return false;
	}

	public static void FileCopy(string source, string dest, bool overwrite = true)
	{
		try
		{
			File.Copy(source, dest, overwrite);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void FileMove(string source, string dest)
	{
		try
		{
			File.Move(source, dest);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void FileRename(string file, string newName)
	{
		File.Move(file, Path.Combine(Path.GetDirectoryName(file), newName));
	}

	private static bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
	{
		bool result = true;
		if (sslPolicyErrors != 0)
		{
			for (int i = 0; i < chain.ChainStatus.Length; i++)
			{
				if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
				{
					chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
					chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
					chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
					chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
					if (!chain.Build((X509Certificate2)certificate))
					{
						result = false;
					}
				}
			}
		}
		return result;
	}

	public static void DownloadFile(string url, string localPath)
	{
		ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;
		using WebClient webClient = new WebClient();
		if (SettingsManager.ENABLE_FTP)
		{
			webClient.Credentials = new NetworkCredential(SettingsManager.FTP_USERNAME, SettingsManager.FTP_PASSWORD);
		}
		webClient.DownloadFile(url, localPath);
	}

	public static string DownloadFileToString(string url)
	{
		try
		{
			ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;
			using WebClient webClient = new WebClient();
			if (SettingsManager.ENABLE_FTP)
			{
				webClient.Credentials = new NetworkCredential(SettingsManager.FTP_USERNAME, SettingsManager.FTP_PASSWORD);
			}
			return Encoding.UTF8.GetString(webClient.DownloadData(url));
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static void CreateShortcut(string targetFile, string shortcutFile, bool asAdmin = true)
	{
		Type typeFromCLSID = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8"));
		object obj = Activator.CreateInstance(typeFromCLSID);
		try
		{
			object obj2 = typeFromCLSID.InvokeMember("CreateShortcut", BindingFlags.InvokeMethod, null, obj, new object[1] { shortcutFile });
			try
			{
				typeFromCLSID.InvokeMember("TargetPath", BindingFlags.SetProperty, null, obj2, new object[1] { targetFile });
				typeFromCLSID.InvokeMember("Arguments", BindingFlags.SetProperty, null, obj2, new object[1] { "-popupwindow" });
				typeFromCLSID.InvokeMember("IconLocation", BindingFlags.SetProperty, null, obj2, new object[1] { "shell32.dll, 2" });
				typeFromCLSID.InvokeMember("Save", BindingFlags.InvokeMethod, null, obj2, null);
				if (asAdmin)
				{
					using (FileStream fileStream = new FileStream(shortcutFile, FileMode.Open, FileAccess.ReadWrite))
					{
						fileStream.Seek(21L, SeekOrigin.Begin);
						fileStream.WriteByte(34);
						return;
					}
				}
			}
			finally
			{
				Marshal.FinalReleaseComObject(obj2);
			}
		}
		finally
		{
			Marshal.FinalReleaseComObject(obj);
		}
	}
}

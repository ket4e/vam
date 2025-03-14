using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MHLab.PATCH.Compression;
using MHLab.PATCH.Downloader;
using MHLab.PATCH.Settings;
using MHLab.PATCH.Utilities;
using Octodiff.Core;

namespace MHLab.PATCH;

internal class PatchApplier
{
	private List<Patch> _patches;

	private Dictionary<string, string> _downloadedArchiveFilesHashes;

	public Action<string> OnFileProcessed;

	public Action<string> OnFileProcessing;

	public Action<string, string> OnLog;

	public Action<string, string, Exception> OnError;

	public Action<string, string, Exception> OnFatalError;

	public Action<string> OnTaskStarted;

	public Action<string> OnTaskCompleted;

	public Action<int, int> OnSetMainProgressBar;

	public Action<int, int> OnSetDetailProgressBar;

	public Action OnIncreaseMainProgressBar;

	public Action OnIncreaseDetailProgressBar;

	public Action<long, long, int> OnDownloadProgress;

	public Action OnDownloadCompleted;

	public bool IsDirtyWorkspace;

	public PatchApplier()
	{
		_patches = new List<Patch>();
		OnFileProcessed = delegate
		{
		};
		OnFileProcessing = delegate
		{
		};
		OnLog = delegate
		{
		};
		OnError = delegate
		{
		};
		OnFatalError = delegate
		{
		};
		OnTaskStarted = delegate
		{
		};
		OnTaskCompleted = delegate
		{
		};
		OnSetMainProgressBar = delegate
		{
		};
		OnSetDetailProgressBar = delegate
		{
		};
		OnIncreaseMainProgressBar = delegate
		{
		};
		OnIncreaseDetailProgressBar = delegate
		{
		};
		OnDownloadProgress = delegate
		{
		};
		OnDownloadCompleted = delegate
		{
		};
	}

	public bool CheckForUpdates()
	{
		OnTaskStarted("Patching process is started!");
		OnSetMainProgressBar(0, 4);
		ClearPatchWorkspace();
		string currentVersion2 = GetCurrentVersion();
		OnLog("Checking version...", "Found version " + currentVersion2);
		OnIncreaseMainProgressBar();
		if (currentVersion2 != null)
		{
			Version currentVersion = new Version(currentVersion2);
			if (UpdateVersions())
			{
				OnIncreaseMainProgressBar();
				List<Patch> list = _patches.Where((Patch p) => p.From.Equals(currentVersion)).ToList();
				list.Sort((Patch p1, Patch p2) => p2.To.CompareTo(p1.To));
				while (list.Count > 0)
				{
					ClearPatchWorkspace();
					if (!PerformUpdate(list[0]))
					{
						return false;
					}
					currentVersion = new Version(GetCurrentVersion());
					list = _patches.Where((Patch p) => p.From.Equals(currentVersion)).ToList();
					list.Sort((Patch p1, Patch p2) => p2.To.CompareTo(p1.To));
				}
				OnIncreaseMainProgressBar();
				ClearPatchWorkspace();
				OnIncreaseMainProgressBar();
				OnTaskCompleted("Patching process is now completed! Press LAUNCH button!");
			}
			else
			{
				OnIncreaseMainProgressBar();
				OnIncreaseMainProgressBar();
				ClearPatchWorkspace();
				OnIncreaseMainProgressBar();
				OnTaskCompleted("No remote patches available!");
			}
			return true;
		}
		OnError("Patch error!", "Local version checking error! Please check your version file!", new Exception("Local version checking error! Please check your version file!"));
		return false;
	}

	public string GetCurrentVersion()
	{
		try
		{
			if (!FileManager.FileExists(SettingsManager.PATCH_VERSION_PATH))
			{
				return null;
			}
			return Rijndael.Decrypt(File.ReadAllText(SettingsManager.PATCH_VERSION_PATH).Replace("\n", "").Replace("\r", ""), SettingsManager.PATCH_VERSION_ENCRYPTION_PASSWORD);
		}
		catch (Exception arg)
		{
			OnFatalError("Patch error!", "Local version file missing or corrupted! Cannot patch", arg);
			return null;
		}
	}

	private bool ClearPatchWorkspace()
	{
		OnLog("Cleaning!", "Cleaning MVR Patcher workspace!");
		OnSetDetailProgressBar(0, 2);
		try
		{
			if (FileManager.DirectoryExists(SettingsManager.PATCHES_TMP_FOLDER))
			{
				FileManager.DeleteDirectory(SettingsManager.PATCHES_TMP_FOLDER);
			}
			OnIncreaseDetailProgressBar();
			if (FileManager.DirectoryExists(SettingsManager.PATCH_SAFE_BACKUP))
			{
				FileManager.DeleteDirectory(SettingsManager.PATCH_SAFE_BACKUP);
			}
			OnIncreaseDetailProgressBar();
			CleanSelfDeletingFiles();
		}
		catch (Exception arg)
		{
			OnError("Error!", "Exception during workspace clean", arg);
			return false;
		}
		return true;
	}

	private void CleanSelfDeletingFiles()
	{
		FileManager.DeleteFiles(SettingsManager.APP_PATH, "*.delete_me");
	}

	private bool UpdateVersions()
	{
		OnLog("Checking...", "Checking for remote service!");
		if (!Utility.IsRemoteServiceAvailable(SettingsManager.VERSIONS_FILE_DOWNLOAD_URL))
		{
			OnFatalError("Error!", "No remote service is responding! Try again!", new Exception("No remote service is responding! Try again!"));
			return false;
		}
		OnSetDetailProgressBar(0, 2);
		string text = null;
		try
		{
			text = FileManager.DownloadFileToString(SettingsManager.VERSIONS_FILE_DOWNLOAD_URL);
		}
		catch
		{
			OnFatalError("Error!", "Can't find remote versions list!", new Exception("Can't find remote versions list!"));
		}
		OnIncreaseDetailProgressBar();
		if (!string.IsNullOrEmpty(text))
		{
			string[] array = text.Split('\n');
			foreach (string text2 in array)
			{
				if (text2 != "")
				{
					_patches.Add(new Patch(text2));
				}
			}
			OnIncreaseDetailProgressBar();
			return true;
		}
		OnIncreaseDetailProgressBar();
		return false;
	}

	private bool PerformUpdate(Patch p)
	{
		bool flag = DownloadPatchArchiveFile(p);
		bool flag2 = false;
		if (!flag)
		{
			for (int i = 1; i <= SettingsManager.PATCH_DOWNLOAD_RETRY_ATTEMPTS; i++)
			{
				flag = DownloadPatchArchiveFile(p);
				if (flag)
				{
					OnLog("Hash checking!", "Comparing hashes for " + p.ArchiveName);
					if (Hashing.SHA1(SettingsManager.PATCHES_TMP_FOLDER + p.ArchiveName) == p.Hash)
					{
						flag2 = true;
						break;
					}
					flag = false;
					flag2 = false;
					OnLog("Issue detected!", "An archive is corrupted, retrying..." + i);
				}
				else
				{
					flag = false;
					flag2 = false;
					OnLog("Issue detected!", "Can't download an archive, retrying..." + i);
				}
			}
		}
		else
		{
			OnLog("Hash checking!", "Comparing hashes for " + p.ArchiveName);
			flag2 = Hashing.SHA1(SettingsManager.PATCHES_TMP_FOLDER + p.ArchiveName) == p.Hash;
		}
		bool result = true;
		if (flag)
		{
			if (flag2)
			{
				OnSetDetailProgressBar(0, 3);
				OnLog("Patch decompression!", "Unzipping pack " + p.ArchiveName);
				Directory.CreateDirectory(SettingsManager.PATCHES_TMP_FOLDER + p.PatchName);
				OnIncreaseDetailProgressBar();
				Compressor.Decompress(SettingsManager.PATCHES_TMP_FOLDER + p.PatchName, SettingsManager.PATCHES_TMP_FOLDER + p.ArchiveName, p.Type, null);
				OnIncreaseDetailProgressBar();
				IEnumerable<string> files = FileManager.GetFiles(SettingsManager.PATCHES_TMP_FOLDER + p.PatchName);
				OnIncreaseDetailProgressBar();
				OnLog("Processing index!", "Computing indexer for " + p.ArchiveName);
				ProcessPatchIndexer(p);
				result = ProcessFiles(files, p);
				ClearPatchWorkspace();
			}
			else
			{
				OnFatalError("Patch failed!", "Patch archive hash checking failed! Try to reopen the launcher to try again.", new Exception("Patch archive hash checking failed! Try to reopen the launcher to try again."));
			}
		}
		else
		{
			OnFatalError("Patch failed!", "Patch download failed! Try to reopen the launcher to try again.", new Exception("Patch download failed! Try to reopen the launcher to try again."));
		}
		return result;
	}

	private bool ProcessFiles(IEnumerable<string> files, Patch p)
	{
		bool result = true;
		OnSetDetailProgressBar(0, files.Count());
		foreach (string file in files)
		{
			OnFileProcessing(file.Replace(SettingsManager.PATCHES_TMP_FOLDER + p.PatchName, ""));
			string oldFile = file.Replace(SettingsManager.PATCHES_TMP_FOLDER + p.PatchName, SettingsManager.APP_PATH);
			if (ProcessFile(oldFile, file))
			{
				OnIncreaseDetailProgressBar();
				OnFileProcessed(file.Replace(SettingsManager.PATCHES_TMP_FOLDER + p.PatchName, ""));
			}
			else
			{
				OnIncreaseDetailProgressBar();
				result = false;
			}
		}
		return result;
	}

	private bool ProcessFile(string oldFile, string newFile)
	{
		try
		{
			if (!FileManager.DirectoryExists(oldFile.Replace(SettingsManager.APP_PATH, SettingsManager.PATCH_SAFE_BACKUP)))
			{
				FileManager.CreateDirectory(Path.GetDirectoryName(oldFile.Replace(SettingsManager.APP_PATH, SettingsManager.PATCH_SAFE_BACKUP)));
			}
			string extension = Path.GetExtension(newFile);
			if (!(extension == ".delete"))
			{
				if (extension == ".patch")
				{
					OnLog("File processing - Patching", "Patching " + oldFile.Replace(".patch", "").Replace(SettingsManager.APP_PATH, "") + " file");
					if (FileManager.IsFileLocked(oldFile.Replace(".patch", "")))
					{
						string text = oldFile.Replace(".patch", "") + ".delete_me";
						FileManager.FileRename(oldFile.Replace(".patch", ""), text);
						FileManager.FileCopy(FileManager.PathCombine(Path.GetDirectoryName(oldFile.Replace(".patch", "")), text), oldFile.Replace(".patch", ""));
						IsDirtyWorkspace = true;
					}
					FileManager.FileCopy(oldFile.Replace(".patch", ""), oldFile.Replace(".patch", "").Replace(SettingsManager.APP_PATH, SettingsManager.PATCH_SAFE_BACKUP));
					ApplyOctodiffPatch(oldFile.Replace(".patch", ""), newFile);
					string text2 = Hashing.SHA1(oldFile.Replace(".patch", ""));
					string key = oldFile.Replace(".patch", "").Replace(SettingsManager.APP_PATH + Path.DirectorySeparatorChar, "").Replace('\\', '/');
					string text3 = _downloadedArchiveFilesHashes[key];
					OnLog("File processing", "Computed hash: " + text2 + " - Expected hash: " + text3);
					if (!(text3 == text2))
					{
						OnError("Patch failed!", "Existing file (" + oldFile.Replace(".patch", "").Replace(SettingsManager.APP_PATH, "") + ") is corrupted and cannot be patched. Will require repair.", new Exception("A file (" + oldFile.Replace(".patch", "").Replace(SettingsManager.APP_PATH, "") + ") is corrupted and cannot be patched. Will require repair."));
						return false;
					}
					OnLog("File processing - Hash checking", "Hash checking " + oldFile.Replace(".patch", "").Replace(SettingsManager.APP_PATH, "") + " file");
					OnLog("File processed - Patched", "Patched " + oldFile.Replace(".patch", "").Replace(SettingsManager.APP_PATH, "") + " file");
				}
				else
				{
					OnLog("File processing - Creating", "Creating new " + oldFile.Replace(".patch", "").Replace(SettingsManager.APP_PATH, "") + " file");
					FileManager.CreateFile(oldFile.Replace(SettingsManager.APP_PATH, SettingsManager.PATCH_SAFE_BACKUP));
					FileManager.CreateDirectory(Path.GetDirectoryName(oldFile));
					FileManager.FileCopy(newFile, oldFile);
					OnLog("File processed - Created", "Created new " + oldFile.Replace(".patch", "").Replace(SettingsManager.APP_PATH, "") + " file");
				}
			}
			else
			{
				OnLog("File processing - Deleting", "Deleting " + oldFile.Replace(".patch", "").Replace(SettingsManager.APP_PATH, "") + " file");
				FileManager.FileMove(oldFile.Replace(".delete", ""), oldFile.Replace(".delete", "").Replace(SettingsManager.APP_PATH, SettingsManager.PATCH_SAFE_BACKUP));
				OnLog("File processed - Deleted", "Deleted " + oldFile.Replace(".patch", "").Replace(SettingsManager.APP_PATH, "") + " file");
			}
		}
		catch (Exception arg)
		{
			OnError("Patch failed!", "Patch of file " + oldFile.Replace(".patch", "").Replace(SettingsManager.APP_PATH, "") + " failed", arg);
			return false;
		}
		return true;
	}

	private void ProcessPatchIndexer(Patch p)
	{
		OnSetDetailProgressBar(0, 1);
		_downloadedArchiveFilesHashes = new Dictionary<string, string>();
		string[] array = Rijndael.Decrypt(File.ReadAllText(SettingsManager.PATCHES_TMP_FOLDER + p.IndexerName), SettingsManager.PATCH_VERSION_ENCRYPTION_PASSWORD).Split('\n');
		foreach (string text in array)
		{
			if (text != "")
			{
				string[] array2 = text.Split(SettingsManager.PATCHES_SYMBOL_SEPARATOR);
				_downloadedArchiveFilesHashes.Add(array2[0], array2[1].Replace("\r", ""));
			}
		}
		OnIncreaseDetailProgressBar();
	}

	private void ApplyOctodiffPatch(string oldFile, string patchFile)
	{
		try
		{
			DeltaApplier deltaApplier = new DeltaApplier
			{
				SkipHashCheck = true
			};
			if (FileManager.FileExists(oldFile + ".bak"))
			{
				FileManager.DeleteFile(oldFile + ".bak");
			}
			File.Move(oldFile, oldFile + ".bak");
			using (FileStream basisFileStream = new FileStream(oldFile + ".bak", FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using FileStream stream = new FileStream(patchFile, FileMode.Open, FileAccess.Read, FileShare.Read);
				using FileStream outputStream = new FileStream(oldFile, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
				deltaApplier.Apply(basisFileStream, new BinaryDeltaReader(stream, null), outputStream);
			}
			FileManager.DeleteFile(oldFile + ".bak");
		}
		catch (Exception arg)
		{
			OnError("Exception", "ApplyOctodiffPath", arg);
		}
	}

	private void RollbackPatch()
	{
		OnLog("Rollback!", "Rollingback to previous version!");
		if (!FileManager.DirectoryExists(SettingsManager.PATCH_SAFE_BACKUP))
		{
			return;
		}
		IEnumerable<string> files = FileManager.GetFiles(SettingsManager.PATCH_SAFE_BACKUP);
		OnSetDetailProgressBar(0, files.Count());
		foreach (string item in files)
		{
			string extension = Path.GetExtension(item);
			if (extension == ".delete")
			{
				FileManager.DeleteFile(item.Replace(".delete", "").Replace(SettingsManager.PATCH_SAFE_BACKUP, SettingsManager.APP_PATH));
			}
			else
			{
				FileManager.DeleteFile(item.Replace(SettingsManager.PATCH_SAFE_BACKUP, SettingsManager.APP_PATH));
				FileManager.FileMove(item, item.Replace(SettingsManager.PATCH_SAFE_BACKUP, SettingsManager.APP_PATH));
			}
			OnIncreaseDetailProgressBar();
		}
	}

	private bool DownloadPatchArchiveFile(Patch p)
	{
		try
		{
			FileDownloader fileDownloader = new FileDownloader();
			fileDownloader.ProgressChanged += downloader_ProgressChanged;
			fileDownloader.DownloadComplete += downloader_Completed;
			OnLog("Checking directories...", "Checking directories and workspace creation!");
			Directory.CreateDirectory(SettingsManager.PATCHES_TMP_FOLDER);
			if (FileManager.FileExists(SettingsManager.PATCHES_TMP_FOLDER + p.ArchiveName))
			{
				FileManager.DeleteFile(SettingsManager.PATCHES_TMP_FOLDER + p.ArchiveName);
			}
			if (FileManager.FileExists(SettingsManager.PATCHES_TMP_FOLDER + p.IndexerName))
			{
				FileManager.DeleteFile(SettingsManager.PATCHES_TMP_FOLDER + p.IndexerName);
			}
			OnLog("Downloading file...", "Downloading patch " + p.PatchName + " indexer from remote server!");
			OnSetDetailProgressBar(0, 100);
			fileDownloader.Download(SettingsManager.PATCHES_DOWNLOAD_URL + p.IndexerName, SettingsManager.PATCHES_TMP_FOLDER);
			OnLog("Downloaded file!", "Downloaded patch " + p.PatchName + " indexer from remote server!");
			OnLog("Downloading file...", "Downloading patch " + p.PatchName + " archive from remote server!");
			OnSetDetailProgressBar(0, 100);
			fileDownloader.Download(SettingsManager.PATCHES_DOWNLOAD_URL + p.ArchiveName, SettingsManager.PATCHES_TMP_FOLDER);
			OnLog("Downloaded file!", "Downloaded patch " + p.PatchName + " archive from remote server!");
			return true;
		}
		catch (Exception arg)
		{
			OnError("Exception", "DownloadPatchArchiveFile", arg);
			return false;
		}
	}

	private void downloader_ProgressChanged(object sender, DownloadEventArgs e)
	{
		OnDownloadProgress(e.CurrentFileSize, e.TotalFileSize, e.PercentDone);
	}

	private void downloader_Completed(object sender, EventArgs e)
	{
		OnDownloadCompleted();
	}
}

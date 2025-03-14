using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using MHLab.PATCH.Compression;
using MHLab.PATCH.Downloader;
using MHLab.PATCH.Settings;
using MHLab.PATCH.Utilities;

namespace MHLab.PATCH.Install;

internal class Installer
{
	private class BuildEntry
	{
		public string build;

		public string file;

		public string hash;
	}

	internal List<Version> availableBuilds;

	public Action<string> OnFileProcessed;

	public Action<string> OnFileProcessing;

	public Action<string, string> OnLog;

	public Action<string, string, Exception> OnError;

	public Action<string, string, Exception> OnFatalError;

	public Action<string> OnTaskStarted;

	public Action<string> OnTaskFailed;

	public Action<string> OnTaskCompleted;

	public Action<int, int> OnSetMainProgressBar;

	public Action<int, int> OnSetDetailProgressBar;

	public Action OnIncreaseMainProgressBar;

	public Action OnIncreaseDetailProgressBar;

	public Action<long, long, int> OnDownloadProgress;

	public Action OnDownloadCompleted;

	public Action OnDownloadCancelled;

	protected bool _wasInit;

	public bool useDevIndex;

	protected bool abortSync;

	private FileDownloader downloader;

	public Installer()
	{
		availableBuilds = new List<Version>();
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
		OnTaskFailed = delegate
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
		OnDownloadCancelled = delegate
		{
		};
	}

	public bool Init()
	{
		try
		{
			if (!_wasInit)
			{
				OnLog("Checking for remote service", "");
				if (!Utility.IsRemoteServiceAvailable(SettingsManager.BUILDS_DOWNLOAD_URL + "index"))
				{
					OnFatalError("Remote service down. Please try again later.", "", null);
					return false;
				}
				OnLog("Downloading versions index file...", "");
				string content = ((!useDevIndex) ? FileManager.DownloadFileToString(SettingsManager.BUILDS_DOWNLOAD_URL + "index") : FileManager.DownloadFileToString(SettingsManager.BUILDS_DOWNLOAD_URL + "index.dev"));
				if (!ProcessBuildsIndex(content))
				{
					OnFatalError("Error while processing versions file", "", null);
					return false;
				}
				_wasInit = true;
			}
		}
		catch (Exception arg)
		{
			OnFatalError("Error during Init", "", arg);
			return false;
		}
		OnLog("Init complete", "");
		return true;
	}

	public InstallationState InstallPatcher(string pathToInstall)
	{
		try
		{
			OnLog("Checking...", "Checking for remote service!");
			if (!Utility.IsRemoteServiceAvailable(SettingsManager.PATCHER_DOWNLOAD_URL + "patcher.zip"))
			{
				OnFatalError("Error!", "No remote service is responding! Try again!", null);
				return InstallationState.FAILED;
			}
			DownloadFile(Path.Combine(SettingsManager.PATCHER_DOWNLOAD_URL, "patcher.zip"), Path.Combine(pathToInstall, "patcher.zip"));
			Compressor.Decompress(pathToInstall, Path.Combine(pathToInstall, "patcher.zip"), CompressionType.ZIP, null);
			File.Delete(Path.Combine(pathToInstall, "patcher.zip"));
			return InstallationState.SUCCESS;
		}
		catch (Exception arg)
		{
			OnFatalError("Error", "Exception", arg);
			return InstallationState.FAILED;
		}
	}

	public InstallationState InstallShortcut(string pathToInstall, bool toPatcher = true)
	{
		try
		{
			File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), SettingsManager.LAUNCHER_NAME + " - Shortcut.lnk"));
			if (toPatcher)
			{
				FileManager.CreateShortcut(Path.Combine(pathToInstall, SettingsManager.LAUNCHER_NAME), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), SettingsManager.LAUNCHER_NAME + " - Shortcut.lnk"));
			}
			else
			{
				FileManager.CreateShortcut(Path.Combine(pathToInstall, SettingsManager.LAUNCH_APP), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), SettingsManager.LAUNCH_APP + " - Shortcut.lnk"));
			}
			return InstallationState.SUCCESS;
		}
		catch (Exception arg)
		{
			OnFatalError("Error", "Exception", arg);
			return InstallationState.FAILED;
		}
	}

	public string GetCurrentVersion()
	{
		try
		{
			if (!FileManager.FileExists(SettingsManager.VERSION_FILE_LOCAL_PATH))
			{
				return null;
			}
			return Rijndael.Decrypt(File.ReadAllText(SettingsManager.VERSION_FILE_LOCAL_PATH).Replace("\n", "").Replace("\r", ""), SettingsManager.PATCH_VERSION_ENCRYPTION_PASSWORD);
		}
		catch (Exception arg)
		{
			OnFatalError("Version file check error", "", arg);
			return null;
		}
	}

	public List<string> GetAvailableVersions()
	{
		List<string> list = new List<string>();
		try
		{
			if (_wasInit)
			{
				foreach (Version availableBuild in availableBuilds)
				{
					string item = availableBuild.ToString();
					list.Add(item);
				}
			}
			else
			{
				OnFatalError("GetAvailableVersions called before Init", "", null);
			}
		}
		catch (Exception arg)
		{
			OnFatalError("Exception during check for available versions", "", arg);
		}
		return list;
	}

	public void AbortSync()
	{
		abortSync = true;
		if (downloader != null)
		{
			downloader.Cancel();
		}
	}

	public bool CheckIntegrity(List<string> excludePatterns, List<string> excludeUnlessMissingPatterns)
	{
		abortSync = false;
		string installationPath = GetInstallationPath();
		bool flag = true;
		string currentVersion = GetCurrentVersion();
		try
		{
			if (_wasInit)
			{
				OnTaskStarted("Checking Integrity " + currentVersion);
				Version version = null;
				if (currentVersion != null)
				{
					version = new Version(currentVersion);
				}
				if (version == null)
				{
					OnTaskFailed(null);
					OnError("Error", "Version " + currentVersion + " is not valid", null);
					return false;
				}
				Version version2 = null;
				foreach (Version availableBuild in availableBuilds)
				{
					if (availableBuild.Equals(version))
					{
						version2 = availableBuild;
						break;
					}
				}
				if (version2 == null)
				{
					OnTaskFailed(null);
					OnError("Error", "Version " + currentVersion + " is not found", null);
					return false;
				}
				string text = version2.ToString();
				try
				{
					string text2 = null;
					string text3 = null;
					string basisContent = null;
					try
					{
						text2 = FileManager.DownloadFileToString(SettingsManager.BUILDS_DOWNLOAD_URL + "index_" + text + ".basis");
					}
					catch
					{
					}
					if (text2 != null)
					{
						text3 = text2.Trim();
						OnLog("Found basis version " + text3, "");
						try
						{
							basisContent = FileManager.DownloadFileToString(SettingsManager.BUILDS_DOWNLOAD_URL + "index_" + text3 + ".bix");
						}
						catch
						{
							OnTaskFailed(string.Concat("Check Integrity Failed: Requested build ", version, " basis version ", text3, " does not exist"));
							return false;
						}
					}
					string text4 = null;
					try
					{
						text4 = FileManager.DownloadFileToString(SettingsManager.BUILDS_DOWNLOAD_URL + "index_" + text + ".bix");
					}
					catch
					{
						OnTaskFailed(string.Concat("Check Integrity Failed: Requested build ", version, " does not exist"));
						return false;
					}
					List<BuildEntry> list = ProcessBuild(text3, basisContent, text, text4);
					if (list != null)
					{
						OnSetMainProgressBar(0, list.Count + 1);
						foreach (BuildEntry item in list)
						{
							if (abortSync)
							{
								OnTaskFailed("Check Integrity Aborted");
								return false;
							}
							_ = item.build;
							string file = item.file;
							string hash = item.hash;
							if (excludePatterns != null)
							{
								bool flag2 = false;
								foreach (string excludePattern in excludePatterns)
								{
									if (Regex.IsMatch(file, excludePattern))
									{
										flag2 = true;
										break;
									}
								}
								if (flag2)
								{
									OnIncreaseMainProgressBar();
									continue;
								}
							}
							bool flag3 = false;
							if (excludeUnlessMissingPatterns != null)
							{
								foreach (string excludeUnlessMissingPattern in excludeUnlessMissingPatterns)
								{
									if (Regex.IsMatch(file, excludeUnlessMissingPattern))
									{
										flag3 = true;
										break;
									}
								}
							}
							if (!file.Equals("version"))
							{
								if (FileManager.FileExists(Path.Combine(installationPath, file)))
								{
									if (!flag3)
									{
										OnLog("Checking integrity " + currentVersion, "Hash checking for " + file + "...");
										string value = Hashing.SHA1(Path.Combine(installationPath, file));
										if (!hash.Equals(value))
										{
											flag = false;
											OnLog("Checking integrity " + currentVersion, "Hash check for " + file + " FAILED");
										}
									}
								}
								else
								{
									flag = false;
									OnLog("Checking integrity " + currentVersion, "Missing file " + file);
								}
							}
							OnIncreaseMainProgressBar();
						}
						if (flag)
						{
							OnTaskCompleted("Checking integrity " + currentVersion + " Passed");
							return true;
						}
						OnTaskCompleted("Checking integrity " + currentVersion + " Failed. Repair required");
						OnLog(null, "See " + SettingsManager.LOGS_ERROR_PATH + " for details");
						return false;
					}
					OnError("Build " + text + " is corrupted. Index file empty", "", null);
				}
				catch (Exception arg)
				{
					OnError("Exception...", "Exception during check integrity", arg);
				}
			}
			else
			{
				OnError("CheckIntegrity called before Init", "", null);
			}
		}
		catch (Exception arg2)
		{
			OnError("Exception...", "Exception during check integrity", arg2);
		}
		OnTaskFailed(null);
		return false;
	}

	public bool SyncToVersion(string sDesiredVersion, List<string> excludePatterns, List<string> excludeUnlessMissingPatterns)
	{
		abortSync = false;
		string installationPath = GetInstallationPath();
		try
		{
			if (_wasInit)
			{
				OnTaskStarted("Syncing " + sDesiredVersion);
				Version version = null;
				if (sDesiredVersion != null)
				{
					version = new Version(sDesiredVersion);
				}
				if (version == null)
				{
					OnFatalError("Error", "Version " + sDesiredVersion + " is not valid", null);
					return false;
				}
				Version version2 = null;
				foreach (Version availableBuild in availableBuilds)
				{
					if (availableBuild.Equals(version))
					{
						version2 = availableBuild;
						break;
					}
				}
				if (version2 == null)
				{
					OnFatalError("Error", "Version " + sDesiredVersion + " is not found", null);
					return false;
				}
				string text = version2.ToString();
				try
				{
					string text2 = null;
					string text3 = null;
					string basisContent = null;
					try
					{
						text2 = FileManager.DownloadFileToString(SettingsManager.BUILDS_DOWNLOAD_URL + "index_" + text + ".basis");
					}
					catch
					{
					}
					if (text2 != null)
					{
						text3 = text2.Trim();
						OnLog("Found basis version " + text3, "");
						try
						{
							basisContent = FileManager.DownloadFileToString(SettingsManager.BUILDS_DOWNLOAD_URL + "index_" + text3 + ".bix");
						}
						catch
						{
							OnTaskFailed(string.Concat("Sync Failed: Requested build ", version, " basis version ", text3, " does not exist"));
							return false;
						}
					}
					string text4 = null;
					try
					{
						text4 = FileManager.DownloadFileToString(SettingsManager.BUILDS_DOWNLOAD_URL + "index_" + text + ".bix");
					}
					catch
					{
						OnTaskFailed(string.Concat("Sync Failed: Requested build ", version, " does not exist"));
						return false;
					}
					List<BuildEntry> list = ProcessBuild(text3, basisContent, text, text4);
					if (list != null)
					{
						OnSetMainProgressBar(0, list.Count + 1);
						foreach (BuildEntry item in list)
						{
							if (abortSync)
							{
								OnTaskFailed("Sync Aborted");
								return false;
							}
							string build = item.build;
							string file = item.file;
							string hash = item.hash;
							if (excludePatterns != null)
							{
								bool flag = false;
								foreach (string excludePattern in excludePatterns)
								{
									if (Regex.IsMatch(file, excludePattern))
									{
										flag = true;
										break;
									}
								}
								if (flag)
								{
									OnIncreaseMainProgressBar();
									continue;
								}
							}
							bool flag2 = false;
							if (excludeUnlessMissingPatterns != null)
							{
								foreach (string excludeUnlessMissingPattern in excludeUnlessMissingPatterns)
								{
									if (Regex.IsMatch(file, excludeUnlessMissingPattern))
									{
										flag2 = true;
										break;
									}
								}
							}
							if (!file.Equals("version"))
							{
								if (FileManager.FileExists(Path.Combine(installationPath, file)))
								{
									if (!flag2)
									{
										OnLog("Syncing " + sDesiredVersion, "Hash checking for " + file + "...");
										string value = Hashing.SHA1(Path.Combine(installationPath, file));
										if (!hash.Equals(value))
										{
											FileManager.DeleteFile(Path.Combine(installationPath, file));
											OnLog("Syncing " + sDesiredVersion, "Downloading " + file + "...");
											DownloadFileWithRetries(installationPath, file, build, hash);
										}
									}
								}
								else
								{
									FileManager.CreateDirectory(Path.GetDirectoryName(Path.Combine(installationPath, file)));
									OnLog("Syncing " + sDesiredVersion, "Downloading " + file + "...");
									DownloadFileWithRetries(installationPath, file, build, hash);
								}
							}
							OnFileProcessed(file);
							OnIncreaseMainProgressBar();
						}
						string text5 = Path.Combine(installationPath, "version");
						if (File.Exists(text5))
						{
							File.Delete(text5);
						}
						DownloadFile(SettingsManager.BUILDS_DOWNLOAD_URL + text + "/version", text5);
						OnTaskCompleted("Sync To " + sDesiredVersion + " Complete");
						return true;
					}
					OnFatalError("Build " + text + " is corrupted. Index file empty", "", null);
				}
				catch (Exception arg)
				{
					OnFatalError("Exception...", "Exception during version sync", arg);
				}
			}
			else
			{
				OnFatalError("SyncToVersion called before Init", "", null);
			}
		}
		catch (Exception arg2)
		{
			OnFatalError("Exception...", "Exception during version sync", arg2);
		}
		OnTaskFailed(null);
		return false;
	}

	public string GetInstallationPath()
	{
		if (!SettingsManager.INSTALL_IN_LOCAL_PATH)
		{
			return SettingsManager.PROGRAM_FILES_DIRECTORY_TO_INSTALL_ABS_PATH;
		}
		return SettingsManager.APP_PATH;
	}

	private bool ProcessBuildsIndex(string content)
	{
		if (content != null)
		{
			if (content != string.Empty)
			{
				availableBuilds = new List<Version>();
				string[] array = content.Split('\n');
				foreach (string text in array)
				{
					if (text != "")
					{
						availableBuilds.Add(new Version(text.Replace("\r", "").Replace("\n", "")));
					}
				}
				availableBuilds.Sort((Version v1, Version v2) => v2.CompareTo(v1));
				return true;
			}
			OnLog("No builds available!", "There are no remote builds to install!");
			return false;
		}
		OnLog("Can't find index file!", "Can't proceed with installation cause of missing build index file!");
		return false;
	}

	private List<BuildEntry> ProcessBuild(string basisBuild, string basisContent, string build, string content)
	{
		List<BuildEntry> list = new List<BuildEntry>();
		List<string> list2 = ProcessBuildIndex(content);
		if (list2 == null)
		{
			return null;
		}
		if (basisBuild != null && basisContent != null)
		{
			List<string> list3 = ProcessBuildIndex(basisContent);
			if (list3 == null)
			{
				return null;
			}
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			foreach (string item in list2)
			{
				string key = item.Split(SettingsManager.PATCHES_SYMBOL_SEPARATOR)[0];
				dictionary.Add(key, value: true);
			}
			foreach (string item2 in list3)
			{
				string[] array = item2.Split(SettingsManager.PATCHES_SYMBOL_SEPARATOR);
				string text = array[0];
				if (!dictionary.ContainsKey(text))
				{
					string hash = array[1];
					BuildEntry buildEntry = new BuildEntry();
					buildEntry.build = basisBuild;
					buildEntry.file = text;
					buildEntry.hash = hash;
					list.Add(buildEntry);
				}
			}
		}
		foreach (string item3 in list2)
		{
			string[] array2 = item3.Split(SettingsManager.PATCHES_SYMBOL_SEPARATOR);
			string file = array2[0];
			string hash2 = array2[1];
			BuildEntry buildEntry2 = new BuildEntry();
			buildEntry2.build = build;
			buildEntry2.file = file;
			buildEntry2.hash = hash2;
			list.Add(buildEntry2);
		}
		return list;
	}

	private List<string> ProcessBuildIndex(string content)
	{
		List<string> list = new List<string>();
		if (content != null)
		{
			if (content != string.Empty)
			{
				string[] array = content.Split('\n');
				foreach (string text in array)
				{
					if (text != "")
					{
						list.Add(text.Replace("\r", ""));
					}
				}
				return list;
			}
			return null;
		}
		return null;
	}

	private bool DownloadFile(string remote, string local)
	{
		if (downloader == null)
		{
			downloader = new FileDownloader();
			downloader.ProgressChanged += downloader_ProgressChanged;
			downloader.DownloadCancelled += downloader_Cancelled;
			downloader.DownloadComplete += downloader_Completed;
		}
		bool flag = false;
		for (int i = 0; i < 5; i++)
		{
			try
			{
				OnSetDetailProgressBar(0, 100);
				downloader.Download(remote, Path.GetDirectoryName(local));
				flag = true;
			}
			catch (Exception arg2)
			{
				string arg = "Failed to download " + local + " and will retry. Attempt " + (i + 1);
				OnError("Syncing...", arg, arg2);
			}
			if (flag)
			{
				break;
			}
		}
		if (!flag)
		{
			throw new Exception("Failed to download " + local + " after 5 retries. Aborting");
		}
		return downloader.WasCancelled();
	}

	private void DownloadFileWithRetries(string pathToInstall, string file, string build, string hash)
	{
		string text = Path.Combine(pathToInstall, file);
		if (DownloadFile(SettingsManager.BUILDS_DOWNLOAD_URL + build + "/" + file, text))
		{
			return;
		}
		string value = Hashing.SHA1(text);
		if (hash.Equals(value))
		{
			return;
		}
		OnLog("Syncing...", "Hash of downloaded file " + file + " is incorrect. Retrying download");
		FileManager.DeleteFile(text);
		if (!DownloadFile(SettingsManager.BUILDS_DOWNLOAD_URL + build + "/" + file, text))
		{
			value = Hashing.SHA1(text);
			if (!hash.Equals(value))
			{
				FileManager.DeleteFile(text);
				throw new Exception("Hash of downloaded file " + file + " is still incorrect after re-download. Aborting");
			}
		}
	}

	private void downloader_ProgressChanged(object sender, DownloadEventArgs e)
	{
		OnDownloadProgress(e.CurrentFileSize, e.TotalFileSize, e.PercentDone);
	}

	private void downloader_Cancelled(object sender, EventArgs e)
	{
		OnDownloadCancelled();
	}

	private void downloader_Completed(object sender, EventArgs e)
	{
		OnDownloadCompleted();
	}
}

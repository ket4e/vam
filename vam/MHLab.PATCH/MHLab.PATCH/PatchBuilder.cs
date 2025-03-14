using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHLab.PATCH.Compression;
using MHLab.PATCH.Debugging;
using MHLab.PATCH.Settings;
using MHLab.PATCH.Utilities;
using Octodiff.Core;

namespace MHLab.PATCH;

internal class PatchBuilder
{
	private List<string> _currentBuildFiles;

	private List<string> _buildsVersions;

	private List<string> _builtPatches;

	private Dictionary<string, string> _newFilesHashes;

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

	public PatchBuilder(bool createDirectories = true)
	{
		try
		{
			_buildsVersions = new List<string>();
			_currentBuildFiles = new List<string>();
			_builtPatches = new List<string>();
			if (createDirectories)
			{
				if (!FileManager.DirectoryExists(SettingsManager.CURRENT_BUILD_PATH))
				{
					FileManager.CreateDirectory(SettingsManager.CURRENT_BUILD_PATH);
				}
				if (!FileManager.DirectoryExists(SettingsManager.BUILDS_PATH))
				{
					FileManager.CreateDirectory(SettingsManager.BUILDS_PATH);
				}
				if (!FileManager.DirectoryExists(SettingsManager.FINAL_PATCHES_PATH))
				{
					FileManager.CreateDirectory(SettingsManager.FINAL_PATCHES_PATH);
				}
				if (!FileManager.DirectoryExists(SettingsManager.DEPLOY_PATH))
				{
					FileManager.CreateDirectory(SettingsManager.DEPLOY_PATH);
				}
				if (!FileManager.DirectoryExists(SettingsManager.PATCHER_FILES_PATH))
				{
					FileManager.CreateDirectory(SettingsManager.PATCHER_FILES_PATH);
				}
			}
			if (!FileManager.FileExists(FileManager.PathCombine(SettingsManager.FINAL_PATCHES_PATH, "versions.txt")))
			{
				FileManager.CreateFile(FileManager.PathCombine(SettingsManager.FINAL_PATCHES_PATH, "versions.txt"));
			}
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
		}
		catch (Exception ex)
		{
			OnFatalError("Fatal error", "Something goes terribly wrong during PatchBuilder init process!", ex);
			Debugger.Log(ex.Message);
		}
	}

	public List<string> GetCurrentBuildFiles()
	{
		_currentBuildFiles = FileManager.GetCurrentBuildFiles().ToList();
		return _currentBuildFiles;
	}

	public List<string> GetVersions()
	{
		List<string> list = FileManager.GetAllBuildsDirectories().ToList().Distinct()
			.ToList();
		_buildsVersions = new List<string>();
		List<Version> list2 = new List<Version>();
		foreach (string item in list)
		{
			string[] array = item.Split(Path.DirectorySeparatorChar);
			list2.Add(new Version(array[array.Count() - 1]));
		}
		list2.Sort((Version version1, Version version2) => version1.CompareTo(version2));
		_buildsVersions.AddRange(list2.Select((Version x) => x.ToString()));
		return _buildsVersions;
	}

	public string[] GetCurrentVersions()
	{
		if (_buildsVersions == null)
		{
			GetVersions();
		}
		return _buildsVersions.ToArray();
	}

	public string[] GetCurrentPatches()
	{
		List<string> list = FileManager.GetFiles(SettingsManager.FINAL_PATCHES_PATH, "*.pix").ToList();
		_builtPatches = new List<string>();
		foreach (string item in list)
		{
			string[] array = item.Split(Path.DirectorySeparatorChar);
			_builtPatches.Add(array[array.Count() - 1].Replace(".pix", ""));
		}
		return _builtPatches.ToArray();
	}

	public string GetLastVersion()
	{
		GetVersions();
		if (_buildsVersions.Count > 0)
		{
			return _buildsVersions.Last();
		}
		return "none";
	}

	public string GetLastBaseVersion()
	{
		GetVersions();
		if (_buildsVersions.Count > 0)
		{
			for (int num = _buildsVersions.Count - 1; num >= 0; num--)
			{
				string text = _buildsVersions[num];
				if (!File.Exists(SettingsManager.BUILDS_PATH + "index_" + text + ".basis"))
				{
					return text;
				}
			}
		}
		return "none";
	}

	public void BuildNewVersion(string v)
	{
		try
		{
			OnTaskStarted("New version creation started...");
			OnSetMainProgressBar(0, 7);
			Version version = new Version(v);
			OnSetDetailProgressBar(0, 1);
			if (FileManager.DirectoryExists(SettingsManager.BUILDS_PATH + version))
			{
				OnError("Error!", string.Concat("This version (", version, ") already exists!"), null);
				throw new Exception(string.Concat("This version (", version, ") already exists!"));
			}
			OnIncreaseDetailProgressBar();
			OnIncreaseMainProgressBar();
			if (FileManager.IsDirectoryEmpty(SettingsManager.CURRENT_BUILD_PATH))
			{
				OnError("Error!", "There are no current builds!", null);
				throw new Exception("There are no current builds!");
			}
			OnIncreaseDetailProgressBar();
			OnIncreaseMainProgressBar();
			try
			{
				OnSetDetailProgressBar(0, 1);
				FileManager.CreateDirectory(SettingsManager.BUILDS_PATH + version);
				OnIncreaseDetailProgressBar();
				OnIncreaseMainProgressBar();
				OnSetDetailProgressBar(0, 1);
				FileManager.CopyDirectory(SettingsManager.CURRENT_BUILD_PATH, string.Concat(SettingsManager.BUILDS_PATH, version, Path.DirectorySeparatorChar.ToString()));
				OnIncreaseDetailProgressBar();
				OnIncreaseMainProgressBar();
				OnSetDetailProgressBar(0, 1);
				BuildVersionFile(version);
				OnIncreaseDetailProgressBar();
				OnIncreaseMainProgressBar();
				OnSetDetailProgressBar(0, 1);
				UpdateBuildsIndex(version);
				OnIncreaseDetailProgressBar();
				OnIncreaseMainProgressBar();
				OnSetDetailProgressBar(0, 1);
				FileManager.CleanDirectory(SettingsManager.CURRENT_BUILD_PATH);
				OnIncreaseDetailProgressBar();
				OnIncreaseMainProgressBar();
				OnLog("Build completed!", "A new build version has been processed!");
				OnTaskCompleted(string.Concat("Build completed! Check your \"builds\" directory to find your ", version, " build!"));
			}
			catch (Exception ex)
			{
				OnError("Error!", "Can't complete new version building process!", ex);
				throw ex;
			}
		}
		catch (Exception ex2)
		{
			OnError("Error!", "Something went wrong during new version building process init!", ex2);
			throw ex2;
		}
	}

	public void BuildIncrementalVersion(string sVersionFrom, string sVersionTo)
	{
		OnTaskStarted("Incremental version creation started...");
		if (sVersionFrom == sVersionTo)
		{
			OnError("Error!", "Can't build a patch! Versions are the same!", null);
			throw new Exception("Can't build a patch! Versions are the same!");
		}
		Version versionFrom = new Version(sVersionFrom);
		Version version = new Version(sVersionTo);
		if (FileManager.DirectoryExists(SettingsManager.BUILDS_PATH + sVersionTo))
		{
			OnError("Error!", "This version (" + sVersionTo + ") already exists!", null);
			throw new Exception("This version (" + sVersionTo + ") already exists!");
		}
		if (File.Exists(string.Concat(SettingsManager.BUILDS_PATH, "index_", versionFrom, ".basis")))
		{
			OnError("Error!", "The build from version (" + sVersionFrom + ") is a basis build. Cannot build incremental from another basis build!", null);
			throw new Exception("The build from version (" + sVersionFrom + ") is a basis build. Cannot build incremental from another basis build!");
		}
		if (FileManager.IsDirectoryEmpty(SettingsManager.CURRENT_BUILD_PATH))
		{
			OnError("Error!", "There are no current builds!", null);
			throw new Exception("There are no current builds!");
		}
		List<string> list = (from f in FileManager.GetFiles(SettingsManager.BUILDS_PATH + versionFrom).ToList()
			select f = f.Replace(string.Concat(SettingsManager.BUILDS_PATH, versionFrom, Path.DirectorySeparatorChar.ToString()), "")).ToList();
		List<string> list2 = (from f in FileManager.GetFiles(SettingsManager.CURRENT_BUILD_PATH).ToList()
			select f = f.Replace(SettingsManager.CURRENT_BUILD_PATH, "")).ToList();
		int arg = list.Count + list2.Count + 5;
		OnSetMainProgressBar(0, arg);
		foreach (string file in list2)
		{
			try
			{
				if (IsFileOSRelated(file))
				{
					continue;
				}
				Debugger.Log("Process current file " + file);
				string text = SettingsManager.CURRENT_BUILD_PATH + file;
				OnFileProcessing(text);
				OnSetDetailProgressBar(0, 1);
				if (list.Contains(file))
				{
					string text2 = Hashing.SHA1(text);
					string text3 = Hashing.SHA1(string.Concat(SettingsManager.BUILDS_PATH, versionFrom, Path.DirectorySeparatorChar.ToString(), list.Single((string f) => f == file)));
					if (text2 != text3)
					{
						Debugger.Log("Hash not same");
						string text4 = string.Concat(SettingsManager.BUILDS_PATH, version, Path.DirectorySeparatorChar.ToString(), file);
						FileManager.CreateDirectory(Path.GetDirectoryName(text4));
						File.Copy(text, text4, overwrite: true);
					}
				}
				else
				{
					Debugger.Log("Missing");
					string text5 = string.Concat(SettingsManager.BUILDS_PATH, version, Path.DirectorySeparatorChar.ToString(), file);
					FileManager.CreateDirectory(Path.GetDirectoryName(text5));
					File.Copy(text, text5, overwrite: true);
				}
				OnFileProcessed(text);
				OnIncreaseDetailProgressBar();
				OnIncreaseMainProgressBar();
			}
			catch (Exception ex)
			{
				OnError("Error!", "An error occurred during incremental build process!", ex);
				throw ex;
			}
		}
		OnSetDetailProgressBar(0, 1);
		BuildVersionFile(version);
		OnIncreaseDetailProgressBar();
		OnIncreaseMainProgressBar();
		OnSetDetailProgressBar(0, 1);
		UpdateBuildsIndex(version);
		OnIncreaseDetailProgressBar();
		OnIncreaseMainProgressBar();
		string text6 = string.Concat(SettingsManager.BUILDS_PATH, "index_", version, ".basis");
		FileManager.CreateFile(text6);
		File.WriteAllText(text6, versionFrom.ToString());
		OnSetDetailProgressBar(0, 1);
		FileManager.CleanDirectory(SettingsManager.CURRENT_BUILD_PATH);
		OnIncreaseDetailProgressBar();
		OnIncreaseMainProgressBar();
		OnLog("Build completed!", "A new build version has been processed!");
		OnTaskCompleted(string.Concat("Build completed! Check your \"builds\" directory to find your ", version, " build!"));
	}

	public void BuildPatch(string sVersionFrom, string sVersionTo, CompressionType type)
	{
		OnTaskStarted("Patch building started...");
		if (sVersionFrom == sVersionTo)
		{
			OnError("Error!", "Can't build a patch! Versions are the same!", null);
			throw new Exception("Can't build a patch! Versions are the same!");
		}
		Version versionFrom = new Version(sVersionFrom);
		Version versionTo = new Version(sVersionTo);
		_newFilesHashes = new Dictionary<string, string>();
		List<string> list = (from f in FileManager.GetFiles(SettingsManager.BUILDS_PATH + versionFrom).ToList()
			select f = f.Replace(string.Concat(SettingsManager.BUILDS_PATH, versionFrom, Path.DirectorySeparatorChar.ToString()), "")).ToList();
		List<string> list2 = (from f in FileManager.GetFiles(SettingsManager.BUILDS_PATH + versionTo).ToList()
			select f = f.Replace(string.Concat(SettingsManager.BUILDS_PATH, versionTo, Path.DirectorySeparatorChar.ToString()), "")).ToList();
		Patch patch = new Patch(versionFrom, versionTo);
		int arg = list.Count + list2.Count + 5;
		OnSetMainProgressBar(0, arg);
		foreach (string file in list2)
		{
			try
			{
				if (IsFileOSRelated(file))
				{
					continue;
				}
				OnFileProcessing(string.Concat(SettingsManager.BUILDS_PATH, versionTo, Path.DirectorySeparatorChar.ToString(), file));
				OnSetDetailProgressBar(0, 1);
				FileManager.CreateDirectory(Path.GetDirectoryName(SettingsManager.PATCHES_PATH + patch.PatchName + Path.DirectorySeparatorChar + file));
				if (list.Contains(file))
				{
					string text = Hashing.SHA1(string.Concat(SettingsManager.BUILDS_PATH, versionTo, Path.DirectorySeparatorChar.ToString(), file));
					string text2 = Hashing.SHA1(string.Concat(SettingsManager.BUILDS_PATH, versionFrom, Path.DirectorySeparatorChar.ToString(), list.Single((string f) => f == file)));
					if (text != text2)
					{
						string patchName = SettingsManager.PATCHES_PATH + patch.PatchName + Path.DirectorySeparatorChar + file + SettingsManager.PATCH_EXTENSION;
						BuildOctodiffPatch(string.Concat(SettingsManager.BUILDS_PATH, versionFrom, Path.DirectorySeparatorChar.ToString(), list.Single((string f) => f == file)), string.Concat(SettingsManager.BUILDS_PATH, versionTo, Path.DirectorySeparatorChar.ToString(), file), patchName);
						_newFilesHashes.Add(file, text);
					}
				}
				else
				{
					FileManager.FileCopy(string.Concat(SettingsManager.BUILDS_PATH, versionTo, Path.DirectorySeparatorChar.ToString(), file), SettingsManager.PATCHES_PATH + patch.PatchName + Path.DirectorySeparatorChar + file);
				}
				OnFileProcessed(string.Concat(SettingsManager.BUILDS_PATH, versionTo, Path.DirectorySeparatorChar.ToString(), file));
				OnIncreaseDetailProgressBar();
				OnIncreaseMainProgressBar();
			}
			catch (Exception ex)
			{
				OnError("Error!", "An error occurred during patch building process!", ex);
				throw ex;
			}
		}
		foreach (string item in list)
		{
			if (!IsFileOSRelated(item))
			{
				OnFileProcessing(string.Concat(SettingsManager.BUILDS_PATH, versionFrom, Path.DirectorySeparatorChar.ToString(), item));
				OnSetDetailProgressBar(0, 1);
				if (!list2.Contains(item))
				{
					FileManager.CreateDirectory(Path.GetDirectoryName(SettingsManager.PATCHES_PATH + patch.PatchName + Path.DirectorySeparatorChar + item));
					FileManager.CreateFile(SettingsManager.PATCHES_PATH + patch.PatchName + Path.DirectorySeparatorChar + item + SettingsManager.PATCH_DELETE_FILE_EXTENSION);
				}
				OnFileProcessed(string.Concat(SettingsManager.BUILDS_PATH, versionFrom, Path.DirectorySeparatorChar.ToString(), item));
				OnIncreaseDetailProgressBar();
				OnIncreaseMainProgressBar();
			}
		}
		OnLog("Cleaning up patch workspace...", "Deleting signature files...");
		OnSetDetailProgressBar(0, 1);
		FileManager.DeleteFiles(SettingsManager.PATCHES_PATH + patch.PatchName + Path.DirectorySeparatorChar, "*.signature");
		OnIncreaseDetailProgressBar();
		OnIncreaseMainProgressBar();
		OnLog("Patch compression...", "Creating a compressed archive for " + patch.PatchName + " patch...");
		OnSetDetailProgressBar(0, 1);
		Compressor.Compress(SettingsManager.PATCHES_PATH + patch.PatchName + Path.DirectorySeparatorChar, SettingsManager.FINAL_PATCHES_PATH + patch.ArchiveName, type, null);
		OnIncreaseDetailProgressBar();
		OnIncreaseMainProgressBar();
		OnLog("Cleaning up patch workspace...", "Deleting useless files...");
		OnSetDetailProgressBar(0, 1);
		FileManager.DeleteDirectory(SettingsManager.PATCHES_PATH);
		OnIncreaseDetailProgressBar();
		OnIncreaseMainProgressBar();
		OnSetDetailProgressBar(0, 2);
		BuildPatchVersionsFile(patch, versionFrom, versionTo, type);
		OnIncreaseDetailProgressBar();
		OnIncreaseMainProgressBar();
		OnLog("Creating index...", "Creating patch index for clients files validation...");
		GeneratePatchIndex(patch);
		OnIncreaseDetailProgressBar();
		OnIncreaseMainProgressBar();
		OnLog("Patch completed!", "");
		OnTaskCompleted(string.Concat("Patch completed! Check your \"patches\" directory to find your ", versionFrom, "_", versionTo, " patch!"));
	}

	private void BuildVersionFile(Version v)
	{
		using FileStream stream = new FileStream(string.Concat(SettingsManager.BUILDS_PATH, v, Path.DirectorySeparatorChar.ToString(), "version"), FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
		using StreamWriter streamWriter = new StreamWriter(stream);
		streamWriter.WriteLine(Rijndael.Encrypt(v.ToString(), SettingsManager.PATCH_VERSION_ENCRYPTION_PASSWORD));
	}

	private void UpdateBuildsIndex(Version v)
	{
		string text = string.Concat(SettingsManager.BUILDS_PATH, "index_", v, ".bix");
		string text2 = SettingsManager.BUILDS_PATH + "index";
		FileManager.CreateFile(text);
		FileManager.CreateFile(text2);
		using (FileStream stream = new FileStream(text2, FileMode.Append, FileAccess.Write))
		{
			using StreamWriter streamWriter = new StreamWriter(stream);
			streamWriter.WriteLine(v.ToString());
		}
		IEnumerable<string> files = FileManager.GetFiles(SettingsManager.BUILDS_PATH + v, "*", SearchOption.AllDirectories);
		using FileStream stream2 = new FileStream(text, FileMode.Append, FileAccess.Write);
		using StreamWriter streamWriter2 = new StreamWriter(stream2);
		foreach (string item in files)
		{
			string text3 = item.Replace(string.Concat(SettingsManager.BUILDS_PATH, v, Path.DirectorySeparatorChar.ToString()), "");
			OnLog("Generate builds index", "Processing b-index for " + text3 + "...");
			streamWriter2.WriteLine(text3 + SettingsManager.PATCHES_SYMBOL_SEPARATOR + Hashing.SHA1(item));
		}
		OnLog("Builds index completed", "Builds index is processed successfully!");
	}

	private void BuildOctodiffPatch(string fromFileName, string toFileName, string patchName)
	{
		try
		{
			SignatureBuilder signatureBuilder = new SignatureBuilder();
			DeltaBuilder deltaBuilder = new DeltaBuilder();
			FileStream fileStream = new FileStream(fromFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			FileStream fileStream2 = new FileStream(toFileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
			FileStream fileStream3 = new FileStream(patchName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
			FileStream fileStream4 = new FileStream(patchName + ".signature", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
			signatureBuilder.Build(fileStream, new SignatureWriter(fileStream4));
			fileStream4.Close();
			fileStream4.Dispose();
			deltaBuilder.BuildDelta(fileStream2, new SignatureReader(fileStream4.Name, deltaBuilder.ProgressReporter), new AggregateCopyOperationsDecorator(new BinaryDeltaWriter(fileStream3)));
			fileStream.Close();
			fileStream.Dispose();
			fileStream2.Close();
			fileStream2.Dispose();
			fileStream3.Close();
			fileStream3.Dispose();
		}
		catch (Exception ex)
		{
			OnError("Error!", "An error occurred while building patch between " + fromFileName + " and " + toFileName, ex);
			throw ex;
		}
	}

	private void BuildPatchVersionsFile(Patch patch, Version versionFrom, Version versionTo, CompressionType type)
	{
		using FileStream s = new FileStream(SettingsManager.FINAL_PATCHES_PATH + patch.ArchiveName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		OnLog("Writing version...", "Writing new version on version.txt file...");
		using FileStream stream = new FileStream(SettingsManager.FINAL_PATCHES_PATH + "versions.txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
		using StreamWriter streamWriter = new StreamWriter(stream);
		string text = Hashing.SHA1(s);
		streamWriter.WriteLine(versionFrom.ToString() + SettingsManager.PATCHES_SYMBOL_SEPARATOR + versionTo.ToString() + SettingsManager.PATCHES_SYMBOL_SEPARATOR + text + SettingsManager.PATCHES_SYMBOL_SEPARATOR + type);
	}

	private void GeneratePatchIndex(Patch patch)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<string, string> newFilesHash in _newFilesHashes)
		{
			stringBuilder.AppendLine(newFilesHash.Key.Replace('\\', '/') + SettingsManager.PATCHES_SYMBOL_SEPARATOR + newFilesHash.Value);
		}
		using FileStream stream = File.Create(SettingsManager.FINAL_PATCHES_PATH + patch.PatchName + ".pix");
		using StreamWriter streamWriter = new StreamWriter(stream);
		streamWriter.Write(Rijndael.Encrypt(stringBuilder.ToString(), SettingsManager.PATCH_VERSION_ENCRYPTION_PASSWORD));
	}

	private bool IsFileOSRelated(string file)
	{
		if (file.Contains(".DS_Store") || file.Contains("desktop.ini"))
		{
			return true;
		}
		return false;
	}
}

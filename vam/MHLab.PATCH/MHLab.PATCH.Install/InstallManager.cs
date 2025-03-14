using System;
using System.Collections.Generic;
using System.IO;
using MHLab.PATCH.Settings;
using MHLab.PATCH.Utilities;

namespace MHLab.PATCH.Install;

public class InstallManager
{
	private Installer m_installer;

	public SettingsOverrider SETTINGS = new SettingsOverrider();

	public InstallManager()
	{
		m_installer = new Installer();
	}

	public void SetPatchBuilderOnFileProcessedAction(Action<string> action)
	{
		m_installer.OnFileProcessed = action;
	}

	public void SetOnFileProcessingAction(Action<string> action)
	{
		m_installer.OnFileProcessing = action;
	}

	public void SetOnTaskStartedAction(Action<string> action)
	{
		m_installer.OnTaskStarted = action;
	}

	public void SetOnTaskFailedAction(Action<string> action)
	{
		m_installer.OnTaskFailed = action;
	}

	public void SetOnTaskCompletedAction(Action<string> action)
	{
		m_installer.OnTaskCompleted = action;
	}

	public void SetOnLogAction(Action<string, string> action)
	{
		m_installer.OnLog = action;
	}

	public void SetOnErrorAction(Action<string, string, Exception> action)
	{
		m_installer.OnError = action;
	}

	public void SetOnFatalErrorAction(Action<string, string, Exception> action)
	{
		m_installer.OnFatalError = action;
	}

	public void SetOnSetMainProgressBarAction(Action<int, int> action)
	{
		m_installer.OnSetMainProgressBar = action;
	}

	public void SetOnSetDetailProgressBarAction(Action<int, int> action)
	{
		m_installer.OnSetDetailProgressBar = action;
	}

	public void SetOnIncreaseMainProgressBarAction(Action action)
	{
		m_installer.OnIncreaseMainProgressBar = action;
	}

	public void SetOnIncreaseDetailProgressBarAction(Action action)
	{
		m_installer.OnIncreaseDetailProgressBar = action;
	}

	public void SetOnDownloadProgressAction(Action<long, long, int> action)
	{
		m_installer.OnDownloadProgress = action;
	}

	public void SetOnDownloadCancelledAction(Action action)
	{
		m_installer.OnDownloadCancelled = action;
	}

	public void SetOnDownloadCompletedAction(Action action)
	{
		m_installer.OnDownloadCompleted = action;
	}

	public void SetDevIndex(bool b)
	{
		m_installer.useDevIndex = b;
	}

	public bool Init()
	{
		return m_installer.Init();
	}

	public void AbortSync()
	{
		m_installer.AbortSync();
	}

	public string GetCurrentVersion()
	{
		return m_installer.GetCurrentVersion();
	}

	public List<string> GetAvailableVersions()
	{
		return m_installer.GetAvailableVersions();
	}

	public string GetLatestAvailableVersion()
	{
		List<string> availableVersions = m_installer.GetAvailableVersions();
		if (availableVersions != null && availableVersions.Count > 0)
		{
			return availableVersions[0];
		}
		return null;
	}

	public bool SyncToVersion(string version, List<string> excludePatterns, List<string> excludeUnlessMissingPatterns)
	{
		return m_installer.SyncToVersion(version, excludePatterns, excludeUnlessMissingPatterns);
	}

	public bool CheckIntegrity(List<string> excludePatterns, List<string> excludeUnlessMissingPatterns)
	{
		return m_installer.CheckIntegrity(excludePatterns, excludeUnlessMissingPatterns);
	}

	public InstallationState InstallPatcher()
	{
		return m_installer.InstallPatcher(GetInstallationPath());
	}

	public InstallationState CreateShortcut(bool toLauncher = true)
	{
		return m_installer.InstallShortcut(GetInstallationPath(), toLauncher);
	}

	public string GetInstallationPath()
	{
		return m_installer.GetInstallationPath();
	}

	public bool LoadSettings()
	{
		try
		{
			if (FileManager.FileExists(SettingsManager.LAUNCHER_CONFIG_PATH))
			{
				string cipherText = File.ReadAllText(SettingsManager.LAUNCHER_CONFIG_PATH);
				cipherText = Rijndael.Decrypt(cipherText, SettingsManager.PATCH_VERSION_ENCRYPTION_PASSWORD);
				SETTINGS = SettingsOverrider.XmlDeserializeFromString<SettingsOverrider>(cipherText);
				SETTINGS.OverrideSettings();
				return true;
			}
			throw new FileNotFoundException("Config file cannot be loaded!", SettingsManager.LAUNCHER_CONFIG_PATH);
		}
		catch (Exception arg)
		{
			m_installer.OnFatalError("Exception", "LoadSettings", arg);
		}
		return false;
	}
}

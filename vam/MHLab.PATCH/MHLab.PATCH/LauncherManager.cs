using System;
using System.IO;
using MHLab.PATCH.Debugging;
using MHLab.PATCH.Settings;
using MHLab.PATCH.Utilities;

namespace MHLab.PATCH;

public class LauncherManager
{
	private PatchApplier m_patchApplier;

	public SettingsOverrider SETTINGS = new SettingsOverrider();

	public LauncherManager()
	{
		m_patchApplier = new PatchApplier();
	}

	public void SetPatchBuilderOnFileProcessedAction(Action<string> action)
	{
		m_patchApplier.OnFileProcessed = action;
	}

	public void SetOnFileProcessingAction(Action<string> action)
	{
		m_patchApplier.OnFileProcessing = action;
	}

	public void SetOnTaskStartedAction(Action<string> action)
	{
		m_patchApplier.OnTaskStarted = action;
	}

	public void SetOnTaskCompletedAction(Action<string> action)
	{
		m_patchApplier.OnTaskCompleted = action;
	}

	public void SetOnLogAction(Action<string, string> action)
	{
		m_patchApplier.OnLog = action;
	}

	public void SetOnErrorAction(Action<string, string, Exception> action)
	{
		m_patchApplier.OnError = action;
	}

	public void SetOnFatalErrorAction(Action<string, string, Exception> action)
	{
		m_patchApplier.OnFatalError = action;
	}

	public void SetOnSetMainProgressBarAction(Action<int, int> action)
	{
		m_patchApplier.OnSetMainProgressBar = action;
	}

	public void SetOnSetDetailProgressBarAction(Action<int, int> action)
	{
		m_patchApplier.OnSetDetailProgressBar = action;
	}

	public void SetOnIncreaseMainProgressBarAction(Action action)
	{
		m_patchApplier.OnIncreaseMainProgressBar = action;
	}

	public void SetOnIncreaseDetailProgressBarAction(Action action)
	{
		m_patchApplier.OnIncreaseDetailProgressBar = action;
	}

	public void SetOnDownloadProgressAction(Action<long, long, int> action)
	{
		m_patchApplier.OnDownloadProgress = action;
	}

	public void SetOnDownloadCompletedAction(Action action)
	{
		m_patchApplier.OnDownloadCompleted = action;
	}

	public bool CheckForUpdates()
	{
		return m_patchApplier.CheckForUpdates();
	}

	public string GetCurrentVersion()
	{
		return m_patchApplier.GetCurrentVersion();
	}

	public bool IsDirty()
	{
		return m_patchApplier.IsDirtyWorkspace;
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
		}
		catch (Exception ex)
		{
			Debugger.Log(ex.Message);
		}
		return false;
	}
}

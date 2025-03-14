using System;
using System.Collections.Generic;
using MHLab.PATCH.Compression;
using MHLab.PATCH.Debugging;

namespace MHLab.PATCH;

public class PatchManager
{
	private PatchBuilder m_patchBuilder;

	public PatchManager()
	{
		m_patchBuilder = new PatchBuilder();
		Debugger.Clean();
	}

	public void SetPatchBuilderOnFileProcessedAction(Action<string> action)
	{
		m_patchBuilder.OnFileProcessed = action;
	}

	public void SetOnFileProcessingAction(Action<string> action)
	{
		m_patchBuilder.OnFileProcessing = action;
	}

	public void SetOnTaskStartedAction(Action<string> action)
	{
		m_patchBuilder.OnTaskStarted = action;
	}

	public void SetOnTaskCompletedAction(Action<string> action)
	{
		m_patchBuilder.OnTaskCompleted = action;
	}

	public void SetOnLogAction(Action<string, string> action)
	{
		m_patchBuilder.OnLog = action;
	}

	public void SetOnErrorAction(Action<string, string, Exception> action)
	{
		m_patchBuilder.OnError = action;
	}

	public void SetOnFatalErrorAction(Action<string, string, Exception> action)
	{
		m_patchBuilder.OnFatalError = action;
	}

	public void SetOnSetMainProgressBarAction(Action<int, int> action)
	{
		m_patchBuilder.OnSetMainProgressBar = action;
	}

	public void SetOnSetDetailProgressBarAction(Action<int, int> action)
	{
		m_patchBuilder.OnSetDetailProgressBar = action;
	}

	public void SetOnIncreaseMainProgressBarAction(Action action)
	{
		m_patchBuilder.OnIncreaseMainProgressBar = action;
	}

	public void SetOnIncreaseDetailProgressBarAction(Action action)
	{
		m_patchBuilder.OnIncreaseDetailProgressBar = action;
	}

	public void BuildNewVersion(string version)
	{
		m_patchBuilder.BuildNewVersion(version);
	}

	public void BuildIncrementalVersion(string versionFrom, string versionTo)
	{
		m_patchBuilder.BuildIncrementalVersion(versionFrom, versionTo);
	}

	public void BuildPatch(string versionFrom, string versionTo, CompressionType type)
	{
		m_patchBuilder.BuildPatch(versionFrom, versionTo, type);
	}

	public string GetLastVersion()
	{
		return m_patchBuilder.GetLastVersion();
	}

	public string GetLastBaseVersion()
	{
		return m_patchBuilder.GetLastBaseVersion();
	}

	public string[] GetCurrentVersions()
	{
		return m_patchBuilder.GetCurrentVersions();
	}

	public string[] GetCurrentPatches()
	{
		return m_patchBuilder.GetCurrentPatches();
	}

	public List<string> GetVersions()
	{
		return m_patchBuilder.GetVersions();
	}

	public void DeployCompress(string folderPath, string outputPath, CompressionType type)
	{
		Compressor.Compress(folderPath, outputPath, type, null);
	}
}

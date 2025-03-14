using System;

namespace MHLab.PATCH.Downloader;

public class DownloadEventArgs : EventArgs
{
	private int percentDone;

	private string downloadState;

	private long totalFileSize;

	private long currentFileSize;

	public long TotalFileSize
	{
		get
		{
			return totalFileSize;
		}
		set
		{
			totalFileSize = value;
		}
	}

	public long CurrentFileSize
	{
		get
		{
			return currentFileSize;
		}
		set
		{
			currentFileSize = value;
		}
	}

	public int PercentDone => percentDone;

	public string DownloadState => downloadState;

	public DownloadEventArgs(long totalFileSize, long currentFileSize)
	{
		this.totalFileSize = totalFileSize;
		this.currentFileSize = currentFileSize;
		percentDone = (int)((double)currentFileSize / (double)totalFileSize * 100.0);
	}

	public DownloadEventArgs(string state)
	{
		downloadState = state;
	}

	public DownloadEventArgs(int percentDone, string state)
	{
		this.percentDone = percentDone;
		downloadState = state;
	}
}

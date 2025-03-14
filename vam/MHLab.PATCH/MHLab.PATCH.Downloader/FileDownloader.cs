using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using MHLab.PATCH.Debugging;
using MHLab.PATCH.Settings;

namespace MHLab.PATCH.Downloader;

internal class FileDownloader
{
	private int downloadBlockSize = SettingsManager.DOWNLOAD_BUFFER_SIZE;

	private bool cancelled;

	private string downloadingTo;

	private IWebProxy proxy;

	public string DownloadingTo => downloadingTo;

	public IWebProxy Proxy
	{
		get
		{
			return proxy;
		}
		set
		{
			proxy = value;
		}
	}

	public event DownloadProgressHandler ProgressChanged;

	public event EventHandler DownloadComplete;

	public event EventHandler DownloadCancelled;

	public void Cancel()
	{
		Debugger.Log("Cancel Called");
		cancelled = true;
	}

	public bool WasCancelled()
	{
		return cancelled;
	}

	private void OnDownloadComplete()
	{
		if (this.DownloadComplete != null)
		{
			this.DownloadComplete(this, new EventArgs());
		}
	}

	private void OnDownloadCancelled()
	{
		if (this.DownloadCancelled != null)
		{
			this.DownloadCancelled(this, new EventArgs());
		}
	}

	public void Download(string url)
	{
		Download(url, "");
	}

	public void Download(string url, string destFolder)
	{
		DownloadData downloadData = null;
		cancelled = false;
		try
		{
			downloadData = DownloadData.Create(url, destFolder, proxy);
			string fileName = Path.GetFileName(downloadData.Response.ResponseUri.ToString());
			destFolder = destFolder.Replace("file:///", "").Replace("file://", "");
			downloadingTo = Path.Combine(destFolder, fileName);
			if (File.Exists(downloadingTo))
			{
				File.Delete(downloadingTo);
			}
			using (FileStream fileStream = File.Create(downloadingTo))
			{
				fileStream.Dispose();
				fileStream.Close();
			}
			byte[] buffer = new byte[downloadBlockSize];
			long num = downloadData.StartPoint;
			bool flag = false;
			using (FileStream fileStream2 = File.Open(downloadingTo, FileMode.Append, FileAccess.Write, FileShare.Write | FileShare.Delete))
			{
				int num2;
				while ((num2 = downloadData.DownloadStream.Read(buffer, 0, downloadBlockSize)) > 0)
				{
					if (cancelled)
					{
						Debugger.Log("Download cancelled");
						flag = true;
						downloadData.Close();
						break;
					}
					num += num2;
					SaveToFile(buffer, num2, fileStream2);
					if (downloadData.IsProgressKnown)
					{
						RaiseProgressChanged(num, downloadData.FileSize);
					}
					if (cancelled)
					{
						Debugger.Log("Download cancelled");
						flag = true;
						downloadData.Close();
						break;
					}
				}
				fileStream2.Dispose();
				fileStream2.Close();
			}
			if (flag)
			{
				OnDownloadCancelled();
			}
			else
			{
				OnDownloadComplete();
			}
		}
		catch (UriFormatException innerException)
		{
			throw new ArgumentException($"Could not parse the URL \"{url}\" - it's either malformed or is an unknown protocol.", innerException);
		}
		finally
		{
			downloadData?.Close();
		}
	}

	public void Download(List<string> urlList)
	{
		Download(urlList, "");
	}

	public void Download(List<string> urlList, string destFolder)
	{
		if (urlList == null)
		{
			throw new ArgumentException("Url list not specified.");
		}
		if (urlList.Count == 0)
		{
			throw new ArgumentException("Url list empty.");
		}
		Exception ex = null;
		foreach (string url in urlList)
		{
			ex = null;
			try
			{
				Download(url, destFolder);
			}
			catch (Exception ex2)
			{
				ex = ex2;
			}
			if (ex == null)
			{
				break;
			}
		}
		if (ex != null)
		{
			throw ex;
		}
	}

	public void AsyncDownload(string url)
	{
		ThreadPool.QueueUserWorkItem(WaitCallbackMethod, new string[2] { url, "" });
	}

	public void AsyncDownload(string url, string destFolder)
	{
		ThreadPool.QueueUserWorkItem(WaitCallbackMethod, new string[2] { url, destFolder });
	}

	public void AsyncDownload(List<string> urlList, string destFolder)
	{
		ThreadPool.QueueUserWorkItem(WaitCallbackMethod, new object[2] { urlList, destFolder });
	}

	public void AsyncDownload(List<string> urlList)
	{
		ThreadPool.QueueUserWorkItem(WaitCallbackMethod, new object[2] { urlList, "" });
	}

	private void WaitCallbackMethod(object data)
	{
		if (data is string[])
		{
			string[] array = data as string[];
			Download(array[0], array[1]);
			return;
		}
		object[] obj = data as object[];
		List<string> urlList = obj[0] as List<string>;
		string destFolder = obj[1] as string;
		Download(urlList, destFolder);
	}

	private void SaveToFile(byte[] buffer, int count, FileStream file)
	{
		try
		{
			file.Write(buffer, 0, count);
		}
		catch (Exception ex)
		{
			throw new Exception($"Error trying to save file \"{file.Name}\": {ex.Message}", ex);
		}
	}

	private void RaiseProgressChanged(long current, long target)
	{
		if (this.ProgressChanged != null)
		{
			this.ProgressChanged(this, new DownloadEventArgs(target, current));
		}
	}
}

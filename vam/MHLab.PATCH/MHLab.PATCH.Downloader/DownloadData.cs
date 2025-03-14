using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using MHLab.PATCH.Settings;

namespace MHLab.PATCH.Downloader;

internal class DownloadData
{
	private WebResponse response;

	private Stream stream;

	private long size;

	private long start;

	private IWebProxy proxy;

	public WebResponse Response
	{
		get
		{
			return response;
		}
		set
		{
			response = value;
		}
	}

	public Stream DownloadStream
	{
		get
		{
			if (start == size)
			{
				return Stream.Null;
			}
			if (stream == null)
			{
				stream = response.GetResponseStream();
			}
			return stream;
		}
	}

	public long FileSize => size;

	public long StartPoint => start;

	public bool IsProgressKnown => size > -1;

	public static DownloadData Create(string url, string destFolder)
	{
		return Create(url, destFolder, null);
	}

	public static DownloadData Create(string url, string destFolder, IWebProxy proxy)
	{
		DownloadData downloadData = new DownloadData();
		downloadData.proxy = proxy;
		long num = (downloadData.size = downloadData.GetFileSize(url));
		WebRequest request = downloadData.GetRequest(url);
		try
		{
			downloadData.response = request.GetResponse();
		}
		catch (Exception ex)
		{
			throw new ArgumentException($"Error downloading \"{url}\": {ex.Message}", ex);
		}
		ValidateResponse(downloadData.response, url);
		string fileName = Path.GetFileName(downloadData.response.ResponseUri.ToString());
		string text = Path.Combine(destFolder, fileName);
		if (!downloadData.IsProgressKnown && File.Exists(text))
		{
			File.Delete(text);
		}
		if (downloadData.IsProgressKnown && File.Exists(text))
		{
			if (!(downloadData.Response is HttpWebResponse))
			{
				File.Delete(text);
			}
			else
			{
				downloadData.start = new FileInfo(text).Length;
				if (downloadData.start > num)
				{
					File.Delete(text);
				}
				else if (downloadData.start < num)
				{
					downloadData.response.Close();
					request = downloadData.GetRequest(url);
					((HttpWebRequest)request).AddRange((int)downloadData.start);
					downloadData.response = request.GetResponse();
					if (((HttpWebResponse)downloadData.Response).StatusCode != HttpStatusCode.PartialContent)
					{
						File.Delete(text);
						downloadData.start = 0L;
					}
				}
			}
		}
		return downloadData;
	}

	private DownloadData()
	{
	}

	private DownloadData(WebResponse response, long size, long start)
	{
		this.response = response;
		this.size = size;
		this.start = start;
		stream = null;
	}

	private static void ValidateResponse(WebResponse response, string url)
	{
		if (response is HttpWebResponse)
		{
			HttpWebResponse httpWebResponse = (HttpWebResponse)response;
			if (httpWebResponse.ContentType != null)
			{
				_ = httpWebResponse.StatusCode;
				_ = 404;
			}
		}
		else if (response is FtpWebResponse && ((FtpWebResponse)response).StatusCode == FtpStatusCode.ConnectionClosed)
		{
			throw new ArgumentException($"Could not download \"{url}\" - FTP server closed the connection.");
		}
	}

	private long GetFileSize(string url)
	{
		WebResponse webResponse = null;
		long num = -1L;
		try
		{
			webResponse = GetRequest(url).GetResponse();
			return webResponse.ContentLength;
		}
		finally
		{
			webResponse?.Close();
		}
	}

	private bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
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

	private WebRequest GetRequest(string url)
	{
		ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;
		WebRequest webRequest = WebRequest.Create(url);
		if (webRequest is HttpWebRequest)
		{
			webRequest.Credentials = CredentialCache.DefaultCredentials;
			webRequest.Proxy.GetProxy(new Uri("http://www.google.com"));
		}
		if (webRequest is FtpWebRequest)
		{
			webRequest.Credentials = new NetworkCredential(SettingsManager.FTP_USERNAME, SettingsManager.FTP_PASSWORD);
		}
		if (proxy != null)
		{
			webRequest.Proxy = proxy;
		}
		return webRequest;
	}

	public void Close()
	{
		response.Close();
	}
}

using System;
using System.IO;
using System.Net;
using System.Net.FtpClient;
using MHLab.PATCH.Utilities;

namespace MHLab.PATCH.Uploader;

public class FileUploader
{
	private string hostname;

	private int port;

	private Protocol protocol;

	private string username;

	private string password;

	public FileUploader(Protocol protocol, string host, int port, string username, string password)
	{
		hostname = host;
		this.port = port;
		this.protocol = protocol;
		this.username = username;
		this.password = password;
	}

	public void UploadFile(string localFile, string remotePath)
	{
		try
		{
			using FtpClient ftpClient = new FtpClient();
			ftpClient.Host = hostname;
			ftpClient.Port = port;
			ftpClient.Credentials = new NetworkCredential(username, password);
			ftpClient.Connect();
			ftpClient.CreateDirectory(remotePath);
			using Stream stream = ftpClient.OpenWrite(Path.Combine(remotePath, Path.GetFileName(localFile)));
			try
			{
				using FileStream input = File.OpenRead(localFile);
				input.CopyTo(stream);
			}
			finally
			{
				stream.Close();
			}
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public bool CreateFTPDirectory(string directory)
	{
		try
		{
			FtpWebRequest obj = (FtpWebRequest)WebRequest.Create(new Uri(directory));
			obj.Method = "MKD";
			obj.Credentials = new NetworkCredential(username, password);
			obj.UsePassive = true;
			obj.UseBinary = true;
			obj.KeepAlive = false;
			FtpWebResponse obj2 = (FtpWebResponse)obj.GetResponse();
			obj2.GetResponseStream().Close();
			obj2.Close();
			return true;
		}
		catch (WebException ex)
		{
			FtpWebResponse ftpWebResponse = (FtpWebResponse)ex.Response;
			if (ftpWebResponse.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
			{
				ftpWebResponse.Close();
				return true;
			}
			ftpWebResponse.Close();
			return false;
		}
		catch
		{
			return false;
		}
	}
}

using System;
using System.Net;
using System.Security;
using Un4seen.Bass.AddOn.Tags;

namespace Un4seen.Bass.Misc;

[Serializable]
[SuppressUnmanagedCodeSecurity]
public abstract class StreamingServer : IStreamingServer, IDisposable
{
	public enum STREAMINGERROR
	{
		Ok = 0,
		Error_ResolvingServerAddress = 100,
		Error_CreatingConnection = 101,
		Error_SendingData = 102,
		Error_EncoderError = 103,
		Error_Login = 104,
		Error_Disconnect = 105,
		Error_NotConnected = 106,
		Warning_LessDataSend = 201,
		Unknown = -1
	}

	private bool disposed;

	private IBaseEncoder _encoder;

	private STREAMINGERROR _lastError;

	private string _lastErrorMsg = string.Empty;

	private string _songTitle = "radio42";

	private string _songUrl;

	private bool _useBASS = true;

	private bool _forceUTF8TitleUpdates;

	public bool UseBASS => _useBASS;

	public string SongTitle
	{
		get
		{
			return _songTitle;
		}
		set
		{
			if (value == null)
			{
				value = string.Empty;
			}
			_songTitle = value;
		}
	}

	public string SongUrl
	{
		get
		{
			return _songUrl;
		}
		set
		{
			_songUrl = value;
		}
	}

	public bool ForceUTF8TitleUpdates
	{
		get
		{
			return _forceUTF8TitleUpdates;
		}
		set
		{
			_forceUTF8TitleUpdates = value;
		}
	}

	public abstract bool IsConnected { get; }

	public IBaseEncoder Encoder => _encoder;

	public STREAMINGERROR LastError
	{
		get
		{
			return _lastError;
		}
		set
		{
			_lastError = value;
		}
	}

	public string LastErrorMessage
	{
		get
		{
			return _lastErrorMsg;
		}
		set
		{
			_lastErrorMsg = value;
		}
	}

	public StreamingServer(IBaseEncoder encoder)
	{
		_encoder = encoder;
		_useBASS = true;
		if (_encoder == null)
		{
			throw new ArgumentNullException("encoder", "No encoder specified!");
		}
		if (!_encoder.EncoderExists)
		{
			throw new ArgumentException("Encoder does NOT exist!");
		}
	}

	public StreamingServer(IBaseEncoder encoder, bool useBASS)
	{
		_encoder = encoder;
		_useBASS = useBASS;
		if (_encoder == null)
		{
			throw new ArgumentNullException("encoder", "No encoder specified!");
		}
		if (!_encoder.EncoderExists)
		{
			throw new ArgumentException("Encoder does NOT exist!");
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (!disposed)
		{
			try
			{
				Disconnect();
				if (Encoder != null && Encoder.IsActive)
				{
					Encoder.Stop();
				}
			}
			catch
			{
			}
		}
		disposed = true;
	}

	~StreamingServer()
	{
		Dispose(disposing: false);
	}

	protected static IPAddress[] GetIPfromHost(string hostname)
	{
		IPAddress address = null;
		if (IPAddress.TryParse(hostname, out address))
		{
			return new IPAddress[1] { address };
		}
		IPAddress[] result = null;
		try
		{
			IPHostEntry hostEntry = Dns.GetHostEntry(hostname);
			result = ((hostEntry == null || hostEntry.AddressList == null || hostEntry.AddressList.Length == 0) ? Dns.GetHostAddresses(hostname) : hostEntry.AddressList);
		}
		catch
		{
		}
		return result;
	}

	public abstract bool Connect();

	public abstract bool Disconnect();

	public abstract bool Login();

	public abstract int SendData(IntPtr buffer, int length);

	public abstract bool UpdateTitle(string song, string url);

	public virtual bool UpdateTitle(TAG_INFO tag, string url)
	{
		return UpdateTitle(tag.ToString(), url);
	}

	public virtual int GetListeners(string password)
	{
		return -1;
	}

	public virtual string GetStats(string password)
	{
		return null;
	}
}

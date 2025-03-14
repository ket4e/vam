using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using Un4seen.Bass.AddOn.Enc;

namespace Un4seen.Bass.Misc;

[Serializable]
[SuppressUnmanagedCodeSecurity]
public sealed class ICEcast : StreamingServer
{
	public string ServerAddress = "localhost";

	public int ServerPort = 8000;

	public string MountPoint = "/stream.ogg";

	public string Username = string.Empty;

	public string Password = "hackme";

	private string _adminUsername = string.Empty;

	private string _adminPassword = string.Empty;

	public string StreamName = "Your Station Name";

	public string StreamDescription = "Your Station Description";

	public string StreamUrl = "http://www.oddsock.org";

	public string StreamGenre = "Genre1 Genre2";

	public bool PublicFlag = true;

	public string Quality;

	private Socket _socket;

	private bool _isConnected;

	private ENCODENOTIFYPROC _myNotifyProc;

	private bool _loggedIn;

	private byte[] _data;

	private object _lock = false;

	private Encoding _encoding = Encoding.GetEncoding("latin1");

	private int _retrycount = 3;

	public string AdminUsername
	{
		get
		{
			if (string.IsNullOrEmpty(_adminUsername))
			{
				if (string.IsNullOrEmpty(Username))
				{
					return "admin";
				}
				return Username;
			}
			return _adminUsername;
		}
		set
		{
			_adminUsername = value;
		}
	}

	public string AdminPassword
	{
		get
		{
			if (string.IsNullOrEmpty(_adminPassword))
			{
				return Password;
			}
			return _adminPassword;
		}
		set
		{
			_adminPassword = value;
		}
	}

	public override bool IsConnected
	{
		get
		{
			if (base.UseBASS)
			{
				return _isConnected;
			}
			if (_socket != null)
			{
				if (_socket.Connected)
				{
					return _loggedIn;
				}
				return false;
			}
			return false;
		}
	}

	public ICEcast(IBaseEncoder encoder)
		: base(encoder)
	{
		if (encoder.EncoderType != BASSChannelType.BASS_CTYPE_STREAM_OGG && encoder.EncoderType != BASSChannelType.BASS_CTYPE_STREAM_MP3 && encoder.EncoderType != BASSChannelType.BASS_CTYPE_STREAM_AAC && encoder.EncoderType != BASSChannelType.BASS_CTYPE_STREAM_FLAC_OGG && encoder.EncoderType != BASSChannelType.BASS_CTYPE_STREAM_OPUS)
		{
			throw new ArgumentException("Invalid EncoderType (only OGG, MP3, AAC,FLAC_OGG and OPUS is supported)!");
		}
	}

	public ICEcast(IBaseEncoder encoder, bool useBASS)
		: base(encoder, useBASS)
	{
		if (encoder.EncoderType != BASSChannelType.BASS_CTYPE_STREAM_OGG && encoder.EncoderType != BASSChannelType.BASS_CTYPE_STREAM_MP3 && encoder.EncoderType != BASSChannelType.BASS_CTYPE_STREAM_AAC && encoder.EncoderType != BASSChannelType.BASS_CTYPE_STREAM_FLAC_OGG && encoder.EncoderType != BASSChannelType.BASS_CTYPE_STREAM_OPUS)
		{
			throw new ArgumentException("Invalid EncoderType (only OGG, MP3, AAC, FLAC_OGG and OPUS is supported)!");
		}
	}

	public override bool Connect()
	{
		if (!base.Encoder.IsActive)
		{
			base.LastError = STREAMINGERROR.Error_EncoderError;
			base.LastErrorMessage = "Encoder not active!";
			return false;
		}
		if (base.UseBASS)
		{
			if (_isConnected)
			{
				return true;
			}
			string content = BassEnc.BASS_ENCODE_TYPE_OGG;
			if (base.Encoder.EncoderType == BASSChannelType.BASS_CTYPE_STREAM_MP3)
			{
				content = BassEnc.BASS_ENCODE_TYPE_MP3;
			}
			else if (base.Encoder.EncoderType == BASSChannelType.BASS_CTYPE_STREAM_AAC)
			{
				content = BassEnc.BASS_ENCODE_TYPE_AAC;
			}
			string pass = Password;
			if (!string.IsNullOrEmpty(Username) && Username.ToLower() != "source")
			{
				pass = Username + ":" + Password;
			}
			if (BassEnc.BASS_Encode_CastInit(base.Encoder.EncoderHandle, $"{ServerAddress}:{ServerPort}{MountPoint}", pass, content, StreamName, StreamUrl, StreamGenre, StreamDescription, (Quality == null) ? null : $"ice-bitrate: {Quality}\r\n", (Quality == null) ? base.Encoder.EffectiveBitrate : 0, PublicFlag))
			{
				_myNotifyProc = EncoderNotifyProc;
				_isConnected = BassEnc.BASS_Encode_SetNotify(base.Encoder.EncoderHandle, _myNotifyProc, IntPtr.Zero);
			}
			else
			{
				base.LastError = STREAMINGERROR.Error_EncoderError;
				base.LastErrorMessage = "Encoder not active or a connction to the server could not be established!";
				switch (Bass.BASS_ErrorGetCode())
				{
				case BASSError.BASS_ERROR_HANDLE:
					base.LastError = STREAMINGERROR.Error_EncoderError;
					base.LastErrorMessage = "Encoder not active or invalif Encoder used!";
					break;
				case BASSError.BASS_ERROR_ALREADY:
					base.LastError = STREAMINGERROR.Error_NotConnected;
					base.LastErrorMessage = "There is already a cast set on the encoder!";
					break;
				case BASSError.BASS_ERROR_ILLPARAM:
					base.LastError = STREAMINGERROR.Error_ResolvingServerAddress;
					base.LastErrorMessage = "Couldn't connect to the server or server doesn't include a port number!";
					break;
				case BASSError.BASS_ERROR_FILEOPEN:
					base.LastError = STREAMINGERROR.Error_CreatingConnection;
					base.LastErrorMessage = "Couldn't connect to the server!";
					break;
				case BASSError.BASS_ERROR_CAST_DENIED:
					base.LastError = STREAMINGERROR.Error_Login;
					base.LastErrorMessage = "Username or Password incorrect!";
					break;
				case BASSError.BASS_ERROR_UNKNOWN:
					base.LastError = STREAMINGERROR.Error_CreatingConnection;
					base.LastErrorMessage = "An unknown error occurred!";
					break;
				}
				_isConnected = false;
			}
			return _isConnected;
		}
		if (_socket != null && _socket.Connected)
		{
			_socket.Close();
			_socket = null;
		}
		_socket = CreateSocket(ServerAddress, ServerPort);
		if (_socket != null)
		{
			return _socket.Connected;
		}
		return false;
	}

	public override bool Disconnect()
	{
		if (base.UseBASS)
		{
			if (base.Encoder.Stop())
			{
				_isConnected = false;
			}
			return !_isConnected;
		}
		bool result = false;
		try
		{
			_socket.Close();
			base.Encoder.Stop();
		}
		catch
		{
		}
		finally
		{
			if (_socket != null && _socket.Connected)
			{
				base.LastError = STREAMINGERROR.Error_Disconnect;
				base.LastErrorMessage = "Winsock error: " + Convert.ToString(Marshal.GetLastWin32Error());
			}
			else
			{
				result = true;
				_loggedIn = false;
				_socket = null;
			}
		}
		return result;
	}

	public override bool Login()
	{
		if (base.UseBASS)
		{
			return true;
		}
		if (_socket == null)
		{
			base.LastError = STREAMINGERROR.Error_NotConnected;
			base.LastErrorMessage = "Not connected to server.";
			return false;
		}
		bool result = false;
		if (ICEcastLogin())
		{
			result = true;
			_loggedIn = true;
			base.LastError = STREAMINGERROR.Ok;
			base.LastErrorMessage = string.Empty;
		}
		else
		{
			base.LastError = STREAMINGERROR.Error_Login;
			base.LastErrorMessage = "Invalid username or password. Server could not be initialized.";
		}
		return result;
	}

	public override int SendData(IntPtr buffer, int length)
	{
		if (buffer == IntPtr.Zero || length == 0 || base.UseBASS)
		{
			return 0;
		}
		int i = -1;
		_retrycount = 3;
		try
		{
			lock (_lock)
			{
				if (_data == null || _data.Length < length)
				{
					_data = new byte[length];
				}
				Marshal.Copy(buffer, _data, 0, length);
				for (i = _socket.Send(_data, 0, length, SocketFlags.None); i < length; i += _socket.Send(_data, i, length - i, SocketFlags.None))
				{
					if (_retrycount <= 0)
					{
						break;
					}
					_retrycount--;
				}
				if (i < 0)
				{
					base.LastError = STREAMINGERROR.Error_SendingData;
					base.LastErrorMessage = $"{length} bytes not send.";
					Disconnect();
				}
				else if (i != length)
				{
					base.LastError = STREAMINGERROR.Warning_LessDataSend;
					base.LastErrorMessage = $"{i} of {length} bytes send.";
					Disconnect();
				}
			}
		}
		catch (Exception ex)
		{
			base.LastError = STREAMINGERROR.Error_SendingData;
			base.LastErrorMessage = ex.Message;
			i = -1;
			Disconnect();
		}
		return i;
	}

	public override bool UpdateTitle(string song, string url)
	{
		base.SongTitle = song.Trim(default(char)).Replace('\0', ' ');
		if (!string.IsNullOrEmpty(url))
		{
			url = url.Trim(default(char)).Replace('\0', ' ');
		}
		if (string.IsNullOrEmpty(base.SongTitle))
		{
			return false;
		}
		if (base.UseBASS && IsConnected)
		{
			return BassEnc.BASS_Encode_CastSetTitle(title: ((!base.ForceUTF8TitleUpdates && base.Encoder.EncoderType != BASSChannelType.BASS_CTYPE_STREAM_OGG && base.Encoder.EncoderType != BASSChannelType.BASS_CTYPE_STREAM_FLAC_OGG && base.Encoder.EncoderType != BASSChannelType.BASS_CTYPE_STREAM_OPUS) ? Encoding.GetEncoding("latin1") : Encoding.UTF8).GetBytes(base.SongTitle + "\0"), handle: base.Encoder.EncoderHandle, url: null);
		}
		return ICEcastUpdateTitle(base.SongTitle);
	}

	public bool UpdateArtistTitle(string artist, string title)
	{
		base.SongTitle = $"{artist} - {title}";
		return ICEcastUpdateTitle(artist, title);
	}

	public override int GetListeners(string password)
	{
		int result = -1;
		try
		{
			if (password == null)
			{
				password = AdminUsername + ":" + AdminPassword;
			}
			else if (password.IndexOf(':') < 0)
			{
				password = AdminUsername + ":" + password;
			}
			string text = null;
			text = ((!base.UseBASS || !IsConnected) ? ICEcastGetStats(password, BASSEncodeStats.BASS_ENCODE_STATS_ICE) : BassEnc.BASS_Encode_CastGetStats(base.Encoder.EncoderHandle, BASSEncodeStats.BASS_ENCODE_STATS_ICE, password));
			if (text != null)
			{
				int num = text.ToUpper().IndexOf("<LISTENERS>");
				int num2 = text.ToUpper().IndexOf("</LISTENERS>");
				if (num > 0 && num2 > 0)
				{
					num += 11;
					result = int.Parse(text.Substring(num, num2 - num));
				}
			}
		}
		catch
		{
			result = -1;
		}
		return result;
	}

	public override string GetStats(string password)
	{
		string text = null;
		try
		{
			if (password == null)
			{
				password = AdminUsername + ":" + AdminPassword;
			}
			else if (password.IndexOf(':') < 0)
			{
				password = AdminUsername + ":" + password;
			}
			if (base.UseBASS && IsConnected)
			{
				return BassEnc.BASS_Encode_CastGetStats(base.Encoder.EncoderHandle, BASSEncodeStats.BASS_ENCODE_STATS_ICESERV, password);
			}
			return ICEcastGetStats(password, BASSEncodeStats.BASS_ENCODE_STATS_ICESERV);
		}
		catch
		{
			return null;
		}
	}

	private void EncoderNotifyProc(int handle, BASSEncodeNotify status, IntPtr user)
	{
		if (status == BASSEncodeNotify.BASS_ENCODE_NOTIFY_CAST_TIMEOUT)
		{
			Disconnect();
			base.LastError = STREAMINGERROR.Error_SendingData;
			base.LastErrorMessage = "Data sending timeout!";
		}
		switch (status)
		{
		case BASSEncodeNotify.BASS_ENCODE_NOTIFY_ENCODER:
			_isConnected = false;
			base.LastError = STREAMINGERROR.Error_EncoderError;
			base.LastErrorMessage = "Encoder died!";
			break;
		case BASSEncodeNotify.BASS_ENCODE_NOTIFY_CAST:
			_isConnected = false;
			base.LastError = STREAMINGERROR.Error_SendingData;
			base.LastErrorMessage = "Connection to the server died!";
			break;
		}
	}

	private Socket CreateSocket(string serveraddress, int port)
	{
		Socket socket = null;
		try
		{
			if (serveraddress.StartsWith("http://"))
			{
				serveraddress.Substring(7);
			}
			IPAddress[] iPfromHost = StreamingServer.GetIPfromHost(serveraddress);
			if (iPfromHost != null)
			{
				IPAddress[] array = iPfromHost;
				foreach (IPAddress address in array)
				{
					try
					{
						IPEndPoint iPEndPoint = new IPEndPoint(address, port);
						Socket socket2 = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
						socket2.Connect(iPEndPoint);
						if (socket2.Connected)
						{
							socket = socket2;
							socket.SendTimeout = Bass.BASS_GetConfig(BASSConfig.BASS_CONFIG_ENCODE_CAST_TIMEOUT);
							break;
						}
					}
					catch (Exception ex)
					{
						base.LastError = STREAMINGERROR.Error_CreatingConnection;
						base.LastErrorMessage = ex.Message;
						socket = null;
					}
				}
			}
		}
		catch (Exception ex2)
		{
			base.LastError = STREAMINGERROR.Error_ResolvingServerAddress;
			base.LastErrorMessage = ex2.Message;
			socket = null;
		}
		return socket;
	}

	private bool ICEcastLogin()
	{
		if (_socket == null)
		{
			return false;
		}
		string text = BassEnc.BASS_ENCODE_TYPE_OGG;
		if (base.Encoder.EncoderType == BASSChannelType.BASS_CTYPE_STREAM_MP3)
		{
			text = BassEnc.BASS_ENCODE_TYPE_MP3;
		}
		else if (base.Encoder.EncoderType == BASSChannelType.BASS_CTYPE_STREAM_AAC)
		{
			text = BassEnc.BASS_ENCODE_TYPE_AAC;
		}
		string text2 = Username;
		if (string.IsNullOrEmpty(text2))
		{
			text2 = "source";
		}
		string request = string.Format("SOURCE {0} ICE/1.0\r\ncontent-type: {1}\r\nAuthorization: Basic {2}\r\nice-name: {3}\r\nice-url: {4}\r\nice-genre: {5}\r\nice-bitrate: {6}\r\nice-private: {7}\r\nice-public: {8}\r\nice-description: {9}\r\nice-audio-info: ice-samplerate={10};ice-bitrate={11};ice-channels={12}\r\n\r\n", MountPoint, text, Base64String(text2 + ":" + Password), StreamName, StreamUrl, StreamGenre, (Quality == null) ? base.Encoder.EffectiveBitrate.ToString() : Quality, PublicFlag ? "0" : "1", PublicFlag ? "1" : "0", StreamDescription, base.Encoder.ChannelSampleRate, base.Encoder.EffectiveBitrate, base.Encoder.ChannelNumChans);
		string text3 = SendAndReceive(_socket, request);
		if (text3 != null && text3.IndexOf("200 OK") >= 0)
		{
			return true;
		}
		return false;
	}

	private bool SendCommand(Socket socket, string command)
	{
		return SendCommand(socket, command, _encoding);
	}

	private bool SendCommand(Socket socket, string command, Encoding encoding)
	{
		if (socket == null || command == null)
		{
			return false;
		}
		if (!command.EndsWith("\r\n"))
		{
			command += "\r\n";
		}
		int num = 0;
		try
		{
			byte[] bytes = encoding.GetBytes(command);
			int num2 = 0;
			int num3 = bytes.Length;
			while (num < num3)
			{
				num = socket.Send(bytes, num2, num3 - num2, SocketFlags.None);
				num2 += num;
			}
		}
		catch
		{
			return false;
		}
		return num > 0;
	}

	private string SendAndReceive(Socket socket, string request)
	{
		return SendAndReceive(socket, request, _encoding);
	}

	private string SendAndReceive(Socket socket, string request, Encoding encoding)
	{
		if (socket == null || request == null)
		{
			return null;
		}
		if (!request.EndsWith("\r\n"))
		{
			request += "\r\n";
		}
		string text = null;
		try
		{
			byte[] bytes = _encoding.GetBytes(request);
			int num = 0;
			int num2 = 0;
			int num3 = bytes.Length;
			while (num < num3)
			{
				num = socket.Send(bytes, num2, num3 - num2, SocketFlags.None);
				num2 += num;
			}
			text = string.Empty;
			DateTime now = DateTime.Now;
			do
			{
				byte[] array = new byte[socket.Available];
				if (socket.Receive(array, 0, array.Length, SocketFlags.None) > 0)
				{
					text = _encoding.GetString(array);
					break;
				}
				Thread.Sleep(100);
			}
			while (!(DateTime.Now - now > TimeSpan.FromSeconds(5.0)));
		}
		catch
		{
			return null;
		}
		return text;
	}

	private bool ICEcastUpdateTitle(string song)
	{
		bool result = false;
		Socket socket = null;
		try
		{
			string command = string.Format("GET /admin/metadata?mount={0}&mode=updinfo&password={1}&song={2} HTTP/1.0\r\nUser-Agent: {3} (Mozilla Compatible)\r\nAuthorization: Basic {4}\r\nHost: {5}\r\n\r\n", Uri.EscapeDataString(MountPoint), AdminPassword, Uri.EscapeDataString(song), Bass.BASS_GetConfigString(BASSConfig.BASS_CONFIG_NET_AGENT), Base64String(AdminUsername + ":" + AdminPassword), ServerAddress);
			socket = CreateSocket(ServerAddress, ServerPort);
			if (socket != null)
			{
				Encoding encoding = ((!base.ForceUTF8TitleUpdates && base.Encoder.EncoderType != BASSChannelType.BASS_CTYPE_STREAM_OGG && base.Encoder.EncoderType != BASSChannelType.BASS_CTYPE_STREAM_FLAC_OGG && base.Encoder.EncoderType != BASSChannelType.BASS_CTYPE_STREAM_OPUS) ? Encoding.GetEncoding("latin1") : Encoding.UTF8);
				if (SendCommand(socket, command, encoding))
				{
					result = true;
				}
			}
		}
		catch
		{
		}
		finally
		{
			if (socket != null)
			{
				socket.Close();
				socket = null;
			}
		}
		return result;
	}

	private bool ICEcastUpdateTitle(string artist, string title)
	{
		bool result = false;
		Socket socket = null;
		try
		{
			string command = string.Format("GET /admin/metadata?mount={0}&mode=updinfo&password={1}&artist={2}&title={3} HTTP/1.0\r\nUser-Agent: {4} (Mozilla Compatible)\r\nAuthorization: Basic {5}\r\nHost: {6}\r\n\r\n", Uri.EscapeDataString(MountPoint), AdminPassword, Uri.EscapeDataString(artist), Uri.EscapeDataString(title), Bass.BASS_GetConfigString(BASSConfig.BASS_CONFIG_NET_AGENT), Base64String(AdminUsername + ":" + AdminPassword), ServerAddress);
			socket = CreateSocket(ServerAddress, ServerPort);
			if (socket != null)
			{
				Encoding encoding = ((!base.ForceUTF8TitleUpdates && base.Encoder.EncoderType != BASSChannelType.BASS_CTYPE_STREAM_OGG && base.Encoder.EncoderType != BASSChannelType.BASS_CTYPE_STREAM_FLAC_OGG && base.Encoder.EncoderType != BASSChannelType.BASS_CTYPE_STREAM_OPUS) ? Encoding.GetEncoding("latin1") : Encoding.UTF8);
				if (SendCommand(socket, command, encoding))
				{
					result = true;
				}
			}
		}
		catch
		{
		}
		finally
		{
			if (socket != null)
			{
				socket.Close();
				socket = null;
			}
		}
		return result;
	}

	private string ICEcastGetStats(string password, BASSEncodeStats type)
	{
		string text = null;
		Socket socket = null;
		if (password == null)
		{
			password = AdminUsername + ":" + AdminPassword;
		}
		else if (password.IndexOf(':') < 0)
		{
			password = AdminUsername + ":" + password;
		}
		try
		{
			string text2 = "";
			text2 = ((type != BASSEncodeStats.BASS_ENCODE_STATS_ICE) ? $"GET /admin/stats HTTP/1.0\r\nUser-Agent: {Bass.BASS_GetConfigString(BASSConfig.BASS_CONFIG_NET_AGENT)}\r\nAuthorization: Basic {Base64String(password)}\r\n\r\n" : $"GET /admin/listclients?mount={Uri.EscapeDataString(MountPoint)} HTTP/1.0\r\nUser-Agent: {Bass.BASS_GetConfigString(BASSConfig.BASS_CONFIG_NET_AGENT)}\r\nAuthorization: Basic {Base64String(password)}\r\n\r\n");
			socket = CreateSocket(ServerAddress, ServerPort);
			if (socket != null)
			{
				text = SendAndReceive(socket, text2, Encoding.UTF8);
				if (text != null)
				{
					int num = text.ToUpper().IndexOf("<?XML");
					if (num >= 0)
					{
						text = text.Substring(num);
					}
				}
			}
		}
		catch
		{
		}
		finally
		{
			if (socket != null)
			{
				socket.Close();
				socket = null;
			}
		}
		return text;
	}

	private string Base64String(string s)
	{
		return Convert.ToBase64String(_encoding.GetBytes(s));
	}
}

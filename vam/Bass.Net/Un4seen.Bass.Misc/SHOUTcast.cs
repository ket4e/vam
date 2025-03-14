using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Un4seen.Bass.AddOn.Enc;
using Un4seen.Bass.AddOn.Tags;

namespace Un4seen.Bass.Misc;

[Serializable]
[SuppressUnmanagedCodeSecurity]
public sealed class SHOUTcast : StreamingServer
{
	public string ServerAddress = "localhost";

	public int ServerPort = 8000;

	public string SID = string.Empty;

	public bool v2SendSongTitleOnly;

	public bool v2SendArtwork;

	public string v2StreamArtwork = string.Empty;

	public string v2StationArtwork = string.Empty;

	public string v2SongTitleNext = string.Empty;

	public string Username = string.Empty;

	public string Password = "changeme";

	private string _adminUsername = string.Empty;

	private string _adminPassword = string.Empty;

	public string StationName = "Your Station Name";

	public string Genre = "Genre1 Genre2";

	public bool PublicFlag = true;

	public string Url = "http://www.shoutcast.com";

	public string Irc = "N/A";

	public string Icq = "N/A";

	public string Aim = "N/A";

	private Socket _socket;

	private bool _isConnected;

	private ENCODENOTIFYPROC _myNotifyProc;

	private bool _loggedIn;

	private byte[] _data;

	private object _lock = false;

	private Encoding _encoding = Encoding.GetEncoding(1252);

	private int _retrycount = 3;

	public bool UseSHOUTcastv2
	{
		get
		{
			if (base.UseBASS)
			{
				return !string.IsNullOrEmpty(SID);
			}
			return false;
		}
		set
		{
			if (value)
			{
				if (base.UseBASS && string.IsNullOrEmpty(SID))
				{
					SID = "1";
				}
			}
			else
			{
				SID = string.Empty;
			}
		}
	}

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

	public SHOUTcast(IBaseEncoder encoder)
		: base(encoder)
	{
		if (encoder.EncoderType != BASSChannelType.BASS_CTYPE_STREAM_MP3 && encoder.EncoderType != BASSChannelType.BASS_CTYPE_STREAM_AAC && encoder.EncoderType != BASSChannelType.BASS_CTYPE_STREAM_WAV)
		{
			throw new ArgumentException("Invalid EncoderType (only MP3 and AAC is supported)!");
		}
	}

	public SHOUTcast(IBaseEncoder encoder, bool useBASS)
		: base(encoder, useBASS)
	{
		if (encoder.EncoderType != BASSChannelType.BASS_CTYPE_STREAM_MP3 && encoder.EncoderType != BASSChannelType.BASS_CTYPE_STREAM_AAC && encoder.EncoderType != BASSChannelType.BASS_CTYPE_STREAM_WAV)
		{
			throw new ArgumentException("Invalid EncoderType (only MP3 and AAC is supported)!");
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
			string content = BassEnc.BASS_ENCODE_TYPE_MP3;
			if (base.Encoder.EncoderType == BASSChannelType.BASS_CTYPE_STREAM_AAC)
			{
				content = BassEnc.BASS_ENCODE_TYPE_AAC;
			}
			string pass = Password;
			if (!string.IsNullOrEmpty(Username) && Username.ToLower() != "source")
			{
				pass = Username + ":" + Password;
			}
			string text = $"{ServerAddress}:{ServerPort}";
			if (UseSHOUTcastv2)
			{
				text = text + "," + SID;
			}
			if (BassEnc.BASS_Encode_CastInit(base.Encoder.EncoderHandle, text, pass, content, StationName, Url, Genre, null, $"icy-irc:{Irc}\r\nicy-icq:{Icq}\r\nicy-aim:{Aim}\r\n", base.Encoder.EffectiveBitrate, PublicFlag))
			{
				_myNotifyProc = EncoderNotifyProc;
				_isConnected = BassEnc.BASS_Encode_SetNotify(base.Encoder.EncoderHandle, _myNotifyProc, IntPtr.Zero);
				SHOUTcastv2UpdateStationArtwork();
			}
			else
			{
				base.LastError = STREAMINGERROR.Error_EncoderError;
				base.LastErrorMessage = "Encoder not active or a connction to the server could not be established!";
				switch (Bass.BASS_ErrorGetCode())
				{
				case BASSError.BASS_ERROR_HANDLE:
					base.LastError = STREAMINGERROR.Error_EncoderError;
					base.LastErrorMessage = "Encoder not active or invalid Encoder used!";
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
		_socket = CreateSocket(ServerAddress, ServerPort + 1);
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
		if (SHOUTcastLogin())
		{
			if (SHOUTcastInit())
			{
				result = true;
				_loggedIn = true;
				base.LastError = STREAMINGERROR.Ok;
				base.LastErrorMessage = string.Empty;
			}
			else
			{
				base.LastError = STREAMINGERROR.Error_Login;
				base.LastErrorMessage = "Server could not be initialized.";
			}
		}
		else
		{
			base.LastError = STREAMINGERROR.Error_Login;
			base.LastErrorMessage = "Invalid username or password.";
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
		if (song == null)
		{
			base.SongTitle = string.Empty;
		}
		else
		{
			base.SongTitle = song.Trim(default(char)).Replace('\0', ' ');
		}
		if (!string.IsNullOrEmpty(url))
		{
			url = url.Trim(default(char)).Replace('\0', ' ');
		}
		if (string.IsNullOrEmpty(base.SongTitle))
		{
			v2SongTitleNext = string.Empty;
		}
		if (base.UseBASS && IsConnected)
		{
			if (UseSHOUTcastv2)
			{
				try
				{
					StringWriterWithEncoding stringWriterWithEncoding = new StringWriterWithEncoding(Encoding.UTF8);
					XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
					xmlWriterSettings.Encoding = stringWriterWithEncoding.Encoding;
					xmlWriterSettings.IndentChars = string.Empty;
					xmlWriterSettings.Indent = false;
					xmlWriterSettings.NewLineHandling = NewLineHandling.None;
					xmlWriterSettings.NewLineChars = string.Empty;
					xmlWriterSettings.CheckCharacters = false;
					using (XmlWriter xmlWriter = XmlWriter.Create(stringWriterWithEncoding, xmlWriterSettings))
					{
						xmlWriter.WriteStartElement("metadata");
						xmlWriter.WriteElementString("TIT2", base.SongTitle);
						if (!string.IsNullOrEmpty(StationName))
						{
							xmlWriter.WriteElementString("TRSN", StationName);
						}
						xmlWriter.WriteElementString("TENC", BassNet.InternalName + " (Broadcast Framework)");
						if (!string.IsNullOrEmpty(url))
						{
							xmlWriter.WriteElementString("WOAF", url);
						}
						if (!string.IsNullOrEmpty(Url))
						{
							xmlWriter.WriteElementString("WORS", Url);
						}
						if (v2SongTitleNext != null)
						{
							xmlWriter.WriteStartElement("extension");
							xmlWriter.WriteStartElement("title");
							xmlWriter.WriteAttributeString("seq", "1");
							xmlWriter.WriteString(base.SongTitle);
							xmlWriter.WriteEndElement();
							xmlWriter.WriteStartElement("title");
							xmlWriter.WriteAttributeString("seq", "2");
							xmlWriter.WriteString(v2SongTitleNext);
							xmlWriter.WriteEndElement();
							xmlWriter.WriteElementString("soon", v2SongTitleNext);
							xmlWriter.WriteEndElement();
						}
						xmlWriter.WriteEndElement();
						xmlWriter.Flush();
					}
					v2SongTitleNext = string.Empty;
					return BassEnc.BASS_Encode_CastSendMeta(base.Encoder.EncoderHandle, BASSEncodeMetaDataType.BASS_METADATA_XML_SHOUTCAST, stringWriterWithEncoding.ToString());
				}
				catch
				{
					return false;
				}
			}
			if (base.ForceUTF8TitleUpdates)
			{
				v2SongTitleNext = string.Empty;
				bool flag = BassEnc.BASS_Encode_CastSetTitle(base.Encoder.EncoderHandle, Encoding.UTF8.GetBytes(base.SongTitle + "\0"), string.IsNullOrEmpty(url) ? null : Encoding.UTF8.GetBytes(url + "\0"));
				if (!flag)
				{
					flag = SHOUTcastUpdateTitle(base.SongTitle, url);
				}
				return flag;
			}
			v2SongTitleNext = string.Empty;
			bool flag2 = BassEnc.BASS_Encode_CastSetTitle(base.Encoder.EncoderHandle, _encoding.GetBytes(base.SongTitle + "\0"), string.IsNullOrEmpty(url) ? null : _encoding.GetBytes(url + "\0"));
			if (!flag2)
			{
				flag2 = SHOUTcastUpdateTitle(base.SongTitle, url);
			}
			return flag2;
		}
		return SHOUTcastUpdateTitle(base.SongTitle, url);
	}

	public override bool UpdateTitle(TAG_INFO tag, string url)
	{
		if (tag == null)
		{
			v2SongTitleNext = string.Empty;
			return false;
		}
		if (!string.IsNullOrEmpty(url))
		{
			url = url.Trim(default(char)).Replace('\0', ' ');
		}
		if (base.UseBASS && IsConnected)
		{
			if (UseSHOUTcastv2)
			{
				try
				{
					StringWriterWithEncoding stringWriterWithEncoding = new StringWriterWithEncoding(Encoding.UTF8);
					XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
					xmlWriterSettings.Encoding = stringWriterWithEncoding.Encoding;
					xmlWriterSettings.IndentChars = string.Empty;
					xmlWriterSettings.Indent = false;
					xmlWriterSettings.NewLineHandling = NewLineHandling.None;
					xmlWriterSettings.NewLineChars = string.Empty;
					xmlWriterSettings.CheckCharacters = false;
					using (XmlWriter xmlWriter = XmlWriter.Create(stringWriterWithEncoding, xmlWriterSettings))
					{
						xmlWriter.WriteStartElement("metadata");
						if (v2SendSongTitleOnly)
						{
							if (string.IsNullOrEmpty(base.SongTitle))
							{
								xmlWriter.WriteElementString("TIT2", tag.ToString());
							}
							else
							{
								xmlWriter.WriteElementString("TIT2", base.SongTitle);
							}
						}
						else
						{
							if (!string.IsNullOrEmpty(tag.artist))
							{
								xmlWriter.WriteElementString("TIT2", tag.title);
								xmlWriter.WriteElementString("TPE1", tag.artist);
							}
							else
							{
								xmlWriter.WriteElementString("TIT2", tag.ToString());
							}
							if (!string.IsNullOrEmpty(tag.album))
							{
								xmlWriter.WriteElementString("TALB", tag.album);
							}
							if (!string.IsNullOrEmpty(tag.albumartist))
							{
								xmlWriter.WriteElementString("TPE2", tag.albumartist);
							}
							if (!string.IsNullOrEmpty(tag.genre))
							{
								xmlWriter.WriteElementString("TCON", tag.genre);
							}
							if (!string.IsNullOrEmpty(tag.year))
							{
								xmlWriter.WriteElementString("TYER", tag.year);
							}
							if (!string.IsNullOrEmpty(tag.copyright))
							{
								xmlWriter.WriteElementString("TCOP", tag.copyright);
							}
							if (!string.IsNullOrEmpty(tag.publisher))
							{
								xmlWriter.WriteElementString("TPUB", tag.publisher);
							}
							if (!string.IsNullOrEmpty(tag.composer))
							{
								xmlWriter.WriteElementString("TCOM", tag.composer);
							}
							if (!string.IsNullOrEmpty(tag.conductor))
							{
								xmlWriter.WriteElementString("TPE3", tag.conductor);
							}
							if (!string.IsNullOrEmpty(tag.remixer))
							{
								xmlWriter.WriteElementString("TPE4", tag.remixer);
							}
							if (!string.IsNullOrEmpty(tag.lyricist))
							{
								xmlWriter.WriteElementString("TEXT", tag.lyricist);
							}
							if (!string.IsNullOrEmpty(tag.isrc))
							{
								xmlWriter.WriteElementString("TSRC", tag.isrc);
							}
							if (!string.IsNullOrEmpty(tag.producer))
							{
								xmlWriter.WriteStartElement("IPLS");
								xmlWriter.WriteAttributeString("role", "producer");
								xmlWriter.WriteString(tag.producer);
								xmlWriter.WriteEndElement();
							}
							if (!string.IsNullOrEmpty(tag.grouping))
							{
								xmlWriter.WriteElementString("TIT1", tag.grouping);
							}
							if (!string.IsNullOrEmpty(StationName))
							{
								xmlWriter.WriteElementString("TRSN", StationName);
							}
							xmlWriter.WriteElementString("TENC", BassNet.InternalName + " (Broadcast Framework)");
							if (!string.IsNullOrEmpty(url))
							{
								xmlWriter.WriteElementString("WOAF", url);
							}
							if (!string.IsNullOrEmpty(Url))
							{
								xmlWriter.WriteElementString("WORS", Url);
							}
						}
						if (v2SongTitleNext != null)
						{
							xmlWriter.WriteStartElement("extension");
							xmlWriter.WriteStartElement("title");
							xmlWriter.WriteAttributeString("seq", "1");
							if (string.IsNullOrEmpty(base.SongTitle))
							{
								xmlWriter.WriteString(tag.ToString());
							}
							else
							{
								xmlWriter.WriteString(base.SongTitle);
							}
							xmlWriter.WriteEndElement();
							xmlWriter.WriteStartElement("title");
							xmlWriter.WriteAttributeString("seq", "2");
							xmlWriter.WriteString(v2SongTitleNext);
							xmlWriter.WriteEndElement();
							xmlWriter.WriteElementString("soon", v2SongTitleNext);
							xmlWriter.WriteEndElement();
						}
						xmlWriter.WriteEndElement();
						xmlWriter.Flush();
					}
					bool result = BassEnc.BASS_Encode_CastSendMeta(base.Encoder.EncoderHandle, BASSEncodeMetaDataType.BASS_METADATA_XML_SHOUTCAST, stringWriterWithEncoding.ToString());
					if (v2SendArtwork)
					{
						((MethodInvoker)delegate
						{
							try
							{
								BASSEncodeMetaDataType bASSEncodeMetaDataType = (BASSEncodeMetaDataType)0;
								TagPicture tagPicture = null;
								byte[] array = null;
								if (tag.PictureCount > 0)
								{
									tagPicture = tag.PictureGet(0);
								}
								if (tagPicture == null && !string.IsNullOrEmpty(v2StreamArtwork))
								{
									tagPicture = new TagPicture(v2StreamArtwork, TagPicture.PICTURE_TYPE.Location, StationName);
								}
								if (tagPicture != null)
								{
									tagPicture = new TagPicture(tagPicture, 300);
									if (tagPicture.PictureStorage == TagPicture.PICTURE_STORAGE.Internal)
									{
										if (tagPicture.Data.Length <= 523680)
										{
											array = tagPicture.Data;
										}
									}
									else
									{
										try
										{
											using Stream stream = new FileStream(Encoding.UTF8.GetString(tagPicture.Data), FileMode.Open, FileAccess.Read);
											if (stream.Length <= 523680)
											{
												byte[] array2 = new byte[stream.Length];
												stream.Read(array2, 0, (int)stream.Length);
												array = array2;
											}
											stream.Close();
										}
										catch
										{
										}
									}
									switch (tagPicture.MIMEType)
									{
									case "image/jpeg":
										bASSEncodeMetaDataType = BASSEncodeMetaDataType.BASS_METADATA_BIN_ALBUMART_JPG;
										break;
									case "image/bmp":
										bASSEncodeMetaDataType = BASSEncodeMetaDataType.BASS_METADATA_BIN_ALBUMART_BMP;
										break;
									case "image/png":
										bASSEncodeMetaDataType = BASSEncodeMetaDataType.BASS_METADATA_BIN_ALBUMART_PNG;
										break;
									case "image/gif":
										bASSEncodeMetaDataType = BASSEncodeMetaDataType.BASS_METADATA_BIN_ALBUMART_GIF;
										break;
									}
								}
								if (bASSEncodeMetaDataType > (BASSEncodeMetaDataType)0 && array != null)
								{
									BassEnc.BASS_Encode_CastSendMeta(base.Encoder.EncoderHandle, bASSEncodeMetaDataType, array);
								}
							}
							catch
							{
							}
						}).BeginInvoke(null, null);
					}
					v2SongTitleNext = string.Empty;
					return result;
				}
				catch
				{
					return false;
				}
			}
			if (base.ForceUTF8TitleUpdates)
			{
				string text = base.SongTitle;
				if (string.IsNullOrEmpty(text))
				{
					text = tag.ToString();
				}
				v2SongTitleNext = string.Empty;
				bool flag = BassEnc.BASS_Encode_CastSetTitle(base.Encoder.EncoderHandle, Encoding.UTF8.GetBytes(text + "\0"), string.IsNullOrEmpty(url) ? null : Encoding.UTF8.GetBytes(url + "\0"));
				if (!flag)
				{
					flag = SHOUTcastUpdateTitle(text, url);
				}
				return flag;
			}
			string text2 = base.SongTitle;
			if (string.IsNullOrEmpty(text2))
			{
				text2 = tag.ToString();
			}
			v2SongTitleNext = string.Empty;
			bool flag2 = BassEnc.BASS_Encode_CastSetTitle(base.Encoder.EncoderHandle, _encoding.GetBytes(text2 + "\0"), string.IsNullOrEmpty(url) ? null : _encoding.GetBytes(url + "\0"));
			if (!flag2)
			{
				flag2 = SHOUTcastUpdateTitle(text2, url);
			}
			return flag2;
		}
		string text3 = base.SongTitle;
		if (string.IsNullOrEmpty(text3))
		{
			text3 = tag.ToString();
		}
		return SHOUTcastUpdateTitle(text3, url);
	}

	public void UpdateStationArtwork(string stationArtwork)
	{
		v2StationArtwork = stationArtwork;
		SHOUTcastv2UpdateStationArtwork();
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
			text = ((!base.UseBASS || !IsConnected) ? SHOUTcastGetStats(password) : BassEnc.BASS_Encode_CastGetStats(base.Encoder.EncoderHandle, BASSEncodeStats.BASS_ENCODE_STATS_SHOUT, password));
			if (text != null)
			{
				int num = text.ToUpper().IndexOf("<CURRENTLISTENERS>");
				int num2 = text.ToUpper().IndexOf("</CURRENTLISTENERS>");
				if (num > 0 && num2 > 0)
				{
					num += 18;
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
				return BassEnc.BASS_Encode_CastGetStats(base.Encoder.EncoderHandle, BASSEncodeStats.BASS_ENCODE_STATS_SHOUT, password);
			}
			return SHOUTcastGetStats(password);
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

	private bool SHOUTcastLogin()
	{
		if (_socket == null)
		{
			return false;
		}
		string text = SendAndReceive(_socket, Password);
		if (text != null && text.IndexOf("OK2") >= 0)
		{
			return true;
		}
		return false;
	}

	private bool SHOUTcastInit()
	{
		if (_socket == null)
		{
			return false;
		}
		string text = "audio/mpeg";
		if (base.Encoder.EncoderType == BASSChannelType.BASS_CTYPE_STREAM_AAC)
		{
			text = "audio/aacp";
		}
		string command = string.Format("icy-name:{0}\r\nicy-genre:{1}\r\nicy-pub:{2}\r\nicy-br:{3}\r\nicy-url:{4}\r\nicy-irc:{5}\r\nicy-icq:{6}\r\nicy-aim:{7}\r\ncontent-type: {8}\r\nicy-reset: 1\r\n\r\n", StationName, Genre, PublicFlag ? "1" : "0", base.Encoder.EffectiveBitrate, Url, Irc, Icq, Aim, text);
		return SendCommand(_socket, command);
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
			byte[] bytes = encoding.GetBytes(request);
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
					text = encoding.GetString(array);
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

	private bool SHOUTcastUpdateTitle(string song, string url)
	{
		bool result = false;
		Socket socket = null;
		try
		{
			string text = "GET /admin.cgi?";
			if (UseSHOUTcastv2)
			{
				text += $"sid={Uri.EscapeDataString(SID)}&";
			}
			text = ((!string.IsNullOrEmpty(url)) ? (text + string.Format("pass={0}&mode=updinfo&song={1}&Url={2} HTTP/1.0\r\nUser-Agent: {3} (Mozilla Compatible)\r\n\r\n", Uri.EscapeDataString(AdminPassword), Uri.EscapeDataString(song), (url == null) ? "" : Uri.EscapeUriString(url), Bass.BASS_GetConfigString(BASSConfig.BASS_CONFIG_NET_AGENT))) : (text + $"pass={Uri.EscapeDataString(AdminPassword)}&mode=updinfo&song={Uri.EscapeDataString(song)} HTTP/1.0\r\nUser-Agent: {Bass.BASS_GetConfigString(BASSConfig.BASS_CONFIG_NET_AGENT)} (Mozilla Compatible)\r\n\r\n"));
			socket = CreateSocket(ServerAddress, ServerPort);
			if (socket != null)
			{
				Encoding encoding = _encoding;
				if (base.ForceUTF8TitleUpdates || UseSHOUTcastv2)
				{
					encoding = Encoding.UTF8;
				}
				if (SendCommand(socket, text, encoding))
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
		v2SongTitleNext = string.Empty;
		return result;
	}

	private string SHOUTcastGetStats(string password)
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
			string text2 = "GET /admin.cgi?";
			if (UseSHOUTcastv2)
			{
				text2 += $"sid={Uri.EscapeDataString(SID)}&";
			}
			text2 += $"mode=viewxml HTTP/1.0\r\nUser-Agent: {Bass.BASS_GetConfigString(BASSConfig.BASS_CONFIG_NET_AGENT)} (Mozilla Compatible)\r\nAuthorization: Basic {Base64String(password)}\r\n\r\n";
			socket = CreateSocket(ServerAddress, ServerPort);
			if (socket != null)
			{
				Encoding encoding = _encoding;
				if (base.ForceUTF8TitleUpdates)
				{
					encoding = Encoding.UTF8;
				}
				text = SendAndReceive(socket, text2, encoding);
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

	private void SHOUTcastv2UpdateStationArtwork()
	{
		if (!IsConnected || !UseSHOUTcastv2 || !v2SendArtwork || string.IsNullOrEmpty(v2StationArtwork))
		{
			return;
		}
		((MethodInvoker)delegate
		{
			try
			{
				BASSEncodeMetaDataType bASSEncodeMetaDataType = (BASSEncodeMetaDataType)0;
				byte[] array = null;
				TagPicture tagPicture = new TagPicture(v2StationArtwork, TagPicture.PICTURE_TYPE.Location, StationName);
				if (tagPicture != null)
				{
					tagPicture = new TagPicture(tagPicture, 300);
					if (tagPicture.PictureStorage == TagPicture.PICTURE_STORAGE.Internal)
					{
						if (tagPicture.Data.Length <= 523680)
						{
							array = tagPicture.Data;
						}
					}
					else
					{
						try
						{
							using Stream stream = new FileStream(Encoding.UTF8.GetString(tagPicture.Data), FileMode.Open, FileAccess.Read);
							if (stream.Length <= 523680)
							{
								byte[] array2 = new byte[stream.Length];
								stream.Read(array2, 0, (int)stream.Length);
								array = array2;
							}
							stream.Close();
						}
						catch
						{
						}
					}
					switch (tagPicture.MIMEType)
					{
					case "image/jpeg":
						bASSEncodeMetaDataType = BASSEncodeMetaDataType.BASS_METADATA_BIN_STATIONLOGO_JPG;
						break;
					case "image/bmp":
						bASSEncodeMetaDataType = BASSEncodeMetaDataType.BASS_METADATA_BIN_STATIONLOGO_BMP;
						break;
					case "image/png":
						bASSEncodeMetaDataType = BASSEncodeMetaDataType.BASS_METADATA_BIN_STATIONLOGO_PNG;
						break;
					case "image/gif":
						bASSEncodeMetaDataType = BASSEncodeMetaDataType.BASS_METADATA_BIN_STATIONLOGO_GIF;
						break;
					}
				}
				if (bASSEncodeMetaDataType > (BASSEncodeMetaDataType)0 && array != null)
				{
					BassEnc.BASS_Encode_CastSendMeta(base.Encoder.EncoderHandle, bASSEncodeMetaDataType, array);
				}
			}
			catch
			{
			}
		}).BeginInvoke(null, null);
	}
}

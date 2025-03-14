using System;
using System.Security;
using System.Text;
using Un4seen.Bass.AddOn.Wma;

namespace Un4seen.Bass.Misc;

[Serializable]
[SuppressUnmanagedCodeSecurity]
public sealed class WMAcast : StreamingServer
{
	public string StreamAuthor = "";

	public string StreamPublisher = "";

	public string StreamGenre = "";

	public string StreamCopyright = "";

	public string StreamDescription = "";

	public string StreamRating = "";

	public string StreamUrl = "";

	private EncoderWMA _wmaEncoder;

	private bool _isConnected;

	public bool UsePublish
	{
		get
		{
			return _wmaEncoder.WMA_UsePublish;
		}
		set
		{
			_wmaEncoder.WMA_UsePublish = value;
		}
	}

	public int NetworkPort
	{
		get
		{
			return _wmaEncoder.WMA_NetworkPort;
		}
		set
		{
			_wmaEncoder.WMA_NetworkPort = value;
		}
	}

	public int NetworkClients
	{
		get
		{
			return _wmaEncoder.WMA_NetworkClients;
		}
		set
		{
			_wmaEncoder.WMA_NetworkClients = value;
		}
	}

	public string PublishUrl
	{
		get
		{
			return _wmaEncoder.WMA_PublishUrl;
		}
		set
		{
			_wmaEncoder.WMA_PublishUrl = value;
		}
	}

	public string PublishUsername
	{
		get
		{
			return _wmaEncoder.WMA_PublishUsername;
		}
		set
		{
			_wmaEncoder.WMA_PublishUsername = value;
		}
	}

	public string PublishPassword
	{
		get
		{
			return _wmaEncoder.WMA_PublishPassword;
		}
		set
		{
			_wmaEncoder.WMA_PublishPassword = value;
		}
	}

	public override bool IsConnected
	{
		get
		{
			if (_wmaEncoder != null && _wmaEncoder.EncoderHandle != 0)
			{
				return _isConnected;
			}
			return false;
		}
	}

	public WMAcast(IBaseEncoder encoder)
		: base(encoder, useBASS: true)
	{
		if (encoder.EncoderType != BASSChannelType.BASS_CTYPE_STREAM_WMA)
		{
			throw new ArgumentException("Invalid EncoderType (only WMA is supported)!");
		}
		_wmaEncoder = encoder as EncoderWMA;
		if (_wmaEncoder == null)
		{
			throw new ArgumentNullException("Invalid Encoder used, encoder must be of type EncoderWMA!");
		}
		_wmaEncoder.WMA_UseNetwork = true;
		_wmaEncoder.InputFile = null;
	}

	public override bool Connect()
	{
		if (!_wmaEncoder.IsActive)
		{
			base.LastError = STREAMINGERROR.Error_EncoderError;
			base.LastErrorMessage = "Encoder not active or a connection to the server could not be established!";
			switch (Bass.BASS_ErrorGetCode())
			{
			case BASSError.BASS_ERROR_NOTAVAIL:
			case BASSError.BASS_ERROR_WMA_WM9:
			case BASSError.BASS_ERROR_WMA_CODEC:
				base.LastError = STREAMINGERROR.Error_EncoderError;
				base.LastErrorMessage = "WMA codec missing or WMA9 required!";
				break;
			case BASSError.BASS_ERROR_WMA_DENIED:
				base.LastError = STREAMINGERROR.Error_Login;
				base.LastErrorMessage = "Access denied - username/password invalid!";
				break;
			case BASSError.BASS_ERROR_FILEOPEN:
				base.LastError = STREAMINGERROR.Error_CreatingConnection;
				base.LastErrorMessage = "Couldn't connect to the server!";
				break;
			case BASSError.BASS_ERROR_ILLPARAM:
				base.LastError = STREAMINGERROR.Error_EncoderError;
				base.LastErrorMessage = "Illegal parameters have been used!";
				break;
			}
			_isConnected = false;
			return false;
		}
		if (base.SongTitle != null && base.SongTitle.Length > 0)
		{
			_wmaEncoder.SetTag("Title", base.SongTitle);
		}
		if (StreamAuthor != null && StreamAuthor.Length > 0)
		{
			_wmaEncoder.SetTag("Author", StreamAuthor);
		}
		if (StreamCopyright != null && StreamCopyright.Length > 0)
		{
			_wmaEncoder.SetTag("Copyright", StreamCopyright);
		}
		if (StreamDescription != null && StreamDescription.Length > 0)
		{
			_wmaEncoder.SetTag("Description", StreamDescription);
		}
		if (StreamRating != null && StreamRating.Length > 0)
		{
			_wmaEncoder.SetTag("Rating", StreamRating);
		}
		if (StreamGenre != null && StreamGenre.Length > 0)
		{
			_wmaEncoder.SetTag("WM/Genre", StreamGenre);
		}
		if (StreamPublisher != null && StreamPublisher.Length > 0)
		{
			_wmaEncoder.SetTag("WM/Publisher", StreamPublisher);
		}
		if (StreamUrl != null && StreamUrl.Length > 0)
		{
			_wmaEncoder.SetTag("WM/AuthorURL", StreamUrl);
		}
		_isConnected = true;
		return true;
	}

	public override bool Disconnect()
	{
		if (_wmaEncoder.IsActive)
		{
			_wmaEncoder.Stop();
		}
		_isConnected = false;
		return true;
	}

	public override bool Login()
	{
		return true;
	}

	public override int SendData(IntPtr buffer, int length)
	{
		return length;
	}

	public override bool UpdateTitle(string song, string url)
	{
		base.SongTitle = song;
		if (string.IsNullOrEmpty(base.SongTitle))
		{
			return false;
		}
		return WMAcastUpdateTitle(song);
	}

	public override int GetListeners(string password)
	{
		if (IsConnected)
		{
			return BassWma.BASS_WMA_EncodeGetClients(base.Encoder.EncoderHandle);
		}
		return -1;
	}

	public override string GetStats(string password)
	{
		if (!IsConnected)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("<?xml version=\"1.0\" standalone=\"yes\" ?>\n");
		stringBuilder.Append("<WMACASTSERVER>");
		stringBuilder.Append("<CURRENTLISTENERS>");
		stringBuilder.Append(GetListeners(password));
		stringBuilder.Append("</CURRENTLISTENERS>");
		stringBuilder.Append("<MAXLISTENERS>");
		if (UsePublish)
		{
			stringBuilder.Append(-1);
		}
		else
		{
			stringBuilder.Append(NetworkClients);
		}
		stringBuilder.Append("</MAXLISTENERS>");
		stringBuilder.Append("<SERVERGENRE>");
		stringBuilder.Append(StreamGenre);
		stringBuilder.Append("</SERVERGENRE>");
		stringBuilder.Append("<SERVERURL>");
		stringBuilder.Append(StreamUrl);
		stringBuilder.Append("</SERVERURL>");
		stringBuilder.Append("<SERVERTITLE>");
		stringBuilder.Append(StreamDescription);
		stringBuilder.Append("</SERVERTITLE>");
		stringBuilder.Append("<AUTHOR>");
		stringBuilder.Append(StreamAuthor);
		stringBuilder.Append("</AUTHOR>");
		stringBuilder.Append("<COPYRIGHT>");
		stringBuilder.Append(StreamCopyright);
		stringBuilder.Append("</COPYRIGHT>");
		stringBuilder.Append("<PUBLISHER>");
		stringBuilder.Append(StreamPublisher);
		stringBuilder.Append("</PUBLISHER>");
		stringBuilder.Append("<RATING>");
		stringBuilder.Append(StreamRating);
		stringBuilder.Append("</RATING>");
		stringBuilder.Append("<SONGTITLE>");
		stringBuilder.Append(base.SongTitle);
		stringBuilder.Append("</SONGTITLE>");
		stringBuilder.Append("<STREAMSTATUS>");
		stringBuilder.Append(IsConnected ? 1 : 0);
		stringBuilder.Append("</STREAMSTATUS>");
		stringBuilder.Append("<BITRATE>");
		stringBuilder.Append(base.Encoder.EffectiveBitrate);
		stringBuilder.Append("</BITRATE>");
		stringBuilder.Append("<CONTENT>");
		stringBuilder.Append("Audio/x-ms-wma");
		stringBuilder.Append("</CONTENT>");
		stringBuilder.Append("<SAMPLERATE>");
		stringBuilder.Append(base.Encoder.ChannelSampleRate);
		stringBuilder.Append("</SAMPLERATE>");
		stringBuilder.Append("<NUMCHANNELS>");
		stringBuilder.Append(base.Encoder.ChannelNumChans);
		stringBuilder.Append("</NUMCHANNELS>");
		stringBuilder.Append("</WMACASTSERVER>");
		return stringBuilder.ToString();
	}

	private bool WMAcastUpdateTitle(string song)
	{
		if (IsConnected)
		{
			_wmaEncoder.SetTag("Title", song);
			return _wmaEncoder.SetTag("CAPTION", song);
		}
		return false;
	}
}

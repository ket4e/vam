using System;
using System.Globalization;
using System.Threading;
using Un4seen.Bass.AddOn.Enc;
using Un4seen.Bass.AddOn.Wma;

namespace Un4seen.Bass.Misc;

[Serializable]
public sealed class EncoderWMA : BaseEncoder
{
	private bool _paused;

	private WMENCODEPROC _wmEncoderProc;

	private ENCODEPROC _encoderProc;

	private ENCODENOTIFYPROC _notify;

	private int _channel;

	private long _byteSend;

	private DSPPROC _dspCallback;

	private int _dspHandle;

	public int WMA_Bitrate = 128;

	public bool WMA_Use24Bit;

	public bool WMA_UsePro;

	public bool WMA_ForceStandard;

	public bool WMA_UseVBR;

	public int WMA_VBRQuality = 75;

	private int WMA_TargetSampleRate = 44100;

	private int WMA_TargetNumChans = 2;

	public bool WMA_UseNetwork;

	public bool WMA_UsePublish;

	public int[] WMA_MultiBitrate;

	public int WMA_NetworkPort = 8080;

	public int WMA_NetworkClients = 1;

	public string WMA_PublishUrl = string.Empty;

	public string WMA_PublishUsername = string.Empty;

	public string WMA_PublishPassword = string.Empty;

	public override bool IsActive
	{
		get
		{
			if (base.EncoderHandle == 0)
			{
				return false;
			}
			return true;
		}
	}

	public override bool IsPaused => _paused;

	public override bool EncoderExists => true;

	public override BASSChannelType EncoderType => BASSChannelType.BASS_CTYPE_STREAM_WMA;

	public override string DefaultOutputExtension => ".wma";

	public override bool SupportsSTDOUT => true;

	public override string EncoderCommandLine => string.Empty;

	public override int EffectiveBitrate
	{
		get
		{
			if (WMA_UseVBR && WMA_VBRQuality == 100)
			{
				return 778;
			}
			return WMA_Bitrate;
		}
	}

	public new bool Force16Bit => false;

	internal long ByteSend => _byteSend;

	public ENCODENOTIFYPROC WMA_Notify
	{
		get
		{
			return _notify;
		}
		set
		{
			_notify = value;
		}
	}

	public EncoderWMA(int channel)
		: base(channel)
	{
		WMA_TargetNumChans = base.ChannelNumChans;
		WMA_TargetSampleRate = base.ChannelSampleRate;
	}

	public override bool Stop()
	{
		if (_dspHandle != 0)
		{
			Bass.BASS_ChannelRemoveDSP(base.ChannelHandle, _dspHandle);
			_dspHandle = 0;
			_dspCallback = null;
		}
		if (base.EncoderHandle != 0)
		{
			BassWma.BASS_WMA_EncodeClose(base.EncoderHandle);
			base.EncoderHandle = 0;
			_wmEncoderProc = null;
			_encoderProc = null;
		}
		_byteSend = 0L;
		return true;
	}

	public override bool Pause(bool paused)
	{
		_paused = paused;
		return true;
	}

	public override string ToString()
	{
		return "Windows Media Audio (.wma)";
	}

	public override bool Start(ENCODEPROC proc, IntPtr user, bool paused)
	{
		if (base.EncoderHandle != 0 || (proc != null && !SupportsSTDOUT))
		{
			return false;
		}
		_paused = paused;
		_encoderProc = null;
		_byteSend = 0L;
		BASSWMAEncode bASSWMAEncode = BASSWMAEncode.BASS_WMA_ENCODE_DEFAULT;
		if (WMA_ForceStandard)
		{
			bASSWMAEncode |= BASSWMAEncode.BASS_WMA_ENCODE_STANDARD;
		}
		else
		{
			if (WMA_UsePro)
			{
				bASSWMAEncode |= BASSWMAEncode.BASS_WMA_ENCODE_PRO;
			}
			if (WMA_Use24Bit)
			{
				bASSWMAEncode |= BASSWMAEncode.BASS_WMA_ENCODE_24BIT;
				bASSWMAEncode |= BASSWMAEncode.BASS_WMA_ENCODE_PRO;
			}
		}
		_channel = base.ChannelHandle;
		WMA_TargetNumChans = base.ChannelNumChans;
		WMA_TargetSampleRate = base.ChannelSampleRate;
		if (base.InputFile != null)
		{
			_channel = Bass.BASS_StreamCreateFile(base.InputFile, 0L, 0L, BASSFlag.BASS_STREAM_DECODE | (WMA_Use24Bit ? BASSFlag.BASS_SAMPLE_FLOAT : BASSFlag.BASS_DEFAULT));
			if (_channel == 0)
			{
				return false;
			}
			if (WMA_Use24Bit)
			{
				bASSWMAEncode |= BASSWMAEncode.BASS_SAMPLE_FLOAT;
			}
		}
		else if (base.ChannelBitwidth == 32)
		{
			bASSWMAEncode |= BASSWMAEncode.BASS_SAMPLE_FLOAT;
		}
		else if (base.ChannelBitwidth == 8)
		{
			bASSWMAEncode |= BASSWMAEncode.BASS_SAMPLE_8BITS;
		}
		if (WMA_UseNetwork)
		{
			bASSWMAEncode |= BASSWMAEncode.BASS_WMA_ENCODE_SCRIPT;
		}
		int bitrate = WMA_Bitrate * 1000;
		if (WMA_UseVBR && WMA_VBRQuality > 0 && WMA_VBRQuality <= 100)
		{
			bitrate = WMA_VBRQuality;
		}
		if (proc != null && !WMA_UseNetwork)
		{
			_encoderProc = proc;
			_wmEncoderProc = EncodingWMAHandler;
		}
		if (base.OutputFile == null)
		{
			if (WMA_UseNetwork && !WMA_UsePublish)
			{
				if (WMA_MultiBitrate != null && WMA_MultiBitrate.Length != 0)
				{
					int[] array = new int[WMA_MultiBitrate.Length];
					for (int i = 0; i < WMA_MultiBitrate.Length; i++)
					{
						array[i] = WMA_MultiBitrate[i] * 1000;
					}
					base.EncoderHandle = BassWma.BASS_WMA_EncodeOpenNetworkMulti(WMA_TargetSampleRate, WMA_TargetNumChans, bASSWMAEncode, array, WMA_NetworkPort, WMA_NetworkClients);
				}
				else
				{
					if (WMA_MultiBitrate != null)
					{
						bitrate = WMA_MultiBitrate[0];
					}
					base.EncoderHandle = BassWma.BASS_WMA_EncodeOpenNetwork(WMA_TargetSampleRate, WMA_TargetNumChans, bASSWMAEncode, bitrate, WMA_NetworkPort, WMA_NetworkClients);
				}
			}
			else if (WMA_UseNetwork && WMA_UsePublish)
			{
				if (WMA_MultiBitrate != null && WMA_MultiBitrate.Length > 1)
				{
					int[] array2 = new int[WMA_MultiBitrate.Length];
					for (int j = 0; j < WMA_MultiBitrate.Length; j++)
					{
						array2[j] = WMA_MultiBitrate[j] * 1000;
					}
					base.EncoderHandle = BassWma.BASS_WMA_EncodeOpenPublishMulti(WMA_TargetSampleRate, WMA_TargetNumChans, bASSWMAEncode, array2, WMA_PublishUrl, WMA_PublishUsername, WMA_PublishPassword);
				}
				else
				{
					base.EncoderHandle = BassWma.BASS_WMA_EncodeOpenPublish(WMA_TargetSampleRate, WMA_TargetNumChans, bASSWMAEncode, bitrate, WMA_PublishUrl, WMA_PublishUsername, WMA_PublishPassword);
				}
			}
			else if (proc != null)
			{
				base.EncoderHandle = BassWma.BASS_WMA_EncodeOpen(WMA_TargetSampleRate, WMA_TargetNumChans, bASSWMAEncode, bitrate, _wmEncoderProc, user);
			}
		}
		else
		{
			base.EncoderHandle = BassWma.BASS_WMA_EncodeOpenFile(WMA_TargetSampleRate, WMA_TargetNumChans, bASSWMAEncode, bitrate, base.OutputFile);
			if (base.TAGs != null)
			{
				if (!string.IsNullOrEmpty(base.TAGs.title))
				{
					SetTag("Title", base.TAGs.title);
				}
				if (!string.IsNullOrEmpty(base.TAGs.artist))
				{
					SetTag("Author", base.TAGs.artist);
				}
				if (!string.IsNullOrEmpty(base.TAGs.album))
				{
					SetTag("WM/AlbumTitle", base.TAGs.album);
				}
				if (!string.IsNullOrEmpty(base.TAGs.albumartist))
				{
					SetTag("WM/AlbumArtist", base.TAGs.albumartist);
				}
				if (!string.IsNullOrEmpty(base.TAGs.year))
				{
					SetTag("WM/Year", base.TAGs.year);
				}
				if (!string.IsNullOrEmpty(base.TAGs.track))
				{
					SetTag("WM/TrackNumber", base.TAGs.track);
				}
				if (!string.IsNullOrEmpty(base.TAGs.disc))
				{
					SetTag("WM/PartOfSet", base.TAGs.disc);
				}
				if (!string.IsNullOrEmpty(base.TAGs.genre))
				{
					SetTag("WM/Genre", base.TAGs.genre);
				}
				if (!string.IsNullOrEmpty(base.TAGs.comment))
				{
					SetTag("Description", base.TAGs.comment);
				}
				if (!string.IsNullOrEmpty(base.TAGs.composer))
				{
					SetTag("WM/Composer", base.TAGs.composer);
				}
				if (!string.IsNullOrEmpty(base.TAGs.conductor))
				{
					SetTag("WM/Conductor", base.TAGs.conductor);
				}
				if (!string.IsNullOrEmpty(base.TAGs.lyricist))
				{
					SetTag("WM/Writer", base.TAGs.lyricist);
				}
				if (!string.IsNullOrEmpty(base.TAGs.remixer))
				{
					SetTag("WM/ModifiedBy", base.TAGs.remixer);
				}
				if (!string.IsNullOrEmpty(base.TAGs.producer))
				{
					SetTag("WM/Producer", base.TAGs.producer);
				}
				if (!string.IsNullOrEmpty(base.TAGs.encodedby))
				{
					SetTag("WM/EncodedBy", base.TAGs.encodedby);
				}
				if (!string.IsNullOrEmpty(base.TAGs.copyright))
				{
					SetTag("Copyright", base.TAGs.copyright);
				}
				if (!string.IsNullOrEmpty(base.TAGs.publisher))
				{
					SetTag("WM/Publisher", base.TAGs.publisher);
				}
				if (!string.IsNullOrEmpty(base.TAGs.bpm))
				{
					SetTag("WM/BeatsPerMinute", base.TAGs.bpm);
				}
				if (!string.IsNullOrEmpty(base.TAGs.grouping))
				{
					SetTag("WM/ContentGroupDescription", base.TAGs.grouping);
				}
				if (!string.IsNullOrEmpty(base.TAGs.rating))
				{
					SetTag("WM/Rating", base.TAGs.rating);
				}
				if (!string.IsNullOrEmpty(base.TAGs.mood))
				{
					SetTag("WM/Mood", base.TAGs.mood);
				}
				if (!string.IsNullOrEmpty(base.TAGs.isrc))
				{
					SetTag("WM/ISRC", base.TAGs.isrc);
				}
				if (base.TAGs.replaygain_track_peak >= 0f)
				{
					SetTag("replaygain_track_peak", base.TAGs.replaygain_track_peak.ToString("R", CultureInfo.InvariantCulture));
				}
				if (base.TAGs.replaygain_track_gain >= -60f && base.TAGs.replaygain_track_gain <= 60f)
				{
					SetTag("replaygain_track_gain", base.TAGs.replaygain_track_gain.ToString("R", CultureInfo.InvariantCulture) + " dB");
				}
			}
		}
		if (base.EncoderHandle == 0)
		{
			return false;
		}
		_dspCallback = EncodingDSPHandler;
		_dspHandle = Bass.BASS_ChannelSetDSP(base.ChannelHandle, _dspCallback, IntPtr.Zero, Bass.BASS_GetConfig(BASSConfig.BASS_CONFIG_ENCODE_PRIORITY));
		if (_dspHandle == 0)
		{
			Stop();
			return false;
		}
		if (base.InputFile != null)
		{
			Utils.DecodeAllData(_channel, autoFree: true);
		}
		_channel = 0;
		if (base.EncoderHandle == 0)
		{
			return false;
		}
		return true;
	}

	public override string SettingsString()
	{
		string text = EffectiveBitrate.ToString();
		if (WMA_MultiBitrate != null && WMA_MultiBitrate.Length != 0)
		{
			text = Convert.ToString(WMA_MultiBitrate[0], CultureInfo.InvariantCulture);
			for (int i = 1; i < WMA_MultiBitrate.Length; i++)
			{
				text = text + "+" + Convert.ToString(WMA_MultiBitrate[i], CultureInfo.InvariantCulture);
			}
		}
		if (WMA_UseVBR && WMA_VBRQuality == 100)
		{
			return string.Format("Lossless-{0} kbps, {1}-bit {2}", text, WMA_Use24Bit ? 24 : 16, WMA_UsePro ? "Pro" : "").Trim();
		}
		return string.Format("{0}-{1} kbps, {2}-bit {3} {4}", WMA_UseVBR ? "VBR" : "CBR", text, WMA_Use24Bit ? 24 : 16, WMA_UsePro ? "Pro" : "", WMA_UseVBR ? (WMA_VBRQuality + "%") : "").Trim();
	}

	private void EncodingWMAHandler(int handle, BASSWMAEncodeCallback type, IntPtr buffer, int length, IntPtr user)
	{
		switch (type)
		{
		case BASSWMAEncodeCallback.BASS_WMA_ENCODE_DATA:
			_encoderProc(handle, _channel, buffer, length, user);
			break;
		case BASSWMAEncodeCallback.BASS_WMA_ENCODE_HEAD:
			_encoderProc(handle, _channel, buffer, -1 * length, user);
			break;
		}
	}

	private void EncodingDSPHandler(int handle, int channel, IntPtr buffer, int length, IntPtr user)
	{
		if (_paused || base.EncoderHandle == 0)
		{
			return;
		}
		if (!BassWma.BASS_WMA_EncodeWrite(base.EncoderHandle, buffer, length))
		{
			ThreadPool.QueueUserWorkItem(DoNotify);
		}
		try
		{
			_byteSend += length;
		}
		catch
		{
			_byteSend = length;
		}
	}

	private void DoNotify(object state)
	{
		Stop();
		if (_notify != null)
		{
			_notify(base.EncoderHandle, BASSEncodeNotify.BASS_ENCODE_NOTIFY_ENCODER, IntPtr.Zero);
		}
	}

	public bool SetTag(string tag, string value)
	{
		if (base.EncoderHandle == 0)
		{
			return false;
		}
		return BassWma.BASS_WMA_EncodeSetTag(base.EncoderHandle, tag, value);
	}
}

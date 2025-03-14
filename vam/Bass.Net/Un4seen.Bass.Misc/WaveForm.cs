using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using Un4seen.Bass.AddOn.Fx;

namespace Un4seen.Bass.Misc;

[Serializable]
[SuppressUnmanagedCodeSecurity]
public sealed class WaveForm
{
	[Flags]
	public enum MARKERDRAWTYPE
	{
		None = 0,
		Line = 1,
		Name = 2,
		NamePositionAlternate = 0,
		NamePositionTop = 4,
		NamePositionBottom = 8,
		NamePositionMiddle = 0x10,
		NameBoxFilled = 0x100,
		NameBoxFillInverted = 0x200
	}

	[Flags]
	public enum VOLUMEDRAWTYPE
	{
		None = 0,
		Solid = 1,
		Dotted = 2,
		NoPoints = 4
	}

	[Flags]
	public enum BEATDRAWTYPE
	{
		None = 0,
		Top = 1,
		Bottom = 2,
		Middle = 3,
		TopBottom = 4
	}

	public enum WAVEFORMDRAWTYPE
	{
		Stereo,
		Mono,
		DualMono,
		HalfMono,
		HalfMonoFlipped
	}

	[Serializable]
	public class WaveBuffer
	{
		[Serializable]
		public struct Level
		{
			public short left;

			public short right;

			public Level(short levelL, short levelR)
			{
				left = levelL;
				right = levelR;
			}
		}

		internal string fileName = string.Empty;

		public BASSFlag flags;

		public int chans = 2;

		public int bpf;

		public double resolution = 0.02;

		public Level[] data;

		public Dictionary<string, long> marker;

		public List<long> beats;

		public int bps
		{
			get
			{
				if ((flags & BASSFlag.BASS_SAMPLE_FLOAT) != 0)
				{
					return 4;
				}
				if ((flags & BASSFlag.BASS_SAMPLE_8BITS) != 0)
				{
					return 1;
				}
				return 2;
			}
		}

		internal WaveBuffer()
		{
		}
	}

	public class VolumePoint : IComparable
	{
		public long Position;

		public float Level = 1f;

		public VolumePoint(long position, float level)
		{
			Position = position;
			Level = level;
		}

		public override string ToString()
		{
			return $"Position: {Position}, Level: {Level}";
		}

		public int CompareTo(object obj)
		{
			if (obj is VolumePoint)
			{
				VolumePoint volumePoint = (VolumePoint)obj;
				return Position.CompareTo(volumePoint.Position);
			}
			throw new ArgumentException("object is not a VolumePoint");
		}
	}

	private object _syncRoot = new object();

	private string _fileName = string.Empty;

	private double _frameResolution = 0.01;

	private int _callbackFrequency = 250;

	private WAVEFORMPROC _notifyHandler;

	private Control _win;

	private bool _preScan = true;

	private bool _useSimpleScan;

	private bool _isRendered;

	private int _framesToRender;

	private int _framesRendered;

	private bool _detectBeats;

	private float _gainFactor = 1f;

	private Color _colorBackground = SystemColors.Control;

	private Color _colorLeft = Color.Gainsboro;

	private Color _colorLeft2 = Color.White;

	private Color _colorLeftEnvelope = Color.Gray;

	private bool _drawEnvelope = true;

	private bool _drawCenterLine = true;

	private Color _colorMiddleLeft = Color.Empty;

	private Color _colorMiddleRight = Color.Empty;

	private bool _drawGradient;

	private Color _colorRight = Color.LightGray;

	private Color _colorRight2 = Color.White;

	private Color _colorRightEnvelope = Color.DimGray;

	private Color _colorVolume = Color.IndianRed;

	private PixelFormat _pixelFormat = PixelFormat.Format32bppArgb;

	private VOLUMEDRAWTYPE _drawVolume;

	private bool _volumeCurveZeroLevel;

	private Color _colorBeat = Color.CornflowerBlue;

	private BEATDRAWTYPE _drawBeat;

	private float _beatLength = 0.05f;

	private int _beatWidth = 1;

	private Color _colorMarker = Color.DarkBlue;

	private Font _markerFont = new Font("Arial", 7.5f, FontStyle.Regular);

	private MARKERDRAWTYPE _drawMarker;

	private float _markerLength = 0.1f;

	private WAVEFORMDRAWTYPE _drawWaveForm;

	private int _decodingStream;

	private DateTime _renderStartTime;

	private bool _killScan;

	private volatile bool _renderingInProgress;

	private WaveBuffer _waveBuffer;

	private float[] _peakLevelFloat;

	private short[] _peakLevelShort;

	private byte[] _peakLevelByte;

	private int _recordingNextLength = 10;

	private double _channelFactor = 1.0;

	private double _syncFactor = 1.0;

	private double _correctionFactor = 1.0;

	private List<VolumePoint> _volumePoints;

	private int _recordingLeftoverBytes;

	private WaveBuffer.Level _recordingLeftoverLevel;

	private bool _freeStream = true;

	public string FileName
	{
		get
		{
			return _fileName;
		}
		set
		{
			if (_renderingInProgress || (!string.IsNullOrEmpty(value) && value == _fileName))
			{
				return;
			}
			lock (_syncRoot)
			{
				_fileName = value;
				_isRendered = false;
				_decodingStream = 0;
				_framesToRender = 0;
				_framesRendered = 0;
				if (_waveBuffer != null)
				{
					_waveBuffer.data = null;
					if (_waveBuffer.marker != null)
					{
						_waveBuffer.marker.Clear();
						_waveBuffer.marker = null;
					}
					if (_waveBuffer.beats != null)
					{
						_waveBuffer.beats.Clear();
						_waveBuffer.beats = null;
					}
					_waveBuffer = null;
				}
				if (_volumePoints != null)
				{
					_volumePoints.Clear();
					_volumePoints = null;
				}
			}
		}
	}

	public double FrameResolution
	{
		get
		{
			return _frameResolution;
		}
		set
		{
			if (value < 0.0010000000474974513)
			{
				_frameResolution = 0.0010000000474974513;
			}
			else if (value > 5.0)
			{
				_frameResolution = 5.0;
			}
			else
			{
				_frameResolution = value;
			}
			if (_useSimpleScan)
			{
				_frameResolution = 0.019999999552965164;
			}
		}
	}

	public int CallbackFrequency
	{
		get
		{
			return _callbackFrequency;
		}
		set
		{
			_callbackFrequency = value;
		}
	}

	public WAVEFORMPROC NotifyHandler
	{
		get
		{
			return _notifyHandler;
		}
		set
		{
			_notifyHandler = value;
		}
	}

	public Control WinControl
	{
		get
		{
			return _win;
		}
		set
		{
			_win = value;
		}
	}

	public bool PreScan
	{
		get
		{
			return _preScan;
		}
		set
		{
			_preScan = value;
		}
	}

	public bool UseSimpleScan
	{
		get
		{
			return _useSimpleScan;
		}
		set
		{
			_useSimpleScan = value;
			if (_useSimpleScan)
			{
				_frameResolution = 0.02;
			}
		}
	}

	public bool IsRendered => _isRendered;

	public bool IsRenderingInProgress => _renderingInProgress;

	public int FramesToRender => _framesToRender;

	public int FramesRendered => _framesRendered;

	public bool DetectBeats
	{
		get
		{
			return _detectBeats;
		}
		set
		{
			_detectBeats = value;
		}
	}

	public double TempoFactor
	{
		get
		{
			return _syncFactor / _channelFactor - 1.0;
		}
		set
		{
			_syncFactor = _channelFactor * (1.0 + value);
		}
	}

	public double SyncFactor => _syncFactor;

	public float GainFactor
	{
		get
		{
			return _gainFactor;
		}
		set
		{
			_gainFactor = value;
		}
	}

	public WaveBuffer Wave
	{
		get
		{
			return _waveBuffer;
		}
		set
		{
			if (_renderingInProgress || value == null)
			{
				return;
			}
			lock (_syncRoot)
			{
				_waveBuffer = value;
				_isRendered = true;
				_fileName = _waveBuffer.fileName;
				_framesRendered = _waveBuffer.data.Length;
				_framesToRender = _waveBuffer.data.Length;
				_frameResolution = _waveBuffer.resolution;
			}
		}
	}

	public Color ColorBackground
	{
		get
		{
			return _colorBackground;
		}
		set
		{
			_colorBackground = value;
		}
	}

	public Color ColorLeft
	{
		get
		{
			return _colorLeft;
		}
		set
		{
			_colorLeft = value;
		}
	}

	public Color ColorLeft2
	{
		get
		{
			return _colorLeft2;
		}
		set
		{
			_colorLeft2 = value;
		}
	}

	public Color ColorLeftEnvelope
	{
		get
		{
			return _colorLeftEnvelope;
		}
		set
		{
			_colorLeftEnvelope = value;
		}
	}

	public bool DrawEnvelope
	{
		get
		{
			return _drawEnvelope;
		}
		set
		{
			_drawEnvelope = value;
		}
	}

	public bool DrawCenterLine
	{
		get
		{
			return _drawCenterLine;
		}
		set
		{
			_drawCenterLine = value;
		}
	}

	public Color ColorMiddleLeft
	{
		get
		{
			return _colorMiddleLeft;
		}
		set
		{
			_colorMiddleLeft = value;
		}
	}

	public Color ColorMiddleRight
	{
		get
		{
			return _colorMiddleRight;
		}
		set
		{
			_colorMiddleRight = value;
		}
	}

	public bool DrawGradient
	{
		get
		{
			return _drawGradient;
		}
		set
		{
			_drawGradient = value;
		}
	}

	public Color ColorRight
	{
		get
		{
			return _colorRight;
		}
		set
		{
			_colorRight = value;
		}
	}

	public Color ColorRight2
	{
		get
		{
			return _colorRight2;
		}
		set
		{
			_colorRight2 = value;
		}
	}

	public Color ColorRightEnvelope
	{
		get
		{
			return _colorRightEnvelope;
		}
		set
		{
			_colorRightEnvelope = value;
		}
	}

	public Color ColorVolume
	{
		get
		{
			return _colorVolume;
		}
		set
		{
			_colorVolume = value;
		}
	}

	public PixelFormat PixelFormat
	{
		get
		{
			return _pixelFormat;
		}
		set
		{
			_pixelFormat = value;
		}
	}

	public VOLUMEDRAWTYPE DrawVolume
	{
		get
		{
			return _drawVolume;
		}
		set
		{
			_drawVolume = value;
		}
	}

	public bool VolumeCurveZeroLevel
	{
		get
		{
			return _volumeCurveZeroLevel;
		}
		set
		{
			_volumeCurveZeroLevel = value;
		}
	}

	public Color ColorBeat
	{
		get
		{
			return _colorBeat;
		}
		set
		{
			_colorBeat = value;
		}
	}

	public BEATDRAWTYPE DrawBeat
	{
		get
		{
			return _drawBeat;
		}
		set
		{
			_drawBeat = value;
		}
	}

	public float BeatLength
	{
		get
		{
			return _beatLength;
		}
		set
		{
			if (value > 0f && value <= 1f)
			{
				_beatLength = value / 2f;
			}
		}
	}

	public int BeatWidth
	{
		get
		{
			return _beatWidth;
		}
		set
		{
			if (value > 0 && value <= 10)
			{
				_beatWidth = value;
			}
		}
	}

	public Color ColorMarker
	{
		get
		{
			return _colorMarker;
		}
		set
		{
			_colorMarker = value;
		}
	}

	public Font MarkerFont
	{
		get
		{
			return _markerFont;
		}
		set
		{
			_markerFont = value;
		}
	}

	public MARKERDRAWTYPE DrawMarker
	{
		get
		{
			return _drawMarker;
		}
		set
		{
			_drawMarker = value;
		}
	}

	public float MarkerLength
	{
		get
		{
			return _markerLength;
		}
		set
		{
			if (value > 0f && value <= 1f)
			{
				_markerLength = value / 2f;
			}
		}
	}

	public WAVEFORMDRAWTYPE DrawWaveForm
	{
		get
		{
			return _drawWaveForm;
		}
		set
		{
			_drawWaveForm = value;
		}
	}

	public WaveForm()
	{
		FileName = string.Empty;
	}

	public WaveForm(string fileName)
	{
		FileName = fileName;
	}

	public WaveForm(string fileName, WAVEFORMPROC proc, Control win)
	{
		FileName = fileName;
		NotifyHandler = proc;
		WinControl = win;
	}

	private WaveForm(WaveForm clone, bool flat)
	{
		lock (clone._syncRoot)
		{
			_fileName = clone._fileName;
			_notifyHandler = clone._notifyHandler;
			_win = clone._win;
			_frameResolution = clone._frameResolution;
			_callbackFrequency = clone._callbackFrequency;
			_isRendered = clone._isRendered;
			_renderingInProgress = clone._renderingInProgress;
			_framesToRender = clone._framesToRender;
			_framesRendered = clone._framesRendered;
			_detectBeats = clone._detectBeats;
			_decodingStream = clone._decodingStream;
			_renderStartTime = clone._renderStartTime;
			_recordingNextLength = clone._recordingNextLength;
			_syncFactor = clone._syncFactor;
			_channelFactor = clone._channelFactor;
			_correctionFactor = clone._correctionFactor;
			_gainFactor = clone._gainFactor;
			if (clone._volumePoints != null)
			{
				_volumePoints = new List<VolumePoint>(clone._volumePoints.Count);
				foreach (VolumePoint volumePoint in clone._volumePoints)
				{
					_volumePoints.Add(new VolumePoint(volumePoint.Position, volumePoint.Level));
				}
			}
			if (clone._waveBuffer != null)
			{
				_waveBuffer = new WaveBuffer();
				_waveBuffer.bpf = clone._waveBuffer.bpf;
				_waveBuffer.chans = clone._waveBuffer.chans;
				_waveBuffer.fileName = clone._waveBuffer.fileName;
				_waveBuffer.flags = clone._waveBuffer.flags;
				_waveBuffer.resolution = clone._waveBuffer.resolution;
				if (clone._waveBuffer.marker != null)
				{
					_waveBuffer.marker = new Dictionary<string, long>(clone._waveBuffer.marker.Count);
					foreach (KeyValuePair<string, long> item in clone._waveBuffer.marker)
					{
						_waveBuffer.marker.Add(item.Key, item.Value);
					}
				}
				if (flat)
				{
					_waveBuffer.beats = clone._waveBuffer.beats;
					_waveBuffer.data = clone._waveBuffer.data;
				}
				else
				{
					if (clone._waveBuffer.beats != null)
					{
						_waveBuffer.beats = new List<long>(clone._waveBuffer.beats.Count);
						_waveBuffer.beats.AddRange(clone._waveBuffer.beats);
					}
					if (clone._waveBuffer.data != null)
					{
						_waveBuffer.data = new WaveBuffer.Level[clone._waveBuffer.data.Length];
						for (int i = 0; i < clone._waveBuffer.data.Length; i++)
						{
							WaveBuffer.Level level = clone._waveBuffer.data[i];
							_waveBuffer.data[i] = new WaveBuffer.Level(level.left, level.right);
						}
					}
				}
			}
			_colorBackground = clone._colorBackground;
			_colorLeft = clone._colorLeft;
			_colorLeft2 = clone._colorLeft2;
			_colorLeftEnvelope = clone._colorLeftEnvelope;
			_drawEnvelope = clone._drawEnvelope;
			_drawCenterLine = clone._drawCenterLine;
			_colorMiddleLeft = clone._colorMiddleLeft;
			_colorMiddleRight = clone._colorMiddleRight;
			_drawGradient = clone._drawGradient;
			_colorRight = clone._colorRight;
			_colorRight2 = clone._colorRight2;
			_colorRightEnvelope = clone._colorRightEnvelope;
			_colorVolume = clone._colorVolume;
			_pixelFormat = clone._pixelFormat;
			_drawVolume = clone._drawVolume;
			_volumeCurveZeroLevel = clone._volumeCurveZeroLevel;
			_colorBeat = clone._colorBeat;
			_drawBeat = clone._drawBeat;
			_beatLength = clone._beatLength;
			_beatWidth = clone._beatWidth;
			_colorMarker = clone._colorMarker;
			_markerFont = clone._markerFont;
			_drawMarker = clone._drawMarker;
			_markerLength = clone._markerLength;
			_drawWaveForm = clone._drawWaveForm;
		}
	}

	public WaveForm Clone(bool flat)
	{
		if (IsRendered && !IsRenderingInProgress)
		{
			return new WaveForm(this, flat);
		}
		return null;
	}

	public void Reset()
	{
		FileName = string.Empty;
		_win = null;
		_notifyHandler = null;
	}

	public bool RenderStart(int decodingStream, bool background)
	{
		return RenderStart(decodingStream, background, ThreadPriority.Normal, freeStream: true);
	}

	public bool RenderStart(int decodingStream, bool background, bool freeStream)
	{
		return RenderStart(decodingStream, background, ThreadPriority.Normal, freeStream);
	}

	public bool RenderStart(int decodingStream, bool background, ThreadPriority prio, bool freeStream)
	{
		if (decodingStream == 0)
		{
			return false;
		}
		_decodingStream = decodingStream;
		Bass.BASS_ChannelSetPosition(_decodingStream, 0L);
		BASS_CHANNELINFO bASS_CHANNELINFO = new BASS_CHANNELINFO();
		if (Bass.BASS_ChannelGetInfo(_decodingStream, bASS_CHANNELINFO))
		{
			if (bASS_CHANNELINFO.chans == 0)
			{
				bASS_CHANNELINFO.chans = 2;
			}
			if ((bASS_CHANNELINFO.ctype & BASSChannelType.BASS_CTYPE_STREAM) == 0 && (bASS_CHANNELINFO.ctype & BASSChannelType.BASS_CTYPE_MUSIC_MOD) == 0)
			{
				return false;
			}
			_freeStream = freeStream;
			return Render(background, prio, bASS_CHANNELINFO.flags, bASS_CHANNELINFO.chans);
		}
		return false;
	}

	public bool RenderStart(bool background, BASSFlag flags)
	{
		return RenderStart(background, ThreadPriority.Normal, flags, IntPtr.Zero, 0L);
	}

	public bool RenderStart(bool background, ThreadPriority prio, BASSFlag flags)
	{
		return RenderStart(background, prio, flags, IntPtr.Zero, 0L);
	}

	public bool RenderStart(bool background, BASSFlag flags, IntPtr memory, long length)
	{
		return RenderStart(background, ThreadPriority.Normal, flags, memory, length);
	}

	private bool RenderStart(bool background, ThreadPriority prio, BASSFlag flags, IntPtr memory, long length)
	{
		if (memory == IntPtr.Zero && string.IsNullOrEmpty(FileName))
		{
			return false;
		}
		flags |= BASSFlag.BASS_STREAM_DECODE | (_preScan ? BASSFlag.BASS_STREAM_PRESCAN : BASSFlag.BASS_DEFAULT);
		_decodingStream = 0;
		if (memory == IntPtr.Zero)
		{
			_decodingStream = Bass.BASS_StreamCreateFile(FileName, 0L, 0L, flags);
		}
		else
		{
			_decodingStream = Bass.BASS_StreamCreateFile(memory, 0L, length, flags);
		}
		if (_decodingStream == 0)
		{
			return false;
		}
		int num = 2;
		BASS_CHANNELINFO bASS_CHANNELINFO = new BASS_CHANNELINFO();
		if (Bass.BASS_ChannelGetInfo(_decodingStream, bASS_CHANNELINFO))
		{
			num = bASS_CHANNELINFO.chans;
			if (num == 0)
			{
				num = 2;
			}
			if ((bASS_CHANNELINFO.ctype & BASSChannelType.BASS_CTYPE_STREAM) == 0 && (bASS_CHANNELINFO.ctype & BASSChannelType.BASS_CTYPE_MUSIC_MOD) == 0)
			{
				return false;
			}
			_freeStream = true;
			return Render(background, prio, bASS_CHANNELINFO.flags, num);
		}
		return false;
	}

	public void RenderStop()
	{
		_killScan = true;
	}

	public bool RenderStartRecording(int recordingStream, int initLength, int nextLength)
	{
		return RenderStartRecording(recordingStream, (float)initLength, (float)nextLength);
	}

	public bool RenderStartRecording(int recordingStream, float initLength, float nextLength)
	{
		if (recordingStream == 0 || initLength <= 0f)
		{
			return false;
		}
		if (nextLength < 0f)
		{
			nextLength = 0f;
		}
		if (initLength < 1f)
		{
			initLength = 1f;
		}
		_decodingStream = recordingStream;
		BASSFlag flags = BASSFlag.BASS_DEFAULT;
		int num = 2;
		BASS_CHANNELINFO bASS_CHANNELINFO = new BASS_CHANNELINFO();
		if (Bass.BASS_ChannelGetInfo(_decodingStream, bASS_CHANNELINFO))
		{
			flags = bASS_CHANNELINFO.flags;
			num = bASS_CHANNELINFO.chans;
			if (num == 0)
			{
				num = 2;
			}
		}
		_isRendered = false;
		_renderingInProgress = true;
		_framesRendered = 0;
		_recordingLeftoverBytes = 0;
		_recordingLeftoverLevel.left = 0;
		_recordingLeftoverLevel.right = 0;
		lock (_syncRoot)
		{
			_waveBuffer = new WaveBuffer();
			double num2 = Bass.BASS_ChannelSeconds2Bytes(_decodingStream, initLength);
			double num3 = Bass.BASS_ChannelSeconds2Bytes(_decodingStream, nextLength);
			_waveBuffer.chans = num;
			_waveBuffer.resolution = FrameResolution;
			_waveBuffer.bpf = (int)Bass.BASS_ChannelSeconds2Bytes(_decodingStream, _waveBuffer.resolution);
			_waveBuffer.flags = flags;
			_framesToRender = (int)Math.Ceiling(num2 / (double)_waveBuffer.bpf);
			_recordingNextLength = (int)Math.Ceiling(num3 / (double)_waveBuffer.bpf);
			_waveBuffer.data = new WaveBuffer.Level[_framesToRender];
			_waveBuffer.fileName = FileName;
			_renderStartTime = DateTime.Now;
		}
		return true;
	}

	public void RenderStopRecording()
	{
		_renderingInProgress = false;
		_isRendered = true;
		_recordingLeftoverBytes = 0;
		_recordingLeftoverLevel.left = 0;
		_recordingLeftoverLevel.right = 0;
	}

	public void RenderRecording()
	{
		if (_waveBuffer == null || _isRendered)
		{
			return;
		}
		if (Bass.BASS_ChannelIsActive(_decodingStream) == BASSActive.BASS_ACTIVE_STOPPED)
		{
			RenderStopRecording();
			return;
		}
		int num = 0;
		int bps = _waveBuffer.bps;
		int num2 = _waveBuffer.bpf / bps;
		int num3 = 0;
		if (_recordingLeftoverBytes > 0)
		{
			num3 = _waveBuffer.bpf - _recordingLeftoverBytes;
		}
		int num4 = Bass.BASS_ChannelGetData(_decodingStream, IntPtr.Zero, 0);
		if (num4 <= 0)
		{
			return;
		}
		int num5 = num4 / bps;
		switch (bps)
		{
		case 2:
			if (_peakLevelShort == null || _peakLevelShort.Length < num5)
			{
				_peakLevelShort = new short[num5];
			}
			num4 = Bass.BASS_ChannelGetData(_decodingStream, _peakLevelShort, num4);
			break;
		case 4:
			if (_peakLevelFloat == null || _peakLevelFloat.Length < num5)
			{
				_peakLevelFloat = new float[num5];
			}
			num4 = Bass.BASS_ChannelGetData(_decodingStream, _peakLevelFloat, num4);
			break;
		default:
			if (_peakLevelByte == null || _peakLevelByte.Length < num5)
			{
				_peakLevelByte = new byte[num5];
			}
			num4 = Bass.BASS_ChannelGetData(_decodingStream, _peakLevelByte, num4);
			break;
		}
		int num6 = (num4 - num3) / _waveBuffer.bpf;
		if (_recordingLeftoverBytes > 0)
		{
			WaveBuffer.Level level = bps switch
			{
				2 => GetLevel(_peakLevelShort, _waveBuffer.chans, 0, num3 / bps), 
				4 => GetLevel(_peakLevelFloat, _waveBuffer.chans, 0, num3 / bps), 
				_ => GetLevel(_peakLevelByte, _waveBuffer.chans, 0, num3 / bps), 
			};
			if (_recordingLeftoverLevel.left == short.MinValue)
			{
				level.left = _recordingLeftoverLevel.left;
			}
			else if (level.left < 0 && _recordingLeftoverLevel.left < 0 && level.left > _recordingLeftoverLevel.left)
			{
				level.left = _recordingLeftoverLevel.left;
			}
			else if (level.left > 0 && _recordingLeftoverLevel.left > 0 && level.left < _recordingLeftoverLevel.left)
			{
				level.left = _recordingLeftoverLevel.left;
			}
			else if (level.left < 0 && level.left > -_recordingLeftoverLevel.left)
			{
				level.left = _recordingLeftoverLevel.left;
			}
			else if (level.left > 0 && level.left < -_recordingLeftoverLevel.left)
			{
				level.left = _recordingLeftoverLevel.left;
			}
			if (_recordingLeftoverLevel.right == short.MinValue)
			{
				level.right = _recordingLeftoverLevel.right;
			}
			else if (level.right < 0 && _recordingLeftoverLevel.right < 0 && level.right > _recordingLeftoverLevel.right)
			{
				level.right = _recordingLeftoverLevel.right;
			}
			else if (level.right > 0 && _recordingLeftoverLevel.right > 0 && level.right < _recordingLeftoverLevel.right)
			{
				level.right = _recordingLeftoverLevel.right;
			}
			else if (level.right < 0 && level.right > -_recordingLeftoverLevel.right)
			{
				level.right = _recordingLeftoverLevel.right;
			}
			else if (level.right > 0 && level.right < -_recordingLeftoverLevel.right)
			{
				level.right = _recordingLeftoverLevel.right;
			}
			_waveBuffer.data[_framesRendered] = level;
			_framesRendered++;
			CheckWaveBufferSize();
			num = num3 / bps;
		}
		for (int i = 0; i < num6; i++)
		{
			WaveBuffer.Level level = bps switch
			{
				2 => GetLevel(_peakLevelShort, _waveBuffer.chans, num, num2), 
				4 => GetLevel(_peakLevelFloat, _waveBuffer.chans, num, num2), 
				_ => GetLevel(_peakLevelByte, _waveBuffer.chans, num, num2), 
			};
			num += num2;
			lock (_syncRoot)
			{
				_waveBuffer.data[_framesRendered] = level;
				_framesRendered++;
			}
			CheckWaveBufferSize();
		}
		_recordingLeftoverBytes = (num4 - num3) % _waveBuffer.bpf;
		if (_recordingLeftoverBytes > 0)
		{
			switch (bps)
			{
			case 2:
				_recordingLeftoverLevel = GetLevel(_peakLevelShort, _waveBuffer.chans, num, _recordingLeftoverBytes / bps);
				break;
			case 4:
				_recordingLeftoverLevel = GetLevel(_peakLevelFloat, _waveBuffer.chans, num, _recordingLeftoverBytes / bps);
				break;
			default:
				_recordingLeftoverLevel = GetLevel(_peakLevelByte, _waveBuffer.chans, num, _recordingLeftoverBytes / bps);
				break;
			}
		}
		else
		{
			_recordingLeftoverLevel.left = 0;
			_recordingLeftoverLevel.right = 0;
		}
	}

	public void RenderRecording(IntPtr buffer, int length)
	{
		if (_waveBuffer == null || _isRendered || buffer == IntPtr.Zero || length <= 0)
		{
			return;
		}
		if (Bass.BASS_ChannelIsActive(_decodingStream) == BASSActive.BASS_ACTIVE_STOPPED)
		{
			RenderStopRecording();
			return;
		}
		int num = 0;
		int bps = _waveBuffer.bps;
		int num2 = _waveBuffer.bpf / bps;
		int num3 = 0;
		if (_recordingLeftoverBytes > 0)
		{
			num3 = _waveBuffer.bpf - _recordingLeftoverBytes;
		}
		int num4 = (length - num3) / _waveBuffer.bpf;
		if (_recordingLeftoverBytes > 0)
		{
			WaveBuffer.Level level = GetLevel(buffer, _waveBuffer.chans, bps, 0, num3 / bps);
			if (_recordingLeftoverLevel.left == short.MinValue)
			{
				level.left = _recordingLeftoverLevel.left;
			}
			else if (level.left < 0 && _recordingLeftoverLevel.left < 0 && level.left > _recordingLeftoverLevel.left)
			{
				level.left = _recordingLeftoverLevel.left;
			}
			else if (level.left > 0 && _recordingLeftoverLevel.left > 0 && level.left < _recordingLeftoverLevel.left)
			{
				level.left = _recordingLeftoverLevel.left;
			}
			else if (level.left < 0 && level.left > -_recordingLeftoverLevel.left)
			{
				level.left = _recordingLeftoverLevel.left;
			}
			else if (level.left > 0 && level.left < -_recordingLeftoverLevel.left)
			{
				level.left = _recordingLeftoverLevel.left;
			}
			if (_recordingLeftoverLevel.right == short.MinValue)
			{
				level.right = _recordingLeftoverLevel.right;
			}
			else if (level.right < 0 && _recordingLeftoverLevel.right < 0 && level.right > _recordingLeftoverLevel.right)
			{
				level.right = _recordingLeftoverLevel.right;
			}
			else if (level.right > 0 && _recordingLeftoverLevel.right > 0 && level.right < _recordingLeftoverLevel.right)
			{
				level.right = _recordingLeftoverLevel.right;
			}
			else if (level.right < 0 && level.right > -_recordingLeftoverLevel.right)
			{
				level.right = _recordingLeftoverLevel.right;
			}
			else if (level.right > 0 && level.right < -_recordingLeftoverLevel.right)
			{
				level.right = _recordingLeftoverLevel.right;
			}
			lock (_syncRoot)
			{
				_waveBuffer.data[_framesRendered] = level;
				_framesRendered++;
			}
			CheckWaveBufferSize();
			num = num3 / bps;
		}
		for (int i = 0; i < num4; i++)
		{
			WaveBuffer.Level level = GetLevel(buffer, _waveBuffer.chans, bps, num, num2);
			num += num2;
			lock (_syncRoot)
			{
				_waveBuffer.data[_framesRendered] = level;
				_framesRendered++;
			}
			CheckWaveBufferSize();
		}
		_recordingLeftoverBytes = (length - num3) % _waveBuffer.bpf;
		if (_recordingLeftoverBytes > 0)
		{
			_recordingLeftoverLevel = GetLevel(buffer, _waveBuffer.chans, bps, num, _recordingLeftoverBytes / bps);
			return;
		}
		_recordingLeftoverLevel.left = 0;
		_recordingLeftoverLevel.right = 0;
	}

	private void CheckWaveBufferSize()
	{
		if (_framesRendered < _framesToRender)
		{
			return;
		}
		if (_recordingNextLength > 0)
		{
			lock (_syncRoot)
			{
				_framesToRender += _recordingNextLength;
				WaveBuffer.Level[] array = new WaveBuffer.Level[_framesToRender];
				_waveBuffer.data.CopyTo(array, 0);
				_waveBuffer.data = array;
				return;
			}
		}
		_framesRendered = 0;
	}

	public bool SyncPlayback(int channel)
	{
		if (_waveBuffer == null)
		{
			return false;
		}
		BASS_CHANNELINFO bASS_CHANNELINFO = new BASS_CHANNELINFO();
		if (Bass.BASS_ChannelGetInfo(channel, bASS_CHANNELINFO))
		{
			double num = 2.0;
			if (bASS_CHANNELINFO.Is32bit)
			{
				num = 4.0;
			}
			else if (bASS_CHANNELINFO.Is8bit)
			{
				num = 1.0;
			}
			_channelFactor = (_syncFactor = (double)_waveBuffer.bps / num);
			_correctionFactor = Bass.BASS_ChannelBytes2Seconds(channel, _waveBuffer.bpf) / _syncFactor / _waveBuffer.resolution;
			return true;
		}
		return false;
	}

	public long Position2Rendering(long bytes)
	{
		long num = (long)((double)bytes * _syncFactor);
		if (bytes > 0 && num < 0)
		{
			num = long.MaxValue;
		}
		else if (bytes < 0 && num > 0)
		{
			num = long.MinValue;
		}
		return num;
	}

	public long Position2Rendering(double seconds)
	{
		long num = -1L;
		if (_waveBuffer != null)
		{
			num = (long)(seconds * ((double)_waveBuffer.bpf / _waveBuffer.resolution) / _correctionFactor);
			if (seconds > 0.0 && num < 0)
			{
				num = long.MaxValue;
			}
			else if (seconds < 0.0 && num > 0)
			{
				num = long.MinValue;
			}
		}
		return num;
	}

	public long Position2Playback(long bytes)
	{
		return (long)((double)bytes / _syncFactor);
	}

	public long Position2Playback(double seconds)
	{
		long num = -1L;
		if (_waveBuffer != null)
		{
			num = (long)(seconds * ((double)_waveBuffer.bpf / _waveBuffer.resolution) / _correctionFactor);
			if (seconds > 0.0 && num < 0)
			{
				num = long.MaxValue;
			}
			else if (seconds < 0.0 && num > 0)
			{
				num = long.MinValue;
			}
		}
		return num;
	}

	public int Position2Frames(long bytes)
	{
		int num = -1;
		if (_waveBuffer != null)
		{
			num = (int)((double)bytes * _syncFactor / (double)_waveBuffer.bpf);
			if (_waveBuffer.data != null && num >= _waveBuffer.data.Length)
			{
				num = _waveBuffer.data.Length - 1;
			}
		}
		return num;
	}

	public int Position2Frames(double seconds)
	{
		int num = -1;
		if (_waveBuffer != null)
		{
			num = (int)(seconds / _waveBuffer.resolution / _correctionFactor);
			if (seconds > 0.0 && num <= 0)
			{
				num = int.MaxValue;
			}
			else if (seconds < 0.0 && num > 0)
			{
				num = int.MinValue;
			}
			if (_waveBuffer.data != null && num >= _waveBuffer.data.Length)
			{
				num = _waveBuffer.data.Length - 1;
			}
		}
		return num;
	}

	public long Frame2Bytes(int frame)
	{
		double num = -1.0;
		if (_waveBuffer != null)
		{
			num = (double)((long)frame * (long)_waveBuffer.bpf) / _syncFactor;
		}
		return (long)(num + 0.5);
	}

	public double Frame2Seconds(int frame)
	{
		double result = -1.0;
		if (_waveBuffer != null)
		{
			result = (double)frame * _waveBuffer.resolution * _correctionFactor;
		}
		return result;
	}

	private string FindMarker(string name)
	{
		if (_waveBuffer == null || _waveBuffer.marker == null)
		{
			return null;
		}
		int num = name.IndexOf('{');
		if (num > 0)
		{
			string value = name.Substring(0, num);
			foreach (string key in _waveBuffer.marker.Keys)
			{
				if (key.StartsWith(value))
				{
					name = key;
					break;
				}
			}
		}
		return name;
	}

	public bool AddMarker(string name, long position)
	{
		bool result = false;
		lock (_syncRoot)
		{
			if (_waveBuffer != null && !string.IsNullOrEmpty(name))
			{
				if (_waveBuffer.marker == null)
				{
					_waveBuffer.marker = new Dictionary<string, long>();
				}
				if (_waveBuffer.marker != null)
				{
					position = Position2Rendering(position);
					string text = FindMarker(name);
					if (_waveBuffer.marker.ContainsKey(text))
					{
						if (text != name)
						{
							_waveBuffer.marker.Remove(text);
							_waveBuffer.marker.Add(name, position);
						}
						else
						{
							_waveBuffer.marker[name] = position;
						}
						result = true;
					}
					else
					{
						_waveBuffer.marker.Add(name, position);
						result = true;
					}
				}
			}
		}
		return result;
	}

	public bool AddMarker(string name, double position)
	{
		bool result = false;
		lock (_syncRoot)
		{
			if (_waveBuffer != null && !string.IsNullOrEmpty(name))
			{
				if (_waveBuffer.marker == null)
				{
					_waveBuffer.marker = new Dictionary<string, long>();
				}
				if (_waveBuffer.marker != null)
				{
					long value = Position2Rendering(position);
					name = FindMarker(name);
					if (_waveBuffer.marker.ContainsKey(name))
					{
						_waveBuffer.marker[name] = value;
						result = true;
					}
					else
					{
						_waveBuffer.marker.Add(name, value);
						result = true;
					}
				}
			}
		}
		return result;
	}

	public long GetMarker(string name)
	{
		long result = -1L;
		lock (_syncRoot)
		{
			if (_waveBuffer != null && _waveBuffer.marker != null && !string.IsNullOrEmpty(name))
			{
				try
				{
					name = FindMarker(name);
					result = Position2Playback(_waveBuffer.marker[name]);
				}
				catch
				{
					result = -1L;
				}
			}
		}
		return result;
	}

	public double GetMarkerSec(string name)
	{
		double result = -1.0;
		lock (_syncRoot)
		{
			if (_waveBuffer != null && _waveBuffer.marker != null && !string.IsNullOrEmpty(name))
			{
				try
				{
					name = FindMarker(name);
					result = (double)_waveBuffer.marker[name] / ((double)_waveBuffer.bpf / _waveBuffer.resolution);
				}
				catch
				{
					result = -1.0;
				}
			}
		}
		return result;
	}

	public int GetMarkerCount()
	{
		int result = 0;
		lock (_syncRoot)
		{
			if (_waveBuffer != null && _waveBuffer.marker != null)
			{
				result = _waveBuffer.marker.Count;
			}
		}
		return result;
	}

	public string[] GetMarkers()
	{
		string[] array = null;
		lock (_syncRoot)
		{
			if (_waveBuffer != null && _waveBuffer.marker != null)
			{
				array = new string[_waveBuffer.marker.Count];
				_waveBuffer.marker.Keys.CopyTo(array, 0);
			}
			else
			{
				array = new string[0];
			}
		}
		return array;
	}

	public bool RemoveMarker(string name)
	{
		bool result = false;
		lock (_syncRoot)
		{
			if (_waveBuffer != null && _waveBuffer.marker != null && !string.IsNullOrEmpty(name))
			{
				name = FindMarker(name);
				if (_waveBuffer.marker.ContainsKey(name))
				{
					_waveBuffer.marker.Remove(name);
					result = true;
				}
			}
		}
		return result;
	}

	public bool ClearAllMarker()
	{
		bool result = false;
		lock (_syncRoot)
		{
			if (_waveBuffer != null)
			{
				if (_waveBuffer.marker != null)
				{
					_waveBuffer.marker.Clear();
				}
				_waveBuffer.marker = null;
				result = true;
			}
		}
		return result;
	}

	public int SearchVolumePoint(long position)
	{
		lock (_syncRoot)
		{
			if (_volumePoints != null)
			{
				return _volumePoints.BinarySearch(new VolumePoint(Position2Rendering(position), 1f));
			}
			return 0;
		}
	}

	public int SearchVolumePoint(double position)
	{
		lock (_syncRoot)
		{
			if (_volumePoints != null)
			{
				return _volumePoints.BinarySearch(new VolumePoint(Position2Rendering(position), 1f));
			}
			return 0;
		}
	}

	public int SearchVolumePoint(long position, ref VolumePoint prev, ref VolumePoint next)
	{
		int num = 0;
		lock (_syncRoot)
		{
			if (_volumePoints != null)
			{
				num = _volumePoints.BinarySearch(new VolumePoint(Position2Rendering(position), 1f));
				if (num >= 0)
				{
					prev = new VolumePoint(Position2Playback(_volumePoints[num].Position), _volumePoints[num].Level);
					if (num < _volumePoints.Count - 1)
					{
						next = new VolumePoint(Position2Playback(_volumePoints[num + 1].Position), _volumePoints[num + 1].Level);
					}
					else
					{
						next = null;
					}
				}
				else
				{
					int num2 = ~num;
					if (num2 > 0)
					{
						prev = new VolumePoint(Position2Playback(_volumePoints[num2 - 1].Position), _volumePoints[num2 - 1].Level);
					}
					else
					{
						prev = null;
					}
					if (num2 < _volumePoints.Count)
					{
						next = new VolumePoint(Position2Playback(_volumePoints[num2].Position), _volumePoints[num2].Level);
					}
					else
					{
						next = null;
					}
				}
			}
		}
		return num;
	}

	public int SearchVolumePoint(double position, ref VolumePoint prev, ref VolumePoint next)
	{
		int num = 0;
		lock (_syncRoot)
		{
			if (_volumePoints != null)
			{
				num = _volumePoints.BinarySearch(new VolumePoint(Position2Rendering(position), 1f));
				if (num >= 0)
				{
					prev = new VolumePoint(Position2Playback(_volumePoints[num].Position), _volumePoints[num].Level);
					if (num < _volumePoints.Count - 1)
					{
						next = new VolumePoint(Position2Playback(_volumePoints[num + 1].Position), _volumePoints[num + 1].Level);
					}
					else
					{
						next = null;
					}
				}
				else
				{
					int num2 = ~num;
					if (num2 > 0)
					{
						prev = new VolumePoint(Position2Playback(_volumePoints[num2 - 1].Position), _volumePoints[num2 - 1].Level);
					}
					else
					{
						prev = null;
					}
					if (num2 < _volumePoints.Count)
					{
						next = new VolumePoint(Position2Playback(_volumePoints[num2].Position), _volumePoints[num2].Level);
					}
					else
					{
						next = null;
					}
				}
			}
		}
		return num;
	}

	public bool AddVolumePoint(long position, float level)
	{
		bool result = false;
		lock (_syncRoot)
		{
			if (_volumePoints == null)
			{
				_volumePoints = new List<VolumePoint>();
			}
			if (_volumePoints != null)
			{
				int num = SearchVolumePoint(position);
				if (num >= 0)
				{
					_volumePoints[num].Level = level;
					result = true;
				}
				else
				{
					position = Position2Rendering(position);
					_volumePoints.Insert(~num, new VolumePoint(position, level));
					result = true;
				}
			}
		}
		return result;
	}

	public bool AddVolumePoint(double position, float level)
	{
		bool result = false;
		lock (_syncRoot)
		{
			if (_volumePoints == null)
			{
				_volumePoints = new List<VolumePoint>();
			}
			if (_volumePoints != null)
			{
				int num = SearchVolumePoint(position);
				if (num >= 0)
				{
					_volumePoints[num].Level = level;
					result = true;
				}
				else
				{
					long position2 = Position2Rendering(position);
					_volumePoints.Insert(~num, new VolumePoint(position2, level));
					result = true;
				}
			}
		}
		return result;
	}

	public float GetVolumePoint(long position)
	{
		float result = -1f;
		lock (_syncRoot)
		{
			if (_volumePoints != null)
			{
				int num = SearchVolumePoint(position);
				if (num >= 0)
				{
					result = _volumePoints[num].Level;
				}
			}
		}
		return result;
	}

	public float GetVolumePoint(double position)
	{
		float result = -1f;
		lock (_syncRoot)
		{
			if (_volumePoints != null)
			{
				int num = SearchVolumePoint(position);
				if (num >= 0)
				{
					result = _volumePoints[num].Level;
				}
			}
		}
		return result;
	}

	public VolumePoint GetVolumePoint(int index)
	{
		lock (_syncRoot)
		{
			if (_volumePoints != null && index >= 0 && index < _volumePoints.Count)
			{
				return new VolumePoint(Position2Playback(_volumePoints[index].Position), _volumePoints[index].Level);
			}
			return null;
		}
	}

	public float GetVolumeLevel(long position, bool reverse, ref long duration, ref float nextLevel)
	{
		float result = (nextLevel = 1f);
		duration = 0L;
		lock (_syncRoot)
		{
			if (_volumePoints != null)
			{
				VolumePoint prev = null;
				VolumePoint next = null;
				if (SearchVolumePoint(position, ref prev, ref next) >= 0)
				{
					if (reverse)
					{
						SearchVolumePoint(position - 1, ref prev, ref next);
						if (prev != null)
						{
							nextLevel = prev.Level;
							duration = next.Position - prev.Position;
						}
						else
						{
							nextLevel = (_volumeCurveZeroLevel ? 0f : 1f);
							duration = next.Position;
						}
						return next.Level;
					}
					if (next != null)
					{
						nextLevel = next.Level;
						duration = next.Position - prev.Position;
					}
					else
					{
						nextLevel = (_volumeCurveZeroLevel ? 0f : 1f);
						duration = Frame2Bytes(FramesRendered - 1) - prev.Position;
					}
					return prev.Level;
				}
				if (_volumePoints.Count > 0)
				{
					long num = 0L;
					float num2 = (_volumeCurveZeroLevel ? 0f : 1f);
					long num3 = Frame2Bytes(FramesRendered - 1);
					float num4 = (_volumeCurveZeroLevel ? 0f : 1f);
					if (prev != null)
					{
						num = prev.Position;
						num2 = prev.Level;
					}
					if (next != null)
					{
						num3 = next.Position;
						num4 = next.Level;
					}
					if (reverse)
					{
						duration = position - num;
						nextLevel = num2;
					}
					else
					{
						duration = num3 - position;
						nextLevel = num4;
					}
					result = ((num4 > num2) ? (num2 + (num4 - num2) * ((float)(position - num) / (float)(num3 - num))) : ((!(num4 < num2)) ? num2 : (num4 + (num2 - num4) * ((float)(num3 - position) / (float)(num3 - num)))));
				}
			}
		}
		return result;
	}

	public float GetVolumeLevel(double position, bool reverse, ref double duration, ref float nextLevel)
	{
		float result = (nextLevel = 1f);
		duration = 0.0;
		lock (_syncRoot)
		{
			if (_volumePoints != null)
			{
				VolumePoint prev = null;
				VolumePoint next = null;
				if (SearchVolumePoint(position, ref prev, ref next) >= 0)
				{
					if (reverse)
					{
						SearchVolumePoint(position - 0.0001, ref prev, ref next);
						if (prev != null)
						{
							nextLevel = prev.Level;
							duration = (double)(next.Position - prev.Position) / ((double)_waveBuffer.bpf / _waveBuffer.resolution);
						}
						else
						{
							nextLevel = (_volumeCurveZeroLevel ? 0f : 1f);
							duration = (double)next.Position / ((double)_waveBuffer.bpf / _waveBuffer.resolution);
						}
						return next.Level;
					}
					if (next != null)
					{
						nextLevel = next.Level;
						duration = (double)(next.Position - prev.Position) / ((double)_waveBuffer.bpf / _waveBuffer.resolution);
					}
					else
					{
						nextLevel = (_volumeCurveZeroLevel ? 0f : 1f);
						duration = (double)(Frame2Bytes(FramesRendered - 1) - prev.Position) / ((double)_waveBuffer.bpf / _waveBuffer.resolution);
					}
					return prev.Level;
				}
				if (_volumePoints.Count > 0)
				{
					long num = 0L;
					float num2 = (_volumeCurveZeroLevel ? 0f : 1f);
					long num3 = Frame2Bytes(FramesRendered - 1);
					float num4 = (_volumeCurveZeroLevel ? 0f : 1f);
					if (prev != null)
					{
						num = prev.Position;
						num2 = prev.Level;
					}
					if (next != null)
					{
						num3 = next.Position;
						num4 = next.Level;
					}
					if (reverse)
					{
						duration = (position - (double)num) / ((double)_waveBuffer.bpf / _waveBuffer.resolution);
						nextLevel = num2;
					}
					else
					{
						duration = ((double)num3 - position) / ((double)_waveBuffer.bpf / _waveBuffer.resolution);
						nextLevel = num4;
					}
					result = ((num4 > num2) ? (num2 + (num4 - num2) * ((float)(position - (double)num) / (float)(num3 - num))) : ((!(num4 < num2)) ? num2 : (num4 + (num2 - num4) * ((float)((double)num3 - position) / (float)(num3 - num)))));
				}
			}
		}
		return result;
	}

	public int GetVolumePointCount()
	{
		int result = 0;
		lock (_syncRoot)
		{
			if (_volumePoints != null)
			{
				result = _volumePoints.Count;
			}
		}
		return result;
	}

	public bool RemoveVolumePoint(long position)
	{
		bool result = false;
		lock (_syncRoot)
		{
			if (_volumePoints != null)
			{
				int num = SearchVolumePoint(position);
				if (num >= 0)
				{
					_volumePoints.RemoveAt(num);
					result = true;
				}
			}
		}
		return result;
	}

	public bool RemoveVolumePoint(double position)
	{
		bool result = false;
		lock (_syncRoot)
		{
			if (_volumePoints != null)
			{
				int num = SearchVolumePoint(position);
				if (num >= 0)
				{
					_volumePoints.RemoveAt(num);
					result = true;
				}
			}
		}
		return result;
	}

	public void RemoveVolumePointsBetween(long from, long to)
	{
		if (_volumePoints == null)
		{
			return;
		}
		lock (_syncRoot)
		{
			int num = _volumePoints.BinarySearch(new VolumePoint(Position2Rendering(from), 1f));
			if (num < 0)
			{
				num = ~num;
				if (num >= _volumePoints.Count)
				{
					return;
				}
			}
			else
			{
				if (num >= _volumePoints.Count - 1)
				{
					return;
				}
				num++;
			}
			int num2 = _volumePoints.BinarySearch(new VolumePoint(Position2Rendering(to), 1f));
			if (num2 < 0)
			{
				num2 = ~num2;
				if (num2 > _volumePoints.Count)
				{
					num2 = _volumePoints.Count;
				}
				if (num2 == 0)
				{
					return;
				}
				num2--;
			}
			else
			{
				if (num2 == 0)
				{
					return;
				}
				num2--;
			}
			for (int num3 = num2; num3 >= num; num3--)
			{
				_volumePoints.RemoveAt(num3);
			}
		}
	}

	public void RemoveVolumePointsBetween(double from, double to)
	{
		if (_volumePoints == null)
		{
			return;
		}
		lock (_syncRoot)
		{
			int num = _volumePoints.BinarySearch(new VolumePoint(Position2Rendering(from), 1f));
			if (num < 0)
			{
				num = ~num;
				if (num >= _volumePoints.Count)
				{
					return;
				}
			}
			else
			{
				if (num >= _volumePoints.Count - 1)
				{
					return;
				}
				num++;
			}
			int num2 = _volumePoints.BinarySearch(new VolumePoint(Position2Rendering(to), 1f));
			if (num2 < 0)
			{
				num2 = ~num2;
				if (num2 > _volumePoints.Count)
				{
					num2 = _volumePoints.Count;
				}
				if (num2 == 0)
				{
					return;
				}
				num2--;
			}
			else
			{
				if (num2 == 0)
				{
					return;
				}
				num2--;
			}
			for (int num3 = num2; num3 >= num; num3--)
			{
				_volumePoints.RemoveAt(num3);
			}
		}
	}

	public bool ClearAllVolumePoints()
	{
		lock (_syncRoot)
		{
			if (_volumePoints != null)
			{
				_volumePoints.Clear();
			}
			_volumePoints = null;
		}
		return true;
	}

	public Bitmap CreateBitmap(int width, int height, int frameStart, int frameEnd, bool highQuality)
	{
		if (width <= 1 || height <= 1 || frameStart > frameEnd || _waveBuffer == null)
		{
			return null;
		}
		Bitmap bitmap = null;
		WAVEFORMDRAWTYPE wAVEFORMDRAWTYPE = _drawWaveForm;
		if (_waveBuffer.chans == 1 && (wAVEFORMDRAWTYPE == WAVEFORMDRAWTYPE.Stereo || wAVEFORMDRAWTYPE == WAVEFORMDRAWTYPE.DualMono))
		{
			wAVEFORMDRAWTYPE = WAVEFORMDRAWTYPE.Mono;
		}
		if (DrawGradient)
		{
			Graphics graphics = null;
			Pen pen = null;
			Pen pen2 = null;
			Pen pen3 = null;
			Pen pen4 = null;
			Pen pen5 = null;
			Pen pen6 = null;
			Pen pen7 = null;
			Pen pen8 = null;
			Pen pen9 = null;
			Pen pen10 = null;
			Pen pen11 = null;
			try
			{
				float num = (float)height / 2f - 1f;
				float num2 = ((float)height - 1f) / 4f;
				RectangleF rect = Rectangle.Empty;
				RectangleF rect2 = Rectangle.Empty;
				RectangleF rectangleF = Rectangle.Empty;
				RectangleF rectangleF2 = Rectangle.Empty;
				switch (wAVEFORMDRAWTYPE)
				{
				case WAVEFORMDRAWTYPE.Stereo:
					rect = new RectangleF(0f, -1f, width, num2 + 1.1f);
					rect2 = new RectangleF(0f, num2 - 1.1f, width, num2 + 1.1f);
					rectangleF = new RectangleF(0f, num2 + num2 - 1.1f, width, num2 + 1.1f);
					rectangleF2 = new RectangleF(0f, num2 + num2 + num2 - 1.1f, width, num2 + 1.1f);
					break;
				case WAVEFORMDRAWTYPE.Mono:
				case WAVEFORMDRAWTYPE.DualMono:
					rectangleF = new RectangleF(0f, -1f, width, num + 1.1f);
					rect = rectangleF;
					rectangleF2 = new RectangleF(0f, num - 1.1f, width, num + 1.1f);
					rect2 = rectangleF2;
					break;
				case WAVEFORMDRAWTYPE.HalfMono:
				case WAVEFORMDRAWTYPE.HalfMonoFlipped:
					rectangleF2 = new RectangleF(0f, -1f, width, (float)height + 1.1f);
					rect = (rectangleF = (rect2 = rectangleF2));
					break;
				}
				using Brush brush = new LinearGradientBrush(rect, ColorLeft, ColorLeft2, (wAVEFORMDRAWTYPE == WAVEFORMDRAWTYPE.HalfMonoFlipped) ? 90f : 270f);
				using Brush brush2 = new LinearGradientBrush(rect2, ColorLeft, ColorLeft2, (wAVEFORMDRAWTYPE == WAVEFORMDRAWTYPE.HalfMonoFlipped) ? 270f : 90f);
				using Brush brush3 = new LinearGradientBrush(rectangleF, ColorRight, ColorRight2, (wAVEFORMDRAWTYPE == WAVEFORMDRAWTYPE.HalfMonoFlipped) ? 90f : 270f);
				using Brush brush4 = new LinearGradientBrush(rectangleF2, ColorRight, ColorRight2, (wAVEFORMDRAWTYPE == WAVEFORMDRAWTYPE.HalfMonoFlipped) ? 270f : 90f);
				pen3 = new Pen(brush);
				pen4 = new Pen(brush2);
				pen5 = new Pen(brush3);
				pen6 = new Pen(brush4);
				pen7 = new Pen(ColorLeftEnvelope, 1f);
				pen8 = new Pen(ColorRightEnvelope, 1f);
				pen9 = new Pen(ColorMarker, 1f);
				pen10 = new Pen(ColorBeat, _beatWidth);
				pen11 = new Pen(ColorVolume, 1f);
				if ((_drawVolume & VOLUMEDRAWTYPE.Dotted) != 0)
				{
					pen11.DashStyle = DashStyle.Dash;
				}
				pen = ((!(ColorMiddleLeft == Color.Empty)) ? new Pen(ColorMiddleLeft, 1f) : pen7);
				pen2 = ((!(ColorMiddleRight == Color.Empty)) ? new Pen(ColorMiddleRight, 1f) : pen8);
				bitmap = new Bitmap(width, height, _pixelFormat);
				graphics = Graphics.FromImage(bitmap);
				if (highQuality)
				{
					graphics.SmoothingMode = SmoothingMode.AntiAlias;
					graphics.CompositingQuality = CompositingQuality.AssumeLinear;
					graphics.PixelOffsetMode = PixelOffsetMode.Default;
					graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				}
				else
				{
					graphics.SmoothingMode = SmoothingMode.Default;
					graphics.CompositingQuality = CompositingQuality.Default;
					graphics.PixelOffsetMode = PixelOffsetMode.Default;
					graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
				}
				DrawBitmap(graphics, width, height, frameStart, frameEnd, pen, pen2, pen3, pen4, pen7, pen5, pen6, pen8, pen9, pen10, pen11);
			}
			catch
			{
				bitmap = null;
			}
			finally
			{
				pen3?.Dispose();
				pen4?.Dispose();
				pen7?.Dispose();
				pen5?.Dispose();
				pen6?.Dispose();
				pen8?.Dispose();
				if (ColorMiddleLeft != Color.Empty)
				{
					pen?.Dispose();
				}
				if (ColorMiddleRight != Color.Empty)
				{
					pen2?.Dispose();
				}
				pen9?.Dispose();
				pen10?.Dispose();
				pen11?.Dispose();
				graphics?.Dispose();
			}
		}
		else
		{
			Graphics graphics2 = null;
			Pen pen12 = null;
			Pen pen13 = null;
			Pen pen14 = null;
			Pen pen15 = null;
			Pen pen16 = null;
			Pen pen17 = null;
			Pen pen18 = null;
			Pen pen19 = null;
			Pen pen20 = null;
			try
			{
				_ = height / 2;
				using Brush brush5 = new SolidBrush(ColorLeft);
				using Brush brush6 = new SolidBrush(ColorRight);
				pen14 = new Pen(brush5);
				pen15 = new Pen(brush6);
				pen16 = new Pen(ColorLeftEnvelope, 1f);
				pen17 = new Pen(ColorRightEnvelope, 1f);
				pen18 = new Pen(ColorMarker, 1f);
				pen19 = new Pen(ColorBeat, _beatWidth);
				pen20 = new Pen(ColorVolume, 1f);
				if ((_drawVolume & VOLUMEDRAWTYPE.Dotted) != 0)
				{
					pen20.DashStyle = DashStyle.Dash;
				}
				pen12 = ((!(ColorMiddleLeft == Color.Empty)) ? new Pen(ColorMiddleLeft, 1f) : pen14);
				pen13 = ((!(ColorMiddleRight == Color.Empty)) ? new Pen(ColorMiddleRight, 1f) : pen15);
				bitmap = new Bitmap(width, height, _pixelFormat);
				graphics2 = Graphics.FromImage(bitmap);
				if (highQuality)
				{
					graphics2.SmoothingMode = SmoothingMode.AntiAlias;
					graphics2.CompositingQuality = CompositingQuality.AssumeLinear;
					graphics2.PixelOffsetMode = PixelOffsetMode.Default;
					graphics2.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				}
				else
				{
					graphics2.SmoothingMode = SmoothingMode.Default;
					graphics2.CompositingQuality = CompositingQuality.Default;
					graphics2.PixelOffsetMode = PixelOffsetMode.Default;
					graphics2.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
				}
				DrawBitmap(graphics2, width, height, frameStart, frameEnd, pen12, pen13, pen14, pen16, pen15, pen17, pen18, pen19, pen20);
			}
			catch
			{
				bitmap = null;
			}
			finally
			{
				pen14?.Dispose();
				pen16?.Dispose();
				pen15?.Dispose();
				pen17?.Dispose();
				if (ColorMiddleLeft != Color.Empty)
				{
					pen12?.Dispose();
				}
				if (ColorMiddleRight != Color.Empty)
				{
					pen13?.Dispose();
				}
				pen18?.Dispose();
				pen19?.Dispose();
				pen20?.Dispose();
				graphics2?.Dispose();
			}
		}
		return bitmap;
	}

	public bool CreateBitmap(Graphics g, Rectangle clipRectangle, int frameStart, int frameEnd, bool highQuality)
	{
		if (g == null || clipRectangle.Width <= 1 || clipRectangle.Height <= 1 || frameStart > frameEnd || _waveBuffer == null)
		{
			return false;
		}
		bool result = true;
		WAVEFORMDRAWTYPE wAVEFORMDRAWTYPE = _drawWaveForm;
		if (_waveBuffer.chans == 1 && (wAVEFORMDRAWTYPE == WAVEFORMDRAWTYPE.Stereo || wAVEFORMDRAWTYPE == WAVEFORMDRAWTYPE.DualMono))
		{
			wAVEFORMDRAWTYPE = WAVEFORMDRAWTYPE.Mono;
		}
		if (DrawGradient)
		{
			Pen pen = null;
			Pen pen2 = null;
			Pen pen3 = null;
			Pen pen4 = null;
			Pen pen5 = null;
			Pen pen6 = null;
			Pen pen7 = null;
			Pen pen8 = null;
			Pen pen9 = null;
			Pen pen10 = null;
			Pen pen11 = null;
			try
			{
				float num = (float)clipRectangle.Height / 2f - 1f;
				float num2 = ((float)clipRectangle.Height - 1f) / 4f;
				RectangleF rect = Rectangle.Empty;
				RectangleF rect2 = Rectangle.Empty;
				RectangleF rectangleF = Rectangle.Empty;
				RectangleF rectangleF2 = Rectangle.Empty;
				switch (wAVEFORMDRAWTYPE)
				{
				case WAVEFORMDRAWTYPE.Stereo:
					rect = new RectangleF(0f, -1f, clipRectangle.Width, num2 + 1.1f);
					rect2 = new RectangleF(0f, num2 - 1.1f, clipRectangle.Width, num2 + 1.1f);
					rectangleF = new RectangleF(0f, num2 + num2 - 1.1f, clipRectangle.Width, num2 + 1.1f);
					rectangleF2 = new RectangleF(0f, num2 + num2 + num2 - 1.1f, clipRectangle.Width, num2 + 1.1f);
					break;
				case WAVEFORMDRAWTYPE.Mono:
				case WAVEFORMDRAWTYPE.DualMono:
					rectangleF = new RectangleF(0f, -1f, clipRectangle.Width, num + 1.1f);
					rect = rectangleF;
					rectangleF2 = new RectangleF(0f, num - 1.1f, clipRectangle.Width, num + 1.1f);
					rect2 = rectangleF2;
					break;
				case WAVEFORMDRAWTYPE.HalfMono:
				case WAVEFORMDRAWTYPE.HalfMonoFlipped:
					rectangleF2 = new RectangleF(0f, -1f, clipRectangle.Width, (float)clipRectangle.Height + 1.1f);
					rect = (rectangleF = (rect2 = rectangleF2));
					break;
				}
				using Brush brush = new LinearGradientBrush(rect, ColorLeft, ColorLeft2, (wAVEFORMDRAWTYPE == WAVEFORMDRAWTYPE.HalfMonoFlipped) ? 90f : 270f);
				using Brush brush2 = new LinearGradientBrush(rect2, ColorLeft, ColorLeft2, (wAVEFORMDRAWTYPE == WAVEFORMDRAWTYPE.HalfMonoFlipped) ? 270f : 90f);
				using Brush brush3 = new LinearGradientBrush(rectangleF, ColorRight, ColorRight2, (wAVEFORMDRAWTYPE == WAVEFORMDRAWTYPE.HalfMonoFlipped) ? 90f : 270f);
				using Brush brush4 = new LinearGradientBrush(rectangleF2, ColorRight, ColorRight2, (wAVEFORMDRAWTYPE == WAVEFORMDRAWTYPE.HalfMonoFlipped) ? 270f : 90f);
				pen3 = new Pen(brush);
				pen4 = new Pen(brush2);
				pen5 = new Pen(brush3);
				pen6 = new Pen(brush4);
				pen7 = new Pen(ColorLeftEnvelope, 1f);
				pen8 = new Pen(ColorRightEnvelope, 1f);
				pen9 = new Pen(ColorMarker, 1f);
				pen10 = new Pen(ColorBeat, _beatWidth);
				pen11 = new Pen(ColorVolume, 1f);
				if ((_drawVolume & VOLUMEDRAWTYPE.Dotted) != 0)
				{
					pen11.DashStyle = DashStyle.Dash;
				}
				pen = ((!(ColorMiddleLeft == Color.Empty)) ? new Pen(ColorMiddleLeft, 1f) : pen7);
				pen2 = ((!(ColorMiddleRight == Color.Empty)) ? new Pen(ColorMiddleRight, 1f) : pen8);
				if (highQuality)
				{
					g.SmoothingMode = SmoothingMode.AntiAlias;
					g.CompositingQuality = CompositingQuality.AssumeLinear;
					g.PixelOffsetMode = PixelOffsetMode.Default;
					g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				}
				else
				{
					g.SmoothingMode = SmoothingMode.Default;
					g.CompositingQuality = CompositingQuality.Default;
					g.PixelOffsetMode = PixelOffsetMode.Default;
					g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
				}
				DrawBitmap(g, clipRectangle.Width, clipRectangle.Height, frameStart, frameEnd, pen, pen2, pen3, pen4, pen7, pen5, pen6, pen8, pen9, pen10, pen11);
			}
			catch
			{
				result = false;
			}
			finally
			{
				pen3?.Dispose();
				pen4?.Dispose();
				pen7?.Dispose();
				pen5?.Dispose();
				pen6?.Dispose();
				pen8?.Dispose();
				if (ColorMiddleLeft != Color.Empty)
				{
					pen?.Dispose();
				}
				if (ColorMiddleRight != Color.Empty)
				{
					pen2?.Dispose();
				}
				pen9?.Dispose();
				pen10?.Dispose();
				pen11?.Dispose();
			}
		}
		else
		{
			Pen pen12 = null;
			Pen pen13 = null;
			Pen pen14 = null;
			Pen pen15 = null;
			Pen pen16 = null;
			Pen pen17 = null;
			Pen pen18 = null;
			Pen pen19 = null;
			Pen pen20 = null;
			try
			{
				_ = clipRectangle.Height / 2;
				using Brush brush5 = new SolidBrush(ColorLeft);
				using Brush brush6 = new SolidBrush(ColorRight);
				pen14 = new Pen(brush5);
				pen15 = new Pen(brush6);
				pen16 = new Pen(ColorLeftEnvelope, 1f);
				pen17 = new Pen(ColorRightEnvelope, 1f);
				pen18 = new Pen(ColorMarker, 1f);
				pen19 = new Pen(ColorBeat, _beatWidth);
				pen20 = new Pen(ColorVolume, 1f);
				if ((_drawVolume & VOLUMEDRAWTYPE.Dotted) != 0)
				{
					pen20.DashStyle = DashStyle.Dash;
				}
				pen12 = ((!(ColorMiddleLeft == Color.Empty)) ? new Pen(ColorMiddleLeft, 1f) : pen14);
				pen13 = ((!(ColorMiddleRight == Color.Empty)) ? new Pen(ColorMiddleRight, 1f) : pen15);
				if (highQuality)
				{
					g.SmoothingMode = SmoothingMode.AntiAlias;
					g.CompositingQuality = CompositingQuality.AssumeLinear;
					g.PixelOffsetMode = PixelOffsetMode.Default;
					g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				}
				else
				{
					g.SmoothingMode = SmoothingMode.Default;
					g.CompositingQuality = CompositingQuality.Default;
					g.PixelOffsetMode = PixelOffsetMode.Default;
					g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
				}
				DrawBitmap(g, clipRectangle.Width, clipRectangle.Height, frameStart, frameEnd, pen12, pen13, pen14, pen16, pen15, pen17, pen18, pen19, pen20);
			}
			catch
			{
				result = false;
			}
			finally
			{
				pen14?.Dispose();
				pen16?.Dispose();
				pen15?.Dispose();
				pen17?.Dispose();
				if (ColorMiddleLeft != Color.Empty)
				{
					pen12?.Dispose();
				}
				if (ColorMiddleRight != Color.Empty)
				{
					pen13?.Dispose();
				}
				pen18?.Dispose();
				pen19?.Dispose();
				pen20?.Dispose();
			}
		}
		return result;
	}

	public long GetBytePositionFromX(int x, int graphicsWidth, int frameStart, int frameEnd)
	{
		if (_waveBuffer == null)
		{
			return -1L;
		}
		if (frameEnd >= _waveBuffer.data.Length)
		{
			frameEnd = _waveBuffer.data.Length - 1;
		}
		if (frameEnd < 0)
		{
			frameEnd = _waveBuffer.data.Length - 1;
		}
		if (frameStart < 0)
		{
			frameStart = 0;
		}
		if (graphicsWidth == 0)
		{
			graphicsWidth = 1;
		}
		if (x >= graphicsWidth)
		{
			x = graphicsWidth - 1;
		}
		long num = Frame2Bytes(frameStart);
		double num2 = (double)(Frame2Bytes(frameEnd) - num) / (double)graphicsWidth;
		return (long)((double)x * num2) + num;
	}

	public bool GetCuePoints(ref long startpos, ref long endpos, double threshold)
	{
		if (!_isRendered || _waveBuffer == null)
		{
			return false;
		}
		int startpos2 = 0;
		int endpos2 = _waveBuffer.data.Length - 1;
		DetectSilence(ref startpos2, ref endpos2, threshold, threshold, -1, -1, -1);
		startpos = Frame2Bytes(startpos2);
		endpos = Frame2Bytes(endpos2);
		return true;
	}

	public bool GetCuePoints(ref long startpos, ref long endpos, double thresholdIn, double thresholdOut)
	{
		if (!_isRendered || _waveBuffer == null)
		{
			return false;
		}
		int startpos2 = 0;
		int endpos2 = _waveBuffer.data.Length - 1;
		DetectSilence(ref startpos2, ref endpos2, thresholdIn, thresholdOut, -1, -1, -1);
		startpos = Frame2Bytes(startpos2);
		endpos = Frame2Bytes(endpos2);
		return true;
	}

	public bool GetCuePoints(ref long startpos, ref long endpos, double thresholdIn, double thresholdOut, bool findZeroCrossing)
	{
		if (!_isRendered || _waveBuffer == null)
		{
			return false;
		}
		int startpos2 = 0;
		int endpos2 = _waveBuffer.data.Length - 1;
		DetectSilence(ref startpos2, ref endpos2, thresholdIn, thresholdOut, -1, -1, findZeroCrossing ? 1 : (-1));
		startpos = Frame2Bytes(startpos2);
		endpos = Frame2Bytes(endpos2);
		return true;
	}

	public bool GetCuePoints(ref long startpos, ref long endpos, double threshold, int frameStart, int frameEnd)
	{
		if (!_isRendered || _waveBuffer == null)
		{
			return false;
		}
		int startpos2 = 0;
		int endpos2 = _waveBuffer.data.Length - 1;
		DetectSilence(ref startpos2, ref endpos2, threshold, threshold, frameStart, frameEnd, -1);
		startpos = Frame2Bytes(startpos2);
		endpos = Frame2Bytes(endpos2);
		return true;
	}

	public bool GetCuePoints(ref long startpos, ref long endpos, double threshold, int frameStart, int frameEnd, bool findZeroCrossing)
	{
		if (!_isRendered || _waveBuffer == null)
		{
			return false;
		}
		int startpos2 = 0;
		int endpos2 = _waveBuffer.data.Length - 1;
		DetectSilence(ref startpos2, ref endpos2, threshold, threshold, frameStart, frameEnd, findZeroCrossing ? 1 : (-1));
		startpos = Frame2Bytes(startpos2);
		endpos = Frame2Bytes(endpos2);
		return true;
	}

	public bool GetCuePoints(ref long startpos, ref long endpos, double thresholdIn, double thresholdOut, int frameStart, int frameEnd)
	{
		if (!_isRendered || _waveBuffer == null)
		{
			return false;
		}
		int startpos2 = 0;
		int endpos2 = _waveBuffer.data.Length - 1;
		DetectSilence(ref startpos2, ref endpos2, thresholdIn, thresholdOut, frameStart, frameEnd, -1);
		startpos = Frame2Bytes(startpos2);
		endpos = Frame2Bytes(endpos2);
		return true;
	}

	public bool GetCuePoints(ref long startpos, ref long endpos, double thresholdIn, double thresholdOut, int frameStart, int frameEnd, bool findZeroCrossing)
	{
		if (!_isRendered || _waveBuffer == null)
		{
			return false;
		}
		int startpos2 = 0;
		int endpos2 = _waveBuffer.data.Length - 1;
		DetectSilence(ref startpos2, ref endpos2, thresholdIn, thresholdOut, frameStart, frameEnd, findZeroCrossing ? 1 : (-1));
		startpos = Frame2Bytes(startpos2);
		endpos = Frame2Bytes(endpos2);
		return true;
	}

	public bool GetCuePoints(ref double startpos, ref double endpos, double threshold)
	{
		if (!_isRendered || _waveBuffer == null)
		{
			return false;
		}
		int startpos2 = 0;
		int endpos2 = _waveBuffer.data.Length - 1;
		DetectSilence(ref startpos2, ref endpos2, threshold, threshold, -1, -1, -1);
		startpos = Frame2Seconds(startpos2);
		endpos = Frame2Seconds(endpos2);
		return true;
	}

	public bool GetCuePoints(ref double startpos, ref double endpos, double thresholdIn, double thresholdOut)
	{
		if (!_isRendered || _waveBuffer == null)
		{
			return false;
		}
		int startpos2 = 0;
		int endpos2 = _waveBuffer.data.Length - 1;
		DetectSilence(ref startpos2, ref endpos2, thresholdIn, thresholdOut, -1, -1, -1);
		startpos = Frame2Seconds(startpos2);
		endpos = Frame2Seconds(endpos2);
		return true;
	}

	public bool GetCuePoints(ref double startpos, ref double endpos, double thresholdIn, double thresholdOut, bool findZeroCrossing)
	{
		if (!_isRendered || _waveBuffer == null)
		{
			return false;
		}
		int startpos2 = 0;
		int endpos2 = _waveBuffer.data.Length - 1;
		DetectSilence(ref startpos2, ref endpos2, thresholdIn, thresholdOut, -1, -1, findZeroCrossing ? 1 : (-1));
		startpos = Frame2Seconds(startpos2);
		endpos = Frame2Seconds(endpos2);
		return true;
	}

	public bool GetCuePoints(ref double startpos, ref double endpos, double threshold, int frameStart, int frameEnd)
	{
		if (!_isRendered || _waveBuffer == null)
		{
			return false;
		}
		int startpos2 = 0;
		int endpos2 = _waveBuffer.data.Length - 1;
		DetectSilence(ref startpos2, ref endpos2, threshold, threshold, frameStart, frameEnd, -1);
		startpos = Frame2Seconds(startpos2);
		endpos = Frame2Seconds(endpos2);
		return true;
	}

	public bool GetCuePoints(ref double startpos, ref double endpos, double threshold, int frameStart, int frameEnd, bool findZeroCrossing)
	{
		if (!_isRendered || _waveBuffer == null)
		{
			return false;
		}
		int startpos2 = 0;
		int endpos2 = _waveBuffer.data.Length - 1;
		DetectSilence(ref startpos2, ref endpos2, threshold, threshold, frameStart, frameEnd, findZeroCrossing ? 1 : (-1));
		startpos = Frame2Seconds(startpos2);
		endpos = Frame2Seconds(endpos2);
		return true;
	}

	public bool GetCuePoints(ref double startpos, ref double endpos, double thresholdIn, double thresholdOut, int frameStart, int frameEnd)
	{
		if (!_isRendered || _waveBuffer == null)
		{
			return false;
		}
		int startpos2 = 0;
		int endpos2 = _waveBuffer.data.Length - 1;
		DetectSilence(ref startpos2, ref endpos2, thresholdIn, thresholdOut, frameStart, frameEnd, -1);
		startpos = Frame2Seconds(startpos2);
		endpos = Frame2Seconds(endpos2);
		return true;
	}

	public bool GetCuePoints(ref double startpos, ref double endpos, double thresholdIn, double thresholdOut, int frameStart, int frameEnd, bool findZeroCrossing)
	{
		if (!_isRendered || _waveBuffer == null)
		{
			return false;
		}
		int startpos2 = 0;
		int endpos2 = _waveBuffer.data.Length - 1;
		DetectSilence(ref startpos2, ref endpos2, thresholdIn, thresholdOut, frameStart, frameEnd, findZeroCrossing ? 1 : (-1));
		startpos = Frame2Seconds(startpos2);
		endpos = Frame2Seconds(endpos2);
		return true;
	}

	private void DetectSilence(ref int startpos, ref int endpos, double dBIn, double dBOut, int frameStart, int frameEnd, int findZeroCrossing)
	{
		if (dBIn > 0.0)
		{
			dBIn = 0.0;
		}
		else if (dBIn < -90.0)
		{
			dBIn = -90.0;
		}
		if (dBOut > 0.0)
		{
			dBOut = 0.0;
		}
		else if (dBOut < -90.0)
		{
			dBOut = -90.0;
		}
		if (frameEnd < frameStart)
		{
			frameEnd = _waveBuffer.data.Length - 1;
			frameStart = 0;
		}
		if (frameEnd >= _waveBuffer.data.Length)
		{
			frameEnd = _waveBuffer.data.Length - 1;
		}
		if (frameEnd < 0)
		{
			frameEnd = _waveBuffer.data.Length - 1;
		}
		if (frameStart < 0)
		{
			frameStart = 0;
		}
		int num = frameStart;
		int num2 = frameEnd;
		short num3 = (short)Utils.DBToLevel(dBIn, 32768);
		short num4 = (short)Utils.DBToLevel(dBOut, 32768);
		int i;
		for (i = num; i < frameEnd && PeakLevelOfFrame(i) < num3; i++)
		{
		}
		if (i < frameEnd)
		{
			if (findZeroCrossing == 0)
			{
				while (i > frameStart && PeakLevelOfFrame(i) > num3 / 2)
				{
					i--;
				}
			}
			else if (findZeroCrossing == 1)
			{
				while (i > frameStart && !IsZeroCrossingFrame(i, i + 1))
				{
					i--;
				}
			}
		}
		num = i;
		i = num2;
		while (i > frameStart && PeakLevelOfFrame(i) < num4)
		{
			i--;
		}
		if (i > frameStart)
		{
			if (findZeroCrossing == 0)
			{
				for (; i < frameEnd && PeakLevelOfFrame(i) > num4 / 2; i++)
				{
				}
			}
			else if (findZeroCrossing == 1)
			{
				for (; i < frameEnd && !IsZeroCrossingFrame(i, i - 1); i++)
				{
				}
			}
		}
		num2 = i;
		if (num2 <= num)
		{
			num2 = frameEnd;
		}
		startpos = num;
		endpos = num2;
	}

	public float GetNormalizationGain(int frameStart, int frameEnd, ref float peak)
	{
		float result = 1f;
		if (_waveBuffer == null)
		{
			return result;
		}
		if (frameEnd < frameStart)
		{
			frameEnd = _waveBuffer.data.Length - 1;
			frameStart = 0;
		}
		if (frameEnd >= _waveBuffer.data.Length)
		{
			frameEnd = _waveBuffer.data.Length - 1;
		}
		if (frameEnd < 0)
		{
			frameEnd = _waveBuffer.data.Length - 1;
		}
		if (frameStart < 0)
		{
			frameStart = 0;
		}
		int i = frameStart;
		short num = 0;
		short num2 = 0;
		for (; i < frameEnd; i++)
		{
			num = PeakLevelOfFrame(i);
			if (num > num2)
			{
				num2 = num;
			}
			if (num2 == short.MaxValue)
			{
				break;
			}
		}
		if (num2 < short.MaxValue && num2 > 0)
		{
			result = 32767f / (float)num2;
		}
		peak = (float)num2 / 32767f;
		return result;
	}

	public long DetectNextLevel(long startpos, double threshold, bool reverse, bool findZeroCrossing)
	{
		if (!_isRendered || _waveBuffer == null)
		{
			return startpos;
		}
		int num = Position2Frames(startpos);
		int num2 = DetectNextLevel(num, threshold, -1, -1, reverse, findZeroCrossing ? 1 : (-1));
		if (num2 == num)
		{
			return startpos;
		}
		return Frame2Bytes(num2);
	}

	public double DetectNextLevel(double startpos, double threshold, bool reverse, bool findZeroCrossing)
	{
		if (!_isRendered || _waveBuffer == null)
		{
			return startpos;
		}
		int num = Position2Frames(startpos);
		int num2 = DetectNextLevel(num, threshold, -1, -1, reverse, findZeroCrossing ? 1 : (-1));
		if (num2 == num)
		{
			return startpos;
		}
		return Frame2Seconds(num2);
	}

	private int DetectNextLevel(int start, double dBvalue, int frameStart, int frameEnd, bool reverse, int findZeroCrossing)
	{
		if (dBvalue > 0.0)
		{
			dBvalue = 0.0;
		}
		else if (dBvalue < -90.0)
		{
			dBvalue = -90.0;
		}
		if (frameEnd < frameStart)
		{
			frameEnd = _waveBuffer.data.Length - 1;
			frameStart = 0;
		}
		if (frameEnd >= _waveBuffer.data.Length)
		{
			frameEnd = _waveBuffer.data.Length - 1;
		}
		if (frameEnd < 0)
		{
			frameEnd = _waveBuffer.data.Length - 1;
		}
		if (frameStart < 0)
		{
			frameStart = 0;
		}
		int i = start;
		short num = (short)Utils.DBToLevel(dBvalue, 32768);
		if (reverse)
		{
			while (i > frameStart && PeakLevelOfFrame(i) < num)
			{
				i--;
			}
			if (i > frameStart)
			{
				if (findZeroCrossing == 0)
				{
					for (; i < frameEnd && PeakLevelOfFrame(i) > num / 2; i++)
					{
					}
				}
				else if (findZeroCrossing == 1)
				{
					for (; i < frameEnd && !IsZeroCrossingFrame(i, i - 1); i++)
					{
					}
				}
			}
			else
			{
				i = start;
			}
		}
		else
		{
			for (; i < frameEnd && PeakLevelOfFrame(i) < num; i++)
			{
			}
			if (i < frameEnd)
			{
				if (findZeroCrossing == 0)
				{
					while (i > frameStart && PeakLevelOfFrame(i) > num / 2)
					{
						i--;
					}
				}
				else if (findZeroCrossing == 1)
				{
					while (i > frameStart && !IsZeroCrossingFrame(i, i + 1))
					{
						i--;
					}
				}
			}
			else
			{
				i = start;
			}
		}
		return i;
	}

	public long FindPreviousZeroCrossing(long position)
	{
		if (_waveBuffer == null || _waveBuffer.data == null)
		{
			return position;
		}
		int num = Position2Frames(position);
		int num2 = 0;
		int num3 = _waveBuffer.data.Length - 1;
		if (num < num3)
		{
			while (num > num2 && !IsZeroCrossingFrame(num, num + 1))
			{
				num--;
			}
		}
		return Frame2Bytes(num);
	}

	public long FindNextZeroCrossing(long position)
	{
		if (_waveBuffer == null || _waveBuffer.data == null)
		{
			return position;
		}
		int i = Position2Frames(position);
		int num = 0;
		int num2 = _waveBuffer.data.Length - 1;
		if (i > num)
		{
			for (; i < num2 && !IsZeroCrossingFrame(i, i - 1); i++)
			{
			}
		}
		return Frame2Bytes(i);
	}

	public short PeakLevelOfFrame(int pos)
	{
		try
		{
			short left = _waveBuffer.data[pos].left;
			short right = _waveBuffer.data[pos].right;
			if (left == short.MinValue)
			{
				return short.MaxValue;
			}
			if (right == short.MinValue)
			{
				return short.MaxValue;
			}
			if (left < 0 && right < 0 && left > right)
			{
				return (short)(-right);
			}
			if (left > 0 && right > 0 && left < right)
			{
				return right;
			}
			if (left < 0 && left > -right)
			{
				return right;
			}
			if (left > 0 && left < -right)
			{
				return right;
			}
			return left;
		}
		catch
		{
			return 0;
		}
	}

	public bool IsZeroCrossingFrame(int pos1, int pos2)
	{
		if (_waveBuffer == null || pos1 > _waveBuffer.data.Length - 1 || pos2 > _waveBuffer.data.Length - 1 || pos1 < 0 || pos2 < 0)
		{
			return false;
		}
		bool result = false;
		try
		{
			WaveBuffer.Level level = _waveBuffer.data[pos1];
			WaveBuffer.Level level2 = _waveBuffer.data[pos2];
			if ((level.left >= 0 && level2.left <= 0) || (level.right >= 0 && level2.right <= 0) || (level.left < 0 && level2.left > 0) || (level.right < 0 && level2.right > 0))
			{
				result = true;
			}
		}
		catch
		{
		}
		return result;
	}

	private static WaveBuffer.Level GetLevel(byte[] buffer, int chans, int startIndex, int length)
	{
		WaveBuffer.Level result = default(WaveBuffer.Level);
		if (buffer == null)
		{
			return result;
		}
		int num = startIndex + length;
		int num2 = 0;
		for (int i = startIndex; i < num; i++)
		{
			num2 = (buffer[i] - 128) * 256;
			if (i % 2 == 0)
			{
				if (num2 < 0)
				{
					if (num2 <= -32768)
					{
						result.left = short.MinValue;
					}
					else if (num2 < result.left)
					{
						result.left = (short)num2;
					}
				}
				else if (num2 >= 32767)
				{
					result.left = short.MaxValue;
				}
				else if (num2 > result.left)
				{
					result.left = (short)num2;
				}
			}
			else if (num2 < 0)
			{
				if (num2 <= -32768)
				{
					result.right = short.MinValue;
				}
				else if (num2 < result.right)
				{
					result.right = (short)num2;
				}
			}
			else if (num2 >= 32767)
			{
				result.right = short.MaxValue;
			}
			else if (num2 > result.right)
			{
				result.right = (short)num2;
			}
		}
		if (chans == 1)
		{
			if (result.left == short.MinValue)
			{
				result.right = result.left;
			}
			else if (result.right == short.MinValue)
			{
				result.left = result.right;
			}
			else if (result.left < 0 && result.right < 0 && result.left > result.right)
			{
				result.left = result.right;
			}
			else if (result.left > 0 && result.right > 0 && result.left < result.right)
			{
				result.left = result.right;
			}
			else if (result.left < 0 && result.left > -result.right)
			{
				result.left = result.right;
			}
			else if (result.left > 0 && result.left < -result.right)
			{
				result.left = result.right;
			}
			else
			{
				result.right = result.left;
			}
		}
		return result;
	}

	private static WaveBuffer.Level GetLevel(short[] buffer, int chans, int startIndex, int length)
	{
		WaveBuffer.Level result = default(WaveBuffer.Level);
		if (buffer == null)
		{
			return result;
		}
		int num = startIndex + length;
		short num2 = 0;
		for (int i = startIndex; i < num; i++)
		{
			num2 = buffer[i];
			if (i % 2 == 0)
			{
				if (num2 < 0)
				{
					if (num2 == short.MinValue)
					{
						result.left = short.MinValue;
					}
					else if (num2 < result.left)
					{
						result.left = num2;
					}
				}
				else if (num2 == short.MaxValue)
				{
					result.left = short.MaxValue;
				}
				else if (num2 > result.left)
				{
					result.left = num2;
				}
			}
			else if (num2 < 0)
			{
				if (num2 == short.MinValue)
				{
					result.right = short.MinValue;
				}
				else if (num2 < result.right)
				{
					result.right = num2;
				}
			}
			else if (num2 == short.MaxValue)
			{
				result.right = short.MaxValue;
			}
			else if (num2 > result.right)
			{
				result.right = num2;
			}
		}
		if (chans == 1)
		{
			if (result.left == short.MinValue)
			{
				result.right = result.left;
			}
			else if (result.right == short.MinValue)
			{
				result.left = result.right;
			}
			else if (result.left < 0 && result.right < 0 && result.left > result.right)
			{
				result.left = result.right;
			}
			else if (result.left > 0 && result.right > 0 && result.left < result.right)
			{
				result.left = result.right;
			}
			else if (result.left < 0 && result.left > -result.right)
			{
				result.left = result.right;
			}
			else if (result.left > 0 && result.left < -result.right)
			{
				result.left = result.right;
			}
			else
			{
				result.right = result.left;
			}
		}
		return result;
	}

	private static WaveBuffer.Level GetLevel(float[] buffer, int chans, int startIndex, int length)
	{
		WaveBuffer.Level result = default(WaveBuffer.Level);
		if (buffer == null)
		{
			return result;
		}
		int num = startIndex + length;
		float num2 = 0f;
		for (int i = startIndex; i < num; i++)
		{
			num2 = ((!(buffer[i] < 0f)) ? ((float)(int)(buffer[i] * 32768f + 0.5f)) : ((float)(int)(buffer[i] * 32768f - 0.5f)));
			if (i % 2 == 0)
			{
				if (num2 < 0f)
				{
					if (num2 <= -32768f)
					{
						result.left = short.MinValue;
					}
					else if (num2 < (float)result.left)
					{
						result.left = (short)num2;
					}
				}
				else if (num2 >= 32767f)
				{
					result.left = short.MaxValue;
				}
				else if (num2 > (float)result.left)
				{
					result.left = (short)num2;
				}
			}
			else if (num2 < 0f)
			{
				if (num2 <= -32768f)
				{
					result.right = short.MinValue;
				}
				else if (num2 < (float)result.right)
				{
					result.right = (short)num2;
				}
			}
			else if (num2 >= 32767f)
			{
				result.right = short.MaxValue;
			}
			else if (num2 > (float)result.right)
			{
				result.right = (short)num2;
			}
		}
		if (chans == 1)
		{
			if (result.left == short.MinValue)
			{
				result.right = result.left;
			}
			else if (result.right == short.MinValue)
			{
				result.left = result.right;
			}
			else if (result.left < 0 && result.right < 0 && result.left > result.right)
			{
				result.left = result.right;
			}
			else if (result.left > 0 && result.right > 0 && result.left < result.right)
			{
				result.left = result.right;
			}
			else if (result.left < 0 && result.left > -result.right)
			{
				result.left = result.right;
			}
			else if (result.left > 0 && result.left < -result.right)
			{
				result.left = result.right;
			}
			else
			{
				result.right = result.left;
			}
		}
		return result;
	}

	private unsafe static WaveBuffer.Level GetLevel(IntPtr buffer, int chans, int bps, int startIndex, int length)
	{
		WaveBuffer.Level result = default(WaveBuffer.Level);
		if (buffer == IntPtr.Zero)
		{
			return result;
		}
		if (bps == 16 || bps == 32 || bps == 8)
		{
			bps /= 8;
		}
		if (startIndex < 0)
		{
			startIndex = 0;
		}
		int num = startIndex + length;
		switch (bps)
		{
		case 2:
		{
			short* ptr2 = (short*)(void*)buffer;
			short num3 = 0;
			for (int j = startIndex; j < num; j++)
			{
				num3 = ptr2[j];
				if (j % 2 == 0)
				{
					if (num3 < 0)
					{
						if (num3 == short.MinValue)
						{
							result.left = short.MinValue;
						}
						else if (num3 < result.left)
						{
							result.left = num3;
						}
					}
					else if (num3 == short.MaxValue)
					{
						result.left = short.MaxValue;
					}
					else if (num3 > result.left)
					{
						result.left = num3;
					}
				}
				else if (num3 < 0)
				{
					if (num3 == short.MinValue)
					{
						result.right = short.MinValue;
					}
					else if (num3 < result.right)
					{
						result.right = num3;
					}
				}
				else if (num3 == short.MaxValue)
				{
					result.right = short.MaxValue;
				}
				else if (num3 > result.right)
				{
					result.right = num3;
				}
			}
			break;
		}
		case 4:
		{
			float* ptr3 = (float*)(void*)buffer;
			int num4 = 0;
			for (int k = startIndex; k < num; k++)
			{
				num4 = ((!(ptr3[k] < 0f)) ? ((int)(ptr3[k] * 32768f + 0.5f)) : ((int)(ptr3[k] * 32768f - 0.5f)));
				if (k % 2 == 0)
				{
					if (num4 < 0)
					{
						if (num4 <= -32768)
						{
							result.left = short.MinValue;
						}
						else if (num4 < result.left)
						{
							result.left = (short)num4;
						}
					}
					else if (num4 >= 32767)
					{
						result.left = short.MaxValue;
					}
					else if (num4 > result.left)
					{
						result.left = (short)num4;
					}
				}
				else if (num4 < 0)
				{
					if (num4 <= -32768)
					{
						result.right = short.MinValue;
					}
					else if (num4 < result.right)
					{
						result.right = (short)num4;
					}
				}
				else if (num4 >= 32767)
				{
					result.right = short.MaxValue;
				}
				else if (num4 > result.right)
				{
					result.right = (short)num4;
				}
			}
			break;
		}
		default:
		{
			byte* ptr = (byte*)(void*)buffer;
			int num2 = 0;
			for (int i = startIndex; i < num; i++)
			{
				num2 = (ptr[i] - 128) * 256;
				if (i % 2 == 0)
				{
					if (num2 < 0)
					{
						if (num2 <= -32768)
						{
							result.left = short.MinValue;
						}
						else if (num2 < result.left)
						{
							result.left = (short)num2;
						}
					}
					else if (num2 >= 32767)
					{
						result.left = short.MaxValue;
					}
					else if (num2 > result.left)
					{
						result.left = (short)num2;
					}
				}
				else if (num2 < 0)
				{
					if (num2 <= -32768)
					{
						result.right = short.MinValue;
					}
					else if (num2 < result.right)
					{
						result.right = (short)num2;
					}
				}
				else if (num2 >= 32767)
				{
					result.right = short.MaxValue;
				}
				else if (num2 > result.right)
				{
					result.right = (short)num2;
				}
			}
			break;
		}
		}
		if (chans == 1)
		{
			if (result.left == short.MinValue)
			{
				result.right = result.left;
			}
			else if (result.right == short.MinValue)
			{
				result.left = result.right;
			}
			else if (result.left < 0 && result.right < 0 && result.left > result.right)
			{
				result.left = result.right;
			}
			else if (result.left > 0 && result.right > 0 && result.left < result.right)
			{
				result.left = result.right;
			}
			else if (result.left < 0 && result.left > -result.right)
			{
				result.left = result.right;
			}
			else if (result.left > 0 && result.left < -result.right)
			{
				result.left = result.right;
			}
			else
			{
				result.right = result.left;
			}
		}
		return result;
	}

	private bool Render(bool background, ThreadPriority prio, BASSFlag flags, int chans)
	{
		bool flag = false;
		_isRendered = false;
		_waveBuffer = new WaveBuffer();
		double num = Bass.BASS_ChannelGetLength(_decodingStream);
		if (num < 0.0)
		{
			num = 0.0;
			byte[] buffer = new byte[32768];
			int num2 = 0;
			while ((num2 = Bass.BASS_ChannelGetData(_decodingStream, buffer, 32768)) >= 0)
			{
				num += (double)num2;
			}
			Bass.BASS_ChannelSetPosition(_decodingStream, 0L);
		}
		_waveBuffer.chans = chans;
		_waveBuffer.resolution = FrameResolution;
		_waveBuffer.bpf = (int)Bass.BASS_ChannelSeconds2Bytes(_decodingStream, _waveBuffer.resolution);
		_waveBuffer.flags = flags;
		_framesToRender = (int)Math.Ceiling(num / (double)_waveBuffer.bpf);
		_correctionFactor = Bass.BASS_ChannelBytes2Seconds(_decodingStream, _waveBuffer.bpf) / _waveBuffer.resolution;
		_waveBuffer.data = new WaveBuffer.Level[_framesToRender];
		_waveBuffer.fileName = FileName;
		if (DetectBeats)
		{
			_waveBuffer.beats = new List<long>((int)(Bass.BASS_ChannelBytes2Seconds(_decodingStream, (long)num) / 60.0 * 120.0));
		}
		_renderStartTime = DateTime.Now;
		if (background)
		{
			if (prio == ThreadPriority.Normal)
			{
				new MethodInvoker(ScanPeaks).BeginInvoke(null, null);
			}
			else if (_useSimpleScan)
			{
				Thread thread = new Thread(ScanPeaksSimple);
				thread.Priority = prio;
				thread.Start();
			}
			else
			{
				Thread thread2 = new Thread(ScanPeaks);
				thread2.Priority = prio;
				thread2.Start();
			}
			return true;
		}
		if (_useSimpleScan)
		{
			ScanPeaksSimple();
		}
		else
		{
			ScanPeaks();
		}
		return true;
	}

	private void ScanPeaks()
	{
		if (_waveBuffer == null)
		{
			return;
		}
		object obj = null;
		if (DetectBeats)
		{
			obj = new BPMBEATPROC(BpmBeatCallback);
			BassFx.BASS_FX_BPM_BeatCallbackSet(_decodingStream, (BPMBEATPROC)obj, IntPtr.Zero);
		}
		_renderingInProgress = true;
		_framesRendered = 0;
		WaveBuffer.Level level = default(WaveBuffer.Level);
		int num = 0;
		_ = _waveBuffer.bpf;
		int num2 = (int)Bass.BASS_ChannelSeconds2Bytes(_decodingStream, 1.0) / _waveBuffer.bpf * _waveBuffer.bpf;
		int num3 = num2 / _waveBuffer.bpf;
		int num4 = 0;
		int bps = _waveBuffer.bps;
		int num5 = num2 / bps;
		int num6 = _waveBuffer.bpf / bps;
		try
		{
			while (!_killScan)
			{
				if (Bass.BASS_ChannelIsActive(_decodingStream) == BASSActive.BASS_ACTIVE_STOPPED)
				{
					num = _framesToRender;
					_framesRendered = _framesToRender;
					_killScan = true;
					continue;
				}
				switch (bps)
				{
				case 2:
				{
					if (_peakLevelShort == null || _peakLevelShort.Length < num5)
					{
						_peakLevelShort = new short[num5];
					}
					num4 = 0;
					num5 = Bass.BASS_ChannelGetData(_decodingStream, _peakLevelShort, num2) / bps;
					for (int j = 0; j < num3; j++)
					{
						level = ((num4 + num6 <= num5) ? GetLevel(_peakLevelShort, _waveBuffer.chans, num4, num6) : GetLevel(_peakLevelShort, _waveBuffer.chans, num4, num5 - num4));
						num4 += num6;
						if (num < _framesToRender)
						{
							_waveBuffer.data[num] = level;
							num = (_framesRendered = num + 1);
						}
						if (CallbackFrequency > 0 && _framesRendered % CallbackFrequency == 0)
						{
							InvokeCallback(finished: false);
						}
						if (num4 > num5)
						{
							break;
						}
					}
					continue;
				}
				case 4:
				{
					if (_peakLevelFloat == null || _peakLevelFloat.Length < num5)
					{
						_peakLevelFloat = new float[num5];
					}
					num4 = 0;
					num5 = Bass.BASS_ChannelGetData(_decodingStream, _peakLevelFloat, num2) / bps;
					for (int i = 0; i < num3; i++)
					{
						level = ((num4 + num6 <= num5) ? GetLevel(_peakLevelFloat, _waveBuffer.chans, num4, num6) : GetLevel(_peakLevelFloat, _waveBuffer.chans, num4, num5 - num4));
						num4 += num6;
						if (num < _framesToRender)
						{
							_waveBuffer.data[num] = level;
							num = (_framesRendered = num + 1);
						}
						if (CallbackFrequency > 0 && _framesRendered % CallbackFrequency == 0)
						{
							InvokeCallback(finished: false);
						}
						if (num4 > num5)
						{
							break;
						}
					}
					continue;
				}
				}
				if (_peakLevelByte == null || _peakLevelByte.Length < num5)
				{
					_peakLevelByte = new byte[num5];
				}
				num4 = 0;
				num5 = Bass.BASS_ChannelGetData(_decodingStream, _peakLevelByte, num2) / bps;
				for (int k = 0; k < num3; k++)
				{
					level = ((num4 + num6 <= num5) ? GetLevel(_peakLevelByte, _waveBuffer.chans, num4, num6) : GetLevel(_peakLevelByte, _waveBuffer.chans, num4, num5 - num4));
					num4 += num6;
					if (num < _framesToRender)
					{
						_waveBuffer.data[num] = level;
						num = (_framesRendered = num + 1);
					}
					if (CallbackFrequency > 0 && _framesRendered % CallbackFrequency == 0)
					{
						InvokeCallback(finished: false);
					}
					if (num4 > num5)
					{
						break;
					}
				}
			}
		}
		catch
		{
		}
		try
		{
			if (obj != null)
			{
				BassFx.BASS_FX_BPM_BeatFree(_decodingStream);
				obj = null;
			}
		}
		catch
		{
		}
		if (_freeStream)
		{
			Bass.BASS_StreamFree(_decodingStream);
		}
		_isRendered = true;
		_renderingInProgress = false;
		try
		{
			InvokeCallback(finished: true);
		}
		catch
		{
		}
	}

	private void ScanPeaksSimple()
	{
		if (_waveBuffer == null)
		{
			return;
		}
		object obj = null;
		if (DetectBeats)
		{
			obj = new BPMBEATPROC(BpmBeatCallback);
			BassFx.BASS_FX_BPM_BeatCallbackSet(_decodingStream, (BPMBEATPROC)obj, IntPtr.Zero);
		}
		_renderingInProgress = true;
		_framesRendered = 0;
		int num = 0;
		try
		{
			int num2 = 0;
			while (!_killScan)
			{
				num2 = Bass.BASS_ChannelGetLevel(_decodingStream);
				if (Bass.BASS_ChannelIsActive(_decodingStream) == BASSActive.BASS_ACTIVE_STOPPED)
				{
					num = _framesToRender;
					_framesRendered = _framesToRender;
					_killScan = true;
					continue;
				}
				_waveBuffer.data[num] = new WaveBuffer.Level(Utils.LowWord(num2), Utils.HighWord(num2));
				num = (_framesRendered = num + 1);
				if (CallbackFrequency > 0 && _framesRendered % CallbackFrequency == 0)
				{
					InvokeCallback(finished: false);
				}
			}
		}
		catch
		{
		}
		try
		{
			if (obj != null)
			{
				BassFx.BASS_FX_BPM_BeatFree(_decodingStream);
				obj = null;
			}
		}
		catch
		{
		}
		if (_freeStream)
		{
			Bass.BASS_StreamFree(_decodingStream);
		}
		_isRendered = true;
		_renderingInProgress = false;
		try
		{
			InvokeCallback(finished: true);
		}
		catch
		{
		}
	}

	private void BpmBeatCallback(int handle, double beatpos, IntPtr user)
	{
		try
		{
			_waveBuffer.beats.Add(Bass.BASS_ChannelSeconds2Bytes(_decodingStream, beatpos));
		}
		catch
		{
		}
	}

	private void InvokeCallback(bool finished)
	{
		if (NotifyHandler != null)
		{
			TimeSpan timeSpan = DateTime.Now - _renderStartTime;
			if (WinControl != null)
			{
				WinControl.BeginInvoke(NotifyHandler, _framesRendered, _framesToRender, timeSpan, finished);
			}
			else
			{
				NotifyHandler(_framesRendered, _framesToRender, timeSpan, finished);
			}
		}
	}

	private void DrawBitmap(Graphics g, int width, int height, int frameStart, int frameEnd, Pen pMiddleL, Pen pMiddleR, Pen pLeft, Pen pLeftEnv, Pen pRight, Pen pRightEnv, Pen pMarker, Pen pBeat, Pen pVolume)
	{
		WAVEFORMDRAWTYPE wAVEFORMDRAWTYPE = _drawWaveForm;
		if (_waveBuffer.chans == 1 && (wAVEFORMDRAWTYPE == WAVEFORMDRAWTYPE.Stereo || wAVEFORMDRAWTYPE == WAVEFORMDRAWTYPE.DualMono))
		{
			wAVEFORMDRAWTYPE = WAVEFORMDRAWTYPE.Mono;
		}
		if (frameEnd >= _waveBuffer.data.Length)
		{
			frameEnd = _waveBuffer.data.Length - 1;
		}
		if (frameEnd < 0)
		{
			frameEnd = _waveBuffer.data.Length - 1;
		}
		if (frameStart < 0)
		{
			frameStart = 0;
		}
		short num = 0;
		short num2 = 0;
		short num3 = 0;
		short num4 = 0;
		float num5 = 0f;
		float num6 = 0f;
		float num7 = 0f;
		float num8 = 0f;
		float num9 = -1f;
		float nextLevel = -1f;
		double num10 = (double)(frameEnd - frameStart) / (double)width;
		float num11 = (float)height / 2f - 1f;
		float num12 = ((float)height - 2f) / 4f - 1f;
		float num13 = ((float)height - 1f) / 4f;
		float num14 = (float)height - num13 - 1f;
		float num15 = num13;
		float num16 = num14;
		float num17 = num13;
		float num18 = num14;
		if (wAVEFORMDRAWTYPE != 0)
		{
			num12 = ((float)height - 3f) / 2f - 1f;
			num13 = (num14 = num11);
			num15 = num13;
			num16 = num14;
			num17 = num13;
			num18 = num14;
		}
		height--;
		if (frameStart > 0)
		{
			num15 = (num17 = num13 - (float)_waveBuffer.data[frameStart - 1].left * (num12 / 32767f) * _gainFactor);
			num16 = (num18 = num14 - (float)_waveBuffer.data[frameStart - 1].right * (num12 / 32767f) * _gainFactor);
			switch (wAVEFORMDRAWTYPE)
			{
			case WAVEFORMDRAWTYPE.Mono:
				num15 = (num16 = (num15 + num16) / 2f);
				break;
			case WAVEFORMDRAWTYPE.HalfMono:
			case WAVEFORMDRAWTYPE.HalfMonoFlipped:
				num15 = (num16 = (num15 + num16) / 2f);
				num15 = ((!(num15 <= num13)) ? (num16 = 2f * (2f * num13 - num15)) : (num16 = 2f * num15));
				break;
			}
		}
		g.Clear(_colorBackground);
		for (int i = frameStart; i <= frameEnd; i++)
		{
			if (_waveBuffer.data[i].left > num)
			{
				num = _waveBuffer.data[i].left;
			}
			if (_waveBuffer.data[i].right > num2)
			{
				num2 = _waveBuffer.data[i].right;
			}
			if (_waveBuffer.data[i].left < num3)
			{
				num3 = _waveBuffer.data[i].left;
			}
			if (_waveBuffer.data[i].right < num4)
			{
				num4 = _waveBuffer.data[i].right;
			}
			num9 = (int)((double)(i - frameStart) / num10 + 0.5);
			if (!(num9 > nextLevel))
			{
				continue;
			}
			if (num9 - nextLevel > 1f)
			{
				if (num3 == short.MinValue)
				{
					num = num3;
				}
				else if (num < 0 && num3 < 0 && num > num3)
				{
					num = num3;
				}
				else if (num > 0 && num3 > 0 && num < num3)
				{
					num = num3;
				}
				else if (num < 0 && num > -num3)
				{
					num = num3;
				}
				else if (num > 0 && num < -num3)
				{
					num = num3;
				}
				if (num4 == short.MinValue)
				{
					num2 = num4;
				}
				else if (num2 < 0 && num4 < 0 && num2 > num4)
				{
					num2 = num4;
				}
				else if (num2 > 0 && num4 > 0 && num2 < num4)
				{
					num2 = num4;
				}
				else if (num2 < 0 && num2 > -num4)
				{
					num2 = num4;
				}
				else if (num2 > 0 && num2 < -num4)
				{
					num2 = num4;
				}
				num5 = num13 - (float)num * (num12 / 32767f) * _gainFactor;
				num6 = num14 - (float)num2 * (num12 / 32767f) * _gainFactor;
				num7 = num13 - (float)num3 * (num12 / 32767f) * _gainFactor;
				num8 = num14 - (float)num4 * (num12 / 32767f) * _gainFactor;
				switch (wAVEFORMDRAWTYPE)
				{
				case WAVEFORMDRAWTYPE.Mono:
					num5 = (num6 = (num5 + num6) / 2f);
					num7 = (num8 = (num7 + num8) / 2f);
					if (num5 > num13)
					{
						num5 = num13;
					}
					if (num7 < num13)
					{
						num7 = num13;
					}
					if (num6 > num14)
					{
						num6 = num14;
					}
					if (num8 < num14)
					{
						num8 = num14;
					}
					break;
				case WAVEFORMDRAWTYPE.HalfMono:
					num5 = (num6 = (num5 + num6) / 2f);
					num5 = ((!(num5 <= num13)) ? (num6 = 2f * (2f * num13 - num5)) : (num6 = 2f * num5));
					num7 = (num8 = (num7 + num8) / 2f);
					num7 = ((!(num7 <= num13)) ? (num8 = 2f * (2f * num13 - num7)) : (num8 = 2f * num7));
					break;
				default:
					if (num5 > num13)
					{
						num5 = num13;
					}
					if (num7 < num13)
					{
						num7 = num13;
					}
					if (num6 > num14)
					{
						num6 = num14;
					}
					if (num8 < num14)
					{
						num8 = num14;
					}
					break;
				}
				float num19 = (num5 - num15) / (num9 - nextLevel);
				float num20 = (num6 - num16) / (num9 - nextLevel);
				float num21 = (num7 - num17) / (num9 - nextLevel);
				float num22 = (num8 - num18) / (num9 - nextLevel);
				switch (wAVEFORMDRAWTYPE)
				{
				case WAVEFORMDRAWTYPE.Stereo:
				{
					for (int k = 0; (float)k < num9 - nextLevel; k++)
					{
						if (num17 + (float)k * num21 < num13)
						{
							g.DrawLine(pLeft, nextLevel + (float)k, num13, nextLevel + (float)k, num15 + (float)k * num19);
						}
						else if (num15 + (float)k * num19 > num13)
						{
							g.DrawLine(pLeft, nextLevel + (float)k, num13, nextLevel + (float)k, num17 + (float)k * num21);
						}
						else
						{
							g.DrawLine(pLeft, nextLevel + (float)k, num17 + (float)k * num21, nextLevel + (float)k, num15 + (float)k * num19);
						}
						if (num18 + (float)k * num22 < num14)
						{
							g.DrawLine(pRight, nextLevel + (float)k, num14, nextLevel + (float)k, num16 + (float)k * num20);
						}
						else if (num16 + (float)k * num20 > num14)
						{
							g.DrawLine(pRight, nextLevel + (float)k, num14, nextLevel + (float)k, num18 + (float)k * num22);
						}
						else
						{
							g.DrawLine(pRight, nextLevel + (float)k, num18 + (float)k * num22, nextLevel + (float)k, num16 + (float)k * num20);
						}
					}
					if (_drawEnvelope)
					{
						g.DrawLine(pLeftEnv, nextLevel, num15, num9, num5);
						g.DrawLine(pLeftEnv, nextLevel, num17, num9, num7);
						g.DrawLine(pRightEnv, nextLevel, num16, num9, num6);
						g.DrawLine(pRightEnv, nextLevel, num18, num9, num8);
					}
					break;
				}
				case WAVEFORMDRAWTYPE.Mono:
				{
					for (int l = 0; (float)l < num9 - nextLevel; l++)
					{
						if (num17 + (float)l * num21 < num13)
						{
							g.DrawLine(pLeft, nextLevel + (float)l, num13, nextLevel + (float)l, num15 + (float)l * num19);
						}
						else if (num15 + (float)l * num19 > num13)
						{
							g.DrawLine(pLeft, nextLevel + (float)l, num13, nextLevel + (float)l, num17 + (float)l * num21);
						}
						else
						{
							g.DrawLine(pLeft, nextLevel + (float)l, num17 + (float)l * num21, nextLevel + (float)l, num15 + (float)l * num19);
						}
					}
					if (_drawEnvelope)
					{
						g.DrawLine(pLeftEnv, nextLevel, num15, num9, num5);
						g.DrawLine(pLeftEnv, nextLevel, num17, num9, num7);
					}
					break;
				}
				case WAVEFORMDRAWTYPE.HalfMono:
				{
					for (int m = 0; (float)m < num9 - nextLevel; m++)
					{
						if (num15 + (float)m * num19 < num17 + (float)m * num21)
						{
							g.DrawLine(pLeft, nextLevel + (float)m, height, nextLevel + (float)m, num15 + (float)m * num19);
						}
						else
						{
							g.DrawLine(pLeft, nextLevel + (float)m, height, nextLevel + (float)m, num17 + (float)m * num21);
						}
					}
					if (_drawEnvelope)
					{
						if (num5 < num7)
						{
							g.DrawLine(pLeftEnv, nextLevel, num15, num9, num5);
						}
						else
						{
							g.DrawLine(pLeftEnv, nextLevel, num17, num9, num7);
						}
					}
					break;
				}
				case WAVEFORMDRAWTYPE.HalfMonoFlipped:
				{
					for (int n = 0; (float)n < num9 - nextLevel; n++)
					{
						if (num15 + (float)n * num19 < num17 + (float)n * num21)
						{
							g.DrawLine(pLeft, nextLevel + (float)n, 0f, nextLevel + (float)n, (float)height - (num15 + (float)n * num19));
						}
						else
						{
							g.DrawLine(pLeft, nextLevel + (float)n, 0f, nextLevel + (float)n, (float)height - (num17 + (float)n * num21));
						}
					}
					if (_drawEnvelope)
					{
						if (num5 < num7)
						{
							g.DrawLine(pLeftEnv, nextLevel, (float)height - num15, num9, (float)height - num5);
						}
						else
						{
							g.DrawLine(pLeftEnv, nextLevel, (float)height - num17, num9, (float)height - num7);
						}
					}
					break;
				}
				default:
				{
					for (int j = 0; (float)j < num9 - nextLevel; j++)
					{
						if (num17 + (float)j * num21 < num13)
						{
							g.DrawLine(pLeft, nextLevel + (float)j, num13, nextLevel + (float)j, num15 + (float)j * num19);
						}
						else if (num15 + (float)j * num19 > num13)
						{
							g.DrawLine(pLeft, nextLevel + (float)j, num13, nextLevel + (float)j, num17 + (float)j * num21);
						}
						else
						{
							g.DrawLine(pLeft, nextLevel + (float)j, num17 + (float)j * num21, nextLevel + (float)j, num15 + (float)j * num19);
						}
						if (num18 + (float)j * num22 < num14)
						{
							g.DrawLine(pRight, nextLevel + (float)j, num14, nextLevel + (float)j, num16 + (float)j * num20);
						}
						else if (num16 + (float)j * num20 > num14)
						{
							g.DrawLine(pRight, nextLevel + (float)j, num14, nextLevel + (float)j, num18 + (float)j * num22);
						}
						else
						{
							g.DrawLine(pRight, nextLevel + (float)j, num18 + (float)j * num22, nextLevel + (float)j, num16 + (float)j * num20);
						}
					}
					if (_drawEnvelope)
					{
						g.DrawLine(pLeftEnv, nextLevel, num15, num9, num5);
						g.DrawLine(pLeftEnv, nextLevel, num17, num9, num7);
						g.DrawLine(pRightEnv, nextLevel, num16, num9, num6);
						g.DrawLine(pRightEnv, nextLevel, num18, num9, num8);
					}
					break;
				}
				}
				num15 = num5;
				num16 = num6;
				num17 = num7;
				num18 = num8;
			}
			else
			{
				num5 = num13 - (float)num * (num12 / 32767f) * _gainFactor;
				num6 = num14 - (float)num2 * (num12 / 32767f) * _gainFactor;
				num7 = num13 - (float)num3 * (num12 / 32767f) * _gainFactor;
				num8 = num14 - (float)num4 * (num12 / 32767f) * _gainFactor;
				switch (wAVEFORMDRAWTYPE)
				{
				case WAVEFORMDRAWTYPE.Mono:
					num5 = (num6 = (num5 + num6) / 2f);
					num7 = (num8 = (num7 + num8) / 2f);
					if (num5 > num13)
					{
						num5 = num13;
					}
					if (num7 < num13)
					{
						num7 = num13;
					}
					if (num6 > num14)
					{
						num6 = num14;
					}
					if (num8 < num14)
					{
						num8 = num14;
					}
					break;
				case WAVEFORMDRAWTYPE.HalfMono:
				case WAVEFORMDRAWTYPE.HalfMonoFlipped:
					num5 = (num6 = (num5 + num6) / 2f);
					num5 = ((!(num5 <= num13)) ? (num6 = 2f * (2f * num13 - num5)) : (num6 = 2f * num5));
					num7 = (num8 = (num7 + num8) / 2f);
					num7 = ((!(num7 <= num13)) ? (num8 = 2f * (2f * num13 - num7)) : (num8 = 2f * num7));
					break;
				default:
					if (num5 > num13)
					{
						num5 = num13;
					}
					if (num7 < num13)
					{
						num7 = num13;
					}
					if (num6 > num14)
					{
						num6 = num14;
					}
					if (num8 < num14)
					{
						num8 = num14;
					}
					break;
				}
				switch (wAVEFORMDRAWTYPE)
				{
				case WAVEFORMDRAWTYPE.Stereo:
					if (num7 < num13)
					{
						g.DrawLine(pLeft, nextLevel, num13, nextLevel, num5);
					}
					else if (num5 > num13)
					{
						g.DrawLine(pLeft, nextLevel, num13, nextLevel, num7);
					}
					else
					{
						g.DrawLine(pLeft, nextLevel, num7, nextLevel, num5);
					}
					if (num8 < num14)
					{
						g.DrawLine(pRight, nextLevel, num14, nextLevel, num6);
					}
					else if (num6 > num14)
					{
						g.DrawLine(pRight, nextLevel, num14, nextLevel, num8);
					}
					else
					{
						g.DrawLine(pRight, nextLevel, num8, nextLevel, num6);
					}
					if (_drawEnvelope)
					{
						g.DrawLine(pLeftEnv, nextLevel, num15, num9, num5);
						g.DrawLine(pLeftEnv, nextLevel, num17, num9, num7);
						g.DrawLine(pRightEnv, nextLevel, num16, num9, num6);
						g.DrawLine(pRightEnv, nextLevel, num18, num9, num8);
					}
					break;
				case WAVEFORMDRAWTYPE.Mono:
					if (num7 < num13)
					{
						g.DrawLine(pLeft, nextLevel, num13, nextLevel, num5);
					}
					else if (num5 > num13)
					{
						g.DrawLine(pLeft, nextLevel, num13, nextLevel, num7);
					}
					else
					{
						g.DrawLine(pLeft, nextLevel, num7, nextLevel, num5);
					}
					if (_drawEnvelope)
					{
						g.DrawLine(pLeftEnv, nextLevel, num15, num9, num5);
						g.DrawLine(pLeftEnv, nextLevel, num17, num9, num7);
					}
					break;
				case WAVEFORMDRAWTYPE.HalfMono:
					if (num5 < num7)
					{
						g.DrawLine(pLeft, nextLevel, height, nextLevel, num5);
					}
					else
					{
						g.DrawLine(pLeft, nextLevel, height, nextLevel, num7);
					}
					if (_drawEnvelope)
					{
						if (num5 < num7)
						{
							g.DrawLine(pLeftEnv, nextLevel, num15, num9, num5);
						}
						else
						{
							g.DrawLine(pLeftEnv, nextLevel, num17, num9, num7);
						}
					}
					break;
				case WAVEFORMDRAWTYPE.HalfMonoFlipped:
					if (num5 < num7)
					{
						g.DrawLine(pLeft, nextLevel, 0f, nextLevel, (float)height - num5);
					}
					else
					{
						g.DrawLine(pLeft, nextLevel, 0f, nextLevel, (float)height - num7);
					}
					if (_drawEnvelope)
					{
						if (num5 < num7)
						{
							g.DrawLine(pLeftEnv, nextLevel, (float)height - num15, num9, (float)height - num5);
						}
						else
						{
							g.DrawLine(pLeftEnv, nextLevel, (float)height - num17, num9, (float)height - num7);
						}
					}
					break;
				default:
					if (num7 < num13)
					{
						g.DrawLine(pLeft, nextLevel, num13, nextLevel, num5);
					}
					else if (num5 > num13)
					{
						g.DrawLine(pLeft, nextLevel, num13, nextLevel, num7);
					}
					else
					{
						g.DrawLine(pLeft, nextLevel, num7, nextLevel, num5);
					}
					if (num8 < num14)
					{
						g.DrawLine(pRight, nextLevel, num14, nextLevel, num6);
					}
					else if (num6 > num14)
					{
						g.DrawLine(pRight, nextLevel, num14, nextLevel, num8);
					}
					else
					{
						g.DrawLine(pRight, nextLevel, num8, nextLevel, num6);
					}
					if (_drawEnvelope)
					{
						g.DrawLine(pLeftEnv, nextLevel, num15, num9, num5);
						g.DrawLine(pLeftEnv, nextLevel, num17, num9, num7);
						g.DrawLine(pRightEnv, nextLevel, num16, num9, num6);
						g.DrawLine(pRightEnv, nextLevel, num18, num9, num8);
					}
					break;
				}
			}
			nextLevel = num9;
			num = 0;
			num2 = 0;
			num3 = 0;
			num4 = 0;
			num15 = num5;
			num16 = num6;
			num17 = num7;
			num18 = num8;
		}
		if (_drawCenterLine && wAVEFORMDRAWTYPE != WAVEFORMDRAWTYPE.HalfMono && wAVEFORMDRAWTYPE != WAVEFORMDRAWTYPE.HalfMonoFlipped)
		{
			g.DrawLine(pMiddleL, 0f, num13, width, num13);
			if (wAVEFORMDRAWTYPE == WAVEFORMDRAWTYPE.Stereo)
			{
				g.DrawLine(pMiddleR, 0f, num14, width, num14);
			}
		}
		long num23 = Frame2Bytes(frameStart);
		long num24 = Frame2Bytes(frameEnd);
		double num25 = (double)(num24 - num23) / (double)width;
		if (DrawBeat != 0 && _waveBuffer.beats != null && _waveBuffer.beats.Count > 0)
		{
			float num26 = (float)(height + 1) * _beatLength;
			long num27 = 0L;
			int num28 = _waveBuffer.beats.BinarySearch((long)frameStart * (long)_waveBuffer.bpf);
			if (num28 < 0)
			{
				num28 = ~num28;
			}
			for (int num29 = num28; num29 < _waveBuffer.beats.Count; num29++)
			{
				num27 = (long)((double)_waveBuffer.beats[num29] / _syncFactor);
				if (num27 >= num23 && num27 <= num24)
				{
					num9 = (int)((double)(num27 - num23) / num25 + 0.5);
					if (DrawBeat == BEATDRAWTYPE.Middle)
					{
						g.DrawLine(pBeat, num9, num11 - num26, num9, num11 + num26);
					}
					else if (DrawBeat == BEATDRAWTYPE.Top)
					{
						g.DrawLine(pBeat, num9, -1f, num9, num26);
					}
					else if (DrawBeat == BEATDRAWTYPE.Bottom)
					{
						g.DrawLine(pBeat, num9, (float)height - num26, num9, height);
					}
					else if (DrawBeat == BEATDRAWTYPE.TopBottom)
					{
						g.DrawLine(pBeat, num9, -1f, num9, num26);
						g.DrawLine(pBeat, num9, (float)height - num26, num9, height);
					}
				}
				else if (num27 > num24)
				{
					break;
				}
			}
		}
		if (DrawMarker != 0 && _waveBuffer.marker != null && _waveBuffer.marker.Count > 0)
		{
			float num30 = (float)(height + 1) * _markerLength;
			bool flag = false;
			int num31 = 0;
			long num32 = 0L;
			long num33 = 0L;
			string text = "Auto";
			List<long> list = null;
			using Brush brush5 = new SolidBrush(((DrawMarker & MARKERDRAWTYPE.NameBoxFillInverted) != 0) ? _colorBackground : pMarker.Color);
			using Brush brush4 = new SolidBrush(((DrawMarker & MARKERDRAWTYPE.NameBoxFillInverted) != 0) ? pMarker.Color : _colorBackground);
			SizeF sizeF = g.MeasureString("M", _markerFont);
			num5 = num11 - sizeF.Height / 2f;
			num6 = num5;
			if ((DrawMarker & MARKERDRAWTYPE.NamePositionTop) != 0)
			{
				num5 -= num30 - sizeF.Height / 2f + 1f;
				num6 = num5;
			}
			else if ((DrawMarker & MARKERDRAWTYPE.NamePositionBottom) != 0)
			{
				num5 += num30 - sizeF.Height / 2f + 1f;
				num6 = num5;
			}
			else if ((DrawMarker & MARKERDRAWTYPE.NamePositionMiddle) == 0)
			{
				num5 -= num30 - sizeF.Height / 2f + 1f;
				num6 += num30 - sizeF.Height / 2f + 1f;
				list = new List<long>(_waveBuffer.marker.Values);
				list.Sort();
				flag = true;
			}
			foreach (string key in _waveBuffer.marker.Keys)
			{
				num32 = Position2Playback(_waveBuffer.marker[key]);
				num9 = (int)((double)(num32 - num23) / num25 + 0.5);
				if (flag)
				{
					num31 = list.IndexOf(_waveBuffer.marker[key]) % 2;
				}
				string text2 = key;
				int num34 = text2.IndexOf('{');
				if (num34 > 0)
				{
					text2 = text2.Substring(0, num34);
				}
				Color color = Color.Empty;
				int num35 = key.LastIndexOf("{Color=");
				if (num35 > 0 && key.EndsWith("}"))
				{
					try
					{
						int num36 = key.IndexOf('}', num35);
						string text3 = key.Substring(num35 + 7, num36 - num35 - 7);
						color = ((!text3.StartsWith("0x")) ? Color.FromName(text3) : Color.FromArgb(int.Parse(text3.Substring(2), NumberStyles.HexNumber)));
					}
					catch
					{
					}
				}
				int num37 = key.LastIndexOf("{Length=");
				if (num37 > 0 && key.EndsWith("}"))
				{
					int num38 = key.IndexOf('}', num37);
					long result = 0L;
					if (long.TryParse(key.Substring(num37 + 8, num38 - num37 - 8), out result) && result > 0)
					{
						num33 = num32 + Position2Playback(result);
						if (num33 >= num23 && num32 <= num24)
						{
							if (num33 > num24)
							{
								num33 = num24;
							}
							nextLevel = (int)((double)(num33 - num23) / num25 + 0.5);
							using Brush brush = new SolidBrush(Color.FromArgb(128, (color == Color.Empty) ? pMarker.Color : color));
							if (num31 == 0)
							{
								g.FillRectangle(brush, (num32 < num23) ? 0f : num9, num11 - 9f, nextLevel - ((num32 < num23) ? 0f : num9), 8f);
							}
							else
							{
								g.FillRectangle(brush, (num32 < num23) ? 0f : num9, num11 + 1f, nextLevel - ((num32 < num23) ? 0f : num9), 8f);
							}
						}
					}
				}
				int num39 = key.LastIndexOf("{Align=");
				if (num39 > 0 && key.EndsWith("}"))
				{
					int num40 = key.IndexOf('}', num39);
					text = key.Substring(num39 + 7, num40 - num39 - 7);
				}
				if (num32 < num23 || num32 > num24)
				{
					continue;
				}
				if (color != Color.Empty)
				{
					using (Pen pen = new Pen(color))
					{
						using Brush brush3 = new SolidBrush(((DrawMarker & MARKERDRAWTYPE.NameBoxFillInverted) != 0) ? _colorBackground : color);
						using Brush brush2 = new SolidBrush(((DrawMarker & MARKERDRAWTYPE.NameBoxFillInverted) != 0) ? color : _colorBackground);
						if ((_drawMarker & MARKERDRAWTYPE.Line) > MARKERDRAWTYPE.None)
						{
							g.DrawLine(pen, num9, num11 - num30, num9, num11 + num30);
						}
						if ((_drawMarker & MARKERDRAWTYPE.Name) <= MARKERDRAWTYPE.None)
						{
							continue;
						}
						sizeF = g.MeasureString(text2, _markerFont);
						switch (text)
						{
						case "Right":
							num9 -= sizeF.Width;
							break;
						case "Center":
							num9 -= sizeF.Width / 2f;
							break;
						case "Auto":
							if (num9 + sizeF.Width > (float)width)
							{
								num9 -= sizeF.Width;
							}
							break;
						}
						if ((_drawMarker & MARKERDRAWTYPE.NameBoxFilled) != 0 || (_drawMarker & MARKERDRAWTYPE.NameBoxFillInverted) != 0)
						{
							if (num31 == 0)
							{
								g.FillRectangle(brush2, num9, num5, sizeF.Width, sizeF.Height);
								g.DrawRectangle(pen, num9, num5, sizeF.Width, sizeF.Height);
							}
							else
							{
								g.FillRectangle(brush2, num9, num6, sizeF.Width, sizeF.Height);
								g.DrawRectangle(pen, num9, num6, sizeF.Width, sizeF.Height);
							}
						}
						else
						{
							num9 -= 3f;
						}
						if (num31 == 0)
						{
							g.DrawString(text2, _markerFont, brush3, num9, num5);
						}
						else
						{
							g.DrawString(text2, _markerFont, brush3, num9, num6);
						}
					}
					continue;
				}
				if ((DrawMarker & MARKERDRAWTYPE.Line) > MARKERDRAWTYPE.None)
				{
					g.DrawLine(pMarker, num9, num11 - num30, num9, num11 + num30);
				}
				if ((DrawMarker & MARKERDRAWTYPE.Name) <= MARKERDRAWTYPE.None)
				{
					continue;
				}
				sizeF = g.MeasureString(text2, _markerFont);
				switch (text)
				{
				case "Right":
					num9 -= sizeF.Width;
					break;
				case "Center":
					num9 -= sizeF.Width / 2f;
					break;
				case "Auto":
					if (num9 + sizeF.Width > (float)width)
					{
						num9 -= sizeF.Width;
					}
					break;
				}
				if ((DrawMarker & MARKERDRAWTYPE.NameBoxFilled) != 0 || (DrawMarker & MARKERDRAWTYPE.NameBoxFillInverted) != 0)
				{
					if (num31 == 0)
					{
						g.FillRectangle(brush4, num9, num5, sizeF.Width, sizeF.Height);
						g.DrawRectangle(pMarker, num9, num5, sizeF.Width, sizeF.Height);
					}
					else
					{
						g.FillRectangle(brush4, num9, num6, sizeF.Width, sizeF.Height);
						g.DrawRectangle(pMarker, num9, num6, sizeF.Width, sizeF.Height);
					}
				}
				else
				{
					num9 -= 3f;
				}
				if (num31 == 0)
				{
					g.DrawString(text2, _markerFont, brush5, num9, num5);
				}
				else
				{
					g.DrawString(text2, _markerFont, brush5, num9, num6);
				}
			}
		}
		if (_drawVolume != 0 && _volumePoints != null && _volumePoints.Count > 0)
		{
			long duration = 0L;
			num5 = GetVolumeLevel(num23, reverse: false, ref duration, ref nextLevel);
			num6 = GetVolumeLevel(num24, reverse: false, ref duration, ref nextLevel);
			int num41 = SearchVolumePoint(num23);
			if (num41 < 0)
			{
				num41 = ~num41;
			}
			num9 = 0f;
			num17 = (float)height - (float)height * num5;
			for (int num42 = num41; num42 < _volumePoints.Count; num42++)
			{
				VolumePoint volumePoint = _volumePoints[num42];
				duration = Position2Playback(volumePoint.Position);
				if (duration >= num23 && duration <= num24)
				{
					nextLevel = (int)((double)(duration - num23) / num25 + 0.5);
					num7 = (float)height - (float)height * volumePoint.Level;
					g.DrawLine(pVolume, num9, num17, nextLevel, num7);
					if ((_drawVolume & VOLUMEDRAWTYPE.NoPoints) == 0)
					{
						g.DrawRectangle(pVolume, nextLevel - 1f, num7 - 1f, 3f, 3f);
						g.DrawRectangle(pVolume, nextLevel - 3f, num7 - 3f, 7f, 7f);
					}
					num9 = nextLevel;
					num17 = num7;
				}
				else if (duration > num24)
				{
					break;
				}
			}
			nextLevel = width;
			num7 = (float)height - (float)height * num6;
			g.DrawLine(pVolume, num9, num17, nextLevel, num7);
		}
		else if (_drawVolume != 0)
		{
			num7 = (VolumeCurveZeroLevel ? ((float)height) : 0f);
			g.DrawLine(pVolume, 0f, num7, width, num7);
		}
	}

	private void DrawBitmap(Graphics g, int width, int height, int frameStart, int frameEnd, Pen pMiddleL, Pen pMiddleR, Pen pLeftUpper, Pen pLeftLower, Pen pLeftEnv, Pen pRightUpper, Pen pRightLower, Pen pRightEnv, Pen pMarker, Pen pBeat, Pen pVolume)
	{
		WAVEFORMDRAWTYPE wAVEFORMDRAWTYPE = _drawWaveForm;
		if (_waveBuffer.chans == 1 && (wAVEFORMDRAWTYPE == WAVEFORMDRAWTYPE.Stereo || wAVEFORMDRAWTYPE == WAVEFORMDRAWTYPE.DualMono))
		{
			wAVEFORMDRAWTYPE = WAVEFORMDRAWTYPE.Mono;
		}
		if (frameEnd >= _waveBuffer.data.Length)
		{
			frameEnd = _waveBuffer.data.Length - 1;
		}
		if (frameEnd < 0)
		{
			frameEnd = _waveBuffer.data.Length - 1;
		}
		if (frameStart < 0)
		{
			frameStart = 0;
		}
		short num = 0;
		short num2 = 0;
		short num3 = 0;
		short num4 = 0;
		float num5 = 0f;
		float num6 = 0f;
		float num7 = 0f;
		float num8 = 0f;
		float num9 = -1f;
		float nextLevel = -1f;
		double num10 = (double)(frameEnd - frameStart) / (double)width;
		float num11 = (float)height / 2f - 1f;
		float num12 = ((float)height - 2f) / 4f - 1f;
		float num13 = ((float)height - 1f) / 4f;
		float num14 = (float)height - num13 - 1f;
		float num15 = num13;
		float num16 = num14;
		float num17 = num13;
		float num18 = num14;
		float num19 = 0f;
		if (wAVEFORMDRAWTYPE != 0)
		{
			num12 = ((float)height - 3f) / 2f - 1f;
			num13 = (num14 = num11);
			num15 = num13;
			num16 = num14;
			num17 = num13;
			num18 = num14;
		}
		height--;
		if (frameStart > 0)
		{
			num15 = (num17 = num13 - (float)_waveBuffer.data[frameStart - 1].left * (num12 / 32767f) * _gainFactor);
			num16 = (num18 = num14 - (float)_waveBuffer.data[frameStart - 1].right * (num12 / 32767f) * _gainFactor);
			switch (wAVEFORMDRAWTYPE)
			{
			case WAVEFORMDRAWTYPE.Mono:
				num15 = (num16 = (num15 + num16) / 2f);
				break;
			case WAVEFORMDRAWTYPE.HalfMono:
			case WAVEFORMDRAWTYPE.HalfMonoFlipped:
				num15 = (num16 = (num15 + num16) / 2f);
				num15 = ((!(num15 <= num13)) ? (num16 = 2f * (2f * num13 - num15)) : (num16 = 2f * num15));
				break;
			}
		}
		g.Clear(_colorBackground);
		for (int i = frameStart; i <= frameEnd; i++)
		{
			if (_waveBuffer.data[i].left > num)
			{
				num = _waveBuffer.data[i].left;
			}
			if (_waveBuffer.data[i].right > num2)
			{
				num2 = _waveBuffer.data[i].right;
			}
			if (_waveBuffer.data[i].left < num3)
			{
				num3 = _waveBuffer.data[i].left;
			}
			if (_waveBuffer.data[i].right < num4)
			{
				num4 = _waveBuffer.data[i].right;
			}
			num9 = (int)((double)(i - frameStart) / num10 + 0.5);
			if (!(num9 > nextLevel))
			{
				continue;
			}
			if (num9 - nextLevel > 1f)
			{
				if (num3 == short.MinValue)
				{
					num = num3;
				}
				else if (num < 0 && num3 < 0 && num > num3)
				{
					num = num3;
				}
				else if (num > 0 && num3 > 0 && num < num3)
				{
					num = num3;
				}
				else if (num < 0 && num > -num3)
				{
					num = num3;
				}
				else if (num > 0 && num < -num3)
				{
					num = num3;
				}
				if (num4 == short.MinValue)
				{
					num2 = num4;
				}
				else if (num2 < 0 && num4 < 0 && num2 > num4)
				{
					num2 = num4;
				}
				else if (num2 > 0 && num4 > 0 && num2 < num4)
				{
					num2 = num4;
				}
				else if (num2 < 0 && num2 > -num4)
				{
					num2 = num4;
				}
				else if (num2 > 0 && num2 < -num4)
				{
					num2 = num4;
				}
				num5 = num13 - (float)num * (num12 / 32767f) * _gainFactor;
				num6 = num14 - (float)num2 * (num12 / 32767f) * _gainFactor;
				num7 = num13 - (float)num3 * (num12 / 32767f) * _gainFactor;
				num8 = num14 - (float)num4 * (num12 / 32767f) * _gainFactor;
				switch (wAVEFORMDRAWTYPE)
				{
				case WAVEFORMDRAWTYPE.Mono:
					num5 = (num6 = (num5 + num6) / 2f);
					num7 = (num8 = (num7 + num8) / 2f);
					if (num5 > num13)
					{
						num5 = num13;
					}
					if (num7 < num13)
					{
						num7 = num13;
					}
					if (num6 > num14)
					{
						num6 = num14;
					}
					if (num8 < num14)
					{
						num8 = num14;
					}
					break;
				case WAVEFORMDRAWTYPE.HalfMono:
				case WAVEFORMDRAWTYPE.HalfMonoFlipped:
					num5 = (num6 = (num5 + num6) / 2f);
					num5 = ((!(num5 <= num13)) ? (num6 = 2f * (2f * num13 - num5)) : (num6 = 2f * num5));
					num7 = (num8 = (num7 + num8) / 2f);
					num7 = ((!(num7 <= num13)) ? (num8 = 2f * (2f * num13 - num7)) : (num8 = 2f * num7));
					break;
				default:
					if (num5 > num13)
					{
						num5 = num13;
					}
					if (num7 < num13)
					{
						num7 = num13;
					}
					if (num6 > num14)
					{
						num6 = num14;
					}
					if (num8 < num14)
					{
						num8 = num14;
					}
					break;
				}
				float num20 = (num5 - num15) / (num9 - nextLevel);
				float num21 = (num6 - num16) / (num9 - nextLevel);
				float num22 = (num7 - num17) / (num9 - nextLevel);
				float num23 = (num8 - num18) / (num9 - nextLevel);
				switch (wAVEFORMDRAWTYPE)
				{
				case WAVEFORMDRAWTYPE.Stereo:
				{
					for (int k = 0; (float)k < num9 - nextLevel; k++)
					{
						num19 = num15 + (float)k * num20;
						g.DrawLine(pLeftUpper, nextLevel + (float)k, num13, nextLevel + (float)k, num19);
						num19 = num17 + (float)k * num22;
						g.DrawLine(pLeftLower, nextLevel + (float)k, num13, nextLevel + (float)k, num19);
						num19 = num16 + (float)k * num21;
						g.DrawLine(pRightUpper, nextLevel + (float)k, num14, nextLevel + (float)k, num19);
						num19 = num18 + (float)k * num23;
						g.DrawLine(pRightLower, nextLevel + (float)k, num14, nextLevel + (float)k, num19);
					}
					if (_drawEnvelope)
					{
						g.DrawLine(pLeftEnv, nextLevel, num15, num9, num5);
						g.DrawLine(pLeftEnv, nextLevel, num17, num9, num7);
						g.DrawLine(pRightEnv, nextLevel, num16, num9, num6);
						g.DrawLine(pRightEnv, nextLevel, num18, num9, num8);
					}
					break;
				}
				case WAVEFORMDRAWTYPE.Mono:
				{
					for (int l = 0; (float)l < num9 - nextLevel; l++)
					{
						num19 = num15 + (float)l * num20;
						g.DrawLine(pLeftUpper, nextLevel + (float)l, num13, nextLevel + (float)l, num19);
						num19 = num17 + (float)l * num22;
						g.DrawLine(pLeftLower, nextLevel + (float)l, num13, nextLevel + (float)l, num19);
					}
					if (_drawEnvelope)
					{
						g.DrawLine(pLeftEnv, nextLevel, num15, num9, num5);
						g.DrawLine(pLeftEnv, nextLevel, num17, num9, num7);
					}
					break;
				}
				case WAVEFORMDRAWTYPE.HalfMono:
				{
					for (int m = 0; (float)m < num9 - nextLevel; m++)
					{
						if (num15 + (float)m * num20 < num17 + (float)m * num22)
						{
							g.DrawLine(pLeftUpper, nextLevel + (float)m, height, nextLevel + (float)m, num15 + (float)m * num20);
						}
						else
						{
							g.DrawLine(pLeftUpper, nextLevel + (float)m, height, nextLevel + (float)m, num17 + (float)m * num22);
						}
					}
					if (_drawEnvelope)
					{
						if (num5 < num7)
						{
							g.DrawLine(pLeftEnv, nextLevel, num15, num9, num5);
						}
						else
						{
							g.DrawLine(pLeftEnv, nextLevel, num17, num9, num7);
						}
					}
					break;
				}
				case WAVEFORMDRAWTYPE.HalfMonoFlipped:
				{
					for (int n = 0; (float)n < num9 - nextLevel; n++)
					{
						if (num15 + (float)n * num20 < num17 + (float)n * num22)
						{
							g.DrawLine(pLeftUpper, nextLevel + (float)n, 0f, nextLevel + (float)n, (float)height - (num15 + (float)n * num20));
						}
						else
						{
							g.DrawLine(pLeftUpper, nextLevel + (float)n, 0f, nextLevel + (float)n, (float)height - (num17 + (float)n * num22));
						}
					}
					if (_drawEnvelope)
					{
						if (num5 < num7)
						{
							g.DrawLine(pLeftEnv, nextLevel, (float)height - num15, num9, (float)height - num5);
						}
						else
						{
							g.DrawLine(pLeftEnv, nextLevel, (float)height - num17, num9, (float)height - num7);
						}
					}
					break;
				}
				default:
				{
					for (int j = 0; (float)j < num9 - nextLevel; j++)
					{
						num19 = num15 + (float)j * num20;
						g.DrawLine(pLeftUpper, nextLevel + (float)j, num13, nextLevel + (float)j, num19);
						num19 = num17 + (float)j * num22;
						g.DrawLine(pLeftLower, nextLevel + (float)j, num13, nextLevel + (float)j, num19);
						num19 = num16 + (float)j * num21;
						g.DrawLine(pRightUpper, nextLevel + (float)j, num14, nextLevel + (float)j, num19);
						num19 = num18 + (float)j * num23;
						g.DrawLine(pRightLower, nextLevel + (float)j, num14, nextLevel + (float)j, num19);
					}
					if (_drawEnvelope)
					{
						g.DrawLine(pLeftEnv, nextLevel, num15, num9, num5);
						g.DrawLine(pLeftEnv, nextLevel, num17, num9, num7);
						g.DrawLine(pRightEnv, nextLevel, num16, num9, num6);
						g.DrawLine(pRightEnv, nextLevel, num18, num9, num8);
					}
					break;
				}
				}
				num15 = num5;
				num16 = num6;
				num17 = num7;
				num18 = num8;
			}
			else
			{
				num5 = num13 - (float)num * (num12 / 32767f) * _gainFactor;
				num6 = num14 - (float)num2 * (num12 / 32767f) * _gainFactor;
				num7 = num13 - (float)num3 * (num12 / 32767f) * _gainFactor;
				num8 = num14 - (float)num4 * (num12 / 32767f) * _gainFactor;
				switch (wAVEFORMDRAWTYPE)
				{
				case WAVEFORMDRAWTYPE.Mono:
					num5 = (num6 = (num5 + num6) / 2f);
					num7 = (num8 = (num7 + num8) / 2f);
					if (num5 > num13)
					{
						num5 = num13;
					}
					if (num7 < num13)
					{
						num7 = num13;
					}
					if (num6 > num14)
					{
						num6 = num14;
					}
					if (num8 < num14)
					{
						num8 = num14;
					}
					break;
				case WAVEFORMDRAWTYPE.HalfMono:
				case WAVEFORMDRAWTYPE.HalfMonoFlipped:
					num5 = (num6 = (num5 + num6) / 2f);
					num5 = ((!(num5 <= num13)) ? (num6 = 2f * (2f * num13 - num5)) : (num6 = 2f * num5));
					num7 = (num8 = (num7 + num8) / 2f);
					num7 = ((!(num7 <= num13)) ? (num8 = 2f * (2f * num13 - num7)) : (num8 = 2f * num7));
					break;
				default:
					if (num5 > num13)
					{
						num5 = num13;
					}
					if (num7 < num13)
					{
						num7 = num13;
					}
					if (num6 > num14)
					{
						num6 = num14;
					}
					if (num8 < num14)
					{
						num8 = num14;
					}
					break;
				}
				switch (wAVEFORMDRAWTYPE)
				{
				case WAVEFORMDRAWTYPE.Stereo:
					g.DrawLine(pLeftUpper, nextLevel, num13, nextLevel, num5);
					g.DrawLine(pLeftLower, nextLevel, num13, nextLevel, num7);
					g.DrawLine(pRightUpper, nextLevel, num14, nextLevel, num6);
					g.DrawLine(pRightLower, nextLevel, num14, nextLevel, num8);
					if (_drawEnvelope)
					{
						g.DrawLine(pLeftEnv, nextLevel, num15, num9, num5);
						g.DrawLine(pLeftEnv, nextLevel, num17, num9, num7);
						g.DrawLine(pRightEnv, nextLevel, num16, num9, num6);
						g.DrawLine(pRightEnv, nextLevel, num18, num9, num8);
					}
					break;
				case WAVEFORMDRAWTYPE.Mono:
					g.DrawLine(pLeftUpper, nextLevel, num13, nextLevel, num5);
					g.DrawLine(pLeftLower, nextLevel, num13, nextLevel, num7);
					if (_drawEnvelope)
					{
						g.DrawLine(pLeftEnv, nextLevel, num15, num9, num5);
						g.DrawLine(pLeftEnv, nextLevel, num17, num9, num7);
					}
					break;
				case WAVEFORMDRAWTYPE.HalfMono:
					if (num5 < num7)
					{
						g.DrawLine(pLeftUpper, nextLevel, height, nextLevel, num5);
					}
					else
					{
						g.DrawLine(pLeftUpper, nextLevel, height, nextLevel, num7);
					}
					if (_drawEnvelope)
					{
						if (num5 < num7)
						{
							g.DrawLine(pLeftEnv, nextLevel, num15, num9, num5);
						}
						else
						{
							g.DrawLine(pLeftEnv, nextLevel, num17, num9, num7);
						}
					}
					break;
				case WAVEFORMDRAWTYPE.HalfMonoFlipped:
					if (num5 < num7)
					{
						g.DrawLine(pLeftUpper, nextLevel, 0f, nextLevel, (float)height - num5);
					}
					else
					{
						g.DrawLine(pLeftUpper, nextLevel, 0f, nextLevel, (float)height - num7);
					}
					if (_drawEnvelope)
					{
						if (num5 < num7)
						{
							g.DrawLine(pLeftEnv, nextLevel, (float)height - num15, num9, (float)height - num5);
						}
						else
						{
							g.DrawLine(pLeftEnv, nextLevel, (float)height - num17, num9, (float)height - num7);
						}
					}
					break;
				default:
					g.DrawLine(pLeftUpper, nextLevel, num13, nextLevel, num5);
					g.DrawLine(pLeftLower, nextLevel, num13, nextLevel, num7);
					g.DrawLine(pRightUpper, nextLevel, num14, nextLevel, num6);
					g.DrawLine(pRightLower, nextLevel, num14, nextLevel, num8);
					if (_drawEnvelope)
					{
						g.DrawLine(pLeftEnv, nextLevel, num15, num9, num5);
						g.DrawLine(pLeftEnv, nextLevel, num17, num9, num7);
						g.DrawLine(pRightEnv, nextLevel, num16, num9, num6);
						g.DrawLine(pRightEnv, nextLevel, num18, num9, num8);
					}
					break;
				}
			}
			nextLevel = num9;
			num = 0;
			num2 = 0;
			num3 = 0;
			num4 = 0;
			num15 = num5;
			num16 = num6;
			num17 = num7;
			num18 = num8;
		}
		if (_drawCenterLine && wAVEFORMDRAWTYPE != WAVEFORMDRAWTYPE.HalfMono && wAVEFORMDRAWTYPE != WAVEFORMDRAWTYPE.HalfMonoFlipped)
		{
			g.DrawLine(pMiddleL, 0f, num13, width, num13);
			if (wAVEFORMDRAWTYPE == WAVEFORMDRAWTYPE.Stereo)
			{
				g.DrawLine(pMiddleR, 0f, num14, width, num14);
			}
		}
		long num24 = Frame2Bytes(frameStart);
		long num25 = Frame2Bytes(frameEnd);
		double num26 = (double)(num25 - num24) / (double)width;
		if (DrawBeat != 0 && _waveBuffer.beats != null && _waveBuffer.beats.Count > 0)
		{
			float num27 = (float)(height + 1) * _beatLength;
			long num28 = 0L;
			int num29 = _waveBuffer.beats.BinarySearch((long)frameStart * (long)_waveBuffer.bpf);
			if (num29 < 0)
			{
				num29 = ~num29;
			}
			for (int num30 = num29; num30 < _waveBuffer.beats.Count; num30++)
			{
				num28 = (long)((double)_waveBuffer.beats[num30] / _syncFactor);
				if (num28 >= num24 && num28 <= num25)
				{
					num9 = (int)((double)(num28 - num24) / num26 + 0.5);
					if (DrawBeat == BEATDRAWTYPE.Middle)
					{
						g.DrawLine(pBeat, num9, num11 - num27, num9, num11 + num27);
					}
					else if (DrawBeat == BEATDRAWTYPE.Top)
					{
						g.DrawLine(pBeat, num9, -1f, num9, num27);
					}
					else if (DrawBeat == BEATDRAWTYPE.Bottom)
					{
						g.DrawLine(pBeat, num9, (float)height - num27, num9, height);
					}
					else if (DrawBeat == BEATDRAWTYPE.TopBottom)
					{
						g.DrawLine(pBeat, num9, -1f, num9, num27);
						g.DrawLine(pBeat, num9, (float)height - num27, num9, height);
					}
				}
				else if (num28 > num25)
				{
					break;
				}
			}
		}
		if (DrawMarker != 0 && _waveBuffer.marker != null && _waveBuffer.marker.Count > 0)
		{
			float num31 = (float)(height + 1) * _markerLength;
			bool flag = false;
			int num32 = 0;
			long num33 = 0L;
			long num34 = 0L;
			string text = "Auto";
			List<long> list = null;
			using Brush brush5 = new SolidBrush(((DrawMarker & MARKERDRAWTYPE.NameBoxFillInverted) != 0) ? _colorBackground : pMarker.Color);
			using Brush brush4 = new SolidBrush(((DrawMarker & MARKERDRAWTYPE.NameBoxFillInverted) != 0) ? pMarker.Color : _colorBackground);
			SizeF sizeF = g.MeasureString("M", _markerFont);
			num5 = num11 - sizeF.Height / 2f;
			num6 = num5;
			if ((DrawMarker & MARKERDRAWTYPE.NamePositionTop) != 0)
			{
				num5 -= num31 - sizeF.Height / 2f + 1f;
				num6 = num5;
			}
			else if ((DrawMarker & MARKERDRAWTYPE.NamePositionBottom) != 0)
			{
				num5 += num31 - sizeF.Height / 2f + 1f;
				num6 = num5;
			}
			else if ((DrawMarker & MARKERDRAWTYPE.NamePositionMiddle) == 0)
			{
				num5 -= num31 - sizeF.Height / 2f + 1f;
				num6 += num31 - sizeF.Height / 2f + 1f;
				list = new List<long>(_waveBuffer.marker.Values);
				list.Sort();
				flag = true;
			}
			foreach (string key in _waveBuffer.marker.Keys)
			{
				num33 = Position2Playback(_waveBuffer.marker[key]);
				num9 = (int)((double)(num33 - num24) / num26 + 0.5);
				if (flag)
				{
					num32 = list.IndexOf(_waveBuffer.marker[key]) % 2;
				}
				string text2 = key;
				int num35 = text2.IndexOf('{');
				if (num35 > 0)
				{
					text2 = text2.Substring(0, num35);
				}
				Color color = Color.Empty;
				int num36 = key.LastIndexOf("{Color=");
				if (num36 > 0 && key.EndsWith("}"))
				{
					try
					{
						int num37 = key.IndexOf('}', num36);
						string text3 = key.Substring(num36 + 7, num37 - num36 - 7);
						color = ((!text3.StartsWith("0x")) ? Color.FromName(text3) : Color.FromArgb(int.Parse(text3.Substring(2), NumberStyles.HexNumber)));
					}
					catch
					{
					}
				}
				int num38 = key.LastIndexOf("{Length=");
				if (num38 > 0 && key.EndsWith("}"))
				{
					int num39 = key.IndexOf('}', num38);
					long result = 0L;
					if (long.TryParse(key.Substring(num38 + 8, num39 - num38 - 8), out result) && result > 0)
					{
						num34 = num33 + Position2Playback(result);
						if (num34 >= num24 && num33 <= num25)
						{
							if (num34 > num25)
							{
								num34 = num25;
							}
							nextLevel = (int)((double)(num34 - num24) / num26 + 0.5);
							using Brush brush = new SolidBrush(Color.FromArgb(128, (color == Color.Empty) ? pMarker.Color : color));
							if (num32 == 0)
							{
								g.FillRectangle(brush, (num33 < num24) ? 0f : num9, num11 - 8f, nextLevel - ((num33 < num24) ? 0f : num9), 8f);
							}
							else
							{
								g.FillRectangle(brush, (num33 < num24) ? 0f : num9, num11 + 1f, nextLevel - ((num33 < num24) ? 0f : num9), 8f);
							}
						}
					}
				}
				int num40 = key.LastIndexOf("{Align=");
				if (num40 > 0 && key.EndsWith("}"))
				{
					int num41 = key.IndexOf('}', num40);
					text = key.Substring(num40 + 7, num41 - num40 - 7);
				}
				if (num33 < num24 || num33 > num25)
				{
					continue;
				}
				if (color != Color.Empty)
				{
					using (Pen pen = new Pen(color))
					{
						using Brush brush3 = new SolidBrush(((DrawMarker & MARKERDRAWTYPE.NameBoxFillInverted) != 0) ? _colorBackground : color);
						using Brush brush2 = new SolidBrush(((DrawMarker & MARKERDRAWTYPE.NameBoxFillInverted) != 0) ? color : _colorBackground);
						if ((DrawMarker & MARKERDRAWTYPE.Line) > MARKERDRAWTYPE.None)
						{
							g.DrawLine(pen, num9, num11 - num31, num9, num11 + num31);
						}
						if ((DrawMarker & MARKERDRAWTYPE.Name) <= MARKERDRAWTYPE.None)
						{
							continue;
						}
						sizeF = g.MeasureString(text2, _markerFont);
						switch (text)
						{
						case "Right":
							num9 -= sizeF.Width;
							break;
						case "Center":
							num9 -= sizeF.Width / 2f;
							break;
						case "Auto":
							if (num9 + sizeF.Width > (float)width)
							{
								num9 -= sizeF.Width;
							}
							break;
						}
						if ((DrawMarker & MARKERDRAWTYPE.NameBoxFilled) != 0 || (DrawMarker & MARKERDRAWTYPE.NameBoxFillInverted) != 0)
						{
							if (num32 == 0)
							{
								g.FillRectangle(brush2, num9, num5, sizeF.Width, sizeF.Height);
								g.DrawRectangle(pen, num9, num5, sizeF.Width, sizeF.Height);
							}
							else
							{
								g.FillRectangle(brush2, num9, num6, sizeF.Width, sizeF.Height);
								g.DrawRectangle(pen, num9, num6, sizeF.Width, sizeF.Height);
							}
						}
						else
						{
							num9 -= 3f;
						}
						if (num32 == 0)
						{
							g.DrawString(text2, _markerFont, brush3, num9, num5);
						}
						else
						{
							g.DrawString(text2, _markerFont, brush3, num9, num6);
						}
					}
					continue;
				}
				if ((DrawMarker & MARKERDRAWTYPE.Line) > MARKERDRAWTYPE.None)
				{
					g.DrawLine(pMarker, num9, num11 - num31, num9, num11 + num31);
				}
				if ((DrawMarker & MARKERDRAWTYPE.Name) <= MARKERDRAWTYPE.None)
				{
					continue;
				}
				sizeF = g.MeasureString(text2, _markerFont);
				switch (text)
				{
				case "Right":
					num9 -= sizeF.Width;
					break;
				case "Center":
					num9 -= sizeF.Width / 2f;
					break;
				case "Auto":
					if (num9 + sizeF.Width > (float)width)
					{
						num9 -= sizeF.Width;
					}
					break;
				}
				if ((DrawMarker & MARKERDRAWTYPE.NameBoxFilled) != 0 || (DrawMarker & MARKERDRAWTYPE.NameBoxFillInverted) != 0)
				{
					if (num32 == 0)
					{
						g.FillRectangle(brush4, num9, num5, sizeF.Width, sizeF.Height);
						g.DrawRectangle(pMarker, num9, num5, sizeF.Width, sizeF.Height);
					}
					else
					{
						g.FillRectangle(brush4, num9, num6, sizeF.Width, sizeF.Height);
						g.DrawRectangle(pMarker, num9, num6, sizeF.Width, sizeF.Height);
					}
				}
				else
				{
					num9 -= 3f;
				}
				if (num32 == 0)
				{
					g.DrawString(text2, _markerFont, brush5, num9, num5);
				}
				else
				{
					g.DrawString(text2, _markerFont, brush5, num9, num6);
				}
			}
		}
		if (_drawVolume != 0 && _volumePoints != null && _volumePoints.Count > 0)
		{
			long duration = 0L;
			num5 = GetVolumeLevel(num24, reverse: false, ref duration, ref nextLevel);
			num6 = GetVolumeLevel(num25, reverse: false, ref duration, ref nextLevel);
			int num42 = SearchVolumePoint(num24);
			if (num42 < 0)
			{
				num42 = ~num42;
			}
			num9 = 0f;
			num17 = (float)height - (float)height * num5;
			for (int num43 = num42; num43 < _volumePoints.Count; num43++)
			{
				VolumePoint volumePoint = _volumePoints[num43];
				duration = Position2Playback(volumePoint.Position);
				if (duration >= num24 && duration <= num25)
				{
					nextLevel = (int)((double)(duration - num24) / num26 + 0.5);
					num7 = (float)height - (float)height * volumePoint.Level;
					g.DrawLine(pVolume, num9, num17, nextLevel, num7);
					if ((_drawVolume & VOLUMEDRAWTYPE.NoPoints) == 0)
					{
						g.DrawRectangle(pVolume, nextLevel - 1f, num7 - 1f, 3f, 3f);
						g.DrawRectangle(pVolume, nextLevel - 3f, num7 - 3f, 7f, 7f);
					}
					num9 = nextLevel;
					num17 = num7;
				}
				else if (duration > num25)
				{
					break;
				}
			}
			nextLevel = width;
			num7 = (float)height - (float)height * num6;
			g.DrawLine(pVolume, num9, num17, nextLevel, num7);
		}
		else if (_drawVolume != 0)
		{
			num7 = (VolumeCurveZeroLevel ? ((float)height) : 0f);
			g.DrawLine(pVolume, 0f, num7, width, num7);
		}
	}

	public bool WaveFormSaveToFile(string fileName)
	{
		return WaveFormSaveToFile(fileName, binary: false);
	}

	public bool WaveFormSaveToFile(string fileName, bool binary)
	{
		if (!IsRendered)
		{
			return false;
		}
		bool result = false;
		Stream stream = null;
		try
		{
			stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
			SerializeWaveForm(stream, binary);
			result = true;
		}
		catch
		{
		}
		finally
		{
			if (stream != null)
			{
				stream.Flush();
				stream.Close();
			}
		}
		return result;
	}

	public byte[] WaveFormSaveToMemory()
	{
		return WaveFormSaveToMemory(binary: false);
	}

	public byte[] WaveFormSaveToMemory(bool binary)
	{
		if (!IsRendered)
		{
			return null;
		}
		byte[] array = null;
		Stream stream = null;
		try
		{
			stream = new MemoryStream();
			SerializeWaveForm(stream, binary);
			array = new byte[stream.Length];
			stream.Position = 0L;
			stream.Read(array, 0, array.Length);
		}
		catch
		{
		}
		finally
		{
			if (stream != null)
			{
				stream.Flush();
				stream.Close();
			}
		}
		return array;
	}

	private void SerializeWaveForm(Stream stream, bool binary)
	{
		lock (_syncRoot)
		{
			if (binary)
			{
				BinaryWriter binaryWriter = new BinaryWriter(stream);
				binaryWriter.Write(1);
				binaryWriter.Write((_waveBuffer.fileName == null) ? string.Empty : _waveBuffer.fileName);
				binaryWriter.Write(_waveBuffer.bpf);
				binaryWriter.Write(_waveBuffer.chans);
				binaryWriter.Write((int)_waveBuffer.flags);
				binaryWriter.Write(_waveBuffer.resolution);
				if (_waveBuffer.beats != null)
				{
					binaryWriter.Write(_waveBuffer.beats.Count);
					foreach (long beat in _waveBuffer.beats)
					{
						binaryWriter.Write(beat);
					}
				}
				else
				{
					binaryWriter.Write(0);
				}
				if (_waveBuffer.marker != null)
				{
					binaryWriter.Write(_waveBuffer.marker.Count);
					foreach (KeyValuePair<string, long> item in _waveBuffer.marker)
					{
						binaryWriter.Write(item.Key);
						binaryWriter.Write(item.Value);
					}
				}
				else
				{
					binaryWriter.Write(0);
				}
				if (_waveBuffer.data != null)
				{
					binaryWriter.Write(_waveBuffer.data.Length);
					WaveBuffer.Level[] data = _waveBuffer.data;
					for (int i = 0; i < data.Length; i++)
					{
						WaveBuffer.Level level = data[i];
						binaryWriter.Write(level.left);
						binaryWriter.Write(level.right);
					}
				}
				else
				{
					binaryWriter.Write(0);
				}
			}
			else
			{
				new BinaryFormatter().Serialize(stream, _waveBuffer);
			}
		}
	}

	public bool WaveFormLoadFromFile(string fileName)
	{
		return WaveFormLoadFromFile(fileName, binary: false);
	}

	public bool WaveFormLoadFromFile(string fileName, bool binary)
	{
		bool result = true;
		Stream stream = null;
		try
		{
			stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			DeserializeWaveForm(stream, binary);
			if (_waveBuffer != null)
			{
				_isRendered = true;
				_fileName = _waveBuffer.fileName;
				_framesRendered = _waveBuffer.data.Length;
				_framesToRender = _waveBuffer.data.Length;
				_frameResolution = _waveBuffer.resolution;
			}
		}
		catch
		{
			result = false;
		}
		finally
		{
			stream?.Close();
		}
		return result;
	}

	public bool WaveFormLoadFromMemory(byte[] data)
	{
		return WaveFormLoadFromMemory(data, binary: false);
	}

	public bool WaveFormLoadFromMemory(byte[] data, bool binary)
	{
		bool result = true;
		Stream stream = null;
		try
		{
			stream = new MemoryStream(data);
			DeserializeWaveForm(stream, binary);
			if (_waveBuffer != null)
			{
				_isRendered = true;
				_fileName = _waveBuffer.fileName;
				_framesRendered = _waveBuffer.data.Length;
				_framesToRender = _waveBuffer.data.Length;
				_frameResolution = _waveBuffer.resolution;
			}
		}
		catch
		{
			result = false;
		}
		finally
		{
			stream?.Close();
		}
		return result;
	}

	private void DeserializeWaveForm(Stream stream, bool binary)
	{
		lock (_syncRoot)
		{
			if (binary)
			{
				BinaryReader binaryReader = new BinaryReader(stream);
				binaryReader.ReadInt32();
				_waveBuffer = new WaveBuffer();
				_waveBuffer.fileName = binaryReader.ReadString();
				_waveBuffer.bpf = binaryReader.ReadInt32();
				_waveBuffer.chans = binaryReader.ReadInt32();
				_waveBuffer.flags = (BASSFlag)binaryReader.ReadInt32();
				_waveBuffer.resolution = binaryReader.ReadDouble();
				int num = binaryReader.ReadInt32();
				if (num > 0)
				{
					_waveBuffer.beats = new List<long>(num);
					for (int i = 0; i < num; i++)
					{
						_waveBuffer.beats.Add(binaryReader.ReadInt64());
					}
				}
				int num2 = binaryReader.ReadInt32();
				if (num2 > 0)
				{
					_waveBuffer.marker = new Dictionary<string, long>(num2);
					for (int j = 0; j < num2; j++)
					{
						string key = binaryReader.ReadString();
						long value = binaryReader.ReadInt64();
						_waveBuffer.marker[key] = value;
					}
				}
				int num3 = binaryReader.ReadInt32();
				if (num3 > 0)
				{
					_waveBuffer.data = new WaveBuffer.Level[num3];
					for (int k = 0; k < num3; k++)
					{
						short levelL = binaryReader.ReadInt16();
						short levelR = binaryReader.ReadInt16();
						_waveBuffer.data[k] = new WaveBuffer.Level(levelL, levelR);
					}
				}
				return;
			}
			IFormatter formatter = new BinaryFormatter();
			bool flag = false;
			while (!flag)
			{
				try
				{
					_waveBuffer = formatter.Deserialize(stream) as WaveBuffer;
					flag = true;
				}
				catch
				{
					flag = true;
				}
			}
		}
	}
}

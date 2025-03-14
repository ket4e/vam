using System;
using System.IO;
using System.Security;

namespace Un4seen.Bass.Misc;

[SuppressUnmanagedCodeSecurity]
public sealed class WaveWriter : IDisposable
{
	private enum WaveFormat
	{
		WAVE_FORMAT_UNKNOWN = 0,
		WAVE_FORMAT_PCM = 1,
		WAVE_FORMAT_IEEE_FLOAT = 3,
		WAVE_FORMAT_EXTENSIBLE = 65534
	}

	[Flags]
	private enum WaveSpeakers
	{
		SPEAKER_FRONT_LEFT = 1,
		SPEAKER_FRONT_RIGHT = 2,
		SPEAKER_FRONT_CENTER = 4,
		SPEAKER_LOW_FREQUENCY = 8,
		SPEAKER_BACK_LEFT = 0x10,
		SPEAKER_BACK_RIGHT = 0x20,
		SPEAKER_FRONT_LEFT_OF_CENTER = 0x40,
		SPEAKER_FRONT_RIGHT_OF_CENTER = 0x80,
		SPEAKER_BACK_CENTER = 0x100,
		SPEAKER_SIDE_LEFT = 0x200,
		SPEAKER_SIDE_RIGHT = 0x400,
		SPEAKER_TOP_CENTER = 0x800,
		SPEAKER_TOP_FRONT_LEFT = 0x1000,
		SPEAKER_TOP_FRONT_CENTER = 0x2000,
		SPEAKER_TOP_FRONT_RIGHT = 0x4000,
		SPEAKER_TOP_BACK_LEFT = 0x8000,
		SPEAKER_TOP_BACK_CENTER = 0x10000,
		SPEAKER_TOP_BACK_RIGHT = 0x20000,
		SPEAKER_ALL = int.MinValue
	}

	private bool disposed;

	private const string Guid_KSDATAFORMAT_SUBTYPE_PCM = "00000001-0000-0010-8000-00aa00389b71";

	private const string Guid_KSDATAFORMAT_SUBTYPE_IEEE_FLOAT = "00000003-0000-0010-8000-00aa00389b71";

	private uint _waveHeaderSize = 38u;

	private uint _waveHeaderFormatSize = 18u;

	private FileStream _fs;

	private BinaryWriter _bw;

	private BufferedStream _bs;

	private uint _dataWritten;

	private uint _offsetDataSize = 42u;

	private uint _offsetSampleSize = 46u;

	private string _fileName = string.Empty;

	private int _numChannels = 2;

	private int _sampleRate = 44100;

	private int _bitsPerSample = 16;

	private int _origResolution = 16;

	public string FileName
	{
		get
		{
			return _fileName;
		}
		set
		{
			_fileName = value;
		}
	}

	public int NumChannels => _numChannels;

	public int SampleRate => _sampleRate;

	public int BitsPerSample
	{
		get
		{
			return _bitsPerSample;
		}
		set
		{
			if (value == 8 || value == 16 || value == 24 || value == 32)
			{
				_bitsPerSample = value;
			}
		}
	}

	public int OrigResolution
	{
		get
		{
			return _origResolution;
		}
		set
		{
			if (value == 8 || value == 16 || value == 32)
			{
				_origResolution = value;
			}
		}
	}

	public WaveWriter(string fileName, int stream, bool rewrite)
	{
		FileName = fileName;
		BASS_CHANNELINFO bASS_CHANNELINFO = new BASS_CHANNELINFO();
		if (Bass.BASS_ChannelGetInfo(stream, bASS_CHANNELINFO))
		{
			if ((bASS_CHANNELINFO.flags & BASSFlag.BASS_SAMPLE_FLOAT) != 0)
			{
				BitsPerSample = 32;
			}
			else if ((bASS_CHANNELINFO.flags & BASSFlag.BASS_SAMPLE_8BITS) != 0)
			{
				BitsPerSample = 8;
			}
			else
			{
				BitsPerSample = 16;
			}
			OrigResolution = BitsPerSample;
			_numChannels = bASS_CHANNELINFO.chans;
			_sampleRate = bASS_CHANNELINFO.freq;
			Initialize(rewrite);
			return;
		}
		throw new ArgumentException("Could not retrieve channel information!");
	}

	public WaveWriter(string fileName, int stream, int bitsPerSample, bool rewrite)
	{
		FileName = fileName;
		BASS_CHANNELINFO bASS_CHANNELINFO = new BASS_CHANNELINFO();
		if (Bass.BASS_ChannelGetInfo(stream, bASS_CHANNELINFO))
		{
			BitsPerSample = bitsPerSample;
			if ((bASS_CHANNELINFO.flags & BASSFlag.BASS_SAMPLE_FLOAT) != 0)
			{
				OrigResolution = 32;
			}
			else if ((bASS_CHANNELINFO.flags & BASSFlag.BASS_SAMPLE_8BITS) != 0)
			{
				OrigResolution = 8;
			}
			else
			{
				OrigResolution = 16;
			}
			_numChannels = bASS_CHANNELINFO.chans;
			_sampleRate = bASS_CHANNELINFO.freq;
			Initialize(rewrite);
			return;
		}
		throw new ArgumentException("Could not retrieve channel information!");
	}

	public WaveWriter(string fileName, int numChannels, int sampleRate, int bitsPerSample, bool rewrite)
	{
		FileName = fileName;
		BitsPerSample = bitsPerSample;
		if (bitsPerSample > 16)
		{
			OrigResolution = 32;
		}
		else
		{
			OrigResolution = BitsPerSample;
		}
		_numChannels = numChannels;
		_sampleRate = sampleRate;
		Initialize(rewrite);
	}

	private void Initialize(bool rewrite)
	{
		_dataWritten = 0u;
		if (File.Exists(FileName) && !rewrite)
		{
			throw new IOException($"The file {FileName} already exists!");
		}
		_fs = new FileStream(FileName, FileMode.Create);
		_bs = new BufferedStream(_fs, 4096 * BitsPerSample);
		_bw = new BinaryWriter(_bs);
		WriteWaveHeader();
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
			Close();
		}
		disposed = true;
	}

	~WaveWriter()
	{
		Dispose(disposing: false);
	}

	private void WriteWaveHeader()
	{
		try
		{
			_bw.Write(new byte[4] { 82, 73, 70, 70 });
			_bw.Write(0u);
			_bw.Write(new byte[4] { 87, 65, 86, 69 });
			uint num = 0u;
			if (NumChannels > 2)
			{
				num = 22u;
				_waveHeaderSize += num;
				_waveHeaderFormatSize += num;
				_offsetDataSize += num;
				_offsetSampleSize += num;
			}
			_bw.Write(new byte[4] { 102, 109, 116, 32 });
			_bw.Write(_waveHeaderFormatSize);
			if (NumChannels > 2)
			{
				_bw.Write((ushort)65534);
			}
			else if (BitsPerSample > 24)
			{
				_bw.Write((ushort)3);
			}
			else
			{
				_bw.Write((ushort)1);
			}
			int num2 = NumChannels * (BitsPerSample / 8);
			_bw.Write((ushort)NumChannels);
			_bw.Write((uint)SampleRate);
			_bw.Write((uint)(SampleRate * num2));
			_bw.Write((ushort)num2);
			_bw.Write((ushort)BitsPerSample);
			_bw.Write((ushort)num);
			if (NumChannels > 2)
			{
				_bw.Write((ushort)BitsPerSample);
				int value = NumChannels switch
				{
					3 => 7, 
					4 => 51, 
					5 => 55, 
					6 => 63, 
					7 => 319, 
					8 => 255, 
					9 => 511, 
					_ => int.MinValue, 
				};
				_bw.Write((uint)value);
				Guid empty = Guid.Empty;
				empty = ((BitsPerSample <= 24) ? new Guid("00000001-0000-0010-8000-00aa00389b71") : new Guid("00000003-0000-0010-8000-00aa00389b71"));
				_bw.Write(empty.ToByteArray());
			}
			if (NumChannels > 2 || BitsPerSample > 16)
			{
				_bw.Write(new byte[4] { 102, 97, 99, 116 });
				_bw.Write(4u);
				_bw.Write(0u);
				_waveHeaderSize += 12u;
				_offsetDataSize += 12u;
			}
			_bw.Write(new byte[4] { 100, 97, 116, 97 });
			_bw.Write(0u);
		}
		catch (Exception innerException)
		{
			throw new IOException("The Wave Header could not be written.", innerException);
		}
	}

	private void WriteSample(float sample)
	{
		if (BitsPerSample == 32)
		{
			_bw.Write(sample);
			_dataWritten += 4u;
		}
		else if (BitsPerSample == 24)
		{
			_bw.Write(Utils.SampleTo24Bit(sample));
			_dataWritten += 3u;
		}
		else if (BitsPerSample == 16)
		{
			_bw.Write(Utils.SampleTo16Bit(sample));
			_dataWritten += 2u;
		}
		else
		{
			_bw.Write(Utils.SampleTo8Bit(sample));
			_dataWritten++;
		}
	}

	private void WriteSample(short sample)
	{
		if (BitsPerSample == 32)
		{
			_bw.Write(Utils.SampleTo32Bit(sample));
			_dataWritten += 4u;
		}
		else if (BitsPerSample == 24)
		{
			_bw.Write(Utils.SampleTo24Bit(sample));
			_dataWritten += 3u;
		}
		else if (BitsPerSample == 16)
		{
			_bw.Write(sample);
			_dataWritten += 2u;
		}
		else
		{
			_bw.Write(Utils.SampleTo8Bit(sample));
			_dataWritten++;
		}
	}

	private void WriteSample(byte sample)
	{
		if (BitsPerSample == 32)
		{
			_bw.Write(Utils.SampleTo32Bit(sample));
			_dataWritten += 4u;
		}
		else if (BitsPerSample == 24)
		{
			_bw.Write(Utils.SampleTo24Bit(sample));
			_dataWritten += 3u;
		}
		else if (BitsPerSample == 16)
		{
			_bw.Write(Utils.SampleTo16Bit(sample));
			_dataWritten += 2u;
		}
		else
		{
			_bw.Write(sample);
			_dataWritten++;
		}
	}

	private void WriteSampleNoConvert(byte sample)
	{
		_bw.Write(sample);
		_dataWritten++;
	}

	public unsafe void Write(IntPtr buffer, int length)
	{
		if (OrigResolution == 32)
		{
			float* ptr = (float*)(void*)buffer;
			for (int i = 0; i < length / 4; i++)
			{
				WriteSample(ptr[i]);
			}
		}
		else if (OrigResolution == 8)
		{
			byte* ptr2 = (byte*)(void*)buffer;
			for (int j = 0; j < length; j++)
			{
				WriteSample(ptr2[j]);
			}
		}
		else
		{
			short* ptr3 = (short*)(void*)buffer;
			for (int k = 0; k < length / 2; k++)
			{
				WriteSample(ptr3[k]);
			}
		}
	}

	public void Write(float[] buffer, int length)
	{
		for (int i = 0; i < length / 4; i++)
		{
			WriteSample(buffer[i]);
		}
	}

	public void Write(short[] buffer, int length)
	{
		for (int i = 0; i < length / 2; i++)
		{
			WriteSample(buffer[i]);
		}
	}

	public void Write(byte[] buffer, int length)
	{
		for (int i = 0; i < length; i++)
		{
			WriteSample(buffer[i]);
		}
	}

	public void WriteNoConvert(byte[] buffer, int length)
	{
		for (int i = 0; i < length; i++)
		{
			WriteSampleNoConvert(buffer[i]);
		}
	}

	public void Close()
	{
		if (_bs != null && _bw != null && _fs != null)
		{
			_bs.Seek(4L, SeekOrigin.Begin);
			_bw.Write(_dataWritten + _waveHeaderSize);
			if (NumChannels > 2 || BitsPerSample > 16)
			{
				int num = NumChannels * (BitsPerSample / 8);
				_bs.Seek(_offsetSampleSize, SeekOrigin.Begin);
				_bw.Write((uint)(_dataWritten / num));
			}
			_bs.Seek(_offsetDataSize, SeekOrigin.Begin);
			_bw.Write(_dataWritten);
			_bw.Flush();
			_bw.Close();
			_bw = null;
			_bs.Close();
			_bs = null;
			_fs.Close();
			_fs = null;
		}
	}
}

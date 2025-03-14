using System;
using System.Runtime.InteropServices;
using Un4seen.Bass.AddOn.Enc;

namespace Un4seen.Bass.Misc;

[Serializable]
public sealed class EncoderACM : BaseEncoder
{
	public ACMFORMAT ACM_Codec = new ACMFORMAT();

	public bool ACM_WriteWaveHeader = true;

	public BASSChannelType ACM_EncoderType = BASSChannelType.BASS_CTYPE_STREAM_WAV;

	public string ACM_DefaultOutputExtension = ".wav";

	public override bool EncoderExists => true;

	public override BASSChannelType EncoderType => ACM_EncoderType;

	public override string DefaultOutputExtension => ACM_DefaultOutputExtension;

	public override bool SupportsSTDOUT => true;

	public override string EncoderCommandLine => string.Empty;

	public override int EffectiveBitrate
	{
		get
		{
			if (ACM_Codec != null && ACM_Codec.waveformatex != null)
			{
				return ACM_Codec.waveformatex.nAvgBytesPerSec * 8 / 1000;
			}
			return 1411;
		}
	}

	public EncoderACM(int channel)
		: base(channel)
	{
	}

	public override string ToString()
	{
		return "Audio Compression Manager (" + DefaultOutputExtension + ")";
	}

	public unsafe override bool Start(ENCODEPROC proc, IntPtr user, bool paused)
	{
		if (base.EncoderHandle != 0 || (proc != null && !SupportsSTDOUT))
		{
			return false;
		}
		if (ACM_Codec == null)
		{
			ACM_Codec = new ACMFORMAT();
		}
		int num = base.ChannelHandle;
		if (base.InputFile != null)
		{
			num = Bass.BASS_StreamCreateFile(base.InputFile, 0L, 0L, BASSFlag.BASS_STREAM_DECODE);
			if (num == 0)
			{
				return false;
			}
		}
		BASSEncode bASSEncode = BASSEncode.BASS_ENCODE_DEFAULT;
		if (paused && base.InputFile == null)
		{
			bASSEncode |= BASSEncode.BASS_ENCODE_PAUSE;
		}
		if (base.NoLimit)
		{
			bASSEncode |= BASSEncode.BASS_ENCODE_CAST_NOLIMIT;
		}
		if (base.UseAsyncQueue)
		{
			bASSEncode |= BASSEncode.BASS_ENCODE_QUEUE;
		}
		fixed (byte* ptr = acmCodecToByteArray())
		{
			if (base.OutputFile == null)
			{
				base.EncoderHandle = BassEnc.BASS_Encode_StartACM(num, (IntPtr)ptr, bASSEncode, proc, user);
			}
			else
			{
				if (base.OutputFile == null || ACM_EncoderType != BASSChannelType.BASS_CTYPE_STREAM_WAV || !ACM_WriteWaveHeader)
				{
					bASSEncode |= BASSEncode.BASS_ENCODE_NOHEAD;
				}
				base.EncoderHandle = BassEnc.BASS_Encode_StartACMFile(num, (IntPtr)ptr, bASSEncode, base.OutputFile);
			}
		}
		if (base.InputFile != null)
		{
			Utils.DecodeAllData(num, autoFree: true);
		}
		if (base.EncoderHandle == 0)
		{
			return false;
		}
		return true;
	}

	public override string SettingsString()
	{
		if (ACM_Codec != null)
		{
			return ACM_Codec.ToString();
		}
		return $"{EffectiveBitrate} kbps";
	}

	private byte[] acmCodecToByteArray()
	{
		byte[] array = new byte[Marshal.SizeOf(ACM_Codec) + ACM_Codec.waveformatex.cbSize];
		int num = Marshal.SizeOf(ACM_Codec);
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		Marshal.StructureToPtr(ACM_Codec, intPtr, fDeleteOld: false);
		Marshal.Copy(intPtr, array, 0, num);
		Marshal.FreeHGlobal(intPtr);
		for (int i = 0; i < ACM_Codec.extension.Length; i++)
		{
			array[18 + i] = ACM_Codec.extension[i];
		}
		return array;
	}

	public bool SaveCodec(string acmFile)
	{
		if (!string.IsNullOrEmpty(acmFile))
		{
			return ACMFORMAT.SaveToFile(ACM_Codec, acmFile);
		}
		return false;
	}

	public bool LoadCodec(string acmFile)
	{
		if (!string.IsNullOrEmpty(acmFile))
		{
			ACMFORMAT aCMFORMAT = ACMFORMAT.LoadFromFile(acmFile);
			if (aCMFORMAT != null)
			{
				ACM_Codec = aCMFORMAT;
				return true;
			}
		}
		return false;
	}
}

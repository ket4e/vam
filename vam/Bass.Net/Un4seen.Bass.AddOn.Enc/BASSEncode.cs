using System;

namespace Un4seen.Bass.AddOn.Enc;

[Flags]
public enum BASSEncode
{
	BASS_ENCODE_DEFAULT = 0,
	BASS_ENCODE_NOHEAD = 1,
	BASS_ENCODE_FP_8BIT = 2,
	BASS_ENCODE_FP_16BIT = 4,
	BASS_ENCODE_FP_24BIT = 6,
	BASS_ENCODE_FP_32BIT = 8,
	BASS_ENCODE_FP_AUTO = 0xE,
	BASS_ENCODE_BIGEND = 0x10,
	BASS_ENCODE_PAUSE = 0x20,
	BASS_ENCODE_PCM = 0x40,
	BASS_ENCODE_RF64 = 0x80,
	BASS_ENCODE_MONO = 0x100,
	BASS_ENCODE_QUEUE = 0x200,
	BASS_ENCODE_WFEXT = 0x400,
	BASS_ENCODE_CAST_NOLIMIT = 0x1000,
	BASS_ENCODE_LIMIT = 0x2000,
	BASS_ENCODE_AIFF = 0x4000,
	BASS_ENCODE_DITHER = 0x8000,
	BASS_ENCODE_AUTOFREE = 0x40000,
	BASS_ENCODE_FLAC_NOCOUNT = 0x1000000,
	BASS_UNICODE = int.MinValue
}

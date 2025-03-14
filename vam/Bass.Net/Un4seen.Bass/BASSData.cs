using System;

namespace Un4seen.Bass;

[Flags]
public enum BASSData
{
	BASS_DATA_AVAILABLE = 0,
	BASS_DATA_FFT_INDIVIDUAL = 0x10,
	BASS_DATA_FFT_NOWINDOW = 0x20,
	BASS_DATA_FFT_REMOVEDC = 0x40,
	BASS_DATA_FFT_COMPLEX = 0x80,
	BASS_DATA_FIXED = 0x20000000,
	BASS_DATA_FLOAT = 0x40000000,
	BASS_DATA_FFT256 = int.MinValue,
	BASS_DATA_FFT512 = -2147483647,
	BASS_DATA_FFT1024 = -2147483646,
	BASS_DATA_FFT2048 = -2147483645,
	BASS_DATA_FFT4096 = -2147483644,
	BASS_DATA_FFT8192 = -2147483643,
	BASS_DATA_FFT16384 = -2147483642,
	BASS_DATA_FFT32768 = -2147483641
}

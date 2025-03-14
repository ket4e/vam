namespace Un4seen.Bass;

public enum BASSError
{
	BASS_OK = 0,
	BASS_ERROR_MEM = 1,
	BASS_ERROR_FILEOPEN = 2,
	BASS_ERROR_DRIVER = 3,
	BASS_ERROR_BUFLOST = 4,
	BASS_ERROR_HANDLE = 5,
	BASS_ERROR_FORMAT = 6,
	BASS_ERROR_POSITION = 7,
	BASS_ERROR_INIT = 8,
	BASS_ERROR_START = 9,
	BASS_ERROR_NOCD = 12,
	BASS_ERROR_CDTRACK = 13,
	BASS_ERROR_ALREADY = 14,
	BASS_ERROR_NOPAUSE = 16,
	BASS_ERROR_NOTAUDIO = 17,
	BASS_ERROR_NOCHAN = 18,
	BASS_ERROR_ILLTYPE = 19,
	BASS_ERROR_ILLPARAM = 20,
	BASS_ERROR_NO3D = 21,
	BASS_ERROR_NOEAX = 22,
	BASS_ERROR_DEVICE = 23,
	BASS_ERROR_NOPLAY = 24,
	BASS_ERROR_FREQ = 25,
	BASS_ERROR_NOTFILE = 27,
	BASS_ERROR_NOHW = 29,
	BASS_ERROR_EMPTY = 31,
	BASS_ERROR_NONET = 32,
	BASS_ERROR_CREATE = 33,
	BASS_ERROR_NOFX = 34,
	BASS_ERROR_PLAYING = 35,
	BASS_ERROR_NOTAVAIL = 37,
	BASS_ERROR_DECODE = 38,
	BASS_ERROR_DX = 39,
	BASS_ERROR_TIMEOUT = 40,
	BASS_ERROR_FILEFORM = 41,
	BASS_ERROR_SPEAKER = 42,
	BASS_ERROR_VERSION = 43,
	BASS_ERROR_CODEC = 44,
	BASS_ERROR_ENDED = 45,
	BASS_ERROR_BUSY = 46,
	BASS_ERROR_UNKNOWN = -1,
	BASS_ERROR_WMA_LICENSE = 1000,
	BASS_ERROR_WMA_WM9 = 1001,
	BASS_ERROR_WMA_DENIED = 1002,
	BASS_ERROR_WMA_CODEC = 1003,
	BASS_ERROR_WMA_INDIVIDUAL = 1004,
	BASS_ERROR_ACM_CANCEL = 2000,
	BASS_ERROR_CAST_DENIED = 2100,
	BASS_VST_ERROR_NOINPUTS = 3000,
	BASS_VST_ERROR_NOOUTPUTS = 3001,
	BASS_VST_ERROR_NOREALTIME = 3002,
	BASS_ERROR_WASAPI = 5000,
	BASS_ERROR_MP4_NOSTREAM = 6000
}

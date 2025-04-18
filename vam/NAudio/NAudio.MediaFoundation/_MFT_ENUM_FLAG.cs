using System;

namespace NAudio.MediaFoundation;

[Flags]
public enum _MFT_ENUM_FLAG
{
	None = 0,
	MFT_ENUM_FLAG_SYNCMFT = 1,
	MFT_ENUM_FLAG_ASYNCMFT = 2,
	MFT_ENUM_FLAG_HARDWARE = 4,
	MFT_ENUM_FLAG_FIELDOFUSE = 8,
	MFT_ENUM_FLAG_LOCALMFT = 0x10,
	MFT_ENUM_FLAG_TRANSCODE_ONLY = 0x20,
	MFT_ENUM_FLAG_SORTANDFILTER = 0x40,
	MFT_ENUM_FLAG_ALL = 0x3F
}

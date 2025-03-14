using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Un4seen.Bass.AddOn.Wma;

namespace Un4seen.Bass.AddOn.Tags;

[Serializable]
[SuppressUnmanagedCodeSecurity]
public sealed class BassTags
{
	public static bool ReadPictureTAGs = true;

	public static bool EvalNativeTAGs = true;

	public static bool EvalNativeTAGsBEXT = true;

	public static bool EvalNativeTAGsCART = true;

	public static readonly string[] ID3v1Genre = new string[148]
	{
		"Blues", "Classic Rock", "Country", "Dance", "Disco", "Funk", "Grunge", "Hip-Hop", "Jazz", "Metal",
		"New Age", "Oldies", "Other", "Pop", "R&B", "Rap", "Reggae", "Rock", "Techno", "Industrial",
		"Alternative", "Ska", "Death Metal", "Pranks", "Soundtrack", "Euro-Techno", "Ambient", "Trip-Hop", "Vocal", "Jazz+Funk",
		"Fusion", "Trance", "Classical", "Instrumental", "Acid", "House", "Game", "Sound Clip", "Gospel", "Noise",
		"Alternative Rock", "Bass", "Soul", "Punk", "Space", "Meditative", "Instrumental Pop", "Instrumental Rock", "Ethnic", "Gothic",
		"Darkwave", "Techno-Industrial", "Electronic", "Pop-Folk", "Eurodance", "Dream", "Southern Rock", "Comedy", "Cult", "Gangsta",
		"Top 40", "Christian Rap", "Pop/Funk", "Jungle", "Native American", "Cabaret", "New Wave", "Psychedelic", "Rave", "Showtunes",
		"Trailer", "Lo-Fi", "Tribal", "Acid Punk", "Acid Jazz", "Polka", "Retro", "Musical", "Rock & Roll", "Hard Rock",
		"Folk", "Folk/Rock", "National Folk", "Swing", "Fusion", "Bebob", "Latin", "Revival", "Celtic", "Bluegrass",
		"Avantgarde", "Gothic Rock", "Progressive Rock", "Psychedelic Rock", "Symphonic Rock", "Slow Rock", "Big Band", "Chorus", "Easy Listening", "Acoustic",
		"Humour", "Speech", "Chanson", "Opera", "Chamber Music", "Sonata", "Symphony", "Booty Bass", "Primus", "Porn Groove",
		"Satire", "Slow Jam", "Club", "Tango", "Samba", "Folklore", "Ballad", "Power Ballad", "Rhythmic Soul", "Freestyle",
		"Duet", "Punk Rock", "Drum Solo", "A Cappella", "Euro-House", "Dance Hall", "Goa", "Drum & Bass", "Club-House", "Hardcore",
		"Terror", "Indie", "BritPop", "Negerpunk", "Polsk Punk", "Beat", "Christian Gangsta Rap", "Heavy Metal", "Black Metal", "Crossover",
		"Contemporary Christian", "Christian Rock", "Merengue", "Salsa", "Thrash Metal", "Anime", "Jpop", "Synthpop"
	};

	private BassTags()
	{
	}

	public static TAG_INFO BASS_TAG_GetFromFile(string file)
	{
		return BASS_TAG_GetFromFile(file, setDefaultTitle: true, prescan: true);
	}

	public static TAG_INFO BASS_TAG_GetFromFile(string file, bool setDefaultTitle, bool prescan)
	{
		if (string.IsNullOrEmpty(file))
		{
			return null;
		}
		TAG_INFO tAG_INFO = new TAG_INFO(file, setDefaultTitle);
		if (BASS_TAG_GetFromFile(file, prescan, tAG_INFO))
		{
			return tAG_INFO;
		}
		return null;
	}

	public static bool BASS_TAG_GetFromFile(string file, bool prescan, TAG_INFO tags)
	{
		if (tags == null)
		{
			return false;
		}
		int num = Bass.BASS_StreamCreateFile(file, 0L, 0L, BASSFlag.BASS_STREAM_DECODE | (prescan ? BASSFlag.BASS_STREAM_PRESCAN : BASSFlag.BASS_DEFAULT));
		if (num != 0)
		{
			BASS_TAG_GetFromFile(num, tags);
			Bass.BASS_StreamFree(num);
			return true;
		}
		string text = Path.GetExtension(file).ToLower();
		if (text == ".wma" || text == ".wmv")
		{
			IntPtr intPtr = BassWma.BASS_WMA_GetTags(file);
			if (intPtr != IntPtr.Zero)
			{
				tags.tagType = BASSTag.BASS_TAG_WMA;
				tags.UpdateFromMETA(intPtr, utf8: true, multiple: false);
				if (ReadPictureTAGs)
				{
					IWMMetadataEditor ppMetadataEditor = null;
					try
					{
						WMFMetadataEditor.WMCreateEditor(out ppMetadataEditor);
						ppMetadataEditor.Open(file);
						List<TagPicture> allPictures = new WMFMetadataEditor((IWMHeaderInfo3)ppMetadataEditor).GetAllPictures();
						if (allPictures != null)
						{
							foreach (TagPicture item in allPictures)
							{
								tags.AddPicture(item);
							}
						}
					}
					catch
					{
					}
					finally
					{
						ppMetadataEditor?.Close();
						ppMetadataEditor = null;
					}
				}
				return true;
			}
			return false;
		}
		return false;
	}

	public static bool BASS_TAG_GetFromFile(int stream, TAG_INFO tags)
	{
		if (stream == 0 || tags == null)
		{
			return false;
		}
		bool flag = false;
		BASS_CHANNELINFO bASS_CHANNELINFO = new BASS_CHANNELINFO();
		if (Bass.BASS_ChannelGetInfo(stream, bASS_CHANNELINFO))
		{
			tags.channelinfo = bASS_CHANNELINFO;
			BASSTag tagType = BASSTag.BASS_TAG_UNKNOWN;
			IntPtr intPtr = BASS_TAG_GetIntPtr(stream, bASS_CHANNELINFO, out tagType);
			tags.tagType = tagType;
			if (intPtr != IntPtr.Zero)
			{
				switch (tagType)
				{
				case BASSTag.BASS_TAG_ID3V2:
				{
					IntPtr intPtr17 = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_ID3);
					if (intPtr17 != IntPtr.Zero)
					{
						ReadID3v1(intPtr17, tags);
						tags.ResetTags2();
					}
					IntPtr intPtr18 = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_APE);
					if (intPtr18 != IntPtr.Zero)
					{
						tags.UpdateFromMETA(intPtr18, utf8: true, multiple: false);
						if (ReadPictureTAGs)
						{
							ReadAPEPictures(stream, tags);
						}
						tags.ResetTags2();
					}
					flag = ReadID3v2(intPtr, tags);
					break;
				}
				case BASSTag.BASS_TAG_ID3:
				{
					IntPtr intPtr6 = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_APE);
					if (intPtr6 != IntPtr.Zero)
					{
						tags.UpdateFromMETA(intPtr6, utf8: true, multiple: false);
						if (ReadPictureTAGs)
						{
							ReadAPEPictures(stream, tags);
						}
						tags.ResetTags2();
					}
					flag = ReadID3v1(intPtr, tags);
					break;
				}
				case BASSTag.BASS_TAG_WMA:
					flag = tags.UpdateFromMETA(intPtr, utf8: true, multiple: false);
					if (!ReadPictureTAGs)
					{
						break;
					}
					try
					{
						IntPtr intPtr7 = BassWma.BASS_WMA_GetWMObject(stream);
						if (!(intPtr7 != IntPtr.Zero))
						{
							break;
						}
						IWMHeaderInfo3 iWMHeaderInfo = (IWMHeaderInfo3)Marshal.GetObjectForIUnknown(intPtr7);
						List<TagPicture> allPictures = new WMFMetadataEditor(iWMHeaderInfo).GetAllPictures();
						if (allPictures != null)
						{
							foreach (TagPicture item in allPictures)
							{
								tags.AddPicture(item);
							}
						}
						Marshal.FinalReleaseComObject(iWMHeaderInfo);
					}
					catch
					{
					}
					break;
				case BASSTag.BASS_TAG_MF:
					flag = tags.UpdateFromMETA(intPtr, utf8: true, multiple: false);
					break;
				case BASSTag.BASS_TAG_OGG:
				{
					IntPtr intPtr14 = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_APE);
					if (intPtr14 != IntPtr.Zero)
					{
						tags.UpdateFromMETA(intPtr14, utf8: true, multiple: false);
						if (ReadPictureTAGs)
						{
							ReadAPEPictures(stream, tags);
						}
						tags.ResetTags2();
					}
					if (ReadPictureTAGs && (bASS_CHANNELINFO.ctype == BASSChannelType.BASS_CTYPE_STREAM_FLAC || bASS_CHANNELINFO.ctype == BASSChannelType.BASS_CTYPE_STREAM_FLAC_OGG))
					{
						int num3 = 0;
						BASS_TAG_FLAC_PICTURE tag2;
						while ((tag2 = BASS_TAG_FLAC_PICTURE.GetTag(stream, num3)) != null)
						{
							tags.AddPicture(new TagPicture(num3, tag2.Mime, TagPicture.PICTURE_TYPE.FrontAlbumCover, tag2.Desc, tag2.Data));
							num3++;
						}
					}
					flag = tags.UpdateFromMETA(intPtr, utf8: true, multiple: false);
					break;
				}
				case BASSTag.BASS_TAG_APE:
				{
					IntPtr intPtr8 = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_ID3);
					if (intPtr8 != IntPtr.Zero)
					{
						ReadID3v1(intPtr8, tags);
						tags.ResetTags2();
					}
					IntPtr intPtr9 = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_ID3V2);
					if (intPtr9 != IntPtr.Zero)
					{
						ReadID3v2(intPtr9, tags);
						tags.ResetTags2();
					}
					if (ReadPictureTAGs && (bASS_CHANNELINFO.ctype == BASSChannelType.BASS_CTYPE_STREAM_FLAC || bASS_CHANNELINFO.ctype == BASSChannelType.BASS_CTYPE_STREAM_FLAC_OGG))
					{
						int num2 = 0;
						BASS_TAG_FLAC_PICTURE tag;
						while ((tag = BASS_TAG_FLAC_PICTURE.GetTag(stream, num2)) != null)
						{
							tags.AddPicture(new TagPicture(num2, tag.Mime, TagPicture.PICTURE_TYPE.FrontAlbumCover, tag.Desc, tag.Data));
							num2++;
						}
					}
					flag = tags.UpdateFromMETA(intPtr, utf8: true, multiple: false);
					if (ReadPictureTAGs)
					{
						ReadAPEPictures(stream, tags);
					}
					break;
				}
				case BASSTag.BASS_TAG_MP4:
				{
					IntPtr intPtr15 = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_APE);
					if (intPtr15 != IntPtr.Zero)
					{
						tags.UpdateFromMETA(intPtr15, utf8: true, multiple: false);
						if (ReadPictureTAGs)
						{
							ReadAPEPictures(stream, tags);
						}
						tags.ResetTags2();
					}
					IntPtr intPtr16 = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_ID3V2);
					if (intPtr16 != IntPtr.Zero)
					{
						ReadID3v2(intPtr16, tags);
						tags.ResetTags2();
					}
					flag = tags.UpdateFromMETA(intPtr, utf8: true, multiple: false);
					break;
				}
				case BASSTag.BASS_TAG_RIFF_INFO:
				{
					flag = tags.UpdateFromMETA(intPtr, BassNet.UseRiffInfoUTF8, multiple: false);
					IntPtr intPtr10 = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_RIFF_BEXT);
					if (intPtr10 != IntPtr.Zero)
					{
						ReadRiffBEXT(intPtr10, tags);
					}
					IntPtr intPtr11 = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_RIFF_CART);
					if (intPtr11 != IntPtr.Zero)
					{
						ReadRiffCART(intPtr11, tags);
					}
					IntPtr intPtr12 = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_ID3V2);
					if (intPtr12 != IntPtr.Zero)
					{
						ReadID3v2(intPtr12, tags);
					}
					IntPtr intPtr13 = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_RIFF_DISP);
					if (!(intPtr13 != IntPtr.Zero))
					{
						break;
					}
					string text = null;
					text = ((!BassNet.UseRiffInfoUTF8) ? Utils.IntPtrAsStringAnsi(intPtr13) : Utils.IntPtrAsStringUtf8orLatin1(intPtr13, out var _));
					if (!string.IsNullOrEmpty(text))
					{
						if (string.IsNullOrEmpty(tags.title))
						{
							tags.title = text;
						}
						else if (string.IsNullOrEmpty(tags.artist))
						{
							tags.artist = text;
						}
						else if (string.IsNullOrEmpty(tags.album))
						{
							tags.album = text;
						}
					}
					break;
				}
				case BASSTag.BASS_TAG_RIFF_BEXT:
				case BASSTag.BASS_TAG_RIFF_CART:
				{
					IntPtr intPtr3 = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_RIFF_INFO);
					if (intPtr3 != IntPtr.Zero)
					{
						tags.UpdateFromMETA(intPtr3, BassNet.UseRiffInfoUTF8, multiple: false);
					}
					IntPtr intPtr4 = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_RIFF_CART);
					if (intPtr4 != IntPtr.Zero)
					{
						ReadRiffCART(intPtr4, tags);
					}
					IntPtr intPtr5 = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_ID3V2);
					if (intPtr5 != IntPtr.Zero)
					{
						ReadID3v2(intPtr5, tags);
						tags.ResetTags2();
					}
					flag = ReadRiffBEXT(intPtr, tags);
					break;
				}
				case BASSTag.BASS_TAG_MIDI_TRACK:
				{
					int num = 0;
					while (true)
					{
						IntPtr intPtr2 = Bass.BASS_ChannelGetTags(stream, (BASSTag)(69632 + num));
						if (!(intPtr2 != IntPtr.Zero))
						{
							break;
						}
						flag |= tags.UpdateFromMETA(intPtr2, utf8: false, multiple: false);
						num++;
					}
					if (!flag && tags.NativeTags.Length != 0)
					{
						flag = true;
						if (tags.NativeTags.Length != 0)
						{
							tags.title = tags.NativeTags[0].Trim();
						}
						if (tags.NativeTags.Length > 1)
						{
							tags.artist = tags.NativeTags[1].Trim();
						}
					}
					break;
				}
				case BASSTag.BASS_TAG_MUSIC_NAME:
					tags.title = Bass.BASS_ChannelGetMusicName(stream);
					if (tags.title == null)
					{
						tags.title = string.Empty;
					}
					tags.artist = Bass.BASS_ChannelGetMusicMessage(stream);
					if (tags.artist == null)
					{
						tags.artist = string.Empty;
					}
					flag = true;
					break;
				case BASSTag.BASS_TAG_DSD_ARTIST:
				case BASSTag.BASS_TAG_DSD_TITLE:
				case BASSTag.BASS_TAG_DSD_COMMENT:
				{
					tags.title = Bass.BASS_ChannelGetTagsDSDTitle(stream);
					if (tags.title == null)
					{
						tags.title = string.Empty;
					}
					tags.artist = Bass.BASS_ChannelGetTagsDSDArtist(stream);
					if (tags.artist == null)
					{
						tags.artist = string.Empty;
					}
					StringBuilder stringBuilder = new StringBuilder();
					BASS_TAG_DSD_COMMENT[] array = Bass.BASS_ChannelGetTagsDSDComments(stream);
					if (array != null)
					{
						BASS_TAG_DSD_COMMENT[] array2 = array;
						foreach (BASS_TAG_DSD_COMMENT bASS_TAG_DSD_COMMENT in array2)
						{
							stringBuilder.AppendFormat("{0}.{1}.{2} {3}:{4} ({5}/{6}) - {7}\n", bASS_TAG_DSD_COMMENT.TimeStampYear, bASS_TAG_DSD_COMMENT.TimeStampMonth, bASS_TAG_DSD_COMMENT.TimeStampDay, bASS_TAG_DSD_COMMENT.TimeStampHour, bASS_TAG_DSD_COMMENT.TimeStampMinutes, bASS_TAG_DSD_COMMENT.CommentType, bASS_TAG_DSD_COMMENT.CommentRef, bASS_TAG_DSD_COMMENT.CommentText);
						}
					}
					tags.comment = stringBuilder.ToString();
					break;
				}
				}
			}
			tags.duration = Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetLength(stream));
			if (tags.duration < 0.0)
			{
				tags.duration = 0.0;
			}
			if (tags.bitrate == 0)
			{
				float value = 0f;
				if (Bass.BASS_ChannelGetAttribute(stream, BASSAttribute.BASS_ATTRIB_BITRATE, ref value))
				{
					tags.bitrate = (int)value;
				}
				else
				{
					long num4 = Bass.BASS_StreamGetFilePosition(stream, BASSStreamFilePosition.BASS_FILEPOS_END);
					tags.bitrate = (int)((double)num4 / (125.0 * tags.duration) + 0.5);
				}
			}
		}
		return flag;
	}

	public static bool BASS_TAG_GetFromURL(int stream, TAG_INFO tags)
	{
		if (stream == 0 || tags == null)
		{
			return false;
		}
		bool result = false;
		BASS_CHANNELINFO bASS_CHANNELINFO = new BASS_CHANNELINFO();
		if (Bass.BASS_ChannelGetInfo(stream, bASS_CHANNELINFO))
		{
			tags.channelinfo = bASS_CHANNELINFO;
		}
		IntPtr intPtr = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_ICY);
		if (intPtr != IntPtr.Zero)
		{
			tags.tagType = BASSTag.BASS_TAG_ICY;
		}
		else
		{
			intPtr = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_HTTP);
			if (intPtr != IntPtr.Zero)
			{
				tags.tagType = BASSTag.BASS_TAG_HTTP;
			}
			else
			{
				intPtr = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_HLS_EXTINF);
				if (intPtr != IntPtr.Zero)
				{
					tags.tagType = BASSTag.BASS_TAG_HLS_EXTINF;
				}
			}
		}
		if (intPtr != IntPtr.Zero)
		{
			result = tags.UpdateFromMETA(intPtr, TAGINFOEncoding.Utf8OrLatin1, multiple: true);
		}
		intPtr = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_META);
		if (intPtr != IntPtr.Zero)
		{
			tags.tagType = BASSTag.BASS_TAG_META;
			result = tags.UpdateFromMETA(intPtr, TAGINFOEncoding.Utf8OrLatin1, multiple: true);
		}
		else
		{
			intPtr = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_OGG);
			if (intPtr == IntPtr.Zero)
			{
				intPtr = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_APE);
				if (intPtr != IntPtr.Zero)
				{
					tags.tagType = BASSTag.BASS_TAG_APE;
				}
			}
			else
			{
				tags.tagType = BASSTag.BASS_TAG_OGG;
			}
			if (intPtr == IntPtr.Zero)
			{
				intPtr = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_WMA);
				if (intPtr != IntPtr.Zero)
				{
					tags.tagType = BASSTag.BASS_TAG_WMA;
				}
			}
			if (intPtr == IntPtr.Zero)
			{
				intPtr = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_ID3V2);
			}
			if (intPtr != IntPtr.Zero)
			{
				result = tags.UpdateFromMETA(intPtr, TAGINFOEncoding.Utf8, multiple: false);
			}
		}
		tags.duration = Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetLength(stream));
		return result;
	}

	private static IntPtr BASS_TAG_GetIntPtr(int stream, BASS_CHANNELINFO info, out BASSTag tagType)
	{
		IntPtr zero = IntPtr.Zero;
		tagType = BASSTag.BASS_TAG_UNKNOWN;
		if (stream == 0 || info == null)
		{
			return zero;
		}
		BASSChannelType bASSChannelType = info.ctype;
		if ((bASSChannelType & BASSChannelType.BASS_CTYPE_STREAM_WAV) > BASSChannelType.BASS_CTYPE_UNKNOWN)
		{
			bASSChannelType = BASSChannelType.BASS_CTYPE_STREAM_WAV;
		}
		switch (bASSChannelType)
		{
		case BASSChannelType.BASS_CTYPE_STREAM_MP1:
		case BASSChannelType.BASS_CTYPE_STREAM_MP2:
		case BASSChannelType.BASS_CTYPE_STREAM_MP3:
		case BASSChannelType.BASS_CTYPE_STREAM_WMA_MP3:
			zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_ID3V2);
			if (zero == IntPtr.Zero)
			{
				zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_ID3);
				if (zero == IntPtr.Zero)
				{
					zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_APE);
					if (zero == IntPtr.Zero)
					{
						zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_RIFF_BEXT);
						if (zero != IntPtr.Zero)
						{
							tagType = BASSTag.BASS_TAG_RIFF_BEXT;
						}
						else
						{
							tagType = BASSTag.BASS_TAG_ID3V2;
						}
					}
					else
					{
						tagType = BASSTag.BASS_TAG_APE;
					}
				}
				else
				{
					tagType = BASSTag.BASS_TAG_ID3;
				}
			}
			else
			{
				tagType = BASSTag.BASS_TAG_ID3V2;
			}
			break;
		case BASSChannelType.BASS_CTYPE_STREAM_OGG:
		case BASSChannelType.BASS_CTYPE_STREAM_FLAC:
		case BASSChannelType.BASS_CTYPE_STREAM_FLAC_OGG:
		case BASSChannelType.BASS_CTYPE_STREAM_OPUS:
			zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_OGG);
			if (zero == IntPtr.Zero)
			{
				zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_APE);
				if (zero != IntPtr.Zero)
				{
					tagType = BASSTag.BASS_TAG_APE;
				}
				else
				{
					tagType = BASSTag.BASS_TAG_OGG;
				}
			}
			else
			{
				tagType = BASSTag.BASS_TAG_OGG;
			}
			break;
		case BASSChannelType.BASS_CTYPE_STREAM_WMA:
			zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_WMA);
			tagType = BASSTag.BASS_TAG_WMA;
			break;
		case BASSChannelType.BASS_CTYPE_STREAM_MF:
			zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_MF);
			if (zero == IntPtr.Zero)
			{
				zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_MP4);
				if (zero == IntPtr.Zero)
				{
					zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_ID3V2);
					if (zero == IntPtr.Zero)
					{
						zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_APE);
						if (zero != IntPtr.Zero)
						{
							tagType = BASSTag.BASS_TAG_APE;
						}
						else
						{
							tagType = BASSTag.BASS_TAG_MF;
						}
					}
					else
					{
						tagType = BASSTag.BASS_TAG_ID3V2;
					}
				}
				else
				{
					tagType = BASSTag.BASS_TAG_MP4;
				}
			}
			else
			{
				tagType = BASSTag.BASS_TAG_MF;
			}
			break;
		case BASSChannelType.BASS_CTYPE_STREAM_AM:
			zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_MP4);
			if (zero == IntPtr.Zero)
			{
				zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_ID3V2);
				if (zero == IntPtr.Zero)
				{
					zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_APE);
					if (zero == IntPtr.Zero)
					{
						zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_OGG);
						if (zero != IntPtr.Zero)
						{
							tagType = BASSTag.BASS_TAG_APE;
						}
						else
						{
							tagType = BASSTag.BASS_TAG_MF;
						}
					}
					else
					{
						tagType = BASSTag.BASS_TAG_ID3V2;
					}
				}
				else
				{
					tagType = BASSTag.BASS_TAG_MP4;
				}
			}
			else
			{
				tagType = BASSTag.BASS_TAG_MF;
			}
			break;
		case BASSChannelType.BASS_CTYPE_STREAM_CA:
		case BASSChannelType.BASS_CTYPE_STREAM_AAC:
		case BASSChannelType.BASS_CTYPE_STREAM_MP4:
		case BASSChannelType.BASS_CTYPE_STREAM_ALAC:
			zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_MP4);
			if (zero == IntPtr.Zero)
			{
				zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_ID3V2);
				if (zero == IntPtr.Zero)
				{
					zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_APE);
					if (zero == IntPtr.Zero)
					{
						zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_OGG);
						if (zero != IntPtr.Zero)
						{
							tagType = BASSTag.BASS_TAG_OGG;
						}
						else
						{
							tagType = BASSTag.BASS_TAG_MP4;
						}
					}
					else
					{
						tagType = BASSTag.BASS_TAG_APE;
					}
				}
				else
				{
					tagType = BASSTag.BASS_TAG_ID3V2;
				}
			}
			else
			{
				tagType = BASSTag.BASS_TAG_MP4;
			}
			break;
		case BASSChannelType.BASS_CTYPE_STREAM_WV:
		case BASSChannelType.BASS_CTYPE_STREAM_WV_H:
		case BASSChannelType.BASS_CTYPE_STREAM_WV_L:
		case BASSChannelType.BASS_CTYPE_STREAM_WV_LH:
		case BASSChannelType.BASS_CTYPE_STREAM_OFR:
		case BASSChannelType.BASS_CTYPE_STREAM_APE:
		case BASSChannelType.BASS_CTYPE_STREAM_MPC:
		case BASSChannelType.BASS_CTYPE_STREAM_SPX:
		case BASSChannelType.BASS_CTYPE_STREAM_TTA:
			zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_APE);
			if (zero == IntPtr.Zero)
			{
				zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_OGG);
				if (zero == IntPtr.Zero)
				{
					zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_ID3V2);
					if (zero == IntPtr.Zero)
					{
						zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_ID3);
						if (zero != IntPtr.Zero)
						{
							tagType = BASSTag.BASS_TAG_ID3;
						}
						else
						{
							tagType = BASSTag.BASS_TAG_APE;
						}
					}
					else
					{
						tagType = BASSTag.BASS_TAG_ID3V2;
					}
				}
				else
				{
					tagType = BASSTag.BASS_TAG_OGG;
				}
			}
			else
			{
				tagType = BASSTag.BASS_TAG_APE;
			}
			break;
		case BASSChannelType.BASS_CTYPE_STREAM_WINAMP:
			zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_ID3V2);
			if (zero == IntPtr.Zero)
			{
				zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_APE);
				if (zero == IntPtr.Zero)
				{
					zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_OGG);
					if (zero == IntPtr.Zero)
					{
						zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_ID3);
						if (zero != IntPtr.Zero)
						{
							tagType = BASSTag.BASS_TAG_ID3;
						}
						else
						{
							tagType = BASSTag.BASS_TAG_ID3V2;
						}
					}
					else
					{
						tagType = BASSTag.BASS_TAG_OGG;
					}
				}
				else
				{
					tagType = BASSTag.BASS_TAG_APE;
				}
			}
			else
			{
				tagType = BASSTag.BASS_TAG_ID3V2;
			}
			break;
		case BASSChannelType.BASS_CTYPE_STREAM_AIFF:
		case BASSChannelType.BASS_CTYPE_STREAM_WAV:
			zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_RIFF_INFO);
			if (zero == IntPtr.Zero)
			{
				zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_RIFF_BEXT);
				if (zero == IntPtr.Zero)
				{
					zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_ID3V2);
					if (zero != IntPtr.Zero)
					{
						tagType = BASSTag.BASS_TAG_ID3V2;
					}
					else
					{
						tagType = BASSTag.BASS_TAG_RIFF_INFO;
					}
				}
				else
				{
					tagType = BASSTag.BASS_TAG_RIFF_BEXT;
				}
			}
			else
			{
				tagType = BASSTag.BASS_TAG_RIFF_INFO;
			}
			break;
		case BASSChannelType.BASS_CTYPE_MUSIC_MO3:
		case BASSChannelType.BASS_CTYPE_MUSIC_MOD:
		case BASSChannelType.BASS_CTYPE_MUSIC_MTM:
		case BASSChannelType.BASS_CTYPE_MUSIC_S3M:
		case BASSChannelType.BASS_CTYPE_MUSIC_XM:
		case BASSChannelType.BASS_CTYPE_MUSIC_IT:
			zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_MUSIC_NAME);
			tagType = BASSTag.BASS_TAG_MUSIC_NAME;
			break;
		case BASSChannelType.BASS_CTYPE_STREAM_MIDI:
			zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_MIDI_TRACK);
			if (zero == IntPtr.Zero)
			{
				zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_RIFF_INFO);
				if (zero != IntPtr.Zero)
				{
					tagType = BASSTag.BASS_TAG_RIFF_INFO;
				}
				else
				{
					tagType = BASSTag.BASS_TAG_MIDI_TRACK;
				}
			}
			else
			{
				tagType = BASSTag.BASS_TAG_MIDI_TRACK;
			}
			break;
		case BASSChannelType.BASS_CTYPE_STREAM_DSD:
			zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_ID3V2);
			if (zero == IntPtr.Zero)
			{
				zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_DSD_COMMENT);
				if (zero == IntPtr.Zero)
				{
					zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_DSD_ARTIST);
					if (zero == IntPtr.Zero)
					{
						zero = Bass.BASS_ChannelGetTags(stream, BASSTag.BASS_TAG_DSD_TITLE);
						if (zero != IntPtr.Zero)
						{
							tagType = BASSTag.BASS_TAG_DSD_TITLE;
						}
					}
					else
					{
						tagType = BASSTag.BASS_TAG_DSD_ARTIST;
					}
				}
				else
				{
					tagType = BASSTag.BASS_TAG_DSD_COMMENT;
				}
			}
			else
			{
				tagType = BASSTag.BASS_TAG_ID3V2;
			}
			break;
		default:
			zero = IntPtr.Zero;
			break;
		}
		return zero;
	}

	private static bool ReadRiffBEXT(IntPtr p, TAG_INFO tags)
	{
		if (p == IntPtr.Zero || tags == null)
		{
			return false;
		}
		bool result = true;
		try
		{
			BASS_TAG_BEXT bASS_TAG_BEXT = (BASS_TAG_BEXT)Marshal.PtrToStructure(p, typeof(BASS_TAG_BEXT));
			if (!string.IsNullOrEmpty(bASS_TAG_BEXT.Description) && string.IsNullOrEmpty(tags.title))
			{
				tags.title = bASS_TAG_BEXT.Description;
			}
			if (EvalNativeTAGsBEXT)
			{
				tags.AddNativeTag("BWFDescription", bASS_TAG_BEXT.Description);
			}
			if (!string.IsNullOrEmpty(bASS_TAG_BEXT.Originator) && string.IsNullOrEmpty(tags.artist))
			{
				tags.artist = bASS_TAG_BEXT.Originator;
			}
			if (EvalNativeTAGsBEXT)
			{
				tags.AddNativeTag("BWFOriginator", bASS_TAG_BEXT.Originator);
			}
			if (!string.IsNullOrEmpty(bASS_TAG_BEXT.OriginatorReference) && string.IsNullOrEmpty(tags.encodedby))
			{
				tags.encodedby = bASS_TAG_BEXT.OriginatorReference;
			}
			if (EvalNativeTAGsBEXT)
			{
				tags.AddNativeTag("BWFOriginatorReference", bASS_TAG_BEXT.OriginatorReference);
			}
			string originationDate = bASS_TAG_BEXT.OriginationDate;
			if (!string.IsNullOrEmpty(originationDate) && string.IsNullOrEmpty(tags.year) && originationDate != "0000-01-01" && originationDate != "0001-01-01")
			{
				tags.year = bASS_TAG_BEXT.OriginationDate;
			}
			if (EvalNativeTAGsBEXT)
			{
				tags.AddNativeTag("BWFOriginationDate", bASS_TAG_BEXT.OriginationDate);
				tags.AddNativeTag("BWFOriginationTime", bASS_TAG_BEXT.OriginationTime);
				tags.AddNativeTag("BWFTimeReference", bASS_TAG_BEXT.TimeReference.ToString());
				tags.AddNativeTag("BWFVersion", bASS_TAG_BEXT.Version.ToString());
				tags.AddNativeTag("BWFUMID", bASS_TAG_BEXT.UMID);
			}
			string codingHistory = bASS_TAG_BEXT.GetCodingHistory(p);
			if (!string.IsNullOrEmpty(codingHistory) && string.IsNullOrEmpty(tags.comment))
			{
				tags.comment = codingHistory;
			}
			if (EvalNativeTAGsBEXT)
			{
				tags.AddNativeTag("BWFCodingHistory", codingHistory);
			}
		}
		catch
		{
			result = false;
		}
		return result;
	}

	private static bool ReadRiffCART(IntPtr p, TAG_INFO tags)
	{
		if (p == IntPtr.Zero || tags == null)
		{
			return false;
		}
		bool result = true;
		try
		{
			BASS_TAG_CART bASS_TAG_CART = (BASS_TAG_CART)Marshal.PtrToStructure(p, typeof(BASS_TAG_CART));
			if (EvalNativeTAGsCART)
			{
				tags.AddNativeTag("CARTVersion", bASS_TAG_CART.Version);
			}
			if (!string.IsNullOrEmpty(bASS_TAG_CART.Title) && string.IsNullOrEmpty(tags.title))
			{
				tags.title = bASS_TAG_CART.Title;
			}
			if (EvalNativeTAGsCART)
			{
				tags.AddNativeTag("CARTTitle", bASS_TAG_CART.Title);
			}
			if (!string.IsNullOrEmpty(bASS_TAG_CART.Artist) && string.IsNullOrEmpty(tags.artist))
			{
				tags.artist = bASS_TAG_CART.Artist;
			}
			if (EvalNativeTAGsCART)
			{
				tags.AddNativeTag("CARTArtist", bASS_TAG_CART.Artist);
			}
			if (!string.IsNullOrEmpty(bASS_TAG_CART.CutID) && string.IsNullOrEmpty(tags.album))
			{
				tags.album = bASS_TAG_CART.CutID;
			}
			if (EvalNativeTAGsCART)
			{
				tags.AddNativeTag("CARTCutID", bASS_TAG_CART.CutID);
			}
			if (!string.IsNullOrEmpty(bASS_TAG_CART.ClientID) && string.IsNullOrEmpty(tags.copyright))
			{
				tags.copyright = bASS_TAG_CART.ClientID;
			}
			if (EvalNativeTAGsCART)
			{
				tags.AddNativeTag("CARTClientID", bASS_TAG_CART.ClientID);
			}
			if (!string.IsNullOrEmpty(bASS_TAG_CART.Category) && string.IsNullOrEmpty(tags.genre))
			{
				tags.genre = bASS_TAG_CART.Category;
			}
			if (EvalNativeTAGsCART)
			{
				tags.AddNativeTag("CARTCategory", bASS_TAG_CART.Category);
			}
			if (!string.IsNullOrEmpty(bASS_TAG_CART.Classification) && string.IsNullOrEmpty(tags.grouping))
			{
				tags.grouping = bASS_TAG_CART.Classification;
			}
			if (EvalNativeTAGsCART)
			{
				tags.AddNativeTag("CARTClassification", bASS_TAG_CART.Classification);
			}
			if (!string.IsNullOrEmpty(bASS_TAG_CART.ProducerAppID) && string.IsNullOrEmpty(tags.encodedby))
			{
				tags.encodedby = bASS_TAG_CART.ProducerAppID;
			}
			if (EvalNativeTAGsCART)
			{
				tags.AddNativeTag("CARTProducerAppID", bASS_TAG_CART.ProducerAppID);
			}
			string tagText = bASS_TAG_CART.GetTagText(p);
			if (!string.IsNullOrEmpty(tagText) && string.IsNullOrEmpty(tags.comment))
			{
				tags.comment = tagText;
			}
			if (EvalNativeTAGsCART)
			{
				tags.AddNativeTag("CARTTagText", tagText);
			}
			if (EvalNativeTAGsCART)
			{
				tags.AddNativeTag("CARTOutCue", bASS_TAG_CART.OutCue);
				tags.AddNativeTag("CARTStartDate", bASS_TAG_CART.StartDate);
				tags.AddNativeTag("CARTStartTime", bASS_TAG_CART.StartTime);
				tags.AddNativeTag("CARTEndDate", bASS_TAG_CART.EndDate);
				tags.AddNativeTag("CARTEndTime", bASS_TAG_CART.EndTime);
				tags.AddNativeTag("CARTProducerAppVersion", bASS_TAG_CART.ProducerAppVersion);
				tags.AddNativeTag("CARTUserDef", bASS_TAG_CART.UserDef);
				tags.AddNativeTag("CARTLevelReference", bASS_TAG_CART.LevelReference.ToString(CultureInfo.InvariantCulture));
				tags.AddNativeTag("CARTTimer1Usage", bASS_TAG_CART.Timer1Usage);
				tags.AddNativeTag("CARTTimer1Value", ((uint)bASS_TAG_CART.Timer1Value).ToString(CultureInfo.InvariantCulture));
				tags.AddNativeTag("CARTTimer2Usage", bASS_TAG_CART.Timer2Usage);
				tags.AddNativeTag("CARTTimer2Value", ((uint)bASS_TAG_CART.Timer2Value).ToString(CultureInfo.InvariantCulture));
				tags.AddNativeTag("CARTTimer3Usage", bASS_TAG_CART.Timer3Usage);
				tags.AddNativeTag("CARTTimer3Value", ((uint)bASS_TAG_CART.Timer3Value).ToString(CultureInfo.InvariantCulture));
				tags.AddNativeTag("CARTTimer4Usage", bASS_TAG_CART.Timer4Usage);
				tags.AddNativeTag("CARTTimer4Value", ((uint)bASS_TAG_CART.Timer4Value).ToString(CultureInfo.InvariantCulture));
				tags.AddNativeTag("CARTTimer5Usage", bASS_TAG_CART.Timer5Usage);
				tags.AddNativeTag("CARTTimer5Value", ((uint)bASS_TAG_CART.Timer5Value).ToString(CultureInfo.InvariantCulture));
				tags.AddNativeTag("CARTTimer6Usage", bASS_TAG_CART.Timer6Usage);
				tags.AddNativeTag("CARTTimer6Value", ((uint)bASS_TAG_CART.Timer6Value).ToString(CultureInfo.InvariantCulture));
				tags.AddNativeTag("CARTTimer7Usage", bASS_TAG_CART.Timer7Usage);
				tags.AddNativeTag("CARTTimer7Value", ((uint)bASS_TAG_CART.Timer7Value).ToString(CultureInfo.InvariantCulture));
				tags.AddNativeTag("CARTTimer8Usage", bASS_TAG_CART.Timer8Usage);
				tags.AddNativeTag("CARTTimer8Value", ((uint)bASS_TAG_CART.Timer8Value).ToString(CultureInfo.InvariantCulture));
				tags.AddNativeTag("CARTURL", bASS_TAG_CART.URL);
			}
		}
		catch
		{
			result = false;
		}
		return result;
	}

	public static bool ReadID3v1(IntPtr p, TAG_INFO tags)
	{
		if (p == IntPtr.Zero || tags == null)
		{
			return false;
		}
		bool result = true;
		try
		{
			BASS_TAG_ID3 bASS_TAG_ID = (BASS_TAG_ID3)Marshal.PtrToStructure(p, typeof(BASS_TAG_ID3));
			if (bASS_TAG_ID.ID.Equals("TAG"))
			{
				tags.title = bASS_TAG_ID.Title;
				if (EvalNativeTAGs)
				{
					tags.AddNativeTag("Title", bASS_TAG_ID.Title);
				}
				tags.artist = bASS_TAG_ID.Artist;
				if (EvalNativeTAGs)
				{
					tags.AddNativeTag("Artist", bASS_TAG_ID.Artist);
				}
				tags.album = bASS_TAG_ID.Album;
				if (EvalNativeTAGs)
				{
					tags.AddNativeTag("Album", bASS_TAG_ID.Album);
				}
				tags.year = bASS_TAG_ID.Year;
				if (EvalNativeTAGs)
				{
					tags.AddNativeTag("Year", bASS_TAG_ID.Year);
				}
				tags.comment = bASS_TAG_ID.Comment;
				if (EvalNativeTAGs)
				{
					tags.AddNativeTag("Comment", bASS_TAG_ID.Comment);
				}
				if (bASS_TAG_ID.Dummy == 0)
				{
					tags.track = bASS_TAG_ID.Track.ToString();
					if (EvalNativeTAGs)
					{
						tags.AddNativeTag("Track", bASS_TAG_ID.Track.ToString());
					}
				}
				if (EvalNativeTAGs)
				{
					tags.AddNativeTag("Genre", bASS_TAG_ID.Genre);
				}
				try
				{
					tags.genre = ID3v1Genre[bASS_TAG_ID.Genre];
				}
				catch
				{
					tags.genre = "Unknown";
				}
			}
		}
		catch
		{
			result = false;
		}
		return result;
	}

	public static bool ReadID3v2(IntPtr p, TAG_INFO tags)
	{
		if (p == IntPtr.Zero || tags == null)
		{
			return false;
		}
		try
		{
			tags.ResetTags();
			int num = 0;
			int num2 = 0;
			ID3v2Reader iD3v2Reader = new ID3v2Reader(p);
			while (iD3v2Reader.Read())
			{
				string key = iD3v2Reader.GetKey();
				short flags = iD3v2Reader.GetFlags();
				object value = iD3v2Reader.GetValue();
				if (key.Length > 0 && value is string)
				{
					tags.EvalTagEntry($"{key}={value}");
				}
				else if ((key == "POPM" || key == "POP") && value is byte)
				{
					if (num2 == 0)
					{
						tags.EvalTagEntry($"POPM={value}");
					}
					num2++;
				}
				else if (ReadPictureTAGs && (key == "APIC" || key == "PIC") && value is byte[])
				{
					num++;
					tags.AddPicture(iD3v2Reader.GetPicture(value as byte[], flags, tags.PictureCount, key == "PIC"));
				}
			}
			iD3v2Reader.Close();
			if (ReadPictureTAGs && EvalNativeTAGs)
			{
				tags.AddNativeTag("APIC", num);
			}
		}
		catch
		{
			return false;
		}
		return true;
	}

	private static void ReadAPEPictures(int stream, TAG_INFO tags)
	{
		TagPicture[] array = Bass.BASS_ChannelGetTagsAPEPictures(stream);
		if (array != null && array.Length != 0)
		{
			TagPicture[] array2 = array;
			foreach (TagPicture tagPicture in array2)
			{
				tags.AddPicture(tagPicture);
			}
		}
	}
}

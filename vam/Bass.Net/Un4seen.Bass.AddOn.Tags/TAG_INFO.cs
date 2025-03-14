using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Security;
using System.Text;
using Un4seen.Bass.AddOn.Midi;

namespace Un4seen.Bass.AddOn.Tags;

[Serializable]
[SuppressUnmanagedCodeSecurity]
public class TAG_INFO
{
	public string title = string.Empty;

	public string artist = string.Empty;

	public string album = string.Empty;

	public string albumartist = string.Empty;

	public string year = string.Empty;

	public string comment = string.Empty;

	public string genre = string.Empty;

	public string track = string.Empty;

	public string disc = string.Empty;

	public string copyright = string.Empty;

	public string encodedby = string.Empty;

	public string composer = string.Empty;

	public string publisher = string.Empty;

	public string lyricist = string.Empty;

	public string remixer = string.Empty;

	public string producer = string.Empty;

	public string bpm = string.Empty;

	public string filename = string.Empty;

	private List<TagPicture> pictures = new List<TagPicture>();

	private List<string> nativetags = new List<string>();

	public BASS_CHANNELINFO channelinfo = new BASS_CHANNELINFO();

	public BASSTag tagType = BASSTag.BASS_TAG_UNKNOWN;

	public double duration;

	public int bitrate;

	public float replaygain_track_gain = -100f;

	public float replaygain_track_peak = -1f;

	public string conductor = string.Empty;

	public string grouping = string.Empty;

	public string mood = string.Empty;

	public string rating = string.Empty;

	public string isrc = string.Empty;

	private int _multiCounter;

	private int _commentCounter;

	public int NativeTagsCount
	{
		get
		{
			if (nativetags != null)
			{
				return nativetags.Count;
			}
			return 0;
		}
	}

	public string[] NativeTags => nativetags.ToArray();

	public int PictureCount => pictures.Count;

	public TAG_INFO()
	{
	}

	public TAG_INFO(TAG_INFO clone)
	{
		title = clone.title;
		artist = clone.artist;
		album = clone.album;
		albumartist = clone.albumartist;
		year = clone.year;
		comment = clone.comment;
		genre = clone.genre;
		track = clone.track;
		disc = clone.disc;
		copyright = clone.copyright;
		encodedby = clone.encodedby;
		composer = clone.composer;
		conductor = clone.conductor;
		publisher = clone.publisher;
		lyricist = clone.lyricist;
		remixer = clone.remixer;
		producer = clone.producer;
		bpm = clone.bpm;
		mood = clone.mood;
		grouping = clone.grouping;
		rating = clone.rating;
		isrc = clone.isrc;
		replaygain_track_peak = clone.replaygain_track_peak;
		replaygain_track_gain = clone.replaygain_track_gain;
		filename = clone.filename;
		pictures = new List<TagPicture>(clone.PictureCount);
		foreach (TagPicture picture in clone.pictures)
		{
			TagPicture item = new TagPicture(picture.AttributeIndex, picture.MIMEType, picture.PictureType, picture.Description, picture.Data)
			{
				PictureStorage = picture.PictureStorage
			};
			pictures.Add(item);
		}
		nativetags = new List<string>(clone.nativetags.Count);
		foreach (string nativetag in clone.nativetags)
		{
			nativetags.Add(nativetag);
		}
		channelinfo.chans = clone.channelinfo.chans;
		channelinfo.ctype = clone.channelinfo.ctype;
		channelinfo.filename = clone.channelinfo.filename;
		channelinfo.flags = clone.channelinfo.flags;
		channelinfo.freq = clone.channelinfo.freq;
		channelinfo.origres = clone.channelinfo.origres;
		channelinfo.plugin = clone.channelinfo.plugin;
		channelinfo.sample = clone.channelinfo.sample;
		tagType = clone.tagType;
		duration = clone.duration;
		bitrate = clone.bitrate;
		_multiCounter = clone._multiCounter;
		_commentCounter = clone._commentCounter;
	}

	public TAG_INFO(string FileName)
	{
		filename = FileName;
		title = Path.GetFileNameWithoutExtension(FileName);
	}

	public TAG_INFO(string FileName, bool setDefaultTitle)
	{
		filename = FileName;
		if (setDefaultTitle)
		{
			title = Path.GetFileNameWithoutExtension(FileName);
		}
	}

	public TAG_INFO Clone()
	{
		return new TAG_INFO(this);
	}

	public override string ToString()
	{
		string text = artist;
		if (string.IsNullOrEmpty(text))
		{
			text = albumartist;
		}
		if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(title))
		{
			return $"{text} - {title}";
		}
		if (string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(title))
		{
			return title;
		}
		if (!string.IsNullOrEmpty(text) && string.IsNullOrEmpty(title))
		{
			return text;
		}
		return Path.GetFileNameWithoutExtension(filename);
	}

	public void ClearAllNativeTags()
	{
		nativetags.Clear();
	}

	public string NativeTag(string tagname)
	{
		if (tagname == null)
		{
			return null;
		}
		try
		{
			string result = null;
			foreach (string nativetag in nativetags)
			{
				if (nativetag.StartsWith(tagname))
				{
					string[] array = nativetag.Split(new char[2] { '=', ':' }, 2);
					if (array.Length == 2)
					{
						result = array[1].Trim();
						break;
					}
				}
			}
			return result;
		}
		catch
		{
		}
		return null;
	}

	public bool UpdateFromMETA(IntPtr data, bool utf8, bool multiple)
	{
		return UpdateFromMETA(data, (!utf8) ? TAGINFOEncoding.Latin1 : TAGINFOEncoding.Utf8, multiple);
	}

	public unsafe bool UpdateFromMETA(IntPtr data, TAGINFOEncoding encoding, bool multiple)
	{
		if (data == IntPtr.Zero)
		{
			return false;
		}
		bool flag = false;
		ResetTags();
		string text = null;
		bool flag2 = true;
		int num = 0;
		IntPtr intPtr = data;
		while (flag2)
		{
			switch (encoding)
			{
			case TAGINFOEncoding.Ansi:
				text = Utils.IntPtrAsStringAnsi(new IntPtr((byte*)intPtr.ToPointer() + num));
				if (text != null)
				{
					num += text.Length + 1;
				}
				break;
			case TAGINFOEncoding.Utf8:
			{
				text = Utils.IntPtrAsStringUtf8(new IntPtr((byte*)intPtr.ToPointer() + num), out var len2);
				if (text != null)
				{
					num += len2 + 1;
				}
				break;
			}
			case TAGINFOEncoding.Utf8OrLatin1:
			{
				text = Utils.IntPtrAsStringUtf8orLatin1(new IntPtr((byte*)intPtr.ToPointer() + num), out var len3);
				if (text != null)
				{
					num += len3 + 1;
				}
				break;
			}
			default:
			{
				text = Utils.IntPtrAsStringLatin1(new IntPtr((byte*)intPtr.ToPointer() + num), out var len);
				if (text != null)
				{
					num += len + 1;
				}
				break;
			}
			}
			if (text != null && text.Length != 0)
			{
				if (multiple)
				{
					string[] array = text.Split(';', ',');
					if (array.Length != 0)
					{
						string[] array2 = array;
						foreach (string text2 in array2)
						{
							flag |= EvalTagEntry(text2.Trim());
						}
					}
				}
				else
				{
					flag |= EvalTagEntry(text.Trim());
				}
			}
			else
			{
				flag2 = false;
			}
		}
		return flag;
	}

	public bool UpdateFromMIDILyric(BASS_MIDI_MARK midiMark)
	{
		if (midiMark == null)
		{
			return false;
		}
		bool flag = false;
		if (!track.Equals(midiMark.track.ToString()))
		{
			flag = true;
			track = midiMark.track.ToString();
		}
		if (midiMark.text == null || midiMark.text == string.Empty)
		{
			return false;
		}
		if (midiMark.text.StartsWith("/"))
		{
			comment += "\n";
			if (midiMark.text.Length > 1)
			{
				comment += midiMark.text.Substring(1);
			}
			return true;
		}
		if (midiMark.text.StartsWith("\\"))
		{
			comment = string.Empty;
			if (midiMark.text.Length > 1)
			{
				comment += midiMark.text.Substring(1);
			}
			return true;
		}
		comment += midiMark.text;
		return true;
	}

	public void AddNativeTag(string key, object value)
	{
		string item = $"{key}={value}";
		if (!nativetags.Contains(item))
		{
			nativetags.Add(item);
		}
	}

	public void AddOrReplaceNativeTag(string key, object value)
	{
		bool flag = false;
		string value2 = key + "=";
		for (int i = 0; i < nativetags.Count; i++)
		{
			if (nativetags[i].StartsWith(value2))
			{
				nativetags[i] = $"{key}={value}";
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			nativetags.Add($"{key}={value}");
		}
	}

	public void RemoveNativeTag(string key)
	{
		string value = key + "=";
		for (int i = 0; i < nativetags.Count; i++)
		{
			if (nativetags[i].StartsWith(value))
			{
				nativetags.RemoveAt(i);
				break;
			}
		}
	}

	internal void ResetTags()
	{
		_multiCounter = 0;
		_commentCounter = 0;
		nativetags.Clear();
	}

	internal void ResetTags2()
	{
		_commentCounter = 0;
	}

	internal bool EvalTagEntry(string tagEntry)
	{
		if (string.IsNullOrEmpty(tagEntry))
		{
			return false;
		}
		bool result = false;
		string empty = string.Empty;
		string[] array = tagEntry.Trim().Split(new char[2] { '=', ':' }, 2);
		if (array.Length == 2)
		{
			if (BassTags.EvalNativeTAGs)
			{
				if (NativeTag(array[0].Trim()) != null)
				{
					_multiCounter++;
					array[0] = array[0].Trim() + _multiCounter;
				}
				try
				{
					nativetags.Add(array[0].Trim() + "=" + array[1].Trim());
				}
				catch
				{
				}
			}
			if (array[0].ToLower().Trim().StartsWith("txx"))
			{
				string[] array2 = array[1].Trim().Split(new char[2] { '=', ':' }, 2);
				if (array2.Length == 2)
				{
					array[0] = array2[0].Trim();
					array[1] = array2[1].Trim();
				}
			}
			switch (array[0].ToLower().Trim())
			{
			case "inam":
			case "tit2":
			case "tt2":
			case "title":
			case "wm/title":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					title = empty.Trim('"');
					result = true;
				}
				break;
			case "iart":
			case "artist":
			case "wm/author":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					artist = empty.Trim('"');
					result = true;
				}
				break;
			case "tpe1":
			case "tp1":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					artist = string.Join("; ", empty.Split('\0', '/'));
					result = true;
				}
				break;
			case "istr":
			case "author":
				empty = array[1].Trim();
				if (empty != string.Empty && string.IsNullOrEmpty(artist))
				{
					artist = empty;
					result = true;
				}
				break;
			case "iprd":
			case "talb":
			case "tal":
			case "album":
			case "album1":
			case "wm/albumtitle":
			case "icy-name":
			case "ultravox-name":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					album = empty;
					result = true;
				}
				break;
			case "isbj":
			case "iaar":
			case "albumartist":
			case "album artist":
			case "wm/albumartist":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					albumartist = empty;
					result = true;
				}
				break;
			case "tpe2":
			case "tp2":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					albumartist = string.Join("; ", empty.Split('\0', '/'));
					result = true;
				}
				break;
			case "h2_albumartist":
			case "ensemble":
			case "orchestra":
			case "band":
			case "performer":
				empty = array[1].Trim();
				if (empty != string.Empty && string.IsNullOrEmpty(albumartist))
				{
					albumartist = empty;
					result = true;
				}
				break;
			case "iprt":
			case "itrk":
			case "trck":
			case "trk":
			case "tracknumber":
			case "track":
			case "wm/tracknumber":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					track = empty;
					result = true;
				}
				break;
			case "tracknum":
				empty = array[1].Trim();
				if (empty != string.Empty && string.IsNullOrEmpty(track))
				{
					track = empty;
					result = true;
				}
				break;
			case "ifrm":
			case "tpos":
			case "tpa":
			case "discnumber":
			case "disc":
			case "wm/partofset":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					disc = empty;
					result = true;
				}
				break;
			case "discnum":
				empty = array[1].Trim();
				if (empty != string.Empty && string.IsNullOrEmpty(disc))
				{
					disc = empty;
					result = true;
				}
				break;
			case "icop":
			case "tcop":
			case "tcr":
			case "copyright":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					copyright = empty;
					result = true;
				}
				break;
			case "wm/provider":
			case "provider":
				empty = array[1].Trim();
				if (empty != string.Empty && string.IsNullOrEmpty(copyright))
				{
					copyright = empty;
					result = true;
				}
				break;
			case "isft":
			case "tenc":
			case "ten":
			case "encodedby":
			case "wm/encodedby":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					encodedby = empty;
					result = true;
				}
				break;
			case "tool":
				empty = array[1].Trim();
				if (empty != string.Empty && string.IsNullOrEmpty(encodedby))
				{
					encodedby = empty;
					result = true;
					if (empty[0] < ' ')
					{
						encodedby = ((byte)empty[0]).ToString();
					}
				}
				break;
			case "version":
			case "encoded by":
			case "encoded-by":
			case "encoder":
			case "software":
				empty = array[1].Trim();
				if (empty != string.Empty && string.IsNullOrEmpty(encodedby))
				{
					encodedby = empty;
					result = true;
				}
				break;
			case "icms":
			case "tpub":
			case "tpb":
			case "label":
			case "wm/publisher":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					publisher = empty;
					result = true;
				}
				break;
			case "publisher":
			case "originalsource":
			case "vendor":
				empty = array[1].Trim();
				if (empty != string.Empty && string.IsNullOrEmpty(publisher))
				{
					publisher = empty;
					result = true;
				}
				break;
			case "ieng":
			case "imus":
			case "composer":
			case "wm/composer":
			case "writer":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					composer = empty;
					result = true;
				}
				break;
			case "tcom":
			case "tcm":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					composer = string.Join("; ", empty.Split('\0', '/'));
					result = true;
				}
				break;
			case "organization":
				empty = array[1].Trim();
				if (empty != string.Empty && string.IsNullOrEmpty(composer))
				{
					composer = empty;
					result = true;
				}
				break;
			case "icmt":
			case "comm":
			case "com":
			case "comment":
			case "description":
			case "wm/description":
				empty = array[1].Trim();
				if (empty != string.Empty && _commentCounter == 0)
				{
					string[] array6 = empty.Trim().Split('(', ')', ':');
					if (array6 != null && array6.Length == 4 && array6[1].Length > 0 && array6[3].Length > 0)
					{
						comment = array6[3];
					}
					else
					{
						comment = empty;
					}
					result = true;
					_commentCounter++;
				}
				break;
			case "icrd":
			case "tyer":
			case "tda":
			case "tye":
			case "year":
			case "wm/year":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					year = empty;
					result = true;
				}
				break;
			case "date":
				empty = array[1].Trim();
				if (empty != string.Empty && tagType != BASSTag.BASS_TAG_ID3V2)
				{
					year = empty;
					result = true;
				}
				break;
			case "tdrc":
			case "releasedate":
			case "release date":
				empty = array[1].Trim();
				if (empty != string.Empty && string.IsNullOrEmpty(year))
				{
					year = empty;
					result = true;
				}
				break;
			case "ignr":
			case "genre":
			case "wm/genre":
			case "icy-genre":
			case "ultravox-genre":
				empty = array[1].Trim('\'', '"').Trim();
				if (!(empty != string.Empty))
				{
					break;
				}
				genre = empty;
				result = true;
				if (channelinfo == null || channelinfo.ctype != BASSChannelType.BASS_CTYPE_STREAM_MP4)
				{
					break;
				}
				try
				{
					int num2 = int.Parse(empty);
					try
					{
						genre = BassTags.ID3v1Genre[num2 - 1];
					}
					catch
					{
					}
				}
				catch
				{
				}
				break;
			case "tcon":
			case "tco":
			{
				empty = array[1].Trim();
				if (!(empty != string.Empty))
				{
					break;
				}
				string[] array7 = empty.Split('\0', '/');
				if (array7 != null && array7.Length != 0)
				{
					for (int j = 0; j < array7.Length; j++)
					{
						string text = array7[j].Trim();
						string text2 = string.Empty;
						while (text.Length > 1 && text[0] == '(')
						{
							int num3 = text.IndexOf(')');
							if (num3 < 0)
							{
								break;
							}
							string text3 = text.Substring(1, num3 - 1).Trim();
							int result3 = -1;
							if (int.TryParse(text3, out result3))
							{
								try
								{
									text3 = BassTags.ID3v1Genre[result3];
								}
								catch
								{
								}
							}
							else if (text3 == "RX")
							{
								text3 = "Remix";
							}
							else if (text3 == "CR")
							{
								text3 = "Cover";
							}
							text = text.Substring(num3 + 1).TrimStart('/', ' ');
							if (text3 != null && text.StartsWith(text3))
							{
								text = text.Substring(text3.Length).TrimStart('/', ' ');
							}
							else if (text.StartsWith("(("))
							{
								num3 = text.IndexOf(')');
								text3 = text.Substring(2, num3 - 2).Trim();
								text = text.Substring(num3 + 1).TrimStart('/', ' ');
							}
							else if (text.Length > 0 && text[0] != '(' && text[0] != ' ' && text[0] != '/')
							{
								num3 = text.IndexOfAny(new char[2] { '(', '/' });
								if (num3 > 0)
								{
									text3 = text.Substring(0, num3).Trim();
									text = text.Substring(num3).TrimStart('/', ' ');
								}
								else
								{
									text3 = text;
									text = string.Empty;
								}
							}
							text2 = ((!(text2 == string.Empty)) ? (text2 + "; " + text3) : text3);
						}
						if (text.Length > 0)
						{
							bool flag = true;
							string text4 = text;
							string text5 = text;
							for (int k = 0; k < text5.Length; k++)
							{
								if (!char.IsNumber(text5[k]))
								{
									flag = false;
									break;
								}
							}
							if (flag)
							{
								int result4 = -1;
								if (int.TryParse(text, out result4))
								{
									try
									{
										text4 = BassTags.ID3v1Genre[result4];
									}
									catch
									{
									}
								}
							}
							text2 = ((!(text2 == string.Empty)) ? (text2 + "; " + text4) : text4);
						}
						if (text2 == string.Empty)
						{
							array7[j] = text;
						}
						else
						{
							array7[j] = text2;
						}
					}
					genre = string.Join("; ", array7);
				}
				else
				{
					genre = empty;
				}
				result = true;
				break;
			}
			case "itch":
			case "conductor":
			case "wm/conductor":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					conductor = empty;
					result = true;
				}
				break;
			case "tpe3":
			case "tp3":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					conductor = string.Join("; ", empty.Split('\0', '/'));
					result = true;
				}
				break;
			case "isrf":
			case "tit1":
			case "tt1":
			case "grouping":
			case "wm/contentgroupdescription":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					grouping = empty;
					result = true;
				}
				break;
			case "group":
				empty = array[1].Trim();
				if (empty != string.Empty && string.IsNullOrEmpty(grouping))
				{
					grouping = empty;
					result = true;
				}
				break;
			case "iwri":
			case "lyricist":
			case "wm/writer":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					lyricist = empty;
					result = true;
				}
				break;
			case "text":
			case "txt":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					lyricist = string.Join("; ", empty.Split('\0', '/'));
					result = true;
				}
				break;
			case "songwriter":
			case "texter":
				empty = array[1].Trim();
				if (empty != string.Empty && string.IsNullOrEmpty(lyricist))
				{
					lyricist = empty;
					result = true;
				}
				break;
			case "iedt":
			case "remixer":
			case "wm/modifiedby":
			case "modifiedby":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					remixer = empty;
					result = true;
				}
				break;
			case "tpe4":
			case "tp4":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					remixer = string.Join("; ", empty.Split('\0', '/'));
					result = true;
				}
				break;
			case "ipro":
			case "producer":
			case "wm/producer":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					producer = empty;
					result = true;
				}
				break;
			case "tipl":
			case "ipl":
			{
				empty = array[1].Trim();
				if (!(empty != string.Empty))
				{
					break;
				}
				string[] array5 = empty.Split('\0', '/', ';', ':');
				if (array5 != null && array5.Length > 1)
				{
					for (int i = 0; i < array5.Length; i += 2)
					{
						if (array5[i].Trim().ToLower() == "producer" && i + 1 < array5.Length)
						{
							empty = array5[i + 1].Trim();
							break;
						}
					}
				}
				producer = empty;
				result = true;
				break;
			}
			case "ishp":
			case "irtd":
			case "wm/shareduserrating":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					rating = empty;
					result = true;
				}
				break;
			case "popm":
			case "rating wmp":
			case "rating mm":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					rating = empty;
					int result2 = 0;
					if (int.TryParse(empty, out result2))
					{
						rating = ((result2 == 255) ? 100 : ((result2 != 196 && result2 != 204) ? ((result2 != 128 && result2 != 153) ? ((result2 != 64 && result2 != 102) ? ((result2 == 1 || result2 == 51) ? 20 : ((result2 > 2 && result2 < 255) ? ((int)((float)result2 / 255f * 100f)) : ((result2 > 0 && result2 <= 255) ? 1 : 0))) : 40) : 60) : 80)).ToString();
					}
					result = true;
				}
				break;
			case "rating":
				empty = array[1].Trim();
				if (empty != string.Empty && string.IsNullOrEmpty(rating))
				{
					rating = empty;
					result = true;
					if (empty.Length == 1 && (empty[0] < '0' || empty[0] > '9'))
					{
						rating = ((byte)empty[0]).ToString();
					}
				}
				break;
			case "ikey":
			case "tmoo":
			case "tmo":
			case "mood":
			case "wm/mood":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					mood = empty;
					result = true;
				}
				break;
			case "tsrc":
			case "trc":
			case "isrc":
			case "wm/isrc":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					isrc = empty;
					result = true;
				}
				break;
			case "ibpm":
			case "tbpm":
			case "tbp":
			case "bpm":
			case "wm/beatsperminute":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					if (empty.ToUpper().EndsWith("BPM"))
					{
						empty = empty.Substring(0, empty.Length - 3).Trim().TrimStart('0');
					}
					bpm = empty;
					result = true;
				}
				break;
			case "tempo":
			case "idpi":
			case "h2_bpm":
			case "beatsperminute":
				empty = array[1].Trim();
				if (empty != string.Empty && string.IsNullOrEmpty(bpm))
				{
					if (empty.ToUpper().EndsWith("BPM"))
					{
						empty = empty.Substring(0, empty.Length - 3).Trim().TrimStart('0');
					}
					bpm = empty;
					result = true;
				}
				break;
			case "irgg":
			case "itgl":
			case "replaygain_track_gain":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					if (empty.ToUpper().EndsWith("DB"))
					{
						empty = empty.Substring(0, empty.Length - 2).Trim().TrimStart('0');
					}
					try
					{
						replaygain_track_gain = float.Parse(empty, NumberStyles.Float, CultureInfo.InvariantCulture);
						result = true;
					}
					catch
					{
					}
				}
				break;
			case "irgp":
			case "replaygain_track_peak":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					if (empty.ToUpper().EndsWith("DB"))
					{
						empty = empty.Substring(0, empty.Length - 2).Trim().TrimStart('0');
					}
					try
					{
						replaygain_track_peak = float.Parse(empty, NumberStyles.Float, CultureInfo.InvariantCulture);
						result = true;
					}
					catch
					{
					}
				}
				break;
			case "coverart":
				if (!BassTags.ReadPictureTAGs)
				{
					break;
				}
				try
				{
					byte[] array4 = Convert.FromBase64String(array[1]);
					if (array4 != null && array4.Length != 0)
					{
						TagPicture tagPicture2 = new TagPicture(array4, 0);
						if (tagPicture2.PictureImage != null)
						{
							AddPicture(tagPicture2);
						}
					}
				}
				catch
				{
				}
				break;
			case "metadata_block_picture":
				if (!BassTags.ReadPictureTAGs)
				{
					break;
				}
				try
				{
					byte[] array3 = Convert.FromBase64String(array[1]);
					if (array3 != null && array3.Length != 0)
					{
						TagPicture tagPicture = new TagPicture(array3, 1);
						if (tagPicture.PictureImage != null)
						{
							AddPicture(tagPicture);
						}
					}
				}
				catch
				{
				}
				break;
			case "caption":
			case "streamtitle":
			case "ultravox-title":
			case "ultravoxtitle":
				empty = array[1].Trim('\'', '"').Trim();
				if (empty != string.Empty)
				{
					int num = empty.IndexOf(" - ");
					if (num > 0 && num + 3 < empty.Length)
					{
						artist = empty.Substring(0, num).Trim();
						title = empty.Substring(num + 3).Trim();
					}
					else
					{
						title = empty;
					}
					result = true;
				}
				break;
			case "streamurl":
			case "icy-url":
			case "ultravox-url":
				empty = array[1].Trim('\'', '"').Trim();
				if (empty != string.Empty)
				{
					comment = empty;
					result = true;
				}
				break;
			case "icy-br":
			case "ultravox-bitrate":
				empty = array[1].Trim('\'', '"').Trim();
				if (empty != string.Empty)
				{
					try
					{
						bitrate = int.Parse(empty);
						result = true;
					}
					catch
					{
					}
				}
				break;
			case "currentbitrate":
				empty = array[1].Trim();
				if (empty != string.Empty)
				{
					try
					{
						bitrate = int.Parse(empty) / 1000;
					}
					catch
					{
					}
				}
				break;
			}
		}
		else if (BassTags.EvalNativeTAGs && !nativetags.Contains(tagEntry) && tagEntry != string.Empty)
		{
			nativetags.Add(tagEntry);
		}
		return result;
	}

	public bool AddPicture(TagPicture tagPicture)
	{
		if (tagPicture == null)
		{
			return false;
		}
		bool result = false;
		try
		{
			pictures.Add(tagPicture);
			result = true;
		}
		catch
		{
		}
		return result;
	}

	public void RemovePicture(int i)
	{
		pictures.RemoveAt(i);
	}

	public void RemoveAllPictures()
	{
		pictures.Clear();
	}

	public TagPicture PictureGet(int i)
	{
		if (i < 0 || i > PictureCount - 1)
		{
			return null;
		}
		return pictures[i];
	}

	public Image PictureGetImage(int i)
	{
		if (i < 0 || i > PictureCount - 1)
		{
			return null;
		}
		try
		{
			return pictures[i].PictureImage;
		}
		catch
		{
		}
		return null;
	}

	public string PictureGetDescription(int i)
	{
		if (i < 0 || i > PictureCount - 1)
		{
			return null;
		}
		try
		{
			return pictures[i].Description;
		}
		catch
		{
		}
		return null;
	}

	public string PictureGetType(int i)
	{
		if (i < 0 || i > PictureCount - 1)
		{
			return null;
		}
		try
		{
			return pictures[i].PictureType.ToString();
		}
		catch
		{
		}
		return null;
	}

	public void ReadPicturesFromDirectory(string searchPattern, bool all)
	{
		try
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filename));
			FileInfo[] array = null;
			if (string.IsNullOrEmpty(searchPattern))
			{
				array = directoryInfo.GetFiles(Path.GetFileName(Path.ChangeExtension(filename, ".jpg")));
				if (array == null || array.Length < 1)
				{
					array = directoryInfo.GetFiles(Path.GetFileName(Path.ChangeExtension(filename, ".gif")));
				}
				if (array == null || array.Length < 1)
				{
					array = directoryInfo.GetFiles(Path.GetFileName(Path.ChangeExtension(filename, ".png")));
				}
				if (array == null || array.Length < 1)
				{
					array = directoryInfo.GetFiles(Path.GetFileName(Path.ChangeExtension(filename, ".bmp")));
				}
				if (array == null || array.Length < 1)
				{
					array = directoryInfo.GetFiles("Folder*.jpg");
				}
				if (array == null || array.Length < 1)
				{
					array = directoryInfo.GetFiles("Album*.jpg");
				}
				if (array == null || (array.Length < 1 && !string.IsNullOrEmpty(album)))
				{
					string text = album.Replace('?', '_').Replace('*', '_').Replace('|', '_')
						.Replace('\\', '-')
						.Replace('/', '-')
						.Replace(':', '-')
						.Replace('"', '\'')
						.Replace('<', '(')
						.Replace('>', ')');
					array = directoryInfo.GetFiles(text + "*.jpg");
					if (array == null || array.Length < 1)
					{
						array = directoryInfo.GetFiles(text + "*.gif");
					}
					if (array == null || array.Length < 1)
					{
						array = directoryInfo.GetFiles(text + "*.png");
					}
					if (array == null || array.Length < 1)
					{
						array = directoryInfo.GetFiles(text + "*.bmp");
					}
				}
			}
			else
			{
				array = directoryInfo.GetFiles(searchPattern);
			}
			if (array == null || array.Length == 0)
			{
				return;
			}
			FileInfo[] array2 = array;
			foreach (FileInfo fileInfo in array2)
			{
				AddPicture(new TagPicture(fileInfo.FullName));
				if (!all)
				{
					break;
				}
			}
		}
		catch
		{
		}
	}

	public static List<TagPicture> ReadPicturesFromDirectory(string filename, string album, string searchPattern, bool all)
	{
		List<TagPicture> list = new List<TagPicture>();
		try
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filename));
			FileInfo[] array = null;
			if (string.IsNullOrEmpty(searchPattern))
			{
				array = directoryInfo.GetFiles(Path.GetFileName(Path.ChangeExtension(filename, ".jpg")));
				if (array == null || array.Length < 1)
				{
					array = directoryInfo.GetFiles(Path.GetFileName(Path.ChangeExtension(filename, ".gif")));
				}
				if (array == null || array.Length < 1)
				{
					array = directoryInfo.GetFiles(Path.GetFileName(Path.ChangeExtension(filename, ".png")));
				}
				if (array == null || array.Length < 1)
				{
					array = directoryInfo.GetFiles(Path.GetFileName(Path.ChangeExtension(filename, ".bmp")));
				}
				if (array == null || array.Length < 1)
				{
					array = directoryInfo.GetFiles("Folder*.jpg");
				}
				if (array == null || array.Length < 1)
				{
					array = directoryInfo.GetFiles("Album*.jpg");
				}
				if (array == null || (array.Length < 1 && !string.IsNullOrEmpty(album)))
				{
					string text = album.Replace('?', '_').Replace('*', '_').Replace('|', '_')
						.Replace('\\', '-')
						.Replace('/', '-')
						.Replace(':', '-')
						.Replace('"', '\'')
						.Replace('<', '(')
						.Replace('>', ')');
					array = directoryInfo.GetFiles(text + "*.jpg");
					if (array == null || array.Length < 1)
					{
						array = directoryInfo.GetFiles(text + "*.gif");
					}
					if (array == null || array.Length < 1)
					{
						array = directoryInfo.GetFiles(text + "*.png");
					}
					if (array == null || array.Length < 1)
					{
						array = directoryInfo.GetFiles(text + "*.bmp");
					}
				}
			}
			else
			{
				array = directoryInfo.GetFiles(searchPattern);
			}
			if (array != null && array.Length != 0)
			{
				FileInfo[] array2 = array;
				foreach (FileInfo fileInfo in array2)
				{
					list.Add(new TagPicture(fileInfo.FullName));
					if (!all)
					{
						break;
					}
				}
			}
		}
		catch
		{
		}
		return list;
	}

	public byte[] ConvertToRiffINFO(bool fromNativeTags)
	{
		try
		{
			ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
			List<byte> list = new List<byte>();
			list.AddRange(aSCIIEncoding.GetBytes("INFO"));
			string text = (fromNativeTags ? NativeTag("IART") : artist);
			if (!string.IsNullOrEmpty(text))
			{
				list.AddRange(aSCIIEncoding.GetBytes("IART"));
				list.AddRange(BitConverter.GetBytes(text.Length));
				byte[] bytes = aSCIIEncoding.GetBytes(text);
				list.AddRange(bytes);
				if (bytes.Length % 2 != 0)
				{
					list.Add(0);
				}
			}
			text = (fromNativeTags ? NativeTag("INAM") : title);
			if (!string.IsNullOrEmpty(text))
			{
				list.AddRange(aSCIIEncoding.GetBytes("INAM"));
				list.AddRange(BitConverter.GetBytes(text.Length));
				byte[] bytes = aSCIIEncoding.GetBytes(text);
				list.AddRange(bytes);
				if (bytes.Length % 2 != 0)
				{
					list.Add(0);
				}
			}
			text = (fromNativeTags ? NativeTag("IPRD") : album);
			if (!string.IsNullOrEmpty(text))
			{
				list.AddRange(aSCIIEncoding.GetBytes("IPRD"));
				list.AddRange(BitConverter.GetBytes(text.Length));
				byte[] bytes = aSCIIEncoding.GetBytes(text);
				list.AddRange(bytes);
				if (bytes.Length % 2 != 0)
				{
					list.Add(0);
				}
			}
			text = (fromNativeTags ? NativeTag("ISBJ") : albumartist);
			if (!string.IsNullOrEmpty(text))
			{
				list.AddRange(aSCIIEncoding.GetBytes("ISBJ"));
				list.AddRange(BitConverter.GetBytes(text.Length));
				byte[] bytes = aSCIIEncoding.GetBytes(text);
				list.AddRange(bytes);
				if (bytes.Length % 2 != 0)
				{
					list.Add(0);
				}
			}
			text = (fromNativeTags ? NativeTag("IPRT") : track);
			if (!string.IsNullOrEmpty(text))
			{
				list.AddRange(aSCIIEncoding.GetBytes("IPRT"));
				list.AddRange(BitConverter.GetBytes(text.Length));
				byte[] bytes = aSCIIEncoding.GetBytes(text);
				list.AddRange(bytes);
				if (bytes.Length % 2 != 0)
				{
					list.Add(0);
				}
			}
			text = (fromNativeTags ? NativeTag("IFRM") : disc);
			if (!string.IsNullOrEmpty(text))
			{
				list.AddRange(aSCIIEncoding.GetBytes("IFRM"));
				list.AddRange(BitConverter.GetBytes(text.Length));
				byte[] bytes = aSCIIEncoding.GetBytes(text);
				list.AddRange(bytes);
				if (bytes.Length % 2 != 0)
				{
					list.Add(0);
				}
			}
			text = (fromNativeTags ? NativeTag("ICRD") : year);
			if (!string.IsNullOrEmpty(text))
			{
				list.AddRange(aSCIIEncoding.GetBytes("ICRD"));
				list.AddRange(BitConverter.GetBytes(text.Length));
				byte[] bytes = aSCIIEncoding.GetBytes(text);
				list.AddRange(bytes);
				if (bytes.Length % 2 != 0)
				{
					list.Add(0);
				}
			}
			text = (fromNativeTags ? NativeTag("IGNR") : genre);
			if (!string.IsNullOrEmpty(text))
			{
				list.AddRange(aSCIIEncoding.GetBytes("IGNR"));
				list.AddRange(BitConverter.GetBytes(text.Length));
				byte[] bytes = aSCIIEncoding.GetBytes(text);
				list.AddRange(bytes);
				if (bytes.Length % 2 != 0)
				{
					list.Add(0);
				}
			}
			text = (fromNativeTags ? NativeTag("ICOP") : copyright);
			if (!string.IsNullOrEmpty(text))
			{
				list.AddRange(aSCIIEncoding.GetBytes("ICOP"));
				list.AddRange(BitConverter.GetBytes(text.Length));
				byte[] bytes = aSCIIEncoding.GetBytes(text);
				list.AddRange(bytes);
				if (bytes.Length % 2 != 0)
				{
					list.Add(0);
				}
			}
			text = (fromNativeTags ? NativeTag("ISFT") : encodedby);
			if (!string.IsNullOrEmpty(text))
			{
				list.AddRange(aSCIIEncoding.GetBytes("ISFT"));
				list.AddRange(BitConverter.GetBytes(text.Length));
				byte[] bytes = aSCIIEncoding.GetBytes(text);
				list.AddRange(bytes);
				if (bytes.Length % 2 != 0)
				{
					list.Add(0);
				}
			}
			text = (fromNativeTags ? NativeTag("ICMT") : comment);
			if (!string.IsNullOrEmpty(text))
			{
				list.AddRange(aSCIIEncoding.GetBytes("ICMT"));
				list.AddRange(BitConverter.GetBytes(text.Length));
				byte[] bytes = aSCIIEncoding.GetBytes(text);
				list.AddRange(bytes);
				if (bytes.Length % 2 != 0)
				{
					list.Add(0);
				}
			}
			text = (fromNativeTags ? NativeTag("IENG") : composer);
			if (!string.IsNullOrEmpty(text))
			{
				list.AddRange(aSCIIEncoding.GetBytes("IENG"));
				list.AddRange(BitConverter.GetBytes(text.Length));
				byte[] bytes = aSCIIEncoding.GetBytes(text);
				list.AddRange(bytes);
				if (bytes.Length % 2 != 0)
				{
					list.Add(0);
				}
			}
			text = (fromNativeTags ? NativeTag("ICMS") : publisher);
			if (!string.IsNullOrEmpty(text))
			{
				list.AddRange(aSCIIEncoding.GetBytes("ICMS"));
				list.AddRange(BitConverter.GetBytes(text.Length));
				byte[] bytes = aSCIIEncoding.GetBytes(text);
				list.AddRange(bytes);
				if (bytes.Length % 2 != 0)
				{
					list.Add(0);
				}
			}
			text = (fromNativeTags ? NativeTag("ITCH") : conductor);
			if (!string.IsNullOrEmpty(text))
			{
				list.AddRange(aSCIIEncoding.GetBytes("ITCH"));
				list.AddRange(BitConverter.GetBytes(text.Length));
				byte[] bytes = aSCIIEncoding.GetBytes(text);
				list.AddRange(bytes);
				if (bytes.Length % 2 != 0)
				{
					list.Add(0);
				}
			}
			text = (fromNativeTags ? NativeTag("IWRI") : lyricist);
			if (!string.IsNullOrEmpty(text))
			{
				list.AddRange(aSCIIEncoding.GetBytes("IWRI"));
				list.AddRange(BitConverter.GetBytes(text.Length));
				byte[] bytes = aSCIIEncoding.GetBytes(text);
				list.AddRange(bytes);
				if (bytes.Length % 2 != 0)
				{
					list.Add(0);
				}
			}
			text = (fromNativeTags ? NativeTag("IEDT") : remixer);
			if (!string.IsNullOrEmpty(text))
			{
				list.AddRange(aSCIIEncoding.GetBytes("IEDT"));
				list.AddRange(BitConverter.GetBytes(text.Length));
				byte[] bytes = aSCIIEncoding.GetBytes(text);
				list.AddRange(bytes);
				if (bytes.Length % 2 != 0)
				{
					list.Add(0);
				}
			}
			text = (fromNativeTags ? NativeTag("IPRO") : producer);
			if (!string.IsNullOrEmpty(text))
			{
				list.AddRange(aSCIIEncoding.GetBytes("IPRO"));
				list.AddRange(BitConverter.GetBytes(text.Length));
				byte[] bytes = aSCIIEncoding.GetBytes(text);
				list.AddRange(bytes);
				if (bytes.Length % 2 != 0)
				{
					list.Add(0);
				}
			}
			text = (fromNativeTags ? NativeTag("ISRF") : grouping);
			if (!string.IsNullOrEmpty(text))
			{
				list.AddRange(aSCIIEncoding.GetBytes("ISRF"));
				list.AddRange(BitConverter.GetBytes(text.Length));
				byte[] bytes = aSCIIEncoding.GetBytes(text);
				list.AddRange(bytes);
				if (bytes.Length % 2 != 0)
				{
					list.Add(0);
				}
			}
			text = (fromNativeTags ? NativeTag("IKEY") : mood);
			if (!string.IsNullOrEmpty(text))
			{
				list.AddRange(aSCIIEncoding.GetBytes("IKEY"));
				list.AddRange(BitConverter.GetBytes(text.Length));
				byte[] bytes = aSCIIEncoding.GetBytes(text);
				list.AddRange(bytes);
				if (bytes.Length % 2 != 0)
				{
					list.Add(0);
				}
			}
			text = (fromNativeTags ? NativeTag("ISHP") : rating);
			if (!string.IsNullOrEmpty(text))
			{
				list.AddRange(aSCIIEncoding.GetBytes("ISHP"));
				list.AddRange(BitConverter.GetBytes(text.Length));
				byte[] bytes = aSCIIEncoding.GetBytes(text);
				list.AddRange(bytes);
				if (bytes.Length % 2 != 0)
				{
					list.Add(0);
				}
			}
			text = (fromNativeTags ? NativeTag("ISRC") : isrc);
			if (!string.IsNullOrEmpty(text))
			{
				list.AddRange(aSCIIEncoding.GetBytes("ISRC"));
				list.AddRange(BitConverter.GetBytes(text.Length));
				byte[] bytes = aSCIIEncoding.GetBytes(text);
				list.AddRange(bytes);
				if (bytes.Length % 2 != 0)
				{
					list.Add(0);
				}
			}
			text = (fromNativeTags ? NativeTag("IBPM") : bpm);
			if (!string.IsNullOrEmpty(text))
			{
				list.AddRange(aSCIIEncoding.GetBytes("IBPM"));
				list.AddRange(BitConverter.GetBytes(text.Length));
				byte[] bytes = aSCIIEncoding.GetBytes(text);
				list.AddRange(bytes);
				if (bytes.Length % 2 != 0)
				{
					list.Add(0);
				}
			}
			text = (fromNativeTags ? NativeTag("IRGP") : replaygain_track_peak.ToString("R", CultureInfo.InvariantCulture));
			if ((fromNativeTags && !string.IsNullOrEmpty(text)) || (!fromNativeTags && replaygain_track_peak >= 0f))
			{
				list.AddRange(aSCIIEncoding.GetBytes("IRGP"));
				list.AddRange(BitConverter.GetBytes(text.Length));
				byte[] bytes = aSCIIEncoding.GetBytes(text);
				list.AddRange(bytes);
				if (bytes.Length % 2 != 0)
				{
					list.Add(0);
				}
			}
			text = (fromNativeTags ? NativeTag("IRGG") : (replaygain_track_gain.ToString("R", CultureInfo.InvariantCulture) + " dB"));
			if ((fromNativeTags && !string.IsNullOrEmpty(text)) || (!fromNativeTags && replaygain_track_gain >= -60f && replaygain_track_gain <= 60f))
			{
				list.AddRange(aSCIIEncoding.GetBytes("IRGG"));
				list.AddRange(BitConverter.GetBytes(text.Length));
				byte[] bytes = aSCIIEncoding.GetBytes(text);
				list.AddRange(bytes);
				if (bytes.Length % 2 != 0)
				{
					list.Add(0);
				}
			}
			return list.ToArray();
		}
		catch
		{
			return null;
		}
	}

	public byte[] ConvertToRiffBEXT(bool fromNativeTags)
	{
		try
		{
			BASS_TAG_BEXT bASS_TAG_BEXT = default(BASS_TAG_BEXT);
			bASS_TAG_BEXT.Description = (fromNativeTags ? NativeTag("BWFDescription") : title);
			bASS_TAG_BEXT.Originator = (fromNativeTags ? NativeTag("BWFOriginator") : artist);
			bASS_TAG_BEXT.OriginatorReference = (fromNativeTags ? NativeTag("BWFOriginatorReference") : encodedby);
			if (fromNativeTags)
			{
				bASS_TAG_BEXT.OriginationDate = NativeTag("BWFOriginationDate");
				bASS_TAG_BEXT.OriginationTime = NativeTag("BWFOriginationTime");
				try
				{
					string value = NativeTag("BWFTimeReference");
					if (string.IsNullOrEmpty(value))
					{
						value = "0";
					}
					bASS_TAG_BEXT.TimeReference = Convert.ToInt64(value);
				}
				catch
				{
					bASS_TAG_BEXT.TimeReference = 0L;
				}
			}
			else
			{
				DateTime result = DateTime.Now;
				try
				{
					if (DateTime.TryParseExact(year, new string[7] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM", "yyyy", "yyyy-MM-dd", "dd.MM.yyyy", "MM/dd/yyyy", "HH:mm:ss" }, CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out result))
					{
						bASS_TAG_BEXT.OriginationDate = result.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
						bASS_TAG_BEXT.OriginationTime = result.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
					}
					else if (year.Length >= 19)
					{
						bASS_TAG_BEXT.OriginationDate = year.Substring(0, 10);
						bASS_TAG_BEXT.OriginationTime = year.Substring(11, 8);
					}
					else if (year.Length >= 10)
					{
						bASS_TAG_BEXT.OriginationDate = year.Substring(0, 10);
						bASS_TAG_BEXT.OriginationTime = "00:00:00";
					}
					else
					{
						bASS_TAG_BEXT.OriginationDate = result.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
						bASS_TAG_BEXT.OriginationTime = result.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
					}
				}
				catch
				{
				}
				bASS_TAG_BEXT.TimeReference = 0L;
			}
			bASS_TAG_BEXT.UMID = (fromNativeTags ? NativeTag("BWFUMID") : string.Empty);
			if (fromNativeTags)
			{
				try
				{
					string value2 = NativeTag("BWFVersion");
					if (string.IsNullOrEmpty(value2))
					{
						value2 = "1";
					}
					bASS_TAG_BEXT.Version = Convert.ToInt16(value2);
				}
				catch
				{
					bASS_TAG_BEXT.Version = 1;
				}
			}
			else
			{
				bASS_TAG_BEXT.Version = 1;
			}
			bASS_TAG_BEXT.Reserved = new byte[190];
			return bASS_TAG_BEXT.AsByteArray(fromNativeTags ? NativeTag("BWFCodingHistory") : string.Empty);
		}
		catch
		{
			return null;
		}
	}

	public byte[] ConvertToRiffCART(bool fromNativeTags)
	{
		try
		{
			BASS_TAG_CART bASS_TAG_CART = default(BASS_TAG_CART);
			bASS_TAG_CART.Title = (fromNativeTags ? NativeTag("CARTTitle") : title);
			bASS_TAG_CART.Artist = (fromNativeTags ? NativeTag("CARTArtist") : artist);
			bASS_TAG_CART.CutID = (fromNativeTags ? NativeTag("CARTCutID") : album);
			bASS_TAG_CART.ClientID = (fromNativeTags ? NativeTag("CARTClientID") : copyright);
			bASS_TAG_CART.Category = (fromNativeTags ? NativeTag("CARTCategory") : genre);
			bASS_TAG_CART.Classification = (fromNativeTags ? NativeTag("CARTClassification") : grouping);
			bASS_TAG_CART.ProducerAppID = (fromNativeTags ? NativeTag("CARTProducerAppID") : encodedby);
			if (fromNativeTags)
			{
				bASS_TAG_CART.Version = NativeTag("CARTVersion");
				bASS_TAG_CART.OutCue = NativeTag("CARTOutCue");
				bASS_TAG_CART.StartDate = NativeTag("CARTStartDate");
				bASS_TAG_CART.StartTime = NativeTag("CARTStartTime");
				bASS_TAG_CART.EndDate = NativeTag("CARTEndDate");
				bASS_TAG_CART.EndTime = NativeTag("CARTEndTime");
				bASS_TAG_CART.ProducerAppVersion = NativeTag("CARTProducerAppVersion");
				bASS_TAG_CART.UserDef = NativeTag("CARTUserDef");
				try
				{
					string value = NativeTag("CARTLevelReference");
					if (string.IsNullOrEmpty(value))
					{
						value = "32768";
					}
					bASS_TAG_CART.LevelReference = Convert.ToInt16(value);
				}
				catch
				{
					bASS_TAG_CART.LevelReference = 32768;
				}
				bASS_TAG_CART.Timer1Usage = NativeTag("CARTTimer1Usage");
				bASS_TAG_CART.Timer2Usage = NativeTag("CARTTimer2Usage");
				bASS_TAG_CART.Timer3Usage = NativeTag("CARTTimer3Usage");
				bASS_TAG_CART.Timer4Usage = NativeTag("CARTTimer4Usage");
				bASS_TAG_CART.Timer5Usage = NativeTag("CARTTimer5Usage");
				bASS_TAG_CART.Timer6Usage = NativeTag("CARTTimer6Usage");
				bASS_TAG_CART.Timer7Usage = NativeTag("CARTTimer7Usage");
				bASS_TAG_CART.Timer8Usage = NativeTag("CARTTimer8Usage");
				try
				{
					string value2 = NativeTag("CARTTimer1Value");
					if (string.IsNullOrEmpty(value2))
					{
						value2 = "0";
					}
					bASS_TAG_CART.Timer1Value = Convert.ToInt16(value2);
				}
				catch
				{
					bASS_TAG_CART.Timer1Value = 0;
				}
				try
				{
					string value3 = NativeTag("CARTTimer2Value");
					if (string.IsNullOrEmpty(value3))
					{
						value3 = "0";
					}
					bASS_TAG_CART.Timer2Value = Convert.ToInt16(value3);
				}
				catch
				{
					bASS_TAG_CART.Timer2Value = 0;
				}
				try
				{
					string value4 = NativeTag("CARTTimer3Value");
					if (string.IsNullOrEmpty(value4))
					{
						value4 = "0";
					}
					bASS_TAG_CART.Timer3Value = Convert.ToInt16(value4);
				}
				catch
				{
					bASS_TAG_CART.Timer3Value = 0;
				}
				try
				{
					string value5 = NativeTag("CARTTimer4Value");
					if (string.IsNullOrEmpty(value5))
					{
						value5 = "0";
					}
					bASS_TAG_CART.Timer4Value = Convert.ToInt16(value5);
				}
				catch
				{
					bASS_TAG_CART.Timer4Value = 0;
				}
				try
				{
					string value6 = NativeTag("CARTTimer5Value");
					if (string.IsNullOrEmpty(value6))
					{
						value6 = "0";
					}
					bASS_TAG_CART.Timer5Value = Convert.ToInt16(value6);
				}
				catch
				{
					bASS_TAG_CART.Timer5Value = 0;
				}
				try
				{
					string value7 = NativeTag("CARTTimer6Value");
					if (string.IsNullOrEmpty(value7))
					{
						value7 = "0";
					}
					bASS_TAG_CART.Timer6Value = Convert.ToInt16(value7);
				}
				catch
				{
					bASS_TAG_CART.Timer6Value = 0;
				}
				try
				{
					string value8 = NativeTag("CARTTimer7Value");
					if (string.IsNullOrEmpty(value8))
					{
						value8 = "0";
					}
					bASS_TAG_CART.Timer7Value = Convert.ToInt16(value8);
				}
				catch
				{
					bASS_TAG_CART.Timer7Value = 0;
				}
				try
				{
					string value9 = NativeTag("CARTTimer8Value");
					if (string.IsNullOrEmpty(value9))
					{
						value9 = "0";
					}
					bASS_TAG_CART.Timer8Value = Convert.ToInt16(value9);
				}
				catch
				{
					bASS_TAG_CART.Timer8Value = 0;
				}
				bASS_TAG_CART.URL = NativeTag("CARTURL");
			}
			else
			{
				bASS_TAG_CART.Version = "0100";
				bASS_TAG_CART.OutCue = string.Empty;
				bASS_TAG_CART.StartDate = null;
				bASS_TAG_CART.StartTime = null;
				bASS_TAG_CART.EndDate = null;
				bASS_TAG_CART.EndTime = null;
				bASS_TAG_CART.ProducerAppVersion = string.Empty;
				bASS_TAG_CART.UserDef = string.Empty;
				bASS_TAG_CART.LevelReference = 32768;
				bASS_TAG_CART.Timer1Usage = null;
				bASS_TAG_CART.Timer1Value = 0;
				bASS_TAG_CART.Timer2Usage = null;
				bASS_TAG_CART.Timer2Value = 0;
				bASS_TAG_CART.Timer3Usage = null;
				bASS_TAG_CART.Timer3Value = 0;
				bASS_TAG_CART.Timer4Usage = null;
				bASS_TAG_CART.Timer4Value = 0;
				bASS_TAG_CART.Timer5Usage = null;
				bASS_TAG_CART.Timer5Value = 0;
				bASS_TAG_CART.Timer6Usage = null;
				bASS_TAG_CART.Timer6Value = 0;
				bASS_TAG_CART.Timer7Usage = null;
				bASS_TAG_CART.Timer7Value = 0;
				bASS_TAG_CART.Timer8Usage = null;
				bASS_TAG_CART.Timer8Value = 0;
				bASS_TAG_CART.URL = string.Empty;
			}
			bASS_TAG_CART.Reserved = new byte[276];
			return bASS_TAG_CART.AsByteArray(fromNativeTags ? NativeTag("CARTTagText") : comment);
		}
		catch
		{
			return null;
		}
	}
}

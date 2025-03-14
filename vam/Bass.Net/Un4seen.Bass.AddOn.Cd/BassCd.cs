using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace Un4seen.Bass.AddOn.Cd;

[SuppressUnmanagedCodeSecurity]
public sealed class BassCd
{
	public static string SupportedStreamExtensions = "*.cda";

	public static string SupportedStreamName = "CD Audio";

	private static int _myModuleHandle = 0;

	private const string _myModuleName = "basscd";

	private BassCd()
	{
	}

	[DllImport("basscd")]
	public static extern int BASS_CD_SetInterface(BASSCDInterface iface);

	[DllImport("basscd", EntryPoint = "BASS_CD_GetInfo")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool BASS_CD_GetInfoInternal(int drive, [In][Out] ref BASS_CD_INFO_INTERNAL info);

	public static bool BASS_CD_GetInfo(int drive, BASS_CD_INFO info)
	{
		bool num = BASS_CD_GetInfoInternal(drive, ref info._internal);
		if (num)
		{
			info.vendor = Utils.IntPtrAsStringAnsi(info._internal.vendor);
			info.product = Utils.IntPtrAsStringAnsi(info._internal.product);
			info.rev = Utils.IntPtrAsStringAnsi(info._internal.rev);
			info.letter = info._internal.letter;
			info.rwflags = info._internal.rwflags;
			info.canopen = info._internal.canopen;
			info.canlock = info._internal.canlock;
			info.maxspeed = info._internal.maxspeed;
			info.cache = info._internal.cache;
			info.cdtext = info._internal.cdtext;
		}
		return num;
	}

	public static BASS_CD_INFO BASS_CD_GetInfo(int drive)
	{
		return BASS_CD_GetInfo(drive, release: true);
	}

	public static BASS_CD_INFO BASS_CD_GetInfo(int drive, bool release)
	{
		BASS_CD_INFO bASS_CD_INFO = new BASS_CD_INFO();
		if (BASS_CD_GetInfo(drive, bASS_CD_INFO))
		{
			if (release)
			{
				BASS_CD_Release(drive);
			}
			return bASS_CD_INFO;
		}
		return null;
	}

	public static BASS_CD_INFO[] BASS_CD_GetInfos()
	{
		return BASS_CD_GetInfos(release: true);
	}

	public static BASS_CD_INFO[] BASS_CD_GetInfos(bool release)
	{
		List<BASS_CD_INFO> list = new List<BASS_CD_INFO>();
		int num = 0;
		BASS_CD_INFO item;
		while ((item = BASS_CD_GetInfo(num, release)) != null)
		{
			list.Add(item);
			num++;
		}
		Bass.BASS_GetCPU();
		return list.ToArray();
	}

	public static int BASS_CD_GetDriveCount()
	{
		return BASS_CD_GetDriveCount(release: true);
	}

	public static int BASS_CD_GetDriveCount(bool release)
	{
		int i;
		for (i = 0; BASS_CD_GetInfo(i, release) != null; i++)
		{
		}
		Bass.BASS_GetCPU();
		return i;
	}

	[DllImport("basscd")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_CD_Door(int drive, BASSCDDoor action);

	[DllImport("basscd")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_CD_DoorIsOpen(int drive);

	[DllImport("basscd")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_CD_DoorIsLocked(int drive);

	[DllImport("basscd")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_CD_Release(int drive);

	[DllImport("basscd")]
	public static extern int BASS_CD_GetSpeed(int drive);

	public static float BASS_CD_GetSpeedFactor(int drive)
	{
		float num = BASS_CD_GetSpeed(drive);
		if (num > 0f)
		{
			return num / 176.4f;
		}
		return -1f;
	}

	[DllImport("basscd")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_CD_SetSpeed(int drive, int speed);

	public static bool BASS_CD_SetSpeed(int drive, float factor)
	{
		return BASS_CD_SetSpeed(drive, (int)Math.Ceiling((double)factor * 176.4));
	}

	[DllImport("basscd")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern int BASS_CD_GetCache(int drive);

	[DllImport("basscd")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_CD_SetCache(int drive, [MarshalAs(UnmanagedType.Bool)] bool enable);

	[DllImport("basscd")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_CD_SetOffset(int drive, int offset);

	[DllImport("basscd")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_CD_IsReady(int drive);

	[DllImport("basscd", EntryPoint = "BASS_CD_GetID")]
	private static extern IntPtr BASS_CD_GetIDPtr(int drive, BASSCDId id);

	public static string BASS_CD_GetID(int drive, BASSCDId id)
	{
		IntPtr intPtr = BASS_CD_GetIDPtr(drive, id);
		if (intPtr != IntPtr.Zero)
		{
			if (id == BASSCDId.BASS_CDID_CDDB_QUERY || id == BASSCDId.BASS_CDID_CDDB_READ || id == BASSCDId.BASS_CDID_CDDB_READ_CACHE)
			{
				return Utils.IntPtrAsStringUtf8(intPtr);
			}
			return Utils.IntPtrAsStringAnsi(intPtr);
		}
		return null;
	}

	public static string BASS_CD_GetISRC(int drive, int track)
	{
		IntPtr intPtr = BASS_CD_GetIDPtr(drive, (BASSCDId)(256 + track));
		if (intPtr != IntPtr.Zero)
		{
			return Utils.IntPtrAsStringAnsi(intPtr);
		}
		return null;
	}

	public static string[] BASS_CD_GetIDText(int drive)
	{
		return Utils.IntPtrToArrayNullTermAnsi(BASS_CD_GetIDPtr(drive, BASSCDId.BASS_CDID_TEXT));
	}

	[DllImport("basscd")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool BASS_CD_GetTOC(int drive, BASSCDTOCMode mode, [In][Out] ref BASS_CD_TOC_INTERNAL toc);

	public static bool BASS_CD_GetTOC(int drive, BASSCDTOCMode mode, BASS_CD_TOC toc)
	{
		BASS_CD_TOC_INTERNAL toc2 = default(BASS_CD_TOC_INTERNAL);
		toc2.tracks = new BASS_CD_TOC_TRACK[100];
		if (BASS_CD_GetTOC(drive, mode, ref toc2))
		{
			toc.first = toc2.first;
			toc.last = toc2.last;
			toc.tracks.Clear();
			if (toc2.NumberOfTracks > 0)
			{
				for (int i = 0; i < toc2.NumberOfTracks; i++)
				{
					toc.tracks.Add(toc2.tracks[i]);
				}
			}
			return true;
		}
		toc.first = 0;
		toc.last = 0;
		toc.tracks.Clear();
		return false;
	}

	public static BASS_CD_TOC BASS_CD_GetTOC(int drive, BASSCDTOCMode mode)
	{
		BASS_CD_TOC_INTERNAL toc = default(BASS_CD_TOC_INTERNAL);
		toc.tracks = new BASS_CD_TOC_TRACK[100];
		if (BASS_CD_GetTOC(drive, mode, ref toc))
		{
			BASS_CD_TOC bASS_CD_TOC = new BASS_CD_TOC();
			bASS_CD_TOC.first = toc.first;
			bASS_CD_TOC.last = toc.last;
			if (toc.NumberOfTracks > 0)
			{
				for (int i = 0; i < toc.NumberOfTracks; i++)
				{
					bASS_CD_TOC.tracks.Add(toc.tracks[i]);
				}
			}
			return bASS_CD_TOC;
		}
		return null;
	}

	[DllImport("basscd")]
	public static extern int BASS_CD_GetTracks(int drive);

	[DllImport("basscd")]
	public static extern int BASS_CD_GetTrackLength(int drive, int track);

	public static double BASS_CD_GetTrackLengthSeconds(int drive, int track)
	{
		int num = BASS_CD_GetTrackLength(drive, track);
		if (num >= 0)
		{
			return (double)num / 176400.0;
		}
		return -1.0;
	}

	[DllImport("basscd")]
	public static extern int BASS_CD_GetTrackPregap(int drive, int track);

	[DllImport("basscd")]
	public static extern int BASS_CD_StreamCreate(int drive, int track, BASSFlag flags);

	[DllImport("basscd")]
	public static extern int BASS_CD_StreamCreateEx(int drive, int track, BASSFlag flags, CDDATAPROC proc, IntPtr user);

	[DllImport("basscd", EntryPoint = "BASS_CD_StreamCreateFile")]
	private static extern int BASS_CD_StreamCreateFileUnicode([In][MarshalAs(UnmanagedType.LPWStr)] string file, BASSFlag flags);

	public static int BASS_CD_StreamCreateFile(string file, BASSFlag flags)
	{
		flags |= BASSFlag.BASS_UNICODE;
		return BASS_CD_StreamCreateFileUnicode(file, flags);
	}

	[DllImport("basscd", EntryPoint = "BASS_CD_StreamCreateFileEx")]
	private static extern int BASS_CD_StreamCreateFileUnicode([In][MarshalAs(UnmanagedType.LPWStr)] string file, BASSFlag flags, CDDATAPROC proc, IntPtr user);

	public static int BASS_CD_StreamCreateFileEx(string file, BASSFlag flags, CDDATAPROC proc, IntPtr user)
	{
		flags |= BASSFlag.BASS_UNICODE;
		return BASS_CD_StreamCreateFileUnicode(file, flags, proc, user);
	}

	[DllImport("basscd")]
	public static extern int BASS_CD_StreamGetTrack(int handle);

	[DllImport("basscd")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_CD_StreamSetTrack(int handle, int track);

	[DllImport("basscd")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_CD_Analog_Play(int drive, int track, int pos);

	public static bool BASS_CD_Analog_Play(int drive, int track, double seconds)
	{
		return BASS_CD_Analog_Play(drive, track, (int)Math.Round(seconds * 75.0));
	}

	[DllImport("basscd")]
	public static extern int BASS_CD_Analog_PlayFile([In][MarshalAs(UnmanagedType.LPStr)] string file, int pos);

	public static int BASS_CD_Analog_PlayFile(string file, double seconds)
	{
		return BASS_CD_Analog_PlayFile(file, (int)Math.Round(seconds * 75.0));
	}

	[DllImport("basscd")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_CD_Analog_Stop(int drive);

	[DllImport("basscd")]
	public static extern BASSActive BASS_CD_Analog_IsActive(int drive);

	[DllImport("basscd")]
	public static extern int BASS_CD_Analog_GetPosition(int drive);

	public static double BASS_CD_Analog_GetPosition(int drive, ref int track)
	{
		int num = BASS_CD_Analog_GetPosition(drive);
		if (num >= 0)
		{
			track = Utils.HighWord32(num);
			return (double)Utils.LowWord32(num) / 75.0;
		}
		return -1.0;
	}

	public static bool LoadMe()
	{
		return Utils.LoadLib("basscd", ref _myModuleHandle);
	}

	public static bool LoadMe(string path)
	{
		return Utils.LoadLib(Path.Combine(path, "basscd"), ref _myModuleHandle);
	}

	public static bool FreeMe()
	{
		return Utils.FreeLib(ref _myModuleHandle);
	}
}

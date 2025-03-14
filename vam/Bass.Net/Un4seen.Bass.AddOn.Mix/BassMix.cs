using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace Un4seen.Bass.AddOn.Mix;

[SuppressUnmanagedCodeSecurity]
public sealed class BassMix
{
	public const int BASSMIXVERSION = 516;

	private static int _myModuleHandle;

	private const string _myModuleName = "bassmix";

	private BassMix()
	{
	}

	[DllImport("bassmix")]
	public static extern int BASS_Mixer_GetVersion();

	public static Version BASS_Mixer_GetVersion(int fieldcount)
	{
		if (fieldcount < 1)
		{
			fieldcount = 1;
		}
		if (fieldcount > 4)
		{
			fieldcount = 4;
		}
		int num = BASS_Mixer_GetVersion();
		Version result = new Version(2, 4);
		switch (fieldcount)
		{
		case 1:
			result = new Version((num >> 24) & 0xFF, 0);
			break;
		case 2:
			result = new Version((num >> 24) & 0xFF, (num >> 16) & 0xFF);
			break;
		case 3:
			result = new Version((num >> 24) & 0xFF, (num >> 16) & 0xFF, (num >> 8) & 0xFF);
			break;
		case 4:
			result = new Version((num >> 24) & 0xFF, (num >> 16) & 0xFF, (num >> 8) & 0xFF, num & 0xFF);
			break;
		}
		return result;
	}

	[DllImport("bassmix")]
	public static extern int BASS_Mixer_StreamCreate(int freq, int chans, BASSFlag flags);

	[DllImport("bassmix")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_Mixer_StreamAddChannel(int handle, int channel, BASSFlag flags);

	[DllImport("bassmix")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_Mixer_StreamAddChannelEx(int handle, int channel, BASSFlag flags, long start, long length);

	[DllImport("bassmix", CharSet = CharSet.Unicode)]
	private static extern int BASS_Mixer_StreamGetChannels(int handle, [In][Out] int[] splits, int count);

	public static int[] BASS_Mixer_StreamGetChannels(int handle)
	{
		int num = BASS_Mixer_StreamGetChannels(handle, null, 0);
		if (num < 0)
		{
			return null;
		}
		int[] array = new int[num];
		num = BASS_Mixer_StreamGetChannels(handle, array, num);
		if (num < 0)
		{
			return null;
		}
		return array;
	}

	public static int BASS_Mixer_StreamGetChannelCount(int handle)
	{
		return BASS_Mixer_StreamGetChannels(handle, null, 0);
	}

	[DllImport("bassmix")]
	public static extern int BASS_Mixer_ChannelGetData(int handle, IntPtr buffer, int length);

	[DllImport("bassmix")]
	public static extern int BASS_Mixer_ChannelGetData(int handle, [In][Out] float[] buffer, int length);

	[DllImport("bassmix")]
	public static extern int BASS_Mixer_ChannelGetData(int handle, [In][Out] short[] buffer, int length);

	[DllImport("bassmix")]
	public static extern int BASS_Mixer_ChannelGetData(int handle, [In][Out] int[] buffer, int length);

	[DllImport("bassmix")]
	public static extern int BASS_Mixer_ChannelGetData(int handle, [In][Out] byte[] buffer, int length);

	[DllImport("bassmix")]
	public static extern int BASS_Mixer_ChannelGetLevel(int handle);

	[DllImport("bassmix", EntryPoint = "BASS_ChannelGetLevelEx")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_Mixer_ChannelGetLevel(int handle, [In][Out] float[] levels, float length, BASSLevel flags);

	public static float[] BASS_Mixer_ChannelGetLevel(int handle, float length = 0.02f, BASSLevel flags = BASSLevel.BASS_LEVEL_ALL)
	{
		BASS_CHANNELINFO bASS_CHANNELINFO = Bass.BASS_ChannelGetInfo(handle);
		if (bASS_CHANNELINFO != null)
		{
			int num = bASS_CHANNELINFO.chans;
			if ((flags & BASSLevel.BASS_LEVEL_MONO) == BASSLevel.BASS_LEVEL_MONO)
			{
				num = 1;
			}
			else if ((flags & BASSLevel.BASS_LEVEL_STEREO) == BASSLevel.BASS_LEVEL_STEREO)
			{
				num = 2;
			}
			float[] array = new float[num];
			if (BASS_Mixer_ChannelGetLevel(handle, array, length, flags))
			{
				return array;
			}
			return null;
		}
		return null;
	}

	[DllImport("bassmix")]
	public static extern BASSFlag BASS_Mixer_ChannelFlags(int handle, BASSFlag flags, BASSFlag mask);

	public static bool BASS_Mixer_ChannelPause(int handle)
	{
		if (BASS_Mixer_ChannelFlags(handle, BASSFlag.BASS_STREAM_PRESCAN, BASSFlag.BASS_STREAM_PRESCAN) > BASSFlag.BASS_DEFAULT)
		{
			return true;
		}
		return false;
	}

	public static bool BASS_Mixer_ChannelPlay(int handle)
	{
		if (BASS_Mixer_ChannelFlags(handle, BASSFlag.BASS_DEFAULT, BASSFlag.BASS_STREAM_PRESCAN) >= BASSFlag.BASS_DEFAULT)
		{
			return true;
		}
		return false;
	}

	public static BASSActive BASS_Mixer_ChannelIsActive(int handle)
	{
		BASSFlag bASSFlag = BASS_Mixer_ChannelFlags(handle, BASSFlag.BASS_STREAM_PRESCAN, BASSFlag.BASS_DEFAULT);
		if (bASSFlag < BASSFlag.BASS_DEFAULT)
		{
			return BASSActive.BASS_ACTIVE_STOPPED;
		}
		if ((bASSFlag & BASSFlag.BASS_STREAM_PRESCAN) != 0)
		{
			return BASSActive.BASS_ACTIVE_PAUSED;
		}
		return BASSActive.BASS_ACTIVE_PLAYING;
	}

	[DllImport("bassmix")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_Mixer_ChannelRemove(int handle);

	[DllImport("bassmix")]
	public static extern int BASS_Mixer_ChannelGetMixer(int handle);

	[DllImport("bassmix")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_Mixer_ChannelSetPosition(int handle, long pos, BASSMode mode);

	public static bool BASS_Mixer_ChannelSetPosition(int handle, long pos)
	{
		return BASS_Mixer_ChannelSetPosition(handle, pos, BASSMode.BASS_POS_BYTE);
	}

	[DllImport("bassmix")]
	public static extern long BASS_Mixer_ChannelGetPosition(int handle, BASSMode mode);

	[DllImport("bassmix")]
	public static extern long BASS_Mixer_ChannelGetPositionEx(int handle, BASSMode mode, int delay);

	public static long BASS_Mixer_ChannelGetPosition(int handle)
	{
		return BASS_Mixer_ChannelGetPosition(handle, BASSMode.BASS_POS_BYTE);
	}

	[DllImport("bassmix")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_Mixer_ChannelSetMatrix(int handle, float[,] matrix);

	[DllImport("bassmix")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_Mixer_ChannelSetMatrixEx(int handle, float[,] matrix, float time);

	[DllImport("bassmix")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_Mixer_ChannelGetMatrix(int handle, [In][Out] float[,] matrix);

	[DllImport("bassmix")]
	public static extern long BASS_Mixer_ChannelGetEnvelopePos(int handle, BASSMIXEnvelope type, ref float value);

	[DllImport("bassmix")]
	public static extern long BASS_Mixer_ChannelGetEnvelopePos(int handle, BASSMIXEnvelope type, [In][Out][MarshalAs(UnmanagedType.AsAny)] object value);

	[DllImport("bassmix")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_Mixer_ChannelSetEnvelopePos(int handle, BASSMIXEnvelope type, long pos);

	[DllImport("bassmix")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_Mixer_ChannelSetEnvelope(int handle, BASSMIXEnvelope type, BASS_MIXER_NODE[] nodes, int count);

	public static bool BASS_Mixer_ChannelSetEnvelope(int handle, BASSMIXEnvelope type, BASS_MIXER_NODE[] nodes)
	{
		return BASS_Mixer_ChannelSetEnvelope(handle, type, nodes, (nodes != null) ? nodes.Length : 0);
	}

	[DllImport("bassmix")]
	public static extern int BASS_Mixer_ChannelSetSync(int handle, BASSSync type, long param, SYNCPROC proc, IntPtr user);

	[DllImport("bassmix")]
	private static extern int BASS_Mixer_ChannelSetSync(int handle, BASSSync type, long param, SYNCPROCEX proc, IntPtr user);

	public static int BASS_Mixer_ChannelSetSyncEx(int handle, BASSSync type, long param, SYNCPROCEX proc, IntPtr user)
	{
		type |= (BASSSync)16777216;
		return BASS_Mixer_ChannelSetSync(handle, type, param, proc, user);
	}

	[DllImport("bassmix")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_Mixer_ChannelRemoveSync(int handle, int sync);

	[DllImport("bassmix")]
	public static extern int BASS_Split_StreamCreate(int channel, BASSFlag flags, int[] mapping);

	[DllImport("bassmix")]
	public static extern int BASS_Split_StreamGetSource(int handle);

	[DllImport("bassmix")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_Split_StreamReset(int handle);

	[DllImport("bassmix")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool BASS_Split_StreamResetEx(int handle, int offset);

	[DllImport("bassmix")]
	private static extern int BASS_Split_StreamGetSplits(int handle, [In][Out] int[] splits, int count);

	public static int[] BASS_Split_StreamGetSplits(int handle)
	{
		int num = BASS_Split_StreamGetSplits(handle, null, 0);
		if (num < 0)
		{
			return null;
		}
		int[] array = new int[num];
		num = BASS_Split_StreamGetSplits(handle, array, num);
		if (num < 0)
		{
			return null;
		}
		return array;
	}

	[DllImport("bassmix")]
	public static extern int BASS_Split_StreamGetAvailable(int handle);

	public static bool LoadMe()
	{
		return Utils.LoadLib("bassmix", ref _myModuleHandle);
	}

	public static bool LoadMe(string path)
	{
		return Utils.LoadLib(Path.Combine(path, "bassmix"), ref _myModuleHandle);
	}

	public static bool FreeMe()
	{
		return Utils.FreeLib(ref _myModuleHandle);
	}
}

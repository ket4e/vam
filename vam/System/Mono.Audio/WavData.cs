using System;
using System.IO;

namespace Mono.Audio;

internal class WavData : Mono.Audio.AudioData
{
	private Stream stream;

	private short channels;

	private ushort frame_divider;

	private int sample_rate;

	private int data_len;

	private Mono.Audio.AudioFormat format;

	public override int Channels => channels;

	public override int Rate => sample_rate;

	public override Mono.Audio.AudioFormat Format => format;

	public WavData(Stream data)
	{
		stream = data;
		byte[] array = new byte[44];
		int num = stream.Read(array, 0, 44);
		if (num != 44 || array[0] != 82 || array[1] != 73 || array[2] != 70 || array[3] != 70 || array[8] != 87 || array[9] != 65 || array[10] != 86 || array[11] != 69)
		{
			throw new Exception("incorrect format" + num);
		}
		if (array[12] != 102 || array[13] != 109 || array[14] != 116 || array[15] != 32)
		{
			throw new Exception("incorrect format (fmt)");
		}
		int num2 = array[16];
		num2 |= array[17] << 8;
		num2 |= array[18] << 16;
		num2 |= array[19] << 24;
		int num3 = array[20] | (array[21] << 8);
		if (num3 != 1)
		{
			throw new Exception("incorrect format (not PCM)");
		}
		channels = (short)(array[22] | (array[23] << 8));
		sample_rate = array[24];
		sample_rate |= array[25] << 8;
		sample_rate |= array[26] << 16;
		sample_rate |= array[27] << 24;
		int num4 = array[28];
		num4 |= array[29] << 8;
		num4 |= array[30] << 16;
		num4 |= array[31] << 24;
		int num5 = array[34] | (array[35] << 8);
		if (array[36] != 100 || array[37] != 97 || array[38] != 116 || array[39] != 97)
		{
			throw new Exception("incorrect format (data)");
		}
		int num6 = array[40];
		num6 |= array[41] << 8;
		num6 |= array[42] << 16;
		num6 |= array[43] << 24;
		data_len = num6;
		switch (num5)
		{
		case 8:
			frame_divider = 1;
			format = Mono.Audio.AudioFormat.U8;
			break;
		case 16:
			frame_divider = 2;
			format = Mono.Audio.AudioFormat.S16_LE;
			break;
		default:
			throw new Exception("bits per sample");
		}
	}

	public override void Play(Mono.Audio.AudioDevice dev)
	{
		int num = data_len;
		byte[] array = new byte[4096];
		stream.Position = 0L;
		int num2;
		while (!IsStopped && num >= 0 && (num2 = stream.Read(array, 0, System.Math.Min(array.Length, num))) > 0)
		{
			dev.PlaySample(array, num2 / frame_divider);
			num -= num2;
		}
	}
}

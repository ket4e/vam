using System;
using System.Runtime.InteropServices;

namespace Mono.Audio;

internal class AlsaDevice : Mono.Audio.AudioDevice, IDisposable
{
	private IntPtr handle;

	public AlsaDevice(string name)
	{
		if (name == null)
		{
			name = "default";
		}
		int num = snd_pcm_open(ref handle, name, 0, 0);
		if (num < 0)
		{
			throw new Exception("no open " + num);
		}
	}

	[DllImport("libasound.so.2")]
	private static extern int snd_pcm_open(ref IntPtr handle, string pcm_name, int stream, int mode);

	[DllImport("libasound.so.2")]
	private static extern int snd_pcm_close(IntPtr handle);

	[DllImport("libasound.so.2")]
	private static extern int snd_pcm_drain(IntPtr handle);

	[DllImport("libasound.so.2")]
	private static extern int snd_pcm_writei(IntPtr handle, byte[] buf, int size);

	[DllImport("libasound.so.2")]
	private static extern int snd_pcm_set_params(IntPtr handle, int format, int access, int channels, int rate, int soft_resample, int latency);

	~AlsaDevice()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
		}
		if (handle != IntPtr.Zero)
		{
			snd_pcm_close(handle);
		}
		handle = IntPtr.Zero;
	}

	public override bool SetFormat(Mono.Audio.AudioFormat format, int channels, int rate)
	{
		int num = snd_pcm_set_params(handle, (int)format, 3, channels, rate, 1, 500000);
		return num == 0;
	}

	public override int PlaySample(byte[] buffer, int num_frames)
	{
		return snd_pcm_writei(handle, buffer, num_frames);
	}

	public override void Wait()
	{
		snd_pcm_drain(handle);
	}
}

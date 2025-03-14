namespace Mono.Audio;

internal class AudioDevice
{
	private static Mono.Audio.AudioDevice TryAlsa(string name)
	{
		try
		{
			return new Mono.Audio.AlsaDevice(name);
		}
		catch
		{
			return null;
		}
	}

	public static Mono.Audio.AudioDevice CreateDevice(string name)
	{
		Mono.Audio.AudioDevice audioDevice = TryAlsa(name);
		if (audioDevice == null)
		{
			audioDevice = new Mono.Audio.AudioDevice();
		}
		return audioDevice;
	}

	public virtual bool SetFormat(Mono.Audio.AudioFormat format, int channels, int rate)
	{
		return true;
	}

	public virtual int PlaySample(byte[] buffer, int num_frames)
	{
		return num_frames;
	}

	public virtual void Wait()
	{
	}
}

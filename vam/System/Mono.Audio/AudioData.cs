namespace Mono.Audio;

internal abstract class AudioData
{
	protected const int buffer_size = 4096;

	private bool stopped;

	public abstract int Channels { get; }

	public abstract int Rate { get; }

	public abstract Mono.Audio.AudioFormat Format { get; }

	public virtual bool IsStopped
	{
		get
		{
			return stopped;
		}
		set
		{
			stopped = value;
		}
	}

	public virtual void Setup(Mono.Audio.AudioDevice dev)
	{
		dev.SetFormat(Format, Channels, Rate);
	}

	public abstract void Play(Mono.Audio.AudioDevice dev);
}

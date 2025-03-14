using System;

namespace NAudio.Wave;

public interface IWavePlayer : IDisposable
{
	PlaybackState PlaybackState { get; }

	float Volume { get; set; }

	event EventHandler<StoppedEventArgs> PlaybackStopped;

	void Play();

	void Stop();

	void Pause();

	void Init(IWaveProvider waveProvider);
}

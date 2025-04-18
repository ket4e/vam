using System.Runtime.InteropServices;
using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi;

internal class AudioSessionNotification : IAudioSessionNotification
{
	private AudioSessionManager parent;

	internal AudioSessionNotification(AudioSessionManager parent)
	{
		this.parent = parent;
	}

	[PreserveSig]
	public int OnSessionCreated(IAudioSessionControl newSession)
	{
		parent.FireSessionCreated(newSession);
		return 0;
	}
}

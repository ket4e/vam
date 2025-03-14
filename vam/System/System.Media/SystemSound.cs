using System.IO;

namespace System.Media;

public class SystemSound
{
	private Stream resource;

	internal SystemSound(string tag)
	{
		resource = typeof(SystemSound).Assembly.GetManifestResourceStream(tag + ".wav");
	}

	public void Play()
	{
		SoundPlayer soundPlayer = new SoundPlayer(resource);
		soundPlayer.Play();
	}
}

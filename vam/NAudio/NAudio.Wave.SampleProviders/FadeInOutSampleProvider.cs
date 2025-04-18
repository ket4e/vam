namespace NAudio.Wave.SampleProviders;

public class FadeInOutSampleProvider : ISampleProvider
{
	private enum FadeState
	{
		Silence,
		FadingIn,
		FullVolume,
		FadingOut
	}

	private readonly object lockObject = new object();

	private readonly ISampleProvider source;

	private int fadeSamplePosition;

	private int fadeSampleCount;

	private FadeState fadeState;

	public WaveFormat WaveFormat => source.WaveFormat;

	public FadeInOutSampleProvider(ISampleProvider source, bool initiallySilent = false)
	{
		this.source = source;
		fadeState = ((!initiallySilent) ? FadeState.FullVolume : FadeState.Silence);
	}

	public void BeginFadeIn(double fadeDurationInMilliseconds)
	{
		lock (lockObject)
		{
			fadeSamplePosition = 0;
			fadeSampleCount = (int)(fadeDurationInMilliseconds * (double)source.WaveFormat.SampleRate / 1000.0);
			fadeState = FadeState.FadingIn;
		}
	}

	public void BeginFadeOut(double fadeDurationInMilliseconds)
	{
		lock (lockObject)
		{
			fadeSamplePosition = 0;
			fadeSampleCount = (int)(fadeDurationInMilliseconds * (double)source.WaveFormat.SampleRate / 1000.0);
			fadeState = FadeState.FadingOut;
		}
	}

	public int Read(float[] buffer, int offset, int count)
	{
		int num = source.Read(buffer, offset, count);
		lock (lockObject)
		{
			if (fadeState == FadeState.FadingIn)
			{
				FadeIn(buffer, offset, num);
			}
			else if (fadeState == FadeState.FadingOut)
			{
				FadeOut(buffer, offset, num);
			}
			else if (fadeState == FadeState.Silence)
			{
				ClearBuffer(buffer, offset, count);
			}
		}
		return num;
	}

	private static void ClearBuffer(float[] buffer, int offset, int count)
	{
		for (int i = 0; i < count; i++)
		{
			buffer[i + offset] = 0f;
		}
	}

	private void FadeOut(float[] buffer, int offset, int sourceSamplesRead)
	{
		int num = 0;
		while (num < sourceSamplesRead)
		{
			float num2 = 1f - (float)fadeSamplePosition / (float)fadeSampleCount;
			for (int i = 0; i < source.WaveFormat.Channels; i++)
			{
				buffer[offset + num++] *= num2;
			}
			fadeSamplePosition++;
			if (fadeSamplePosition > fadeSampleCount)
			{
				fadeState = FadeState.Silence;
				ClearBuffer(buffer, num + offset, sourceSamplesRead - num);
				break;
			}
		}
	}

	private void FadeIn(float[] buffer, int offset, int sourceSamplesRead)
	{
		int num = 0;
		while (num < sourceSamplesRead)
		{
			float num2 = (float)fadeSamplePosition / (float)fadeSampleCount;
			for (int i = 0; i < source.WaveFormat.Channels; i++)
			{
				buffer[offset + num++] *= num2;
			}
			fadeSamplePosition++;
			if (fadeSamplePosition > fadeSampleCount)
			{
				fadeState = FadeState.FullVolume;
				break;
			}
		}
	}
}

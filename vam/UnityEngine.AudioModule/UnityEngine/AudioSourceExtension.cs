namespace UnityEngine;

internal class AudioSourceExtension : ScriptableObject
{
	[SerializeField]
	private AudioSource m_audioSource;

	internal int m_ExtensionManagerUpdateIndex = -1;

	public AudioSource audioSource
	{
		get
		{
			return m_audioSource;
		}
		set
		{
			m_audioSource = value;
		}
	}

	public virtual float ReadExtensionProperty(PropertyName propertyName)
	{
		return 0f;
	}

	public virtual void WriteExtensionProperty(PropertyName propertyName, float propertyValue)
	{
	}

	public virtual void Play()
	{
	}

	public virtual void Stop()
	{
	}

	public virtual void ExtensionUpdate()
	{
	}

	public void OnDestroy()
	{
		Stop();
		AudioExtensionManager.RemoveExtensionFromManager(this);
		if (audioSource != null)
		{
			if (audioSource.spatializerExtension == this)
			{
				audioSource.spatializerExtension = null;
			}
			if (audioSource.ambisonicExtension == this)
			{
				audioSource.ambisonicExtension = null;
			}
		}
	}
}

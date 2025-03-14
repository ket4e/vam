namespace UnityEngine;

internal class AudioListenerExtension : ScriptableObject
{
	[SerializeField]
	private AudioListener m_audioListener;

	public AudioListener audioListener
	{
		get
		{
			return m_audioListener;
		}
		set
		{
			m_audioListener = value;
		}
	}

	public virtual float ReadExtensionProperty(PropertyName propertyName)
	{
		return 0f;
	}

	public virtual void WriteExtensionProperty(PropertyName propertyName, float propertyValue)
	{
	}

	public virtual void ExtensionUpdate()
	{
	}
}

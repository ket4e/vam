using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

internal sealed class AudioExtensionManager
{
	private static List<AudioSpatializerExtensionDefinition> m_ListenerSpatializerExtensionDefinitions = new List<AudioSpatializerExtensionDefinition>();

	private static List<AudioSpatializerExtensionDefinition> m_SourceSpatializerExtensionDefinitions = new List<AudioSpatializerExtensionDefinition>();

	private static List<AudioAmbisonicExtensionDefinition> m_SourceAmbisonicDecoderExtensionDefinitions = new List<AudioAmbisonicExtensionDefinition>();

	private static List<AudioSourceExtension> m_SourceExtensionsToUpdate = new List<AudioSourceExtension>();

	private static int m_NextStopIndex = 0;

	private static bool m_BuiltinDefinitionsRegistered = false;

	private static PropertyName m_SpatializerName = 0;

	private static PropertyName m_SpatializerExtensionName = 0;

	private static PropertyName m_ListenerSpatializerExtensionName = 0;

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	internal static extern Object GetAudioListener();

	internal static bool IsListenerSpatializerExtensionRegistered()
	{
		foreach (AudioSpatializerExtensionDefinition listenerSpatializerExtensionDefinition in m_ListenerSpatializerExtensionDefinitions)
		{
			if (AudioSettings.GetSpatializerPluginName() == listenerSpatializerExtensionDefinition.spatializerName)
			{
				return true;
			}
		}
		return false;
	}

	internal static bool IsSourceSpatializerExtensionRegistered()
	{
		foreach (AudioSpatializerExtensionDefinition sourceSpatializerExtensionDefinition in m_SourceSpatializerExtensionDefinitions)
		{
			if (AudioSettings.GetSpatializerPluginName() == sourceSpatializerExtensionDefinition.spatializerName)
			{
				return true;
			}
		}
		return false;
	}

	internal static bool IsSourceAmbisonicDecoderExtensionRegistered()
	{
		foreach (AudioAmbisonicExtensionDefinition sourceAmbisonicDecoderExtensionDefinition in m_SourceAmbisonicDecoderExtensionDefinitions)
		{
			if (AudioSettings.GetAmbisonicDecoderPluginName() == sourceAmbisonicDecoderExtensionDefinition.ambisonicPluginName)
			{
				return true;
			}
		}
		return false;
	}

	internal static AudioSourceExtension AddSpatializerExtension(AudioSource source)
	{
		if (!source.spatialize)
		{
			return null;
		}
		if (source.spatializerExtension != null)
		{
			return source.spatializerExtension;
		}
		RegisterBuiltinDefinitions();
		foreach (AudioSpatializerExtensionDefinition sourceSpatializerExtensionDefinition in m_SourceSpatializerExtensionDefinitions)
		{
			if (AudioSettings.GetSpatializerPluginName() == sourceSpatializerExtensionDefinition.spatializerName)
			{
				AudioSourceExtension audioSourceExtension = source.AddSpatializerExtension(sourceSpatializerExtensionDefinition.definition.GetExtensionType());
				if (audioSourceExtension != null)
				{
					audioSourceExtension.audioSource = source;
					source.spatializerExtension = audioSourceExtension;
					WriteExtensionProperties(audioSourceExtension, sourceSpatializerExtensionDefinition.definition.GetExtensionType().Name);
					return audioSourceExtension;
				}
			}
		}
		return null;
	}

	internal static AudioSourceExtension AddAmbisonicDecoderExtension(AudioSource source)
	{
		if (source.ambisonicExtension != null)
		{
			return source.ambisonicExtension;
		}
		RegisterBuiltinDefinitions();
		foreach (AudioAmbisonicExtensionDefinition sourceAmbisonicDecoderExtensionDefinition in m_SourceAmbisonicDecoderExtensionDefinitions)
		{
			if (AudioSettings.GetAmbisonicDecoderPluginName() == sourceAmbisonicDecoderExtensionDefinition.ambisonicPluginName)
			{
				AudioSourceExtension audioSourceExtension = source.AddAmbisonicExtension(sourceAmbisonicDecoderExtensionDefinition.definition.GetExtensionType());
				if (audioSourceExtension != null)
				{
					audioSourceExtension.audioSource = source;
					source.ambisonicExtension = audioSourceExtension;
					return audioSourceExtension;
				}
			}
		}
		return null;
	}

	internal static void WriteExtensionProperties(AudioSourceExtension extension, string extensionName)
	{
		if (m_SpatializerExtensionName == 0)
		{
			m_SpatializerExtensionName = extensionName;
		}
		for (int i = 0; i < extension.audioSource.GetNumExtensionProperties(); i++)
		{
			if (extension.audioSource.ReadExtensionName(i) == m_SpatializerExtensionName)
			{
				PropertyName propertyName = extension.audioSource.ReadExtensionPropertyName(i);
				float propertyValue = extension.audioSource.ReadExtensionPropertyValue(i);
				extension.WriteExtensionProperty(propertyName, propertyValue);
			}
		}
		extension.audioSource.ClearExtensionProperties(m_SpatializerExtensionName);
	}

	internal static AudioListenerExtension AddSpatializerExtension(AudioListener listener)
	{
		if (listener.spatializerExtension != null)
		{
			return listener.spatializerExtension;
		}
		RegisterBuiltinDefinitions();
		foreach (AudioSpatializerExtensionDefinition listenerSpatializerExtensionDefinition in m_ListenerSpatializerExtensionDefinitions)
		{
			if (AudioSettings.GetSpatializerPluginName() == listenerSpatializerExtensionDefinition.spatializerName || AudioSettings.GetAmbisonicDecoderPluginName() == listenerSpatializerExtensionDefinition.spatializerName)
			{
				AudioListenerExtension audioListenerExtension = listener.AddExtension(listenerSpatializerExtensionDefinition.definition.GetExtensionType());
				if (audioListenerExtension != null)
				{
					audioListenerExtension.audioListener = listener;
					listener.spatializerExtension = audioListenerExtension;
					WriteExtensionProperties(audioListenerExtension, listenerSpatializerExtensionDefinition.definition.GetExtensionType().Name);
					return audioListenerExtension;
				}
			}
		}
		return null;
	}

	internal static void WriteExtensionProperties(AudioListenerExtension extension, string extensionName)
	{
		if (m_ListenerSpatializerExtensionName == 0)
		{
			m_ListenerSpatializerExtensionName = extensionName;
		}
		for (int i = 0; i < extension.audioListener.GetNumExtensionProperties(); i++)
		{
			if (extension.audioListener.ReadExtensionName(i) == m_ListenerSpatializerExtensionName)
			{
				PropertyName propertyName = extension.audioListener.ReadExtensionPropertyName(i);
				float propertyValue = extension.audioListener.ReadExtensionPropertyValue(i);
				extension.WriteExtensionProperty(propertyName, propertyValue);
			}
		}
		extension.audioListener.ClearExtensionProperties(m_ListenerSpatializerExtensionName);
	}

	internal static AudioListenerExtension GetSpatializerExtension(AudioListener listener)
	{
		if (listener.spatializerExtension != null)
		{
			return listener.spatializerExtension;
		}
		return null;
	}

	internal static AudioSourceExtension GetSpatializerExtension(AudioSource source)
	{
		return (!source.spatialize) ? null : source.spatializerExtension;
	}

	internal static AudioSourceExtension GetAmbisonicExtension(AudioSource source)
	{
		return source.ambisonicExtension;
	}

	internal static Type GetListenerSpatializerExtensionType()
	{
		foreach (AudioSpatializerExtensionDefinition listenerSpatializerExtensionDefinition in m_ListenerSpatializerExtensionDefinitions)
		{
			if (AudioSettings.GetSpatializerPluginName() == listenerSpatializerExtensionDefinition.spatializerName)
			{
				return listenerSpatializerExtensionDefinition.definition.GetExtensionType();
			}
		}
		return null;
	}

	internal static Type GetListenerSpatializerExtensionEditorType()
	{
		foreach (AudioSpatializerExtensionDefinition listenerSpatializerExtensionDefinition in m_ListenerSpatializerExtensionDefinitions)
		{
			if (AudioSettings.GetSpatializerPluginName() == listenerSpatializerExtensionDefinition.spatializerName)
			{
				return listenerSpatializerExtensionDefinition.editorDefinition.GetExtensionType();
			}
		}
		return null;
	}

	internal static Type GetSourceSpatializerExtensionType()
	{
		foreach (AudioSpatializerExtensionDefinition sourceSpatializerExtensionDefinition in m_SourceSpatializerExtensionDefinitions)
		{
			if (AudioSettings.GetSpatializerPluginName() == sourceSpatializerExtensionDefinition.spatializerName)
			{
				return sourceSpatializerExtensionDefinition.definition.GetExtensionType();
			}
		}
		return null;
	}

	internal static Type GetSourceSpatializerExtensionEditorType()
	{
		foreach (AudioSpatializerExtensionDefinition sourceSpatializerExtensionDefinition in m_SourceSpatializerExtensionDefinitions)
		{
			if (AudioSettings.GetSpatializerPluginName() == sourceSpatializerExtensionDefinition.spatializerName)
			{
				return sourceSpatializerExtensionDefinition.editorDefinition.GetExtensionType();
			}
		}
		return null;
	}

	internal static Type GetSourceAmbisonicExtensionType()
	{
		foreach (AudioAmbisonicExtensionDefinition sourceAmbisonicDecoderExtensionDefinition in m_SourceAmbisonicDecoderExtensionDefinitions)
		{
			if (AudioSettings.GetAmbisonicDecoderPluginName() == sourceAmbisonicDecoderExtensionDefinition.ambisonicPluginName)
			{
				return sourceAmbisonicDecoderExtensionDefinition.definition.GetExtensionType();
			}
		}
		return null;
	}

	internal static PropertyName GetSpatializerName()
	{
		return m_SpatializerName;
	}

	internal static PropertyName GetSourceSpatializerExtensionName()
	{
		return m_SpatializerExtensionName;
	}

	internal static PropertyName GetListenerSpatializerExtensionName()
	{
		return m_ListenerSpatializerExtensionName;
	}

	internal static void AddExtensionToManager(AudioSourceExtension extension)
	{
		RegisterBuiltinDefinitions();
		if (extension.m_ExtensionManagerUpdateIndex == -1)
		{
			m_SourceExtensionsToUpdate.Add(extension);
			extension.m_ExtensionManagerUpdateIndex = m_SourceExtensionsToUpdate.Count - 1;
		}
	}

	internal static void RemoveExtensionFromManager(AudioSourceExtension extension)
	{
		int extensionManagerUpdateIndex = extension.m_ExtensionManagerUpdateIndex;
		if (extensionManagerUpdateIndex >= 0 && extensionManagerUpdateIndex < m_SourceExtensionsToUpdate.Count)
		{
			int index = m_SourceExtensionsToUpdate.Count - 1;
			m_SourceExtensionsToUpdate[extensionManagerUpdateIndex] = m_SourceExtensionsToUpdate[index];
			m_SourceExtensionsToUpdate[extensionManagerUpdateIndex].m_ExtensionManagerUpdateIndex = extensionManagerUpdateIndex;
			m_SourceExtensionsToUpdate.RemoveAt(index);
		}
		extension.m_ExtensionManagerUpdateIndex = -1;
	}

	internal static void Update()
	{
		RegisterBuiltinDefinitions();
		AudioListener audioListener = GetAudioListener() as AudioListener;
		if (audioListener != null)
		{
			AudioListenerExtension audioListenerExtension = AddSpatializerExtension(audioListener);
			if (audioListenerExtension != null)
			{
				audioListenerExtension.ExtensionUpdate();
			}
		}
		for (int i = 0; i < m_SourceExtensionsToUpdate.Count; i++)
		{
			m_SourceExtensionsToUpdate[i].ExtensionUpdate();
		}
		m_NextStopIndex = ((m_NextStopIndex < m_SourceExtensionsToUpdate.Count) ? m_NextStopIndex : 0);
		int num = ((m_SourceExtensionsToUpdate.Count > 0) ? (1 + m_SourceExtensionsToUpdate.Count / 8) : 0);
		for (int j = 0; j < num; j++)
		{
			AudioSourceExtension audioSourceExtension = m_SourceExtensionsToUpdate[m_NextStopIndex];
			if (audioSourceExtension.audioSource == null || !audioSourceExtension.audioSource.enabled || !audioSourceExtension.audioSource.isPlaying)
			{
				audioSourceExtension.Stop();
				RemoveExtensionFromManager(audioSourceExtension);
			}
			else
			{
				m_NextStopIndex++;
				m_NextStopIndex = ((m_NextStopIndex < m_SourceExtensionsToUpdate.Count) ? m_NextStopIndex : 0);
			}
		}
	}

	internal static void GetReadyToPlay(AudioSourceExtension extension)
	{
		if (extension != null)
		{
			extension.Play();
			AddExtensionToManager(extension);
		}
	}

	private static void RegisterBuiltinDefinitions()
	{
		bool flag = false;
		if (!m_BuiltinDefinitionsRegistered)
		{
			if (flag || AudioSettings.GetSpatializerPluginName() == "GVR Audio Spatializer")
			{
			}
			if (flag || AudioSettings.GetAmbisonicDecoderPluginName() == "GVR Audio Spatializer")
			{
			}
			m_BuiltinDefinitionsRegistered = true;
		}
	}

	private static bool RegisterListenerSpatializerDefinition(string spatializerName, AudioExtensionDefinition extensionDefinition, AudioExtensionDefinition editorDefinition)
	{
		foreach (AudioSpatializerExtensionDefinition listenerSpatializerExtensionDefinition in m_ListenerSpatializerExtensionDefinitions)
		{
			if (spatializerName == listenerSpatializerExtensionDefinition.spatializerName)
			{
				Debug.Log(string.Concat("RegisterListenerSpatializerDefinition failed for ", extensionDefinition.GetExtensionType(), ". We only allow one audio listener extension to be registered for each spatializer."));
				return false;
			}
		}
		AudioSpatializerExtensionDefinition item = new AudioSpatializerExtensionDefinition(spatializerName, extensionDefinition, editorDefinition);
		m_ListenerSpatializerExtensionDefinitions.Add(item);
		return true;
	}

	private static bool RegisterSourceSpatializerDefinition(string spatializerName, AudioExtensionDefinition extensionDefinition, AudioExtensionDefinition editorDefinition)
	{
		foreach (AudioSpatializerExtensionDefinition sourceSpatializerExtensionDefinition in m_SourceSpatializerExtensionDefinitions)
		{
			if (spatializerName == sourceSpatializerExtensionDefinition.spatializerName)
			{
				Debug.Log(string.Concat("RegisterSourceSpatializerDefinition failed for ", extensionDefinition.GetExtensionType(), ". We only allow one audio source extension to be registered for each spatializer."));
				return false;
			}
		}
		AudioSpatializerExtensionDefinition item = new AudioSpatializerExtensionDefinition(spatializerName, extensionDefinition, editorDefinition);
		m_SourceSpatializerExtensionDefinitions.Add(item);
		return true;
	}

	private static bool RegisterSourceAmbisonicDefinition(string ambisonicDecoderName, AudioExtensionDefinition extensionDefinition)
	{
		foreach (AudioAmbisonicExtensionDefinition sourceAmbisonicDecoderExtensionDefinition in m_SourceAmbisonicDecoderExtensionDefinitions)
		{
			if (ambisonicDecoderName == sourceAmbisonicDecoderExtensionDefinition.ambisonicPluginName)
			{
				Debug.Log(string.Concat("RegisterSourceAmbisonicDefinition failed for ", extensionDefinition.GetExtensionType(), ". We only allow one audio source extension to be registered for each ambisonic decoder."));
				return false;
			}
		}
		AudioAmbisonicExtensionDefinition item = new AudioAmbisonicExtensionDefinition(ambisonicDecoderName, extensionDefinition);
		m_SourceAmbisonicDecoderExtensionDefinitions.Add(item);
		return true;
	}
}

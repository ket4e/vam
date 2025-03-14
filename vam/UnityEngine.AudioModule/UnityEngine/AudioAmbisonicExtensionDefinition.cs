namespace UnityEngine;

internal class AudioAmbisonicExtensionDefinition
{
	public PropertyName ambisonicPluginName;

	public AudioExtensionDefinition definition;

	public AudioAmbisonicExtensionDefinition(string ambisonicNameIn, AudioExtensionDefinition definitionIn)
	{
		ambisonicPluginName = ambisonicNameIn;
		definition = definitionIn;
	}
}

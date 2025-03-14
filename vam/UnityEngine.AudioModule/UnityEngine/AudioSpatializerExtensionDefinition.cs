namespace UnityEngine;

internal class AudioSpatializerExtensionDefinition
{
	public PropertyName spatializerName;

	public AudioExtensionDefinition definition;

	public AudioExtensionDefinition editorDefinition;

	public AudioSpatializerExtensionDefinition(string spatializerNameIn, AudioExtensionDefinition definitionIn, AudioExtensionDefinition editorDefinitionIn)
	{
		spatializerName = spatializerNameIn;
		definition = definitionIn;
		editorDefinition = editorDefinitionIn;
	}
}

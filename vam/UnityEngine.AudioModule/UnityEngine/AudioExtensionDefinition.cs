using System;

namespace UnityEngine;

internal class AudioExtensionDefinition
{
	private string assemblyName;

	private string extensionNamespace;

	private string extensionTypeName;

	private Type extensionType;

	public AudioExtensionDefinition(AudioExtensionDefinition definition)
	{
		assemblyName = definition.assemblyName;
		extensionNamespace = definition.extensionNamespace;
		extensionTypeName = definition.extensionTypeName;
		extensionType = GetExtensionType();
	}

	public AudioExtensionDefinition(string assemblyNameIn, string extensionNamespaceIn, string extensionTypeNameIn)
	{
		assemblyName = assemblyNameIn;
		extensionNamespace = extensionNamespaceIn;
		extensionTypeName = extensionTypeNameIn;
		extensionType = GetExtensionType();
	}

	public Type GetExtensionType()
	{
		if (extensionType == null)
		{
			extensionType = Type.GetType(extensionNamespace + "." + extensionTypeName + ", " + assemblyName);
		}
		return extensionType;
	}
}

using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Mono.Remoting.Channels.Unix;

internal class SimpleBinder : SerializationBinder
{
	public static SimpleBinder Instance = new SimpleBinder();

	public override Type BindToType(string assemblyName, string typeName)
	{
		if (assemblyName.IndexOf(',') != -1)
		{
			try
			{
				Assembly assembly = Assembly.Load(assemblyName);
				if (assembly == null)
				{
					return null;
				}
				Type type = assembly.GetType(typeName);
				if (type != null)
				{
					return type;
				}
			}
			catch
			{
			}
		}
		return Assembly.LoadWithPartialName(assemblyName)?.GetType(typeName, throwOnError: true);
	}
}

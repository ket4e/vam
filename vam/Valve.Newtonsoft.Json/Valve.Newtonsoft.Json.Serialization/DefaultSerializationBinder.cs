using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using Valve.Newtonsoft.Json.Utilities;

namespace Valve.Newtonsoft.Json.Serialization;

public class DefaultSerializationBinder : SerializationBinder
{
	internal struct TypeNameKey : IEquatable<TypeNameKey>
	{
		internal readonly string AssemblyName;

		internal readonly string TypeName;

		public TypeNameKey(string assemblyName, string typeName)
		{
			AssemblyName = assemblyName;
			TypeName = typeName;
		}

		public override int GetHashCode()
		{
			return ((AssemblyName != null) ? AssemblyName.GetHashCode() : 0) ^ ((TypeName != null) ? TypeName.GetHashCode() : 0);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is TypeNameKey))
			{
				return false;
			}
			return Equals((TypeNameKey)obj);
		}

		public bool Equals(TypeNameKey other)
		{
			if (AssemblyName == other.AssemblyName)
			{
				return TypeName == other.TypeName;
			}
			return false;
		}
	}

	internal static readonly DefaultSerializationBinder Instance = new DefaultSerializationBinder();

	private readonly ThreadSafeStore<TypeNameKey, Type> _typeCache = new ThreadSafeStore<TypeNameKey, Type>(GetTypeFromTypeNameKey);

	private static Type GetTypeFromTypeNameKey(TypeNameKey typeNameKey)
	{
		string assemblyName = typeNameKey.AssemblyName;
		string typeName = typeNameKey.TypeName;
		if (assemblyName != null)
		{
			Assembly assembly = Assembly.LoadWithPartialName(assemblyName);
			if (assembly == null)
			{
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach (Assembly assembly2 in assemblies)
				{
					if (assembly2.FullName == assemblyName)
					{
						assembly = assembly2;
						break;
					}
				}
			}
			if (assembly == null)
			{
				throw new JsonSerializationException("Could not load assembly '{0}'.".FormatWith(CultureInfo.InvariantCulture, assemblyName));
			}
			return assembly.GetType(typeName) ?? throw new JsonSerializationException("Could not find type '{0}' in assembly '{1}'.".FormatWith(CultureInfo.InvariantCulture, typeName, assembly.FullName));
		}
		return Type.GetType(typeName);
	}

	public override Type BindToType(string assemblyName, string typeName)
	{
		return _typeCache.Get(new TypeNameKey(assemblyName, typeName));
	}
}

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Valve.Newtonsoft.Json.Utilities;

namespace Valve.Newtonsoft.Json.Serialization;

internal static class JsonTypeReflector
{
	private static bool? _dynamicCodeGeneration;

	private static bool? _fullyTrusted;

	public const string IdPropertyName = "$id";

	public const string RefPropertyName = "$ref";

	public const string TypePropertyName = "$type";

	public const string ValuePropertyName = "$value";

	public const string ArrayValuesPropertyName = "$values";

	public const string ShouldSerializePrefix = "ShouldSerialize";

	public const string SpecifiedPostfix = "Specified";

	private static readonly ThreadSafeStore<Type, Func<object[], object>> CreatorCache = new ThreadSafeStore<Type, Func<object[], object>>(GetCreator);

	private static readonly ThreadSafeStore<Type, Type> AssociatedMetadataTypesCache = new ThreadSafeStore<Type, Type>(GetAssociateMetadataTypeFromAttribute);

	private static ReflectionObject _metadataTypeAttributeReflectionObject;

	public static bool DynamicCodeGeneration
	{
		get
		{
			if (!_dynamicCodeGeneration.HasValue)
			{
				_dynamicCodeGeneration = false;
			}
			return _dynamicCodeGeneration.GetValueOrDefault();
		}
	}

	public static bool FullyTrusted
	{
		get
		{
			if (!_fullyTrusted.HasValue)
			{
				try
				{
					new SecurityPermission(PermissionState.Unrestricted).Demand();
					_fullyTrusted = true;
				}
				catch (Exception)
				{
					_fullyTrusted = false;
				}
			}
			return _fullyTrusted.GetValueOrDefault();
		}
	}

	public static ReflectionDelegateFactory ReflectionDelegateFactory => LateBoundReflectionDelegateFactory.Instance;

	public static T GetCachedAttribute<T>(object attributeProvider) where T : Attribute
	{
		return CachedAttributeGetter<T>.GetAttribute(attributeProvider);
	}

	public static DataContractAttribute GetDataContractAttribute(Type type)
	{
		for (Type type2 = type; type2 != null; type2 = type2.BaseType())
		{
			DataContractAttribute attribute = CachedAttributeGetter<DataContractAttribute>.GetAttribute(type2);
			if (attribute != null)
			{
				return attribute;
			}
		}
		return null;
	}

	public static DataMemberAttribute GetDataMemberAttribute(MemberInfo memberInfo)
	{
		if (memberInfo.MemberType() == MemberTypes.Field)
		{
			return CachedAttributeGetter<DataMemberAttribute>.GetAttribute(memberInfo);
		}
		PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
		DataMemberAttribute attribute = CachedAttributeGetter<DataMemberAttribute>.GetAttribute(propertyInfo);
		if (attribute == null && propertyInfo.IsVirtual())
		{
			Type type = propertyInfo.DeclaringType;
			while (attribute == null && type != null)
			{
				PropertyInfo propertyInfo2 = (PropertyInfo)ReflectionUtils.GetMemberInfoFromType(type, propertyInfo);
				if (propertyInfo2 != null && propertyInfo2.IsVirtual())
				{
					attribute = CachedAttributeGetter<DataMemberAttribute>.GetAttribute(propertyInfo2);
				}
				type = type.BaseType();
			}
		}
		return attribute;
	}

	public static MemberSerialization GetObjectMemberSerialization(Type objectType, bool ignoreSerializableAttribute)
	{
		JsonObjectAttribute cachedAttribute = GetCachedAttribute<JsonObjectAttribute>(objectType);
		if (cachedAttribute != null)
		{
			return cachedAttribute.MemberSerialization;
		}
		if (GetDataContractAttribute(objectType) != null)
		{
			return MemberSerialization.OptIn;
		}
		if (!ignoreSerializableAttribute && GetCachedAttribute<SerializableAttribute>(objectType) != null)
		{
			return MemberSerialization.Fields;
		}
		return MemberSerialization.OptOut;
	}

	public static JsonConverter GetJsonConverter(object attributeProvider)
	{
		JsonConverterAttribute cachedAttribute = GetCachedAttribute<JsonConverterAttribute>(attributeProvider);
		if (cachedAttribute != null)
		{
			Func<object[], object> func = CreatorCache.Get(cachedAttribute.ConverterType);
			if (func != null)
			{
				return (JsonConverter)func(cachedAttribute.ConverterParameters);
			}
		}
		return null;
	}

	public static JsonConverter CreateJsonConverterInstance(Type converterType, object[] converterArgs)
	{
		return (JsonConverter)CreatorCache.Get(converterType)(converterArgs);
	}

	public static NamingStrategy CreateNamingStrategyInstance(Type namingStrategyType, object[] converterArgs)
	{
		return (NamingStrategy)CreatorCache.Get(namingStrategyType)(converterArgs);
	}

	public static NamingStrategy GetContainerNamingStrategy(JsonContainerAttribute containerAttribute)
	{
		if (containerAttribute.NamingStrategyInstance == null)
		{
			if (containerAttribute.NamingStrategyType == null)
			{
				return null;
			}
			containerAttribute.NamingStrategyInstance = CreateNamingStrategyInstance(containerAttribute.NamingStrategyType, containerAttribute.NamingStrategyParameters);
		}
		return containerAttribute.NamingStrategyInstance;
	}

	private static Func<object[], object> GetCreator(Type type)
	{
		Func<object> defaultConstructor = (ReflectionUtils.HasDefaultConstructor(type, nonPublic: false) ? ReflectionDelegateFactory.CreateDefaultConstructor<object>(type) : null);
		return delegate(object[] parameters)
		{
			try
			{
				if (parameters != null)
				{
					Type[] types = parameters.Select((object param) => param.GetType()).ToArray();
					ConstructorInfo constructor = type.GetConstructor(types);
					if (constructor == null)
					{
						throw new JsonException("No matching parameterized constructor found for '{0}'.".FormatWith(CultureInfo.InvariantCulture, type));
					}
					return ReflectionDelegateFactory.CreateParameterizedConstructor(constructor)(parameters);
				}
				if (defaultConstructor == null)
				{
					throw new JsonException("No parameterless constructor defined for '{0}'.".FormatWith(CultureInfo.InvariantCulture, type));
				}
				return defaultConstructor();
			}
			catch (Exception innerException)
			{
				throw new JsonException("Error creating '{0}'.".FormatWith(CultureInfo.InvariantCulture, type), innerException);
			}
		};
	}

	public static TypeConverter GetTypeConverter(Type type)
	{
		return TypeDescriptor.GetConverter(type);
	}

	private static Type GetAssociatedMetadataType(Type type)
	{
		return AssociatedMetadataTypesCache.Get(type);
	}

	private static Type GetAssociateMetadataTypeFromAttribute(Type type)
	{
		Attribute[] attributes = ReflectionUtils.GetAttributes(type, null, inherit: true);
		foreach (Attribute attribute in attributes)
		{
			Type type2 = attribute.GetType();
			if (string.Equals(type2.FullName, "System.ComponentModel.DataAnnotations.MetadataTypeAttribute", StringComparison.Ordinal))
			{
				if (_metadataTypeAttributeReflectionObject == null)
				{
					_metadataTypeAttributeReflectionObject = ReflectionObject.Create(type2, "MetadataClassType");
				}
				return (Type)_metadataTypeAttributeReflectionObject.GetValue(attribute, "MetadataClassType");
			}
		}
		return null;
	}

	private static T GetAttribute<T>(Type type) where T : Attribute
	{
		Type associatedMetadataType = GetAssociatedMetadataType(type);
		T attribute;
		if (associatedMetadataType != null)
		{
			attribute = ReflectionUtils.GetAttribute<T>(associatedMetadataType, inherit: true);
			if (attribute != null)
			{
				return attribute;
			}
		}
		attribute = ReflectionUtils.GetAttribute<T>(type, inherit: true);
		if (attribute != null)
		{
			return attribute;
		}
		Type[] interfaces = type.GetInterfaces();
		for (int i = 0; i < interfaces.Length; i++)
		{
			attribute = ReflectionUtils.GetAttribute<T>(interfaces[i], inherit: true);
			if (attribute != null)
			{
				return attribute;
			}
		}
		return null;
	}

	private static T GetAttribute<T>(MemberInfo memberInfo) where T : Attribute
	{
		Type associatedMetadataType = GetAssociatedMetadataType(memberInfo.DeclaringType);
		T attribute;
		if (associatedMetadataType != null)
		{
			MemberInfo memberInfoFromType = ReflectionUtils.GetMemberInfoFromType(associatedMetadataType, memberInfo);
			if (memberInfoFromType != null)
			{
				attribute = ReflectionUtils.GetAttribute<T>(memberInfoFromType, inherit: true);
				if (attribute != null)
				{
					return attribute;
				}
			}
		}
		attribute = ReflectionUtils.GetAttribute<T>(memberInfo, inherit: true);
		if (attribute != null)
		{
			return attribute;
		}
		if (memberInfo.DeclaringType != null)
		{
			Type[] interfaces = memberInfo.DeclaringType.GetInterfaces();
			for (int i = 0; i < interfaces.Length; i++)
			{
				MemberInfo memberInfoFromType2 = ReflectionUtils.GetMemberInfoFromType(interfaces[i], memberInfo);
				if (memberInfoFromType2 != null)
				{
					attribute = ReflectionUtils.GetAttribute<T>(memberInfoFromType2, inherit: true);
					if (attribute != null)
					{
						return attribute;
					}
				}
			}
		}
		return null;
	}

	public static T GetAttribute<T>(object provider) where T : Attribute
	{
		if (provider is Type type)
		{
			return GetAttribute<T>(type);
		}
		if (provider is MemberInfo memberInfo)
		{
			return GetAttribute<T>(memberInfo);
		}
		return ReflectionUtils.GetAttribute<T>(provider, inherit: true);
	}
}

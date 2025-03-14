using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using Valve.Newtonsoft.Json.Linq;
using Valve.Newtonsoft.Json.Utilities;

namespace Valve.Newtonsoft.Json.Serialization;

public class JsonObjectContract : JsonContainerContract
{
	internal bool ExtensionDataIsJToken;

	private bool? _hasRequiredOrDefaultValueProperties;

	private ConstructorInfo _parametrizedConstructor;

	private ConstructorInfo _overrideConstructor;

	private ObjectConstructor<object> _overrideCreator;

	private ObjectConstructor<object> _parameterizedCreator;

	private JsonPropertyCollection _creatorParameters;

	private Type _extensionDataValueType;

	public MemberSerialization MemberSerialization { get; set; }

	public Required? ItemRequired { get; set; }

	public JsonPropertyCollection Properties { get; private set; }

	[Obsolete("ConstructorParameters is obsolete. Use CreatorParameters instead.")]
	public JsonPropertyCollection ConstructorParameters => CreatorParameters;

	public JsonPropertyCollection CreatorParameters
	{
		get
		{
			if (_creatorParameters == null)
			{
				_creatorParameters = new JsonPropertyCollection(base.UnderlyingType);
			}
			return _creatorParameters;
		}
	}

	[Obsolete("OverrideConstructor is obsolete. Use OverrideCreator instead.")]
	public ConstructorInfo OverrideConstructor
	{
		get
		{
			return _overrideConstructor;
		}
		set
		{
			_overrideConstructor = value;
			_overrideCreator = ((value != null) ? JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(value) : null);
		}
	}

	[Obsolete("ParametrizedConstructor is obsolete. Use OverrideCreator instead.")]
	public ConstructorInfo ParametrizedConstructor
	{
		get
		{
			return _parametrizedConstructor;
		}
		set
		{
			_parametrizedConstructor = value;
			_parameterizedCreator = ((value != null) ? JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(value) : null);
		}
	}

	public ObjectConstructor<object> OverrideCreator
	{
		get
		{
			return _overrideCreator;
		}
		set
		{
			_overrideCreator = value;
			_overrideConstructor = null;
		}
	}

	internal ObjectConstructor<object> ParameterizedCreator => _parameterizedCreator;

	public ExtensionDataSetter ExtensionDataSetter { get; set; }

	public ExtensionDataGetter ExtensionDataGetter { get; set; }

	public Type ExtensionDataValueType
	{
		get
		{
			return _extensionDataValueType;
		}
		set
		{
			_extensionDataValueType = value;
			ExtensionDataIsJToken = value != null && typeof(JToken).IsAssignableFrom(value);
		}
	}

	internal bool HasRequiredOrDefaultValueProperties
	{
		get
		{
			if (!_hasRequiredOrDefaultValueProperties.HasValue)
			{
				_hasRequiredOrDefaultValueProperties = false;
				if (ItemRequired.GetValueOrDefault(Required.Default) != 0)
				{
					_hasRequiredOrDefaultValueProperties = true;
				}
				else
				{
					foreach (JsonProperty property in Properties)
					{
						if (property.Required != 0 || ((uint?)property.DefaultValueHandling & 2u) == 2)
						{
							_hasRequiredOrDefaultValueProperties = true;
							break;
						}
					}
				}
			}
			return _hasRequiredOrDefaultValueProperties.GetValueOrDefault();
		}
	}

	public JsonObjectContract(Type underlyingType)
		: base(underlyingType)
	{
		ContractType = JsonContractType.Object;
		Properties = new JsonPropertyCollection(base.UnderlyingType);
	}

	internal object GetUninitializedObject()
	{
		if (!JsonTypeReflector.FullyTrusted)
		{
			throw new JsonException("Insufficient permissions. Creating an uninitialized '{0}' type requires full trust.".FormatWith(CultureInfo.InvariantCulture, NonNullableUnderlyingType));
		}
		return FormatterServices.GetUninitializedObject(NonNullableUnderlyingType);
	}
}

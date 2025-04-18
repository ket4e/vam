using System.Globalization;
using Valve.Newtonsoft.Json.Utilities;

namespace Valve.Newtonsoft.Json.Serialization;

internal class DefaultReferenceResolver : IReferenceResolver
{
	private int _referenceCount;

	private BidirectionalDictionary<string, object> GetMappings(object context)
	{
		JsonSerializerInternalBase jsonSerializerInternalBase;
		if (context is JsonSerializerInternalBase)
		{
			jsonSerializerInternalBase = (JsonSerializerInternalBase)context;
		}
		else
		{
			if (!(context is JsonSerializerProxy))
			{
				throw new JsonException("The DefaultReferenceResolver can only be used internally.");
			}
			jsonSerializerInternalBase = ((JsonSerializerProxy)context).GetInternalSerializer();
		}
		return jsonSerializerInternalBase.DefaultReferenceMappings;
	}

	public object ResolveReference(object context, string reference)
	{
		GetMappings(context).TryGetByFirst(reference, out var second);
		return second;
	}

	public string GetReference(object context, object value)
	{
		BidirectionalDictionary<string, object> mappings = GetMappings(context);
		if (!mappings.TryGetBySecond(value, out var first))
		{
			_referenceCount++;
			first = _referenceCount.ToString(CultureInfo.InvariantCulture);
			mappings.Set(first, value);
		}
		return first;
	}

	public void AddReference(object context, string reference, object value)
	{
		GetMappings(context).Set(reference, value);
	}

	public bool IsReferenced(object context, object value)
	{
		string first;
		return GetMappings(context).TryGetBySecond(value, out first);
	}
}

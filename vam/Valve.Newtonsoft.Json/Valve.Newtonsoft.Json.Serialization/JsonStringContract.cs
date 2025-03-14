using System;

namespace Valve.Newtonsoft.Json.Serialization;

public class JsonStringContract : JsonPrimitiveContract
{
	public JsonStringContract(Type underlyingType)
		: base(underlyingType)
	{
		ContractType = JsonContractType.String;
	}
}

using System;
using System.Collections.Generic;

namespace Valve.Newtonsoft.Json.Serialization;

public interface IAttributeProvider
{
	IList<Attribute> GetAttributes(bool inherit);

	IList<Attribute> GetAttributes(Type attributeType, bool inherit);
}

using System;

namespace Valve.Newtonsoft.Json;

[Flags]
public enum DefaultValueHandling
{
	Include = 0,
	Ignore = 1,
	Populate = 2,
	IgnoreAndPopulate = 3
}

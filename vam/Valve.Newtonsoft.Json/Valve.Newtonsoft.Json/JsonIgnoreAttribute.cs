using System;

namespace Valve.Newtonsoft.Json;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class JsonIgnoreAttribute : Attribute
{
}

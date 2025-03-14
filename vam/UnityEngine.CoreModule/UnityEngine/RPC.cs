using System;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Attribute for setting up RPC functions.</para>
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[RequiredByNativeCode(Optional = true)]
[Obsolete("NetworkView RPC functions are deprecated. Refer to the new Multiplayer Networking system.")]
public sealed class RPC : Attribute
{
}

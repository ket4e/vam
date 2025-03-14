using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Asynchronous load request from the Resources bundle.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[RequiredByNativeCode]
public class ResourceRequest : AsyncOperation
{
	internal string m_Path;

	internal Type m_Type;

	/// <summary>
	///   <para>Asset object being loaded (Read Only).</para>
	/// </summary>
	public Object asset => Resources.Load(m_Path, m_Type);
}

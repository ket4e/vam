using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>MonoBehaviour.StartCoroutine returns a Coroutine. Instances of this class are only used to reference these coroutines, and do not hold any exposed properties or functions.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[NativeHeader("Runtime/Mono/Coroutine.h")]
[RequiredByNativeCode]
public sealed class Coroutine : YieldInstruction
{
	internal IntPtr m_Ptr;

	private Coroutine()
	{
	}

	~Coroutine()
	{
		ReleaseCoroutine(m_Ptr);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Coroutine::CleanupCoroutineGC", true)]
	private static extern void ReleaseCoroutine(IntPtr ptr);
}

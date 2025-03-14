using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Networking;

/// <summary>
///   <para>Asynchronous operation object returned from UnityWebRequest.SendWebRequest().
///
/// You can yield until it continues, register an event handler with AsyncOperation.completed, or manually check whether it's done (AsyncOperation.isDone) or progress (AsyncOperation.progress).</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[NativeHeader("UnityWebRequestScriptingClasses.h")]
[NativeHeader("Modules/UnityWebRequest/Public/UnityWebRequestAsyncOperation.h")]
[UsedByNativeCode]
public class UnityWebRequestAsyncOperation : AsyncOperation
{
	/// <summary>
	///   <para>Returns the associated UnityWebRequest that created the operation.</para>
	/// </summary>
	public UnityWebRequest webRequest { get; internal set; }
}

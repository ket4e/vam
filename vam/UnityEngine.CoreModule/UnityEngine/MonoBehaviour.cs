using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>MonoBehaviour is the base class from which every Unity script derives.</para>
/// </summary>
[RequiredByNativeCode]
[NativeHeader("Runtime/Mono/MonoBehaviour.h")]
[NativeHeader("Runtime/Scripting/DelayedCallUtility.h")]
public class MonoBehaviour : Behaviour
{
	/// <summary>
	///   <para>Disabling this lets you skip the GUI layout phase.</para>
	/// </summary>
	public extern bool useGUILayout
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Is any invoke pending on this MonoBehaviour?</para>
	/// </summary>
	public bool IsInvoking()
	{
		return Internal_IsInvokingAll(this);
	}

	/// <summary>
	///   <para>Cancels all Invoke calls on this MonoBehaviour.</para>
	/// </summary>
	public void CancelInvoke()
	{
		Internal_CancelInvokeAll(this);
	}

	/// <summary>
	///   <para>Invokes the method methodName in time seconds.</para>
	/// </summary>
	/// <param name="methodName"></param>
	/// <param name="time"></param>
	public void Invoke(string methodName, float time)
	{
		InvokeDelayed(this, methodName, time, 0f);
	}

	/// <summary>
	///   <para>Invokes the method methodName in time seconds, then repeatedly every repeatRate seconds.</para>
	/// </summary>
	/// <param name="methodName"></param>
	/// <param name="time"></param>
	/// <param name="repeatRate"></param>
	public void InvokeRepeating(string methodName, float time, float repeatRate)
	{
		if (repeatRate <= 1E-05f && repeatRate != 0f)
		{
			throw new UnityException("Invoke repeat rate has to be larger than 0.00001F)");
		}
		InvokeDelayed(this, methodName, time, repeatRate);
	}

	/// <summary>
	///   <para>Cancels all Invoke calls with name methodName on this behaviour.</para>
	/// </summary>
	/// <param name="methodName"></param>
	public void CancelInvoke(string methodName)
	{
		CancelInvoke(this, methodName);
	}

	/// <summary>
	///   <para>Is any invoke on methodName pending?</para>
	/// </summary>
	/// <param name="methodName"></param>
	public bool IsInvoking(string methodName)
	{
		return IsInvoking(this, methodName);
	}

	/// <summary>
	///   <para>Starts a coroutine named methodName.</para>
	/// </summary>
	/// <param name="methodName"></param>
	/// <param name="value"></param>
	[ExcludeFromDocs]
	public Coroutine StartCoroutine(string methodName)
	{
		object value = null;
		return StartCoroutine(methodName, value);
	}

	/// <summary>
	///   <para>Starts a coroutine named methodName.</para>
	/// </summary>
	/// <param name="methodName"></param>
	/// <param name="value"></param>
	public Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value)
	{
		if (string.IsNullOrEmpty(methodName))
		{
			throw new NullReferenceException("methodName is null or empty");
		}
		if (!IsObjectMonoBehaviour(this))
		{
			throw new ArgumentException("Coroutines can only be stopped on a MonoBehaviour");
		}
		return StartCoroutineManaged(methodName, value);
	}

	/// <summary>
	///   <para>Starts a coroutine.</para>
	/// </summary>
	/// <param name="routine"></param>
	public Coroutine StartCoroutine(IEnumerator routine)
	{
		if (routine == null)
		{
			throw new NullReferenceException("routine is null");
		}
		if (!IsObjectMonoBehaviour(this))
		{
			throw new ArgumentException("Coroutines can only be stopped on a MonoBehaviour");
		}
		return StartCoroutineManaged2(routine);
	}

	[Obsolete("StartCoroutine_Auto has been deprecated. Use StartCoroutine instead (UnityUpgradable) -> StartCoroutine([mscorlib] System.Collections.IEnumerator)", false)]
	public Coroutine StartCoroutine_Auto(IEnumerator routine)
	{
		return StartCoroutine(routine);
	}

	/// <summary>
	///   <para>Stops the first coroutine named methodName, or the coroutine stored in routine running on this behaviour.</para>
	/// </summary>
	/// <param name="methodName">Name of coroutine.</param>
	/// <param name="routine">Name of the function in code, including coroutines.</param>
	public void StopCoroutine(IEnumerator routine)
	{
		if (routine == null)
		{
			throw new NullReferenceException("routine is null");
		}
		if (!IsObjectMonoBehaviour(this))
		{
			throw new ArgumentException("Coroutines can only be stopped on a MonoBehaviour");
		}
		StopCoroutineFromEnumeratorManaged(routine);
	}

	/// <summary>
	///   <para>Stops the first coroutine named methodName, or the coroutine stored in routine running on this behaviour.</para>
	/// </summary>
	/// <param name="methodName">Name of coroutine.</param>
	/// <param name="routine">Name of the function in code, including coroutines.</param>
	public void StopCoroutine(Coroutine routine)
	{
		if (routine == null)
		{
			throw new NullReferenceException("routine is null");
		}
		if (!IsObjectMonoBehaviour(this))
		{
			throw new ArgumentException("Coroutines can only be stopped on a MonoBehaviour");
		}
		StopCoroutineManaged(routine);
	}

	/// <summary>
	///   <para>Stops the first coroutine named methodName, or the coroutine stored in routine running on this behaviour.</para>
	/// </summary>
	/// <param name="methodName">Name of coroutine.</param>
	/// <param name="routine">Name of the function in code, including coroutines.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void StopCoroutine(string methodName);

	/// <summary>
	///   <para>Stops all coroutines running on this behaviour.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void StopAllCoroutines();

	/// <summary>
	///   <para>Logs message to the Unity Console (identical to Debug.Log).</para>
	/// </summary>
	/// <param name="message"></param>
	public static void print(object message)
	{
		Debug.Log(message);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("CancelInvoke")]
	private static extern void Internal_CancelInvokeAll(MonoBehaviour self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("IsInvoking")]
	private static extern bool Internal_IsInvokingAll(MonoBehaviour self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	private static extern void InvokeDelayed(MonoBehaviour self, string methodName, float time, float repeatRate);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	private static extern void CancelInvoke(MonoBehaviour self, string methodName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	private static extern bool IsInvoking(MonoBehaviour self, string methodName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	private static extern bool IsObjectMonoBehaviour(Object obj);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern Coroutine StartCoroutineManaged(string methodName, object value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern Coroutine StartCoroutineManaged2(IEnumerator enumerator);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void StopCoroutineManaged(Coroutine routine);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void StopCoroutineFromEnumeratorManaged(IEnumerator routine);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern string GetScriptClassName();
}

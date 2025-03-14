using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Asynchronous operation coroutine.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[RequiredByNativeCode]
public class AsyncOperation : YieldInstruction
{
	internal IntPtr m_Ptr;

	private Action<AsyncOperation> m_completeCallback;

	/// <summary>
	///   <para>Has the operation finished? (Read Only)</para>
	/// </summary>
	public extern bool isDone
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>What's the operation's progress. (Read Only)</para>
	/// </summary>
	public extern float progress
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Priority lets you tweak in which order async operation calls will be performed.</para>
	/// </summary>
	public extern int priority
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	/// <summary>
	///   <para>Allow scenes to be activated as soon as it is ready.</para>
	/// </summary>
	public extern bool allowSceneActivation
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		set;
	}

	public event Action<AsyncOperation> completed
	{
		add
		{
			if (isDone)
			{
				value(this);
			}
			else
			{
				m_completeCallback = (Action<AsyncOperation>)Delegate.Combine(m_completeCallback, value);
			}
		}
		remove
		{
			m_completeCallback = (Action<AsyncOperation>)Delegate.Remove(m_completeCallback, value);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	private extern void InternalDestroy();

	~AsyncOperation()
	{
		InternalDestroy();
	}

	[RequiredByNativeCode]
	internal void InvokeCompletionEvent()
	{
		if (m_completeCallback != null)
		{
			m_completeCallback(this);
			m_completeCallback = null;
		}
	}
}

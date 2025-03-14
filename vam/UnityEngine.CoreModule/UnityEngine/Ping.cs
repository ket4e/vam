using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

/// <summary>
///   <para>Ping any given IP address (given in dot notation).</para>
/// </summary>
[NativeHeader("Runtime/Export/Ping.bindings.h")]
public sealed class Ping
{
	internal IntPtr m_Ptr;

	/// <summary>
	///   <para>Has the ping function completed?</para>
	/// </summary>
	public bool isDone
	{
		get
		{
			if (m_Ptr == IntPtr.Zero)
			{
				return false;
			}
			return Internal_IsDone();
		}
	}

	/// <summary>
	///   <para>This property contains the ping time result after isDone returns true.</para>
	/// </summary>
	public extern int time
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	/// <summary>
	///   <para>The IP target of the ping.</para>
	/// </summary>
	public extern string ip
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetIP")]
		get;
	}

	/// <summary>
	///   <para>Perform a ping to the supplied target IP address.</para>
	/// </summary>
	/// <param name="address"></param>
	public Ping(string address)
	{
		m_Ptr = Internal_Create(address);
	}

	~Ping()
	{
		DestroyPing();
	}

	[ThreadAndSerializationSafe]
	public void DestroyPing()
	{
		if (!(m_Ptr == IntPtr.Zero))
		{
			Internal_Destroy(m_Ptr);
			m_Ptr = IntPtr.Zero;
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "DestroyPing", IsFreeFunction = true, IsThreadSafe = true)]
	private static extern void Internal_Destroy(IntPtr ptr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("CreatePing")]
	private static extern IntPtr Internal_Create(string address);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("GetIsDone")]
	private extern bool Internal_IsDone();
}

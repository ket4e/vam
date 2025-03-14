using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Scripting;
using UnityEngine.Serialization;

namespace UnityEngine.Events;

/// <summary>
///   <para>Abstract base class for UnityEvents.</para>
/// </summary>
[Serializable]
[UsedByNativeCode]
public abstract class UnityEventBase : ISerializationCallbackReceiver
{
	private InvokableCallList m_Calls;

	[FormerlySerializedAs("m_PersistentListeners")]
	[SerializeField]
	private PersistentCallGroup m_PersistentCalls;

	[SerializeField]
	private string m_TypeName;

	private bool m_CallsDirty = true;

	protected UnityEventBase()
	{
		m_Calls = new InvokableCallList();
		m_PersistentCalls = new PersistentCallGroup();
		m_TypeName = GetType().AssemblyQualifiedName;
	}

	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
	}

	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		DirtyPersistentCalls();
		m_TypeName = GetType().AssemblyQualifiedName;
	}

	protected abstract MethodInfo FindMethod_Impl(string name, object targetObj);

	internal abstract BaseInvokableCall GetDelegate(object target, MethodInfo theFunction);

	internal MethodInfo FindMethod(PersistentCall call)
	{
		Type argumentType = typeof(Object);
		if (!string.IsNullOrEmpty(call.arguments.unityObjectArgumentAssemblyTypeName))
		{
			argumentType = Type.GetType(call.arguments.unityObjectArgumentAssemblyTypeName, throwOnError: false) ?? typeof(Object);
		}
		return FindMethod(call.methodName, call.target, call.mode, argumentType);
	}

	internal MethodInfo FindMethod(string name, object listener, PersistentListenerMode mode, Type argumentType)
	{
		return mode switch
		{
			PersistentListenerMode.EventDefined => FindMethod_Impl(name, listener), 
			PersistentListenerMode.Void => GetValidMethodInfo(listener, name, new Type[0]), 
			PersistentListenerMode.Float => GetValidMethodInfo(listener, name, new Type[1] { typeof(float) }), 
			PersistentListenerMode.Int => GetValidMethodInfo(listener, name, new Type[1] { typeof(int) }), 
			PersistentListenerMode.Bool => GetValidMethodInfo(listener, name, new Type[1] { typeof(bool) }), 
			PersistentListenerMode.String => GetValidMethodInfo(listener, name, new Type[1] { typeof(string) }), 
			PersistentListenerMode.Object => GetValidMethodInfo(listener, name, new Type[1] { argumentType ?? typeof(Object) }), 
			_ => null, 
		};
	}

	/// <summary>
	///   <para>Get the number of registered persistent listeners.</para>
	/// </summary>
	public int GetPersistentEventCount()
	{
		return m_PersistentCalls.Count;
	}

	/// <summary>
	///   <para>Get the target component of the listener at index index.</para>
	/// </summary>
	/// <param name="index">Index of the listener to query.</param>
	public Object GetPersistentTarget(int index)
	{
		return m_PersistentCalls.GetListener(index)?.target;
	}

	/// <summary>
	///   <para>Get the target method name of the listener at index index.</para>
	/// </summary>
	/// <param name="index">Index of the listener to query.</param>
	public string GetPersistentMethodName(int index)
	{
		PersistentCall listener = m_PersistentCalls.GetListener(index);
		return (listener == null) ? string.Empty : listener.methodName;
	}

	private void DirtyPersistentCalls()
	{
		m_Calls.ClearPersistent();
		m_CallsDirty = true;
	}

	private void RebuildPersistentCallsIfNeeded()
	{
		if (m_CallsDirty)
		{
			m_PersistentCalls.Initialize(m_Calls, this);
			m_CallsDirty = false;
		}
	}

	/// <summary>
	///   <para>Modify the execution state of a persistent listener.</para>
	/// </summary>
	/// <param name="index">Index of the listener to query.</param>
	/// <param name="state">State to set.</param>
	public void SetPersistentListenerState(int index, UnityEventCallState state)
	{
		PersistentCall listener = m_PersistentCalls.GetListener(index);
		if (listener != null)
		{
			listener.callState = state;
		}
		DirtyPersistentCalls();
	}

	protected void AddListener(object targetObj, MethodInfo method)
	{
		m_Calls.AddListener(GetDelegate(targetObj, method));
	}

	internal void AddCall(BaseInvokableCall call)
	{
		m_Calls.AddListener(call);
	}

	protected void RemoveListener(object targetObj, MethodInfo method)
	{
		m_Calls.RemoveListener(targetObj, method);
	}

	/// <summary>
	///   <para>Remove all non-persisent (ie created from script) listeners  from the event.</para>
	/// </summary>
	public void RemoveAllListeners()
	{
		m_Calls.Clear();
	}

	internal List<BaseInvokableCall> PrepareInvoke()
	{
		RebuildPersistentCallsIfNeeded();
		return m_Calls.PrepareInvoke();
	}

	protected void Invoke(object[] parameters)
	{
		List<BaseInvokableCall> list = PrepareInvoke();
		for (int i = 0; i < list.Count; i++)
		{
			list[i].Invoke(parameters);
		}
	}

	public override string ToString()
	{
		return base.ToString() + " " + GetType().FullName;
	}

	/// <summary>
	///   <para>Given an object, function name, and a list of argument types; find the method that matches.</para>
	/// </summary>
	/// <param name="obj">Object to search for the method.</param>
	/// <param name="functionName">Function name to search for.</param>
	/// <param name="argumentTypes">Argument types for the function.</param>
	public static MethodInfo GetValidMethodInfo(object obj, string functionName, Type[] argumentTypes)
	{
		Type type = obj.GetType();
		while (type != typeof(object) && type != null)
		{
			MethodInfo method = type.GetMethod(functionName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, argumentTypes, null);
			if (method != null)
			{
				ParameterInfo[] parameters = method.GetParameters();
				bool flag = true;
				int num = 0;
				ParameterInfo[] array = parameters;
				foreach (ParameterInfo parameterInfo in array)
				{
					Type type2 = argumentTypes[num];
					Type parameterType = parameterInfo.ParameterType;
					flag = type2.IsPrimitive == parameterType.IsPrimitive;
					if (!flag)
					{
						break;
					}
					num++;
				}
				if (flag)
				{
					return method;
				}
			}
			type = type.BaseType;
		}
		return null;
	}
}

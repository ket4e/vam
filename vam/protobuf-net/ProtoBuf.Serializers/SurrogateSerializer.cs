using System;
using System.Reflection;
using ProtoBuf.Compiler;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers;

internal sealed class SurrogateSerializer : IProtoTypeSerializer, IProtoSerializer
{
	private readonly Type forType;

	private readonly Type declaredType;

	private readonly MethodInfo toTail;

	private readonly MethodInfo fromTail;

	private IProtoTypeSerializer rootTail;

	public bool ReturnsValue => false;

	public bool RequiresOldValue => true;

	public Type ExpectedType => forType;

	bool IProtoTypeSerializer.HasCallbacks(TypeModel.CallbackType callbackType)
	{
		return false;
	}

	void IProtoTypeSerializer.EmitCallback(CompilerContext ctx, Local valueFrom, TypeModel.CallbackType callbackType)
	{
	}

	void IProtoTypeSerializer.EmitCreateInstance(CompilerContext ctx)
	{
		throw new NotSupportedException();
	}

	bool IProtoTypeSerializer.CanCreateInstance()
	{
		return false;
	}

	object IProtoTypeSerializer.CreateInstance(ProtoReader source)
	{
		throw new NotSupportedException();
	}

	void IProtoTypeSerializer.Callback(object value, TypeModel.CallbackType callbackType, SerializationContext context)
	{
	}

	public SurrogateSerializer(Type forType, Type declaredType, IProtoTypeSerializer rootTail)
	{
		this.forType = forType;
		this.declaredType = declaredType;
		this.rootTail = rootTail;
		toTail = GetConversion(toTail: true);
		fromTail = GetConversion(toTail: false);
	}

	private static bool HasCast(Type type, Type from, Type to, out MethodInfo op)
	{
		MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (MethodInfo methodInfo in methods)
		{
			if ((!(methodInfo.Name != "op_Implicit") || !(methodInfo.Name != "op_Explicit")) && methodInfo.ReturnType == to)
			{
				ParameterInfo[] parameters = methodInfo.GetParameters();
				if (parameters.Length == 1 && parameters[0].ParameterType == from)
				{
					op = methodInfo;
					return true;
				}
			}
		}
		op = null;
		return false;
	}

	public MethodInfo GetConversion(bool toTail)
	{
		Type to = (toTail ? declaredType : forType);
		Type from = (toTail ? forType : declaredType);
		if (HasCast(declaredType, from, to, out var op) || HasCast(forType, from, to, out op))
		{
			return op;
		}
		throw new InvalidOperationException("No suitable conversion operator found for surrogate: " + forType.FullName + " / " + declaredType.FullName);
	}

	public void Write(object value, ProtoWriter writer)
	{
		rootTail.Write(toTail.Invoke(null, new object[1] { value }), writer);
	}

	public object Read(object value, ProtoReader source)
	{
		object[] array = new object[1] { value };
		value = toTail.Invoke(null, array);
		array[0] = rootTail.Read(value, source);
		return fromTail.Invoke(null, array);
	}

	void IProtoSerializer.EmitRead(CompilerContext ctx, Local valueFrom)
	{
		using Local local = new Local(ctx, declaredType);
		ctx.LoadValue(valueFrom);
		ctx.EmitCall(toTail);
		ctx.StoreValue(local);
		rootTail.EmitRead(ctx, local);
		ctx.LoadValue(local);
		ctx.EmitCall(fromTail);
		ctx.StoreValue(valueFrom);
	}

	void IProtoSerializer.EmitWrite(CompilerContext ctx, Local valueFrom)
	{
		ctx.LoadValue(valueFrom);
		ctx.EmitCall(toTail);
		rootTail.EmitWrite(ctx, null);
	}
}

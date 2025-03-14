using System;
using ProtoBuf.Compiler;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers;

internal sealed class DateTimeSerializer : IProtoSerializer
{
	private static readonly Type expectedType = typeof(DateTime);

	public Type ExpectedType => expectedType;

	bool IProtoSerializer.RequiresOldValue => false;

	bool IProtoSerializer.ReturnsValue => true;

	public DateTimeSerializer(TypeModel model)
	{
	}

	public object Read(object value, ProtoReader source)
	{
		return BclHelpers.ReadDateTime(source);
	}

	public void Write(object value, ProtoWriter dest)
	{
		BclHelpers.WriteDateTime((DateTime)value, dest);
	}

	void IProtoSerializer.EmitWrite(CompilerContext ctx, Local valueFrom)
	{
		ctx.EmitWrite(ctx.MapType(typeof(BclHelpers)), "WriteDateTime", valueFrom);
	}

	void IProtoSerializer.EmitRead(CompilerContext ctx, Local valueFrom)
	{
		ctx.EmitBasicRead(ctx.MapType(typeof(BclHelpers)), "ReadDateTime", ExpectedType);
	}
}

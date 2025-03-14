using System;
using System.Reflection.Emit;

namespace Mono.CSharp;

public class DecimalConstant : Constant
{
	public readonly decimal Value;

	public override bool IsDefaultValue => Value == 0m;

	public override bool IsNegative => Value < 0m;

	public DecimalConstant(BuiltinTypes types, decimal d, Location loc)
		: this(types.Decimal, d, loc)
	{
	}

	public DecimalConstant(TypeSpec type, decimal d, Location loc)
		: base(loc)
	{
		base.type = type;
		eclass = ExprClass.Value;
		Value = d;
	}

	public override void Emit(EmitContext ec)
	{
		int[] bits = decimal.GetBits(Value);
		int num = (bits[3] >> 16) & 0xFF;
		MethodSpec methodSpec;
		if (num == 0)
		{
			if (Value <= 2147483647m && Value >= -2147483648m)
			{
				methodSpec = ec.Module.PredefinedMembers.DecimalCtorInt.Resolve(loc);
				if (methodSpec != null)
				{
					ec.EmitInt((int)Value);
					ec.Emit(OpCodes.Newobj, methodSpec);
				}
				return;
			}
			if (Value <= 9223372036854775807m && Value >= -9223372036854775808m)
			{
				methodSpec = ec.Module.PredefinedMembers.DecimalCtorLong.Resolve(loc);
				if (methodSpec != null)
				{
					ec.EmitLong((long)Value);
					ec.Emit(OpCodes.Newobj, methodSpec);
				}
				return;
			}
		}
		ec.EmitInt(bits[0]);
		ec.EmitInt(bits[1]);
		ec.EmitInt(bits[2]);
		ec.EmitInt(bits[3] >> 31);
		ec.EmitInt(num);
		methodSpec = ec.Module.PredefinedMembers.DecimalCtor.Resolve(loc);
		if (methodSpec != null)
		{
			ec.Emit(OpCodes.Newobj, methodSpec);
		}
	}

	public override Constant ConvertExplicitly(bool in_checked_context, TypeSpec target_type)
	{
		return target_type.BuiltinType switch
		{
			BuiltinTypeSpec.Type.SByte => new SByteConstant(target_type, (sbyte)Value, loc), 
			BuiltinTypeSpec.Type.Byte => new ByteConstant(target_type, (byte)Value, loc), 
			BuiltinTypeSpec.Type.Short => new ShortConstant(target_type, (short)Value, loc), 
			BuiltinTypeSpec.Type.UShort => new UShortConstant(target_type, (ushort)Value, loc), 
			BuiltinTypeSpec.Type.Int => new IntConstant(target_type, (int)Value, loc), 
			BuiltinTypeSpec.Type.UInt => new UIntConstant(target_type, (uint)Value, loc), 
			BuiltinTypeSpec.Type.Long => new LongConstant(target_type, (long)Value, loc), 
			BuiltinTypeSpec.Type.ULong => new ULongConstant(target_type, (ulong)Value, loc), 
			BuiltinTypeSpec.Type.Char => new CharConstant(target_type, (char)Value, loc), 
			BuiltinTypeSpec.Type.Float => new FloatConstant(target_type, (float)Value, loc), 
			BuiltinTypeSpec.Type.Double => new DoubleConstant(target_type, (double)Value, loc), 
			_ => null, 
		};
	}

	public override object GetValue()
	{
		return Value;
	}

	public override string GetValueAsLiteral()
	{
		return Value + "M";
	}

	public override long GetValueAsLong()
	{
		throw new NotSupportedException();
	}
}

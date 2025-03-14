using System.Reflection.Emit;

namespace Mono.CSharp;

public class ConvCast : TypeCast
{
	public enum Mode : byte
	{
		I1_U1,
		I1_U2,
		I1_U4,
		I1_U8,
		I1_CH,
		U1_I1,
		U1_CH,
		I2_I1,
		I2_U1,
		I2_U2,
		I2_U4,
		I2_U8,
		I2_CH,
		U2_I1,
		U2_U1,
		U2_I2,
		U2_CH,
		I4_I1,
		I4_U1,
		I4_I2,
		I4_U2,
		I4_U4,
		I4_U8,
		I4_CH,
		U4_I1,
		U4_U1,
		U4_I2,
		U4_U2,
		U4_I4,
		U4_CH,
		I8_I1,
		I8_U1,
		I8_I2,
		I8_U2,
		I8_I4,
		I8_U4,
		I8_U8,
		I8_CH,
		I8_I,
		U8_I1,
		U8_U1,
		U8_I2,
		U8_U2,
		U8_I4,
		U8_U4,
		U8_I8,
		U8_CH,
		U8_I,
		CH_I1,
		CH_U1,
		CH_I2,
		R4_I1,
		R4_U1,
		R4_I2,
		R4_U2,
		R4_I4,
		R4_U4,
		R4_I8,
		R4_U8,
		R4_CH,
		R8_I1,
		R8_U1,
		R8_I2,
		R8_U2,
		R8_I4,
		R8_U4,
		R8_I8,
		R8_U8,
		R8_CH,
		R8_R4,
		I_I8
	}

	private Mode mode;

	public ConvCast(Expression child, TypeSpec return_type, Mode m)
		: base(child, return_type)
	{
		mode = m;
	}

	protected override Expression DoResolve(ResolveContext ec)
	{
		return this;
	}

	public override string ToString()
	{
		return $"ConvCast ({mode}, {child})";
	}

	public override void Emit(EmitContext ec)
	{
		base.Emit(ec);
		Emit(ec, mode);
	}

	public static void Emit(EmitContext ec, Mode mode)
	{
		if (ec.HasSet(BuilderContext.Options.CheckedScope))
		{
			switch (mode)
			{
			case Mode.I1_U1:
				ec.Emit(OpCodes.Conv_Ovf_U1);
				break;
			case Mode.I1_U2:
				ec.Emit(OpCodes.Conv_Ovf_U2);
				break;
			case Mode.I1_U4:
				ec.Emit(OpCodes.Conv_Ovf_U4);
				break;
			case Mode.I1_U8:
				ec.Emit(OpCodes.Conv_Ovf_U8);
				break;
			case Mode.I1_CH:
				ec.Emit(OpCodes.Conv_Ovf_U2);
				break;
			case Mode.U1_I1:
				ec.Emit(OpCodes.Conv_Ovf_I1_Un);
				break;
			case Mode.I2_I1:
				ec.Emit(OpCodes.Conv_Ovf_I1);
				break;
			case Mode.I2_U1:
				ec.Emit(OpCodes.Conv_Ovf_U1);
				break;
			case Mode.I2_U2:
				ec.Emit(OpCodes.Conv_Ovf_U2);
				break;
			case Mode.I2_U4:
				ec.Emit(OpCodes.Conv_Ovf_U4);
				break;
			case Mode.I2_U8:
				ec.Emit(OpCodes.Conv_Ovf_U8);
				break;
			case Mode.I2_CH:
				ec.Emit(OpCodes.Conv_Ovf_U2);
				break;
			case Mode.U2_I1:
				ec.Emit(OpCodes.Conv_Ovf_I1_Un);
				break;
			case Mode.U2_U1:
				ec.Emit(OpCodes.Conv_Ovf_U1_Un);
				break;
			case Mode.U2_I2:
				ec.Emit(OpCodes.Conv_Ovf_I2_Un);
				break;
			case Mode.I4_I1:
				ec.Emit(OpCodes.Conv_Ovf_I1);
				break;
			case Mode.I4_U1:
				ec.Emit(OpCodes.Conv_Ovf_U1);
				break;
			case Mode.I4_I2:
				ec.Emit(OpCodes.Conv_Ovf_I2);
				break;
			case Mode.I4_U4:
				ec.Emit(OpCodes.Conv_Ovf_U4);
				break;
			case Mode.I4_U2:
				ec.Emit(OpCodes.Conv_Ovf_U2);
				break;
			case Mode.I4_U8:
				ec.Emit(OpCodes.Conv_Ovf_U8);
				break;
			case Mode.I4_CH:
				ec.Emit(OpCodes.Conv_Ovf_U2);
				break;
			case Mode.U4_I1:
				ec.Emit(OpCodes.Conv_Ovf_I1_Un);
				break;
			case Mode.U4_U1:
				ec.Emit(OpCodes.Conv_Ovf_U1_Un);
				break;
			case Mode.U4_I2:
				ec.Emit(OpCodes.Conv_Ovf_I2_Un);
				break;
			case Mode.U4_U2:
				ec.Emit(OpCodes.Conv_Ovf_U2_Un);
				break;
			case Mode.U4_I4:
				ec.Emit(OpCodes.Conv_Ovf_I4_Un);
				break;
			case Mode.U4_CH:
				ec.Emit(OpCodes.Conv_Ovf_U2_Un);
				break;
			case Mode.I8_I1:
				ec.Emit(OpCodes.Conv_Ovf_I1);
				break;
			case Mode.I8_U1:
				ec.Emit(OpCodes.Conv_Ovf_U1);
				break;
			case Mode.I8_I2:
				ec.Emit(OpCodes.Conv_Ovf_I2);
				break;
			case Mode.I8_U2:
				ec.Emit(OpCodes.Conv_Ovf_U2);
				break;
			case Mode.I8_I4:
				ec.Emit(OpCodes.Conv_Ovf_I4);
				break;
			case Mode.I8_U4:
				ec.Emit(OpCodes.Conv_Ovf_U4);
				break;
			case Mode.I8_U8:
				ec.Emit(OpCodes.Conv_Ovf_U8);
				break;
			case Mode.I8_CH:
				ec.Emit(OpCodes.Conv_Ovf_U2);
				break;
			case Mode.I8_I:
				ec.Emit(OpCodes.Conv_Ovf_U);
				break;
			case Mode.U8_I1:
				ec.Emit(OpCodes.Conv_Ovf_I1_Un);
				break;
			case Mode.U8_U1:
				ec.Emit(OpCodes.Conv_Ovf_U1_Un);
				break;
			case Mode.U8_I2:
				ec.Emit(OpCodes.Conv_Ovf_I2_Un);
				break;
			case Mode.U8_U2:
				ec.Emit(OpCodes.Conv_Ovf_U2_Un);
				break;
			case Mode.U8_I4:
				ec.Emit(OpCodes.Conv_Ovf_I4_Un);
				break;
			case Mode.U8_U4:
				ec.Emit(OpCodes.Conv_Ovf_U4_Un);
				break;
			case Mode.U8_I8:
				ec.Emit(OpCodes.Conv_Ovf_I8_Un);
				break;
			case Mode.U8_CH:
				ec.Emit(OpCodes.Conv_Ovf_U2_Un);
				break;
			case Mode.U8_I:
				ec.Emit(OpCodes.Conv_Ovf_U_Un);
				break;
			case Mode.CH_I1:
				ec.Emit(OpCodes.Conv_Ovf_I1_Un);
				break;
			case Mode.CH_U1:
				ec.Emit(OpCodes.Conv_Ovf_U1_Un);
				break;
			case Mode.CH_I2:
				ec.Emit(OpCodes.Conv_Ovf_I2_Un);
				break;
			case Mode.R4_I1:
				ec.Emit(OpCodes.Conv_Ovf_I1);
				break;
			case Mode.R4_U1:
				ec.Emit(OpCodes.Conv_Ovf_U1);
				break;
			case Mode.R4_I2:
				ec.Emit(OpCodes.Conv_Ovf_I2);
				break;
			case Mode.R4_U2:
				ec.Emit(OpCodes.Conv_Ovf_U2);
				break;
			case Mode.R4_I4:
				ec.Emit(OpCodes.Conv_Ovf_I4);
				break;
			case Mode.R4_U4:
				ec.Emit(OpCodes.Conv_Ovf_U4);
				break;
			case Mode.R4_I8:
				ec.Emit(OpCodes.Conv_Ovf_I8);
				break;
			case Mode.R4_U8:
				ec.Emit(OpCodes.Conv_Ovf_U8);
				break;
			case Mode.R4_CH:
				ec.Emit(OpCodes.Conv_Ovf_U2);
				break;
			case Mode.R8_I1:
				ec.Emit(OpCodes.Conv_Ovf_I1);
				break;
			case Mode.R8_U1:
				ec.Emit(OpCodes.Conv_Ovf_U1);
				break;
			case Mode.R8_I2:
				ec.Emit(OpCodes.Conv_Ovf_I2);
				break;
			case Mode.R8_U2:
				ec.Emit(OpCodes.Conv_Ovf_U2);
				break;
			case Mode.R8_I4:
				ec.Emit(OpCodes.Conv_Ovf_I4);
				break;
			case Mode.R8_U4:
				ec.Emit(OpCodes.Conv_Ovf_U4);
				break;
			case Mode.R8_I8:
				ec.Emit(OpCodes.Conv_Ovf_I8);
				break;
			case Mode.R8_U8:
				ec.Emit(OpCodes.Conv_Ovf_U8);
				break;
			case Mode.R8_CH:
				ec.Emit(OpCodes.Conv_Ovf_U2);
				break;
			case Mode.R8_R4:
				ec.Emit(OpCodes.Conv_R4);
				break;
			case Mode.I_I8:
				ec.Emit(OpCodes.Conv_Ovf_I8_Un);
				break;
			case Mode.U1_CH:
			case Mode.U2_CH:
				break;
			}
		}
		else
		{
			switch (mode)
			{
			case Mode.I1_U1:
				ec.Emit(OpCodes.Conv_U1);
				break;
			case Mode.I1_U2:
				ec.Emit(OpCodes.Conv_U2);
				break;
			case Mode.I1_U4:
				ec.Emit(OpCodes.Conv_U4);
				break;
			case Mode.I1_U8:
				ec.Emit(OpCodes.Conv_I8);
				break;
			case Mode.I1_CH:
				ec.Emit(OpCodes.Conv_U2);
				break;
			case Mode.U1_I1:
				ec.Emit(OpCodes.Conv_I1);
				break;
			case Mode.U1_CH:
				ec.Emit(OpCodes.Conv_U2);
				break;
			case Mode.I2_I1:
				ec.Emit(OpCodes.Conv_I1);
				break;
			case Mode.I2_U1:
				ec.Emit(OpCodes.Conv_U1);
				break;
			case Mode.I2_U2:
				ec.Emit(OpCodes.Conv_U2);
				break;
			case Mode.I2_U4:
				ec.Emit(OpCodes.Conv_U4);
				break;
			case Mode.I2_U8:
				ec.Emit(OpCodes.Conv_I8);
				break;
			case Mode.I2_CH:
				ec.Emit(OpCodes.Conv_U2);
				break;
			case Mode.U2_I1:
				ec.Emit(OpCodes.Conv_I1);
				break;
			case Mode.U2_U1:
				ec.Emit(OpCodes.Conv_U1);
				break;
			case Mode.U2_I2:
				ec.Emit(OpCodes.Conv_I2);
				break;
			case Mode.I4_I1:
				ec.Emit(OpCodes.Conv_I1);
				break;
			case Mode.I4_U1:
				ec.Emit(OpCodes.Conv_U1);
				break;
			case Mode.I4_I2:
				ec.Emit(OpCodes.Conv_I2);
				break;
			case Mode.I4_U2:
				ec.Emit(OpCodes.Conv_U2);
				break;
			case Mode.I4_U8:
				ec.Emit(OpCodes.Conv_I8);
				break;
			case Mode.I4_CH:
				ec.Emit(OpCodes.Conv_U2);
				break;
			case Mode.U4_I1:
				ec.Emit(OpCodes.Conv_I1);
				break;
			case Mode.U4_U1:
				ec.Emit(OpCodes.Conv_U1);
				break;
			case Mode.U4_I2:
				ec.Emit(OpCodes.Conv_I2);
				break;
			case Mode.U4_U2:
				ec.Emit(OpCodes.Conv_U2);
				break;
			case Mode.U4_CH:
				ec.Emit(OpCodes.Conv_U2);
				break;
			case Mode.I8_I1:
				ec.Emit(OpCodes.Conv_I1);
				break;
			case Mode.I8_U1:
				ec.Emit(OpCodes.Conv_U1);
				break;
			case Mode.I8_I2:
				ec.Emit(OpCodes.Conv_I2);
				break;
			case Mode.I8_U2:
				ec.Emit(OpCodes.Conv_U2);
				break;
			case Mode.I8_I4:
				ec.Emit(OpCodes.Conv_I4);
				break;
			case Mode.I8_U4:
				ec.Emit(OpCodes.Conv_U4);
				break;
			case Mode.I8_CH:
				ec.Emit(OpCodes.Conv_U2);
				break;
			case Mode.I8_I:
				ec.Emit(OpCodes.Conv_U);
				break;
			case Mode.U8_I1:
				ec.Emit(OpCodes.Conv_I1);
				break;
			case Mode.U8_U1:
				ec.Emit(OpCodes.Conv_U1);
				break;
			case Mode.U8_I2:
				ec.Emit(OpCodes.Conv_I2);
				break;
			case Mode.U8_U2:
				ec.Emit(OpCodes.Conv_U2);
				break;
			case Mode.U8_I4:
				ec.Emit(OpCodes.Conv_I4);
				break;
			case Mode.U8_U4:
				ec.Emit(OpCodes.Conv_U4);
				break;
			case Mode.U8_CH:
				ec.Emit(OpCodes.Conv_U2);
				break;
			case Mode.U8_I:
				ec.Emit(OpCodes.Conv_U);
				break;
			case Mode.CH_I1:
				ec.Emit(OpCodes.Conv_I1);
				break;
			case Mode.CH_U1:
				ec.Emit(OpCodes.Conv_U1);
				break;
			case Mode.CH_I2:
				ec.Emit(OpCodes.Conv_I2);
				break;
			case Mode.R4_I1:
				ec.Emit(OpCodes.Conv_I1);
				break;
			case Mode.R4_U1:
				ec.Emit(OpCodes.Conv_U1);
				break;
			case Mode.R4_I2:
				ec.Emit(OpCodes.Conv_I2);
				break;
			case Mode.R4_U2:
				ec.Emit(OpCodes.Conv_U2);
				break;
			case Mode.R4_I4:
				ec.Emit(OpCodes.Conv_I4);
				break;
			case Mode.R4_U4:
				ec.Emit(OpCodes.Conv_U4);
				break;
			case Mode.R4_I8:
				ec.Emit(OpCodes.Conv_I8);
				break;
			case Mode.R4_U8:
				ec.Emit(OpCodes.Conv_U8);
				break;
			case Mode.R4_CH:
				ec.Emit(OpCodes.Conv_U2);
				break;
			case Mode.R8_I1:
				ec.Emit(OpCodes.Conv_I1);
				break;
			case Mode.R8_U1:
				ec.Emit(OpCodes.Conv_U1);
				break;
			case Mode.R8_I2:
				ec.Emit(OpCodes.Conv_I2);
				break;
			case Mode.R8_U2:
				ec.Emit(OpCodes.Conv_U2);
				break;
			case Mode.R8_I4:
				ec.Emit(OpCodes.Conv_I4);
				break;
			case Mode.R8_U4:
				ec.Emit(OpCodes.Conv_U4);
				break;
			case Mode.R8_I8:
				ec.Emit(OpCodes.Conv_I8);
				break;
			case Mode.R8_U8:
				ec.Emit(OpCodes.Conv_U8);
				break;
			case Mode.R8_CH:
				ec.Emit(OpCodes.Conv_U2);
				break;
			case Mode.R8_R4:
				ec.Emit(OpCodes.Conv_R4);
				break;
			case Mode.I_I8:
				ec.Emit(OpCodes.Conv_U8);
				break;
			case Mode.U2_CH:
			case Mode.I4_U4:
			case Mode.U4_I4:
			case Mode.I8_U8:
			case Mode.U8_I8:
				break;
			}
		}
	}
}

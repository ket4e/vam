using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace System.Text.RegularExpressions;

internal class CILCompiler : System.Text.RegularExpressions.RxCompiler, System.Text.RegularExpressions.ICompiler
{
	private class Frame
	{
		public Label label_pass;

		public Label label_fail;

		public Frame(ILGenerator ilgen)
		{
			label_fail = ilgen.DefineLabel();
			label_pass = ilgen.DefineLabel();
		}
	}

	private DynamicMethod[] eval_methods;

	private bool[] eval_methods_defined;

	private Dictionary<int, int> generic_ops;

	private Dictionary<int, int> op_flags;

	private Dictionary<int, Label> labels;

	private static FieldInfo fi_str = typeof(System.Text.RegularExpressions.RxInterpreter).GetField("str", BindingFlags.Instance | BindingFlags.NonPublic);

	private static FieldInfo fi_string_start = typeof(System.Text.RegularExpressions.RxInterpreter).GetField("string_start", BindingFlags.Instance | BindingFlags.NonPublic);

	private static FieldInfo fi_string_end = typeof(System.Text.RegularExpressions.RxInterpreter).GetField("string_end", BindingFlags.Instance | BindingFlags.NonPublic);

	private static FieldInfo fi_program = typeof(System.Text.RegularExpressions.RxInterpreter).GetField("program", BindingFlags.Instance | BindingFlags.NonPublic);

	private static FieldInfo fi_marks = typeof(System.Text.RegularExpressions.RxInterpreter).GetField("marks", BindingFlags.Instance | BindingFlags.NonPublic);

	private static FieldInfo fi_groups = typeof(System.Text.RegularExpressions.RxInterpreter).GetField("groups", BindingFlags.Instance | BindingFlags.NonPublic);

	private static FieldInfo fi_deep = typeof(System.Text.RegularExpressions.RxInterpreter).GetField("deep", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

	private static FieldInfo fi_stack = typeof(System.Text.RegularExpressions.RxInterpreter).GetField("stack", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

	private static FieldInfo fi_mark_start = typeof(System.Text.RegularExpressions.Mark).GetField("Start", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

	private static FieldInfo fi_mark_end = typeof(System.Text.RegularExpressions.Mark).GetField("End", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

	private static FieldInfo fi_mark_index = typeof(System.Text.RegularExpressions.Mark).GetField("Index", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

	private static MethodInfo mi_stack_get_count;

	private static MethodInfo mi_stack_set_count;

	private static MethodInfo mi_stack_push;

	private static MethodInfo mi_stack_pop;

	private static MethodInfo mi_set_start_of_match;

	private static MethodInfo mi_is_word_char;

	private static MethodInfo mi_reset_groups;

	private static MethodInfo mi_checkpoint;

	private static MethodInfo mi_backtrack;

	private static MethodInfo mi_open;

	private static MethodInfo mi_close;

	private static MethodInfo mi_get_last_defined;

	private static MethodInfo mi_mark_get_index;

	private static MethodInfo mi_mark_get_length;

	public static readonly bool trace_compile = Environment.GetEnvironmentVariable("MONO_TRACE_RX_COMPILE") != null;

	private LocalBuilder local_textinfo;

	public CILCompiler()
	{
		generic_ops = new Dictionary<int, int>();
		op_flags = new Dictionary<int, int>();
	}

	System.Text.RegularExpressions.IMachineFactory System.Text.RegularExpressions.ICompiler.GetMachineFactory()
	{
		byte[] array = new byte[curpos];
		Buffer.BlockCopy(program, 0, array, 0, curpos);
		eval_methods = new DynamicMethod[array.Length];
		eval_methods_defined = new bool[array.Length];
		DynamicMethod evalMethod = GetEvalMethod(array, 11);
		if (evalMethod != null)
		{
			return new System.Text.RegularExpressions.RxInterpreterFactory(array, (System.Text.RegularExpressions.EvalDelegate)evalMethod.CreateDelegate(typeof(System.Text.RegularExpressions.EvalDelegate)));
		}
		return new System.Text.RegularExpressions.RxInterpreterFactory(array, null);
	}

	private DynamicMethod GetEvalMethod(byte[] program, int pc)
	{
		if (eval_methods_defined[pc])
		{
			return eval_methods[pc];
		}
		eval_methods_defined[pc] = true;
		eval_methods[pc] = CreateEvalMethod(program, pc);
		return eval_methods[pc];
	}

	private MethodInfo GetMethod(Type t, string name, ref MethodInfo cached)
	{
		if (cached == null)
		{
			cached = t.GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (cached == null)
			{
				throw new Exception("Method not found: " + name);
			}
		}
		return cached;
	}

	private MethodInfo GetMethod(string name, ref MethodInfo cached)
	{
		return GetMethod(typeof(System.Text.RegularExpressions.RxInterpreter), name, ref cached);
	}

	private int ReadInt(byte[] code, int pc)
	{
		int num = code[pc];
		num |= code[pc + 1] << 8;
		num |= code[pc + 2] << 16;
		return num | (code[pc + 3] << 24);
	}

	private static System.Text.RegularExpressions.OpFlags MakeFlags(bool negate, bool ignore, bool reverse, bool lazy)
	{
		System.Text.RegularExpressions.OpFlags opFlags = System.Text.RegularExpressions.OpFlags.None;
		if (negate)
		{
			opFlags |= System.Text.RegularExpressions.OpFlags.Negate;
		}
		if (ignore)
		{
			opFlags |= System.Text.RegularExpressions.OpFlags.IgnoreCase;
		}
		if (reverse)
		{
			opFlags |= System.Text.RegularExpressions.OpFlags.RightToLeft;
		}
		if (lazy)
		{
			opFlags |= System.Text.RegularExpressions.OpFlags.Lazy;
		}
		return opFlags;
	}

	private void EmitGenericOp(System.Text.RegularExpressions.RxOp op, bool negate, bool ignore, bool reverse, bool lazy)
	{
		generic_ops[curpos] = (int)op;
		op_flags[curpos] = (int)MakeFlags(negate, ignore, reverse, lazy: false);
	}

	public override void EmitOp(System.Text.RegularExpressions.RxOp op, bool negate, bool ignore, bool reverse)
	{
		EmitGenericOp(op, negate, ignore, reverse, lazy: false);
		base.EmitOp(op, negate, ignore, reverse);
	}

	public override void EmitOpIgnoreReverse(System.Text.RegularExpressions.RxOp op, bool ignore, bool reverse)
	{
		EmitGenericOp(op, negate: false, ignore, reverse, lazy: false);
		base.EmitOpIgnoreReverse(op, ignore, reverse);
	}

	public override void EmitOpNegateReverse(System.Text.RegularExpressions.RxOp op, bool negate, bool reverse)
	{
		EmitGenericOp(op, negate, ignore: false, reverse, lazy: false);
		base.EmitOpNegateReverse(op, negate, reverse);
	}

	private DynamicMethod CreateEvalMethod(byte[] program, int pc)
	{
		DynamicMethod dynamicMethod = new DynamicMethod("Eval_" + pc, typeof(bool), new Type[3]
		{
			typeof(System.Text.RegularExpressions.RxInterpreter),
			typeof(int),
			typeof(int).MakeByRefType()
		}, typeof(System.Text.RegularExpressions.RxInterpreter), skipVisibility: true);
		ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
		Frame frame = new Frame(iLGenerator);
		local_textinfo = iLGenerator.DeclareLocal(typeof(TextInfo));
		iLGenerator.Emit(OpCodes.Call, typeof(Thread).GetMethod("get_CurrentThread"));
		iLGenerator.Emit(OpCodes.Call, typeof(Thread).GetMethod("get_CurrentCulture"));
		iLGenerator.Emit(OpCodes.Call, typeof(CultureInfo).GetMethod("get_TextInfo"));
		iLGenerator.Emit(OpCodes.Stloc, local_textinfo);
		dynamicMethod = EmitEvalMethodBody(dynamicMethod, iLGenerator, frame, program, pc, program.Length, one_op: false, no_bump: false, out pc);
		if (dynamicMethod == null)
		{
			return null;
		}
		iLGenerator.MarkLabel(frame.label_pass);
		iLGenerator.Emit(OpCodes.Ldarg_2);
		iLGenerator.Emit(OpCodes.Ldarg_1);
		iLGenerator.Emit(OpCodes.Stind_I4);
		iLGenerator.Emit(OpCodes.Ldc_I4_1);
		iLGenerator.Emit(OpCodes.Ret);
		iLGenerator.MarkLabel(frame.label_fail);
		iLGenerator.Emit(OpCodes.Ldc_I4_0);
		iLGenerator.Emit(OpCodes.Ret);
		return dynamicMethod;
	}

	private int ReadShort(byte[] program, int pc)
	{
		return program[pc] | (program[pc + 1] << 8);
	}

	private Label CreateLabelForPC(ILGenerator ilgen, int pc)
	{
		if (labels == null)
		{
			labels = new Dictionary<int, Label>();
		}
		if (!labels.TryGetValue(pc, out var value))
		{
			value = ilgen.DefineLabel();
			labels[pc] = value;
		}
		return value;
	}

	private int GetILOffset(ILGenerator ilgen)
	{
		return (int)typeof(ILGenerator).GetField("code_len", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ilgen);
	}

	private DynamicMethod EmitEvalMethodBody(DynamicMethod m, ILGenerator ilgen, Frame frame, byte[] program, int pc, int end_pc, bool one_op, bool no_bump, out int out_pc)
	{
		out_pc = 0;
		int num = 1 + ReadShort(program, 1);
		while (pc < end_pc)
		{
			System.Text.RegularExpressions.RxOp rxOp = (System.Text.RegularExpressions.RxOp)program[pc];
			if (generic_ops.ContainsKey(pc))
			{
				rxOp = (System.Text.RegularExpressions.RxOp)generic_ops[pc];
			}
			if (trace_compile)
			{
				Console.WriteLine("compiling {0} pc={1} end_pc={2}, il_offset=0x{3:x}", rxOp, pc, end_pc, GetILOffset(ilgen));
			}
			if (labels != null && labels.TryGetValue(pc, out var value))
			{
				ilgen.MarkLabel(value);
				labels.Remove(pc);
			}
			if (System.Text.RegularExpressions.RxInterpreter.trace_rx)
			{
				ilgen.Emit(OpCodes.Ldstr, "evaluating: {0} at pc: {1}, strpos: {2}");
				ilgen.Emit(OpCodes.Ldc_I4, (int)rxOp);
				ilgen.Emit(OpCodes.Box, typeof(System.Text.RegularExpressions.RxOp));
				ilgen.Emit(OpCodes.Ldc_I4, pc);
				ilgen.Emit(OpCodes.Box, typeof(int));
				ilgen.Emit(OpCodes.Ldarg_1);
				ilgen.Emit(OpCodes.Box, typeof(int));
				ilgen.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[4]
				{
					typeof(string),
					typeof(object),
					typeof(object),
					typeof(object)
				}));
			}
			switch (rxOp)
			{
			case System.Text.RegularExpressions.RxOp.Anchor:
			case System.Text.RegularExpressions.RxOp.AnchorReverse:
			{
				bool flag19 = program[pc] == 151;
				int num6 = ReadShort(program, pc + 3);
				pc += ReadShort(program, pc + 1);
				System.Text.RegularExpressions.RxOp rxOp2 = (System.Text.RegularExpressions.RxOp)program[pc];
				if (!flag19 && num == 1 && rxOp2 == System.Text.RegularExpressions.RxOp.Char && program[pc + 2] == 2)
				{
					LocalBuilder local16 = ilgen.DeclareLocal(typeof(int));
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldfld, fi_string_end);
					ilgen.Emit(OpCodes.Stloc, local16);
					LocalBuilder local17 = ilgen.DeclareLocal(typeof(string));
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldfld, fi_str);
					ilgen.Emit(OpCodes.Stloc, local17);
					Label label34 = ilgen.DefineLabel();
					Label label35 = ilgen.DefineLabel();
					ilgen.Emit(OpCodes.Br, label35);
					ilgen.MarkLabel(label34);
					Label label36 = ilgen.DefineLabel();
					ilgen.Emit(OpCodes.Ldloc, local17);
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Callvirt, typeof(string).GetMethod("get_Chars"));
					ilgen.Emit(OpCodes.Conv_I4);
					ilgen.Emit(OpCodes.Ldc_I4, (int)program[pc + 1]);
					ilgen.Emit(OpCodes.Beq, label36);
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldc_I4_1);
					ilgen.Emit(OpCodes.Add);
					ilgen.Emit(OpCodes.Starg, 1);
					ilgen.MarkLabel(label35);
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldloc, local16);
					ilgen.Emit(OpCodes.Blt, label34);
					ilgen.Emit(OpCodes.Br, frame.label_fail);
					ilgen.MarkLabel(label36);
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Call, GetMethod(typeof(System.Text.RegularExpressions.RxInterpreter), "SetStartOfMatch", ref mi_set_start_of_match));
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldc_I4_1);
					ilgen.Emit(OpCodes.Add);
					ilgen.Emit(OpCodes.Starg, 1);
					ilgen.Emit(OpCodes.Br, frame.label_pass);
					break;
				}
				LocalBuilder local18 = ilgen.DeclareLocal(typeof(int));
				ilgen.Emit(OpCodes.Ldarg_0);
				ilgen.Emit(OpCodes.Ldfld, fi_string_end);
				ilgen.Emit(OpCodes.Ldc_I4_1);
				ilgen.Emit(OpCodes.Add);
				ilgen.Emit(OpCodes.Stloc, local18);
				Label label37 = ilgen.DefineLabel();
				Label label38 = ilgen.DefineLabel();
				ilgen.Emit(OpCodes.Br, label38);
				ilgen.MarkLabel(label37);
				if (num > 1)
				{
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Call, GetMethod("ResetGroups", ref mi_reset_groups));
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldfld, fi_marks);
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldfld, fi_groups);
					ilgen.Emit(OpCodes.Ldc_I4_0);
					ilgen.Emit(OpCodes.Ldelem_I4);
					ilgen.Emit(OpCodes.Ldelema, typeof(System.Text.RegularExpressions.Mark));
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Stfld, fi_mark_start);
				}
				Frame frame8 = new Frame(ilgen);
				LocalBuilder local19 = ilgen.DeclareLocal(typeof(int));
				ilgen.Emit(OpCodes.Ldarg_1);
				ilgen.Emit(OpCodes.Stloc, local19);
				m = EmitEvalMethodBody(m, ilgen, frame8, program, pc, end_pc, one_op: false, no_bump: false, out out_pc);
				if (m == null)
				{
					return null;
				}
				ilgen.MarkLabel(frame8.label_pass);
				ilgen.Emit(OpCodes.Ldarg_0);
				ilgen.Emit(OpCodes.Ldfld, fi_marks);
				ilgen.Emit(OpCodes.Ldarg_0);
				ilgen.Emit(OpCodes.Ldfld, fi_groups);
				ilgen.Emit(OpCodes.Ldc_I4_0);
				ilgen.Emit(OpCodes.Ldelem_I4);
				ilgen.Emit(OpCodes.Ldelema, typeof(System.Text.RegularExpressions.Mark));
				ilgen.Emit(OpCodes.Ldloc, local19);
				ilgen.Emit(OpCodes.Stfld, fi_mark_start);
				if (num > 1)
				{
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldfld, fi_marks);
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldfld, fi_groups);
					ilgen.Emit(OpCodes.Ldc_I4_0);
					ilgen.Emit(OpCodes.Ldelem_I4);
					ilgen.Emit(OpCodes.Ldelema, typeof(System.Text.RegularExpressions.Mark));
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Stfld, fi_mark_end);
				}
				ilgen.Emit(OpCodes.Br, frame.label_pass);
				ilgen.MarkLabel(frame8.label_fail);
				ilgen.Emit(OpCodes.Ldloc, local19);
				ilgen.Emit(OpCodes.Ldc_I4_1);
				if (flag19)
				{
					ilgen.Emit(OpCodes.Sub);
				}
				else
				{
					ilgen.Emit(OpCodes.Add);
				}
				ilgen.Emit(OpCodes.Starg, 1);
				ilgen.MarkLabel(label38);
				if (flag19)
				{
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldc_I4_0);
					ilgen.Emit(OpCodes.Bge, label37);
				}
				else
				{
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldloc, local18);
					ilgen.Emit(OpCodes.Blt, label37);
				}
				ilgen.Emit(OpCodes.Br, frame.label_fail);
				break;
			}
			case System.Text.RegularExpressions.RxOp.Branch:
			{
				int num11 = pc + ReadShort(program, pc + 1);
				Frame frame9 = new Frame(ilgen);
				LocalBuilder local20 = ilgen.DeclareLocal(typeof(int));
				ilgen.Emit(OpCodes.Ldarg_1);
				ilgen.Emit(OpCodes.Stloc, local20);
				m = EmitEvalMethodBody(m, ilgen, frame9, program, pc + 3, num11, one_op: false, no_bump: false, out out_pc);
				if (m == null)
				{
					return null;
				}
				ilgen.MarkLabel(frame9.label_pass);
				ilgen.Emit(OpCodes.Br, frame.label_pass);
				ilgen.MarkLabel(frame9.label_fail);
				ilgen.Emit(OpCodes.Ldloc, local20);
				ilgen.Emit(OpCodes.Starg, 1);
				pc = num11;
				goto IL_370d;
			}
			case System.Text.RegularExpressions.RxOp.Char:
			case System.Text.RegularExpressions.RxOp.Range:
			case System.Text.RegularExpressions.RxOp.UnicodeChar:
			case System.Text.RegularExpressions.RxOp.UnicodeRange:
			{
				System.Text.RegularExpressions.OpFlags opFlags5 = (System.Text.RegularExpressions.OpFlags)op_flags[pc];
				bool flag14 = (int)(opFlags5 & System.Text.RegularExpressions.OpFlags.Negate) > 0;
				bool flag15 = (int)(opFlags5 & System.Text.RegularExpressions.OpFlags.IgnoreCase) > 0;
				bool flag16 = (int)(opFlags5 & System.Text.RegularExpressions.OpFlags.RightToLeft) > 0;
				Label label24 = ilgen.DefineLabel();
				if (flag16)
				{
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldc_I4_0);
					ilgen.Emit(OpCodes.Ble, label24);
				}
				else
				{
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldfld, fi_string_end);
					ilgen.Emit(OpCodes.Bge, label24);
				}
				if (flag15)
				{
					ilgen.Emit(OpCodes.Ldloc, local_textinfo);
				}
				LocalBuilder local14 = ilgen.DeclareLocal(typeof(char));
				ilgen.Emit(OpCodes.Ldarg_0);
				ilgen.Emit(OpCodes.Ldfld, fi_str);
				ilgen.Emit(OpCodes.Ldarg_1);
				if (flag16)
				{
					ilgen.Emit(OpCodes.Ldc_I4_1);
					ilgen.Emit(OpCodes.Sub);
				}
				ilgen.Emit(OpCodes.Callvirt, typeof(string).GetMethod("get_Chars"));
				if (flag15)
				{
					ilgen.Emit(OpCodes.Callvirt, typeof(TextInfo).GetMethod("ToLower", new Type[1] { typeof(char) }));
				}
				switch (rxOp)
				{
				case System.Text.RegularExpressions.RxOp.Char:
					ilgen.Emit(OpCodes.Conv_I4);
					ilgen.Emit(OpCodes.Ldc_I4, (int)program[pc + 1]);
					ilgen.Emit((!flag14) ? OpCodes.Bne_Un : OpCodes.Beq, label24);
					pc += 2;
					break;
				case System.Text.RegularExpressions.RxOp.UnicodeChar:
					ilgen.Emit(OpCodes.Conv_I4);
					ilgen.Emit(OpCodes.Ldc_I4, ReadShort(program, pc + 1));
					ilgen.Emit((!flag14) ? OpCodes.Bne_Un : OpCodes.Beq, label24);
					pc += 3;
					break;
				case System.Text.RegularExpressions.RxOp.Range:
					ilgen.Emit(OpCodes.Stloc, local14);
					if (flag14)
					{
						Label label26 = ilgen.DefineLabel();
						ilgen.Emit(OpCodes.Ldloc, local14);
						ilgen.Emit(OpCodes.Ldc_I4, (int)program[pc + 1]);
						ilgen.Emit(OpCodes.Blt, label26);
						ilgen.Emit(OpCodes.Ldloc, local14);
						ilgen.Emit(OpCodes.Ldc_I4, (int)program[pc + 2]);
						ilgen.Emit(OpCodes.Bgt, label26);
						ilgen.Emit(OpCodes.Br, label24);
						ilgen.MarkLabel(label26);
					}
					else
					{
						ilgen.Emit(OpCodes.Ldloc, local14);
						ilgen.Emit(OpCodes.Ldc_I4, (int)program[pc + 1]);
						ilgen.Emit(OpCodes.Blt, label24);
						ilgen.Emit(OpCodes.Ldloc, local14);
						ilgen.Emit(OpCodes.Ldc_I4, (int)program[pc + 2]);
						ilgen.Emit(OpCodes.Bgt, label24);
					}
					pc += 3;
					break;
				case System.Text.RegularExpressions.RxOp.UnicodeRange:
					ilgen.Emit(OpCodes.Stloc, local14);
					if (flag14)
					{
						Label label25 = ilgen.DefineLabel();
						ilgen.Emit(OpCodes.Ldloc, local14);
						ilgen.Emit(OpCodes.Ldc_I4, ReadShort(program, pc + 1));
						ilgen.Emit(OpCodes.Blt, label25);
						ilgen.Emit(OpCodes.Ldloc, local14);
						ilgen.Emit(OpCodes.Ldc_I4, ReadShort(program, pc + 3));
						ilgen.Emit(OpCodes.Bgt, label25);
						ilgen.Emit(OpCodes.Br, label24);
						ilgen.MarkLabel(label25);
					}
					else
					{
						ilgen.Emit(OpCodes.Ldloc, local14);
						ilgen.Emit(OpCodes.Ldc_I4, ReadShort(program, pc + 1));
						ilgen.Emit(OpCodes.Blt, label24);
						ilgen.Emit(OpCodes.Ldloc, local14);
						ilgen.Emit(OpCodes.Ldc_I4, ReadShort(program, pc + 3));
						ilgen.Emit(OpCodes.Bgt, label24);
					}
					pc += 5;
					break;
				default:
					throw new NotSupportedException();
				}
				if (!no_bump)
				{
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldc_I4_1);
					if (flag16)
					{
						ilgen.Emit(OpCodes.Sub);
					}
					else
					{
						ilgen.Emit(OpCodes.Add);
					}
					ilgen.Emit(OpCodes.Starg, 1);
				}
				Label label27 = ilgen.DefineLabel();
				ilgen.Emit(OpCodes.Br, label27);
				ilgen.MarkLabel(label24);
				ilgen.Emit(OpCodes.Br, frame.label_fail);
				ilgen.MarkLabel(label27);
				goto IL_370d;
			}
			case System.Text.RegularExpressions.RxOp.True:
				ilgen.Emit(OpCodes.Br, frame.label_pass);
				pc++;
				goto IL_370d;
			case System.Text.RegularExpressions.RxOp.False:
				ilgen.Emit(OpCodes.Br, frame.label_fail);
				pc++;
				goto IL_370d;
			case System.Text.RegularExpressions.RxOp.AnyPosition:
				pc++;
				goto IL_370d;
			case System.Text.RegularExpressions.RxOp.StartOfString:
				ilgen.Emit(OpCodes.Ldarg_1);
				ilgen.Emit(OpCodes.Ldc_I4_0);
				ilgen.Emit(OpCodes.Bgt, frame.label_fail);
				pc++;
				goto IL_370d;
			case System.Text.RegularExpressions.RxOp.StartOfLine:
			{
				Label label6 = ilgen.DefineLabel();
				ilgen.Emit(OpCodes.Ldarg_1);
				ilgen.Emit(OpCodes.Ldc_I4_0);
				ilgen.Emit(OpCodes.Beq, label6);
				ilgen.Emit(OpCodes.Ldarg_0);
				ilgen.Emit(OpCodes.Ldfld, fi_str);
				ilgen.Emit(OpCodes.Ldarg_1);
				ilgen.Emit(OpCodes.Ldc_I4_1);
				ilgen.Emit(OpCodes.Sub);
				ilgen.Emit(OpCodes.Callvirt, typeof(string).GetMethod("get_Chars"));
				ilgen.Emit(OpCodes.Ldc_I4, 10);
				ilgen.Emit(OpCodes.Beq, label6);
				ilgen.Emit(OpCodes.Br, frame.label_fail);
				ilgen.MarkLabel(label6);
				pc++;
				goto IL_370d;
			}
			case System.Text.RegularExpressions.RxOp.StartOfScan:
				ilgen.Emit(OpCodes.Ldarg_1);
				ilgen.Emit(OpCodes.Ldarg_0);
				ilgen.Emit(OpCodes.Ldfld, fi_string_start);
				ilgen.Emit(OpCodes.Bne_Un, frame.label_fail);
				pc++;
				goto IL_370d;
			case System.Text.RegularExpressions.RxOp.End:
			{
				Label label = ilgen.DefineLabel();
				ilgen.Emit(OpCodes.Ldarg_1);
				ilgen.Emit(OpCodes.Ldarg_0);
				ilgen.Emit(OpCodes.Ldfld, fi_string_end);
				ilgen.Emit(OpCodes.Beq, label);
				Label label2 = ilgen.DefineLabel();
				ilgen.Emit(OpCodes.Ldarg_1);
				ilgen.Emit(OpCodes.Ldarg_0);
				ilgen.Emit(OpCodes.Ldfld, fi_string_end);
				ilgen.Emit(OpCodes.Ldc_I4_1);
				ilgen.Emit(OpCodes.Sub);
				ilgen.Emit(OpCodes.Bne_Un, label2);
				ilgen.Emit(OpCodes.Ldarg_0);
				ilgen.Emit(OpCodes.Ldfld, fi_str);
				ilgen.Emit(OpCodes.Ldarg_1);
				ilgen.Emit(OpCodes.Callvirt, typeof(string).GetMethod("get_Chars"));
				ilgen.Emit(OpCodes.Ldc_I4, 10);
				ilgen.Emit(OpCodes.Bne_Un, label2);
				ilgen.Emit(OpCodes.Br, label);
				ilgen.MarkLabel(label2);
				ilgen.Emit(OpCodes.Br, frame.label_fail);
				ilgen.MarkLabel(label);
				pc++;
				goto IL_370d;
			}
			case System.Text.RegularExpressions.RxOp.EndOfString:
				ilgen.Emit(OpCodes.Ldarg_1);
				ilgen.Emit(OpCodes.Ldarg_0);
				ilgen.Emit(OpCodes.Ldfld, fi_string_end);
				ilgen.Emit(OpCodes.Bne_Un, frame.label_fail);
				pc++;
				goto IL_370d;
			case System.Text.RegularExpressions.RxOp.EndOfLine:
			{
				Label label33 = ilgen.DefineLabel();
				ilgen.Emit(OpCodes.Ldarg_1);
				ilgen.Emit(OpCodes.Ldarg_0);
				ilgen.Emit(OpCodes.Ldfld, fi_string_end);
				ilgen.Emit(OpCodes.Beq, label33);
				ilgen.Emit(OpCodes.Ldarg_0);
				ilgen.Emit(OpCodes.Ldfld, fi_str);
				ilgen.Emit(OpCodes.Ldarg_1);
				ilgen.Emit(OpCodes.Callvirt, typeof(string).GetMethod("get_Chars"));
				ilgen.Emit(OpCodes.Ldc_I4, 10);
				ilgen.Emit(OpCodes.Beq, label33);
				ilgen.Emit(OpCodes.Br, frame.label_fail);
				ilgen.MarkLabel(label33);
				pc++;
				goto IL_370d;
			}
			case System.Text.RegularExpressions.RxOp.WordBoundary:
			case System.Text.RegularExpressions.RxOp.NoWordBoundary:
			{
				bool flag10 = rxOp == System.Text.RegularExpressions.RxOp.NoWordBoundary;
				ilgen.Emit(OpCodes.Ldarg_0);
				ilgen.Emit(OpCodes.Ldfld, fi_string_end);
				ilgen.Emit(OpCodes.Ldc_I4_0);
				ilgen.Emit(OpCodes.Beq, frame.label_fail);
				Label label12 = ilgen.DefineLabel();
				Label label13 = ilgen.DefineLabel();
				ilgen.Emit(OpCodes.Ldarg_1);
				ilgen.Emit(OpCodes.Ldc_I4_0);
				ilgen.Emit(OpCodes.Bne_Un, label13);
				ilgen.Emit(OpCodes.Ldarg_0);
				ilgen.Emit(OpCodes.Ldfld, fi_str);
				ilgen.Emit(OpCodes.Ldarg_1);
				ilgen.Emit(OpCodes.Callvirt, typeof(string).GetMethod("get_Chars"));
				ilgen.Emit(OpCodes.Call, GetMethod("IsWordChar", ref mi_is_word_char));
				ilgen.Emit((!flag10) ? OpCodes.Brfalse : OpCodes.Brtrue, frame.label_fail);
				ilgen.Emit(OpCodes.Br, label12);
				ilgen.MarkLabel(label13);
				Label label14 = ilgen.DefineLabel();
				ilgen.Emit(OpCodes.Ldarg_1);
				ilgen.Emit(OpCodes.Ldarg_0);
				ilgen.Emit(OpCodes.Ldfld, fi_string_end);
				ilgen.Emit(OpCodes.Bne_Un, label14);
				ilgen.Emit(OpCodes.Ldarg_0);
				ilgen.Emit(OpCodes.Ldfld, fi_str);
				ilgen.Emit(OpCodes.Ldarg_1);
				ilgen.Emit(OpCodes.Ldc_I4_1);
				ilgen.Emit(OpCodes.Sub);
				ilgen.Emit(OpCodes.Callvirt, typeof(string).GetMethod("get_Chars"));
				ilgen.Emit(OpCodes.Call, GetMethod("IsWordChar", ref mi_is_word_char));
				ilgen.Emit((!flag10) ? OpCodes.Brfalse : OpCodes.Brtrue, frame.label_fail);
				ilgen.Emit(OpCodes.Br, label12);
				ilgen.MarkLabel(label14);
				ilgen.Emit(OpCodes.Ldarg_0);
				ilgen.Emit(OpCodes.Ldfld, fi_str);
				ilgen.Emit(OpCodes.Ldarg_1);
				ilgen.Emit(OpCodes.Callvirt, typeof(string).GetMethod("get_Chars"));
				ilgen.Emit(OpCodes.Call, GetMethod("IsWordChar", ref mi_is_word_char));
				ilgen.Emit(OpCodes.Ldarg_0);
				ilgen.Emit(OpCodes.Ldfld, fi_str);
				ilgen.Emit(OpCodes.Ldarg_1);
				ilgen.Emit(OpCodes.Ldc_I4_1);
				ilgen.Emit(OpCodes.Sub);
				ilgen.Emit(OpCodes.Callvirt, typeof(string).GetMethod("get_Chars"));
				ilgen.Emit(OpCodes.Call, GetMethod("IsWordChar", ref mi_is_word_char));
				ilgen.Emit((!flag10) ? OpCodes.Beq : OpCodes.Bne_Un, frame.label_fail);
				ilgen.Emit(OpCodes.Br, label12);
				ilgen.MarkLabel(label12);
				pc++;
				goto IL_370d;
			}
			case System.Text.RegularExpressions.RxOp.Bitmap:
			case System.Text.RegularExpressions.RxOp.UnicodeBitmap:
			{
				System.Text.RegularExpressions.OpFlags opFlags3 = (System.Text.RegularExpressions.OpFlags)op_flags[pc];
				bool flag6 = (int)(opFlags3 & System.Text.RegularExpressions.OpFlags.Negate) > 0;
				bool flag7 = (int)(opFlags3 & System.Text.RegularExpressions.OpFlags.IgnoreCase) > 0;
				bool flag8 = (int)(opFlags3 & System.Text.RegularExpressions.OpFlags.RightToLeft) > 0;
				bool flag9 = rxOp == System.Text.RegularExpressions.RxOp.UnicodeBitmap;
				Label label9 = ilgen.DefineLabel();
				Label label10 = ilgen.DefineLabel();
				Label label11 = ilgen.DefineLabel();
				if (flag8)
				{
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldc_I4_0);
					ilgen.Emit(OpCodes.Ble, label9);
				}
				else
				{
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldfld, fi_string_end);
					ilgen.Emit(OpCodes.Bge, label9);
				}
				LocalBuilder local8 = ilgen.DeclareLocal(typeof(int));
				if (flag7)
				{
					ilgen.Emit(OpCodes.Ldloc, local_textinfo);
				}
				ilgen.Emit(OpCodes.Ldarg_0);
				ilgen.Emit(OpCodes.Ldfld, fi_str);
				ilgen.Emit(OpCodes.Ldarg_1);
				if (flag8)
				{
					ilgen.Emit(OpCodes.Ldc_I4_1);
					ilgen.Emit(OpCodes.Sub);
				}
				ilgen.Emit(OpCodes.Callvirt, typeof(string).GetMethod("get_Chars"));
				ilgen.Emit(OpCodes.Conv_I4);
				if (flag7)
				{
					ilgen.Emit(OpCodes.Callvirt, typeof(TextInfo).GetMethod("ToLower", new Type[1] { typeof(char) }));
				}
				int num6;
				if (flag9)
				{
					ilgen.Emit(OpCodes.Ldc_I4, ReadShort(program, pc + 1));
					ilgen.Emit(OpCodes.Sub);
					ilgen.Emit(OpCodes.Stloc, local8);
					num6 = ReadShort(program, pc + 3);
					pc += 5;
				}
				else
				{
					ilgen.Emit(OpCodes.Ldc_I4, (int)program[pc + 1]);
					ilgen.Emit(OpCodes.Sub);
					ilgen.Emit(OpCodes.Stloc, local8);
					num6 = program[pc + 2];
					pc += 3;
				}
				ilgen.Emit(OpCodes.Ldloc, local8);
				ilgen.Emit(OpCodes.Ldc_I4_0);
				ilgen.Emit(OpCodes.Blt, (!flag6) ? frame.label_fail : label11);
				ilgen.Emit(OpCodes.Ldloc, local8);
				ilgen.Emit(OpCodes.Ldc_I4, num6 << 3);
				ilgen.Emit(OpCodes.Bge, (!flag6) ? frame.label_fail : label11);
				if (num6 <= 4)
				{
					uint num8 = program[pc];
					if (num6 > 1)
					{
						num8 |= (uint)(program[pc + 1] << 8);
					}
					if (num6 > 2)
					{
						num8 |= (uint)(program[pc + 2] << 16);
					}
					if (num6 > 3)
					{
						num8 |= (uint)(program[pc + 3] << 24);
					}
					ilgen.Emit(OpCodes.Ldc_I4, num8);
					ilgen.Emit(OpCodes.Ldloc, local8);
					ilgen.Emit(OpCodes.Shr_Un);
					ilgen.Emit(OpCodes.Ldc_I4_1);
					ilgen.Emit(OpCodes.And);
					ilgen.Emit((!flag6) ? OpCodes.Brfalse : OpCodes.Brtrue, label9);
				}
				else
				{
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldfld, fi_program);
					ilgen.Emit(OpCodes.Ldloc, local8);
					ilgen.Emit(OpCodes.Ldc_I4_3);
					ilgen.Emit(OpCodes.Shr);
					ilgen.Emit(OpCodes.Ldc_I4, pc);
					ilgen.Emit(OpCodes.Add);
					ilgen.Emit(OpCodes.Ldelem_I1);
					ilgen.Emit(OpCodes.Ldc_I4_1);
					ilgen.Emit(OpCodes.Ldloc, local8);
					ilgen.Emit(OpCodes.Ldc_I4, 7);
					ilgen.Emit(OpCodes.And);
					ilgen.Emit(OpCodes.Shl);
					ilgen.Emit(OpCodes.And);
					ilgen.Emit(OpCodes.Ldc_I4_0);
					ilgen.Emit((!flag6) ? OpCodes.Beq : OpCodes.Bne_Un, label9);
				}
				ilgen.MarkLabel(label11);
				if (!no_bump)
				{
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldc_I4_1);
					if (flag8)
					{
						ilgen.Emit(OpCodes.Sub);
					}
					else
					{
						ilgen.Emit(OpCodes.Add);
					}
					ilgen.Emit(OpCodes.Starg, 1);
				}
				ilgen.Emit(OpCodes.Br, label10);
				ilgen.MarkLabel(label9);
				ilgen.Emit(OpCodes.Br, frame.label_fail);
				ilgen.MarkLabel(label10);
				pc += num6;
				goto IL_370d;
			}
			case System.Text.RegularExpressions.RxOp.String:
			case System.Text.RegularExpressions.RxOp.UnicodeString:
			{
				System.Text.RegularExpressions.OpFlags opFlags2 = (System.Text.RegularExpressions.OpFlags)op_flags[pc];
				bool flag3 = (int)(opFlags2 & System.Text.RegularExpressions.OpFlags.IgnoreCase) > 0;
				bool flag4 = (int)(opFlags2 & System.Text.RegularExpressions.OpFlags.RightToLeft) > 0;
				bool flag5 = rxOp == System.Text.RegularExpressions.RxOp.UnicodeString;
				int num5;
				int num6;
				if (flag5)
				{
					num5 = pc + 3;
					num6 = ReadShort(program, pc + 1);
				}
				else
				{
					num5 = pc + 2;
					num6 = program[pc + 1];
				}
				if (flag4)
				{
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldc_I4, num6);
					ilgen.Emit(OpCodes.Blt, frame.label_fail);
				}
				else
				{
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldc_I4, num6);
					ilgen.Emit(OpCodes.Add);
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldfld, fi_string_end);
					ilgen.Emit(OpCodes.Bgt, frame.label_fail);
				}
				LocalBuilder local7 = ilgen.DeclareLocal(typeof(string));
				ilgen.Emit(OpCodes.Ldarg_0);
				ilgen.Emit(OpCodes.Ldfld, fi_str);
				ilgen.Emit(OpCodes.Stloc, local7);
				if (flag4)
				{
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldc_I4, num6);
					ilgen.Emit(OpCodes.Sub);
					ilgen.Emit(OpCodes.Starg, 1);
				}
				int num7 = num5 + ((!flag5) ? num6 : (num6 * 2));
				while (num5 < num7)
				{
					if (flag3)
					{
						ilgen.Emit(OpCodes.Ldloc, local_textinfo);
					}
					ilgen.Emit(OpCodes.Ldloc, local7);
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Callvirt, typeof(string).GetMethod("get_Chars"));
					if (flag3)
					{
						ilgen.Emit(OpCodes.Callvirt, typeof(TextInfo).GetMethod("ToLower", new Type[1] { typeof(char) }));
					}
					ilgen.Emit(OpCodes.Ldc_I4, (!flag5) ? program[num5] : ReadShort(program, num5));
					ilgen.Emit(OpCodes.Bne_Un, frame.label_fail);
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldc_I4_1);
					ilgen.Emit(OpCodes.Add);
					ilgen.Emit(OpCodes.Starg, 1);
					num5 = ((!flag5) ? (num5 + 1) : (num5 + 2));
				}
				if (flag4)
				{
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldc_I4, num6);
					ilgen.Emit(OpCodes.Sub);
					ilgen.Emit(OpCodes.Starg, 1);
				}
				pc = num7;
				goto IL_370d;
			}
			case System.Text.RegularExpressions.RxOp.OpenGroup:
			{
				int arg2 = ReadShort(program, pc + 1);
				ilgen.Emit(OpCodes.Ldarg_0);
				ilgen.Emit(OpCodes.Ldc_I4, arg2);
				ilgen.Emit(OpCodes.Ldarg_1);
				ilgen.Emit(OpCodes.Call, GetMethod("Open", ref mi_open));
				pc += 3;
				goto IL_370d;
			}
			case System.Text.RegularExpressions.RxOp.CloseGroup:
			{
				int arg = ReadShort(program, pc + 1);
				ilgen.Emit(OpCodes.Ldarg_0);
				ilgen.Emit(OpCodes.Ldc_I4, arg);
				ilgen.Emit(OpCodes.Ldarg_1);
				ilgen.Emit(OpCodes.Call, GetMethod("Close", ref mi_close));
				pc += 3;
				goto IL_370d;
			}
			case System.Text.RegularExpressions.RxOp.Jump:
			{
				int num12 = pc + ReadShort(program, pc + 1);
				if (num12 > end_pc)
				{
					return null;
				}
				if (trace_compile)
				{
					Console.WriteLine("\tjump target: {0}", num12);
				}
				if (labels == null)
				{
					labels = new Dictionary<int, Label>();
				}
				Label label39 = CreateLabelForPC(ilgen, num12);
				ilgen.Emit(OpCodes.Br, label39);
				pc += 3;
				goto IL_370d;
			}
			case System.Text.RegularExpressions.RxOp.Test:
			{
				int num3 = pc + ReadShort(program, pc + 1);
				int num4 = pc + ReadShort(program, pc + 3);
				if (trace_compile)
				{
					Console.WriteLine("\temitting <test_expr>");
				}
				LocalBuilder local6 = ilgen.DeclareLocal(typeof(int));
				ilgen.Emit(OpCodes.Ldarg_1);
				ilgen.Emit(OpCodes.Stloc, local6);
				Frame frame3 = new Frame(ilgen);
				m = EmitEvalMethodBody(m, ilgen, frame3, program, pc + 5, (num3 >= num4) ? num4 : num3, one_op: false, no_bump: false, out pc);
				if (m == null)
				{
					return null;
				}
				if (trace_compile)
				{
					Console.WriteLine("\temitted <test_expr>");
					Console.WriteLine("\ttarget1 = {0}", num3);
					Console.WriteLine("\ttarget2 = {0}", num4);
				}
				Label label7 = CreateLabelForPC(ilgen, num3);
				Label label8 = CreateLabelForPC(ilgen, num4);
				ilgen.MarkLabel(frame3.label_pass);
				ilgen.Emit(OpCodes.Ldloc, local6);
				ilgen.Emit(OpCodes.Starg, 1);
				ilgen.Emit(OpCodes.Br, label7);
				ilgen.MarkLabel(frame3.label_fail);
				ilgen.Emit(OpCodes.Ldloc, local6);
				ilgen.Emit(OpCodes.Starg, 1);
				ilgen.Emit(OpCodes.Br, label8);
				goto IL_370d;
			}
			case System.Text.RegularExpressions.RxOp.SubExpression:
			{
				int num2 = pc + ReadShort(program, pc + 1);
				if (trace_compile)
				{
					Console.WriteLine("\temitting <sub_expr>");
				}
				LocalBuilder local5 = ilgen.DeclareLocal(typeof(int));
				ilgen.Emit(OpCodes.Ldarg_1);
				ilgen.Emit(OpCodes.Stloc, local5);
				Frame frame2 = new Frame(ilgen);
				m = EmitEvalMethodBody(m, ilgen, frame2, program, pc + 3, num2, one_op: false, no_bump: false, out pc);
				if (m == null)
				{
					return null;
				}
				if (trace_compile)
				{
					Console.WriteLine("\temitted <sub_expr>");
					Console.WriteLine("\ttarget = {0}", num2);
				}
				Label label5 = CreateLabelForPC(ilgen, num2);
				ilgen.MarkLabel(frame2.label_pass);
				ilgen.Emit(OpCodes.Br, label5);
				ilgen.MarkLabel(frame2.label_fail);
				ilgen.Emit(OpCodes.Ldloc, local5);
				ilgen.Emit(OpCodes.Starg, 1);
				ilgen.Emit(OpCodes.Br, frame.label_fail);
				goto IL_370d;
			}
			case System.Text.RegularExpressions.RxOp.TestCharGroup:
			{
				int num10 = pc + ReadShort(program, pc + 1);
				pc += 3;
				Label label22 = ilgen.DefineLabel();
				System.Text.RegularExpressions.OpFlags opFlags4 = (System.Text.RegularExpressions.OpFlags)op_flags[pc];
				bool flag12 = (int)(opFlags4 & System.Text.RegularExpressions.OpFlags.Negate) > 0;
				bool flag13 = (int)(opFlags4 & System.Text.RegularExpressions.OpFlags.RightToLeft) > 0;
				while (pc < num10)
				{
					Frame frame7 = new Frame(ilgen);
					m = EmitEvalMethodBody(m, ilgen, frame7, program, pc, int.MaxValue, one_op: true, no_bump: true, out pc);
					if (m == null)
					{
						return null;
					}
					if (!flag12)
					{
						ilgen.MarkLabel(frame7.label_pass);
						ilgen.Emit(OpCodes.Br, label22);
						ilgen.MarkLabel(frame7.label_fail);
						continue;
					}
					ilgen.MarkLabel(frame7.label_pass);
					Label label23 = ilgen.DefineLabel();
					ilgen.Emit(OpCodes.Br, label23);
					ilgen.MarkLabel(frame7.label_fail);
					ilgen.Emit(OpCodes.Br, frame.label_fail);
					ilgen.MarkLabel(label23);
				}
				if (flag12)
				{
					ilgen.Emit(OpCodes.Br, label22);
				}
				else
				{
					ilgen.Emit(OpCodes.Br, frame.label_fail);
				}
				ilgen.MarkLabel(label22);
				ilgen.Emit(OpCodes.Ldarg_1);
				ilgen.Emit(OpCodes.Ldc_I4_1);
				if (flag13)
				{
					ilgen.Emit(OpCodes.Sub);
				}
				else
				{
					ilgen.Emit(OpCodes.Add);
				}
				ilgen.Emit(OpCodes.Starg, 1);
				goto IL_370d;
			}
			case System.Text.RegularExpressions.RxOp.FastRepeat:
			case System.Text.RegularExpressions.RxOp.FastRepeatLazy:
			{
				bool flag11 = program[pc] == 156;
				int num9 = pc + ReadShort(program, pc + 1);
				int num5 = ReadInt(program, pc + 3);
				int num7 = ReadInt(program, pc + 7);
				ilgen.Emit(OpCodes.Ldarg_0);
				ilgen.Emit(OpCodes.Ldnull);
				ilgen.Emit(OpCodes.Stfld, fi_deep);
				LocalBuilder local9 = ilgen.DeclareLocal(typeof(int));
				ilgen.Emit(OpCodes.Ldc_I4_0);
				ilgen.Emit(OpCodes.Stloc, local9);
				LocalBuilder local10 = ilgen.DeclareLocal(typeof(int));
				if (num5 > 0)
				{
					Label label15 = ilgen.DefineLabel();
					ilgen.Emit(OpCodes.Br, label15);
					Label label16 = ilgen.DefineLabel();
					ilgen.MarkLabel(label16);
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Stloc, local10);
					Frame frame4 = new Frame(ilgen);
					m = EmitEvalMethodBody(m, ilgen, frame4, program, pc + 11, num9, one_op: false, no_bump: false, out out_pc);
					if (m == null)
					{
						return null;
					}
					ilgen.MarkLabel(frame4.label_fail);
					ilgen.Emit(OpCodes.Br, frame.label_fail);
					ilgen.MarkLabel(frame4.label_pass);
					ilgen.Emit(OpCodes.Ldloc, local9);
					ilgen.Emit(OpCodes.Ldc_I4_1);
					ilgen.Emit(OpCodes.Add);
					ilgen.Emit(OpCodes.Stloc, local9);
					ilgen.MarkLabel(label15);
					ilgen.Emit(OpCodes.Ldloc, local9);
					ilgen.Emit(OpCodes.Ldc_I4, num5);
					ilgen.Emit(OpCodes.Blt, label16);
				}
				if (flag11)
				{
					Label label17 = ilgen.DefineLabel();
					ilgen.Emit(OpCodes.Br, label17);
					Label label18 = ilgen.DefineLabel();
					ilgen.MarkLabel(label18);
					LocalBuilder local11 = ilgen.DeclareLocal(typeof(int));
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Call, GetMethod("Checkpoint", ref mi_checkpoint));
					ilgen.Emit(OpCodes.Stloc, local11);
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Stloc, local10);
					Frame frame5 = new Frame(ilgen);
					m = EmitEvalMethodBody(m, ilgen, frame5, program, num9, end_pc, one_op: false, no_bump: false, out out_pc);
					if (m == null)
					{
						return null;
					}
					ilgen.MarkLabel(frame5.label_pass);
					ilgen.Emit(OpCodes.Br, frame.label_pass);
					ilgen.MarkLabel(frame5.label_fail);
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldloc, local11);
					ilgen.Emit(OpCodes.Call, GetMethod("Backtrack", ref mi_backtrack));
					ilgen.Emit(OpCodes.Ldloc, local10);
					ilgen.Emit(OpCodes.Starg, 1);
					ilgen.Emit(OpCodes.Ldloc, local9);
					ilgen.Emit(OpCodes.Ldc_I4, num7);
					ilgen.Emit(OpCodes.Bge, frame.label_fail);
					frame5 = new Frame(ilgen);
					m = EmitEvalMethodBody(m, ilgen, frame5, program, pc + 11, num9, one_op: false, no_bump: false, out out_pc);
					if (m == null)
					{
						return null;
					}
					ilgen.MarkLabel(frame5.label_pass);
					ilgen.Emit(OpCodes.Ldloc, local9);
					ilgen.Emit(OpCodes.Ldc_I4_1);
					ilgen.Emit(OpCodes.Add);
					ilgen.Emit(OpCodes.Stloc, local9);
					ilgen.Emit(OpCodes.Br, label18);
					ilgen.MarkLabel(frame5.label_fail);
					ilgen.Emit(OpCodes.Br, frame.label_fail);
					ilgen.MarkLabel(label17);
					ilgen.Emit(OpCodes.Br, label18);
				}
				else
				{
					LocalBuilder local12 = ilgen.DeclareLocal(typeof(int));
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldflda, fi_stack);
					ilgen.Emit(OpCodes.Call, GetMethod(typeof(System.Text.RegularExpressions.RxInterpreter.IntStack), "get_Count", ref mi_stack_get_count));
					ilgen.Emit(OpCodes.Stloc, local12);
					Label label19 = ilgen.DefineLabel();
					ilgen.Emit(OpCodes.Br, label19);
					Label label20 = ilgen.DefineLabel();
					ilgen.MarkLabel(label20);
					LocalBuilder local13 = ilgen.DeclareLocal(typeof(int));
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Call, GetMethod("Checkpoint", ref mi_checkpoint));
					ilgen.Emit(OpCodes.Stloc, local13);
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Stloc, local10);
					Frame frame6 = new Frame(ilgen);
					m = EmitEvalMethodBody(m, ilgen, frame6, program, pc + 11, num9, one_op: false, no_bump: false, out out_pc);
					if (m == null)
					{
						return null;
					}
					ilgen.MarkLabel(frame6.label_fail);
					ilgen.Emit(OpCodes.Ldloc, local10);
					ilgen.Emit(OpCodes.Starg, 1);
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldloc, local13);
					ilgen.Emit(OpCodes.Call, GetMethod("Backtrack", ref mi_backtrack));
					Label label21 = ilgen.DefineLabel();
					ilgen.Emit(OpCodes.Br, label21);
					ilgen.MarkLabel(frame6.label_pass);
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldflda, fi_stack);
					ilgen.Emit(OpCodes.Ldloc, local13);
					ilgen.Emit(OpCodes.Call, GetMethod(typeof(System.Text.RegularExpressions.RxInterpreter.IntStack), "Push", ref mi_stack_push));
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldflda, fi_stack);
					ilgen.Emit(OpCodes.Ldloc, local10);
					ilgen.Emit(OpCodes.Call, GetMethod(typeof(System.Text.RegularExpressions.RxInterpreter.IntStack), "Push", ref mi_stack_push));
					ilgen.Emit(OpCodes.Ldloc, local9);
					ilgen.Emit(OpCodes.Ldc_I4_1);
					ilgen.Emit(OpCodes.Add);
					ilgen.Emit(OpCodes.Stloc, local9);
					ilgen.MarkLabel(label19);
					ilgen.Emit(OpCodes.Ldloc, local9);
					ilgen.Emit(OpCodes.Ldc_I4, num7);
					ilgen.Emit(OpCodes.Blt, label20);
					ilgen.MarkLabel(label21);
					label19 = ilgen.DefineLabel();
					ilgen.Emit(OpCodes.Br, label19);
					label20 = ilgen.DefineLabel();
					ilgen.MarkLabel(label20);
					if (System.Text.RegularExpressions.RxInterpreter.trace_rx)
					{
						ilgen.Emit(OpCodes.Ldstr, "matching tail at: {0}");
						ilgen.Emit(OpCodes.Ldarg_1);
						ilgen.Emit(OpCodes.Box, typeof(int));
						ilgen.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[2]
						{
							typeof(string),
							typeof(object)
						}));
					}
					frame6 = new Frame(ilgen);
					m = EmitEvalMethodBody(m, ilgen, frame6, program, num9, end_pc, one_op: false, no_bump: false, out out_pc);
					if (m == null)
					{
						return null;
					}
					ilgen.MarkLabel(frame6.label_pass);
					if (System.Text.RegularExpressions.RxInterpreter.trace_rx)
					{
						ilgen.Emit(OpCodes.Ldstr, "tail matched at: {0}");
						ilgen.Emit(OpCodes.Ldarg_1);
						ilgen.Emit(OpCodes.Box, typeof(int));
						ilgen.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[2]
						{
							typeof(string),
							typeof(object)
						}));
					}
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldflda, fi_stack);
					ilgen.Emit(OpCodes.Ldloc, local12);
					ilgen.Emit(OpCodes.Call, GetMethod(typeof(System.Text.RegularExpressions.RxInterpreter.IntStack), "set_Count", ref mi_stack_set_count));
					ilgen.Emit(OpCodes.Br, frame.label_pass);
					ilgen.MarkLabel(frame6.label_fail);
					if (System.Text.RegularExpressions.RxInterpreter.trace_rx)
					{
						ilgen.Emit(OpCodes.Ldstr, "tail failed to match at: {0}");
						ilgen.Emit(OpCodes.Ldarg_1);
						ilgen.Emit(OpCodes.Box, typeof(int));
						ilgen.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[2]
						{
							typeof(string),
							typeof(object)
						}));
					}
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldflda, fi_stack);
					ilgen.Emit(OpCodes.Call, GetMethod(typeof(System.Text.RegularExpressions.RxInterpreter.IntStack), "get_Count", ref mi_stack_get_count));
					ilgen.Emit(OpCodes.Ldloc, local12);
					ilgen.Emit(OpCodes.Beq, frame.label_fail);
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldflda, fi_stack);
					ilgen.Emit(OpCodes.Call, GetMethod(typeof(System.Text.RegularExpressions.RxInterpreter.IntStack), "Pop", ref mi_stack_pop));
					ilgen.Emit(OpCodes.Starg, 1);
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldflda, fi_stack);
					ilgen.Emit(OpCodes.Call, GetMethod(typeof(System.Text.RegularExpressions.RxInterpreter.IntStack), "Pop", ref mi_stack_pop));
					ilgen.Emit(OpCodes.Call, GetMethod("Backtrack", ref mi_backtrack));
					if (System.Text.RegularExpressions.RxInterpreter.trace_rx)
					{
						ilgen.Emit(OpCodes.Ldstr, "backtracking to: {0}");
						ilgen.Emit(OpCodes.Ldarg_1);
						ilgen.Emit(OpCodes.Box, typeof(int));
						ilgen.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[2]
						{
							typeof(string),
							typeof(object)
						}));
					}
					ilgen.MarkLabel(label19);
					ilgen.Emit(OpCodes.Br, label20);
				}
				pc = out_pc;
				break;
			}
			case System.Text.RegularExpressions.RxOp.CategoryAny:
			case System.Text.RegularExpressions.RxOp.CategoryAnySingleline:
			case System.Text.RegularExpressions.RxOp.CategoryDigit:
			case System.Text.RegularExpressions.RxOp.CategoryWord:
			case System.Text.RegularExpressions.RxOp.CategoryWhiteSpace:
			case System.Text.RegularExpressions.RxOp.CategoryEcmaWord:
			case System.Text.RegularExpressions.RxOp.CategoryEcmaWhiteSpace:
			case System.Text.RegularExpressions.RxOp.CategoryUnicode:
			case System.Text.RegularExpressions.RxOp.CategoryUnicodeSpecials:
			{
				System.Text.RegularExpressions.OpFlags opFlags6 = (System.Text.RegularExpressions.OpFlags)op_flags[pc];
				bool flag17 = (int)(opFlags6 & System.Text.RegularExpressions.OpFlags.Negate) > 0;
				bool flag18 = (int)(opFlags6 & System.Text.RegularExpressions.OpFlags.RightToLeft) > 0;
				Label label28 = ilgen.DefineLabel();
				if (flag18)
				{
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldc_I4_0);
					ilgen.Emit(OpCodes.Ble, label28);
				}
				else
				{
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldfld, fi_string_end);
					ilgen.Emit(OpCodes.Bge, label28);
				}
				LocalBuilder local15 = ilgen.DeclareLocal(typeof(char));
				ilgen.Emit(OpCodes.Ldarg_0);
				ilgen.Emit(OpCodes.Ldfld, fi_str);
				ilgen.Emit(OpCodes.Ldarg_1);
				if (flag18)
				{
					ilgen.Emit(OpCodes.Ldc_I4_1);
					ilgen.Emit(OpCodes.Sub);
				}
				ilgen.Emit(OpCodes.Callvirt, typeof(string).GetMethod("get_Chars"));
				ilgen.Emit(OpCodes.Stloc, local15);
				Label label29 = ilgen.DefineLabel();
				Label label30 = ((!flag17) ? label29 : label28);
				Label label31 = ((!flag17) ? label28 : label29);
				switch (rxOp)
				{
				case System.Text.RegularExpressions.RxOp.CategoryAny:
					ilgen.Emit(OpCodes.Ldloc, local15);
					ilgen.Emit(OpCodes.Ldc_I4, 10);
					ilgen.Emit(OpCodes.Bne_Un, label30);
					break;
				case System.Text.RegularExpressions.RxOp.CategoryAnySingleline:
					ilgen.Emit(OpCodes.Br, label30);
					break;
				case System.Text.RegularExpressions.RxOp.CategoryWord:
					ilgen.Emit(OpCodes.Ldloc, local15);
					ilgen.Emit(OpCodes.Call, typeof(char).GetMethod("IsLetterOrDigit", new Type[1] { typeof(char) }));
					ilgen.Emit(OpCodes.Brtrue, label30);
					ilgen.Emit(OpCodes.Ldloc, local15);
					ilgen.Emit(OpCodes.Call, typeof(char).GetMethod("GetUnicodeCategory", new Type[1] { typeof(char) }));
					ilgen.Emit(OpCodes.Ldc_I4, 18);
					ilgen.Emit(OpCodes.Beq, label30);
					break;
				case System.Text.RegularExpressions.RxOp.CategoryDigit:
					ilgen.Emit(OpCodes.Ldloc, local15);
					ilgen.Emit(OpCodes.Call, typeof(char).GetMethod("IsDigit", new Type[1] { typeof(char) }));
					ilgen.Emit(OpCodes.Brtrue, label30);
					break;
				case System.Text.RegularExpressions.RxOp.CategoryWhiteSpace:
					ilgen.Emit(OpCodes.Ldloc, local15);
					ilgen.Emit(OpCodes.Call, typeof(char).GetMethod("IsWhiteSpace", new Type[1] { typeof(char) }));
					ilgen.Emit(OpCodes.Brtrue, label30);
					break;
				case System.Text.RegularExpressions.RxOp.CategoryEcmaWord:
					ilgen.Emit(OpCodes.Ldloc, local15);
					ilgen.Emit(OpCodes.Ldc_I4, 96);
					ilgen.Emit(OpCodes.Cgt);
					ilgen.Emit(OpCodes.Ldloc, local15);
					ilgen.Emit(OpCodes.Ldc_I4, 123);
					ilgen.Emit(OpCodes.Clt);
					ilgen.Emit(OpCodes.And);
					ilgen.Emit(OpCodes.Brtrue, label30);
					ilgen.Emit(OpCodes.Ldloc, local15);
					ilgen.Emit(OpCodes.Ldc_I4, 64);
					ilgen.Emit(OpCodes.Cgt);
					ilgen.Emit(OpCodes.Ldloc, local15);
					ilgen.Emit(OpCodes.Ldc_I4, 91);
					ilgen.Emit(OpCodes.Clt);
					ilgen.Emit(OpCodes.And);
					ilgen.Emit(OpCodes.Brtrue, label30);
					ilgen.Emit(OpCodes.Ldloc, local15);
					ilgen.Emit(OpCodes.Ldc_I4, 47);
					ilgen.Emit(OpCodes.Cgt);
					ilgen.Emit(OpCodes.Ldloc, local15);
					ilgen.Emit(OpCodes.Ldc_I4, 58);
					ilgen.Emit(OpCodes.Clt);
					ilgen.Emit(OpCodes.And);
					ilgen.Emit(OpCodes.Brtrue, label30);
					ilgen.Emit(OpCodes.Ldloc, local15);
					ilgen.Emit(OpCodes.Ldc_I4, 95);
					ilgen.Emit(OpCodes.Beq, label30);
					break;
				case System.Text.RegularExpressions.RxOp.CategoryEcmaWhiteSpace:
					ilgen.Emit(OpCodes.Ldloc, local15);
					ilgen.Emit(OpCodes.Ldc_I4, 32);
					ilgen.Emit(OpCodes.Beq, label30);
					ilgen.Emit(OpCodes.Ldloc, local15);
					ilgen.Emit(OpCodes.Ldc_I4, 9);
					ilgen.Emit(OpCodes.Beq, label30);
					ilgen.Emit(OpCodes.Ldloc, local15);
					ilgen.Emit(OpCodes.Ldc_I4, 10);
					ilgen.Emit(OpCodes.Beq, label30);
					ilgen.Emit(OpCodes.Ldloc, local15);
					ilgen.Emit(OpCodes.Ldc_I4, 13);
					ilgen.Emit(OpCodes.Beq, label30);
					ilgen.Emit(OpCodes.Ldloc, local15);
					ilgen.Emit(OpCodes.Ldc_I4, 12);
					ilgen.Emit(OpCodes.Beq, label30);
					ilgen.Emit(OpCodes.Ldloc, local15);
					ilgen.Emit(OpCodes.Ldc_I4, 11);
					ilgen.Emit(OpCodes.Beq, label30);
					break;
				case System.Text.RegularExpressions.RxOp.CategoryUnicodeSpecials:
					ilgen.Emit(OpCodes.Ldloc, local15);
					ilgen.Emit(OpCodes.Ldc_I4, 65278);
					ilgen.Emit(OpCodes.Cgt);
					ilgen.Emit(OpCodes.Ldloc, local15);
					ilgen.Emit(OpCodes.Ldc_I4, 65280);
					ilgen.Emit(OpCodes.Clt);
					ilgen.Emit(OpCodes.And);
					ilgen.Emit(OpCodes.Brtrue, label30);
					ilgen.Emit(OpCodes.Ldloc, local15);
					ilgen.Emit(OpCodes.Ldc_I4, 65519);
					ilgen.Emit(OpCodes.Cgt);
					ilgen.Emit(OpCodes.Ldloc, local15);
					ilgen.Emit(OpCodes.Ldc_I4, 65534);
					ilgen.Emit(OpCodes.Clt);
					ilgen.Emit(OpCodes.And);
					ilgen.Emit(OpCodes.Brtrue, label30);
					break;
				case System.Text.RegularExpressions.RxOp.CategoryUnicode:
					ilgen.Emit(OpCodes.Ldloc, local15);
					ilgen.Emit(OpCodes.Call, typeof(char).GetMethod("GetUnicodeCategory", new Type[1] { typeof(char) }));
					ilgen.Emit(OpCodes.Ldc_I4, (int)program[pc + 1]);
					ilgen.Emit(OpCodes.Beq, label30);
					break;
				}
				ilgen.Emit(OpCodes.Br, label31);
				ilgen.MarkLabel(label29);
				if (!no_bump)
				{
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldc_I4_1);
					if (flag18)
					{
						ilgen.Emit(OpCodes.Sub);
					}
					else
					{
						ilgen.Emit(OpCodes.Add);
					}
					ilgen.Emit(OpCodes.Starg, 1);
				}
				Label label32 = ilgen.DefineLabel();
				ilgen.Emit(OpCodes.Br, label32);
				ilgen.MarkLabel(label28);
				ilgen.Emit(OpCodes.Br, frame.label_fail);
				ilgen.MarkLabel(label32);
				pc = ((rxOp != System.Text.RegularExpressions.RxOp.CategoryUnicode) ? (pc + 1) : (pc + 2));
				goto IL_370d;
			}
			case System.Text.RegularExpressions.RxOp.Reference:
			{
				System.Text.RegularExpressions.OpFlags opFlags = (System.Text.RegularExpressions.OpFlags)op_flags[pc];
				bool flag = (int)(opFlags & System.Text.RegularExpressions.OpFlags.IgnoreCase) > 0;
				bool flag2 = (int)(opFlags & System.Text.RegularExpressions.OpFlags.RightToLeft) > 0;
				LocalBuilder local = ilgen.DeclareLocal(typeof(int));
				ilgen.Emit(OpCodes.Ldarg_0);
				ilgen.Emit(OpCodes.Ldc_I4, ReadShort(program, pc + 1));
				ilgen.Emit(OpCodes.Call, GetMethod(typeof(System.Text.RegularExpressions.RxInterpreter), "GetLastDefined", ref mi_get_last_defined));
				ilgen.Emit(OpCodes.Stloc, local);
				ilgen.Emit(OpCodes.Ldloc, local);
				ilgen.Emit(OpCodes.Ldc_I4_0);
				ilgen.Emit(OpCodes.Blt, frame.label_fail);
				LocalBuilder local2 = ilgen.DeclareLocal(typeof(int));
				ilgen.Emit(OpCodes.Ldarg_0);
				ilgen.Emit(OpCodes.Ldfld, fi_marks);
				ilgen.Emit(OpCodes.Ldloc, local);
				ilgen.Emit(OpCodes.Ldelema, typeof(System.Text.RegularExpressions.Mark));
				ilgen.Emit(OpCodes.Call, GetMethod(typeof(System.Text.RegularExpressions.Mark), "get_Index", ref mi_mark_get_index));
				ilgen.Emit(OpCodes.Stloc, local2);
				ilgen.Emit(OpCodes.Ldarg_0);
				ilgen.Emit(OpCodes.Ldfld, fi_marks);
				ilgen.Emit(OpCodes.Ldloc, local);
				ilgen.Emit(OpCodes.Ldelema, typeof(System.Text.RegularExpressions.Mark));
				ilgen.Emit(OpCodes.Call, GetMethod(typeof(System.Text.RegularExpressions.Mark), "get_Length", ref mi_mark_get_length));
				ilgen.Emit(OpCodes.Stloc, local);
				if (flag2)
				{
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldloc, local);
					ilgen.Emit(OpCodes.Sub);
					ilgen.Emit(OpCodes.Starg, 1);
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldc_I4_0);
					ilgen.Emit(OpCodes.Blt, frame.label_fail);
				}
				else
				{
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldloc, local);
					ilgen.Emit(OpCodes.Add);
					ilgen.Emit(OpCodes.Ldarg_0);
					ilgen.Emit(OpCodes.Ldfld, fi_string_end);
					ilgen.Emit(OpCodes.Bgt, frame.label_fail);
				}
				LocalBuilder local3 = ilgen.DeclareLocal(typeof(string));
				ilgen.Emit(OpCodes.Ldarg_0);
				ilgen.Emit(OpCodes.Ldfld, fi_str);
				ilgen.Emit(OpCodes.Stloc, local3);
				LocalBuilder local4 = ilgen.DeclareLocal(typeof(int));
				ilgen.Emit(OpCodes.Ldloc, local2);
				ilgen.Emit(OpCodes.Ldloc, local);
				ilgen.Emit(OpCodes.Add);
				ilgen.Emit(OpCodes.Stloc, local4);
				Label label3 = ilgen.DefineLabel();
				ilgen.Emit(OpCodes.Br, label3);
				Label label4 = ilgen.DefineLabel();
				ilgen.MarkLabel(label4);
				if (flag)
				{
					ilgen.Emit(OpCodes.Ldloc, local_textinfo);
				}
				ilgen.Emit(OpCodes.Ldloc, local3);
				ilgen.Emit(OpCodes.Ldarg_1);
				ilgen.Emit(OpCodes.Callvirt, typeof(string).GetMethod("get_Chars"));
				if (flag)
				{
					ilgen.Emit(OpCodes.Callvirt, typeof(TextInfo).GetMethod("ToLower", new Type[1] { typeof(char) }));
				}
				if (flag)
				{
					ilgen.Emit(OpCodes.Ldloc, local_textinfo);
				}
				ilgen.Emit(OpCodes.Ldloc, local3);
				ilgen.Emit(OpCodes.Ldloc, local2);
				ilgen.Emit(OpCodes.Callvirt, typeof(string).GetMethod("get_Chars"));
				if (flag)
				{
					ilgen.Emit(OpCodes.Callvirt, typeof(TextInfo).GetMethod("ToLower", new Type[1] { typeof(char) }));
				}
				ilgen.Emit(OpCodes.Bne_Un, frame.label_fail);
				ilgen.Emit(OpCodes.Ldarg_1);
				ilgen.Emit(OpCodes.Ldc_I4_1);
				ilgen.Emit(OpCodes.Add);
				ilgen.Emit(OpCodes.Starg, 1);
				ilgen.Emit(OpCodes.Ldloc, local2);
				ilgen.Emit(OpCodes.Ldc_I4_1);
				ilgen.Emit(OpCodes.Add);
				ilgen.Emit(OpCodes.Stloc, local2);
				ilgen.MarkLabel(label3);
				ilgen.Emit(OpCodes.Ldloc, local2);
				ilgen.Emit(OpCodes.Ldloc, local4);
				ilgen.Emit(OpCodes.Blt, label4);
				if (flag2)
				{
					ilgen.Emit(OpCodes.Ldarg_1);
					ilgen.Emit(OpCodes.Ldloc, local);
					ilgen.Emit(OpCodes.Sub);
					ilgen.Emit(OpCodes.Starg, 1);
				}
				pc += 3;
				goto IL_370d;
			}
			case System.Text.RegularExpressions.RxOp.IfDefined:
			case System.Text.RegularExpressions.RxOp.Repeat:
			case System.Text.RegularExpressions.RxOp.RepeatLazy:
				if (System.Text.RegularExpressions.RxInterpreter.trace_rx || trace_compile)
				{
					Console.WriteLine(string.Concat("Opcode ", rxOp, " not supported."));
				}
				return null;
			default:
				throw new NotImplementedException(string.Concat("Opcode '", rxOp, "' not supported by the regex->IL compiler."));
			}
			break;
			IL_370d:
			if (one_op)
			{
				break;
			}
		}
		out_pc = pc;
		return m;
	}
}

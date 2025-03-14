using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.CSharp.Nullable;

namespace Mono.CSharp;

public class Unary : Expression
{
	public enum Operator : byte
	{
		UnaryPlus,
		UnaryNegation,
		LogicalNot,
		OnesComplement,
		AddressOf,
		TOP
	}

	public readonly Operator Oper;

	public Expression Expr;

	private ConvCast.Mode enum_conversion;

	public Unary(Operator op, Expression expr, Location loc)
	{
		Oper = op;
		Expr = expr;
		base.loc = loc;
	}

	private Constant TryReduceConstant(ResolveContext ec, Constant constant)
	{
		Constant constant2 = constant;
		while (constant2 is EmptyConstantCast)
		{
			constant2 = ((EmptyConstantCast)constant2).child;
		}
		if (constant2 is SideEffectConstant)
		{
			Constant constant3 = TryReduceConstant(ec, ((SideEffectConstant)constant2).value);
			if (constant3 != null)
			{
				return new SideEffectConstant(constant3, constant2, constant3.Location);
			}
			return null;
		}
		TypeSpec typeSpec = constant2.Type;
		switch (Oper)
		{
		case Operator.UnaryPlus:
			switch (typeSpec.BuiltinType)
			{
			case BuiltinTypeSpec.Type.Byte:
				return new IntConstant(ec.BuiltinTypes, ((ByteConstant)constant2).Value, constant2.Location);
			case BuiltinTypeSpec.Type.SByte:
				return new IntConstant(ec.BuiltinTypes, ((SByteConstant)constant2).Value, constant2.Location);
			case BuiltinTypeSpec.Type.Short:
				return new IntConstant(ec.BuiltinTypes, ((ShortConstant)constant2).Value, constant2.Location);
			case BuiltinTypeSpec.Type.UShort:
				return new IntConstant(ec.BuiltinTypes, ((UShortConstant)constant2).Value, constant2.Location);
			case BuiltinTypeSpec.Type.Char:
				return new IntConstant(ec.BuiltinTypes, ((CharConstant)constant2).Value, constant2.Location);
			case BuiltinTypeSpec.Type.Int:
			case BuiltinTypeSpec.Type.UInt:
			case BuiltinTypeSpec.Type.Long:
			case BuiltinTypeSpec.Type.ULong:
			case BuiltinTypeSpec.Type.Float:
			case BuiltinTypeSpec.Type.Double:
			case BuiltinTypeSpec.Type.Decimal:
				return constant2;
			default:
				return null;
			}
		case Operator.UnaryNegation:
			switch (typeSpec.BuiltinType)
			{
			case BuiltinTypeSpec.Type.Byte:
				return new IntConstant(ec.BuiltinTypes, -((ByteConstant)constant2).Value, constant2.Location);
			case BuiltinTypeSpec.Type.SByte:
				return new IntConstant(ec.BuiltinTypes, -((SByteConstant)constant2).Value, constant2.Location);
			case BuiltinTypeSpec.Type.Short:
				return new IntConstant(ec.BuiltinTypes, -((ShortConstant)constant2).Value, constant2.Location);
			case BuiltinTypeSpec.Type.UShort:
				return new IntConstant(ec.BuiltinTypes, -((UShortConstant)constant2).Value, constant2.Location);
			case BuiltinTypeSpec.Type.Char:
				return new IntConstant(ec.BuiltinTypes, 0 - ((CharConstant)constant2).Value, constant2.Location);
			case BuiltinTypeSpec.Type.Int:
			{
				int value2 = ((IntConstant)constant2).Value;
				if (value2 == int.MinValue)
				{
					if (ec.ConstantCheckState)
					{
						ConstantFold.Error_CompileTimeOverflow(ec, loc);
						return null;
					}
					return constant2;
				}
				return new IntConstant(ec.BuiltinTypes, -value2, constant2.Location);
			}
			case BuiltinTypeSpec.Type.Long:
			{
				long value3 = ((LongConstant)constant2).Value;
				if (value3 == long.MinValue)
				{
					if (ec.ConstantCheckState)
					{
						ConstantFold.Error_CompileTimeOverflow(ec, loc);
						return null;
					}
					return constant2;
				}
				return new LongConstant(ec.BuiltinTypes, -value3, constant2.Location);
			}
			case BuiltinTypeSpec.Type.UInt:
				if (constant is UIntLiteral uIntLiteral)
				{
					if (uIntLiteral.Value == 2147483648u)
					{
						return new IntLiteral(ec.BuiltinTypes, int.MinValue, constant2.Location);
					}
					return new LongLiteral(ec.BuiltinTypes, 0L - (long)uIntLiteral.Value, constant2.Location);
				}
				return new LongConstant(ec.BuiltinTypes, 0L - (long)((UIntConstant)constant2).Value, constant2.Location);
			case BuiltinTypeSpec.Type.ULong:
				if (constant is ULongLiteral uLongLiteral && uLongLiteral.Value == 9223372036854775808uL)
				{
					return new LongLiteral(ec.BuiltinTypes, long.MinValue, constant2.Location);
				}
				return null;
			case BuiltinTypeSpec.Type.Float:
				if (constant is FloatLiteral floatLiteral)
				{
					return new FloatLiteral(ec.BuiltinTypes, 0f - floatLiteral.Value, constant2.Location);
				}
				return new FloatConstant(ec.BuiltinTypes, 0f - ((FloatConstant)constant2).Value, constant2.Location);
			case BuiltinTypeSpec.Type.Double:
				if (constant is DoubleLiteral doubleLiteral)
				{
					return new DoubleLiteral(ec.BuiltinTypes, 0.0 - doubleLiteral.Value, constant2.Location);
				}
				return new DoubleConstant(ec.BuiltinTypes, 0.0 - ((DoubleConstant)constant2).Value, constant2.Location);
			case BuiltinTypeSpec.Type.Decimal:
				return new DecimalConstant(ec.BuiltinTypes, -((DecimalConstant)constant2).Value, constant2.Location);
			default:
				return null;
			}
		case Operator.LogicalNot:
		{
			if (typeSpec.BuiltinType != BuiltinTypeSpec.Type.FirstPrimitive)
			{
				return null;
			}
			bool flag = (bool)constant2.GetValue();
			return new BoolConstant(ec.BuiltinTypes, !flag, constant2.Location);
		}
		case Operator.OnesComplement:
			switch (typeSpec.BuiltinType)
			{
			case BuiltinTypeSpec.Type.Byte:
				return new IntConstant(ec.BuiltinTypes, ~((ByteConstant)constant2).Value, constant2.Location);
			case BuiltinTypeSpec.Type.SByte:
				return new IntConstant(ec.BuiltinTypes, ~((SByteConstant)constant2).Value, constant2.Location);
			case BuiltinTypeSpec.Type.Short:
				return new IntConstant(ec.BuiltinTypes, ~((ShortConstant)constant2).Value, constant2.Location);
			case BuiltinTypeSpec.Type.UShort:
				return new IntConstant(ec.BuiltinTypes, ~((UShortConstant)constant2).Value, constant2.Location);
			case BuiltinTypeSpec.Type.Char:
				return new IntConstant(ec.BuiltinTypes, ~(int)((CharConstant)constant2).Value, constant2.Location);
			case BuiltinTypeSpec.Type.Int:
				return new IntConstant(ec.BuiltinTypes, ~((IntConstant)constant2).Value, constant2.Location);
			case BuiltinTypeSpec.Type.UInt:
				return new UIntConstant(ec.BuiltinTypes, ~((UIntConstant)constant2).Value, constant2.Location);
			case BuiltinTypeSpec.Type.Long:
				return new LongConstant(ec.BuiltinTypes, ~((LongConstant)constant2).Value, constant2.Location);
			case BuiltinTypeSpec.Type.ULong:
				return new ULongConstant(ec.BuiltinTypes, ~((ULongConstant)constant2).Value, constant2.Location);
			default:
				if (constant2 is EnumConstant)
				{
					Constant constant4 = TryReduceConstant(ec, ((EnumConstant)constant2).Child);
					if (constant4 != null)
					{
						if (constant4.Type.BuiltinType == BuiltinTypeSpec.Type.Int)
						{
							int value = ((IntConstant)constant4).Value;
							switch (((EnumConstant)constant2).Child.Type.BuiltinType)
							{
							case BuiltinTypeSpec.Type.UShort:
								constant4 = new UShortConstant(ec.BuiltinTypes, (ushort)value, constant2.Location);
								break;
							case BuiltinTypeSpec.Type.Short:
								constant4 = new ShortConstant(ec.BuiltinTypes, (short)value, constant2.Location);
								break;
							case BuiltinTypeSpec.Type.Byte:
								constant4 = new ByteConstant(ec.BuiltinTypes, (byte)value, constant2.Location);
								break;
							case BuiltinTypeSpec.Type.SByte:
								constant4 = new SByteConstant(ec.BuiltinTypes, (sbyte)value, constant2.Location);
								break;
							}
						}
						constant4 = new EnumConstant(constant4, typeSpec);
					}
					return constant4;
				}
				return null;
			}
		default:
			throw new Exception("Can not constant fold: " + Oper);
		}
	}

	protected virtual Expression ResolveOperator(ResolveContext ec, Expression expr)
	{
		eclass = ExprClass.Value;
		TypeSpec typeSpec = expr.Type;
		TypeSpec[] predefined = ec.BuiltinTypes.OperatorsUnary[(uint)Oper];
		if (BuiltinTypeSpec.IsPrimitiveType(typeSpec))
		{
			Expression expression = ResolvePrimitivePredefinedType(ec, expr, predefined);
			if (expression == null)
			{
				return null;
			}
			type = expression.Type;
			Expr = expression;
			return this;
		}
		if (Oper == Operator.OnesComplement && typeSpec.IsEnum)
		{
			return ResolveEnumOperator(ec, expr, predefined);
		}
		return ResolveUserType(ec, expr, predefined);
	}

	protected virtual Expression ResolveEnumOperator(ResolveContext ec, Expression expr, TypeSpec[] predefined)
	{
		TypeSpec underlyingType = EnumSpec.GetUnderlyingType(expr.Type);
		Expression expression = ResolvePrimitivePredefinedType(ec, EmptyCast.Create(expr, underlyingType), predefined);
		if (expression == null)
		{
			return null;
		}
		Expr = expression;
		enum_conversion = Binary.GetEnumResultCast(underlyingType);
		type = expr.Type;
		return EmptyCast.Create(this, type);
	}

	public override bool ContainsEmitWithAwait()
	{
		return Expr.ContainsEmitWithAwait();
	}

	public override Expression CreateExpressionTree(ResolveContext ec)
	{
		return CreateExpressionTree(ec, null);
	}

	private Expression CreateExpressionTree(ResolveContext ec, Expression user_op)
	{
		string name;
		switch (Oper)
		{
		case Operator.AddressOf:
			Error_PointerInsideExpressionTree(ec);
			return null;
		case Operator.UnaryNegation:
			name = ((!ec.HasSet(ResolveContext.Options.CheckedScope) || user_op != null || IsFloat(type)) ? "Negate" : "NegateChecked");
			break;
		case Operator.LogicalNot:
		case Operator.OnesComplement:
			name = "Not";
			break;
		case Operator.UnaryPlus:
			name = "UnaryPlus";
			break;
		default:
			throw new InternalErrorException("Unknown unary operator " + Oper);
		}
		Arguments arguments = new Arguments(2);
		arguments.Add(new Argument(Expr.CreateExpressionTree(ec)));
		if (user_op != null)
		{
			arguments.Add(new Argument(user_op));
		}
		return CreateExpressionFactoryCall(ec, name, arguments);
	}

	public static TypeSpec[][] CreatePredefinedOperatorsTable(BuiltinTypes types)
	{
		return new TypeSpec[5][]
		{
			new TypeSpec[7] { types.Int, types.UInt, types.Long, types.ULong, types.Float, types.Double, types.Decimal },
			new TypeSpec[5] { types.Int, types.Long, types.Float, types.Double, types.Decimal },
			new TypeSpec[1] { types.Bool },
			new TypeSpec[4] { types.Int, types.UInt, types.Long, types.ULong },
			null
		};
	}

	private static Expression DoNumericPromotion(ResolveContext rc, Operator op, Expression expr)
	{
		TypeSpec typeSpec = expr.Type;
		if (op == Operator.UnaryPlus || op == Operator.UnaryNegation || op == Operator.OnesComplement)
		{
			switch (typeSpec.BuiltinType)
			{
			case BuiltinTypeSpec.Type.Byte:
			case BuiltinTypeSpec.Type.SByte:
			case BuiltinTypeSpec.Type.Char:
			case BuiltinTypeSpec.Type.Short:
			case BuiltinTypeSpec.Type.UShort:
				return Convert.ImplicitNumericConversion(expr, rc.BuiltinTypes.Int);
			}
		}
		if (op == Operator.UnaryNegation && typeSpec.BuiltinType == BuiltinTypeSpec.Type.UInt)
		{
			return Convert.ImplicitNumericConversion(expr, rc.BuiltinTypes.Long);
		}
		return expr;
	}

	protected override Expression DoResolve(ResolveContext ec)
	{
		if (Oper == Operator.AddressOf)
		{
			return ResolveAddressOf(ec);
		}
		Expr = Expr.Resolve(ec);
		if (Expr == null)
		{
			return null;
		}
		if (Expr.Type.BuiltinType == BuiltinTypeSpec.Type.Dynamic)
		{
			Arguments arguments = new Arguments(1);
			arguments.Add(new Argument(Expr));
			return new DynamicUnaryConversion(GetOperatorExpressionTypeName(), arguments, loc).Resolve(ec);
		}
		if (Expr.Type.IsNullableType)
		{
			return new LiftedUnaryOperator(Oper, Expr, loc).Resolve(ec);
		}
		if (Expr is Constant constant)
		{
			Constant constant2 = TryReduceConstant(ec, constant);
			if (constant2 != null)
			{
				return constant2;
			}
		}
		Expression expression = ResolveOperator(ec, Expr);
		if (expression == null)
		{
			Error_OperatorCannotBeApplied(ec, loc, OperName(Oper), Expr.Type);
		}
		if (expression == this && Oper == Operator.UnaryPlus)
		{
			return Expr;
		}
		return expression;
	}

	public override Expression DoResolveLValue(ResolveContext ec, Expression right)
	{
		return null;
	}

	public override void Emit(EmitContext ec)
	{
		EmitOperator(ec, type);
	}

	protected void EmitOperator(EmitContext ec, TypeSpec type)
	{
		switch (Oper)
		{
		case Operator.UnaryPlus:
			Expr.Emit(ec);
			break;
		case Operator.UnaryNegation:
			if (ec.HasSet(BuilderContext.Options.CheckedScope) && !IsFloat(type))
			{
				if (ec.HasSet(BuilderContext.Options.AsyncBody) && Expr.ContainsEmitWithAwait())
				{
					Expr = Expr.EmitToField(ec);
				}
				ec.EmitInt(0);
				if (type.BuiltinType == BuiltinTypeSpec.Type.Long)
				{
					ec.Emit(OpCodes.Conv_U8);
				}
				Expr.Emit(ec);
				ec.Emit(OpCodes.Sub_Ovf);
			}
			else
			{
				Expr.Emit(ec);
				ec.Emit(OpCodes.Neg);
			}
			break;
		case Operator.LogicalNot:
			Expr.Emit(ec);
			ec.EmitInt(0);
			ec.Emit(OpCodes.Ceq);
			break;
		case Operator.OnesComplement:
			Expr.Emit(ec);
			ec.Emit(OpCodes.Not);
			break;
		case Operator.AddressOf:
			((IMemoryLocation)Expr).AddressOf(ec, AddressOp.LoadStore);
			break;
		default:
			throw new Exception("This should not happen: Operator = " + Oper);
		}
		if (enum_conversion != 0)
		{
			using (ec.With(BuilderContext.Options.CheckedScope, enable: false))
			{
				ConvCast.Emit(ec, enum_conversion);
			}
		}
	}

	public override void EmitBranchable(EmitContext ec, Label target, bool on_true)
	{
		if (Oper == Operator.LogicalNot)
		{
			Expr.EmitBranchable(ec, target, !on_true);
		}
		else
		{
			base.EmitBranchable(ec, target, on_true);
		}
	}

	public override void EmitSideEffect(EmitContext ec)
	{
		Expr.EmitSideEffect(ec);
	}

	public static void Error_Ambiguous(ResolveContext rc, string oper, TypeSpec type, Location loc)
	{
		rc.Report.Error(35, loc, "Operator `{0}' is ambiguous on an operand of type `{1}'", oper, type.GetSignatureForError());
	}

	public override void FlowAnalysis(FlowAnalysisContext fc)
	{
		FlowAnalysis(fc, conditional: false);
	}

	public override void FlowAnalysisConditional(FlowAnalysisContext fc)
	{
		FlowAnalysis(fc, conditional: true);
	}

	private void FlowAnalysis(FlowAnalysisContext fc, bool conditional)
	{
		if (Oper == Operator.AddressOf)
		{
			if (Expr is VariableReference variableReference && variableReference.VariableInfo != null)
			{
				fc.SetVariableAssigned(variableReference.VariableInfo);
			}
		}
		else if (Oper == Operator.LogicalNot && conditional)
		{
			Expr.FlowAnalysisConditional(fc);
			DefiniteAssignmentBitSet definiteAssignmentOnTrue = fc.DefiniteAssignmentOnTrue;
			fc.DefiniteAssignmentOnTrue = fc.DefiniteAssignmentOnFalse;
			fc.DefiniteAssignmentOnFalse = definiteAssignmentOnTrue;
		}
		else
		{
			Expr.FlowAnalysis(fc);
		}
	}

	private string GetOperatorExpressionTypeName()
	{
		return Oper switch
		{
			Operator.OnesComplement => "OnesComplement", 
			Operator.LogicalNot => "Not", 
			Operator.UnaryNegation => "Negate", 
			Operator.UnaryPlus => "UnaryPlus", 
			_ => throw new NotImplementedException("Unknown express type operator " + Oper), 
		};
	}

	private static bool IsFloat(TypeSpec t)
	{
		if (t.BuiltinType != BuiltinTypeSpec.Type.Double)
		{
			return t.BuiltinType == BuiltinTypeSpec.Type.Float;
		}
		return true;
	}

	public static string OperName(Operator oper)
	{
		return oper switch
		{
			Operator.UnaryPlus => "+", 
			Operator.UnaryNegation => "-", 
			Operator.LogicalNot => "!", 
			Operator.OnesComplement => "~", 
			Operator.AddressOf => "&", 
			_ => throw new NotImplementedException(oper.ToString()), 
		};
	}

	public override System.Linq.Expressions.Expression MakeExpression(BuilderContext ctx)
	{
		System.Linq.Expressions.Expression expression = Expr.MakeExpression(ctx);
		bool flag = ctx.HasSet(BuilderContext.Options.CheckedScope);
		switch (Oper)
		{
		case Operator.UnaryNegation:
			if (!flag)
			{
				return System.Linq.Expressions.Expression.Negate(expression);
			}
			return System.Linq.Expressions.Expression.NegateChecked(expression);
		case Operator.LogicalNot:
			return System.Linq.Expressions.Expression.Not(expression);
		default:
			throw new NotImplementedException(Oper.ToString());
		}
	}

	private Expression ResolveAddressOf(ResolveContext ec)
	{
		if (!ec.IsUnsafe)
		{
			Expression.UnsafeError(ec, loc);
		}
		Expr = Expr.DoResolveLValue(ec, EmptyExpression.UnaryAddress);
		if (Expr == null || Expr.eclass != ExprClass.Variable)
		{
			ec.Report.Error(211, loc, "Cannot take the address of the given expression");
			return null;
		}
		if (!TypeManager.VerifyUnmanaged(ec.Module, Expr.Type, loc))
		{
			return null;
		}
		bool flag;
		if (Expr is IVariableReference variableReference)
		{
			flag = variableReference.IsFixed;
			variableReference.SetHasAddressTaken();
			if (variableReference.IsHoisted)
			{
				AnonymousMethodExpression.Error_AddressOfCapturedVar(ec, variableReference, loc);
			}
		}
		else
		{
			flag = Expr is IFixedExpression fixedExpression && fixedExpression.IsFixed;
		}
		if (!flag && !ec.HasSet(ResolveContext.Options.FixedInitializerScope))
		{
			ec.Report.Error(212, loc, "You can only take the address of unfixed expression inside of a fixed statement initializer");
		}
		type = PointerContainer.MakeType(ec.Module, Expr.Type);
		eclass = ExprClass.Value;
		return this;
	}

	private Expression ResolvePrimitivePredefinedType(ResolveContext rc, Expression expr, TypeSpec[] predefined)
	{
		expr = DoNumericPromotion(rc, Oper, expr);
		TypeSpec typeSpec = expr.Type;
		for (int i = 0; i < predefined.Length; i++)
		{
			if (predefined[i] == typeSpec)
			{
				return expr;
			}
		}
		return null;
	}

	protected virtual Expression ResolveUserOperator(ResolveContext ec, Expression expr)
	{
		IList<MemberSpec> list = MemberCache.GetUserOperator(op: Oper switch
		{
			Operator.LogicalNot => Mono.CSharp.Operator.OpType.LogicalNot, 
			Operator.OnesComplement => Mono.CSharp.Operator.OpType.OnesComplement, 
			Operator.UnaryNegation => Mono.CSharp.Operator.OpType.UnaryNegation, 
			Operator.UnaryPlus => Mono.CSharp.Operator.OpType.UnaryPlus, 
			_ => throw new InternalErrorException(Oper.ToString()), 
		}, container: expr.Type, declaredOnly: false);
		if (list == null)
		{
			return null;
		}
		Arguments args = new Arguments(1);
		args.Add(new Argument(expr));
		MethodSpec methodSpec = new OverloadResolver(list, OverloadResolver.Restrictions.NoBaseMembers | OverloadResolver.Restrictions.BaseMembersIncluded, loc).ResolveOperator(ec, ref args);
		if (methodSpec == null)
		{
			return null;
		}
		Expr = args[0].Expr;
		return new UserOperatorCall(methodSpec, args, CreateExpressionTree, expr.Location);
	}

	private Expression ResolveUserType(ResolveContext ec, Expression expr, TypeSpec[] predefined)
	{
		Expression expression = ResolveUserOperator(ec, expr);
		if (expression != null)
		{
			return expression;
		}
		foreach (TypeSpec typeSpec in predefined)
		{
			Expression expression2 = Convert.ImplicitUserConversion(ec, expr, typeSpec, expr.Location);
			if (expression2 == null)
			{
				continue;
			}
			if (expression2 == ErrorExpression.Instance)
			{
				return expression2;
			}
			expression2 = ((expression2.Type.BuiltinType != BuiltinTypeSpec.Type.Decimal) ? ResolvePrimitivePredefinedType(ec, expression2, predefined) : ResolveUserType(ec, expression2, predefined));
			if (expression2 == null)
			{
				continue;
			}
			if (expression == null)
			{
				expression = expression2;
				continue;
			}
			switch (OverloadResolver.BetterTypeConversion(ec, expression.Type, typeSpec))
			{
			case 0:
				break;
			case 2:
				expression = expression2;
				continue;
			default:
				continue;
			}
			if ((expression2 is UserOperatorCall || expression2 is UserCast) && (expression is UserOperatorCall || expression is UserCast))
			{
				Error_Ambiguous(ec, OperName(Oper), expr.Type, loc);
			}
			else
			{
				Error_OperatorCannotBeApplied(ec, loc, OperName(Oper), expr.Type);
			}
			break;
		}
		if (expression == null)
		{
			return null;
		}
		if (expression.Type.BuiltinType == BuiltinTypeSpec.Type.Decimal)
		{
			return expression;
		}
		Expr = expression;
		type = expression.Type;
		return this;
	}

	protected override void CloneTo(CloneContext clonectx, Expression t)
	{
		((Unary)t).Expr = Expr.Clone(clonectx);
	}

	public override object Accept(StructuralVisitor visitor)
	{
		return visitor.Visit(this);
	}
}

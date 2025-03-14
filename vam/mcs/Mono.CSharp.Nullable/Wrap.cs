using System.Reflection.Emit;

namespace Mono.CSharp.Nullable;

public class Wrap : TypeCast
{
	private Wrap(Expression expr, TypeSpec type)
		: base(expr, type)
	{
		eclass = ExprClass.Value;
	}

	public override Expression CreateExpressionTree(ResolveContext ec)
	{
		if (child is TypeCast typeCast)
		{
			child.Type = type;
			return typeCast.CreateExpressionTree(ec);
		}
		if (child is UserCast userCast)
		{
			child.Type = type;
			return userCast.CreateExpressionTree(ec);
		}
		return base.CreateExpressionTree(ec);
	}

	public static Expression Create(Expression expr, TypeSpec type)
	{
		if (expr is Unwrap unwrap && expr.Type == NullableInfo.GetUnderlyingType(type))
		{
			return unwrap.Original;
		}
		return new Wrap(expr, type);
	}

	public override void Emit(EmitContext ec)
	{
		child.Emit(ec);
		ec.Emit(OpCodes.Newobj, NullableInfo.GetConstructor(type));
	}
}

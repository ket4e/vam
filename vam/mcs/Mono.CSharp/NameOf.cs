using System;

namespace Mono.CSharp;

internal class NameOf : StringConstant
{
	private readonly SimpleName name;

	public NameOf(SimpleName name)
		: base(name.Location)
	{
		this.name = name;
	}

	private static void Error_MethodGroupWithTypeArguments(ResolveContext rc, Location loc)
	{
		rc.Report.Error(8084, loc, "An argument to nameof operator cannot be method group with type arguments");
	}

	protected override Expression DoResolve(ResolveContext rc)
	{
		throw new NotSupportedException();
	}

	private bool ResolveArgumentExpression(ResolveContext rc, Expression expr)
	{
		if (expr is SimpleName simpleName)
		{
			base.Value = simpleName.Name;
			if (rc.Module.Compiler.Settings.Version < LanguageVersion.V_6)
			{
				rc.Report.FeatureIsNotAvailable(rc.Module.Compiler, base.Location, "nameof operator");
			}
			Expression expression = simpleName.LookupNameExpression(rc, MemberLookupRestrictions.IgnoreAmbiguity | MemberLookupRestrictions.NameOfExcluded);
			if (simpleName.HasTypeArguments && expression is MethodGroupExpr)
			{
				Error_MethodGroupWithTypeArguments(rc, expr.Location);
			}
			return true;
		}
		if (expr is MemberAccess memberAccess)
		{
			Expression leftExpression = memberAccess.LeftExpression;
			Expression expression2 = memberAccess.LookupNameExpression(rc, MemberLookupRestrictions.IgnoreAmbiguity);
			if (expression2 == null)
			{
				return false;
			}
			if (rc.Module.Compiler.Settings.Version < LanguageVersion.V_6)
			{
				rc.Report.FeatureIsNotAvailable(rc.Module.Compiler, base.Location, "nameof operator");
			}
			if (memberAccess is QualifiedAliasMember)
			{
				rc.Report.Error(8083, loc, "An alias-qualified name is not an expression");
				return false;
			}
			if (!IsLeftExpressionValid(leftExpression))
			{
				rc.Report.Error(8082, leftExpression.Location, "An argument to nameof operator cannot include sub-expression");
				return false;
			}
			if (expression2 is MethodGroupExpr methodGroupExpr)
			{
				if (expression2 is ExtensionMethodGroupExpr extensionMethodGroupExpr && !extensionMethodGroupExpr.ResolveNameOf(rc, memberAccess))
				{
					return true;
				}
				if (!methodGroupExpr.HasAccessibleCandidate(rc))
				{
					Expression.ErrorIsInaccesible(rc, memberAccess.GetSignatureForError(), loc);
				}
				if (memberAccess.HasTypeArguments)
				{
					Error_MethodGroupWithTypeArguments(rc, memberAccess.Location);
				}
			}
			base.Value = memberAccess.Name;
			return true;
		}
		rc.Report.Error(8081, loc, "Expression does not have a name");
		return false;
	}

	private static bool IsLeftExpressionValid(Expression expr)
	{
		if (expr is SimpleName)
		{
			return true;
		}
		if (expr is This)
		{
			return true;
		}
		if (expr is NamespaceExpression)
		{
			return true;
		}
		if (expr is TypeExpr)
		{
			return true;
		}
		if (expr is MemberAccess memberAccess)
		{
			return IsLeftExpressionValid(memberAccess.LeftExpression);
		}
		return false;
	}

	public Expression ResolveOverload(ResolveContext rc, Arguments args)
	{
		if (args == null || args.Count != 1)
		{
			name.Error_NameDoesNotExist(rc);
			return null;
		}
		Argument argument = args[0];
		if (!ResolveArgumentExpression(rc, argument.Expr))
		{
			return null;
		}
		type = rc.BuiltinTypes.String;
		eclass = ExprClass.Value;
		return this;
	}
}

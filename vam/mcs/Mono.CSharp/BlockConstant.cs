namespace Mono.CSharp;

public class BlockConstant : BlockVariable
{
	public BlockConstant(FullNamedExpression type, LocalVariable li)
		: base(type, li)
	{
	}

	public override void Emit(EmitContext ec)
	{
	}

	protected override Expression ResolveInitializer(BlockContext bc, LocalVariable li, Expression initializer)
	{
		initializer = initializer.Resolve(bc);
		if (initializer == null)
		{
			return null;
		}
		if (!(initializer is Constant constant))
		{
			initializer.Error_ExpressionMustBeConstant(bc, initializer.Location, li.Name);
			return null;
		}
		Constant constant2 = constant.ConvertImplicitly(li.Type);
		if (constant2 == null)
		{
			if (TypeSpec.IsReferenceType(li.Type))
			{
				initializer.Error_ConstantCanBeInitializedWithNullOnly(bc, li.Type, initializer.Location, li.Name);
			}
			else
			{
				initializer.Error_ValueCannotBeConverted(bc, li.Type, expl: false);
			}
			return null;
		}
		li.ConstantValue = constant2;
		return initializer;
	}

	public override object Accept(StructuralVisitor visitor)
	{
		return visitor.Visit(this);
	}
}

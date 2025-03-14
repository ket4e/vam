using System;

namespace Mono.CSharp;

public class ReferenceContainer : ElementTypeSpec
{
	private ReferenceContainer(TypeSpec element)
		: base(MemberKind.Class, element, null)
	{
	}

	public override Type GetMetaInfo()
	{
		if (info == null)
		{
			info = base.Element.GetMetaInfo().MakeByRefType();
		}
		return info;
	}

	public static ReferenceContainer MakeType(ModuleContainer module, TypeSpec element)
	{
		if (!module.ReferenceTypesCache.TryGetValue(element, out var value))
		{
			value = new ReferenceContainer(element);
			module.ReferenceTypesCache.Add(element, value);
		}
		return value;
	}
}

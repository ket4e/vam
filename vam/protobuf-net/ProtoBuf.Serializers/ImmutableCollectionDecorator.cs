using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ProtoBuf.Compiler;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers;

internal sealed class ImmutableCollectionDecorator : ListDecorator
{
	private readonly MethodInfo builderFactory;

	private readonly MethodInfo add;

	private readonly MethodInfo addRange;

	private readonly MethodInfo finish;

	protected override bool RequireAdd => false;

	private static Type ResolveIReadOnlyCollection(Type declaredType, Type t)
	{
		Type[] interfaces = declaredType.GetInterfaces();
		foreach (Type type in interfaces)
		{
			if (!type.IsGenericType || !type.Name.StartsWith("IReadOnlyCollection`"))
			{
				continue;
			}
			if (t != null)
			{
				Type[] genericArguments = type.GetGenericArguments();
				if (genericArguments.Length != 1 && genericArguments[0] != t)
				{
					continue;
				}
			}
			return type;
		}
		return null;
	}

	internal static bool IdentifyImmutable(TypeModel model, Type declaredType, out MethodInfo builderFactory, out MethodInfo add, out MethodInfo addRange, out MethodInfo finish)
	{
		builderFactory = (add = (addRange = (finish = null)));
		if (model == null || declaredType == null)
		{
			return false;
		}
		if (!declaredType.IsGenericType)
		{
			return false;
		}
		Type[] genericArguments = declaredType.GetGenericArguments();
		Type[] array;
		switch (genericArguments.Length)
		{
		case 1:
			array = genericArguments;
			break;
		case 2:
		{
			Type type = model.MapType(typeof(KeyValuePair<, >));
			if (type == null)
			{
				return false;
			}
			type = type.MakeGenericType(genericArguments);
			array = new Type[1] { type };
			break;
		}
		default:
			return false;
		}
		if (ResolveIReadOnlyCollection(declaredType, null) == null)
		{
			return false;
		}
		string name = declaredType.Name;
		int num = name.IndexOf('`');
		if (num <= 0)
		{
			return false;
		}
		name = (declaredType.IsInterface ? name.Substring(1, num - 1) : name.Substring(0, num));
		Type type2 = model.GetType(declaredType.Namespace + "." + name, declaredType.Assembly);
		if (type2 == null && name == "ImmutableSet")
		{
			type2 = model.GetType(declaredType.Namespace + ".ImmutableHashSet", declaredType.Assembly);
		}
		if (type2 == null)
		{
			return false;
		}
		MethodInfo[] methods = type2.GetMethods();
		foreach (MethodInfo methodInfo in methods)
		{
			if (methodInfo.IsStatic && !(methodInfo.Name != "CreateBuilder") && methodInfo.IsGenericMethodDefinition && methodInfo.GetParameters().Length == 0 && methodInfo.GetGenericArguments().Length == genericArguments.Length)
			{
				builderFactory = methodInfo.MakeGenericMethod(genericArguments);
				break;
			}
		}
		Type type3 = model.MapType(typeof(void));
		if (builderFactory == null || builderFactory.ReturnType == null || builderFactory.ReturnType == type3)
		{
			return false;
		}
		add = Helpers.GetInstanceMethod(builderFactory.ReturnType, "Add", array);
		if (add == null)
		{
			return false;
		}
		finish = Helpers.GetInstanceMethod(builderFactory.ReturnType, "ToImmutable", Helpers.EmptyTypes);
		if (finish == null || finish.ReturnType == null || finish.ReturnType == type3)
		{
			return false;
		}
		if (finish.ReturnType != declaredType && !Helpers.IsAssignableFrom(declaredType, finish.ReturnType))
		{
			return false;
		}
		addRange = Helpers.GetInstanceMethod(builderFactory.ReturnType, "AddRange", new Type[1] { declaredType });
		if (addRange == null)
		{
			Type type4 = model.MapType(typeof(IEnumerable<>), demand: false);
			if (type4 != null)
			{
				addRange = Helpers.GetInstanceMethod(builderFactory.ReturnType, "AddRange", new Type[1] { type4.MakeGenericType(array) });
			}
		}
		return true;
	}

	internal ImmutableCollectionDecorator(TypeModel model, Type declaredType, Type concreteType, IProtoSerializer tail, int fieldNumber, bool writePacked, WireType packedWireType, bool returnList, bool overwriteList, bool supportNull, MethodInfo builderFactory, MethodInfo add, MethodInfo addRange, MethodInfo finish)
		: base(model, declaredType, concreteType, tail, fieldNumber, writePacked, packedWireType, returnList, overwriteList, supportNull)
	{
		this.builderFactory = builderFactory;
		this.add = add;
		this.addRange = addRange;
		this.finish = finish;
	}

	public override object Read(object value, ProtoReader source)
	{
		object obj = builderFactory.Invoke(null, null);
		int field = source.FieldNumber;
		object[] array = new object[1];
		if (base.AppendToCollection && value != null && ((IList)value).Count != 0)
		{
			if (addRange != null)
			{
				array[0] = value;
				addRange.Invoke(obj, array);
			}
			else
			{
				foreach (object item in (IList)value)
				{
					array[0] = item;
					add.Invoke(obj, array);
				}
			}
		}
		if (packedWireType != WireType.None && source.WireType == WireType.String)
		{
			SubItemToken token = ProtoReader.StartSubItem(source);
			while (ProtoReader.HasSubValue(packedWireType, source))
			{
				array[0] = Tail.Read(null, source);
				add.Invoke(obj, array);
			}
			ProtoReader.EndSubItem(token, source);
		}
		else
		{
			do
			{
				array[0] = Tail.Read(null, source);
				add.Invoke(obj, array);
			}
			while (source.TryReadFieldHeader(field));
		}
		return finish.Invoke(obj, null);
	}

	protected override void EmitRead(CompilerContext ctx, Local valueFrom)
	{
		using Local local2 = (base.AppendToCollection ? ctx.GetLocalWithValue(ExpectedType, valueFrom) : null);
		using Local local = new Local(ctx, builderFactory.ReturnType);
		ctx.EmitCall(builderFactory);
		ctx.StoreValue(local);
		if (base.AppendToCollection)
		{
			CodeLabel label = ctx.DefineLabel();
			if (!ExpectedType.IsValueType)
			{
				ctx.LoadValue(local2);
				ctx.BranchIfFalse(label, @short: false);
			}
			PropertyInfo property = Helpers.GetProperty(ExpectedType, "Length", nonPublic: false);
			if (property == null)
			{
				property = Helpers.GetProperty(ExpectedType, "Count", nonPublic: false);
			}
			if (property == null)
			{
				property = Helpers.GetProperty(ResolveIReadOnlyCollection(ExpectedType, Tail.ExpectedType), "Count", nonPublic: false);
			}
			ctx.LoadAddress(local2, local2.Type);
			ctx.EmitCall(Helpers.GetGetMethod(property, nonPublic: false, allowInternal: false));
			ctx.BranchIfFalse(label, @short: false);
			Type type = ctx.MapType(typeof(void));
			if (addRange != null)
			{
				ctx.LoadValue(local);
				ctx.LoadValue(local2);
				ctx.EmitCall(addRange);
				if (addRange.ReturnType != null && add.ReturnType != type)
				{
					ctx.DiscardValue();
				}
			}
			else
			{
				MethodInfo moveNext;
				MethodInfo current;
				MethodInfo enumeratorInfo = GetEnumeratorInfo(ctx.Model, out moveNext, out current);
				Type returnType = enumeratorInfo.ReturnType;
				using Local local3 = new Local(ctx, returnType);
				ctx.LoadAddress(local2, ExpectedType);
				ctx.EmitCall(enumeratorInfo);
				ctx.StoreValue(local3);
				using (ctx.Using(local3))
				{
					CodeLabel label2 = ctx.DefineLabel();
					CodeLabel label3 = ctx.DefineLabel();
					ctx.Branch(label3, @short: false);
					ctx.MarkLabel(label2);
					ctx.LoadAddress(local, local.Type);
					ctx.LoadAddress(local3, returnType);
					ctx.EmitCall(current);
					ctx.EmitCall(add);
					if (add.ReturnType != null && add.ReturnType != type)
					{
						ctx.DiscardValue();
					}
					ctx.MarkLabel(label3);
					ctx.LoadAddress(local3, returnType);
					ctx.EmitCall(moveNext);
					ctx.BranchIfTrue(label2, @short: false);
				}
			}
			ctx.MarkLabel(label);
		}
		ListDecorator.EmitReadList(ctx, local, Tail, add, packedWireType, castListForAdd: false);
		ctx.LoadAddress(local, local.Type);
		ctx.EmitCall(finish);
		if (ExpectedType != finish.ReturnType)
		{
			ctx.Cast(ExpectedType);
		}
	}
}

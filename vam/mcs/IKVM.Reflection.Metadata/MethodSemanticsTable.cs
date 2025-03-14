using System;
using System.Collections.Generic;
using IKVM.Reflection.Emit;
using IKVM.Reflection.Reader;
using IKVM.Reflection.Writer;

namespace IKVM.Reflection.Metadata;

internal sealed class MethodSemanticsTable : SortedTable<MethodSemanticsTable.Record>
{
	internal struct Record : IRecord
	{
		internal short Semantics;

		internal int Method;

		internal int Association;

		int IRecord.SortKey => Association;

		int IRecord.FilterKey => Association;
	}

	internal const int Index = 24;

	internal const short Setter = 1;

	internal const short Getter = 2;

	internal const short Other = 4;

	internal const short AddOn = 8;

	internal const short RemoveOn = 16;

	internal const short Fire = 32;

	internal override void Read(MetadataReader mr)
	{
		for (int i = 0; i < records.Length; i++)
		{
			records[i].Semantics = mr.ReadInt16();
			records[i].Method = mr.ReadMethodDef();
			records[i].Association = mr.ReadHasSemantics();
		}
	}

	internal override void Write(MetadataWriter mw)
	{
		for (int i = 0; i < rowCount; i++)
		{
			mw.Write(records[i].Semantics);
			mw.WriteMethodDef(records[i].Method);
			mw.WriteHasSemantics(records[i].Association);
		}
	}

	protected override int GetRowSize(RowSizeCalc rsc)
	{
		return rsc.AddFixed(2).WriteMethodDef().WriteHasSemantics()
			.Value;
	}

	internal void Fixup(ModuleBuilder moduleBuilder)
	{
		for (int i = 0; i < rowCount; i++)
		{
			moduleBuilder.FixupPseudoToken(ref records[i].Method);
			int association = records[i].Association;
			association = (association >> 24) switch
			{
				20 => ((association & 0xFFFFFF) << 1) | 0, 
				23 => ((association & 0xFFFFFF) << 1) | 1, 
				_ => throw new InvalidOperationException(), 
			};
			records[i].Association = association;
		}
		Sort();
	}

	internal MethodInfo GetMethod(Module module, int token, bool nonPublic, short semantics)
	{
		Enumerator enumerator = Filter(token).GetEnumerator();
		while (enumerator.MoveNext())
		{
			int current = enumerator.Current;
			if ((records[current].Semantics & semantics) != 0)
			{
				MethodBase methodBase = module.ResolveMethod((6 << 24) + records[current].Method);
				if (nonPublic || methodBase.IsPublic)
				{
					return (MethodInfo)methodBase;
				}
			}
		}
		return null;
	}

	internal MethodInfo[] GetMethods(Module module, int token, bool nonPublic, short semantics)
	{
		List<MethodInfo> list = new List<MethodInfo>();
		Enumerator enumerator = Filter(token).GetEnumerator();
		while (enumerator.MoveNext())
		{
			int current = enumerator.Current;
			if ((records[current].Semantics & semantics) != 0)
			{
				MethodInfo methodInfo = (MethodInfo)module.ResolveMethod((6 << 24) + records[current].Method);
				if (nonPublic || methodInfo.IsPublic)
				{
					list.Add(methodInfo);
				}
			}
		}
		return list.ToArray();
	}

	internal void ComputeFlags(Module module, int token, out bool isPublic, out bool isNonPrivate, out bool isStatic)
	{
		isPublic = false;
		isNonPrivate = false;
		isStatic = false;
		Enumerator enumerator = Filter(token).GetEnumerator();
		while (enumerator.MoveNext())
		{
			int current = enumerator.Current;
			MethodBase methodBase = module.ResolveMethod((6 << 24) + records[current].Method);
			isPublic |= methodBase.IsPublic;
			isNonPrivate |= (methodBase.Attributes & MethodAttributes.MemberAccessMask) > MethodAttributes.Private;
			isStatic |= methodBase.IsStatic;
		}
	}
}

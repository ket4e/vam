using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Mono.Cecil.Metadata;
using Mono.Collections.Generic;

namespace Mono.Cecil;

internal sealed class MetadataSystem
{
	internal AssemblyNameReference[] AssemblyReferences;

	internal ModuleReference[] ModuleReferences;

	internal TypeDefinition[] Types;

	internal TypeReference[] TypeReferences;

	internal FieldDefinition[] Fields;

	internal MethodDefinition[] Methods;

	internal MemberReference[] MemberReferences;

	internal Dictionary<uint, Collection<uint>> NestedTypes;

	internal Dictionary<uint, uint> ReverseNestedTypes;

	internal Dictionary<uint, Collection<Row<uint, MetadataToken>>> Interfaces;

	internal Dictionary<uint, Row<ushort, uint>> ClassLayouts;

	internal Dictionary<uint, uint> FieldLayouts;

	internal Dictionary<uint, uint> FieldRVAs;

	internal Dictionary<MetadataToken, uint> FieldMarshals;

	internal Dictionary<MetadataToken, Row<ElementType, uint>> Constants;

	internal Dictionary<uint, Collection<MetadataToken>> Overrides;

	internal Dictionary<MetadataToken, Range[]> CustomAttributes;

	internal Dictionary<MetadataToken, Range[]> SecurityDeclarations;

	internal Dictionary<uint, Range> Events;

	internal Dictionary<uint, Range> Properties;

	internal Dictionary<uint, Row<MethodSemanticsAttributes, MetadataToken>> Semantics;

	internal Dictionary<uint, Row<PInvokeAttributes, uint, uint>> PInvokes;

	internal Dictionary<MetadataToken, Range[]> GenericParameters;

	internal Dictionary<uint, Collection<MetadataToken>> GenericConstraints;

	internal Document[] Documents;

	internal Dictionary<uint, Collection<Row<uint, Range, Range, uint, uint, uint>>> LocalScopes;

	internal ImportDebugInformation[] ImportScopes;

	internal Dictionary<uint, uint> StateMachineMethods;

	internal Dictionary<MetadataToken, Row<Guid, uint, uint>[]> CustomDebugInformations;

	private static Dictionary<string, Row<ElementType, bool>> primitive_value_types;

	private static void InitializePrimitives()
	{
		primitive_value_types = new Dictionary<string, Row<ElementType, bool>>(18, StringComparer.Ordinal)
		{
			{
				"Void",
				new Row<ElementType, bool>(ElementType.Void, col2: false)
			},
			{
				"Boolean",
				new Row<ElementType, bool>(ElementType.Boolean, col2: true)
			},
			{
				"Char",
				new Row<ElementType, bool>(ElementType.Char, col2: true)
			},
			{
				"SByte",
				new Row<ElementType, bool>(ElementType.I1, col2: true)
			},
			{
				"Byte",
				new Row<ElementType, bool>(ElementType.U1, col2: true)
			},
			{
				"Int16",
				new Row<ElementType, bool>(ElementType.I2, col2: true)
			},
			{
				"UInt16",
				new Row<ElementType, bool>(ElementType.U2, col2: true)
			},
			{
				"Int32",
				new Row<ElementType, bool>(ElementType.I4, col2: true)
			},
			{
				"UInt32",
				new Row<ElementType, bool>(ElementType.U4, col2: true)
			},
			{
				"Int64",
				new Row<ElementType, bool>(ElementType.I8, col2: true)
			},
			{
				"UInt64",
				new Row<ElementType, bool>(ElementType.U8, col2: true)
			},
			{
				"Single",
				new Row<ElementType, bool>(ElementType.R4, col2: true)
			},
			{
				"Double",
				new Row<ElementType, bool>(ElementType.R8, col2: true)
			},
			{
				"String",
				new Row<ElementType, bool>(ElementType.String, col2: false)
			},
			{
				"TypedReference",
				new Row<ElementType, bool>(ElementType.TypedByRef, col2: false)
			},
			{
				"IntPtr",
				new Row<ElementType, bool>(ElementType.I, col2: true)
			},
			{
				"UIntPtr",
				new Row<ElementType, bool>(ElementType.U, col2: true)
			},
			{
				"Object",
				new Row<ElementType, bool>(ElementType.Object, col2: false)
			}
		};
	}

	public static void TryProcessPrimitiveTypeReference(TypeReference type)
	{
		if (!(type.Namespace != "System"))
		{
			IMetadataScope scope = type.scope;
			if (scope != null && scope.MetadataScopeType == MetadataScopeType.AssemblyNameReference && TryGetPrimitiveData(type, out var primitive_data))
			{
				type.etype = primitive_data.Col1;
				type.IsValueType = primitive_data.Col2;
			}
		}
	}

	public static bool TryGetPrimitiveElementType(TypeDefinition type, out ElementType etype)
	{
		etype = ElementType.None;
		if (type.Namespace != "System")
		{
			return false;
		}
		if (TryGetPrimitiveData(type, out var primitive_data))
		{
			etype = primitive_data.Col1;
			return true;
		}
		return false;
	}

	private static bool TryGetPrimitiveData(TypeReference type, out Row<ElementType, bool> primitive_data)
	{
		if (primitive_value_types == null)
		{
			InitializePrimitives();
		}
		return primitive_value_types.TryGetValue(type.Name, out primitive_data);
	}

	public void Clear()
	{
		if (NestedTypes != null)
		{
			NestedTypes = new Dictionary<uint, Collection<uint>>(0);
		}
		if (ReverseNestedTypes != null)
		{
			ReverseNestedTypes = new Dictionary<uint, uint>(0);
		}
		if (Interfaces != null)
		{
			Interfaces = new Dictionary<uint, Collection<Row<uint, MetadataToken>>>(0);
		}
		if (ClassLayouts != null)
		{
			ClassLayouts = new Dictionary<uint, Row<ushort, uint>>(0);
		}
		if (FieldLayouts != null)
		{
			FieldLayouts = new Dictionary<uint, uint>(0);
		}
		if (FieldRVAs != null)
		{
			FieldRVAs = new Dictionary<uint, uint>(0);
		}
		if (FieldMarshals != null)
		{
			FieldMarshals = new Dictionary<MetadataToken, uint>(0);
		}
		if (Constants != null)
		{
			Constants = new Dictionary<MetadataToken, Row<ElementType, uint>>(0);
		}
		if (Overrides != null)
		{
			Overrides = new Dictionary<uint, Collection<MetadataToken>>(0);
		}
		if (CustomAttributes != null)
		{
			CustomAttributes = new Dictionary<MetadataToken, Range[]>(0);
		}
		if (SecurityDeclarations != null)
		{
			SecurityDeclarations = new Dictionary<MetadataToken, Range[]>(0);
		}
		if (Events != null)
		{
			Events = new Dictionary<uint, Range>(0);
		}
		if (Properties != null)
		{
			Properties = new Dictionary<uint, Range>(0);
		}
		if (Semantics != null)
		{
			Semantics = new Dictionary<uint, Row<MethodSemanticsAttributes, MetadataToken>>(0);
		}
		if (PInvokes != null)
		{
			PInvokes = new Dictionary<uint, Row<PInvokeAttributes, uint, uint>>(0);
		}
		if (GenericParameters != null)
		{
			GenericParameters = new Dictionary<MetadataToken, Range[]>(0);
		}
		if (GenericConstraints != null)
		{
			GenericConstraints = new Dictionary<uint, Collection<MetadataToken>>(0);
		}
		Documents = Empty<Document>.Array;
		ImportScopes = Empty<ImportDebugInformation>.Array;
		if (LocalScopes != null)
		{
			LocalScopes = new Dictionary<uint, Collection<Row<uint, Range, Range, uint, uint, uint>>>(0);
		}
		if (StateMachineMethods != null)
		{
			StateMachineMethods = new Dictionary<uint, uint>(0);
		}
	}

	public AssemblyNameReference GetAssemblyNameReference(uint rid)
	{
		if (rid < 1 || rid > AssemblyReferences.Length)
		{
			return null;
		}
		return AssemblyReferences[rid - 1];
	}

	public TypeDefinition GetTypeDefinition(uint rid)
	{
		if (rid < 1 || rid > Types.Length)
		{
			return null;
		}
		return Types[rid - 1];
	}

	public void AddTypeDefinition(TypeDefinition type)
	{
		Types[type.token.RID - 1] = type;
	}

	public TypeReference GetTypeReference(uint rid)
	{
		if (rid < 1 || rid > TypeReferences.Length)
		{
			return null;
		}
		return TypeReferences[rid - 1];
	}

	public void AddTypeReference(TypeReference type)
	{
		TypeReferences[type.token.RID - 1] = type;
	}

	public FieldDefinition GetFieldDefinition(uint rid)
	{
		if (rid < 1 || rid > Fields.Length)
		{
			return null;
		}
		return Fields[rid - 1];
	}

	public void AddFieldDefinition(FieldDefinition field)
	{
		Fields[field.token.RID - 1] = field;
	}

	public MethodDefinition GetMethodDefinition(uint rid)
	{
		if (rid < 1 || rid > Methods.Length)
		{
			return null;
		}
		return Methods[rid - 1];
	}

	public void AddMethodDefinition(MethodDefinition method)
	{
		Methods[method.token.RID - 1] = method;
	}

	public MemberReference GetMemberReference(uint rid)
	{
		if (rid < 1 || rid > MemberReferences.Length)
		{
			return null;
		}
		return MemberReferences[rid - 1];
	}

	public void AddMemberReference(MemberReference member)
	{
		MemberReferences[member.token.RID - 1] = member;
	}

	public bool TryGetNestedTypeMapping(TypeDefinition type, out Collection<uint> mapping)
	{
		return NestedTypes.TryGetValue(type.token.RID, out mapping);
	}

	public void SetNestedTypeMapping(uint type_rid, Collection<uint> mapping)
	{
		NestedTypes[type_rid] = mapping;
	}

	public void RemoveNestedTypeMapping(TypeDefinition type)
	{
		NestedTypes.Remove(type.token.RID);
	}

	public bool TryGetReverseNestedTypeMapping(TypeDefinition type, out uint declaring)
	{
		return ReverseNestedTypes.TryGetValue(type.token.RID, out declaring);
	}

	public void SetReverseNestedTypeMapping(uint nested, uint declaring)
	{
		ReverseNestedTypes[nested] = declaring;
	}

	public void RemoveReverseNestedTypeMapping(TypeDefinition type)
	{
		ReverseNestedTypes.Remove(type.token.RID);
	}

	public bool TryGetInterfaceMapping(TypeDefinition type, out Collection<Row<uint, MetadataToken>> mapping)
	{
		return Interfaces.TryGetValue(type.token.RID, out mapping);
	}

	public void SetInterfaceMapping(uint type_rid, Collection<Row<uint, MetadataToken>> mapping)
	{
		Interfaces[type_rid] = mapping;
	}

	public void RemoveInterfaceMapping(TypeDefinition type)
	{
		Interfaces.Remove(type.token.RID);
	}

	public void AddPropertiesRange(uint type_rid, Range range)
	{
		Properties.Add(type_rid, range);
	}

	public bool TryGetPropertiesRange(TypeDefinition type, out Range range)
	{
		return Properties.TryGetValue(type.token.RID, out range);
	}

	public void RemovePropertiesRange(TypeDefinition type)
	{
		Properties.Remove(type.token.RID);
	}

	public void AddEventsRange(uint type_rid, Range range)
	{
		Events.Add(type_rid, range);
	}

	public bool TryGetEventsRange(TypeDefinition type, out Range range)
	{
		return Events.TryGetValue(type.token.RID, out range);
	}

	public void RemoveEventsRange(TypeDefinition type)
	{
		Events.Remove(type.token.RID);
	}

	public bool TryGetGenericParameterRanges(IGenericParameterProvider owner, out Range[] ranges)
	{
		return GenericParameters.TryGetValue(owner.MetadataToken, out ranges);
	}

	public void RemoveGenericParameterRange(IGenericParameterProvider owner)
	{
		GenericParameters.Remove(owner.MetadataToken);
	}

	public bool TryGetCustomAttributeRanges(ICustomAttributeProvider owner, out Range[] ranges)
	{
		return CustomAttributes.TryGetValue(owner.MetadataToken, out ranges);
	}

	public void RemoveCustomAttributeRange(ICustomAttributeProvider owner)
	{
		CustomAttributes.Remove(owner.MetadataToken);
	}

	public bool TryGetSecurityDeclarationRanges(ISecurityDeclarationProvider owner, out Range[] ranges)
	{
		return SecurityDeclarations.TryGetValue(owner.MetadataToken, out ranges);
	}

	public void RemoveSecurityDeclarationRange(ISecurityDeclarationProvider owner)
	{
		SecurityDeclarations.Remove(owner.MetadataToken);
	}

	public bool TryGetGenericConstraintMapping(GenericParameter generic_parameter, out Collection<MetadataToken> mapping)
	{
		return GenericConstraints.TryGetValue(generic_parameter.token.RID, out mapping);
	}

	public void SetGenericConstraintMapping(uint gp_rid, Collection<MetadataToken> mapping)
	{
		GenericConstraints[gp_rid] = mapping;
	}

	public void RemoveGenericConstraintMapping(GenericParameter generic_parameter)
	{
		GenericConstraints.Remove(generic_parameter.token.RID);
	}

	public bool TryGetOverrideMapping(MethodDefinition method, out Collection<MetadataToken> mapping)
	{
		return Overrides.TryGetValue(method.token.RID, out mapping);
	}

	public void SetOverrideMapping(uint rid, Collection<MetadataToken> mapping)
	{
		Overrides[rid] = mapping;
	}

	public void RemoveOverrideMapping(MethodDefinition method)
	{
		Overrides.Remove(method.token.RID);
	}

	public Document GetDocument(uint rid)
	{
		if (rid < 1 || rid > Documents.Length)
		{
			return null;
		}
		return Documents[rid - 1];
	}

	public bool TryGetLocalScopes(MethodDefinition method, out Collection<Row<uint, Range, Range, uint, uint, uint>> scopes)
	{
		return LocalScopes.TryGetValue(method.MetadataToken.RID, out scopes);
	}

	public void SetLocalScopes(uint method_rid, Collection<Row<uint, Range, Range, uint, uint, uint>> records)
	{
		LocalScopes[method_rid] = records;
	}

	public ImportDebugInformation GetImportScope(uint rid)
	{
		if (rid < 1 || rid > ImportScopes.Length)
		{
			return null;
		}
		return ImportScopes[rid - 1];
	}

	public bool TryGetStateMachineKickOffMethod(MethodDefinition method, out uint rid)
	{
		return StateMachineMethods.TryGetValue(method.MetadataToken.RID, out rid);
	}

	public TypeDefinition GetFieldDeclaringType(uint field_rid)
	{
		return BinaryRangeSearch(Types, field_rid, field: true);
	}

	public TypeDefinition GetMethodDeclaringType(uint method_rid)
	{
		return BinaryRangeSearch(Types, method_rid, field: false);
	}

	private static TypeDefinition BinaryRangeSearch(TypeDefinition[] types, uint rid, bool field)
	{
		int num = 0;
		int num2 = types.Length - 1;
		while (num <= num2)
		{
			int num3 = num + (num2 - num) / 2;
			TypeDefinition typeDefinition = types[num3];
			Range range = (field ? typeDefinition.fields_range : typeDefinition.methods_range);
			if (rid < range.Start)
			{
				num2 = num3 - 1;
				continue;
			}
			if (rid >= range.Start + range.Length)
			{
				num = num3 + 1;
				continue;
			}
			return typeDefinition;
		}
		return null;
	}
}

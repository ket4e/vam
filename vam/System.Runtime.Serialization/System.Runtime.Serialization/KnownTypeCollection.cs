using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace System.Runtime.Serialization;

internal sealed class KnownTypeCollection : Collection<Type>
{
	internal const string MSSimpleNamespace = "http://schemas.microsoft.com/2003/10/Serialization/";

	internal const string MSArraysNamespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";

	internal const string DefaultClrNamespaceBase = "http://schemas.datacontract.org/2004/07/";

	private static XmlQualifiedName any_type;

	private static XmlQualifiedName bool_type;

	private static XmlQualifiedName byte_type;

	private static XmlQualifiedName date_type;

	private static XmlQualifiedName decimal_type;

	private static XmlQualifiedName double_type;

	private static XmlQualifiedName float_type;

	private static XmlQualifiedName string_type;

	private static XmlQualifiedName short_type;

	private static XmlQualifiedName int_type;

	private static XmlQualifiedName long_type;

	private static XmlQualifiedName ubyte_type;

	private static XmlQualifiedName ushort_type;

	private static XmlQualifiedName uint_type;

	private static XmlQualifiedName ulong_type;

	private static XmlQualifiedName any_uri_type;

	private static XmlQualifiedName base64_type;

	private static XmlQualifiedName duration_type;

	private static XmlQualifiedName qname_type;

	private static XmlQualifiedName char_type;

	private static XmlQualifiedName guid_type;

	private static XmlQualifiedName dbnull_type;

	private List<SerializationMap> contracts = new List<SerializationMap>();

	static KnownTypeCollection()
	{
		string ns = "http://schemas.microsoft.com/2003/10/Serialization/";
		any_type = new XmlQualifiedName("anyType", ns);
		any_uri_type = new XmlQualifiedName("anyURI", ns);
		bool_type = new XmlQualifiedName("boolean", ns);
		base64_type = new XmlQualifiedName("base64Binary", ns);
		date_type = new XmlQualifiedName("dateTime", ns);
		duration_type = new XmlQualifiedName("duration", ns);
		qname_type = new XmlQualifiedName("QName", ns);
		decimal_type = new XmlQualifiedName("decimal", ns);
		double_type = new XmlQualifiedName("double", ns);
		float_type = new XmlQualifiedName("float", ns);
		byte_type = new XmlQualifiedName("byte", ns);
		short_type = new XmlQualifiedName("short", ns);
		int_type = new XmlQualifiedName("int", ns);
		long_type = new XmlQualifiedName("long", ns);
		ubyte_type = new XmlQualifiedName("unsignedByte", ns);
		ushort_type = new XmlQualifiedName("unsignedShort", ns);
		uint_type = new XmlQualifiedName("unsignedInt", ns);
		ulong_type = new XmlQualifiedName("unsignedLong", ns);
		string_type = new XmlQualifiedName("string", ns);
		guid_type = new XmlQualifiedName("guid", ns);
		char_type = new XmlQualifiedName("char", ns);
		dbnull_type = new XmlQualifiedName("DBNull", "http://schemas.microsoft.com/2003/10/Serialization/System");
	}

	internal XmlQualifiedName GetXmlName(Type type)
	{
		SerializationMap serializationMap = FindUserMap(type);
		if (serializationMap != null)
		{
			return serializationMap.XmlName;
		}
		return GetPredefinedTypeName(type);
	}

	internal static XmlQualifiedName GetPredefinedTypeName(Type type)
	{
		XmlQualifiedName primitiveTypeName = GetPrimitiveTypeName(type);
		if (primitiveTypeName != XmlQualifiedName.Empty)
		{
			return primitiveTypeName;
		}
		if (type == typeof(DBNull))
		{
			return dbnull_type;
		}
		return XmlQualifiedName.Empty;
	}

	internal static XmlQualifiedName GetPrimitiveTypeName(Type type)
	{
		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
		{
			return GetPrimitiveTypeName(type.GetGenericArguments()[0]);
		}
		if (type.IsEnum)
		{
			return XmlQualifiedName.Empty;
		}
		switch (Type.GetTypeCode(type))
		{
		default:
			if (type == typeof(object))
			{
				return any_type;
			}
			if (type == typeof(Guid))
			{
				return guid_type;
			}
			if (type == typeof(TimeSpan))
			{
				return duration_type;
			}
			if (type == typeof(byte[]))
			{
				return base64_type;
			}
			if (type == typeof(Uri))
			{
				return any_uri_type;
			}
			return XmlQualifiedName.Empty;
		case TypeCode.Boolean:
			return bool_type;
		case TypeCode.Byte:
			return ubyte_type;
		case TypeCode.Char:
			return char_type;
		case TypeCode.DateTime:
			return date_type;
		case TypeCode.Decimal:
			return decimal_type;
		case TypeCode.Double:
			return double_type;
		case TypeCode.Int16:
			return short_type;
		case TypeCode.Int32:
			return int_type;
		case TypeCode.Int64:
			return long_type;
		case TypeCode.SByte:
			return byte_type;
		case TypeCode.Single:
			return float_type;
		case TypeCode.String:
			return string_type;
		case TypeCode.UInt16:
			return ushort_type;
		case TypeCode.UInt32:
			return uint_type;
		case TypeCode.UInt64:
			return ulong_type;
		}
	}

	internal static string PredefinedTypeObjectToString(object obj)
	{
		Type type = obj.GetType();
		switch (Type.GetTypeCode(type))
		{
		default:
			if (type == typeof(object))
			{
				return string.Empty;
			}
			if (type == typeof(Guid))
			{
				return XmlConvert.ToString((Guid)obj);
			}
			if (type == typeof(TimeSpan))
			{
				return XmlConvert.ToString((TimeSpan)obj);
			}
			if (type == typeof(byte[]))
			{
				return Convert.ToBase64String((byte[])obj);
			}
			if (type == typeof(Uri))
			{
				return ((Uri)obj).ToString();
			}
			throw new Exception("Internal error: missing predefined type serialization for type " + type.FullName);
		case TypeCode.DBNull:
			return string.Empty;
		case TypeCode.Boolean:
			return XmlConvert.ToString((bool)obj);
		case TypeCode.Byte:
			return XmlConvert.ToString((int)(byte)obj);
		case TypeCode.Char:
			return XmlConvert.ToString((uint)(char)obj);
		case TypeCode.DateTime:
			return XmlConvert.ToString((DateTime)obj, XmlDateTimeSerializationMode.RoundtripKind);
		case TypeCode.Decimal:
			return XmlConvert.ToString((decimal)obj);
		case TypeCode.Double:
			return XmlConvert.ToString((double)obj);
		case TypeCode.Int16:
			return XmlConvert.ToString((short)obj);
		case TypeCode.Int32:
			return XmlConvert.ToString((int)obj);
		case TypeCode.Int64:
			return XmlConvert.ToString((long)obj);
		case TypeCode.SByte:
			return XmlConvert.ToString((sbyte)obj);
		case TypeCode.Single:
			return XmlConvert.ToString((float)obj);
		case TypeCode.String:
			return (string)obj;
		case TypeCode.UInt16:
			return XmlConvert.ToString((int)(ushort)obj);
		case TypeCode.UInt32:
			return XmlConvert.ToString((uint)obj);
		case TypeCode.UInt64:
			return XmlConvert.ToString((ulong)obj);
		}
	}

	internal static Type GetPrimitiveTypeFromName(string name)
	{
		return name switch
		{
			"anyURI" => typeof(Uri), 
			"boolean" => typeof(bool), 
			"base64Binary" => typeof(byte[]), 
			"dateTime" => typeof(DateTime), 
			"duration" => typeof(TimeSpan), 
			"QName" => typeof(XmlQualifiedName), 
			"decimal" => typeof(decimal), 
			"double" => typeof(double), 
			"float" => typeof(float), 
			"byte" => typeof(sbyte), 
			"short" => typeof(short), 
			"int" => typeof(int), 
			"long" => typeof(long), 
			"unsignedByte" => typeof(byte), 
			"unsignedShort" => typeof(ushort), 
			"unsignedInt" => typeof(uint), 
			"unsignedLong" => typeof(ulong), 
			"string" => typeof(string), 
			"anyType" => typeof(object), 
			"guid" => typeof(Guid), 
			"char" => typeof(char), 
			_ => null, 
		};
	}

	internal static object PredefinedTypeStringToObject(string s, string name, XmlReader reader)
	{
		switch (name)
		{
		case "anyURI":
			return new Uri(s, UriKind.RelativeOrAbsolute);
		case "boolean":
			return XmlConvert.ToBoolean(s);
		case "base64Binary":
			return Convert.FromBase64String(s);
		case "dateTime":
			return XmlConvert.ToDateTime(s, XmlDateTimeSerializationMode.RoundtripKind);
		case "duration":
			return XmlConvert.ToTimeSpan(s);
		case "QName":
		{
			int num = s.IndexOf(':');
			string name2 = ((num >= 0) ? s.Substring(num + 1) : s);
			return (num >= 0) ? new XmlQualifiedName(name2, reader.LookupNamespace(s.Substring(0, num))) : new XmlQualifiedName(name2);
		}
		case "decimal":
			return XmlConvert.ToDecimal(s);
		case "double":
			return XmlConvert.ToDouble(s);
		case "float":
			return XmlConvert.ToSingle(s);
		case "byte":
			return XmlConvert.ToSByte(s);
		case "short":
			return XmlConvert.ToInt16(s);
		case "int":
			return XmlConvert.ToInt32(s);
		case "long":
			return XmlConvert.ToInt64(s);
		case "unsignedByte":
			return XmlConvert.ToByte(s);
		case "unsignedShort":
			return XmlConvert.ToUInt16(s);
		case "unsignedInt":
			return XmlConvert.ToUInt32(s);
		case "unsignedLong":
			return XmlConvert.ToUInt64(s);
		case "string":
			return s;
		case "guid":
			return XmlConvert.ToGuid(s);
		case "anyType":
			return s;
		case "char":
			return (char)XmlConvert.ToUInt32(s);
		default:
			throw new Exception("Unanticipated primitive type: " + name);
		}
	}

	protected override void ClearItems()
	{
		Clear();
	}

	protected override void InsertItem(int index, Type type)
	{
		if (TryRegister(type))
		{
			base.InsertItem(index, type);
		}
	}

	protected override void RemoveItem(int index)
	{
		Type type = base[index];
		List<SerializationMap> list = new List<SerializationMap>();
		foreach (SerializationMap contract in contracts)
		{
			if (contract.RuntimeType == type)
			{
				list.Add(contract);
			}
		}
		foreach (SerializationMap item in list)
		{
			contracts.Remove(item);
			base.RemoveItem(index);
		}
	}

	protected override void SetItem(int index, Type type)
	{
		if (index == Count)
		{
			InsertItem(index, type);
			return;
		}
		RemoveItem(index);
		if (TryRegister(type))
		{
			base.InsertItem(index - 1, type);
		}
	}

	internal SerializationMap FindUserMap(XmlQualifiedName qname)
	{
		for (int i = 0; i < contracts.Count; i++)
		{
			if (qname == contracts[i].XmlName)
			{
				return contracts[i];
			}
		}
		return null;
	}

	internal Type GetSerializedType(Type type)
	{
		Type collectionElementType = GetCollectionElementType(type);
		if (collectionElementType == null)
		{
			return type;
		}
		XmlQualifiedName qName = GetQName(type);
		SerializationMap serializationMap = FindUserMap(qName);
		if (serializationMap != null)
		{
			return serializationMap.RuntimeType;
		}
		return type;
	}

	internal SerializationMap FindUserMap(Type type)
	{
		for (int i = 0; i < contracts.Count; i++)
		{
			if (type == contracts[i].RuntimeType)
			{
				return contracts[i];
			}
		}
		return null;
	}

	internal XmlQualifiedName GetQName(Type type)
	{
		if (IsPrimitiveNotEnum(type))
		{
			return GetPrimitiveTypeName(type);
		}
		SerializationMap serializationMap = FindUserMap(type);
		if (serializationMap != null)
		{
			return serializationMap.XmlName;
		}
		if (type.IsEnum)
		{
			return GetEnumQName(type);
		}
		XmlQualifiedName contractQName = GetContractQName(type);
		if (contractQName != null)
		{
			return contractQName;
		}
		if (type.GetInterface("System.Xml.Serialization.IXmlSerializable") != null)
		{
			return GetSerializableQName(type);
		}
		contractQName = GetCollectionContractQName(type);
		if (contractQName != null)
		{
			return contractQName;
		}
		Type collectionElementType = GetCollectionElementType(type);
		if (collectionElementType != null)
		{
			return GetCollectionQName(collectionElementType);
		}
		if (GetAttribute<SerializableAttribute>(type) != null)
		{
			return GetSerializableQName(type);
		}
		return XmlQualifiedName.Empty;
	}

	private XmlQualifiedName GetContractQName(Type type)
	{
		DataContractAttribute attribute = GetAttribute<DataContractAttribute>(type);
		return (attribute != null) ? GetContractQName(type, attribute.Name, attribute.Namespace) : null;
	}

	private XmlQualifiedName GetCollectionContractQName(Type type)
	{
		CollectionDataContractAttribute attribute = GetAttribute<CollectionDataContractAttribute>(type);
		return (attribute != null) ? GetContractQName(type, attribute.Name, attribute.Namespace) : null;
	}

	internal static XmlQualifiedName GetContractQName(Type type, string name, string ns)
	{
		if (name == null)
		{
			name = ((type.Namespace != null && type.Namespace.Length != 0) ? type.FullName.Substring(type.Namespace.Length + 1).Replace('+', '.') : type.Name);
			if (type.IsGenericType)
			{
				name = name.Substring(0, name.IndexOf('`')) + "Of";
				Type[] genericArguments = type.GetGenericArguments();
				foreach (Type type2 in genericArguments)
				{
					name += type2.Name;
				}
			}
		}
		if (ns == null)
		{
			ns = "http://schemas.datacontract.org/2004/07/" + type.Namespace;
		}
		return new XmlQualifiedName(name, ns);
	}

	private XmlQualifiedName GetEnumQName(Type type)
	{
		string text = null;
		string text2 = null;
		if (!type.IsEnum)
		{
			return null;
		}
		DataContractAttribute attribute = GetAttribute<DataContractAttribute>(type);
		if (attribute != null)
		{
			text2 = attribute.Namespace;
			text = attribute.Name;
		}
		if (text2 == null)
		{
			text2 = "http://schemas.datacontract.org/2004/07/" + type.Namespace;
		}
		if (text == null)
		{
			text = ((type.Namespace != null) ? type.FullName.Substring(type.Namespace.Length + 1).Replace('+', '.') : type.Name);
		}
		return new XmlQualifiedName(text, text2);
	}

	private XmlQualifiedName GetCollectionQName(Type element)
	{
		XmlQualifiedName qName = GetQName(element);
		string ns = qName.Namespace;
		if (qName.Namespace == "http://schemas.microsoft.com/2003/10/Serialization/")
		{
			ns = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";
		}
		return new XmlQualifiedName("ArrayOf" + XmlConvert.EncodeLocalName(qName.Name), ns);
	}

	private XmlQualifiedName GetSerializableQName(Type type)
	{
		string text = type.Name;
		if (type.IsGenericType)
		{
			text = text.Substring(0, text.IndexOf('`')) + "Of";
			Type[] genericArguments = type.GetGenericArguments();
			foreach (Type type2 in genericArguments)
			{
				text += GetQName(type2).Name;
			}
		}
		string ns = "http://schemas.datacontract.org/2004/07/" + type.Namespace;
		XmlRootAttribute attribute = GetAttribute<XmlRootAttribute>(type);
		if (attribute != null)
		{
			text = attribute.ElementName;
			ns = attribute.Namespace;
		}
		return new XmlQualifiedName(XmlConvert.EncodeLocalName(text), ns);
	}

	internal bool IsPrimitiveNotEnum(Type type)
	{
		if (type.IsEnum)
		{
			return false;
		}
		if (Type.GetTypeCode(type) != TypeCode.Object)
		{
			return true;
		}
		if (type == typeof(Guid) || type == typeof(object) || type == typeof(TimeSpan) || type == typeof(byte[]) || type == typeof(Uri))
		{
			return true;
		}
		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
		{
			return IsPrimitiveNotEnum(type.GetGenericArguments()[0]);
		}
		return false;
	}

	internal bool TryRegister(Type type)
	{
		if (IsPrimitiveNotEnum(type))
		{
			return false;
		}
		if (FindUserMap(type) != null)
		{
			return false;
		}
		if (RegisterEnum(type) != null)
		{
			return true;
		}
		if (RegisterContract(type) != null)
		{
			return true;
		}
		if (RegisterIXmlSerializable(type) != null)
		{
			return true;
		}
		if (RegisterDictionary(type) != null)
		{
			return true;
		}
		if (RegisterCollectionContract(type) != null)
		{
			return true;
		}
		if (RegisterCollection(type) != null)
		{
			return true;
		}
		if (GetAttribute<SerializableAttribute>(type) != null)
		{
			RegisterSerializable(type);
			return true;
		}
		RegisterDefaultTypeMap(type);
		return true;
	}

	internal static Type GetCollectionElementType(Type type)
	{
		if (type.IsArray)
		{
			return type.GetElementType();
		}
		Type[] interfaces = type.GetInterfaces();
		Type[] array = interfaces;
		foreach (Type type2 in array)
		{
			if (type2.IsGenericType && type2.GetGenericTypeDefinition().Equals(typeof(ICollection<>)))
			{
				return type2.GetGenericArguments()[0];
			}
		}
		Type[] array2 = interfaces;
		foreach (Type type3 in array2)
		{
			if (type3 == typeof(IList))
			{
				return typeof(object);
			}
		}
		return null;
	}

	internal T GetAttribute<T>(MemberInfo mi) where T : Attribute
	{
		object[] customAttributes = mi.GetCustomAttributes(typeof(T), inherit: false);
		return (customAttributes.Length != 0) ? ((T)customAttributes[0]) : ((T)null);
	}

	private CollectionContractTypeMap RegisterCollectionContract(Type type)
	{
		CollectionDataContractAttribute attribute = GetAttribute<CollectionDataContractAttribute>(type);
		if (attribute == null)
		{
			return null;
		}
		Type collectionElementType = GetCollectionElementType(type);
		if (collectionElementType == null)
		{
			throw new InvalidOperationException($"Type '{type}' is marked as collection contract, but it is not a collection");
		}
		TryRegister(collectionElementType);
		XmlQualifiedName collectionContractQName = GetCollectionContractQName(type);
		CheckStandardQName(collectionContractQName);
		if (FindUserMap(collectionContractQName) != null)
		{
			throw new InvalidOperationException($"Failed to add type {type} to known type collection. There already is a registered type for XML name {collectionContractQName}");
		}
		CollectionContractTypeMap collectionContractTypeMap = new CollectionContractTypeMap(type, attribute, collectionElementType, collectionContractQName, this);
		contracts.Add(collectionContractTypeMap);
		return collectionContractTypeMap;
	}

	private CollectionTypeMap RegisterCollection(Type type)
	{
		Type collectionElementType = GetCollectionElementType(type);
		if (collectionElementType == null)
		{
			return null;
		}
		TryRegister(collectionElementType);
		XmlQualifiedName collectionQName = GetCollectionQName(collectionElementType);
		SerializationMap serializationMap = FindUserMap(collectionQName);
		if (serializationMap != null)
		{
			if (!(serializationMap is CollectionTypeMap collectionTypeMap) || collectionTypeMap.RuntimeType != type)
			{
				throw new InvalidOperationException($"Failed to add type {type} to known type collection. There already is a registered type for XML name {collectionQName}");
			}
			return collectionTypeMap;
		}
		CollectionTypeMap collectionTypeMap2 = new CollectionTypeMap(type, collectionElementType, collectionQName, this);
		contracts.Add(collectionTypeMap2);
		return collectionTypeMap2;
	}

	private static bool TypeImplementsIDictionary(Type type)
	{
		Type[] interfaces = type.GetInterfaces();
		foreach (Type type2 in interfaces)
		{
			if (type2 == typeof(IDictionary) || (type2.IsGenericType && type2.GetGenericTypeDefinition() == typeof(IDictionary<, >)))
			{
				return true;
			}
		}
		return false;
	}

	private DictionaryTypeMap RegisterDictionary(Type type)
	{
		if (!TypeImplementsIDictionary(type))
		{
			return null;
		}
		CollectionDataContractAttribute attribute = GetAttribute<CollectionDataContractAttribute>(type);
		DictionaryTypeMap dictionaryTypeMap = new DictionaryTypeMap(type, attribute, this);
		if (FindUserMap(dictionaryTypeMap.XmlName) != null)
		{
			throw new InvalidOperationException($"Failed to add type {type} to known type collection. There already is a registered type for XML name {dictionaryTypeMap.XmlName}");
		}
		contracts.Add(dictionaryTypeMap);
		TryRegister(dictionaryTypeMap.KeyType);
		TryRegister(dictionaryTypeMap.ValueType);
		return dictionaryTypeMap;
	}

	private SerializationMap RegisterSerializable(Type type)
	{
		XmlQualifiedName serializableQName = GetSerializableQName(type);
		if (FindUserMap(serializableQName) != null)
		{
			throw new InvalidOperationException($"There is already a registered type for XML name {serializableQName}");
		}
		SharedTypeMap sharedTypeMap = new SharedTypeMap(type, serializableQName, this);
		contracts.Add(sharedTypeMap);
		return sharedTypeMap;
	}

	private SerializationMap RegisterIXmlSerializable(Type type)
	{
		if (type.GetInterface("System.Xml.Serialization.IXmlSerializable") == null)
		{
			return null;
		}
		XmlQualifiedName serializableQName = GetSerializableQName(type);
		if (FindUserMap(serializableQName) != null)
		{
			throw new InvalidOperationException($"There is already a registered type for XML name {serializableQName}");
		}
		XmlSerializableMap xmlSerializableMap = new XmlSerializableMap(type, serializableQName, this);
		contracts.Add(xmlSerializableMap);
		return xmlSerializableMap;
	}

	private void CheckStandardQName(XmlQualifiedName qname)
	{
		switch (qname.Namespace)
		{
		case "http://www.w3.org/2001/XMLSchema":
		case "http://www.w3.org/2001/XMLSchema-instance":
		case "http://schemas.microsoft.com/2003/10/Serialization/":
		case "http://schemas.microsoft.com/2003/10/Serialization/Arrays":
			throw new InvalidOperationException($"Namespace {qname.Namespace} is reserved and cannot be used for user serialization");
		}
	}

	private SharedContractMap RegisterContract(Type type)
	{
		XmlQualifiedName contractQName = GetContractQName(type);
		if (contractQName == null)
		{
			return null;
		}
		CheckStandardQName(contractQName);
		if (FindUserMap(contractQName) != null)
		{
			throw new InvalidOperationException($"There is already a registered type for XML name {contractQName}");
		}
		SharedContractMap sharedContractMap = new SharedContractMap(type, contractQName, this);
		contracts.Add(sharedContractMap);
		sharedContractMap.Initialize();
		object[] customAttributes = type.GetCustomAttributes(typeof(KnownTypeAttribute), inherit: true);
		for (int i = 0; i < customAttributes.Length; i++)
		{
			KnownTypeAttribute knownTypeAttribute = (KnownTypeAttribute)customAttributes[i];
			TryRegister(knownTypeAttribute.Type);
		}
		return sharedContractMap;
	}

	private DefaultTypeMap RegisterDefaultTypeMap(Type type)
	{
		DefaultTypeMap defaultTypeMap = new DefaultTypeMap(type, this);
		contracts.Add(defaultTypeMap);
		return defaultTypeMap;
	}

	private EnumMap RegisterEnum(Type type)
	{
		XmlQualifiedName enumQName = GetEnumQName(type);
		if (enumQName == null)
		{
			return null;
		}
		if (FindUserMap(enumQName) != null)
		{
			throw new InvalidOperationException($"There is already a registered type for XML name {enumQName}");
		}
		EnumMap enumMap = new EnumMap(type, enumQName, this);
		contracts.Add(enumMap);
		return enumMap;
	}
}

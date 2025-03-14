using System.Collections;
using System.Xml.XPath;

namespace System.Xml.Schema;

[System.MonoTODO]
public sealed class XmlAtomicValue : XPathItem, ICloneable
{
	private bool booleanValue;

	private DateTime dateTimeValue;

	private decimal decimalValue;

	private double doubleValue;

	private int intValue;

	private long longValue;

	private object objectValue;

	private float floatValue;

	private string stringValue;

	private XmlSchemaType schemaType;

	private XmlTypeCode xmlTypeCode;

	public override bool IsNode => false;

	internal XmlTypeCode ResolvedTypeCode
	{
		get
		{
			if (schemaType != XmlSchemaComplexType.AnyType)
			{
				return schemaType.TypeCode;
			}
			return xmlTypeCode;
		}
	}

	public override object TypedValue
	{
		get
		{
			switch (ResolvedTypeCode)
			{
			case XmlTypeCode.Boolean:
				return ValueAsBoolean;
			case XmlTypeCode.DateTime:
				return ValueAsDateTime;
			case XmlTypeCode.Float:
			case XmlTypeCode.Double:
				return ValueAsDouble;
			case XmlTypeCode.Long:
				return ValueAsLong;
			case XmlTypeCode.Int:
				return ValueAsInt;
			case XmlTypeCode.String:
				return Value;
			default:
				return objectValue;
			}
		}
	}

	public override string Value
	{
		get
		{
			switch (ResolvedTypeCode)
			{
			case XmlTypeCode.Boolean:
				stringValue = XQueryConvert.BooleanToString(ValueAsBoolean);
				break;
			case XmlTypeCode.DateTime:
				stringValue = XQueryConvert.DateTimeToString(ValueAsDateTime);
				break;
			case XmlTypeCode.Float:
			case XmlTypeCode.Double:
				stringValue = XQueryConvert.DoubleToString(ValueAsDouble);
				break;
			case XmlTypeCode.NonPositiveInteger:
			case XmlTypeCode.NegativeInteger:
			case XmlTypeCode.Long:
			case XmlTypeCode.NonNegativeInteger:
			case XmlTypeCode.UnsignedLong:
			case XmlTypeCode.PositiveInteger:
				stringValue = XQueryConvert.IntegerToString(ValueAsLong);
				break;
			case XmlTypeCode.Int:
			case XmlTypeCode.Short:
			case XmlTypeCode.Byte:
			case XmlTypeCode.UnsignedInt:
			case XmlTypeCode.UnsignedShort:
			case XmlTypeCode.UnsignedByte:
				stringValue = XQueryConvert.IntToString(ValueAsInt);
				break;
			case XmlTypeCode.String:
				return stringValue;
			case XmlTypeCode.None:
			case XmlTypeCode.Item:
			case XmlTypeCode.AnyAtomicType:
				switch (XmlTypeCodeFromRuntimeType(objectValue.GetType(), raiseError: false))
				{
				case XmlTypeCode.String:
					stringValue = (string)objectValue;
					break;
				case XmlTypeCode.DateTime:
					stringValue = XQueryConvert.DateTimeToString((DateTime)objectValue);
					break;
				case XmlTypeCode.Boolean:
					stringValue = XQueryConvert.BooleanToString((bool)objectValue);
					break;
				case XmlTypeCode.Float:
					stringValue = XQueryConvert.FloatToString((float)objectValue);
					break;
				case XmlTypeCode.Double:
					stringValue = XQueryConvert.DoubleToString((double)objectValue);
					break;
				case XmlTypeCode.Decimal:
					stringValue = XQueryConvert.DecimalToString((decimal)objectValue);
					break;
				case XmlTypeCode.Long:
					stringValue = XQueryConvert.IntegerToString((long)objectValue);
					break;
				case XmlTypeCode.Int:
					stringValue = XQueryConvert.IntToString((int)objectValue);
					break;
				}
				break;
			}
			if (stringValue != null)
			{
				return stringValue;
			}
			if (objectValue != null)
			{
				throw new InvalidCastException($"Conversion from runtime type {objectValue.GetType()} to {XmlTypeCode.String} is not supported");
			}
			throw new InvalidCastException($"Conversion from schema type {schemaType.QualifiedName} (type code {xmlTypeCode}, resolved type code {ResolvedTypeCode}) to {XmlTypeCode.String} is not supported.");
		}
	}

	public override bool ValueAsBoolean
	{
		get
		{
			switch (xmlTypeCode)
			{
			case XmlTypeCode.Boolean:
				return booleanValue;
			case XmlTypeCode.Decimal:
				return XQueryConvert.DecimalToBoolean(decimalValue);
			case XmlTypeCode.Double:
				return XQueryConvert.DoubleToBoolean(doubleValue);
			case XmlTypeCode.Long:
				return XQueryConvert.IntegerToBoolean(longValue);
			case XmlTypeCode.Int:
				return XQueryConvert.IntToBoolean(intValue);
			case XmlTypeCode.Float:
				return XQueryConvert.FloatToBoolean(floatValue);
			case XmlTypeCode.String:
				return XQueryConvert.StringToBoolean(stringValue);
			case XmlTypeCode.None:
			case XmlTypeCode.Item:
			case XmlTypeCode.AnyAtomicType:
				if (objectValue is bool)
				{
					return (bool)objectValue;
				}
				break;
			}
			throw new InvalidCastException($"Conversion from {schemaType.QualifiedName} to {XmlSchemaSimpleType.XsBoolean.QualifiedName} is not supported");
		}
	}

	public override DateTime ValueAsDateTime
	{
		get
		{
			switch (xmlTypeCode)
			{
			case XmlTypeCode.DateTime:
				return dateTimeValue;
			case XmlTypeCode.String:
				return XQueryConvert.StringToDateTime(stringValue);
			case XmlTypeCode.None:
			case XmlTypeCode.Item:
			case XmlTypeCode.AnyAtomicType:
				if (objectValue is DateTime)
				{
					return (DateTime)objectValue;
				}
				break;
			}
			throw new InvalidCastException($"Conversion from {schemaType.QualifiedName} to {XmlSchemaSimpleType.XsDateTime.QualifiedName} is not supported");
		}
	}

	public override double ValueAsDouble
	{
		get
		{
			switch (xmlTypeCode)
			{
			case XmlTypeCode.Boolean:
				return XQueryConvert.BooleanToDouble(booleanValue);
			case XmlTypeCode.Decimal:
				return XQueryConvert.DecimalToDouble(decimalValue);
			case XmlTypeCode.Double:
				return doubleValue;
			case XmlTypeCode.Long:
				return XQueryConvert.IntegerToDouble(longValue);
			case XmlTypeCode.Int:
				return XQueryConvert.IntToDouble(intValue);
			case XmlTypeCode.Float:
				return XQueryConvert.FloatToDouble(floatValue);
			case XmlTypeCode.String:
				return XQueryConvert.StringToDouble(stringValue);
			case XmlTypeCode.None:
			case XmlTypeCode.Item:
			case XmlTypeCode.AnyAtomicType:
				if (objectValue is double)
				{
					return (double)objectValue;
				}
				break;
			}
			throw new InvalidCastException($"Conversion from {schemaType.QualifiedName} to {XmlSchemaSimpleType.XsDouble.QualifiedName} is not supported");
		}
	}

	public override int ValueAsInt
	{
		get
		{
			switch (xmlTypeCode)
			{
			case XmlTypeCode.Boolean:
				return XQueryConvert.BooleanToInt(booleanValue);
			case XmlTypeCode.Decimal:
				return XQueryConvert.DecimalToInt(decimalValue);
			case XmlTypeCode.Double:
				return XQueryConvert.DoubleToInt(doubleValue);
			case XmlTypeCode.Long:
				return XQueryConvert.IntegerToInt(longValue);
			case XmlTypeCode.Int:
				return intValue;
			case XmlTypeCode.Float:
				return XQueryConvert.FloatToInt(floatValue);
			case XmlTypeCode.String:
				return XQueryConvert.StringToInt(stringValue);
			case XmlTypeCode.None:
			case XmlTypeCode.Item:
			case XmlTypeCode.AnyAtomicType:
				if (objectValue is int)
				{
					return (int)objectValue;
				}
				break;
			}
			throw new InvalidCastException($"Conversion from {schemaType.QualifiedName} to {XmlSchemaSimpleType.XsInt.QualifiedName} is not supported");
		}
	}

	public override long ValueAsLong
	{
		get
		{
			switch (xmlTypeCode)
			{
			case XmlTypeCode.Boolean:
				return XQueryConvert.BooleanToInteger(booleanValue);
			case XmlTypeCode.Decimal:
				return XQueryConvert.DecimalToInteger(decimalValue);
			case XmlTypeCode.Double:
				return XQueryConvert.DoubleToInteger(doubleValue);
			case XmlTypeCode.Long:
				return longValue;
			case XmlTypeCode.Int:
				return XQueryConvert.IntegerToInt(intValue);
			case XmlTypeCode.Float:
				return XQueryConvert.FloatToInteger(floatValue);
			case XmlTypeCode.String:
				return XQueryConvert.StringToInteger(stringValue);
			case XmlTypeCode.None:
			case XmlTypeCode.Item:
			case XmlTypeCode.AnyAtomicType:
				if (objectValue is long)
				{
					return (long)objectValue;
				}
				break;
			}
			throw new InvalidCastException($"Conversion from {schemaType.QualifiedName} to {XmlSchemaSimpleType.XsLong.QualifiedName} is not supported");
		}
	}

	public override Type ValueType => schemaType.Datatype.ValueType;

	public override XmlSchemaType XmlType => schemaType;

	internal XmlAtomicValue(bool value, XmlSchemaType xmlType)
	{
		Init(value, xmlType);
	}

	internal XmlAtomicValue(DateTime value, XmlSchemaType xmlType)
	{
		Init(value, xmlType);
	}

	internal XmlAtomicValue(decimal value, XmlSchemaType xmlType)
	{
		Init(value, xmlType);
	}

	internal XmlAtomicValue(double value, XmlSchemaType xmlType)
	{
		Init(value, xmlType);
	}

	internal XmlAtomicValue(int value, XmlSchemaType xmlType)
	{
		Init(value, xmlType);
	}

	internal XmlAtomicValue(long value, XmlSchemaType xmlType)
	{
		Init(value, xmlType);
	}

	internal XmlAtomicValue(float value, XmlSchemaType xmlType)
	{
		Init(value, xmlType);
	}

	internal XmlAtomicValue(string value, XmlSchemaType xmlType)
	{
		Init(value, xmlType);
	}

	internal XmlAtomicValue(object value, XmlSchemaType xmlType)
	{
		Init(value, xmlType);
	}

	object ICloneable.Clone()
	{
		return Clone();
	}

	private void Init(bool value, XmlSchemaType xmlType)
	{
		if (xmlType == null)
		{
			throw new ArgumentNullException("xmlType");
		}
		xmlTypeCode = XmlTypeCode.Boolean;
		booleanValue = value;
		schemaType = xmlType;
	}

	private void Init(DateTime value, XmlSchemaType xmlType)
	{
		if (xmlType == null)
		{
			throw new ArgumentNullException("xmlType");
		}
		xmlTypeCode = XmlTypeCode.DateTime;
		dateTimeValue = value;
		schemaType = xmlType;
	}

	private void Init(decimal value, XmlSchemaType xmlType)
	{
		if (xmlType == null)
		{
			throw new ArgumentNullException("xmlType");
		}
		xmlTypeCode = XmlTypeCode.Decimal;
		decimalValue = value;
		schemaType = xmlType;
	}

	private void Init(double value, XmlSchemaType xmlType)
	{
		if (xmlType == null)
		{
			throw new ArgumentNullException("xmlType");
		}
		xmlTypeCode = XmlTypeCode.Double;
		doubleValue = value;
		schemaType = xmlType;
	}

	private void Init(int value, XmlSchemaType xmlType)
	{
		if (xmlType == null)
		{
			throw new ArgumentNullException("xmlType");
		}
		xmlTypeCode = XmlTypeCode.Int;
		intValue = value;
		schemaType = xmlType;
	}

	private void Init(long value, XmlSchemaType xmlType)
	{
		if (xmlType == null)
		{
			throw new ArgumentNullException("xmlType");
		}
		xmlTypeCode = XmlTypeCode.Long;
		longValue = value;
		schemaType = xmlType;
	}

	private void Init(float value, XmlSchemaType xmlType)
	{
		if (xmlType == null)
		{
			throw new ArgumentNullException("xmlType");
		}
		xmlTypeCode = XmlTypeCode.Float;
		floatValue = value;
		schemaType = xmlType;
	}

	private void Init(string value, XmlSchemaType xmlType)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (xmlType == null)
		{
			throw new ArgumentNullException("xmlType");
		}
		xmlTypeCode = XmlTypeCode.String;
		stringValue = value;
		schemaType = xmlType;
	}

	private void Init(object value, XmlSchemaType xmlType)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (xmlType == null)
		{
			throw new ArgumentNullException("xmlType");
		}
		switch (Type.GetTypeCode(value.GetType()))
		{
		case TypeCode.Int16:
		case TypeCode.UInt16:
		case TypeCode.Int32:
			Init((int)value, xmlType);
			return;
		case TypeCode.Decimal:
			Init((decimal)value, xmlType);
			return;
		case TypeCode.Double:
			Init((double)value, xmlType);
			return;
		case TypeCode.Single:
			Init((float)value, xmlType);
			return;
		case TypeCode.UInt32:
		case TypeCode.Int64:
			Init((long)value, xmlType);
			return;
		case TypeCode.String:
			Init((string)value, xmlType);
			return;
		case TypeCode.DateTime:
			Init((DateTime)value, xmlType);
			return;
		case TypeCode.Boolean:
			Init((bool)value, xmlType);
			return;
		}
		if (value is ICollection collection && collection.Count == 1)
		{
			if (collection is IList)
			{
				Init(((IList)collection)[0], xmlType);
				return;
			}
			IEnumerator enumerator = collection.GetEnumerator();
			if (enumerator.MoveNext())
			{
				if (enumerator.Current is DictionaryEntry)
				{
					Init(((DictionaryEntry)enumerator.Current).Value, xmlType);
				}
				else
				{
					Init(enumerator.Current, xmlType);
				}
			}
			return;
		}
		if (value is XmlAtomicValue xmlAtomicValue)
		{
			switch (xmlAtomicValue.xmlTypeCode)
			{
			case XmlTypeCode.Boolean:
				Init(xmlAtomicValue.booleanValue, xmlType);
				return;
			case XmlTypeCode.DateTime:
				Init(xmlAtomicValue.dateTimeValue, xmlType);
				return;
			case XmlTypeCode.Decimal:
				Init(xmlAtomicValue.decimalValue, xmlType);
				return;
			case XmlTypeCode.Double:
				Init(xmlAtomicValue.doubleValue, xmlType);
				return;
			case XmlTypeCode.Int:
				Init(xmlAtomicValue.intValue, xmlType);
				return;
			case XmlTypeCode.Long:
				Init(xmlAtomicValue.longValue, xmlType);
				return;
			case XmlTypeCode.Float:
				Init(xmlAtomicValue.floatValue, xmlType);
				return;
			case XmlTypeCode.String:
				Init(xmlAtomicValue.stringValue, xmlType);
				return;
			}
			objectValue = xmlAtomicValue.objectValue;
		}
		objectValue = value;
		schemaType = xmlType;
	}

	public XmlAtomicValue Clone()
	{
		return new XmlAtomicValue(this, schemaType);
	}

	public override object ValueAs(Type type, IXmlNamespaceResolver nsResolver)
	{
		switch (XmlTypeCodeFromRuntimeType(type, raiseError: false))
		{
		case XmlTypeCode.Int:
		case XmlTypeCode.Short:
		case XmlTypeCode.UnsignedShort:
			return ValueAsInt;
		case XmlTypeCode.Float:
		case XmlTypeCode.Double:
			return ValueAsDouble;
		case XmlTypeCode.Long:
		case XmlTypeCode.UnsignedInt:
			return ValueAsLong;
		case XmlTypeCode.String:
			return Value;
		case XmlTypeCode.DateTime:
			return ValueAsDateTime;
		case XmlTypeCode.Boolean:
			return ValueAsBoolean;
		case XmlTypeCode.Item:
			return TypedValue;
		case XmlTypeCode.QName:
			return XmlQualifiedName.Parse(Value, nsResolver, considerDefaultNamespace: true);
		default:
			throw new NotImplementedException();
		}
	}

	public override string ToString()
	{
		return Value;
	}

	internal static Type RuntimeTypeFromXmlTypeCode(XmlTypeCode typeCode)
	{
		return typeCode switch
		{
			XmlTypeCode.Int => typeof(int), 
			XmlTypeCode.Decimal => typeof(decimal), 
			XmlTypeCode.Double => typeof(double), 
			XmlTypeCode.Float => typeof(float), 
			XmlTypeCode.Long => typeof(long), 
			XmlTypeCode.Short => typeof(short), 
			XmlTypeCode.UnsignedShort => typeof(ushort), 
			XmlTypeCode.UnsignedInt => typeof(uint), 
			XmlTypeCode.String => typeof(string), 
			XmlTypeCode.DateTime => typeof(DateTime), 
			XmlTypeCode.Boolean => typeof(bool), 
			XmlTypeCode.Item => typeof(object), 
			_ => throw new NotSupportedException($"XQuery internal error: Cannot infer Runtime Type from XmlTypeCode {typeCode}."), 
		};
	}

	internal static XmlTypeCode XmlTypeCodeFromRuntimeType(Type cliType, bool raiseError)
	{
		switch (Type.GetTypeCode(cliType))
		{
		case TypeCode.Int32:
			return XmlTypeCode.Int;
		case TypeCode.Decimal:
			return XmlTypeCode.Decimal;
		case TypeCode.Double:
			return XmlTypeCode.Double;
		case TypeCode.Single:
			return XmlTypeCode.Float;
		case TypeCode.Int64:
			return XmlTypeCode.Long;
		case TypeCode.Int16:
			return XmlTypeCode.Short;
		case TypeCode.UInt16:
			return XmlTypeCode.UnsignedShort;
		case TypeCode.UInt32:
			return XmlTypeCode.UnsignedInt;
		case TypeCode.String:
			return XmlTypeCode.String;
		case TypeCode.DateTime:
			return XmlTypeCode.DateTime;
		case TypeCode.Boolean:
			return XmlTypeCode.Boolean;
		case TypeCode.Object:
			return XmlTypeCode.Item;
		default:
			if (raiseError)
			{
				throw new NotSupportedException($"XQuery internal error: Cannot infer XmlTypeCode from Runtime Type {cliType}");
			}
			return XmlTypeCode.None;
		}
	}
}

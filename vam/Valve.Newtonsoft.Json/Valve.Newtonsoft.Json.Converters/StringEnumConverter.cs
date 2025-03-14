using System;
using System.Globalization;
using Valve.Newtonsoft.Json.Utilities;

namespace Valve.Newtonsoft.Json.Converters;

public class StringEnumConverter : JsonConverter
{
	public bool CamelCaseText { get; set; }

	public bool AllowIntegerValues { get; set; }

	public StringEnumConverter()
	{
		AllowIntegerValues = true;
	}

	public StringEnumConverter(bool camelCaseText)
		: this()
	{
		CamelCaseText = camelCaseText;
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		if (value == null)
		{
			writer.WriteNull();
			return;
		}
		Enum @enum = (Enum)value;
		string text = @enum.ToString("G");
		if (char.IsNumber(text[0]) || text[0] == '-')
		{
			writer.WriteValue(value);
			return;
		}
		string value2 = EnumUtils.ToEnumName(@enum.GetType(), text, CamelCaseText);
		writer.WriteValue(value2);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		if (reader.TokenType == JsonToken.Null)
		{
			if (!ReflectionUtils.IsNullableType(objectType))
			{
				throw JsonSerializationException.Create(reader, "Cannot convert null value to {0}.".FormatWith(CultureInfo.InvariantCulture, objectType));
			}
			return null;
		}
		bool flag = ReflectionUtils.IsNullableType(objectType);
		Type type = (flag ? Nullable.GetUnderlyingType(objectType) : objectType);
		try
		{
			if (reader.TokenType == JsonToken.String)
			{
				return EnumUtils.ParseEnumName(reader.Value.ToString(), flag, type);
			}
			if (reader.TokenType == JsonToken.Integer)
			{
				if (!AllowIntegerValues)
				{
					throw JsonSerializationException.Create(reader, "Integer value {0} is not allowed.".FormatWith(CultureInfo.InvariantCulture, reader.Value));
				}
				return ConvertUtils.ConvertOrCast(reader.Value, CultureInfo.InvariantCulture, type);
			}
		}
		catch (Exception ex)
		{
			throw JsonSerializationException.Create(reader, "Error converting value {0} to type '{1}'.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.FormatValueForPrint(reader.Value), objectType), ex);
		}
		throw JsonSerializationException.Create(reader, "Unexpected token {0} when parsing enum.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
	}

	public override bool CanConvert(Type objectType)
	{
		return (ReflectionUtils.IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType).IsEnum();
	}
}

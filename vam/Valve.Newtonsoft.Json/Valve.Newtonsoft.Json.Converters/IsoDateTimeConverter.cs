using System;
using System.Globalization;
using Valve.Newtonsoft.Json.Utilities;

namespace Valve.Newtonsoft.Json.Converters;

public class IsoDateTimeConverter : DateTimeConverterBase
{
	private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

	private DateTimeStyles _dateTimeStyles = DateTimeStyles.RoundtripKind;

	private string _dateTimeFormat;

	private CultureInfo _culture;

	public DateTimeStyles DateTimeStyles
	{
		get
		{
			return _dateTimeStyles;
		}
		set
		{
			_dateTimeStyles = value;
		}
	}

	public string DateTimeFormat
	{
		get
		{
			return _dateTimeFormat ?? string.Empty;
		}
		set
		{
			_dateTimeFormat = (string.IsNullOrEmpty(value) ? null : value);
		}
	}

	public CultureInfo Culture
	{
		get
		{
			return _culture ?? CultureInfo.CurrentCulture;
		}
		set
		{
			_culture = value;
		}
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		string value2;
		if (value is DateTime dateTime)
		{
			if ((_dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal || (_dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
			{
				dateTime = dateTime.ToUniversalTime();
			}
			value2 = dateTime.ToString(_dateTimeFormat ?? "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK", Culture);
		}
		else
		{
			if (!(value is DateTimeOffset dateTimeOffset))
			{
				throw new JsonSerializationException("Unexpected value when converting date. Expected DateTime or DateTimeOffset, got {0}.".FormatWith(CultureInfo.InvariantCulture, ReflectionUtils.GetObjectType(value)));
			}
			if ((_dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal || (_dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
			{
				dateTimeOffset = dateTimeOffset.ToUniversalTime();
			}
			value2 = dateTimeOffset.ToString(_dateTimeFormat ?? "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK", Culture);
		}
		writer.WriteValue(value2);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		bool flag = ReflectionUtils.IsNullableType(objectType);
		Type type = (flag ? Nullable.GetUnderlyingType(objectType) : objectType);
		if (reader.TokenType == JsonToken.Null)
		{
			if (!ReflectionUtils.IsNullableType(objectType))
			{
				throw JsonSerializationException.Create(reader, "Cannot convert null value to {0}.".FormatWith(CultureInfo.InvariantCulture, objectType));
			}
			return null;
		}
		if (reader.TokenType == JsonToken.Date)
		{
			if (type == typeof(DateTimeOffset))
			{
				if (!(reader.Value is DateTimeOffset))
				{
					return new DateTimeOffset((DateTime)reader.Value);
				}
				return reader.Value;
			}
			if (reader.Value is DateTimeOffset)
			{
				return ((DateTimeOffset)reader.Value).DateTime;
			}
			return reader.Value;
		}
		if (reader.TokenType != JsonToken.String)
		{
			throw JsonSerializationException.Create(reader, "Unexpected token parsing date. Expected String, got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
		}
		string text = reader.Value.ToString();
		if (string.IsNullOrEmpty(text) && flag)
		{
			return null;
		}
		if (type == typeof(DateTimeOffset))
		{
			if (!string.IsNullOrEmpty(_dateTimeFormat))
			{
				return DateTimeOffset.ParseExact(text, _dateTimeFormat, Culture, _dateTimeStyles);
			}
			return DateTimeOffset.Parse(text, Culture, _dateTimeStyles);
		}
		if (!string.IsNullOrEmpty(_dateTimeFormat))
		{
			return DateTime.ParseExact(text, _dateTimeFormat, Culture, _dateTimeStyles);
		}
		return DateTime.Parse(text, Culture, _dateTimeStyles);
	}
}

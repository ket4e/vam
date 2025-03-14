using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Valve.Newtonsoft.Json.Linq.JsonPath;
using Valve.Newtonsoft.Json.Utilities;

namespace Valve.Newtonsoft.Json.Linq;

public abstract class JToken : IJEnumerable<JToken>, IEnumerable<JToken>, IEnumerable, IJsonLineInfo, ICloneable
{
	private class LineInfoAnnotation
	{
		internal readonly int LineNumber;

		internal readonly int LinePosition;

		public LineInfoAnnotation(int lineNumber, int linePosition)
		{
			LineNumber = lineNumber;
			LinePosition = linePosition;
		}
	}

	private static JTokenEqualityComparer _equalityComparer;

	private JContainer _parent;

	private JToken _previous;

	private JToken _next;

	private object _annotations;

	private static readonly JTokenType[] BooleanTypes = new JTokenType[6]
	{
		JTokenType.Integer,
		JTokenType.Float,
		JTokenType.String,
		JTokenType.Comment,
		JTokenType.Raw,
		JTokenType.Boolean
	};

	private static readonly JTokenType[] NumberTypes = new JTokenType[6]
	{
		JTokenType.Integer,
		JTokenType.Float,
		JTokenType.String,
		JTokenType.Comment,
		JTokenType.Raw,
		JTokenType.Boolean
	};

	private static readonly JTokenType[] StringTypes = new JTokenType[11]
	{
		JTokenType.Date,
		JTokenType.Integer,
		JTokenType.Float,
		JTokenType.String,
		JTokenType.Comment,
		JTokenType.Raw,
		JTokenType.Boolean,
		JTokenType.Bytes,
		JTokenType.Guid,
		JTokenType.TimeSpan,
		JTokenType.Uri
	};

	private static readonly JTokenType[] GuidTypes = new JTokenType[5]
	{
		JTokenType.String,
		JTokenType.Comment,
		JTokenType.Raw,
		JTokenType.Guid,
		JTokenType.Bytes
	};

	private static readonly JTokenType[] TimeSpanTypes = new JTokenType[4]
	{
		JTokenType.String,
		JTokenType.Comment,
		JTokenType.Raw,
		JTokenType.TimeSpan
	};

	private static readonly JTokenType[] UriTypes = new JTokenType[4]
	{
		JTokenType.String,
		JTokenType.Comment,
		JTokenType.Raw,
		JTokenType.Uri
	};

	private static readonly JTokenType[] CharTypes = new JTokenType[5]
	{
		JTokenType.Integer,
		JTokenType.Float,
		JTokenType.String,
		JTokenType.Comment,
		JTokenType.Raw
	};

	private static readonly JTokenType[] DateTimeTypes = new JTokenType[4]
	{
		JTokenType.Date,
		JTokenType.String,
		JTokenType.Comment,
		JTokenType.Raw
	};

	private static readonly JTokenType[] BytesTypes = new JTokenType[5]
	{
		JTokenType.Bytes,
		JTokenType.String,
		JTokenType.Comment,
		JTokenType.Raw,
		JTokenType.Integer
	};

	public static JTokenEqualityComparer EqualityComparer
	{
		get
		{
			if (_equalityComparer == null)
			{
				_equalityComparer = new JTokenEqualityComparer();
			}
			return _equalityComparer;
		}
	}

	public JContainer Parent
	{
		[DebuggerStepThrough]
		get
		{
			return _parent;
		}
		internal set
		{
			_parent = value;
		}
	}

	public JToken Root
	{
		get
		{
			JContainer parent = Parent;
			if (parent == null)
			{
				return this;
			}
			while (parent.Parent != null)
			{
				parent = parent.Parent;
			}
			return parent;
		}
	}

	public abstract JTokenType Type { get; }

	public abstract bool HasValues { get; }

	public JToken Next
	{
		get
		{
			return _next;
		}
		internal set
		{
			_next = value;
		}
	}

	public JToken Previous
	{
		get
		{
			return _previous;
		}
		internal set
		{
			_previous = value;
		}
	}

	public string Path
	{
		get
		{
			if (Parent == null)
			{
				return string.Empty;
			}
			List<JsonPosition> list = new List<JsonPosition>();
			JToken jToken = null;
			for (JToken jToken2 = this; jToken2 != null; jToken2 = jToken2.Parent)
			{
				switch (jToken2.Type)
				{
				case JTokenType.Property:
				{
					JProperty jProperty = (JProperty)jToken2;
					list.Add(new JsonPosition(JsonContainerType.Object)
					{
						PropertyName = jProperty.Name
					});
					break;
				}
				case JTokenType.Array:
				case JTokenType.Constructor:
					if (jToken != null)
					{
						int position = ((IList<JToken>)jToken2).IndexOf(jToken);
						list.Add(new JsonPosition(JsonContainerType.Array)
						{
							Position = position
						});
					}
					break;
				}
				jToken = jToken2;
			}
			list.Reverse();
			return JsonPosition.BuildPath(list, null);
		}
	}

	public virtual JToken this[object key]
	{
		get
		{
			throw new InvalidOperationException("Cannot access child value on {0}.".FormatWith(CultureInfo.InvariantCulture, GetType()));
		}
		set
		{
			throw new InvalidOperationException("Cannot set child value on {0}.".FormatWith(CultureInfo.InvariantCulture, GetType()));
		}
	}

	public virtual JToken First
	{
		get
		{
			throw new InvalidOperationException("Cannot access child value on {0}.".FormatWith(CultureInfo.InvariantCulture, GetType()));
		}
	}

	public virtual JToken Last
	{
		get
		{
			throw new InvalidOperationException("Cannot access child value on {0}.".FormatWith(CultureInfo.InvariantCulture, GetType()));
		}
	}

	IJEnumerable<JToken> IJEnumerable<JToken>.this[object key] => this[key];

	int IJsonLineInfo.LineNumber => Annotation<LineInfoAnnotation>()?.LineNumber ?? 0;

	int IJsonLineInfo.LinePosition => Annotation<LineInfoAnnotation>()?.LinePosition ?? 0;

	internal abstract JToken CloneToken();

	internal abstract bool DeepEquals(JToken node);

	public static bool DeepEquals(JToken t1, JToken t2)
	{
		if (t1 != t2)
		{
			if (t1 != null && t2 != null)
			{
				return t1.DeepEquals(t2);
			}
			return false;
		}
		return true;
	}

	internal JToken()
	{
	}

	public void AddAfterSelf(object content)
	{
		if (_parent == null)
		{
			throw new InvalidOperationException("The parent is missing.");
		}
		int num = _parent.IndexOfItem(this);
		_parent.AddInternal(num + 1, content, skipParentCheck: false);
	}

	public void AddBeforeSelf(object content)
	{
		if (_parent == null)
		{
			throw new InvalidOperationException("The parent is missing.");
		}
		int index = _parent.IndexOfItem(this);
		_parent.AddInternal(index, content, skipParentCheck: false);
	}

	public IEnumerable<JToken> Ancestors()
	{
		return GetAncestors(self: false);
	}

	public IEnumerable<JToken> AncestorsAndSelf()
	{
		return GetAncestors(self: true);
	}

	internal IEnumerable<JToken> GetAncestors(bool self)
	{
		for (JToken current = (self ? this : Parent); current != null; current = current.Parent)
		{
			yield return current;
		}
	}

	public IEnumerable<JToken> AfterSelf()
	{
		if (Parent != null)
		{
			for (JToken o = Next; o != null; o = o.Next)
			{
				yield return o;
			}
		}
	}

	public IEnumerable<JToken> BeforeSelf()
	{
		for (JToken o = Parent.First; o != this; o = o.Next)
		{
			yield return o;
		}
	}

	public virtual T Value<T>(object key)
	{
		JToken jToken = this[key];
		if (jToken != null)
		{
			return jToken.Convert<JToken, T>();
		}
		return default(T);
	}

	public virtual JEnumerable<JToken> Children()
	{
		return JEnumerable<JToken>.Empty;
	}

	public JEnumerable<T> Children<T>() where T : JToken
	{
		return new JEnumerable<T>(Children().OfType<T>());
	}

	public virtual IEnumerable<T> Values<T>()
	{
		throw new InvalidOperationException("Cannot access child value on {0}.".FormatWith(CultureInfo.InvariantCulture, GetType()));
	}

	public void Remove()
	{
		if (_parent == null)
		{
			throw new InvalidOperationException("The parent is missing.");
		}
		_parent.RemoveItem(this);
	}

	public void Replace(JToken value)
	{
		if (_parent == null)
		{
			throw new InvalidOperationException("The parent is missing.");
		}
		_parent.ReplaceItem(this, value);
	}

	public abstract void WriteTo(JsonWriter writer, params JsonConverter[] converters);

	public override string ToString()
	{
		return ToString(Formatting.Indented);
	}

	public string ToString(Formatting formatting, params JsonConverter[] converters)
	{
		using StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
		JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter);
		jsonTextWriter.Formatting = formatting;
		WriteTo(jsonTextWriter, converters);
		return stringWriter.ToString();
	}

	private static JValue EnsureValue(JToken value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (value is JProperty)
		{
			value = ((JProperty)value).Value;
		}
		return value as JValue;
	}

	private static string GetType(JToken token)
	{
		ValidationUtils.ArgumentNotNull(token, "token");
		if (token is JProperty)
		{
			token = ((JProperty)token).Value;
		}
		return token.Type.ToString();
	}

	private static bool ValidateToken(JToken o, JTokenType[] validTypes, bool nullable)
	{
		if (Array.IndexOf(validTypes, o.Type) == -1)
		{
			if (nullable)
			{
				if (o.Type != JTokenType.Null)
				{
					return o.Type == JTokenType.Undefined;
				}
				return true;
			}
			return false;
		}
		return true;
	}

	public static explicit operator bool(JToken value)
	{
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, BooleanTypes, nullable: false))
		{
			throw new ArgumentException("Can not convert {0} to Boolean.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		return Convert.ToBoolean(jValue.Value, CultureInfo.InvariantCulture);
	}

	public static explicit operator DateTimeOffset(JToken value)
	{
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, DateTimeTypes, nullable: false))
		{
			throw new ArgumentException("Can not convert {0} to DateTimeOffset.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		if (jValue.Value is DateTimeOffset)
		{
			return (DateTimeOffset)jValue.Value;
		}
		if (jValue.Value is string)
		{
			return DateTimeOffset.Parse((string)jValue.Value, CultureInfo.InvariantCulture);
		}
		return new DateTimeOffset(Convert.ToDateTime(jValue.Value, CultureInfo.InvariantCulture));
	}

	public static explicit operator bool?(JToken value)
	{
		if (value == null)
		{
			return null;
		}
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, BooleanTypes, nullable: true))
		{
			throw new ArgumentException("Can not convert {0} to Boolean.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		if (jValue.Value == null)
		{
			return null;
		}
		return Convert.ToBoolean(jValue.Value, CultureInfo.InvariantCulture);
	}

	public static explicit operator long(JToken value)
	{
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, NumberTypes, nullable: false))
		{
			throw new ArgumentException("Can not convert {0} to Int64.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		return Convert.ToInt64(jValue.Value, CultureInfo.InvariantCulture);
	}

	public static explicit operator DateTime?(JToken value)
	{
		if (value == null)
		{
			return null;
		}
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, DateTimeTypes, nullable: true))
		{
			throw new ArgumentException("Can not convert {0} to DateTime.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		if (jValue.Value is DateTimeOffset)
		{
			return ((DateTimeOffset)jValue.Value).DateTime;
		}
		if (jValue.Value == null)
		{
			return null;
		}
		return Convert.ToDateTime(jValue.Value, CultureInfo.InvariantCulture);
	}

	public static explicit operator DateTimeOffset?(JToken value)
	{
		if (value == null)
		{
			return null;
		}
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, DateTimeTypes, nullable: true))
		{
			throw new ArgumentException("Can not convert {0} to DateTimeOffset.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		if (jValue.Value == null)
		{
			return null;
		}
		if (jValue.Value is DateTimeOffset)
		{
			return (DateTimeOffset?)jValue.Value;
		}
		if (jValue.Value is string)
		{
			return DateTimeOffset.Parse((string)jValue.Value, CultureInfo.InvariantCulture);
		}
		return new DateTimeOffset(Convert.ToDateTime(jValue.Value, CultureInfo.InvariantCulture));
	}

	public static explicit operator decimal?(JToken value)
	{
		if (value == null)
		{
			return null;
		}
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, NumberTypes, nullable: true))
		{
			throw new ArgumentException("Can not convert {0} to Decimal.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		if (jValue.Value == null)
		{
			return null;
		}
		return Convert.ToDecimal(jValue.Value, CultureInfo.InvariantCulture);
	}

	public static explicit operator double?(JToken value)
	{
		if (value == null)
		{
			return null;
		}
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, NumberTypes, nullable: true))
		{
			throw new ArgumentException("Can not convert {0} to Double.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		if (jValue.Value == null)
		{
			return null;
		}
		return Convert.ToDouble(jValue.Value, CultureInfo.InvariantCulture);
	}

	public static explicit operator char?(JToken value)
	{
		if (value == null)
		{
			return null;
		}
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, CharTypes, nullable: true))
		{
			throw new ArgumentException("Can not convert {0} to Char.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		if (jValue.Value == null)
		{
			return null;
		}
		return Convert.ToChar(jValue.Value, CultureInfo.InvariantCulture);
	}

	public static explicit operator int(JToken value)
	{
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, NumberTypes, nullable: false))
		{
			throw new ArgumentException("Can not convert {0} to Int32.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		return Convert.ToInt32(jValue.Value, CultureInfo.InvariantCulture);
	}

	public static explicit operator short(JToken value)
	{
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, NumberTypes, nullable: false))
		{
			throw new ArgumentException("Can not convert {0} to Int16.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		return Convert.ToInt16(jValue.Value, CultureInfo.InvariantCulture);
	}

	[CLSCompliant(false)]
	public static explicit operator ushort(JToken value)
	{
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, NumberTypes, nullable: false))
		{
			throw new ArgumentException("Can not convert {0} to UInt16.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		return Convert.ToUInt16(jValue.Value, CultureInfo.InvariantCulture);
	}

	[CLSCompliant(false)]
	public static explicit operator char(JToken value)
	{
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, CharTypes, nullable: false))
		{
			throw new ArgumentException("Can not convert {0} to Char.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		return Convert.ToChar(jValue.Value, CultureInfo.InvariantCulture);
	}

	public static explicit operator byte(JToken value)
	{
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, NumberTypes, nullable: false))
		{
			throw new ArgumentException("Can not convert {0} to Byte.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		return Convert.ToByte(jValue.Value, CultureInfo.InvariantCulture);
	}

	[CLSCompliant(false)]
	public static explicit operator sbyte(JToken value)
	{
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, NumberTypes, nullable: false))
		{
			throw new ArgumentException("Can not convert {0} to SByte.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		return Convert.ToSByte(jValue.Value, CultureInfo.InvariantCulture);
	}

	public static explicit operator int?(JToken value)
	{
		if (value == null)
		{
			return null;
		}
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, NumberTypes, nullable: true))
		{
			throw new ArgumentException("Can not convert {0} to Int32.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		if (jValue.Value == null)
		{
			return null;
		}
		return Convert.ToInt32(jValue.Value, CultureInfo.InvariantCulture);
	}

	public static explicit operator short?(JToken value)
	{
		if (value == null)
		{
			return null;
		}
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, NumberTypes, nullable: true))
		{
			throw new ArgumentException("Can not convert {0} to Int16.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		if (jValue.Value == null)
		{
			return null;
		}
		return Convert.ToInt16(jValue.Value, CultureInfo.InvariantCulture);
	}

	[CLSCompliant(false)]
	public static explicit operator ushort?(JToken value)
	{
		if (value == null)
		{
			return null;
		}
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, NumberTypes, nullable: true))
		{
			throw new ArgumentException("Can not convert {0} to UInt16.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		if (jValue.Value == null)
		{
			return null;
		}
		return Convert.ToUInt16(jValue.Value, CultureInfo.InvariantCulture);
	}

	public static explicit operator byte?(JToken value)
	{
		if (value == null)
		{
			return null;
		}
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, NumberTypes, nullable: true))
		{
			throw new ArgumentException("Can not convert {0} to Byte.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		if (jValue.Value == null)
		{
			return null;
		}
		return Convert.ToByte(jValue.Value, CultureInfo.InvariantCulture);
	}

	[CLSCompliant(false)]
	public static explicit operator sbyte?(JToken value)
	{
		if (value == null)
		{
			return null;
		}
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, NumberTypes, nullable: true))
		{
			throw new ArgumentException("Can not convert {0} to SByte.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		if (jValue.Value == null)
		{
			return null;
		}
		return Convert.ToSByte(jValue.Value, CultureInfo.InvariantCulture);
	}

	public static explicit operator DateTime(JToken value)
	{
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, DateTimeTypes, nullable: false))
		{
			throw new ArgumentException("Can not convert {0} to DateTime.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		if (jValue.Value is DateTimeOffset)
		{
			return ((DateTimeOffset)jValue.Value).DateTime;
		}
		return Convert.ToDateTime(jValue.Value, CultureInfo.InvariantCulture);
	}

	public static explicit operator long?(JToken value)
	{
		if (value == null)
		{
			return null;
		}
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, NumberTypes, nullable: true))
		{
			throw new ArgumentException("Can not convert {0} to Int64.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		if (jValue.Value == null)
		{
			return null;
		}
		return Convert.ToInt64(jValue.Value, CultureInfo.InvariantCulture);
	}

	public static explicit operator float?(JToken value)
	{
		if (value == null)
		{
			return null;
		}
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, NumberTypes, nullable: true))
		{
			throw new ArgumentException("Can not convert {0} to Single.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		if (jValue.Value == null)
		{
			return null;
		}
		return Convert.ToSingle(jValue.Value, CultureInfo.InvariantCulture);
	}

	public static explicit operator decimal(JToken value)
	{
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, NumberTypes, nullable: false))
		{
			throw new ArgumentException("Can not convert {0} to Decimal.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		return Convert.ToDecimal(jValue.Value, CultureInfo.InvariantCulture);
	}

	[CLSCompliant(false)]
	public static explicit operator uint?(JToken value)
	{
		if (value == null)
		{
			return null;
		}
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, NumberTypes, nullable: true))
		{
			throw new ArgumentException("Can not convert {0} to UInt32.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		if (jValue.Value == null)
		{
			return null;
		}
		return Convert.ToUInt32(jValue.Value, CultureInfo.InvariantCulture);
	}

	[CLSCompliant(false)]
	public static explicit operator ulong?(JToken value)
	{
		if (value == null)
		{
			return null;
		}
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, NumberTypes, nullable: true))
		{
			throw new ArgumentException("Can not convert {0} to UInt64.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		if (jValue.Value == null)
		{
			return null;
		}
		return Convert.ToUInt64(jValue.Value, CultureInfo.InvariantCulture);
	}

	public static explicit operator double(JToken value)
	{
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, NumberTypes, nullable: false))
		{
			throw new ArgumentException("Can not convert {0} to Double.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		return Convert.ToDouble(jValue.Value, CultureInfo.InvariantCulture);
	}

	public static explicit operator float(JToken value)
	{
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, NumberTypes, nullable: false))
		{
			throw new ArgumentException("Can not convert {0} to Single.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		return Convert.ToSingle(jValue.Value, CultureInfo.InvariantCulture);
	}

	public static explicit operator string(JToken value)
	{
		if (value == null)
		{
			return null;
		}
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, StringTypes, nullable: true))
		{
			throw new ArgumentException("Can not convert {0} to String.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		if (jValue.Value == null)
		{
			return null;
		}
		if (jValue.Value is byte[])
		{
			return Convert.ToBase64String((byte[])jValue.Value);
		}
		return Convert.ToString(jValue.Value, CultureInfo.InvariantCulture);
	}

	[CLSCompliant(false)]
	public static explicit operator uint(JToken value)
	{
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, NumberTypes, nullable: false))
		{
			throw new ArgumentException("Can not convert {0} to UInt32.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		return Convert.ToUInt32(jValue.Value, CultureInfo.InvariantCulture);
	}

	[CLSCompliant(false)]
	public static explicit operator ulong(JToken value)
	{
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, NumberTypes, nullable: false))
		{
			throw new ArgumentException("Can not convert {0} to UInt64.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		return Convert.ToUInt64(jValue.Value, CultureInfo.InvariantCulture);
	}

	public static explicit operator byte[](JToken value)
	{
		if (value == null)
		{
			return null;
		}
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, BytesTypes, nullable: false))
		{
			throw new ArgumentException("Can not convert {0} to byte array.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		if (jValue.Value is string)
		{
			return Convert.FromBase64String(Convert.ToString(jValue.Value, CultureInfo.InvariantCulture));
		}
		if (jValue.Value is byte[])
		{
			return (byte[])jValue.Value;
		}
		throw new ArgumentException("Can not convert {0} to byte array.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
	}

	public static explicit operator Guid(JToken value)
	{
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, GuidTypes, nullable: false))
		{
			throw new ArgumentException("Can not convert {0} to Guid.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		if (jValue.Value is byte[])
		{
			return new Guid((byte[])jValue.Value);
		}
		if (!(jValue.Value is Guid))
		{
			return new Guid(Convert.ToString(jValue.Value, CultureInfo.InvariantCulture));
		}
		return (Guid)jValue.Value;
	}

	public static explicit operator Guid?(JToken value)
	{
		if (value == null)
		{
			return null;
		}
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, GuidTypes, nullable: true))
		{
			throw new ArgumentException("Can not convert {0} to Guid.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		if (jValue.Value == null)
		{
			return null;
		}
		if (jValue.Value is byte[])
		{
			return new Guid((byte[])jValue.Value);
		}
		return (jValue.Value is Guid) ? ((Guid)jValue.Value) : new Guid(Convert.ToString(jValue.Value, CultureInfo.InvariantCulture));
	}

	public static explicit operator TimeSpan(JToken value)
	{
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, TimeSpanTypes, nullable: false))
		{
			throw new ArgumentException("Can not convert {0} to TimeSpan.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		if (!(jValue.Value is TimeSpan))
		{
			return ConvertUtils.ParseTimeSpan(Convert.ToString(jValue.Value, CultureInfo.InvariantCulture));
		}
		return (TimeSpan)jValue.Value;
	}

	public static explicit operator TimeSpan?(JToken value)
	{
		if (value == null)
		{
			return null;
		}
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, TimeSpanTypes, nullable: true))
		{
			throw new ArgumentException("Can not convert {0} to TimeSpan.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		if (jValue.Value == null)
		{
			return null;
		}
		return (jValue.Value is TimeSpan) ? ((TimeSpan)jValue.Value) : ConvertUtils.ParseTimeSpan(Convert.ToString(jValue.Value, CultureInfo.InvariantCulture));
	}

	public static explicit operator Uri(JToken value)
	{
		if (value == null)
		{
			return null;
		}
		JValue jValue = EnsureValue(value);
		if (jValue == null || !ValidateToken(jValue, UriTypes, nullable: true))
		{
			throw new ArgumentException("Can not convert {0} to Uri.".FormatWith(CultureInfo.InvariantCulture, GetType(value)));
		}
		if (jValue.Value == null)
		{
			return null;
		}
		if (!(jValue.Value is Uri))
		{
			return new Uri(Convert.ToString(jValue.Value, CultureInfo.InvariantCulture));
		}
		return (Uri)jValue.Value;
	}

	public static implicit operator JToken(bool value)
	{
		return new JValue(value);
	}

	public static implicit operator JToken(DateTimeOffset value)
	{
		return new JValue(value);
	}

	public static implicit operator JToken(byte value)
	{
		return new JValue(value);
	}

	public static implicit operator JToken(byte? value)
	{
		return new JValue(value);
	}

	[CLSCompliant(false)]
	public static implicit operator JToken(sbyte value)
	{
		return new JValue(value);
	}

	[CLSCompliant(false)]
	public static implicit operator JToken(sbyte? value)
	{
		return new JValue(value);
	}

	public static implicit operator JToken(bool? value)
	{
		return new JValue(value);
	}

	public static implicit operator JToken(long value)
	{
		return new JValue(value);
	}

	public static implicit operator JToken(DateTime? value)
	{
		return new JValue(value);
	}

	public static implicit operator JToken(DateTimeOffset? value)
	{
		return new JValue(value);
	}

	public static implicit operator JToken(decimal? value)
	{
		return new JValue(value);
	}

	public static implicit operator JToken(double? value)
	{
		return new JValue(value);
	}

	[CLSCompliant(false)]
	public static implicit operator JToken(short value)
	{
		return new JValue(value);
	}

	[CLSCompliant(false)]
	public static implicit operator JToken(ushort value)
	{
		return new JValue(value);
	}

	public static implicit operator JToken(int value)
	{
		return new JValue(value);
	}

	public static implicit operator JToken(int? value)
	{
		return new JValue(value);
	}

	public static implicit operator JToken(DateTime value)
	{
		return new JValue(value);
	}

	public static implicit operator JToken(long? value)
	{
		return new JValue(value);
	}

	public static implicit operator JToken(float? value)
	{
		return new JValue(value);
	}

	public static implicit operator JToken(decimal value)
	{
		return new JValue(value);
	}

	[CLSCompliant(false)]
	public static implicit operator JToken(short? value)
	{
		return new JValue(value);
	}

	[CLSCompliant(false)]
	public static implicit operator JToken(ushort? value)
	{
		return new JValue(value);
	}

	[CLSCompliant(false)]
	public static implicit operator JToken(uint? value)
	{
		return new JValue(value);
	}

	[CLSCompliant(false)]
	public static implicit operator JToken(ulong? value)
	{
		return new JValue(value);
	}

	public static implicit operator JToken(double value)
	{
		return new JValue(value);
	}

	public static implicit operator JToken(float value)
	{
		return new JValue(value);
	}

	public static implicit operator JToken(string value)
	{
		return new JValue(value);
	}

	[CLSCompliant(false)]
	public static implicit operator JToken(uint value)
	{
		return new JValue(value);
	}

	[CLSCompliant(false)]
	public static implicit operator JToken(ulong value)
	{
		return new JValue(value);
	}

	public static implicit operator JToken(byte[] value)
	{
		return new JValue(value);
	}

	public static implicit operator JToken(Uri value)
	{
		return new JValue(value);
	}

	public static implicit operator JToken(TimeSpan value)
	{
		return new JValue(value);
	}

	public static implicit operator JToken(TimeSpan? value)
	{
		return new JValue(value);
	}

	public static implicit operator JToken(Guid value)
	{
		return new JValue(value);
	}

	public static implicit operator JToken(Guid? value)
	{
		return new JValue(value);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable<JToken>)this).GetEnumerator();
	}

	IEnumerator<JToken> IEnumerable<JToken>.GetEnumerator()
	{
		return Children().GetEnumerator();
	}

	internal abstract int GetDeepHashCode();

	public JsonReader CreateReader()
	{
		return new JTokenReader(this);
	}

	internal static JToken FromObjectInternal(object o, JsonSerializer jsonSerializer)
	{
		ValidationUtils.ArgumentNotNull(o, "o");
		ValidationUtils.ArgumentNotNull(jsonSerializer, "jsonSerializer");
		using JTokenWriter jTokenWriter = new JTokenWriter();
		jsonSerializer.Serialize(jTokenWriter, o);
		return jTokenWriter.Token;
	}

	public static JToken FromObject(object o)
	{
		return FromObjectInternal(o, JsonSerializer.CreateDefault());
	}

	public static JToken FromObject(object o, JsonSerializer jsonSerializer)
	{
		return FromObjectInternal(o, jsonSerializer);
	}

	public T ToObject<T>()
	{
		return (T)ToObject(typeof(T));
	}

	public object ToObject(Type objectType)
	{
		if (JsonConvert.DefaultSettings == null)
		{
			bool isEnum;
			PrimitiveTypeCode typeCode = ConvertUtils.GetTypeCode(objectType, out isEnum);
			if (isEnum)
			{
				if (Type == JTokenType.String)
				{
					try
					{
						return ToObject(objectType, JsonSerializer.CreateDefault());
					}
					catch (Exception innerException)
					{
						Type type = (objectType.IsEnum() ? objectType : Nullable.GetUnderlyingType(objectType));
						throw new ArgumentException("Could not convert '{0}' to {1}.".FormatWith(CultureInfo.InvariantCulture, (string)this, type.Name), innerException);
					}
				}
				if (Type == JTokenType.Integer)
				{
					return Enum.ToObject(objectType.IsEnum() ? objectType : Nullable.GetUnderlyingType(objectType), ((JValue)this).Value);
				}
			}
			switch (typeCode)
			{
			case PrimitiveTypeCode.BooleanNullable:
				return (bool?)this;
			case PrimitiveTypeCode.Boolean:
				return (bool)this;
			case PrimitiveTypeCode.CharNullable:
				return (char?)this;
			case PrimitiveTypeCode.Char:
				return (char)this;
			case PrimitiveTypeCode.SByte:
				return (sbyte?)this;
			case PrimitiveTypeCode.SByteNullable:
				return (sbyte)this;
			case PrimitiveTypeCode.ByteNullable:
				return (byte?)this;
			case PrimitiveTypeCode.Byte:
				return (byte)this;
			case PrimitiveTypeCode.Int16Nullable:
				return (short?)this;
			case PrimitiveTypeCode.Int16:
				return (short)this;
			case PrimitiveTypeCode.UInt16Nullable:
				return (ushort?)this;
			case PrimitiveTypeCode.UInt16:
				return (ushort)this;
			case PrimitiveTypeCode.Int32Nullable:
				return (int?)this;
			case PrimitiveTypeCode.Int32:
				return (int)this;
			case PrimitiveTypeCode.UInt32Nullable:
				return (uint?)this;
			case PrimitiveTypeCode.UInt32:
				return (uint)this;
			case PrimitiveTypeCode.Int64Nullable:
				return (long?)this;
			case PrimitiveTypeCode.Int64:
				return (long)this;
			case PrimitiveTypeCode.UInt64Nullable:
				return (ulong?)this;
			case PrimitiveTypeCode.UInt64:
				return (ulong)this;
			case PrimitiveTypeCode.SingleNullable:
				return (float?)this;
			case PrimitiveTypeCode.Single:
				return (float)this;
			case PrimitiveTypeCode.DoubleNullable:
				return (double?)this;
			case PrimitiveTypeCode.Double:
				return (double)this;
			case PrimitiveTypeCode.DecimalNullable:
				return (decimal?)this;
			case PrimitiveTypeCode.Decimal:
				return (decimal)this;
			case PrimitiveTypeCode.DateTimeNullable:
				return (DateTime?)this;
			case PrimitiveTypeCode.DateTime:
				return (DateTime)this;
			case PrimitiveTypeCode.DateTimeOffsetNullable:
				return (DateTimeOffset?)this;
			case PrimitiveTypeCode.DateTimeOffset:
				return (DateTimeOffset)this;
			case PrimitiveTypeCode.String:
				return (string)this;
			case PrimitiveTypeCode.GuidNullable:
				return (Guid?)this;
			case PrimitiveTypeCode.Guid:
				return (Guid)this;
			case PrimitiveTypeCode.Uri:
				return (Uri)this;
			case PrimitiveTypeCode.TimeSpanNullable:
				return (TimeSpan?)this;
			case PrimitiveTypeCode.TimeSpan:
				return (TimeSpan)this;
			}
		}
		return ToObject(objectType, JsonSerializer.CreateDefault());
	}

	public T ToObject<T>(JsonSerializer jsonSerializer)
	{
		return (T)ToObject(typeof(T), jsonSerializer);
	}

	public object ToObject(Type objectType, JsonSerializer jsonSerializer)
	{
		ValidationUtils.ArgumentNotNull(jsonSerializer, "jsonSerializer");
		using JTokenReader reader = new JTokenReader(this);
		return jsonSerializer.Deserialize(reader, objectType);
	}

	public static JToken ReadFrom(JsonReader reader)
	{
		return ReadFrom(reader, null);
	}

	public static JToken ReadFrom(JsonReader reader, JsonLoadSettings settings)
	{
		ValidationUtils.ArgumentNotNull(reader, "reader");
		if (reader.TokenType == JsonToken.None && !((settings != null && settings.CommentHandling == CommentHandling.Ignore) ? reader.ReadAndMoveToContent() : reader.Read()))
		{
			throw JsonReaderException.Create(reader, "Error reading JToken from JsonReader.");
		}
		IJsonLineInfo lineInfo = reader as IJsonLineInfo;
		switch (reader.TokenType)
		{
		case JsonToken.StartObject:
			return JObject.Load(reader, settings);
		case JsonToken.StartArray:
			return JArray.Load(reader, settings);
		case JsonToken.StartConstructor:
			return JConstructor.Load(reader, settings);
		case JsonToken.PropertyName:
			return JProperty.Load(reader, settings);
		case JsonToken.Integer:
		case JsonToken.Float:
		case JsonToken.String:
		case JsonToken.Boolean:
		case JsonToken.Date:
		case JsonToken.Bytes:
		{
			JValue jValue4 = new JValue(reader.Value);
			jValue4.SetLineInfo(lineInfo, settings);
			return jValue4;
		}
		case JsonToken.Comment:
		{
			JValue jValue3 = JValue.CreateComment(reader.Value.ToString());
			jValue3.SetLineInfo(lineInfo, settings);
			return jValue3;
		}
		case JsonToken.Null:
		{
			JValue jValue2 = JValue.CreateNull();
			jValue2.SetLineInfo(lineInfo, settings);
			return jValue2;
		}
		case JsonToken.Undefined:
		{
			JValue jValue = JValue.CreateUndefined();
			jValue.SetLineInfo(lineInfo, settings);
			return jValue;
		}
		default:
			throw JsonReaderException.Create(reader, "Error reading JToken from JsonReader. Unexpected token: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
		}
	}

	public static JToken Parse(string json)
	{
		return Parse(json, null);
	}

	public static JToken Parse(string json, JsonLoadSettings settings)
	{
		using JsonReader jsonReader = new JsonTextReader(new StringReader(json));
		JToken result = Load(jsonReader, settings);
		if (jsonReader.Read() && jsonReader.TokenType != JsonToken.Comment)
		{
			throw JsonReaderException.Create(jsonReader, "Additional text found in JSON string after parsing content.");
		}
		return result;
	}

	public static JToken Load(JsonReader reader, JsonLoadSettings settings)
	{
		return ReadFrom(reader, settings);
	}

	public static JToken Load(JsonReader reader)
	{
		return Load(reader, null);
	}

	internal void SetLineInfo(IJsonLineInfo lineInfo, JsonLoadSettings settings)
	{
		if ((settings == null || settings.LineInfoHandling != LineInfoHandling.Load) && lineInfo != null && lineInfo.HasLineInfo())
		{
			SetLineInfo(lineInfo.LineNumber, lineInfo.LinePosition);
		}
	}

	internal void SetLineInfo(int lineNumber, int linePosition)
	{
		AddAnnotation(new LineInfoAnnotation(lineNumber, linePosition));
	}

	bool IJsonLineInfo.HasLineInfo()
	{
		return Annotation<LineInfoAnnotation>() != null;
	}

	public JToken SelectToken(string path)
	{
		return SelectToken(path, errorWhenNoMatch: false);
	}

	public JToken SelectToken(string path, bool errorWhenNoMatch)
	{
		JPath jPath = new JPath(path);
		JToken jToken = null;
		foreach (JToken item in jPath.Evaluate(this, errorWhenNoMatch))
		{
			if (jToken != null)
			{
				throw new JsonException("Path returned multiple tokens.");
			}
			jToken = item;
		}
		return jToken;
	}

	public IEnumerable<JToken> SelectTokens(string path)
	{
		return SelectTokens(path, errorWhenNoMatch: false);
	}

	public IEnumerable<JToken> SelectTokens(string path, bool errorWhenNoMatch)
	{
		return new JPath(path).Evaluate(this, errorWhenNoMatch);
	}

	object ICloneable.Clone()
	{
		return DeepClone();
	}

	public JToken DeepClone()
	{
		return CloneToken();
	}

	public void AddAnnotation(object annotation)
	{
		if (annotation == null)
		{
			throw new ArgumentNullException("annotation");
		}
		if (_annotations == null)
		{
			_annotations = ((!(annotation is object[])) ? annotation : new object[1] { annotation });
			return;
		}
		object[] array = _annotations as object[];
		if (array == null)
		{
			_annotations = new object[2] { _annotations, annotation };
			return;
		}
		int i;
		for (i = 0; i < array.Length && array[i] != null; i++)
		{
		}
		if (i == array.Length)
		{
			Array.Resize(ref array, i * 2);
			_annotations = array;
		}
		array[i] = annotation;
	}

	public T Annotation<T>() where T : class
	{
		if (_annotations != null)
		{
			if (!(_annotations is object[] array))
			{
				return _annotations as T;
			}
			foreach (object obj in array)
			{
				if (obj == null)
				{
					break;
				}
				if (obj is T result)
				{
					return result;
				}
			}
		}
		return null;
	}

	public object Annotation(Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		if (_annotations != null)
		{
			if (!(_annotations is object[] array))
			{
				if (type.IsInstanceOfType(_annotations))
				{
					return _annotations;
				}
			}
			else
			{
				foreach (object obj in array)
				{
					if (obj == null)
					{
						break;
					}
					if (type.IsInstanceOfType(obj))
					{
						return obj;
					}
				}
			}
		}
		return null;
	}

	public IEnumerable<T> Annotations<T>() where T : class
	{
		if (_annotations == null)
		{
			yield break;
		}
		if (_annotations is object[] annotations)
		{
			foreach (object obj in annotations)
			{
				if (obj != null)
				{
					if (obj is T val)
					{
						yield return val;
					}
					continue;
				}
				break;
			}
		}
		else if (_annotations is T val2)
		{
			yield return val2;
		}
	}

	public IEnumerable<object> Annotations(Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		if (_annotations == null)
		{
			yield break;
		}
		if (_annotations is object[] annotations)
		{
			foreach (object obj in annotations)
			{
				if (obj != null)
				{
					if (type.IsInstanceOfType(obj))
					{
						yield return obj;
					}
					continue;
				}
				break;
			}
		}
		else if (type.IsInstanceOfType(_annotations))
		{
			yield return _annotations;
		}
	}

	public void RemoveAnnotations<T>() where T : class
	{
		if (_annotations == null)
		{
			return;
		}
		if (!(_annotations is object[] array))
		{
			if (_annotations is T)
			{
				_annotations = null;
			}
			return;
		}
		int i = 0;
		int num = 0;
		for (; i < array.Length; i++)
		{
			object obj = array[i];
			if (obj == null)
			{
				break;
			}
			if (!(obj is T))
			{
				array[num++] = obj;
			}
		}
		if (num != 0)
		{
			while (num < i)
			{
				array[num++] = null;
			}
		}
		else
		{
			_annotations = null;
		}
	}

	public void RemoveAnnotations(Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		if (_annotations == null)
		{
			return;
		}
		if (!(_annotations is object[] array))
		{
			if (type.IsInstanceOfType(_annotations))
			{
				_annotations = null;
			}
			return;
		}
		int i = 0;
		int num = 0;
		for (; i < array.Length; i++)
		{
			object obj = array[i];
			if (obj == null)
			{
				break;
			}
			if (!type.IsInstanceOfType(obj))
			{
				array[num++] = obj;
			}
		}
		if (num != 0)
		{
			while (num < i)
			{
				array[num++] = null;
			}
		}
		else
		{
			_annotations = null;
		}
	}
}

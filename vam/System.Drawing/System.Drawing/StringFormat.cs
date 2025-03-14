using System.ComponentModel;
using System.Drawing.Text;

namespace System.Drawing;

public sealed class StringFormat : MarshalByRefObject, IDisposable, ICloneable
{
	private IntPtr nativeStrFmt = IntPtr.Zero;

	private int language;

	public StringAlignment Alignment
	{
		get
		{
			StringAlignment align;
			Status status = GDIPlus.GdipGetStringFormatAlign(nativeStrFmt, out align);
			GDIPlus.CheckStatus(status);
			return align;
		}
		set
		{
			if (value < StringAlignment.Near || value > StringAlignment.Far)
			{
				throw new InvalidEnumArgumentException("Alignment");
			}
			Status status = GDIPlus.GdipSetStringFormatAlign(nativeStrFmt, value);
			GDIPlus.CheckStatus(status);
		}
	}

	public StringAlignment LineAlignment
	{
		get
		{
			StringAlignment align;
			Status status = GDIPlus.GdipGetStringFormatLineAlign(nativeStrFmt, out align);
			GDIPlus.CheckStatus(status);
			return align;
		}
		set
		{
			if (value < StringAlignment.Near || value > StringAlignment.Far)
			{
				throw new InvalidEnumArgumentException("Alignment");
			}
			Status status = GDIPlus.GdipSetStringFormatLineAlign(nativeStrFmt, value);
			GDIPlus.CheckStatus(status);
		}
	}

	public StringFormatFlags FormatFlags
	{
		get
		{
			StringFormatFlags flags;
			Status status = GDIPlus.GdipGetStringFormatFlags(nativeStrFmt, out flags);
			GDIPlus.CheckStatus(status);
			return flags;
		}
		set
		{
			Status status = GDIPlus.GdipSetStringFormatFlags(nativeStrFmt, value);
			GDIPlus.CheckStatus(status);
		}
	}

	public HotkeyPrefix HotkeyPrefix
	{
		get
		{
			HotkeyPrefix hotkeyPrefix;
			Status status = GDIPlus.GdipGetStringFormatHotkeyPrefix(nativeStrFmt, out hotkeyPrefix);
			GDIPlus.CheckStatus(status);
			return hotkeyPrefix;
		}
		set
		{
			if (value < HotkeyPrefix.None || value > HotkeyPrefix.Hide)
			{
				throw new InvalidEnumArgumentException("HotkeyPrefix");
			}
			Status status = GDIPlus.GdipSetStringFormatHotkeyPrefix(nativeStrFmt, value);
			GDIPlus.CheckStatus(status);
		}
	}

	public StringTrimming Trimming
	{
		get
		{
			StringTrimming trimming;
			Status status = GDIPlus.GdipGetStringFormatTrimming(nativeStrFmt, out trimming);
			GDIPlus.CheckStatus(status);
			return trimming;
		}
		set
		{
			if (value < StringTrimming.None || value > StringTrimming.EllipsisPath)
			{
				throw new InvalidEnumArgumentException("Trimming");
			}
			Status status = GDIPlus.GdipSetStringFormatTrimming(nativeStrFmt, value);
			GDIPlus.CheckStatus(status);
		}
	}

	public static StringFormat GenericDefault
	{
		get
		{
			IntPtr format;
			Status status = GDIPlus.GdipStringFormatGetGenericDefault(out format);
			GDIPlus.CheckStatus(status);
			return new StringFormat(format);
		}
	}

	public int DigitSubstitutionLanguage => language;

	public static StringFormat GenericTypographic
	{
		get
		{
			IntPtr format;
			Status status = GDIPlus.GdipStringFormatGetGenericTypographic(out format);
			GDIPlus.CheckStatus(status);
			return new StringFormat(format);
		}
	}

	public StringDigitSubstitute DigitSubstitutionMethod
	{
		get
		{
			StringDigitSubstitute substitute;
			Status status = GDIPlus.GdipGetStringFormatDigitSubstitution(nativeStrFmt, language, out substitute);
			GDIPlus.CheckStatus(status);
			return substitute;
		}
	}

	internal IntPtr NativeObject
	{
		get
		{
			return nativeStrFmt;
		}
		set
		{
			nativeStrFmt = value;
		}
	}

	public StringFormat()
		: this((StringFormatFlags)0, 0)
	{
	}

	public StringFormat(StringFormatFlags options, int language)
	{
		Status status = GDIPlus.GdipCreateStringFormat(options, language, out nativeStrFmt);
		GDIPlus.CheckStatus(status);
	}

	internal StringFormat(IntPtr native)
	{
		nativeStrFmt = native;
	}

	public StringFormat(StringFormat format)
	{
		if (format == null)
		{
			throw new ArgumentNullException("format");
		}
		Status status = GDIPlus.GdipCloneStringFormat(format.NativeObject, out nativeStrFmt);
		GDIPlus.CheckStatus(status);
	}

	public StringFormat(StringFormatFlags options)
	{
		Status status = GDIPlus.GdipCreateStringFormat(options, 0, out nativeStrFmt);
		GDIPlus.CheckStatus(status);
	}

	~StringFormat()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (nativeStrFmt != IntPtr.Zero)
		{
			Status status = GDIPlus.GdipDeleteStringFormat(nativeStrFmt);
			nativeStrFmt = IntPtr.Zero;
			GDIPlus.CheckStatus(status);
		}
	}

	public void SetMeasurableCharacterRanges(CharacterRange[] ranges)
	{
		Status status = GDIPlus.GdipSetStringFormatMeasurableCharacterRanges(nativeStrFmt, ranges.Length, ranges);
		GDIPlus.CheckStatus(status);
	}

	internal int GetMeasurableCharacterRangeCount()
	{
		int cnt;
		Status status = GDIPlus.GdipGetStringFormatMeasurableCharacterRangeCount(nativeStrFmt, out cnt);
		GDIPlus.CheckStatus(status);
		return cnt;
	}

	public object Clone()
	{
		IntPtr format;
		Status status = GDIPlus.GdipCloneStringFormat(nativeStrFmt, out format);
		GDIPlus.CheckStatus(status);
		return new StringFormat(format);
	}

	public override string ToString()
	{
		return "[StringFormat, FormatFlags=" + FormatFlags.ToString() + "]";
	}

	public void SetTabStops(float firstTabOffset, float[] tabStops)
	{
		Status status = GDIPlus.GdipSetStringFormatTabStops(nativeStrFmt, firstTabOffset, tabStops.Length, tabStops);
		GDIPlus.CheckStatus(status);
	}

	public void SetDigitSubstitution(int language, StringDigitSubstitute substitute)
	{
		Status status = GDIPlus.GdipSetStringFormatDigitSubstitution(nativeStrFmt, this.language, substitute);
		GDIPlus.CheckStatus(status);
	}

	public float[] GetTabStops(out float firstTabOffset)
	{
		int count = 0;
		firstTabOffset = 0f;
		Status status = GDIPlus.GdipGetStringFormatTabStopCount(nativeStrFmt, out count);
		GDIPlus.CheckStatus(status);
		float[] array = new float[count];
		if (count != 0)
		{
			status = GDIPlus.GdipGetStringFormatTabStops(nativeStrFmt, count, out firstTabOffset, array);
			GDIPlus.CheckStatus(status);
		}
		return array;
	}
}

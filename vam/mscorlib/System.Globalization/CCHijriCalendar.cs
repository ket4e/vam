namespace System.Globalization;

internal class CCHijriCalendar
{
	public enum Month
	{
		muharram = 1,
		safar,
		rabi_I,
		rabi_II,
		jumada_I,
		jumada_II,
		rajab,
		shaban,
		ramadan,
		shawwal,
		dhu_al_quada,
		dhu_al_hijja
	}

	private const int epoch = 227014;

	public static bool is_leap_year(int year)
	{
		return CCMath.mod(14 + 11 * year, 30) < 11;
	}

	public static int fixed_from_dmy(int day, int month, int year)
	{
		int num = 227013;
		num += 354 * (year - 1);
		num += CCMath.div(3 + 11 * year, 30);
		num += (int)Math.Ceiling(29.5 * (double)(month - 1));
		return num + day;
	}

	public static int year_from_fixed(int date)
	{
		return CCMath.div(30 * (date - 227014) + 10646, 10631);
	}

	public static void my_from_fixed(out int month, out int year, int date)
	{
		year = year_from_fixed(date);
		int num = 1 + (int)Math.Ceiling((double)(date - 29 - fixed_from_dmy(1, 1, year)) / 29.5);
		month = ((num >= 12) ? 12 : num);
	}

	public static void dmy_from_fixed(out int day, out int month, out int year, int date)
	{
		my_from_fixed(out month, out year, date);
		day = date - fixed_from_dmy(1, month, year) + 1;
	}

	public static int month_from_fixed(int date)
	{
		my_from_fixed(out var month, out var _, date);
		return month;
	}

	public static int day_from_fixed(int date)
	{
		dmy_from_fixed(out var day, out var _, out var _, date);
		return day;
	}

	public static int date_difference(int dayA, int monthA, int yearA, int dayB, int monthB, int yearB)
	{
		return fixed_from_dmy(dayB, monthB, yearB) - fixed_from_dmy(dayA, monthA, yearA);
	}

	public static int day_number(int day, int month, int year)
	{
		return date_difference(31, 12, year - 1, day, month, year);
	}

	public static int days_remaining(int day, int month, int year)
	{
		return date_difference(day, month, year, 31, 12, year);
	}
}

namespace System.Globalization;

[Serializable]
public class JapaneseLunisolarCalendar : EastAsianLunisolarCalendar
{
	public const int JapaneseEra = 1;

	internal static readonly CCEastAsianLunisolarEraHandler era_handler;

	private static DateTime JapanMin;

	private static DateTime JapanMax;

	internal override int ActualCurrentEra => 4;

	public override int[] Eras => (int[])era_handler.Eras.Clone();

	public override DateTime MinSupportedDateTime => JapanMin;

	public override DateTime MaxSupportedDateTime => JapanMax;

	[MonoTODO]
	public JapaneseLunisolarCalendar()
		: base(era_handler)
	{
	}

	static JapaneseLunisolarCalendar()
	{
		JapanMin = new DateTime(1960, 1, 28, 0, 0, 0);
		JapanMax = new DateTime(2050, 1, 22, 23, 59, 59);
		era_handler = new CCEastAsianLunisolarEraHandler();
		era_handler.appendEra(3, CCGregorianCalendar.fixed_from_dmy(25, 12, 1926), CCGregorianCalendar.fixed_from_dmy(7, 1, 1989));
		era_handler.appendEra(4, CCGregorianCalendar.fixed_from_dmy(8, 1, 1989));
	}

	public override int GetEra(DateTime time)
	{
		int date = CCFixed.FromDateTime(time);
		era_handler.EraYear(out var era, date);
		return era;
	}
}

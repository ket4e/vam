namespace System.Globalization;

[Serializable]
public class TaiwanLunisolarCalendar : EastAsianLunisolarCalendar
{
	private const int TaiwanEra = 1;

	internal static readonly CCEastAsianLunisolarEraHandler era_handler;

	private static DateTime TaiwanMin;

	private static DateTime TaiwanMax;

	public override int[] Eras => (int[])era_handler.Eras.Clone();

	public override DateTime MinSupportedDateTime => TaiwanMin;

	public override DateTime MaxSupportedDateTime => TaiwanMax;

	[MonoTODO]
	public TaiwanLunisolarCalendar()
		: base(era_handler)
	{
	}

	static TaiwanLunisolarCalendar()
	{
		TaiwanMin = new DateTime(1912, 2, 18);
		TaiwanMax = new DateTime(2051, 2, 10, 23, 59, 59, 999);
		era_handler = new CCEastAsianLunisolarEraHandler();
		era_handler.appendEra(1, CCFixed.FromDateTime(TaiwanMin), CCFixed.FromDateTime(TaiwanMax));
	}

	public override int GetEra(DateTime time)
	{
		int date = CCFixed.FromDateTime(time);
		era_handler.EraYear(out var era, date);
		return era;
	}
}

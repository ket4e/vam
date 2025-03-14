namespace System.Windows.Forms;

public class PowerStatus
{
	private BatteryChargeStatus battery_charge_status;

	private int battery_full_lifetime;

	private float battery_life_percent;

	private int battery_life_remaining;

	private PowerLineStatus power_line_status;

	public BatteryChargeStatus BatteryChargeStatus => battery_charge_status;

	public int BatteryFullLifetime => battery_full_lifetime;

	public float BatteryLifePercent => battery_life_percent;

	public int BatteryLifeRemaining => battery_life_remaining;

	public PowerLineStatus PowerLineStatus => power_line_status;

	internal PowerStatus(BatteryChargeStatus batteryChargeStatus, int batteryFullLifetime, float batteryLifePercent, int batteryLifeRemaining, PowerLineStatus powerLineStatus)
	{
		battery_charge_status = batteryChargeStatus;
		battery_full_lifetime = batteryFullLifetime;
		battery_life_percent = batteryLifePercent;
		battery_life_remaining = batteryLifeRemaining;
		power_line_status = powerLineStatus;
	}
}

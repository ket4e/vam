namespace System.Security.AccessControl;

public enum AceType
{
	AccessAllowed = 0,
	AccessDenied = 1,
	SystemAudit = 2,
	SystemAlarm = 3,
	AccessAllowedCompound = 4,
	AccessAllowedObject = 5,
	AccessDeniedObject = 6,
	SystemAuditObject = 7,
	SystemAlarmObject = 8,
	AccessAllowedCallback = 9,
	AccessDeniedCallback = 10,
	AccessAllowedCallbackObject = 11,
	AccessDeniedCallbackObject = 12,
	SystemAuditCallback = 13,
	SystemAlarmCallback = 14,
	SystemAuditCallbackObject = 15,
	SystemAlarmCallbackObject = 16,
	MaxDefinedAceType = 16
}

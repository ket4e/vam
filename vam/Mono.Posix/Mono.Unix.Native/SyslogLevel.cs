using System;

namespace Mono.Unix.Native;

[CLSCompliant(false)]
[Map]
public enum SyslogLevel
{
	LOG_EMERG,
	LOG_ALERT,
	LOG_CRIT,
	LOG_ERR,
	LOG_WARNING,
	LOG_NOTICE,
	LOG_INFO,
	LOG_DEBUG
}

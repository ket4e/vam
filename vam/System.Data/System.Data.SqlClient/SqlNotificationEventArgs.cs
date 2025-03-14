namespace System.Data.SqlClient;

public class SqlNotificationEventArgs : EventArgs
{
	private SqlNotificationType type;

	private SqlNotificationInfo info;

	private SqlNotificationSource source;

	public SqlNotificationType Type => type;

	public SqlNotificationInfo Info => info;

	public SqlNotificationSource Source => source;

	public SqlNotificationEventArgs(SqlNotificationType type, SqlNotificationInfo info, SqlNotificationSource source)
	{
		this.type = type;
		this.info = info;
		this.source = source;
	}
}

namespace System.Data.Odbc;

internal enum OdbcIsolationLevel
{
	ReadUncommitted = 1,
	ReadCommitted = 2,
	RepeatableRead = 4,
	Serializable = 8,
	Snapshot = 0x20
}

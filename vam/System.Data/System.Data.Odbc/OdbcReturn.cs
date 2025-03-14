namespace System.Data.Odbc;

internal enum OdbcReturn : short
{
	Error = -1,
	InvalidHandle = -2,
	StillExecuting = 2,
	NeedData = 99,
	Success = 0,
	SuccessWithInfo = 1,
	NoData = 100
}

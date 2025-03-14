namespace System.Net.FtpClient;

public enum FtpResponseType
{
	None,
	PositivePreliminary,
	PositiveCompletion,
	PositiveIntermediate,
	TransientNegativeCompletion,
	PermanentNegativeCompletion
}

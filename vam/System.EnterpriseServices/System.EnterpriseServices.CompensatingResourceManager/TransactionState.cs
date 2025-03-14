namespace System.EnterpriseServices.CompensatingResourceManager;

[Serializable]
public enum TransactionState
{
	Active,
	Committed,
	Aborted,
	Indoubt
}

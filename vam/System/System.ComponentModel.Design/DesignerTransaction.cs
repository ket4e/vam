namespace System.ComponentModel.Design;

public abstract class DesignerTransaction : IDisposable
{
	private string description;

	private bool committed;

	private bool canceled;

	public bool Canceled => canceled;

	public bool Committed => committed;

	public string Description => description;

	protected DesignerTransaction()
		: this(string.Empty)
	{
	}

	protected DesignerTransaction(string description)
	{
		this.description = description;
		committed = false;
		canceled = false;
	}

	void IDisposable.Dispose()
	{
		Dispose(disposing: true);
	}

	protected virtual void Dispose(bool disposing)
	{
		Cancel();
		if (disposing)
		{
			GC.SuppressFinalize(true);
		}
	}

	protected abstract void OnCancel();

	protected abstract void OnCommit();

	public void Cancel()
	{
		if (!Canceled && !Committed)
		{
			canceled = true;
			OnCancel();
		}
	}

	public void Commit()
	{
		if (!Canceled && !Committed)
		{
			committed = true;
			OnCommit();
		}
	}

	~DesignerTransaction()
	{
		Dispose(disposing: false);
	}
}

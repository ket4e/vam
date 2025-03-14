namespace System.Windows.Forms.RTF;

internal class ClassCallback
{
	private ClassDelegate[] callbacks;

	public ClassDelegate this[TokenClass c]
	{
		get
		{
			return callbacks[(int)c];
		}
		set
		{
			callbacks[(int)c] = value;
		}
	}

	public ClassCallback()
	{
		callbacks = new ClassDelegate[Enum.GetValues(typeof(Major)).Length];
	}
}

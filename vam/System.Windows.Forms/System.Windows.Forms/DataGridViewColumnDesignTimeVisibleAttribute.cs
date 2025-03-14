namespace System.Windows.Forms;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class DataGridViewColumnDesignTimeVisibleAttribute : Attribute
{
	public static readonly DataGridViewColumnDesignTimeVisibleAttribute Default = new DataGridViewColumnDesignTimeVisibleAttribute(visible: true);

	public static readonly DataGridViewColumnDesignTimeVisibleAttribute No = new DataGridViewColumnDesignTimeVisibleAttribute(visible: false);

	public static readonly DataGridViewColumnDesignTimeVisibleAttribute Yes = new DataGridViewColumnDesignTimeVisibleAttribute(visible: true);

	private bool visible;

	public bool Visible => visible;

	public DataGridViewColumnDesignTimeVisibleAttribute()
	{
	}

	public DataGridViewColumnDesignTimeVisibleAttribute(bool visible)
	{
		this.visible = visible;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is DataGridViewColumnDesignTimeVisibleAttribute))
		{
			return false;
		}
		if ((obj as DataGridViewColumnDesignTimeVisibleAttribute).visible != visible)
		{
			return false;
		}
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override bool IsDefaultAttribute()
	{
		return Equals(Default);
	}
}

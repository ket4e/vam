namespace System.Windows.Forms;

public class FileDialogCustomPlace
{
	private string path;

	private Guid guid;

	public string Path
	{
		get
		{
			return path;
		}
		set
		{
			path = value;
			guid = Guid.Empty;
		}
	}

	public Guid KnownFolderGuid
	{
		get
		{
			return guid;
		}
		set
		{
			guid = value;
			path = string.Empty;
		}
	}

	public FileDialogCustomPlace(string path)
	{
		this.path = path;
		guid = Guid.Empty;
	}

	public FileDialogCustomPlace(Guid knownFolderGuid)
	{
		path = string.Empty;
		guid = knownFolderGuid;
	}

	public override string ToString()
	{
		return $"{GetType().ToString()} Path: {path} KnownFolderGuid: {guid}";
	}
}

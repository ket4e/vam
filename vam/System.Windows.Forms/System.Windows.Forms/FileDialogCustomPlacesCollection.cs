using System.Collections.ObjectModel;

namespace System.Windows.Forms;

public class FileDialogCustomPlacesCollection : Collection<FileDialogCustomPlace>
{
	public void Add(Guid knownFolderGuid)
	{
		Add(new FileDialogCustomPlace(knownFolderGuid));
	}

	public void Add(string path)
	{
		Add(new FileDialogCustomPlace(path));
	}
}

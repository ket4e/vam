using UnityEngine;
using UnityEngine.UI;

namespace uFileBrowser;

public class ShortCutButton : MonoBehaviour
{
	public Text packageLabel;

	public Text label;

	[HideInInspector]
	public string package;

	[HideInInspector]
	public string packageFilter;

	[HideInInspector]
	public bool flatten;

	[HideInInspector]
	public bool includeRegularDirsInFlatten;

	[HideInInspector]
	public string text;

	[HideInInspector]
	public string fullPath;

	[HideInInspector]
	public int id;

	private FileBrowser browser;

	public void OnClick()
	{
		if ((bool)browser)
		{
			browser.OnShortCutClick(id);
		}
	}

	public void Set(FileBrowser b, string pkg, string pkgFilter, bool flat, bool includeRegDirsInFlat, string txt, string path, int i)
	{
		browser = b;
		package = pkg;
		packageFilter = pkgFilter;
		flatten = flat;
		includeRegularDirsInFlatten = includeRegDirsInFlat;
		text = txt;
		fullPath = path;
		id = i;
		if (packageLabel != null)
		{
			if (package == string.Empty)
			{
				packageLabel.gameObject.SetActive(value: false);
			}
			else
			{
				packageLabel.gameObject.SetActive(value: true);
			}
			packageLabel.text = package;
		}
		if (label != null)
		{
			label.text = text;
		}
	}
}

using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeRegionDirective : CodeDirective
{
	private CodeRegionMode regionMode;

	private string regionText;

	public CodeRegionMode RegionMode
	{
		get
		{
			return regionMode;
		}
		set
		{
			regionMode = value;
		}
	}

	public string RegionText
	{
		get
		{
			if (regionText == null)
			{
				return string.Empty;
			}
			return regionText;
		}
		set
		{
			regionText = value;
		}
	}

	public CodeRegionDirective()
	{
	}

	public CodeRegionDirective(CodeRegionMode regionMode, string regionText)
	{
		this.regionMode = regionMode;
		this.regionText = regionText;
	}
}

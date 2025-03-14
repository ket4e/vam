namespace System.Windows.Forms;

public interface IFeatureSupport
{
	Version GetVersionPresent(object feature);

	bool IsPresent(object feature);

	bool IsPresent(object feature, Version minimumVersion);
}

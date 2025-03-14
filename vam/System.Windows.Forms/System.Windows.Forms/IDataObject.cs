using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ComVisible(true)]
public interface IDataObject
{
	object GetData(string format);

	object GetData(string format, bool autoConvert);

	object GetData(Type format);

	bool GetDataPresent(string format);

	bool GetDataPresent(string format, bool autoConvert);

	bool GetDataPresent(Type format);

	string[] GetFormats();

	string[] GetFormats(bool autoConvert);

	void SetData(object data);

	void SetData(string format, bool autoConvert, object data);

	void SetData(string format, object data);

	void SetData(Type format, object data);
}

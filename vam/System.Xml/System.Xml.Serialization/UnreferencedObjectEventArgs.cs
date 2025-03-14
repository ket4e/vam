namespace System.Xml.Serialization;

public class UnreferencedObjectEventArgs : EventArgs
{
	private object unreferencedObject;

	private string unreferencedId;

	public string UnreferencedId => unreferencedId;

	public object UnreferencedObject => unreferencedObject;

	public UnreferencedObjectEventArgs(object o, string id)
	{
		unreferencedObject = o;
		unreferencedId = id;
	}
}

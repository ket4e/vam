namespace UnityEngine.Experimental.UIElements;

public class Label : BaseTextElement
{
	public Label()
		: this(string.Empty)
	{
	}

	public Label(string text)
	{
		this.text = text;
	}
}

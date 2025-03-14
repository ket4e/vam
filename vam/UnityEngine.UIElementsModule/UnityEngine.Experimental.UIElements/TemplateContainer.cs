namespace UnityEngine.Experimental.UIElements;

public class TemplateContainer : VisualElement
{
	public readonly string templateId;

	private VisualElement m_ContentContainer;

	public override VisualElement contentContainer => m_ContentContainer;

	public TemplateContainer(string templateId)
	{
		this.templateId = templateId;
		m_ContentContainer = this;
	}

	internal void SetContentContainer(VisualElement content)
	{
		m_ContentContainer = content;
	}
}

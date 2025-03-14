using UnityEngine.Experimental.UIElements;

namespace UnityEngine.Internal.Experimental.UIElements;

public class PanelWrapper : ScriptableObject
{
	private Panel m_Panel;

	public VisualElement visualTree => m_Panel.visualTree;

	private void OnEnable()
	{
		m_Panel = UIElementsUtility.FindOrCreatePanel(this);
	}

	private void OnDisable()
	{
		m_Panel = null;
	}

	public void Repaint(Event e)
	{
		m_Panel.Repaint(e);
	}
}

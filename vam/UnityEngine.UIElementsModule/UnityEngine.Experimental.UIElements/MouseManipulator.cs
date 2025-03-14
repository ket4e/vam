using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements;

public abstract class MouseManipulator : Manipulator
{
	private ManipulatorActivationFilter m_currentActivator;

	public List<ManipulatorActivationFilter> activators { get; private set; }

	protected MouseManipulator()
	{
		activators = new List<ManipulatorActivationFilter>();
	}

	protected bool CanStartManipulation(IMouseEvent e)
	{
		if (MouseCaptureController.IsMouseCaptureTaken())
		{
			return false;
		}
		foreach (ManipulatorActivationFilter activator in activators)
		{
			if (activator.Matches(e))
			{
				m_currentActivator = activator;
				return true;
			}
		}
		return false;
	}

	protected bool CanStopManipulation(IMouseEvent e)
	{
		return e.button == (int)m_currentActivator.button && base.target.HasMouseCapture();
	}
}

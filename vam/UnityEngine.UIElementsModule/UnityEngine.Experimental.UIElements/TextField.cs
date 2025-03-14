using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>A textfield is a rectangular area where the user can edit a string.</para>
/// </summary>
public class TextField : TextInputFieldBase, INotifyValueChanged<string>
{
	private bool m_Multiline;

	protected string m_Value;

	/// <summary>
	///   <para>Set this to true to allow multiple lines in the textfield and false if otherwise.</para>
	/// </summary>
	public bool multiline
	{
		get
		{
			return m_Multiline;
		}
		set
		{
			m_Multiline = value;
			if (!value)
			{
				text = text.Replace("\n", "");
			}
		}
	}

	/// <summary>
	///   <para>Set this to true to mask the characters and false if otherwise.</para>
	/// </summary>
	public override bool isPasswordField
	{
		set
		{
			base.isPasswordField = value;
			if (value)
			{
				multiline = false;
			}
		}
	}

	/// <summary>
	///   <para>The string currently being exposed by the field.</para>
	/// </summary>
	public string value
	{
		get
		{
			return m_Value;
		}
		set
		{
			m_Value = value;
			text = m_Value;
		}
	}

	/// <summary>
	///   <para>Creates a new textfield.</para>
	/// </summary>
	/// <param name="maxLength">The maximum number of characters this textfield can hold. If 0, there is no limit.</param>
	/// <param name="multiline">Set this to true to allow multiple lines in the textfield and false if otherwise.</param>
	/// <param name="isPasswordField">Set this to true to mask the characters and false if otherwise.</param>
	/// <param name="maskChar">The character used for masking in a password field.</param>
	public TextField()
		: this(-1, multiline: false, isPasswordField: false, '\0')
	{
	}

	/// <summary>
	///   <para>Creates a new textfield.</para>
	/// </summary>
	/// <param name="maxLength">The maximum number of characters this textfield can hold. If 0, there is no limit.</param>
	/// <param name="multiline">Set this to true to allow multiple lines in the textfield and false if otherwise.</param>
	/// <param name="isPasswordField">Set this to true to mask the characters and false if otherwise.</param>
	/// <param name="maskChar">The character used for masking in a password field.</param>
	public TextField(int maxLength, bool multiline, bool isPasswordField, char maskChar)
		: base(maxLength, maskChar)
	{
		this.multiline = multiline;
		this.isPasswordField = isPasswordField;
	}

	/// <summary>
	///   <para>Set the value and, if different, notifies registers callbacks with a ChangeEvent&lt;string&gt;</para>
	/// </summary>
	/// <param name="newValue">The new value to be set.</param>
	public void SetValueAndNotify(string newValue)
	{
		if (!EqualityComparer<string>.Default.Equals(value, newValue))
		{
			using (ChangeEvent<string> changeEvent = ChangeEvent<string>.GetPooled(value, newValue))
			{
				changeEvent.target = this;
				value = newValue;
				UIElementsUtility.eventDispatcher.DispatchEvent(changeEvent, base.panel);
			}
		}
	}

	public void OnValueChanged(EventCallback<ChangeEvent<string>> callback)
	{
		RegisterCallback(callback);
	}

	/// <summary>
	///   <para>Called when the persistent data is accessible and/or when the data or persistence key have changed (VisualElement is properly parented).</para>
	/// </summary>
	public override void OnPersistentDataReady()
	{
		base.OnPersistentDataReady();
		string fullHierarchicalPersistenceKey = GetFullHierarchicalPersistenceKey();
		OverwriteFromPersistedData(this, fullHierarchicalPersistenceKey);
	}

	internal override void SyncTextEngine()
	{
		base.editorEngine.multiline = multiline;
		base.editorEngine.isPasswordField = isPasswordField;
		base.SyncTextEngine();
	}

	internal override void DoRepaint(IStylePainter painter)
	{
		if (isPasswordField)
		{
			string newText = "".PadRight(text.Length, base.maskChar);
			if (!base.hasFocus)
			{
				painter.DrawBackground(this);
				painter.DrawBorder(this);
				if (!string.IsNullOrEmpty(newText) && base.contentRect.width > 0f && base.contentRect.height > 0f)
				{
					TextStylePainterParameters defaultTextParameters = painter.GetDefaultTextParameters(this);
					defaultTextParameters.text = newText;
					painter.DrawText(defaultTextParameters);
				}
			}
			else
			{
				DrawWithTextSelectionAndCursor(painter, newText);
			}
		}
		else
		{
			base.DoRepaint(painter);
		}
	}

	protected internal override void ExecuteDefaultActionAtTarget(EventBase evt)
	{
		base.ExecuteDefaultActionAtTarget(evt);
		if (evt.GetEventTypeId() == EventBase<KeyDownEvent>.TypeId())
		{
			KeyDownEvent keyDownEvent = evt as KeyDownEvent;
			if (keyDownEvent.character == '\n')
			{
				SetValueAndNotify(text);
			}
		}
	}

	protected internal override void ExecuteDefaultAction(EventBase evt)
	{
		base.ExecuteDefaultAction(evt);
		if (evt.GetEventTypeId() == EventBase<BlurEvent>.TypeId())
		{
			SetValueAndNotify(text);
		}
	}
}

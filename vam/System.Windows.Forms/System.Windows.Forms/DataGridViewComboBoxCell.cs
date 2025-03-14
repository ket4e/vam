using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

public class DataGridViewComboBoxCell : DataGridViewCell
{
	[ListBindable(false)]
	public class ObjectCollection : ICollection, IEnumerable, IList
	{
		private ArrayList list;

		private DataGridViewComboBoxCell owner;

		bool IList.IsFixedSize => list.IsFixedSize;

		bool ICollection.IsSynchronized => list.IsSynchronized;

		object ICollection.SyncRoot => list.SyncRoot;

		public int Count => list.Count;

		public bool IsReadOnly => list.IsReadOnly;

		public virtual object this[int index]
		{
			get
			{
				return list[index];
			}
			set
			{
				ThrowIfOwnerIsDataBound();
				list[index] = value;
			}
		}

		public ObjectCollection(DataGridViewComboBoxCell owner)
		{
			this.owner = owner;
			list = new ArrayList();
		}

		void ICollection.CopyTo(Array destination, int index)
		{
			CopyTo((object[])destination, index);
		}

		int IList.Add(object item)
		{
			return Add(item);
		}

		public int Add(object item)
		{
			ThrowIfOwnerIsDataBound();
			int result = AddInternal(item);
			SyncOwnerItems();
			return result;
		}

		internal int AddInternal(object item)
		{
			return list.Add(item);
		}

		internal void AddRangeInternal(ICollection items)
		{
			list.AddRange(items);
		}

		public void AddRange(ObjectCollection value)
		{
			ThrowIfOwnerIsDataBound();
			AddRangeInternal(value);
			SyncOwnerItems();
		}

		private void SyncOwnerItems()
		{
			ThrowIfOwnerIsDataBound();
			if (owner != null)
			{
				owner.SyncItems();
			}
		}

		public void ThrowIfOwnerIsDataBound()
		{
			if (owner != null && owner.DataGridView != null && owner.DataSource != null)
			{
				throw new ArgumentException("Cannot modify collection if the cell is data bound.");
			}
		}

		public void AddRange(params object[] items)
		{
			ThrowIfOwnerIsDataBound();
			AddRangeInternal(items);
			SyncOwnerItems();
		}

		public void Clear()
		{
			ThrowIfOwnerIsDataBound();
			ClearInternal();
			SyncOwnerItems();
		}

		internal void ClearInternal()
		{
			list.Clear();
		}

		public bool Contains(object value)
		{
			return list.Contains(value);
		}

		public void CopyTo(object[] destination, int arrayIndex)
		{
			list.CopyTo(destination, arrayIndex);
		}

		public IEnumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}

		public int IndexOf(object value)
		{
			return list.IndexOf(value);
		}

		public void Insert(int index, object item)
		{
			ThrowIfOwnerIsDataBound();
			InsertInternal(index, item);
			SyncOwnerItems();
		}

		internal void InsertInternal(int index, object item)
		{
			list.Insert(index, item);
		}

		public void Remove(object value)
		{
			ThrowIfOwnerIsDataBound();
			RemoveInternal(value);
			SyncOwnerItems();
		}

		internal void RemoveInternal(object value)
		{
			list.Remove(value);
		}

		public void RemoveAt(int index)
		{
			ThrowIfOwnerIsDataBound();
			RemoveAtInternal(index);
			SyncOwnerItems();
		}

		internal void RemoveAtInternal(int index)
		{
			list.RemoveAt(index);
		}
	}

	private bool autoComplete;

	private object dataSource;

	private string displayMember;

	private DataGridViewComboBoxDisplayStyle displayStyle;

	private bool displayStyleForCurrentCellOnly;

	private int dropDownWidth;

	private FlatStyle flatStyle;

	private ObjectCollection items;

	private int maxDropDownItems;

	private bool sorted;

	private string valueMember;

	private DataGridViewComboBoxColumn owningColumnTemlate;

	[DefaultValue(true)]
	public virtual bool AutoComplete
	{
		get
		{
			return autoComplete;
		}
		set
		{
			autoComplete = value;
		}
	}

	public virtual object DataSource
	{
		get
		{
			return dataSource;
		}
		set
		{
			if (value is IList || value is IListSource || value == null)
			{
				dataSource = value;
				return;
			}
			throw new Exception("Value is no IList, IListSource or null.");
		}
	}

	[DefaultValue("")]
	public virtual string DisplayMember
	{
		get
		{
			return displayMember;
		}
		set
		{
			displayMember = value;
		}
	}

	[DefaultValue(DataGridViewComboBoxDisplayStyle.DropDownButton)]
	public DataGridViewComboBoxDisplayStyle DisplayStyle
	{
		get
		{
			return displayStyle;
		}
		set
		{
			displayStyle = value;
		}
	}

	[DefaultValue(false)]
	public bool DisplayStyleForCurrentCellOnly
	{
		get
		{
			return displayStyleForCurrentCellOnly;
		}
		set
		{
			displayStyleForCurrentCellOnly = value;
		}
	}

	[DefaultValue(1)]
	public virtual int DropDownWidth
	{
		get
		{
			return dropDownWidth;
		}
		set
		{
			if (value < 1)
			{
				throw new ArgumentOutOfRangeException("Value is less than 1.");
			}
			dropDownWidth = value;
		}
	}

	public override Type EditType => typeof(DataGridViewComboBoxEditingControl);

	[DefaultValue(FlatStyle.Standard)]
	public FlatStyle FlatStyle
	{
		get
		{
			return flatStyle;
		}
		set
		{
			if (!Enum.IsDefined(typeof(FlatStyle), value))
			{
				throw new InvalidEnumArgumentException("Value is not valid FlatStyle.");
			}
			flatStyle = value;
		}
	}

	public override Type FormattedValueType => typeof(string);

	[Browsable(false)]
	public virtual ObjectCollection Items
	{
		get
		{
			if (base.DataGridView != null && base.DataGridView.BindingContext != null && DataSource != null && !string.IsNullOrEmpty(ValueMember))
			{
				items.ClearInternal();
				CurrencyManager currencyManager = (CurrencyManager)base.DataGridView.BindingContext[DataSource];
				if (currencyManager != null && currencyManager.Count > 0)
				{
					foreach (object item in currencyManager.List)
					{
						items.AddInternal(item);
					}
				}
			}
			return items;
		}
	}

	[DefaultValue(8)]
	public virtual int MaxDropDownItems
	{
		get
		{
			return maxDropDownItems;
		}
		set
		{
			if (value < 1 || value > 100)
			{
				throw new ArgumentOutOfRangeException("Value is less than 1 or greater than 100.");
			}
			maxDropDownItems = value;
		}
	}

	[DefaultValue(false)]
	public virtual bool Sorted
	{
		get
		{
			return sorted;
		}
		set
		{
			sorted = value;
		}
	}

	[DefaultValue("")]
	public virtual string ValueMember
	{
		get
		{
			return valueMember;
		}
		set
		{
			valueMember = value;
		}
	}

	public override Type ValueType => typeof(string);

	internal DataGridViewComboBoxColumn OwningColumnTemplate
	{
		get
		{
			return owningColumnTemlate;
		}
		set
		{
			owningColumnTemlate = value;
		}
	}

	public DataGridViewComboBoxCell()
	{
		autoComplete = true;
		dataSource = null;
		displayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
		displayStyleForCurrentCellOnly = false;
		dropDownWidth = 1;
		flatStyle = FlatStyle.Standard;
		items = new ObjectCollection(this);
		maxDropDownItems = 8;
		sorted = false;
		owningColumnTemlate = null;
	}

	public override object Clone()
	{
		DataGridViewComboBoxCell dataGridViewComboBoxCell = (DataGridViewComboBoxCell)base.Clone();
		dataGridViewComboBoxCell.autoComplete = autoComplete;
		dataGridViewComboBoxCell.dataSource = dataSource;
		dataGridViewComboBoxCell.displayStyle = displayStyle;
		dataGridViewComboBoxCell.displayMember = displayMember;
		dataGridViewComboBoxCell.valueMember = valueMember;
		dataGridViewComboBoxCell.displayStyleForCurrentCellOnly = displayStyleForCurrentCellOnly;
		dataGridViewComboBoxCell.dropDownWidth = dropDownWidth;
		dataGridViewComboBoxCell.flatStyle = flatStyle;
		dataGridViewComboBoxCell.items.AddRangeInternal(items);
		dataGridViewComboBoxCell.maxDropDownItems = maxDropDownItems;
		dataGridViewComboBoxCell.sorted = sorted;
		return dataGridViewComboBoxCell;
	}

	public override void DetachEditingControl()
	{
		base.DataGridView.EditingControlInternal = null;
	}

	public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
	{
		base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
		ComboBox comboBox = base.DataGridView.EditingControl as ComboBox;
		comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
		comboBox.Sorted = Sorted;
		comboBox.DataSource = null;
		comboBox.ValueMember = null;
		comboBox.DisplayMember = null;
		comboBox.Items.Clear();
		comboBox.SelectedIndex = -1;
		if (DataSource != null)
		{
			comboBox.DataSource = DataSource;
			comboBox.ValueMember = ValueMember;
			comboBox.DisplayMember = DisplayMember;
			return;
		}
		comboBox.Items.AddRange(Items);
		if (base.FormattedValue != null && comboBox.Items.IndexOf(base.FormattedValue) != -1)
		{
			comboBox.SelectedItem = base.FormattedValue;
		}
	}

	internal void SyncItems()
	{
		if (DataSource != null || OwningColumnTemplate == null)
		{
			return;
		}
		if (OwningColumnTemplate.DataGridView != null && OwningColumnTemplate.DataGridView.EditingControl is DataGridViewComboBoxEditingControl dataGridViewComboBoxEditingControl)
		{
			object selectedItem = dataGridViewComboBoxEditingControl.SelectedItem;
			dataGridViewComboBoxEditingControl.Items.Clear();
			dataGridViewComboBoxEditingControl.Items.AddRange(items);
			if (dataGridViewComboBoxEditingControl.Items.IndexOf(selectedItem) != -1)
			{
				dataGridViewComboBoxEditingControl.SelectedItem = selectedItem;
			}
		}
		OwningColumnTemplate.SyncItems(Items);
	}

	public override bool KeyEntersEditMode(KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Space)
		{
			return true;
		}
		if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.Z)
		{
			return true;
		}
		if (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.Divide)
		{
			return true;
		}
		if (e.KeyCode == Keys.BrowserSearch || e.KeyCode == Keys.SelectMedia)
		{
			return true;
		}
		if (e.KeyCode >= Keys.OemSemicolon && e.KeyCode <= Keys.ProcessKey)
		{
			return true;
		}
		if (e.KeyCode == Keys.Attn || e.KeyCode == Keys.Packet)
		{
			return true;
		}
		if (e.KeyCode >= Keys.Exsel && e.KeyCode <= Keys.OemClear)
		{
			return true;
		}
		if (e.KeyCode == Keys.F4)
		{
			return true;
		}
		if (e.Modifiers == Keys.Alt && (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up))
		{
			return true;
		}
		return false;
	}

	public override object ParseFormattedValue(object formattedValue, DataGridViewCellStyle cellStyle, TypeConverter formattedValueTypeConverter, TypeConverter valueTypeConverter)
	{
		return base.ParseFormattedValue(formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter);
	}

	public override string ToString()
	{
		return $"DataGridViewComboBoxCell {{ ColumnIndex={base.ColumnIndex}, RowIndex={base.RowIndex} }}";
	}

	protected override Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
	{
		if (base.DataGridView == null)
		{
			return Rectangle.Empty;
		}
		object formattedValue = base.FormattedValue;
		Size size = Size.Empty;
		if (formattedValue != null)
		{
			size = DataGridViewCell.MeasureTextSize(graphics, formattedValue.ToString(), cellStyle.Font, TextFormatFlags.Left);
		}
		return new Rectangle(1, (base.OwningRow.Height - size.Height) / 2, size.Width - 3, size.Height);
	}

	protected override Rectangle GetErrorIconBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
	{
		if (base.DataGridView == null || string.IsNullOrEmpty(base.ErrorText))
		{
			return Rectangle.Empty;
		}
		Size size = new Size(12, 11);
		return new Rectangle(new Point(base.Size.Width - size.Width - 23, (base.Size.Height - size.Height) / 2), size);
	}

	protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
	{
		return base.GetFormattedValue(value, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);
	}

	protected override Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
	{
		object formattedValue = base.FormattedValue;
		if (formattedValue != null)
		{
			Size result = DataGridViewCell.MeasureTextSize(graphics, formattedValue.ToString(), cellStyle.Font, TextFormatFlags.Left);
			result.Height = Math.Max(result.Height, 22);
			result.Width += 25;
			return result;
		}
		return new Size(39, 22);
	}

	protected override void OnDataGridViewChanged()
	{
		base.OnDataGridViewChanged();
	}

	protected override void OnEnter(int rowIndex, bool throughMouseClick)
	{
		base.OnEnter(rowIndex, throughMouseClick);
	}

	protected override void OnLeave(int rowIndex, bool throughMouseClick)
	{
		base.OnLeave(rowIndex, throughMouseClick);
	}

	protected override void OnMouseClick(DataGridViewCellMouseEventArgs e)
	{
		base.OnMouseClick(e);
	}

	protected override void OnMouseEnter(int rowIndex)
	{
		base.OnMouseEnter(rowIndex);
	}

	protected override void OnMouseLeave(int rowIndex)
	{
		base.OnMouseLeave(rowIndex);
	}

	protected override void OnMouseMove(DataGridViewCellMouseEventArgs e)
	{
		base.OnMouseMove(e);
	}

	protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
	{
		base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
	}

	internal override void PaintPartContent(Graphics graphics, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, DataGridViewCellStyle cellStyle, object formattedValue)
	{
		Color foreColor = ((!Selected) ? cellStyle.ForeColor : cellStyle.SelectionForeColor);
		TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.TextBoxControl | TextFormatFlags.EndEllipsis;
		Rectangle contentBounds = base.ContentBounds;
		contentBounds.X += cellBounds.X;
		contentBounds.Y += cellBounds.Y;
		Rectangle rectangle = CalculateButtonArea(cellBounds);
		graphics.FillRectangle(SystemBrushes.Control, rectangle);
		ThemeEngine.Current.CPDrawComboButton(graphics, rectangle, ButtonState.Normal);
		if (formattedValue != null)
		{
			TextRenderer.DrawText(graphics, formattedValue.ToString(), cellStyle.Font, contentBounds, foreColor, flags);
		}
	}

	private Rectangle CalculateButtonArea(Rectangle cellBounds)
	{
		int width = ThemeEngine.Current.Border3DSize.Width;
		Rectangle rectangle = cellBounds;
		Rectangle result = cellBounds;
		result.X = rectangle.Right - 16 - width;
		result.Y = rectangle.Y + width;
		result.Width = 16;
		result.Height = rectangle.Height - 2 * width;
		return result;
	}
}

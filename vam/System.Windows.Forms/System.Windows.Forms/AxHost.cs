using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Windows.Forms.Design;

namespace System.Windows.Forms;

[Designer("System.Windows.Forms.Design.AxHostDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[DesignTimeVisible(false)]
[DefaultEvent("Enter")]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[System.MonoTODO("Possibly implement this for Win32; find a way for Linux and Mac")]
[ToolboxItem(false)]
[ComVisible(true)]
public abstract class AxHost : Control, ISupportInitialize, ICustomTypeDescriptor
{
	public enum ActiveXInvokeKind
	{
		MethodInvoke,
		PropertyGet,
		PropertySet
	}

	[ComVisible(false)]
	public class AxComponentEditor : WindowsFormsComponentEditor
	{
		public AxComponentEditor()
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}

		public override bool EditComponent(ITypeDescriptorContext context, object obj, IWin32Window parent)
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
	}

	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class ClsidAttribute : Attribute
	{
		private string clsid;

		public string Value => clsid;

		public ClsidAttribute(string clsid)
		{
			this.clsid = clsid;
		}
	}

	public class ConnectionPointCookie
	{
		public ConnectionPointCookie(object source, object sink, Type eventInterface)
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}

		public void Disconnect()
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}

		~ConnectionPointCookie()
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
	}

	public class InvalidActiveXStateException : Exception
	{
		public InvalidActiveXStateException()
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}

		public InvalidActiveXStateException(string name, ActiveXInvokeKind kind)
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}

		public override string ToString()
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
	}

	[Serializable]
	[TypeConverter("System.ComponentModel.TypeConverter, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
	public class State : ISerializable
	{
		public State(Stream ms, int storageType, bool manualUpdate, string licKey)
		{
		}

		protected State(SerializationInfo info, StreamingContext context)
		{
		}

		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
		{
		}
	}

	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class TypeLibraryTimeStampAttribute : Attribute
	{
		public DateTime Value
		{
			get
			{
				throw new NotImplementedException("COM/ActiveX support is not implemented");
			}
		}

		public TypeLibraryTimeStampAttribute(string timestamp)
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
	}

	public class StateConverter : TypeConverter
	{
		public StateConverter()
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
	}

	protected delegate void AboutBoxDelegate();

	private static object MouseClickEvent;

	private static object MouseDoubleClickEvent;

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public override Color BackColor
	{
		get
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
		set
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override Image BackgroundImage
	{
		get
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
		set
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public override ImageLayout BackgroundImageLayout
	{
		get
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
		set
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public ContainerControl ContainingControl
	{
		get
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
		set
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override ContextMenu ContextMenu
	{
		get
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
		set
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public override Cursor Cursor
	{
		get
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
		set
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool EditMode
	{
		get
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new virtual bool Enabled
	{
		get
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
		set
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public override Font Font
	{
		get
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
		set
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public override Color ForeColor
	{
		get
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
		set
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public bool HasAboutBox
	{
		get
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new ImeMode ImeMode
	{
		get
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
		set
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[DefaultValue(null)]
	[RefreshProperties(RefreshProperties.All)]
	public State OcxState
	{
		get
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
		set
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
	}

	[Localizable(true)]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new virtual bool RightToLeft
	{
		get
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
		set
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
	}

	public override ISite Site
	{
		set
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override string Text
	{
		get
		{
			return base.Text;
		}
		set
		{
			base.Text = value;
		}
	}

	protected override CreateParams CreateParams
	{
		get
		{
			throw new NotImplementedException("COM/ActiveX support is not implemented");
		}
	}

	protected override Size DefaultSize => new Size(75, 23);

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler BackColorChanged
	{
		add
		{
			base.BackColorChanged += value;
		}
		remove
		{
			base.BackColorChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler BackgroundImageChanged
	{
		add
		{
			base.BackgroundImageChanged += value;
		}
		remove
		{
			base.BackgroundImageChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler BindingContextChanged
	{
		add
		{
			base.BindingContextChanged += value;
		}
		remove
		{
			base.BindingContextChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event UICuesEventHandler ChangeUICues
	{
		add
		{
			base.ChangeUICues += value;
		}
		remove
		{
			base.ChangeUICues -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler Click
	{
		add
		{
			base.Click += value;
		}
		remove
		{
			base.Click -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler ContextMenuChanged
	{
		add
		{
			base.ContextMenuChanged += value;
		}
		remove
		{
			base.ContextMenuChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler CursorChanged
	{
		add
		{
			base.CursorChanged += value;
		}
		remove
		{
			base.CursorChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler DoubleClick
	{
		add
		{
			base.DoubleClick += value;
		}
		remove
		{
			base.DoubleClick -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event DragEventHandler DragDrop
	{
		add
		{
			base.DragDrop += value;
		}
		remove
		{
			base.DragDrop -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event DragEventHandler DragEnter
	{
		add
		{
			base.DragEnter += value;
		}
		remove
		{
			base.DragEnter -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler DragLeave
	{
		add
		{
			base.DragLeave += value;
		}
		remove
		{
			base.DragLeave -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event DragEventHandler DragOver
	{
		add
		{
			base.DragOver += value;
		}
		remove
		{
			base.DragOver -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler EnabledChanged
	{
		add
		{
			base.EnabledChanged += value;
		}
		remove
		{
			base.EnabledChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler FontChanged
	{
		add
		{
			base.FontChanged += value;
		}
		remove
		{
			base.FontChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler ForeColorChanged
	{
		add
		{
			base.ForeColorChanged += value;
		}
		remove
		{
			base.ForeColorChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event GiveFeedbackEventHandler GiveFeedback
	{
		add
		{
			base.GiveFeedback += value;
		}
		remove
		{
			base.GiveFeedback -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event HelpEventHandler HelpRequested
	{
		add
		{
			base.HelpRequested += value;
		}
		remove
		{
			base.HelpRequested -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler ImeModeChanged
	{
		add
		{
			base.ImeModeChanged += value;
		}
		remove
		{
			base.ImeModeChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event KeyEventHandler KeyDown
	{
		add
		{
			base.KeyDown += value;
		}
		remove
		{
			base.KeyDown -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event KeyPressEventHandler KeyPress
	{
		add
		{
			base.KeyPress += value;
		}
		remove
		{
			base.KeyPress -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event KeyEventHandler KeyUp
	{
		add
		{
			base.KeyUp += value;
		}
		remove
		{
			base.KeyUp -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event LayoutEventHandler Layout
	{
		add
		{
			base.Layout += value;
		}
		remove
		{
			base.Layout -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event MouseEventHandler MouseDown
	{
		add
		{
			base.MouseDown += value;
		}
		remove
		{
			base.MouseDown -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler MouseEnter
	{
		add
		{
			base.MouseEnter += value;
		}
		remove
		{
			base.MouseEnter -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler MouseHover
	{
		add
		{
			base.MouseHover += value;
		}
		remove
		{
			base.MouseHover -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler MouseLeave
	{
		add
		{
			base.MouseLeave += value;
		}
		remove
		{
			base.MouseLeave -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event MouseEventHandler MouseMove
	{
		add
		{
			base.MouseMove += value;
		}
		remove
		{
			base.MouseMove -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event MouseEventHandler MouseUp
	{
		add
		{
			base.MouseUp += value;
		}
		remove
		{
			base.MouseUp -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event MouseEventHandler MouseWheel
	{
		add
		{
			base.MouseWheel += value;
		}
		remove
		{
			base.MouseWheel -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event PaintEventHandler Paint
	{
		add
		{
			base.Paint += value;
		}
		remove
		{
			base.Paint -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event QueryAccessibilityHelpEventHandler QueryAccessibilityHelp
	{
		add
		{
			base.QueryAccessibilityHelp += value;
		}
		remove
		{
			base.QueryAccessibilityHelp -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event QueryContinueDragEventHandler QueryContinueDrag
	{
		add
		{
			base.QueryContinueDrag += value;
		}
		remove
		{
			base.QueryContinueDrag -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler RightToLeftChanged
	{
		add
		{
			base.RightToLeftChanged += value;
		}
		remove
		{
			base.RightToLeftChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler StyleChanged
	{
		add
		{
			base.StyleChanged += value;
		}
		remove
		{
			base.StyleChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler BackgroundImageLayoutChanged
	{
		add
		{
			base.BackgroundImageLayoutChanged += value;
		}
		remove
		{
			base.BackgroundImageLayoutChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler MouseClick
	{
		add
		{
			base.Events.AddHandler(MouseClickEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(MouseClickEvent, value);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler MouseDoubleClick
	{
		add
		{
			base.Events.AddHandler(MouseDoubleClickEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(MouseDoubleClickEvent, value);
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler TextChanged
	{
		add
		{
			base.TextChanged += value;
		}
		remove
		{
			base.TextChanged -= value;
		}
	}

	protected AxHost(string clsid)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	protected AxHost(string clsid, int flags)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	static AxHost()
	{
		MouseClick = new object();
		MouseDoubleClick = new object();
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	AttributeCollection ICustomTypeDescriptor.GetAttributes()
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	string ICustomTypeDescriptor.GetClassName()
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	string ICustomTypeDescriptor.GetComponentName()
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	TypeConverter ICustomTypeDescriptor.GetConverter()
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[CLSCompliant(false)]
	protected static Color GetColorFromOleColor(uint color)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected static Font GetFontFromIFont(object font)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected static Font GetFontFromIFontDisp(object font)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected static object GetIFontDispFromFont(Font font)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected static object GetIFontFromFont(Font font)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected static object GetIPictureDispFromPicture(Image image)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected static object GetIPictureFromCursor(Cursor cursor)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected static object GetIPictureFromPicture(Image image)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected static double GetOADateFromTime(DateTime time)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[CLSCompliant(false)]
	protected static uint GetOleColorFromColor(Color color)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected static Image GetPictureFromIPicture(object picture)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected static Image GetPictureFromIPictureDisp(object picture)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected static DateTime GetTimeFromOADate(double date)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public void BeginInit()
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	public void DoVerb(int verb)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public void EndInit()
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public object GetOcx()
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	public bool HasPropertyPages()
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public void InvokeEditMode()
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public void MakeDirty()
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	public override bool PreProcessMessage(ref Message msg)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	public void ShowAboutBox()
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	public void ShowPropertyPages()
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	public void ShowPropertyPages(Control control)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	protected virtual void AttachInterfaces()
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	protected override void CreateHandle()
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	protected virtual object CreateInstanceCore(Guid clsid)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected virtual void CreateSink()
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	protected override void DestroyHandle()
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected virtual void DetachSink()
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	protected override void Dispose(bool disposing)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public new void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected new virtual Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	protected override bool IsInputChar(char charCode)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	protected override void OnBackColorChanged(EventArgs e)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	protected override void OnFontChanged(EventArgs e)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	protected override void OnForeColorChanged(EventArgs e)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	protected virtual void OnInPlaceActive()
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override void OnLostFocus(EventArgs e)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	protected override bool ProcessDialogKey(Keys keyData)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	protected override bool ProcessMnemonic(char charCode)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected bool PropsValid()
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected void RaiseOnMouseDown(short button, short shift, int x, int y)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected void RaiseOnMouseDown(short button, short shift, float x, float y)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected void RaiseOnMouseDown(object o1, object o2, object o3, object o4)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected void RaiseOnMouseMove(short button, short shift, int x, int y)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected void RaiseOnMouseMove(short button, short shift, float x, float y)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected void RaiseOnMouseMove(object o1, object o2, object o3, object o4)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected void RaiseOnMouseUp(short button, short shift, int x, int y)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected void RaiseOnMouseUp(short button, short shift, float x, float y)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected void RaiseOnMouseUp(object o1, object o2, object o3, object o4)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	protected void SetAboutBoxDelegate(AboutBoxDelegate d)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	protected override void SetVisibleCore(bool value)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}

	protected override void WndProc(ref Message m)
	{
		throw new NotImplementedException("COM/ActiveX support is not implemented");
	}
}

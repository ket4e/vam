using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Design;

public abstract class PropertyTab : IExtenderProvider
{
	private Bitmap bitmap;

	private object[] components;

	public virtual Bitmap Bitmap
	{
		get
		{
			if (bitmap == null)
			{
				Type type = GetType();
				bitmap = new Bitmap(type, type.Name + ".bmp");
			}
			return bitmap;
		}
	}

	public virtual object[] Components
	{
		get
		{
			return components;
		}
		set
		{
			components = value;
		}
	}

	public virtual string HelpKeyword => TabName;

	public abstract string TabName { get; }

	~PropertyTab()
	{
		Dispose(disposing: false);
	}

	public virtual bool CanExtend(object extendee)
	{
		return true;
	}

	public virtual void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing && bitmap != null)
		{
			bitmap.Dispose();
			bitmap = null;
		}
	}

	public virtual PropertyDescriptor GetDefaultProperty(object component)
	{
		return TypeDescriptor.GetDefaultProperty(component);
	}

	public virtual PropertyDescriptorCollection GetProperties(object component)
	{
		return GetProperties(component, null);
	}

	public abstract PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes);

	public virtual PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object component, Attribute[] attributes)
	{
		return GetProperties(component, attributes);
	}
}

using System.ComponentModel;

namespace System.Windows.Forms;

public interface IBindableComponent : IDisposable, IComponent
{
	BindingContext BindingContext { get; set; }

	ControlBindingsCollection DataBindings { get; }
}

using System.Collections;

namespace Mono.Xml;

internal class DTDContentModelCollection
{
	private ArrayList contentModel = new ArrayList();

	public IList Items => contentModel;

	public DTDContentModel this[int i] => contentModel[i] as DTDContentModel;

	public int Count => contentModel.Count;

	public void Add(DTDContentModel model)
	{
		contentModel.Add(model);
	}
}

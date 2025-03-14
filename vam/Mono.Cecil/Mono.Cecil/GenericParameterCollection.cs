using Mono.Collections.Generic;

namespace Mono.Cecil;

internal sealed class GenericParameterCollection : Collection<GenericParameter>
{
	private readonly IGenericParameterProvider owner;

	internal GenericParameterCollection(IGenericParameterProvider owner)
	{
		this.owner = owner;
	}

	internal GenericParameterCollection(IGenericParameterProvider owner, int capacity)
		: base(capacity)
	{
		this.owner = owner;
	}

	protected override void OnAdd(GenericParameter item, int index)
	{
		UpdateGenericParameter(item, index);
	}

	protected override void OnInsert(GenericParameter item, int index)
	{
		UpdateGenericParameter(item, index);
		for (int i = index; i < size; i++)
		{
			items[i].position = i + 1;
		}
	}

	protected override void OnSet(GenericParameter item, int index)
	{
		UpdateGenericParameter(item, index);
	}

	private void UpdateGenericParameter(GenericParameter item, int index)
	{
		item.owner = owner;
		item.position = index;
		item.type = owner.GenericParameterType;
	}

	protected override void OnRemove(GenericParameter item, int index)
	{
		item.owner = null;
		item.position = -1;
		item.type = GenericParameterType.Type;
		for (int i = index + 1; i < size; i++)
		{
			items[i].position = i - 1;
		}
	}
}

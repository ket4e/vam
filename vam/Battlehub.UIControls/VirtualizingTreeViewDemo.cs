using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Battlehub.UIControls;

public class VirtualizingTreeViewDemo : MonoBehaviour
{
	public VirtualizingTreeView TreeView;

	private GameObject[] m_dataItems;

	private int m_counter = 1;

	public static bool IsPrefab(Transform This)
	{
		if (Application.isEditor && !Application.isPlaying)
		{
			throw new InvalidOperationException("Does not work in edit mode");
		}
		return This.gameObject.scene.buildIndex < 0;
	}

	private void Start()
	{
		if (!TreeView)
		{
			Debug.LogError("Set TreeView field");
			return;
		}
		TreeView.ItemDataBinding += OnItemDataBinding;
		TreeView.SelectionChanged += OnSelectionChanged;
		TreeView.ItemsRemoved += OnItemsRemoved;
		TreeView.ItemExpanding += OnItemExpanding;
		TreeView.ItemBeginDrag += OnItemBeginDrag;
		TreeView.ItemDrop += OnItemDrop;
		TreeView.ItemBeginDrop += OnItemBeginDrop;
		TreeView.ItemEndDrag += OnItemEndDrag;
		TreeView.Items = (from go in Resources.FindObjectsOfTypeAll<GameObject>()
			where !IsPrefab(go.transform) && go.transform.parent == null && go.name != "VirtualizingTreeViewDemo"
			select go into t
			orderby t.transform.GetSiblingIndex()
			select t).ToArray();
	}

	private void OnItemBeginDrop(object sender, ItemDropCancelArgs e)
	{
	}

	private void OnDestroy()
	{
		if ((bool)TreeView)
		{
			TreeView.ItemDataBinding -= OnItemDataBinding;
			TreeView.SelectionChanged -= OnSelectionChanged;
			TreeView.ItemsRemoved -= OnItemsRemoved;
			TreeView.ItemExpanding -= OnItemExpanding;
			TreeView.ItemBeginDrag -= OnItemBeginDrag;
			TreeView.ItemBeginDrop -= OnItemBeginDrop;
			TreeView.ItemDrop -= OnItemDrop;
			TreeView.ItemEndDrag -= OnItemEndDrag;
		}
	}

	private void OnItemExpanding(object sender, ItemExpandingArgs e)
	{
		GameObject gameObject = (GameObject)e.Item;
		if (gameObject.transform.childCount > 0)
		{
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < gameObject.transform.childCount; i++)
			{
				GameObject item = gameObject.transform.GetChild(i).gameObject;
				list.Add(item);
			}
			e.Children = list;
		}
	}

	private void OnSelectionChanged(object sender, SelectionChangedArgs e)
	{
	}

	private void OnItemsRemoved(object sender, ItemsRemovedArgs e)
	{
		for (int i = 0; i < e.Items.Length; i++)
		{
			GameObject gameObject = (GameObject)e.Items[i];
			if (gameObject != null)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}
	}

	private void OnItemDataBinding(object sender, TreeViewItemDataBindingArgs e)
	{
		GameObject gameObject = e.Item as GameObject;
		if (gameObject != null)
		{
			Text componentInChildren = e.ItemPresenter.GetComponentInChildren<Text>(includeInactive: true);
			componentInChildren.text = gameObject.name;
			Image image = e.ItemPresenter.GetComponentsInChildren<Image>()[4];
			image.sprite = Resources.Load<Sprite>("cube");
			e.HasChildren = gameObject.transform.childCount > 0;
		}
	}

	private void OnItemBeginDrag(object sender, ItemArgs e)
	{
	}

	private void OnItemDrop(object sender, ItemDropArgs e)
	{
		if (e.DropTarget == null)
		{
			return;
		}
		Transform transform = ((GameObject)e.DropTarget).transform;
		if (e.Action == ItemDropAction.SetLastChild)
		{
			for (int i = 0; i < e.DragItems.Length; i++)
			{
				Transform transform2 = ((GameObject)e.DragItems[i]).transform;
				transform2.SetParent(transform, worldPositionStays: true);
				transform2.SetAsLastSibling();
			}
		}
		else if (e.Action == ItemDropAction.SetNextSibling)
		{
			for (int num = e.DragItems.Length - 1; num >= 0; num--)
			{
				Transform transform3 = ((GameObject)e.DragItems[num]).transform;
				int siblingIndex = transform.GetSiblingIndex();
				if (transform3.parent != transform.parent)
				{
					transform3.SetParent(transform.parent, worldPositionStays: true);
					transform3.SetSiblingIndex(siblingIndex + 1);
				}
				else
				{
					int siblingIndex2 = transform3.GetSiblingIndex();
					if (siblingIndex < siblingIndex2)
					{
						transform3.SetSiblingIndex(siblingIndex + 1);
					}
					else
					{
						transform3.SetSiblingIndex(siblingIndex);
					}
				}
			}
		}
		else
		{
			if (e.Action != ItemDropAction.SetPrevSibling)
			{
				return;
			}
			for (int j = 0; j < e.DragItems.Length; j++)
			{
				Transform transform4 = ((GameObject)e.DragItems[j]).transform;
				if (transform4.parent != transform.parent)
				{
					transform4.SetParent(transform.parent, worldPositionStays: true);
				}
				int siblingIndex3 = transform.GetSiblingIndex();
				int siblingIndex4 = transform4.GetSiblingIndex();
				if (siblingIndex3 > siblingIndex4)
				{
					transform4.SetSiblingIndex(siblingIndex3 - 1);
				}
				else
				{
					transform4.SetSiblingIndex(siblingIndex3);
				}
			}
		}
	}

	private void OnItemEndDrag(object sender, ItemArgs e)
	{
	}

	public void AddItem()
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "GameObject " + m_counter;
		m_counter++;
		TreeView.Add(gameObject);
	}
}

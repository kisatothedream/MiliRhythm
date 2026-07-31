using System;
using System.Collections.Generic;
using UnityEngine;

namespace MilliRhythm.UI
{
	public class UIListBase<TItemListModel, TItemListView, TId> : MonoBehaviour
		where TItemListModel : UIListItemModelBase<TId>
		where TItemListView : UIListItemViewBase<TItemListModel, TId>
		where TId : IEquatable<TId>
	{
		[SerializeField] protected Transform content;

		public List<TItemListView> listItems = new();
		public TItemListView listItemPrefab;

		public virtual void Set(List<TItemListModel> models)
		{
			Clear();
			foreach (var model in models)
			{
				var item = Instantiate(listItemPrefab, content);
				listItems.Add(item);
				item.Set(model);
			}
		}

		protected virtual void Clear()
		{
			for (var i = content.childCount - 1; i >= 0; i--)
			{
				Destroy(content.GetChild(i).gameObject);
			}

			listItems.Clear();
		}
	}
}

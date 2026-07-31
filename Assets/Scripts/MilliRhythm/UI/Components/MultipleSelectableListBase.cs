using System;
using System.Collections.Generic;

namespace MilliRhythm.UI.Components
{
	public class MultipleSelectableListBase<TModel, TView, TKey> : UIListBase<TModel, TView, TKey>
		where TModel : SelectableListModelBase<TKey>
		where TView : SelectableListViewBase<TModel, TKey>
		where TKey : IEquatable<TKey>
	{
		private HashSet<SelectableListViewBase<TModel, TKey>> selectedItems = new();

		public override void Set(List<TModel> models)
		{
			Clear();
			foreach (var model in models)
			{
				var item = Instantiate(listItemPrefab, content);
				listItems.Add(item);
				item.Set(model);
				item.RegisterOnSelectAction(OnClickViewItem);
			}
		}

		protected override void Clear()
		{
			base.Clear();
			ClearItems();
		}

		public void AddItem(SelectableListViewBase<TModel, TKey> item)
		{
			selectedItems.Add(item);
		}

		public void RemoveItem(SelectableListViewBase<TModel, TKey> item)
		{
			selectedItems.Remove(item);
		}

		public void ClearItems()
		{
			foreach (var item in selectedItems)
			{
			}

			selectedItems.Clear();
		}

		public HashSet<SelectableListViewBase<TModel, TKey>> GetSelectedItems()
		{
			return selectedItems;
		}

		private void OnClickViewItem(SelectableListViewBase<TModel, TKey> item)
		{
			if (!item.IsSelected)
			{
				item.Select();
				AddItem(item);
			}
			else
			{
				item.Deselect();
				RemoveItem(item);
			}
		}
	}
}
